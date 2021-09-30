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
			string WordSegment, // Trying to make it that this is not needed, as it can be generated from the others
			Position StartPosition,
			TileBag RemainingTiles,
			ImmutableList<TilePlacement> TilePlacements,
			ImmutableList<PlacedTile> IncludedTiles,
			int AccumulatedCrossScore
		);


		private Func<PlacedTile, (bool, int)> GenerateVerticalCrossTester(
			Board board,
			Position position,
			IEnumerable<TilePlacement> placementsAlready,
			Func<WordSegmentValidity, bool> passingRequirement
		)
		{
			PlacedTile?[] boardTiles = board.GetColumn(position.Column);
			PlacedTile?[] turnPlacements = new PlacedTile?[board.Dimensions.Rows];
			foreach (var p in placementsAlready.Where(p => p.Position.Column == position.Column))
			{
				turnPlacements[p.Position.Row] = p.PlacedTile;
			}
			return GenerateChecker(
				board,
				position,
				placementsAlready,
				passingRequirement,
				boardTiles,
				turnPlacements,
				p => p.Row
			);
		}


		private Func<PlacedTile, (bool, int)> GenerateHorizontalCrossTester(
			Board board,
			Position position,
			IEnumerable<TilePlacement> placementsAlready,
			Func<WordSegmentValidity, bool> passingRequirement
		)
		{
			PlacedTile?[] boardTiles = board.GetRow(position.Row);
			PlacedTile?[] turnPlacements = new PlacedTile?[board.Dimensions.Columns];
			foreach (var p in placementsAlready.Where(p => p.Position.Row == position.Row))
			{
				turnPlacements[p.Position.Column] = p.PlacedTile;
			}
			var indexThatWillBeTested = position.Column;
			return GenerateChecker(
				board,
				position,
				placementsAlready,
				passingRequirement,
				boardTiles,
				turnPlacements,
				p => p.Column
			);
		}

		private Func<PlacedTile, (bool, int)> GenerateChecker(
			Board board,
			Position position,
			IEnumerable<TilePlacement> placementsAlready,
			Func<WordSegmentValidity, bool> passingRequirement,
			PlacedTile?[] boardTiles,
			PlacedTile?[] turnPlacements,
			Func<Position, int> getIndexToTest
		)
		{
			var leftIndex = getIndexToTest(position);
			List<char> leftChars = new List<char>();
			List<char> rightChars = new List<char>();
			List<PlacedTile> includedBoardTiles = new List<PlacedTile>();
			while (
				(leftIndex > 0) &&
				!(
					turnPlacements[leftIndex - 1] == default
					&& boardTiles[leftIndex - 1] == default
				)
			)
			{
				var boardTile = boardTiles[leftIndex - 1];
				if (boardTile != null)
				{
					includedBoardTiles.Add(boardTile);
					leftChars.Insert(0, boardTile.EffectiveChar);
				}
				else
				{
					var turnPlacement = turnPlacements[leftIndex - 1];
					if (turnPlacement != default)
					{
						leftChars.Insert(0, turnPlacement.EffectiveChar);
					}
				}
				--leftIndex;
			}
			var exclusiveRightIndex = getIndexToTest(position) + 1;
			while (
				(exclusiveRightIndex < turnPlacements.Length) &&
				!(
					turnPlacements[exclusiveRightIndex] == default
					&& boardTiles[exclusiveRightIndex] == default
				)
			)
			{
				var boardTile = boardTiles[exclusiveRightIndex];
				if (boardTile != null)
				{
					includedBoardTiles.Add(boardTile);
					rightChars.Add(boardTile.EffectiveChar);
				}
				else
				{
					var turnPlacement = turnPlacements[exclusiveRightIndex];
					if (turnPlacement != default)
					{
						rightChars.Add(turnPlacement.EffectiveChar);
					}
				}
				++exclusiveRightIndex;
			}

			var leftString = new String(leftChars.ToArray());
			var rightString = new String(rightChars.ToArray());

			if (leftString.Length + rightString.Length == 0)
			{
				return _ => (true, 0);
			}
			return (placedTile) =>
			{
				var testResult = _wordSegmentLookup.Evaluate($"{leftString}{placedTile.EffectiveChar}{rightString}");
				if (passingRequirement(testResult))
				{
					return (
						true,
						board.CalculateScore(
							placementsAlready
								.Concat(
									new[] { new TilePlacement(position, placedTile) }
								),
							includedBoardTiles
						)
					);
				}
				else
				{
					return (
						false,
						0
					);
				}
			};
		}

		private IEnumerable<ExtensionResult> ExtendUp(
			Board board,
			Position position,
			Func<Position, bool> continuePredicate,
			ExtensionResult extensionSoFar
		)
		{
			if (!continuePredicate(position))
			{
				yield break;
			}
			if((extensionSoFar.WordSegment.Length > 1) && (_wordSegmentLookup.Evaluate(extensionSoFar.WordSegment) == WordSegmentValidity.Invalid))
			{
				throw new Exception($"Should not be evaluating an invalid word segment ({extensionSoFar.WordSegment})");
			}
			char charAtPosition = board.CharAt(position);
			if (charAtPosition != default)
			{
				throw new Exception("This should not happen");
			}
			var crossTester = GenerateHorizontalCrossTester(
				board,
				position,
				Enumerable.Empty<TilePlacement>(),
				s => s == WordSegmentValidity.CompleteWord
			);
			var lengthwaysTester = GenerateVerticalCrossTester(
				board,
				position,
				extensionSoFar.TilePlacements,
				s => s != WordSegmentValidity.Invalid
			);
			var aboveString = board.GetStringAbove(position);
			var aboveIncludedTiles = board.GetTilesAbove(position);
			foreach (var c in extensionSoFar.RemainingTiles.GetAvailableCharacters())
			{
				(bool crossSuccess, int thisCrossScore) = crossTester(c);
				if (!crossSuccess)
				{
					continue;
				}
				(bool lengthwaysSuccess, int lengthwaysScore) = lengthwaysTester(c);
				if(!lengthwaysSuccess)
				{
					continue;
				}
				var extensionResult = new ExtensionResult(
					$"{aboveString}{c.EffectiveChar}{extensionSoFar.WordSegment}",
					position.Move(0-aboveString.Length, 0),
					extensionSoFar.RemainingTiles.Remove(c),
					extensionSoFar.TilePlacements.Add(new TilePlacement(position, c)),
					extensionSoFar.IncludedTiles.AddRange(aboveIncludedTiles),
					extensionSoFar.AccumulatedCrossScore + thisCrossScore
				);
				yield return extensionResult;
				foreach(var futher in ExtendUp(
					board,
					position.Move(0-aboveString.Length-1, 0),
					continuePredicate,
					extensionResult
				))
				{
					yield return futher;
				}

			}
			yield break;
		}

		private IEnumerable<ExtensionResult> ExtendDown(
				Board board,
				Position position,
				Func<Position, bool> continuePredicate,
				ExtensionResult extensionSoFar
			)
		{
			if (!continuePredicate(position))
			{
				yield break;
			}
			char charAtPosition = board.CharAt(position);
			if (charAtPosition != default)
			{
				throw new Exception("This should not happen");
			}
			var crossTester = GenerateHorizontalCrossTester(
				board,
				position,
				Enumerable.Empty<TilePlacement>(),
				s => s == WordSegmentValidity.CompleteWord
			);
			var lengthwaysTester = GenerateVerticalCrossTester(
				board,
				position,
				extensionSoFar.TilePlacements,
				s => s != WordSegmentValidity.Invalid
			);
			var belowString = board.GetStringBelow(position);
			var belowIncludedTiles = board.GetTilesBelow(position);
			foreach (var c in extensionSoFar.RemainingTiles.GetAvailableCharacters())
			{
				(bool crossSuccess, int thisCrossScore) = crossTester(c);
				if (!crossSuccess)
				{
					continue;
				}
				(bool lengthwaysSuccess, int lengthwaysScore) = lengthwaysTester(c);
				if (!lengthwaysSuccess)
				{
					continue;
				}
				var extensionResult = new ExtensionResult(
					$"{extensionSoFar.WordSegment}{c.EffectiveChar}{belowString}",
					position.Move(0-extensionSoFar.WordSegment.Length, 0),
					extensionSoFar.RemainingTiles.Remove(c),
					extensionSoFar.TilePlacements.Add(new TilePlacement(position, c)),
					extensionSoFar.IncludedTiles.AddRange(belowIncludedTiles),
					extensionSoFar.AccumulatedCrossScore + thisCrossScore
				);
				yield return extensionResult;
				foreach (var futher in ExtendDown(
					board,
					position.Move(0 + belowString.Length + 1, 0),
					continuePredicate,
					extensionResult
				))
				{
					yield return futher;
				}

			}
			yield break;
		}


		private IEnumerable<ExtensionResult> ExtendLeft(
			Board board,
			Position position,
			Func<Position, bool> continuePredicate,
			ExtensionResult extensionSoFar
		)
		{
			if(!continuePredicate(position))
			{
				yield break;
			}
			if ((extensionSoFar.WordSegment.Length > 1) && (_wordSegmentLookup.Evaluate(extensionSoFar.WordSegment) == WordSegmentValidity.Invalid))
			{
				throw new Exception($"Should not be evaluating an invalid word segment {extensionSoFar.WordSegment}");
			}
			char charAtPosition = board.CharAt(position);
			if(charAtPosition != default)
			{
				throw new Exception("This should not happen");
			}
			var crossTester = GenerateVerticalCrossTester(
				board,
				position,
				Enumerable.Empty<TilePlacement>(),
				s => s == WordSegmentValidity.CompleteWord
			);
			var lengthwaysTester = GenerateHorizontalCrossTester(
				board,
				position,
				extensionSoFar.TilePlacements,
				s => s != WordSegmentValidity.Invalid
			);
			var leftString = board.GetStringToLeftOf(position);
			var leftIncludedTiles = board.GetTilesToLeftOf(position);
			foreach(var c in extensionSoFar.RemainingTiles.GetAvailableCharacters())
			{
				(bool success, int thisCrossScore) = crossTester(c);
				if(!success)
				{
					continue;
				}
				(bool lengthwaysSuccess, int thisLengthScore) = lengthwaysTester(c);
				if(!lengthwaysSuccess)
				{
					continue;
				}
				var horizontallyCombined = $"{leftString}{c.EffectiveChar}{extensionSoFar.WordSegment}";
				var newTileBag = extensionSoFar.RemainingTiles.Remove(c);

				TilePlacement tilePlacement = new TilePlacement(
					position,
					c
				);
				var newPlacementList = extensionSoFar.TilePlacements.Add(tilePlacement);
				var extensionResult = new ExtensionResult(
					horizontallyCombined,
					position.Move(0, 0-leftString.Length),
					newTileBag,
					newPlacementList,
					extensionSoFar.IncludedTiles.AddRange(leftIncludedTiles),
					extensionSoFar.AccumulatedCrossScore + thisCrossScore
				);
				yield return extensionResult;
				foreach(var further in ExtendLeft(
					board,
					position.Move(0, 0-leftString.Length-1),
					continuePredicate,
					extensionResult
				))
				{
					yield return further;
				}
			}
		}

		private IEnumerable<ExtensionResult> ExtendRight(
			Board board,
			Position position,
			Func<Position, bool> continuePredicate,
			ExtensionResult extensionSoFar
		)
		{
			if (!continuePredicate(position))
			{
				yield break;
			}
			if (board.CharAt(position) != default)
			{
				throw new Exception("This should not happen");
			}
			var crossTester = GenerateVerticalCrossTester(
				board,
				position,
				Enumerable.Empty<TilePlacement>(),
				s => s == WordSegmentValidity.CompleteWord
			);
			var lengthwaysTester = GenerateHorizontalCrossTester(
				board,
				position,
				extensionSoFar.TilePlacements,
				s => s != WordSegmentValidity.Invalid
			);
			var rightString = board.GetStringToRightOf(position);
			foreach (var c in extensionSoFar.RemainingTiles.GetAvailableCharacters())
			{
				(bool crossTestSuccess, int thisCrossScore) = crossTester(c);
				if (!crossTestSuccess)
				{
					continue;
				}
				(bool lengthWaysSuccess, int lengthWaysCrossScoree) = lengthwaysTester(c);
				if(!lengthWaysSuccess)
				{
					continue;
				}
				var newExtensionResult = new ExtensionResult(
					$"{extensionSoFar.WordSegment}{c.EffectiveChar}{rightString}", // Not needed anymore?
					position.Move(
						0,
						0-extensionSoFar.WordSegment.Length
					),
					extensionSoFar.RemainingTiles.Remove(c),
					extensionSoFar.TilePlacements.Add(
						new TilePlacement(position, c)
					),
					extensionSoFar.IncludedTiles.AddRange(
						board.GetTilesToRightOf(position)
					),
					thisCrossScore + extensionSoFar.AccumulatedCrossScore
				);
				yield return newExtensionResult;
				foreach (var further in ExtendRight(
					board,
					position.Move(0, rightString.Length + 1),
					continuePredicate,
					newExtensionResult
				))
				{
					yield return further;
				}
			}
		}

		private static WordSegmentLookup _wordSegmentLookup = new WordSegmentLookup();

		private int Score(Board board, ExtensionResult extensionResult) =>
			extensionResult.AccumulatedCrossScore +
				board.CalculateScore(
					extensionResult.TilePlacements,
					extensionResult.IncludedTiles
				);


		private IEnumerable<Move> ListAllForRow(
			Board board,
			TileBag tileBag,
			IEnumerable<Position> candidatePositions
		)
		{
			int minLeft = 0;
			foreach (var position in candidatePositions)
			{
				var extensionResult = new ExtensionResult(
					board.GetStringToRightOf(position),
					position,
					tileBag,
					ImmutableList<TilePlacement>.Empty,
					ImmutableList<PlacedTile>.Empty.AddRange(board.GetTilesToRightOf(position)),
					0
				);
				foreach (
					var extensionLeftStep in ExtendLeft(
						board,
						position,
						position => position.Column >= minLeft,
						extensionResult
					)
				)
				{
					if (_wordSegmentLookup.Evaluate(extensionLeftStep.WordSegment) == WordSegmentValidity.CompleteWord)
					{
						yield return new Move(
							extensionLeftStep.StartPosition,
							extensionLeftStep.WordSegment,
							Score(board, extensionLeftStep),
							Direction.Across,
							extensionLeftStep.TilePlacements
						);
					}
					foreach (
						var wordSegment2 in ExtendRight(
							board,
							position.Move(0, board.GetStringToRightOf(position).Length + 1),
							position => position.Column < board.Dimensions.Columns,
							extensionLeftStep
						)
					)
					{
						if (_wordSegmentLookup.Evaluate(wordSegment2.WordSegment) == WordSegmentValidity.CompleteWord)
						{
							yield return new Move(
								wordSegment2.StartPosition,
								wordSegment2.WordSegment,
								Score(board, wordSegment2),
								Direction.Across,
								wordSegment2.TilePlacements
							);
						}
					}
				}
				minLeft = position.Column + 1;
			}
		}

		private IEnumerable<Move> ListAllForColumn(
			Board board,
			TileBag tileBag,
			IEnumerable<Position> candidatePositions
		)
		{
			int minRow = 0;
			foreach(var position in candidatePositions)
			{
				var extensionResult = new ExtensionResult(
					board.GetStringBelow(position),
					position,
					tileBag,
					ImmutableList<TilePlacement>.Empty,
					ImmutableList<PlacedTile>.Empty.AddRange(board.GetTilesBelow(position)),
					0
				);
				foreach (
					var extendUpStep in ExtendUp(
						board,
						position,
						position => position.Row >= minRow,
						extensionResult
					)
				)
				{
					if (_wordSegmentLookup.Evaluate(extendUpStep.WordSegment) == WordSegmentValidity.CompleteWord)
					{
						yield return new Move(
							extendUpStep.StartPosition,
							extendUpStep.WordSegment,
							Score(board, extendUpStep),
							Direction.Down,
							extendUpStep.TilePlacements
						);
					}
					foreach (
							var wordSegment2 in ExtendDown(
								board,
								position.Move(board.GetStringBelow(position).Length + 1, 0),
								position => position.Row < board.Dimensions.Rows,
								extendUpStep
							)
)
					{
						if (_wordSegmentLookup.Evaluate(wordSegment2.WordSegment) == WordSegmentValidity.CompleteWord)
						{
							yield return new Move(
								wordSegment2.StartPosition,
								wordSegment2.WordSegment,
								Score(board, wordSegment2),
								Direction.Down,
								wordSegment2.TilePlacements
							);
						}
					}
				}
				minRow = position.Row + 1;
			}
		}

		public IEnumerable<Move> ListAll(Board board, string availableCharacters)
		{
			// Go through the board and find all of the places that a word start can go
			var wordPlacementPositions = GetPotentialWordPlacementPositions(board)
				.ToList();

			TileBag tileBag = TileBag.FromString(availableCharacters);

			if(wordPlacementPositions.Count == 0)
			{
				wordPlacementPositions = new()
				{
					new(
						(board.Dimensions.Rows-1)/2,
						(board.Dimensions.Columns-1)/2
					)
				};
			}

			var groupedByRows = wordPlacementPositions.GroupBy(wpp => wpp.Row);
			foreach(var row in groupedByRows.OrderBy(g => g.Key))
			{
				var positions = row.OrderBy(p => p.Column);
				foreach (var move in ListAllForRow(board, tileBag, positions))
				{
					yield return move;
				}
			}

			var groupedByColumns = wordPlacementPositions.GroupBy(wpp => wpp.Column);
			foreach(var col in groupedByColumns.OrderBy(g => g.Key))
			{
				var positions = col.OrderBy(p => p.Row);
				foreach(var move in ListAllForColumn(board, tileBag, positions))
				{
					yield return move;
				}
			}


		}
	}

	public static class MoveFinderExtensions
	{
		public static Move FindBest(this MoveFinder moveFinder, Board board, string letters) =>
			moveFinder.ListAll(board, letters)
				.OrderByDescending(move => move.Score)
				.First();
	}
}
