namespace WordsWithFriends
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Linq;
	public sealed class MoveFinder
	{
		private static IEnumerable<Position> GetAllBoardPositions(BoardDimensions dimensions)
		{
			for(int r = 0; r < dimensions.Rows; ++r)
			{
				for(int c = 0; c < dimensions.Columns; ++c)
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
			return board.GetAdjacentPositions(p)
				.Select(p => board.CharAt(p))
				.Any(c => c != default);
		}

		private static IEnumerable<Position> GetPotentialWordPlacementPositions(Board board) =>
			GetAllBoardPositions(board.Dimensions)
				.Where(p => IsBlankAndAdjacentToChar(board, p));


		record ExtensionResult(
			string WordSegment,
			Position StartPosition,
			TileBag RemainingTiles,
			ImmutableList<TilePlacement> TilePlacements,
			ImmutableList<PlacedTile> IncludedTiles,
			int AccumulatedCrossScore
		);

		private IEnumerable<ExtensionResult> ExtendLeft(
			Board board,
			TileBag tileBag,
			Position position,
			int minLeft,
			string rightString,
			ImmutableList<TilePlacement> tilePlacementsSoFar,
			ImmutableList<PlacedTile> tilesIncludedSoFar,
			int accumulatedCrossScore
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
				var leftIncludedTiles = board.GetTilesToLeftOf(position);
				foreach(var c in tileBag.GetAvailableCharacters())
				{
					var thisCrossScore = accumulatedCrossScore;
					if((wordAbove.Length + wordBelow.Length) > 0)
					{
						if (this._wordSegmentLookup.Evaluate($"{wordAbove}{c.EffectiveChar}{wordBelow}") != WordSegmentValidity.CompleteWord)
						{
							continue;
						}

						thisCrossScore += board.CalculateScore(
							new[] { new TilePlacement(position, c) },
							board.GetTilesAbove(position).Concat(board.GetTilesBelow(position))
						);
					}
					var horizontallyCombined = $"{leftString}{c.EffectiveChar}{rightString}";
					if (horizontallyCombined.Length > 1)
					{
						if (this._wordSegmentLookup.Evaluate(horizontallyCombined) == WordSegmentValidity.Invalid)
						{
							continue;
						}
					}
					var newTileBag = tileBag.Remove(c);

					TilePlacement tilePlacement = new TilePlacement(
						position,
						c
					);
					var newPlacementList = tilePlacementsSoFar.Add(tilePlacement);
					var incorporatedLeft = tilesIncludedSoFar.AddRange(leftIncludedTiles);
					yield return new ExtensionResult(
						horizontallyCombined,
						position.Move(0, 0-leftString.Length),
						newTileBag,
						newPlacementList,
						incorporatedLeft,
						thisCrossScore
					);
					foreach(var further in ExtendLeft(
						board,
						tileBag.Remove(c),
						position.Move(0, 0-leftString.Length-1),
						minLeft,
						horizontallyCombined,
						newPlacementList,
						incorporatedLeft,
						thisCrossScore
					))
					{
						yield return further;
					}
				}
			}
		}

		private IEnumerable<ExtensionResult> ExtendRight(
			Board board,
			TileBag tileBag,
			Position position,
			string leftString,
			ImmutableList<TilePlacement> tilePlacementsSoFar,
			ImmutableList<PlacedTile> incorporatedTiles,
			int accumulatedCrossScore
		)
		{
			if (position.Column >= board.Dimensions.Columns)
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
					var thisCrossScore = accumulatedCrossScore;
					if ((wordAbove.Length + wordBelow.Length) > 0)
					{
						if (this._wordSegmentLookup.Evaluate($"{wordAbove}{c.EffectiveChar}{wordBelow}") != WordSegmentValidity.CompleteWord)
						{
							continue;
						}
						thisCrossScore += board.CalculateScore(
							new[] { new TilePlacement(position, c) },
							board.GetTilesAbove(position).Concat(board.GetTilesBelow(position))
						);
					}
					var horizontallyCombined = $"{leftString}{c.EffectiveChar}{rightString}";
					if (horizontallyCombined.Length > 1)
					{
						if (this._wordSegmentLookup.Evaluate(horizontallyCombined) == WordSegmentValidity.Invalid)
						{
							continue;
						}
					}
					var tileBagRemoved = tileBag.Remove(c);
					TilePlacement tilePlacement = new TilePlacement(
						position,
						c
					);
					var tilePlacementsPlus = tilePlacementsSoFar.Add(tilePlacement);

					var incorporatedTilesPlus = incorporatedTiles.AddRange(board.GetTilesToRightOf(position));
					yield return new ExtensionResult(
						horizontallyCombined,
						position.Move(0, 0-leftString.Length),
						tileBagRemoved,
						tilePlacementsPlus,
						incorporatedTilesPlus,
						thisCrossScore
					);
					foreach (var further in ExtendRight(
						board,
						tileBag.Remove(c),
						position.Move(0, rightString.Length + 1),
						horizontallyCombined,
						tilePlacementsPlus,
						incorporatedTilesPlus,
						thisCrossScore
					))
					{
						yield return further;
					}
				}
			}
		}

		private WordSegmentLookup _wordSegmentLookup = new WordSegmentLookup();

		private int Score(Board board, ExtensionResult extensionResult) =>
			extensionResult.AccumulatedCrossScore +
				board.CalculateScore(
					extensionResult.TilePlacements,
					extensionResult.IncludedTiles
				);

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
							board.GetStringToRightOf(position),
							ImmutableList<TilePlacement>.Empty,
							ImmutableList<PlacedTile>.Empty.AddRange(board.GetTilesToRightOf(position)),
							0
						)
					)
					{
						if (_wordSegmentLookup.Evaluate(extensionLeftStep.WordSegment) == WordSegmentValidity.CompleteWord)
						{
							yield return new Move(
								extensionLeftStep.StartPosition,
								extensionLeftStep.WordSegment,
								Score(board, extensionLeftStep)
							);
						}
						foreach (
							var wordSegment2 in ExtendRight(
								board,
								extensionLeftStep.RemainingTiles,
								position.Move(0, board.GetStringToRightOf(position).Length + 1),
								extensionLeftStep.WordSegment,
								extensionLeftStep.TilePlacements,
								extensionLeftStep.IncludedTiles,
								extensionLeftStep.AccumulatedCrossScore
							)
						)
						{
							if (_wordSegmentLookup.Evaluate(wordSegment2.WordSegment) == WordSegmentValidity.CompleteWord)
							{
								yield return new Move(
									wordSegment2.StartPosition,
									wordSegment2.WordSegment,
									Score(board, wordSegment2)
								);
							}
						}
					}
					minLeft = position.Column + 1;
				}
			}
		}
	}
}
