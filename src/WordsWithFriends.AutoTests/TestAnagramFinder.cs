using NUnit.Framework;
using System.Linq;

namespace WordsWithFriends.AutoTests
{
	public class TestAnagramFinder
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Mflebie()
		{
			var anagramFinder = new AnagramFinder();
			var results = anagramFinder.ListAll("mflebie");
			Assert.That(results.Count(), Is.Not.EqualTo(0));
			results = results.OrderByDescending(r => r.Length);
		}

		[Test]
		public void NoDuplicatesNoWildcards()
		{
			var anagramFinder = new AnagramFinder();
			var results = anagramFinder.ListAll("TSAMKEX");
			Assert.That(results.Count(), Is.Not.EqualTo(0));
			Assert.That(results.Contains("texas"), Is.True);
			Assert.That(results.Contains("zoo"), Is.False);
		}

		[Test]
		public void IncludeWildcards()
		{
			var anagramFinder = new AnagramFinder();
			var results = anagramFinder.ListAll("TSAMKE?");
			Assert.That(results.Count(), Is.Not.EqualTo(0));
			Assert.That(results.Contains("texas"), Is.True);
			Assert.That(results.Contains("zoo"), Is.False);
		}
	}
}