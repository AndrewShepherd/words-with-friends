using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends.Gui
{
	public enum BoardType { Small, Large }

	public class MainWindowViewModel : INotifyPropertyChanged
	{
		enum ScriptRunState { Idle, Running, RunningButStale };
		private ScriptRunState _currentScriptRunState = ScriptRunState.Idle;

		record BoardGenerationFields(BoardType BoardType, string Script);
		private BoardGenerationFields _pendingBoardGeneration = new(BoardType.Small, string.Empty);


		private BoardType _boardType = BoardType.Small;
		public BoardType BoardType
		{
			get => _boardType;
			set
			{
				if(_boardType != value)
				{
					_boardType = value;
					PropertyChanged?.Invoke(this, new(nameof(BoardType)));
					LaunchScriptUpdate(new(_boardType, _script));
				}
			}
		}

		async void LaunchScriptUpdate(BoardGenerationFields boardGenerationFields)
		{
			var result = await Task.Run(
				() => ScriptExecutor.Run(
					() => boardGenerationFields.BoardType == BoardType.Large 
						? BoardBuilder.ConstructLargeBoard()
						: BoardBuilder.ConstructSmallBoard(),
					boardGenerationFields.Script
				)
			);
			this.Board = result.Board;
			this.RegenerateSuggestions();
			switch(_currentScriptRunState)
			{
				case ScriptRunState.Running:
					_currentScriptRunState = ScriptRunState.Idle;
					break;
				case ScriptRunState.RunningButStale:
					_currentScriptRunState = ScriptRunState.Running;
					LaunchScriptUpdate(_pendingBoardGeneration);
					break;
			}
		}

		void RefreshBoard(BoardType boardType, string script)
		{
			switch (_currentScriptRunState)
			{
				case ScriptRunState.Idle:
					_currentScriptRunState = ScriptRunState.Running;
					LaunchScriptUpdate(new(boardType, script));
					break;
				case ScriptRunState.Running:
					_pendingBoardGeneration = new(boardType, script);
					_currentScriptRunState = ScriptRunState.RunningButStale;
					break;
				case ScriptRunState.RunningButStale:
					_pendingBoardGeneration = new(boardType, script);
					break;
			}
		}

		public MainWindowViewModel()
		{
		
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
				RefreshBoard(_boardType, _script);
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
					RegenerateSuggestions();
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

		void RegenerateSuggestions()
		{
			try
			{
				var suggestions = _moveFinder.Value.ListAll(
					this._board,
					this._availableTiles
				).OrderByDescending(s => s.Score)
				.ToList();
				StringBuilder sb = new StringBuilder();
				foreach (var s in suggestions)
				{
					sb.AppendLine(MoveToScriptString(s));
				}
				this.Suggestions = sb.ToString();
			}
			catch(Exception ex)
			{
				this.Suggestions = ex.ToString();
			}
		}
	}
}
