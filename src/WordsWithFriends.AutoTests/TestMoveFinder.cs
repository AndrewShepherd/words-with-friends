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
			Assert.That(sortedByLength.First().WordSegment, Has.Length.EqualTo(6));


		}

		[Test]
		public void PlayingGranny()
		{
			var board = BoardBuilder.ConstructLargeBoard();
			var initialPlacements = new[]
			{
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(7, 6),
					Word = "kerfed"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(7, 9),
					Word = "fives"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(2, 10),
					Word = "hup"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(3, 11),
					Word = "hate"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(4, 12),
					Word = "hat"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(5, 13),
					Word = "legal"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(3, 14),
					Word = "peas"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(6, 10),
					Word = "jetes"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(8, 14),
					Word = "bong"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(10, 7),
					Word = "autos"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(11, 9),
					Word = "sax"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(12, 10),
					Word = "win"
				},
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(10, 11),
					Word = "axios"
				},
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(14, 7),
					Word = "squint"
				}
			};
			foreach(var wp in initialPlacements)
			{
				board.Place(wp);
			}
			var moveFinder = new MoveFinder();
			var best = moveFinder.FindBest(board, "llzms?a");
			Assert.That(best.Score, Is.EqualTo(37));
			Assert.That(best.WordSegment, Is.EqualTo("mesa"));
			board.Accept(best);
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new Position(1, 9),
					Word = "yo"
				}
			);
			board.Place(
				new WordPlacement
				{
					Word = "welly",
					Direction = Direction.Across,
					Position = new Position(1, 5)
				}
			);
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(0, 3),
					Word = "troy"
				}
			);
			best = moveFinder.FindBest(board, "zunaner");
			Assert.That(best.Score, Is.EqualTo(35));
			Assert.That(best.WordSegment, Is.EqualTo("zen"));
			board.Accept(best);
			board.Place(
				new WordPlacement
				{
					Position = new Position(2, 1),
					Word = "frozen",
					Direction = Direction.Across
				}
			);
			board.Place(
				new WordPlacement
				{
					Position = new Position(2, 1),
					Direction = Direction.Down,
					Word = "fauve"
				}
			);
			board.Place(
				new WordPlacement
				{
					Position = new Position(6, 0),
					Word = "dig",
					Direction = Direction.Down
				}
			);
			best = moveFinder.FindBest(board, "nrbretc");
			// Should place the letters 'be'
			// at 10, 12
			Assert.That(best.WordSegment, Is.EqualTo("ben"));
			Assert.That(best.Score, Is.EqualTo(31));
			board.Place(
				new WordPlacement
				{
					Position = new Position(9, 5),
					Direction = Direction.Down,
					Word = "bren"
				}
			);
		}

		[Test]
		public void TaraM()
		{
			var board = BoardBuilder.ConstructLargeBoard();
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(7, 3),
					Word = "gather"
				}
			);
			var moveFinder = new MoveFinder();
			var bestMove = moveFinder.FindBest(board, "eamidan");
			Assert.That(bestMove.Score, Is.EqualTo(65));

			Assert.That(bestMove.WordSegment, Is.EqualTo("diamante"));
			board.Accept(bestMove);

			board.Place(
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new Position(3, 2),
					Word = "kuvasz"
				}
			);
			board.Place(
				new TilePlacement(
					new Position(3, 3),
					new PlacedTile(' ', 'u')
				)
			);
			bestMove = moveFinder.FindBest(board, "ceawjyi");
			Assert.That(bestMove.Score, Is.EqualTo(34));
			Assert.That(bestMove.WordSegment, Is.EqualTo("ja"));
			// But that's not what I played!
			board.Place(
				new WordPlacement
				{
					Position = new Position(1, 2),
					Direction = Direction.Down,
					Word = "jake"
				}
			);
			board.Place(
				new WordPlacement
				{
					Position = new Position(8, 4),
					Direction = Direction.Across,
					Word = "hear"
				}
			);

			bestMove = moveFinder.FindBest(board, "cwyicho");
			Assert.That(bestMove.Score, Is.EqualTo(33));
			Assert.That(bestMove.WordSegment, Is.EqualTo("wych"));
			board.Accept(bestMove);
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
