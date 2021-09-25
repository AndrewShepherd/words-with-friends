using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends
{
	public sealed class Board
	{
		private readonly char[,] _chars = new char[BoardDimensions.Height, BoardDimensions.Width];
		public void Place(WordPlacement wordPlacement)
		{
			Position pos = wordPlacement.Position;
			for (int i = 0; i < wordPlacement.Word.Length; ++i)
			{
				_chars[pos.Row, pos.Column] = wordPlacement.Word[i];
				pos = wordPlacement.Direction switch
				{
					Direction.Across => new Position(pos.Row, pos.Column + 1),
					_ => new Position(pos.Row + 1, pos.Column)
				};
			}
		}

		public char CharAt(Position position) =>
			_chars[position.Row, position.Column];

	}

	public static class BoardExtensions
	{
		public static string GetStringAbove(this Board board, Position position)
		{
			string result = string.Empty;
			while (true)
			{
				if (position.Row == 0)
				{
					return result;
				}
				position = position.MoveUp();
				char c = board.CharAt(position);
				if (c == default)
				{
					return result;
				}
				result = $"{c}{result}";
			}
		}

		public static string GetStringBelow(this Board board, Position position)
		{
			string result = string.Empty;
			while (true)
			{
				if (position.Row == BoardDimensions.Height - 1)
				{
					return result;
				}
				position = position.MoveDown();
				char c = board.CharAt(position);
				if (c == default)
				{
					return result;
				}
				result = $"{result}{c}";
			}
		}

		public static string GetStringToRightOf(this Board board, Position position)
		{
			string result = string.Empty;
			while (true)
			{
				if (position.Column == BoardDimensions.Width - 1)
				{
					return result;
				}
				position = position.MoveRight();
				char c = board.CharAt(position);
				if (c == default)
				{
					return result;
				}
				result = $"{result}{c}";
			}
		}

		public static string GetStringToLeftOf(this Board board, Position position)
		{
			string result = string.Empty;
			while (true)
			{
				if (position.Column == 0)
				{
					return result;
				}
				position = position.MoveLeft();
				char c = board.CharAt(position);
				if (c == default)
				{
					return result;
				}
				result = $"{c}{result}";
			}
		}
	}
}
