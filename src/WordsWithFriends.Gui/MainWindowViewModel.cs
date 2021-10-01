using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends.Gui
{


	public class MainWindowViewModel : INotifyPropertyChanged
	{
		enum ScriptRunState { Idle, Running, RunningButStale };
		private ScriptRunState _currentScriptRunState = ScriptRunState.Idle;
		private string _pendingScript = String.Empty;


		private static string BoardToString(Board board)
		{
			StringBuilder sb = new StringBuilder();
			for(int row = 0; row < board.Dimensions.Rows; ++row)
			{
				for(int col = 0; col < board.Dimensions.Columns; ++col)
				{
					var c = board.CharAt(new (row, col));
					sb.Append(c == default ? ' ' : c);
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}

		async void LaunchScriptUpdate(string script)
		{
			var result = await Task.Run(
				() => ScriptExecutor.Run(
					() => BoardBuilder.ConstructLargeBoard(),
					script
				)
			);
			this._board = result.Board;
			this.BoardString = BoardToString(result.Board);
			this.RegenerateSuggestions();
			switch(_currentScriptRunState)
			{
				case ScriptRunState.Running:
					_currentScriptRunState = ScriptRunState.Idle;
					break;
				case ScriptRunState.RunningButStale:
					_currentScriptRunState = ScriptRunState.Running;
					LaunchScriptUpdate(_pendingScript);
					break;
			}
		}

		void RefreshBoard(string script)
		{
			switch (_currentScriptRunState)
			{
				case ScriptRunState.Idle:
					_currentScriptRunState = ScriptRunState.Running;
					LaunchScriptUpdate(script);
					break;
				case ScriptRunState.Running:
					_pendingScript = script;
					_currentScriptRunState = ScriptRunState.RunningButStale;
					break;
				case ScriptRunState.RunningButStale:
					_pendingScript = script;
					break;
			}
		}

		public MainWindowViewModel()
		{
		
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		private string _script = String.Empty;

		private string _boardString;
		private Board _board = BoardBuilder.ConstructLargeBoard();
		public string BoardString
		{
			get => _boardString;
			set
			{
				if(_boardString != value)
				{
					_boardString = value;
					PropertyChanged?.Invoke(this, new(nameof(BoardString)));
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
				RefreshBoard(_script);
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
