using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends
{
	class TileBag
	{
		private readonly char[] _characters = new char[26];
		private readonly int _blankTileCount = 0;
		
		private TileBag(char[] characters, int blankTileCount)
		{
			_characters = characters;
			_blankTileCount = blankTileCount;
		}

		public IEnumerable<char> GetAvailableCharacters()
		{
			if(_blankTileCount > 0)
			{
				for(char c = 'a'; c <= 'z'; ++c)
				{
					yield return c;
				}
			}
			else
			{
				for(int i = 0; i < _characters.Length; ++i)
				{
					if(_characters[i] > 0)
					{
						yield return (char)('a' + i);
					}
				}
			}
		}

		public TileBag Remove(char c)
		{
			var chars = (char[])this._characters.Clone();
			var index = (int)(c - 'a');
			var blankTileCount = this._blankTileCount;
			if(chars[index] > 0)
			{
				--chars[index];
			}
			else
			{
				--blankTileCount;
			}
			return new TileBag(chars, blankTileCount);

		}

		public static TileBag FromString(string s)
		{
			char[] characters = new char[26];
			int blankTileCount = 0;
			foreach(var c in s.ToLower())
			{
				if(Char.IsLetter(c))
				{
					int index = (int)(c - 'a');
					++characters[index];
				}
				else
				{
					++blankTileCount;
				}
			}
			return new TileBag(characters, blankTileCount);
		}

	}
}
