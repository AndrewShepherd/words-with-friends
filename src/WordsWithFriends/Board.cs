namespace WordsWithFriends
{
	using System.Collections.Generic;
	using System.Linq;

	public sealed class Board
	{
		private readonly PlacedTile?[,] _placedTiles;
		private readonly SquareBonus[,] _squareBonuses;
		public void Place(WordPlacement wordPlacement)
		{
			Position pos = wordPlacement.Position;
			for (int i = 0; i < wordPlacement.Word.Length; ++i)
			{
				var c = wordPlacement.Word[i];
				_placedTiles[pos.Row, pos.Column] = new PlacedTile(c, c);
				pos = wordPlacement.Direction switch
				{
					Direction.Across => new Position(pos.Row, pos.Column + 1),
					_ => new Position(pos.Row + 1, pos.Column)
				};
			}
		}

		public void Place(TilePlacement tilePlacement)
		{
			this._placedTiles[tilePlacement.Position.Row, tilePlacement.Position.Column] = tilePlacement.PlacedTile;
		}

		internal void SetBonus(Position position, SquareBonus squareBonus)
		{
			this._squareBonuses[position.Row, position.Column] = squareBonus;
		}

		public char CharAt(Position position) =>
			TileAt(position)?.EffectiveChar ?? default;


		public PlacedTile? TileAt(Position position) =>
			_placedTiles[position.Row, position.Column];

		public SquareBonus SquareBonusAt(Position position) =>
			_squareBonuses[position.Row, position.Column];

		public readonly BoardDimensions Dimensions;

		public Board(BoardDimensions dimensions)
		{
			this.Dimensions = dimensions;
			this._placedTiles = new PlacedTile[dimensions.Rows, dimensions.Columns];
			this._squareBonuses = new SquareBonus[dimensions.Rows, dimensions.Columns];
		}

		public PlacedTile?[] GetRow(int row)
		{
			PlacedTile?[] rv = new PlacedTile?[this.Dimensions.Columns];
			for(int col = 0; col < this.Dimensions.Columns; ++col)
			{
				rv[col] = this._placedTiles[row, col];
			}
			return rv;

		}
	}

	public static class BoardExtensions
	{
		public static IEnumerable<PlacedTile> GetTilesAbove(this Board board, Position position)
		{
			List<PlacedTile> result = new List<PlacedTile>();
			while (true)
			{
				if (position.Row == 0)
				{
					return result;
				}
				position = position.MoveUp();
				var c = board.TileAt(position);
				if (c == default)
				{
					return result;
				}
				result.Insert(0, c);
			}
		}
		public static string GetStringAbove(this Board board, Position position) =>
			ConvertToString(board.GetTilesAbove(position));

		public static string GetStringBelow(this Board board, Position position) =>
			ConvertToString(board.GetTilesBelow(position));

		public static IEnumerable<PlacedTile> GetTilesBelow(this Board board, Position position)
		{
			while (true)
			{
				if (position.Row == board.Dimensions.Rows - 1)
				{
					yield break;
				}
				position = position.MoveDown();
				var c = board.TileAt(position);
				if (c == default)
				{
					yield break;
				}
				yield return c;
			}
		}

		public static IEnumerable<PlacedTile> GetTilesToRightOf(this Board board, Position position)
		{
			
			while (true)
			{
				if (position.Column == board.Dimensions.Columns - 1)
				{
					yield break;
				}
				position = position.MoveRight();
				var t = board.TileAt(position);
				if (t == default)
				{
					yield break;
				}
				yield return t;
			}
		}

		public static string ConvertToString(this IEnumerable<PlacedTile> placedTiles) =>
			new string(placedTiles.Select(t => t.EffectiveChar).ToArray());

		public static string GetStringToLeftOf(this Board board, Position position) =>
			board.GetTilesToLeftOf(position).ConvertToString();

		public static string GetStringToRightOf(this Board board, Position position) =>
			board.GetTilesToRightOf(position).ConvertToString();

		public static IEnumerable<PlacedTile> GetTilesToLeftOf(this Board board, Position position)
		{
			var result = new List<PlacedTile>();
			while (true)
			{
				if (position.Column == 0)
				{
					return result;
				}
				position = position.MoveLeft();
				PlacedTile? c = board.TileAt(position);
				if (c == null)
				{
					return result;
				}
				result.Insert(0, c);
			}
		}

		public static IEnumerable<Position> GetAdjacentPositions(this Board board, Position position)
		{
			if (position.Row > 0)
			{
				yield return position.Move(-1, 0);
			}
			if (position.Row < (board.Dimensions.Rows - 2))
			{
				yield return position.Move(1, 0);
			}
			if (position.Column > 0)
			{
				yield return position.Move(0, -1);
			}
			if (position.Column < (board.Dimensions.Columns - 2))
			{
				yield return position.Move(0, 1);
			}
		}

		private static int CharacterScore(char c) =>
			char.ToLower(c) switch
			{
				'e' => 1,
				'a' => 1,
				'i' => 1,
				'o' => 1,
				'r' => 1,
				't' => 1,
				's' => 1,
				'd' => 2,
				'n' => 2,
				'l' => 2,
				'u' => 2,
				'h' => 3,
				'g' => 3,
				'y' => 3,
				'b' => 4,
				'c' => 4,
				'f' => 4,
				'm' => 4,
				'p' => 4,
				'w' => 4,
				'v' => 5,
				'k' => 5,
				'x' => 8,
				'j' => 10,
				'q' => 10,
				'z' => 10,
				_ => 0
			};

		public static int CalculateScore(this Board board, IEnumerable<TilePlacement> tilePlacements, IEnumerable<PlacedTile> alreadyPlaced)
		{
			var placedCharacterScore = alreadyPlaced
				.Select(t => CharacterScore(t.TileChar))
				.Sum();
			var characterBasedBonus = 0;
			var multiplier = 1;
			foreach(var tp in tilePlacements)
			{
				var tileCharacterScore = CharacterScore(tp.PlacedTile.TileChar);
				var squareBonus = board.SquareBonusAt(tp.Position);
				characterBasedBonus += squareBonus switch
				{
					SquareBonus.DoubleLetter => tileCharacterScore,
					SquareBonus.TripleLetter => tileCharacterScore * 2,
					_ => 0
				};
				multiplier *= squareBonus switch
				{
					SquareBonus.DoubleWord => 2,
					SquareBonus.TripleWord => 3,
					_ => 1
				};
				placedCharacterScore += tileCharacterScore;
			}
			return placedCharacterScore * multiplier + characterBasedBonus;
		}
	}
}
