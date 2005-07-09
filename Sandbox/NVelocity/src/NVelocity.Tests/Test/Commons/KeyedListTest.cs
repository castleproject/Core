using KeyedList = Commons.Collections.KeyedList;

namespace NVelocity.Test.Commons
{
	using System;
	using System.Collections;
	using NUnit.Framework;

	/// <summary>
	/// Tests for Commons.Collections.KeyedList
	/// </summary>
	[TestFixture]
	public class KeyedListTest
	{
		[Test]
		public void Test()
		{
			KeyedList k = new KeyedList();
			k.Add("One", 1);
			k.Add("Two", 2);
			k.Add("Three", 3);
			k.Add("Four", 4);
			k.Add("Five", 5);
			k.Add("Six", 6);
			k.Add("Seven", 7);
			k.Add("Eight", 8);
			k.Add("Nine", 9);
			k.Add("Ten", 10);

			AssertCollectionsAreInSync(k);
			k.Remove("Seven");
			AssertCollectionsAreInSync(k);
			k.RemoveAt(0);
			AssertCollectionsAreInSync(k);
			k.Add("last", 11);
			Assertion.AssertEquals(11, k[k.Count - 1]);
			k.Remove("last");
			Assertion.AssertEquals(10, k[k.Count - 1]);
			k.Insert(0, "One", 1);
			AssertCollectionsAreInSync(k);
			k.Insert(6, "Seven", 7);

			// make sure the 
			Int32 i = 1;
			foreach (DictionaryEntry entry in k)
			{
				Assertion.AssertEquals(i, entry.Value);
				i++;
			}
		}

		private void AssertCollectionsAreInSync(KeyedList k)
		{
			Int32 i = 0;
			foreach (DictionaryEntry entry in k)
			{
				Object value = k[i];
				Object key = ((ArrayList) k.Keys)[i];
				Assertion.AssertEquals(entry.Key, key);
				Assertion.AssertEquals(entry.Value, value);
				i++;
			}
		}

	}
}