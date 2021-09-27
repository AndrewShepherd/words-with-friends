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
					Word = "aneles",
					Position = new Position(5, 0)
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "ibe",
					Position = new Position(4, 1)
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "jings",
					Position = new Position(1, 5)
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "om",
					Position = new Position(3, 2)
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "etui",
					Position = new Position(1, 6)
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "zitis",
					Position = new Position(2, 4)
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "aerobe",
					Position = new Position(0, 2)
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "kor",
					Position = new Position(2, 0)
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "seq",
					Position = new Position(1, 1)
				}
			})
			{
				board.Place(wordPlacement);
			}
			board.Place(new TilePlacement(new Position(4, 0), new PlacedTile('?', 'k')));
			var moveFinder = new MoveFinder();
			IEnumerable<Move> moves = moveFinder.ListAll(board, "fwdatev").ToList();
			Assert.That(moves.Count(), Is.GreaterThan(0));
			Assert.That(moves.Count(), Is.EqualTo(moves.Distinct().Count()));
			var sortedByScore = moves.OrderByDescending(m => m.Score).ToList();
			Assert.That(sortedByScore.First().WordSegment, Is.EqualTo("jete"));
			Assert.That(sortedByScore.First().Score, Is.EqualTo(17));

			var sortedByLength = moves.OrderByDescending(m => m.WordSegment.Length).ToList();
			Assert.That(sortedByLength.First().WordSegment, Is.EqualTo("gifted"));
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
