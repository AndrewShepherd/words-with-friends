using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace WordsWithFriends
{
	public class TestMoveFinder
	{
		[Test]
		public void SimpleTest()
		{
			var board = new Board();
			foreach (var wordPlacement in new[]
			{
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "quoin",
					Position = new Position(5, 2)
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "seizer",
					Position = new Position(4, 5)
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "roseate",
					Position = new Position(4, 10)
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "debye",
					Position = new Position(10, 6)
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "may",
					Position = new Position(8, 9)
				}
			})
			{
				board.Place(wordPlacement);
			}
			var moveFinder = new MoveFinder();
			IEnumerable<Move> moves = moveFinder.ListAll(board, "nxvspet").ToList();
			Assert.That(moves.Count(), Is.GreaterThan(0));
			Assert.That(moves.Count(), Is.EqualTo(moves.Distinct().Count()));
			var sortedMoves = moves.OrderByDescending(m => m.WordSegment.Length).ToList();
		}
	}
}
