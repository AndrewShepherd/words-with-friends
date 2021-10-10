namespace WordsWithFriends.Gui
{
	using System;
	using System.Linq;
	using ReactiveUI;
	using System.Reactive.Linq;
	using System.Windows.Media;
	using System.Collections.Generic;
	using System.Windows;

	class BoardCell
	{
		public char Char { get; set; }
		public Color BackgroundColor { get; set; }

		public Color ForegroundColor { get; set; } = Colors.Black;

		public FontWeight FontWeight { get; set; } = FontWeights.Normal;
	}

	class BoardViewModel : ReactiveObject
	{
		public BoardViewModel()
		{
			this._boardCells = this.WhenAnyValue(
				vm => vm.Board,
				vm => vm.SuggestedMove,
				(board, move) => new { Board = board, Move = move }
			).TransformOnBackground(
				values => BoardToBoardCells(values.Board, values.Move)
			).ToProperty(this, vm => vm.BoardCells);
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

		private Move? _suggestedMove;
		public Move? SuggestedMove
		{
			get => _suggestedMove;
			set => this.RaiseAndSetIfChanged(ref _suggestedMove, value);
		}

		private readonly ObservableAsPropertyHelper<IEnumerable<BoardCell>> _boardCells;
		public IEnumerable<BoardCell> BoardCells => _boardCells.Value;

		private static IEnumerable<BoardCell> BoardToBoardCells(Board? board, Move? suggestedMove)
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
					Position position = new(row, col);
					var tp = suggestedMove?.TilePlacements?.Where(tp => tp.Position == position).FirstOrDefault();
					if(tp != null)
					{
						rv.Add(
							new()
							{
								Char = Char.ToUpper(tp.PlacedTile.EffectiveChar),
								BackgroundColor = Colors.DarkGreen,
								ForegroundColor = Colors.White,
								FontWeight = FontWeights.Bold
							}
						);
						continue;
					}
					PlacedTile? tile = board.TileAt(position);
					if (tile != null)
					{
						rv.Add(
							new()
							{
								Char = Char.ToUpper(tile.EffectiveChar),
								BackgroundColor = Colors.Ivory
							}
						);
						continue;
					}
					var squareBonus = board.SquareBonusAt(position);
					rv.Add(
						new()
						{
							Char = ' ',
							BackgroundColor = SquareBonusToColor(squareBonus)
						}
					);
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
