using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace WordsWithFriends
{
	public class TestMoveFinder
	{
		[Test]
		public void ActualGame()
		{
			var board = BoardBuilder.ConstructSmallBoard();
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
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "sex",
					Position = new Position(6, 3)
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "lathis",
					Position = new Position(0, 7)
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "pavan",
					Position = new Position(1, 6)
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "wilco",
					Position = new Position(9, 2)
				}
			})
			{
				board.Place(wordPlacement);
			}
			var moveFinder = new MoveFinder();
			IEnumerable<Move> moves = moveFinder.ListAll(board, "trikfig").ToList();
			Assert.That(moves.Count(), Is.GreaterThan(0));
			Assert.That(moves.Count(), Is.EqualTo(moves.Distinct().Count()));
			var sortedMoves = moves.OrderByDescending(m => m.Score).ToList();

			// First word SEPT, scores 7 + 6 + 4 = 17
			// Second word STEP scores 7 + 6 = 13
			// Thirds word QUOINS, should score 18
		}

		[Test]
		public void OneWord()
		{
			var board = new Board(new BoardDimensions(11, 11));
			foreach (var wordPlacement in new[]
			{
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "anagram",
					Position = new Position(5, 2)
				}
			})
			{
				board.Place(wordPlacement);
			}
			var moveFinder = new MoveFinder();
			IEnumerable<Move> moves = moveFinder.ListAll(board, "abcdefg").ToList();
			Assert.That(moves.Count(), Is.GreaterThan(0));
			Assert.That(moves.Count(), Is.EqualTo(moves.Distinct().Count()));
			var sortedMoves = moves.OrderByDescending(m => m.Score).ToList();
		}
	}
}
