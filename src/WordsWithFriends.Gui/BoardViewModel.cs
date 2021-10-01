using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends.Gui
{
	class BoardViewModel : INotifyPropertyChanged
	{
		private Board? _board;
		public Board? Board
		{
			get => _board;
			set
			{
				if(_board != value)
				{
					_board = value;
					PropertyChanged?.Invoke(this, new(nameof(Board)));
					BoardString = BoardToString(value);
				}
			}
		}

		private string _boardString = String.Empty;
		public String BoardString
		{
			get => _boardString;
			set
			{
				if(_boardString != value)
				{
					_boardString = value;
					this.PropertyChanged?.Invoke(this, new(nameof(BoardString)));
				}
			}
		}

		private static string BoardToString(Board? board)
		{
			if(board == null)
			{
				return string.Empty;
			}
			StringBuilder sb = new StringBuilder();
			for (int row = 0; row < board.Dimensions.Rows; ++row)
			{
				for (int col = 0; col < board.Dimensions.Columns; ++col)
				{
					var c = board.CharAt(new(row, col));
					sb.Append(c == default ? ' ' : Char.ToUpper(c));
				}
			}
			return sb.ToString();
		}

		public event PropertyChangedEventHandler? PropertyChanged;
	}
}
