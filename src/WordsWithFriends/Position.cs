using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends
{
	public record Position(int Row, int Column);

	public static class PositionExtensions
	{
		public static Position Move(this Position position, int rowDelta, int columnDelta) =>
			new Position(position.Row + rowDelta, position.Column + columnDelta);

		public static Position MoveUp(this Position position) =>
			position.Move(-1, 0);

		public static Position MoveDown(this Position position) =>
			position.Move(1, 0);

		public static Position MoveRight(this Position position) =>
			position.Move(0, 1);

		public static Position MoveLeft(this Position position) =>
			position.Move(0, -1);
	}

}
