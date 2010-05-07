using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Castle.Components.Pagination.Tests
{
	[TestFixture]
	public class PaginationSupportTestCase
	{
		[Test]
		public void GetItemAtIndexForIEnumerable()
		{
			var l = GetSampleEnumerableData().Cast<DictionaryEntry>().OrderBy(x => x.Key);

			var entry = PaginationSupport.GetItemAtIndex(l, 0);
			Assert.AreEqual(0, entry.Key);

			entry = PaginationSupport.GetItemAtIndex(l, 3);
			Assert.AreEqual(3, entry.Key);
		}

		[Test]
		public void GetItemAtIndexForGenericIEnumerable()
		{
			var l = GetSampleGenericEnumerableData();

			var entry = PaginationSupport.GetItemAtIndex(l, 0);
			Assert.AreEqual(0, entry.Key);

			entry = PaginationSupport.GetItemAtIndex(l, 3);
			Assert.AreEqual(3, entry.Key);
		}

		protected static IEnumerable GetSampleEnumerableData()
		{
			return new Hashtable { { 0, "zero" }, { 1, "one" }, { 2, "two" }, { 3, "three" }, { 4, "four" }, { 5, "five" } };
		}

		protected static Dictionary<int, string> GetSampleGenericEnumerableData()
		{
			return new Dictionary<int, string> { { 0, "zero" }, { 1, "one" }, { 2, "two" }, { 3, "three" }, { 4, "four" }, { 5, "five" } };
		}
	}
}