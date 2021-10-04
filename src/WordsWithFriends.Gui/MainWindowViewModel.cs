namespace WordsWithFriends.Gui
{
	using System;
	using System.Linq;
	using System.Reactive.Linq;
	using System.Text;
	using ReactiveUI;

	public enum BoardType { Small, Large }

	public record SuggestionGenerationFields(Board Board, string AvailableCharacters);
	record BoardGenerationFields(BoardType BoardType, string Script);

	public class MainWindowViewModel : ReactiveObject
	{
		private BoardType _boardType = BoardType.Small;
		public BoardType BoardType
		{
			get => _boardType;
			set
			{
				this.RaiseAndSetIfChanged(ref _boardType, value);
			}
		}

		private static ExecuteScriptResult ExecuteScript(BoardGenerationFields record) =>
			ScriptExecutor.Run(
				record.BoardType == BoardType.Small
					? BoardBuilder.ConstructSmallBoard
					: BoardBuilder.ConstructLargeBoard,
				record.Script
			);

		private string GenerateSuggestions(SuggestionGenerationFields record)
		{
			try
			{
				var suggestions = this._moveFinder.Value.ListAll(
					record.Board,
					record.AvailableCharacters
				).OrderByDescending(s => s.Score)
				.ToList();
				StringBuilder sb = new StringBuilder();
				foreach (var s in suggestions)
				{
					sb.AppendLine(MoveToScriptString(s));
				}
				return sb.ToString();
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}

		public MainWindowViewModel()
		{
			var boardObservable = this.WhenAnyValue(
				vm => vm.BoardType,
				vm => vm.Script,
				(boardType,script) => new BoardGenerationFields(boardType, script)
			).TransformOnBackground(
				ExecuteScript
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

			this._suggestions = this.WhenAnyValue(
				vm => vm.Board,
				vm => vm.AvailableTiles,
				(board, tiles) => new SuggestionGenerationFields(board, tiles)
			).TransformOnBackground(
				GenerateSuggestions
			).ToProperty(this, vm => vm.Suggestions, out _suggestions);
		}

		private readonly ObservableAsPropertyHelper<Board> _board;

		public Board Board => _board.Value;

		private string _script = String.Empty;
		public string Script
		{
			get => _script;
			set
			{
				this.RaiseAndSetIfChanged(ref _script, value);
			}
		}

		private string _availableTiles = string.Empty;
		public string AvailableTiles
		{
			get => _availableTiles;
			set
			{
				this.RaiseAndSetIfChanged(ref _availableTiles, value);
			}
		}

		private Lazy<MoveFinder> _moveFinder = new Lazy<MoveFinder>();

		private ObservableAsPropertyHelper<string> _suggestions;
		public string Suggestions => _suggestions.Value;

		static string MoveToScriptString(Move s) =>
			$"{s.Score}: add {s.Position.Row} {s.Position.Column} {(s.Direction == Direction.Across ? 'a' : 'd')} {s.Word}";
	}
}
