using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithFriends
{
	class ValidWordProvider
	{
		const string DictionaryResourceName = "WordsWithFriends.Resources.words-with-friends-dictionary.txt";

		private static IEnumerable<string> ListAllValidWords()
		{
			var assembly = Assembly.GetExecutingAssembly();
			using var stream = assembly.GetManifestResourceStream(DictionaryResourceName);

			using var streamReader = new StreamReader(stream);
			while (!streamReader.EndOfStream)
			{
				var word = streamReader.ReadLine();
				if (!string.IsNullOrEmpty(word))
				{
					yield return word;
				}
			}
		}


		private Lazy<List<String>> _lazyWords = new Lazy<List<String>>(() => ListAllValidWords().ToList());
		public IEnumerable<string> Words =>
			_lazyWords.Value;
	}
}
