using LRUMap=Commons.Collections.LRUMap;

namespace NVelocity.Test.Commons
{
	using System;
	using System.Collections;
	using NUnit.Framework;

	/// <summary>
	/// Tests for Commons.Collections.KeyedList
	/// </summary>
	[TestFixture]
	public class LRUMapTest
	{
		[Test]
		public void Test()
		{
			LRUMap map = new LRUMap(5);
			AssertAddedFirst(map, "One", 1);
			AssertAddedFirst(map, "Two", 2);
			AssertAddedFirst(map, "Three", 3);
			AssertAddedFirst(map, "Four", 4);
			AssertAddedFirst(map, "Five", 5);
			AssertAddedFirst(map, "Six", 6);
			Assert.IsTrue(!map.Contains("One"));
			AssertAddedFirst(map, "Seven", 7);
			Assert.IsTrue(!map.Contains("Two"));
			AssertAddedFirst(map, "Eight", 8);
			Assert.IsTrue(!map.Contains("Three"));
			AssertAddedFirst(map, "Nine", 9);
			Assert.IsTrue(!map.Contains("Four"));
			AssertAddedFirst(map, "Ten", 10);
			Assert.IsTrue(!map.Contains("Five"));

			map.Remove("Eight");
			Assert.AreEqual(4, map.Count);
			map.Add("One", 1);
			Assert.AreEqual(5, map.Count);
			Assert.IsTrue(map.Contains("One"));
			Assert.IsTrue(map.Contains("Six"));
			Assert.IsTrue(map.Contains("Seven"));
			Assert.IsTrue(map.Contains("Nine"));
			Assert.IsTrue(map.Contains("Ten"));
			Assert.AreEqual("Six", ((ArrayList) map.Keys)[map.Count - 1]);
			Assert.AreEqual("One", ((ArrayList) map.Keys)[0]);

			AssertGetIsMostRecent(map, "Six", 6);
			AssertGetIsMostRecent(map, "Nine", 9);
			AssertGetIsMostRecent(map, "Seven", 7);
			AssertGetIsMostRecent(map, "Ten", 10);
			AssertGetIsMostRecent(map, "One", 1);
			Assert.AreEqual("Six", ((ArrayList) map.Keys)[map.Count - 1]);

			AssertSetIsMostRecent(map, "One", "Uno");
			AssertSetIsMostRecent(map, "Two", "Dos");
			Assert.AreEqual(5, map.Count);
		}

		private void AssertAddedFirst(LRUMap map, Object key, Object value)
		{
			map.Add(key, value);
			Assert.AreEqual(key, ((ArrayList) map.Keys)[0]);
			Assert.AreEqual(value, ((ArrayList) map.Values)[0]);
			Assert.IsTrue(map.Count <= map.MaxSize);
		}

		private void AssertSetIsMostRecent(LRUMap map, Object key, Object value)
		{
			map[key] = value;
			Assert.AreEqual(key, ((ArrayList) map.Keys)[0]);
			Assert.AreEqual(value, ((ArrayList) map.Values)[0]);
			Assert.IsTrue(map.Count <= map.MaxSize);
		}

		private void AssertGetIsMostRecent(LRUMap map, Object key, Object value)
		{
			Object o = map[key];
			Assert.AreEqual(value, o);
			Assert.AreEqual(key, ((ArrayList) map.Keys)[0]);
			Assert.AreEqual(value, ((ArrayList) map.Values)[0]);
		}
	}
}