using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends
{
	public static class BoardBuilder
	{
		public static Board ConstructLargeBoard()
		{
			Board board = new Board(new BoardDimensions(15, 15));
			var squareBonuses = new[]
			{
				// Row, Column, Bonus
				(0, 3, SquareBonus.TripleWord),
				(0, 6, SquareBonus.TripleLetter),
				(0, 8, SquareBonus.TripleLetter),
				(0, 11, SquareBonus.TripleWord),
				(14, 3, SquareBonus.TripleWord),
				(14, 6, SquareBonus.TripleLetter),
				(14, 8, SquareBonus.TripleLetter),
				(14, 11, SquareBonus.TripleWord),
				(1, 2, SquareBonus.DoubleLetter),
				(1, 5, SquareBonus.DoubleWord),
				(1, 9, SquareBonus.DoubleWord),
				(1, 12, SquareBonus.DoubleLetter),
				(1, 2, SquareBonus.DoubleLetter),
				(13, 2, SquareBonus.DoubleLetter),
				(13, 5, SquareBonus.DoubleWord),
				(13, 9, SquareBonus.DoubleWord),
				(13, 12, SquareBonus.DoubleLetter),
				(2, 1, SquareBonus.DoubleLetter),
				(2, 4, SquareBonus.DoubleLetter),
				(2, 10, SquareBonus.DoubleLetter),
				(2, 13, SquareBonus.DoubleLetter),
				(12, 1, SquareBonus.DoubleLetter),
				(12, 4, SquareBonus.DoubleLetter),
				(12, 10, SquareBonus.DoubleLetter),
				(12, 13, SquareBonus.DoubleLetter),
				(3, 0, SquareBonus.TripleWord),
				(3, 3, SquareBonus.TripleLetter),
				(3, 7, SquareBonus.DoubleWord),
				(3, 11, SquareBonus.TripleLetter),
				(3, 14, SquareBonus.TripleWord),
				(11, 0, SquareBonus.TripleWord),
				(11, 3, SquareBonus.TripleLetter),
				(11, 7, SquareBonus.DoubleWord),
				(11, 11, SquareBonus.TripleLetter),
				(11, 14, SquareBonus.TripleWord),
				(4, 2, SquareBonus.DoubleLetter),
				(4, 6, SquareBonus.DoubleLetter),
				(4, 8, SquareBonus.DoubleLetter),
				(4, 12, SquareBonus.DoubleLetter),
				(10, 2, SquareBonus.DoubleLetter),
				(10, 6, SquareBonus.DoubleLetter),
				(10, 8, SquareBonus.DoubleLetter),
				(10, 12, SquareBonus.DoubleLetter),
				(5, 1, SquareBonus.DoubleWord),
				(5, 5, SquareBonus.TripleLetter),
				(5, 9, SquareBonus.TripleLetter),
				(5, 13, SquareBonus.DoubleWord),
				(9, 1, SquareBonus.DoubleWord),
				(9, 5, SquareBonus.TripleLetter),
				(9, 9, SquareBonus.TripleLetter),
				(9, 13, SquareBonus.DoubleWord),
				(6, 0, SquareBonus.TripleLetter),
				(6, 4, SquareBonus.DoubleLetter),
				(6, 10, SquareBonus.DoubleLetter),
				(6, 14, SquareBonus.TripleLetter),
				(8, 0, SquareBonus.TripleLetter),
				(8, 4, SquareBonus.DoubleLetter),
				(8, 10, SquareBonus.DoubleLetter),
				(8, 14, SquareBonus.TripleLetter),
				(7, 3, SquareBonus.DoubleWord),
				(7, 11, SquareBonus.DoubleWord)

			};
			foreach (var entry in squareBonuses)
			{
				board.SetBonus(new Position(entry.Item1, entry.Item2), entry.Item3);
			}
			return board;
		}
		public static Board ConstructSmallBoard()
		{
			Board board = new Board(new BoardDimensions(11, 11));
			var squareBonuses = new[]
			{
				// Row, Column, Bonus
				(0, 0, SquareBonus.TripleLetter),
				(0, 2, SquareBonus.TripleWord),
				(0, 8, SquareBonus.TripleWord),
				(0, 10, SquareBonus.TripleLetter),
				(1, 1, SquareBonus.DoubleWord),
				(1, 5, SquareBonus.DoubleWord),
				(1, 9, SquareBonus.DoubleWord),
				(2, 0, SquareBonus.TripleWord),
				(2, 2, SquareBonus.DoubleLetter),
				(2, 4, SquareBonus.DoubleLetter),
				(2, 6, SquareBonus.DoubleLetter),
				(2, 8, SquareBonus.DoubleLetter),
				(2, 10, SquareBonus.DoubleLetter),
				(3, 3, SquareBonus.TripleLetter),
				(3, 7, SquareBonus.TripleLetter),
				(4, 2, SquareBonus.DoubleLetter),
				(4, 8, SquareBonus.DoubleLetter),
				(5, 1, SquareBonus.DoubleWord),
				(5, 9, SquareBonus.DoubleWord),
				(6, 2, SquareBonus.DoubleLetter),
				(6, 8, SquareBonus.DoubleLetter),
				(7, 3, SquareBonus.TripleLetter),
				(7, 7, SquareBonus.TripleLetter),
				(8, 0, SquareBonus.TripleWord),
				(8, 2, SquareBonus.DoubleLetter),
				(8, 4, SquareBonus.DoubleLetter),
				(8, 6, SquareBonus.DoubleLetter),
				(8, 8, SquareBonus.DoubleLetter),
				(8, 10, SquareBonus.TripleWord),
				(9, 1, SquareBonus.DoubleWord),
				(9, 5, SquareBonus.DoubleWord),
				(9, 9, SquareBonus.DoubleWord),
				(10, 0, SquareBonus.TripleLetter),
				(10, 2, SquareBonus.TripleWord),
				(10, 8, SquareBonus.TripleWord),
				(10, 10, SquareBonus.TripleLetter)
			};
			foreach(var entry in squareBonuses)
			{
				board.SetBonus(new Position(entry.Item1, entry.Item2), entry.Item3);
			}
			return board;
		}
	}
}
