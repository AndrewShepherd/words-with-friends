namespace WordsWithFriends.Gui
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using System.Reactive.Linq;
	using System.Reactive.Subjects;
	using System.Text;
	using ReactiveUI;

	public enum BoardType { Small, Large }

	public record SuggestionGenerationFields(Board Board, string AvailableCharacters);
	record BoardGenerationFields(BoardType BoardType, string Script);

	public class MainWindowViewModel : ReactiveObject
	{

		private readonly ReplaySubject<string> _availableTilesReplaySubject = new();

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

			boardObservable
				.ToProperty(this, vm => vm.Board, out _board);

			boardObservable.CombineLatest(
				_availableTilesReplaySubject,
				(board, availableTiles) => new SuggestionGenerationFields(Board, availableTiles)
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
				if(_availableTiles != value)
				{
					this.RaiseAndSetIfChanged(ref _availableTiles, value);
					this._availableTilesReplaySubject.OnNext(
						this._availableTiles
					);
				}
			}
		}

		private Lazy<MoveFinder> _moveFinder = new Lazy<MoveFinder>();

		private ObservableAsPropertyHelper<string> _suggestions;
		public string Suggestions
		{
			get => _suggestions.Value;
		}

		static string MoveToScriptString(Move s) =>
			$"{s.Score}: add {s.Position.Row} {s.Position.Column} {(s.Direction == Direction.Across ? 'a' : 'd')} {s.Word}";
	}
}
