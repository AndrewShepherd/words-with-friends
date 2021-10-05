namespace WordsWithFriends.Gui
{
	using System;
	using System.Linq;
	using ReactiveUI;
	using System.Reactive.Linq;
	using System.Windows.Media;
	using System.Collections.Generic;

	class BoardCell
	{
		public char Char { get; set; }
		public Color BackgroundColor { get; set; }
	}

	class BoardViewModel : ReactiveObject
	{
		public BoardViewModel()
		{
			this._boardCells = this.WhenAnyValue(vm => vm.Board)
				.Select(board => BoardToBoardCells(board))
				.ToProperty(this, vm => vm.BoardCells);
		}

		private Board? _board;
		public Board? Board
		{
			get => _board;
			set
			{
				this.RaiseAndSetIfChanged(ref _board, value);
			}
		}

		private readonly ObservableAsPropertyHelper<IEnumerable<BoardCell>> _boardCells;
		public IEnumerable<BoardCell> BoardCells => _boardCells.Value;

		private static IEnumerable<BoardCell> BoardToBoardCells(Board? board)
		{
			if (board == null)
			{
				return Enumerable.Empty<BoardCell>();
			}
			List<BoardCell> rv = new();
			for (int row = 0; row < board.Dimensions.Rows; ++row)
			{
				for (int col = 0; col < board.Dimensions.Columns; ++col)
				{
					PlacedTile? tile = board.TileAt(new(row, col));
					if (tile != null)
					{
						rv.Add(
							new()
							{
								Char = Char.ToUpper(tile.EffectiveChar),
								BackgroundColor = Colors.Ivory
							}
						); ;
					}
					else
					{
						var squareBonus = board.SquareBonusAt(new(row, col));
						rv.Add(
							new()
							{
								Char = ' ',
								BackgroundColor = SquareBonusToColor(squareBonus)
							}
						);
					}
				}
			}
			return rv;
		}

		private static Color SquareBonusToColor(SquareBonus squareBonus) =>
			squareBonus switch
			{
				SquareBonus.TripleWord => Colors.Orange,
				SquareBonus.DoubleWord => Colors.Red,
				SquareBonus.TripleLetter => Colors.LightGreen,
				SquareBonus.DoubleLetter => Colors.SteelBlue,
				_ => Colors.WhiteSmoke
			};
	}
}
