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
			var dictionary = new Dictionary<string, object>();
			dictionary.Add("Team", "Giants");
			dictionary.Add("Points", 48);

			var player = _factory.GetAdapter<IPlayer, object>(dictionary);
			Assert.AreEqual("Giants", player.Team);
			Assert.AreEqual(48, player.Points);
		}

		[Test]
		public void Should_Handle_String_String()
		{
			var dictionary = new Dictionary<string, string>();
			dictionary.Add("Team", "Giants");
			dictionary.Add("Points", "23");

			var player = _factory.GetAdapter<IPlayer, string>(dictionary);
			Assert.AreEqual("Giants", player.Team);
			Assert.AreEqual(23, player.Points);
		}

		[Test]
		public void Can_Save_Everything_As_A_String()
		{
			var dictionary = new Dictionary<string, string>();
			var player = (IPlayer)_factory.GetAdapter(typeof(IPlayer), dictionary.ForDictionaryAdapter(),
				new PropertyDescriptor().AddSetter(new StringStorageAttribute()));
			player.Points = 23;
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
