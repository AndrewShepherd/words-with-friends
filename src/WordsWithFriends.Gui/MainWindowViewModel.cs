using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends.Gui
{
	public enum BoardType { Small, Large }

	public class MainWindowViewModel : INotifyPropertyChanged
	{

		private BoardType _boardType = BoardType.Small;

		private readonly BoardGenerationObservable _boardGenerationObservable = new();

		public BoardType BoardType
		{
			get => _boardType;
			set
			{
				if(_boardType != value)
				{
					_boardType = value;
					PropertyChanged?.Invoke(this, new(nameof(BoardType)));
					_boardGenerationObservable.OnNext(new(_boardType, _script));
				}
			}
		}

		public MainWindowViewModel()
		{
			_boardGenerationObservable.Subscribe(
				board => this.Board = board
			);

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
				this._boardGenerationObservable.OnNext(
					new(_boardType, _script)
				);
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
