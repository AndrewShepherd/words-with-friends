namespace WordsWithFriends
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	public sealed class MoveFinder
	{
		private static IEnumerable<Position> GetAllBoardPositions()
		{
			for(int r = 0; r < BoardDimensions.Height; ++r)
			{
				for(int c = 0; c < BoardDimensions.Width; ++c)
				{
					yield return new Position(r, c);
				}
			}
		}


		private static bool IsBlankAndAdjacentToChar(Board board, Position p)
		{
			if(board.CharAt(p) != default)
			{
				return false;
			}
			return p.GetAdjacentPositions()
				.Select(p => board.CharAt(p))
				.Any(c => c != default);
		}

		private static IEnumerable<Position> GetPotentialWordPlacementPositions(Board board) =>
			GetAllBoardPositions()
				.Where(p => IsBlankAndAdjacentToChar(board, p));


		record ExtensionResult(string WordSegment, Position Position, TileBag RemainingTiles);

		private IEnumerable<ExtensionResult> ExtendLeft(
			Board board,
			TileBag tileBag,
			Position position,
			int minLeft,
			string rightString
		)
		{
			if(position.Column < minLeft)
			{
				yield break;
			}
			char charAtPosition = board.CharAt(position);
			if(charAtPosition != default)
			{
				throw new Exception("This should not happen");
			}
			else
			{
				var wordAbove = board.GetStringAbove(position);
				var wordBelow = board.GetStringBelow(position);
				var leftString = board.GetStringToLeftOf(position);
				foreach(var c in tileBag.GetAvailableCharacters())
				{
					if((wordAbove.Length + wordBelow.Length) > 0)
					{
						if (this._wordSegmentLookup.Evaluate($"{wordAbove}{c}{wordBelow}") != WordSegmentValidity.CompleteWord)
						{
							continue;
						}
					}
					var horizontallyCombined = $"{leftString}{c}{rightString}";
					if (horizontallyCombined.Length > 1)
					{
						if (this._wordSegmentLookup.Evaluate(horizontallyCombined) == WordSegmentValidity.Invalid)
						{
							continue;
						}
					}
					var newTileBag = tileBag.Remove(c);
					yield return new ExtensionResult(
						horizontallyCombined,
						position.Move(0, 0-leftString.Length),
						newTileBag
					);
					foreach(var further in ExtendLeft(
						board,
						tileBag.Remove(c),
						position.Move(0, 0-leftString.Length-1),
						minLeft,
						horizontallyCombined
					))
					{
						yield return further;
					}
				}
			}
		}

		private IEnumerable<ExtensionResult> ExtendRight(Board board, TileBag tileBag, Position position, string leftString)
		{
			if (position.Column >= BoardDimensions.Width)
			{
				yield break;
			}
			char charAtPosition = board.CharAt(position);
			if (charAtPosition != default)
			{
				throw new Exception("This should not happen");
			}
			else
			{
				var wordAbove = board.GetStringAbove(position);
				var wordBelow = board.GetStringBelow(position);
				var rightString = board.GetStringToRightOf(position);
				foreach (var c in tileBag.GetAvailableCharacters())
				{
					if ((wordAbove.Length + wordBelow.Length) > 0)
					{
						if (this._wordSegmentLookup.Evaluate($"{wordAbove}{c}{wordBelow}") != WordSegmentValidity.CompleteWord)
						{
							continue;
						}
					}
					var horizontallyCombined = $"{leftString}{c}{rightString}";
					if (horizontallyCombined.Length > 1)
					{
						if (this._wordSegmentLookup.Evaluate(horizontallyCombined) == WordSegmentValidity.Invalid)
						{
							continue;
						}
					}
					var tileBagRemoved = tileBag.Remove(c);
					yield return new ExtensionResult(
						horizontallyCombined,
						position.Move(0, 0-leftString.Length),
						tileBagRemoved
					);
					foreach (var further in ExtendRight(
						board,
						tileBag.Remove(c),
						position.Move(0, rightString.Length + 1),
						horizontallyCombined
					))
					{
						yield return further;
					}
				}
			}
		}

		private WordSegmentLookup _wordSegmentLookup = new WordSegmentLookup();

		public IEnumerable<Move> ListAll(Board board, string availableCharacters)
		{
			// Go through the board and find all of the places that a word start can go
			var wordPlacementPositions = GetPotentialWordPlacementPositions(board)
				.ToList();

			TileBag tileBag = TileBag.FromString(availableCharacters);

			var groupedByRows = wordPlacementPositions.GroupBy(wpp => wpp.Row);
			foreach(var row in groupedByRows)
			{
				int minLeft = 0;
				foreach(var position in row.OrderBy(p => p.Column))
				{
					foreach (
						var extensionLeftStep in ExtendLeft(
							board,
							tileBag,
							position,
							minLeft,
							board.GetStringToRightOf(position)
						)
					)
					{
						if (_wordSegmentLookup.Evaluate(extensionLeftStep.WordSegment) == WordSegmentValidity.CompleteWord)
						{
							yield return new Move(extensionLeftStep.Position, extensionLeftStep.WordSegment);
						}
						foreach (
							var wordSegment2 in ExtendRight(
								board,
								extensionLeftStep.RemainingTiles,
								position.Move(0, board.GetStringToRightOf(position).Length + 1),
								extensionLeftStep.WordSegment
							)
						)
						{
							if (_wordSegmentLookup.Evaluate(wordSegment2.WordSegment) == WordSegmentValidity.CompleteWord)
							{
								yield return new Move(wordSegment2.Position, wordSegment2.WordSegment);
							}
						}
					}
					minLeft = position.Column + 1;
				}
			}
		}
	}
}
