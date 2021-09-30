using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace WordsWithFriends
{
	public class TestMoveFinder
	{
		[Test]
		public void SouperSamanther()
		{
			var board = BoardBuilder.ConstructSmallBoard();
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new(4, 5),
					Word = "grykes"
				}
			);
			board.Place(
				new TilePlacement
				(
					new(5, 5),
					new(' ', 'r')
				)
			);
			var moveFinder = new MoveFinder();
			var best = moveFinder.FindBest(board, "leewopa");
			Assert.That(best.Score, Is.EqualTo(24));
			Assert.That(best.Word, Is.EqualTo("plower"));
			board.Accept(best);
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new(8, 5),
					Word = "eonian"
				}
			);
			best = moveFinder.FindBest(board, "eamailx");
			Assert.That(best.Score, Is.EqualTo(75));
			Assert.That(best.Word, Is.EqualTo("maxillae"));
			board.Accept(best);
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new(2, 0),
					Word = "carate"
				}
			);
			best = moveFinder.FindBest(board, "iavrsse");
			Assert.That(best.Score, Is.EqualTo(57));
			Assert.That(best.Word, Is.EqualTo("rev"));
			board.Accept(best);
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Down,
					Position = new(5, 8),
					Word = "equid"
				}
			);
			best = moveFinder.FindBest(board, "iassej?");
			Assert.That(best.Score, Is.EqualTo(96));
			Assert.That(best.Word, Is.EqualTo("jesse"));
			board.Accept(best);
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Across,
					Position = new(1, 5),
					Word = "both"
				}
			);
			best = moveFinder.FindBest(board, "iaosf");
			Assert.That(best.Score, Is.EqualTo(54));
			Assert.That(best.Word, Is.EqualTo("oaf"));
		}
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
			Assert.That(sortedByScore.First().Word, Is.EqualTo("raver"));
			Assert.That(sortedByScore.First().Score, Is.EqualTo(27));

			var sortedByLength = moves.OrderByDescending(m => m.Word.Length).ToList();
			Assert.That(sortedByLength.First().Word, Has.Length.EqualTo(6));
		}

		[Test]
		public void PlayingDavid()
		{
			var board = BoardBuilder.ConstructLargeBoard();
			var moveFinder = new MoveFinder();
			var best = moveFinder.FindBest(board, "bnbdaet");
			Assert.That(best.Score, Is.EqualTo(28));
			Assert.That(best.Word, Is.EqualTo("nabbed"));
			board.Accept(best);

			board.Place(
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "deaf",
					Position = new Position(8, 7)
				}
			);
			best = moveFinder.FindBest(board, "tvleakm");
			Assert.That(best.Score, Is.EqualTo(36));
			Assert.That(best.Word, Is.EqualTo("em"));

			best = moveFinder.FindBest(board, "tvlakxw");
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
			Assert.That(best.Word, Is.EqualTo("mesa"));
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
			Assert.That(best.Word, Is.EqualTo("zen"));
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
			Assert.That(best.Word, Is.EqualTo("ben"));
			Assert.That(best.Score, Is.EqualTo(31));
			board.Place(
				new WordPlacement
				{
					Position = new Position(9, 5),
					Direction = Direction.Down,
					Word = "bren"
				}
			);
			board.Place(
				new WordPlacement
				{
					Position = new Position(3, 7),
					Direction = Direction.Down,
					Word = "chided"
				}
			);
			best = moveFinder.FindBest(board, "rtcdise");
			Assert.That(best.Word, Is.EqualTo("digest"));
			Assert.That(best.Score, Is.EqualTo(27));
			board.Accept(best);
			board.Place(
				new WordPlacement
				{
					Position = new Position(9, 1),
					Direction = Direction.Down,
					Word = "tier"
				}
			);
			best = moveFinder.FindBest(board, "rcdiiom");
			Assert.That(best.Word, Is.EqualTo("rim"));
			Assert.That(best.Score, Is.EqualTo(22));
			board.Accept(best);

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

			Assert.That(bestMove.Word, Is.EqualTo("diamante"));
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
			Assert.That(bestMove.Word, Is.EqualTo("ja"));
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
			Assert.That(bestMove.Word, Is.EqualTo("wych"));
			board.Accept(bestMove);


			board.Place(
				new WordPlacement
				{
					Position = new Position(7, 0),
					Direction = Direction.Down,
					Word = "eluent"
				}
			);
			bestMove = moveFinder.FindBest(board, "ciowdil");
			Assert.That(bestMove.Word, Is.EqualTo("wilco"));
			Assert.That(bestMove.Score, Is.EqualTo(42));
			board.Accept(bestMove);

			board.Place(
				new WordPlacement
				{
					Position = new Position(0, 3),
					Direction = Direction.Down,
					Word = "no"
				}
			);
			bestMove = moveFinder.FindBest(board, "idiabos");
			Assert.That(bestMove.Word, Is.EqualTo("bias"));
			Assert.That(bestMove.Score, Is.EqualTo(30));
			board.Accept(bestMove);
			board.Place(
				new WordPlacement
				{
					Position = new Position(2, 9),
					Direction = Direction.Down,
					Word = "trouts"
				}
			);
			bestMove = moveFinder.FindBest(board, "idoneit");
			Assert.That(bestMove.Word, Is.EqualTo("edition"));
			Assert.That(bestMove.Score, Is.EqualTo(60));
			board.Accept(bestMove);
			board.Place(
				new WordPlacement
				{
					Position = new Position(5, 13),
					Direction = Direction.Down,
					Word = "threnode"
				}
			);
			board.Place(
				new TilePlacement(
					new Position(6, 13),
					new PlacedTile(' ', 'h')
				)
			);
			bestMove = moveFinder.FindBest(board, "qvgeaed");
			Assert.That(bestMove.Word, Is.EqualTo("evade"));
			Assert.That(bestMove.Score, Is.EqualTo(60));
			board.Accept(bestMove);
			board.Place(
				new WordPlacement
				{
					Position = new Position(8, 12),
					Direction = Direction.Across,
					Word = "dex"
				}
			);
			bestMove = moveFinder.FindBest(board, "qgdoiei");
			Assert.That(bestMove.Word, Is.EqualTo("dogie"));
			Assert.That(bestMove.Score, Is.EqualTo(35));
			board.Accept(bestMove);
			board.Place(
				new WordPlacement
				{
					Position = new Position(13, 10),
					Direction = Direction.Across,
					Word = "gyms"
				}
			);
			bestMove = moveFinder.FindBest(board, "qiabtph");
			Assert.That(bestMove.Word, Is.EqualTo("qats"));
			Assert.That(bestMove.Score, Is.EqualTo(38));
			board.Accept(bestMove);
			board.Place(
				new WordPlacement
				{
					Position = new Position(12, 4),
					Direction = Direction.Across,
					Word = "friller"
				}
			);
			bestMove = moveFinder.FindBest(board, "ibphopu");
			Assert.That(bestMove.Word, Is.EqualTo("hippo"));
			Assert.That(bestMove.Score, Is.EqualTo(33));
			board.Accept(bestMove);

			board.Place(
				new WordPlacement
				{
					Position = new Position(0, 6),
					Direction = Direction.Across,
					Word = "queans"
				}
			);

			bestMove = moveFinder.FindBest(board, "bu");

			Assert.That(bestMove.Word, Is.EqualTo("bur"));
			Assert.That(bestMove.Score, Is.EqualTo(19));
		}

		[Test]
		public void Granny2()
		{
			var board = BoardBuilder.ConstructLargeBoard();
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "ar",
					Position = new Position(7, 6)
				}
			);
			var moveFinder = new MoveFinder();
			var bestMove = moveFinder.FindBest(board, "rvntedm");
			Assert.That(bestMove.Word, Is.EqualTo("varment"));
			board.Accept(bestMove);
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "chair",
					Position = new Position(3, 7)
				}
			);
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "delf",
					Position = new Position(6, 9)
				}
			);
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Down,
					Word = "wavy",
					Position = new Position(5, 5)
				}
			);
			board.Place(
				new WordPlacement
				{
					Direction = Direction.Across,
					Word = "fjeld",
					Position = new Position(9, 9)
				}
			);

			bestMove = moveFinder.FindBest(board, "rsveiir");
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
