using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends
{
	public enum WordSegmentValidity
	{
		CompleteWord,
		PartialWord,
		Invalid
	}

	class WordSegmentLookup
	{

		private Dictionary<string, bool> _lookup = new Dictionary<string, bool>();

		private static IEnumerable<string> GetAllSegments(string word)
		{
			for(int i = 0; i < word.Length-1; ++i)
			{
				for(int j = 2; j + i <= word.Length; ++j)
				{
					yield return word.Substring(i, j);
				}
			}
		}
		public WordSegmentLookup()
		{
			var provider = new ValidWordProvider();
			foreach(var word in provider.Words)
			{
				_lookup[word] = true;
				foreach(var segment in GetAllSegments(word))
				{
					if(!_lookup.ContainsKey(segment))
					{
						_lookup[segment] = false;
					}
				}
			}
		}
		public WordSegmentValidity Evaluate(string wordSegment)
		{
			if(_lookup.TryGetValue(wordSegment, out bool isCompleteWord))
			{
				return isCompleteWord
					? WordSegmentValidity.CompleteWord
					: WordSegmentValidity.PartialWord;
			}
			else
			{
				return WordSegmentValidity.Invalid;
			}
		}

	}
}
