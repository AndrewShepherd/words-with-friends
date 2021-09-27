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
					Direction = Direction.Down,
					Word = "josh",
					Position = new Position(2, 5)
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "planchettes",
					Position = new Position(5, 0)
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "beet",
					Position = new Position(2, 7)
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "makeup",
					Position = new Position(0, 0)
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "dewing",
					Position = new Position(1,3)
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "oxide",
					Position = new Position(1, 9)
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "fay",
					Position = new Position(0, 2)
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "via",
					Position = new Position(0, 6)
				}
				// ,
				//new WordPlacement
				//{
				//	Direction = Direction.Across,
				//	Word = "jar",
				//	Position = new Position
				//}
			})
			{
				board.Place(wordPlacement);
			}
			board.Place(new TilePlacement(new Position(5, 4), new PlacedTile('?', 'c')));
			board.Place(new TilePlacement(new Position(5, 8), new PlacedTile('?', 't')));
			var moveFinder = new MoveFinder();
			IEnumerable<Move> moves = moveFinder.ListAll(board, "reoioar").ToList();
			Assert.That(moves.Count(), Is.GreaterThan(0));
			//Assert.That(moves.Count(), Is.EqualTo(24));
			var sortedByScore = moves.OrderByDescending(m => m.Score).ToList();
			Assert.That(sortedByScore.First().WordSegment, Is.EqualTo("raver"));
			Assert.That(sortedByScore.First().Score, Is.EqualTo(27));

			var sortedByLength = moves.OrderByDescending(m => m.WordSegment.Length).ToList();
			Assert.That(sortedByLength.First().WordSegment, Is.EqualTo("saithe"));
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
