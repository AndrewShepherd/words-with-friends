namespace WordsWithFriends
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;

	public class AnagramFinder
	{



		private static bool IsAnagram(string target, string source)
		{
			var lookup = new int[26];
			var wildcardCount = 0;

			Func<char, int> toIndex = (c) => (int)(Char.ToLower(c) - 'a');

			foreach(var c in source.ToLower())
			{
				if(c == '?')
				{
					++wildcardCount;
				}
				else
				{
					++lookup[toIndex(c)];
				}
			}

			foreach(var c in target)
			{
				var lookupIndex = toIndex(c);
				if(lookup[lookupIndex] > 0)
				{
					--lookup[lookupIndex];
				}
				else if (wildcardCount > 0)
				{
					--wildcardCount;
				}
				else
				{
					return false;
				}
			}
			return true;
		}

		private readonly ValidWordProvider _validWordProvider = new ValidWordProvider();

		public IEnumerable<string> ListAll(string letters)
		{
			// How do I get the resource?
			foreach(var word in _validWordProvider.Words)
			{
				if(IsAnagram(word, letters))
				{
					yield return word;
				}
			}
		}
	}
}
