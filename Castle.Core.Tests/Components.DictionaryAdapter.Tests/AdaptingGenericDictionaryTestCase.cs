namespace Castle.Components.DictionaryAdapter.Tests
{
	using System.Collections.Generic;
	using NUnit.Framework;

	[TestFixture]
	public class AdaptingGenericDictionaryTestCase
	{
		[Test]
		public void Should_Handle_String_Object()
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Team", "Giants");
			dictionary.Add("Points", 48);

			var player = _factory.GetAdapter<IPlayer, object>(dictionary);
			Assert.AreEqual("Giants", player.Team);
			Assert.AreEqual(48, player.Points);
		}

		[Test]
		public void Should_Handle_String_String()
		{
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Team", "Giants");
			dictionary.Add("Points", "23");

			var player = _factory.GetAdapter<IPlayer, string>(dictionary);
			Assert.AreEqual("Giants", player.Team);
			Assert.AreEqual(23, player.Points);
		}

		[SetUp]
		public void Setup()
		{
			_factory = new DictionaryAdapterFactory();
		}

		private DictionaryAdapterFactory _factory;
	}

	public interface IPlayer
	{
		string Team { get; set; }
		int Points { get; set; }
	}
}
