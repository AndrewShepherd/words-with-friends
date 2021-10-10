namespace WordsWithFriends.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reactive.Linq;
	using System.Windows;
	using ReactiveUI;

	public enum BoardType { Small, Large }

	public record SuggestionGenerationFields(Board Board, string AvailableCharacters);
	record BoardGenerationFields(BoardType BoardType, string Script);

	public class SuggestionQueryResult
	{
		public bool Succeeded { get; set; }
		public string Description { get; set; } = String.Empty;

		public IEnumerable<Move> Moves { get; set; } = Enumerable.Empty<Move>();
	}

	public class SuggestedMoveEntry
	{
		public SuggestedMoveEntry(Move move)
		{
			this.Move = move;
		}

		public int Score { get; set; }
		public string Script { get; set; } = String.Empty;

		public Move Move { get; init; }
	}

	// Putting the standalone static methods into a separate class
	// Supporting the functional approach
	internal static class MainWindowViewModelStatics
	{
		internal static ExecuteScriptResult ExecuteScript(BoardGenerationFields record) =>
			ScriptExecutor.Run(
				record.BoardType == BoardType.Small
					? BoardBuilder.ConstructSmallBoard
					: BoardBuilder.ConstructLargeBoard,
				record.Script
			);

		public static string MoveToScriptString(Move s) =>
			$"add {s.Position.Row} {s.Position.Column} {(s.Direction == Direction.Across ? 'a' : 'd')} {s.Word}";

		public static SuggestedMoveEntry MoveToSuggestedMoveEntry(Move s) =>
			new SuggestedMoveEntry(s)
			{
				Score = s.Score,
				Script = MoveToScriptString(s)
			};


		internal static SuggestionQueryResult GenerateSuggestions(MoveFinder moveFinder, SuggestionGenerationFields record)
		{
			try
			{
				return new()
				{
					Succeeded = true,
					Moves = moveFinder.ListAll(
						record.Board,
						record.AvailableCharacters
					).OrderByDescending(s => s.Score)
					.Take(40)
					.ToList(),
					Description = "Successfully generated moves"
				};
			}
			catch (Exception ex)
			{
				return new()
				{
					Succeeded = false,
					Moves = Enumerable.Empty<Move>(),
					Description = ex.ToString()
				};
			}
		}

	}



	public class SuggestionsViewModel : ReactiveObject
	{
		private SuggestionQueryResult _suggestionQueryResult = new();
		public SuggestionQueryResult SuggestionQueryResult
		{
			get => _suggestionQueryResult;
			set => this.RaiseAndSetIfChanged(ref _suggestionQueryResult, value);
		}

		private readonly ObservableAsPropertyHelper<Visibility> _suggestionErrorVisibility;
		public Visibility SuggestionErrorVisibility => _suggestionErrorVisibility.Value;

		private readonly ObservableAsPropertyHelper<Visibility> _suggestionListVisibility;
		public Visibility SuggestionListVisibility => _suggestionListVisibility.Value;

		private ObservableAsPropertyHelper<IEnumerable<SuggestedMoveEntry>> _suggestions;
		public IEnumerable<SuggestedMoveEntry> Suggestions => _suggestions.Value;

		private SuggestedMoveEntry? _selectedEntry;
		public SuggestedMoveEntry? SelectedEntry
		{
			get => _selectedEntry;
			set => this.RaiseAndSetIfChanged(ref _selectedEntry, value);
		}

		public SuggestionsViewModel()
		{
			var suggestionQueryResultObservable = this.WhenAnyValue(vm => vm.SuggestionQueryResult);

			this._suggestionErrorVisibility = suggestionQueryResultObservable
				.Select(s => s.Succeeded ? Visibility.Collapsed : Visibility.Visible)
				.ToProperty(this, vm => vm.SuggestionErrorVisibility, out _suggestionErrorVisibility);

			this._suggestionListVisibility = suggestionQueryResultObservable
				.Select(s => s.Succeeded ? Visibility.Visible : Visibility.Collapsed)
				.ToProperty(this, vm => vm.SuggestionListVisibility, out _suggestionListVisibility);

			this._suggestions = suggestionQueryResultObservable
				.Select(sqr => sqr.Moves.Select(MainWindowViewModelStatics.MoveToSuggestedMoveEntry).ToList())
				.ToProperty(this, vm => vm.Suggestions, out _suggestions);
		}	
	}

	public class MainWindowViewModel : ReactiveObject
	{
		private BoardType _boardType = BoardType.Small;
		public BoardType BoardType
		{
			get => _boardType;
			set => this.RaiseAndSetIfChanged(ref _boardType, value);
		}

		private SuggestionQueryResult GenerateSuggestions(SuggestionGenerationFields record) =>
			MainWindowViewModelStatics.GenerateSuggestions(
				this._moveFinder.Value,
				record
			);

		public MainWindowViewModel()
		{
			var boardObservable = this.WhenAnyValue(
				vm => vm.BoardType,
				vm => vm.Script,
				(boardType,script) => new BoardGenerationFields(boardType, script)
			).TransformOnBackground(
				MainWindowViewModelStatics.ExecuteScript
			).Select(result => result.Board);

			this._board = boardObservable
				.ToProperty(
					this,
					vm => vm.Board,
					out _board,
					this.BoardType == BoardType.Large
						? BoardBuilder.ConstructLargeBoard
						: BoardBuilder.ConstructSmallBoard
				);

			var suggestionQueryResultObservable = this.WhenAnyValue(
				vm => vm.Board,
				vm => vm.AvailableTiles,
				(board, tiles) => new SuggestionGenerationFields(board, tiles)
			).TransformOnBackground(
				GenerateSuggestions
			);
			suggestionQueryResultObservable.Subscribe(
				result => this.SuggestionsViewModel.SuggestionQueryResult = result
			);
		}

		private readonly ObservableAsPropertyHelper<Board> _board;

		public Board Board => _board.Value;

		private string _script = String.Empty;
		public string Script
		{
			get => _script;
			set => this.RaiseAndSetIfChanged(ref _script, value);
		}

		private string _availableTiles = string.Empty;
		public string AvailableTiles
		{
			get => _availableTiles;
			set => this.RaiseAndSetIfChanged(ref _availableTiles, value);
		}

		private Lazy<MoveFinder> _moveFinder = new Lazy<MoveFinder>();

		public SuggestionsViewModel SuggestionsViewModel { get; set; } = new();
	}
}
