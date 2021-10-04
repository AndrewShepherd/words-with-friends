namespace WordsWithFriends.Gui
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using System.Reactive.Linq;
	using System.Reactive.Subjects;
	using System.Text;

	public enum BoardType { Small, Large }

	public record SuggestionGenerationFields(Board Board, string AvailableCharacters);
	record BoardGenerationFields(BoardType BoardType, string Script);

	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private BoardType _boardType = BoardType.Small;

		private readonly ReplaySubject<string> _scriptReplaySubject = new();
		private readonly ReplaySubject<BoardType> _boardTypeReplaySubject = new();
		private readonly ReplaySubject<string> _availableTilesReplaySubject = new();

		public BoardType BoardType
		{
			get => _boardType;
			set
			{
				if(_boardType != value)
				{
					_boardType = value;
					PropertyChanged?.Invoke(this, new(nameof(BoardType)));
					_boardTypeReplaySubject.OnNext(_boardType);
				}
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
			var boardObservable = _boardTypeReplaySubject.CombineLatest(
				_scriptReplaySubject,
				(boardType, script) => new BoardGenerationFields(boardType, script)
			).TransformOnBackground(
				ExecuteScript
			);
			
			boardObservable.Subscribe(
				result => this.Board = result.Board
			);
			boardObservable.CombineLatest(
				_availableTilesReplaySubject,
				(board, availableTiles) => new SuggestionGenerationFields(Board, availableTiles)
			).TransformOnBackground(
				GenerateSuggestions
			).Subscribe(
				result => this.Suggestions = result
			);
			_scriptReplaySubject.OnNext(_script);
			_boardTypeReplaySubject.OnNext(_boardType);
		}


		public event PropertyChangedEventHandler? PropertyChanged;

		private string _script = String.Empty;

		private Board _board = BoardBuilder.ConstructLargeBoard();

		public Board Board
		{
			get => _board;
			set
			{
				if (_board != value)
				{
					_board = value;
					this.PropertyChanged?.Invoke(this, new(nameof(Board)));
				}
			}
		}

		public string Script
		{
			get => _script;
			set
			{
				_script = value ?? string.Empty;
				PropertyChanged?.Invoke(
					this,
					new (nameof(Script))
				);
				this._scriptReplaySubject.OnNext(_script);
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
					_availableTiles = value;
					PropertyChanged?.Invoke(
						this,
						new(nameof(Script))
					);
					this._availableTilesReplaySubject.OnNext(
						this._availableTiles
					);
				}
			}
		}

		private Lazy<MoveFinder> _moveFinder = new Lazy<MoveFinder>();

		private string _suggestions = string.Empty;
		public string Suggestions
		{
			get => _suggestions;
			set
			{
				if(_suggestions != value)
				{
					_suggestions = value;
					PropertyChanged?.Invoke(
						this,
						new(nameof(Suggestions))
					);
				}
			}
		}

		static string MoveToScriptString(Move s) =>
			$"{s.Score}: add {s.Position.Row} {s.Position.Column} {(s.Direction == Direction.Across ? 'a' : 'd')} {s.Word}";
	}
}
