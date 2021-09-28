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
			Assert.That(sortedByLength.First().WordSegment, Is.EqualTo("josher"));
		}

		[Test]
		public void CheatOnKim()
		{
			var board = BoardBuilder.ConstructLargeBoard();
			foreach (var wordPlacement in new[]
			{
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(7, 3),
					Word = "dumps"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(3, 7),
					Word = "gleds"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(5, 6),
					Word = "servo"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(0, 11),
					Word = "winces"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(8, 4),
					Word = "tai"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(3, 11),
					Word = "chon"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(2, 13),
					Word = "fogdog"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(6, 14),
					Word = "novels"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(1, 9),
					Word = "quid"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(9, 4),
					Word = "acerate"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(9, 11),
					Word = "dry"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(0, 8),
					Word = "jee"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(8, 8),
					Word = "fib"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(9, 14),
					Word = "hao"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(6, 5),
					Word = "emaciate"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(11, 0),
					Word = "haniwa"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(8, 1),
					Word = "umiak"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(9, 3),
					Word = "laity"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(14, 5),
					Word = "stub"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(11, 7),
					Word = "pithy"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(7, 0),
					Word = "axe"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(10, 5),
					Word = "ire"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(7, 9),
					Word = "forego"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(0, 11),
					Word = "wiz"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(0, 6),
					Word = "raj"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(1, 5),
					Word = "ne"
				}
			})
			{
				board.Place(wordPlacement);
			}
			var moveFinder = new MoveFinder();
			IEnumerable<Move> moves = moveFinder.ListAll(board, "oo").ToList();
			var sortedByScore = moves.OrderByDescending(m => m.Score).ToList();
			var best = sortedByScore.First();
		}

		[Test]
		public void AmiJ()
		{
			var board = BoardBuilder.ConstructLargeBoard();
			foreach (var wordPlacement in new[]
			{
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(7, 7),
					Word = "snow"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(5, 7),
					Word = "bisque"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(10, 6),
					Word = "mend"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(6, 9),
					Word = "pa"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(5, 9),
					Word = "unite"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(3, 11),
					Word = "thrip"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(11, 3),
					Word = "ankus"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(5, 5),
					Word = "job"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(9, 5),
					Word = "faker"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(14, 3),
					Word = "buy"
				}
			})
			{
				board.Place(wordPlacement);
			}
			var moveFinder = new MoveFinder();
			IEnumerable<Move> moves = moveFinder.ListAll(board, "toieiae").ToList();
			var sortedByScore = moves.OrderByDescending(m => m.Score).ToList();
			var best = sortedByScore.First();
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
