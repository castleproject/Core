
namespace CastleTests.Components.DictionaryAdapter.Tests
{
	using System.Collections.Specialized;
	using NUnit.Framework;
	using Castle.Components.DictionaryAdapter;
	[TestFixture]
	public class NameValueCollectionAdapterTests
	{
		private NameValueCollection nameValueCollection;

		[SetUp]
		public void SetUp()
		{
			nameValueCollection = new NameValueCollection();
		}

		[Test]
		public void Contains_IsCaseInsensitive()
		{
			NameValueCollectionAdapter adapter = new NameValueCollectionAdapter(nameValueCollection);
			adapter["a key"] = "a value";

			Assert.IsTrue(adapter.Contains("A Key"));
		}

		[Test]
		public void Contains_IsCorrectWhenValueIsNull()
		{
			NameValueCollectionAdapter adapter = new NameValueCollectionAdapter(nameValueCollection);
			adapter["a key"] = null;

			Assert.IsTrue(adapter.Contains("A Key"));
		}
	}
}
