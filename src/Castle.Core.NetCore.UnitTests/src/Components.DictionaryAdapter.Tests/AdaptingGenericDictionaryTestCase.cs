namespace Castle.Components.DictionaryAdapter.Tests
{
	using System.Collections.Generic;

	using Xunit;

	public class AdaptingGenericDictionaryTestCase
	{
		[Fact]
		public void Should_Handle_String_Object()
		{
			var dictionary = new Dictionary<string, object>
			{
				{ "Team", "Giants" },
				{ "Points", 48 }
			};

			var player = _factory.GetAdapter<IPlayer>(dictionary);
			Assert.Equal("Giants", player.Team);
			Assert.Equal(48, player.Points);
		}

		[Fact]
		public void Should_Handle_String_String()
		{
			var dictionary = new Dictionary<string, string>();
			dictionary.Add("Team", "Giants");
			dictionary.Add("Points", "23");

			var player = _factory.GetAdapter<IPlayer, string>(dictionary);
			Assert.Equal("Giants", player.Team);
			Assert.Equal(23, player.Points);
		}

		[Fact]
		public void Can_Save_Everything_As_A_String()
		{
			var dictionary = new Dictionary<string, string>();
			var player = (IPlayer)_factory.GetAdapter(typeof(IPlayer), dictionary.ForDictionaryAdapter(),
				new PropertyDescriptor().AddBehaviors(new StringStorageAttribute()));
			player.Points = 23;
			Assert.Equal(23, player.Points);
		}

		public AdaptingGenericDictionaryTestCase()
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