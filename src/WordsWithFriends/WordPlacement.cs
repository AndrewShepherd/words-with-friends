﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends
{
	public record WordPlacement
	{
		public Direction Direction { get; set; }
		public string Word { get; set; }
		public Position Position { get; set; }

	}
}
