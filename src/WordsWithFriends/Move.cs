namespace WordsWithFriends
{
	using System.Collections.Generic;

	public record Move(
		Position Position,
		string Word,
		int Score,
		Direction Direction,
		IEnumerable<TilePlacement> TilePlacements
	);
}
