namespace WordsWithFriends
{
	using System.Collections.Generic;

	public record Move(
		Position Position,
		string WordSegment,
		int Score,
		Direction Direction,
		IEnumerable<TilePlacement> TilePlacements
	);
}
