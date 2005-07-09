using System;
using NUnit.Framework;
using Commons.Collections;
using System.Collections;

namespace NVelocity.Test.Commons {

    /// <summary>
    /// Tests for Commons.Collections.KeyedList
    /// </summary>
    [TestFixture]
    public class LRUMapTest {

	[Test]
	public void Test() {
	    LRUMap map = new LRUMap(5);
	    AssertAddedFirst(map, "One", 1);
	    AssertAddedFirst(map, "Two", 2);
	    AssertAddedFirst(map, "Three", 3);
	    AssertAddedFirst(map, "Four", 4);
	    AssertAddedFirst(map, "Five", 5);
	    AssertAddedFirst(map, "Six", 6);
	    Assertion.Assert(!map.Contains("One"));
	    AssertAddedFirst(map, "Seven", 7);
	    Assertion.Assert(!map.Contains("Two"));
	    AssertAddedFirst(map, "Eight", 8);
	    Assertion.Assert(!map.Contains("Three"));
	    AssertAddedFirst(map, "Nine", 9);
	    Assertion.Assert(!map.Contains("Four"));
	    AssertAddedFirst(map, "Ten", 10);
	    Assertion.Assert(!map.Contains("Five"));

	    map.Remove("Eight");
	    Assertion.AssertEquals(4, map.Count);
	    map.Add("One", 1);
	    Assertion.AssertEquals(5, map.Count);
	    Assertion.Assert(map.Contains("One"));
	    Assertion.Assert(map.Contains("Six"));
	    Assertion.Assert(map.Contains("Seven"));
	    Assertion.Assert(map.Contains("Nine"));
	    Assertion.Assert(map.Contains("Ten"));
	    Assertion.AssertEquals("Six", ((ArrayList)map.Keys)[map.Count-1]);
	    Assertion.AssertEquals("One", ((ArrayList)map.Keys)[0]);

	    AssertGetIsMostRecent(map, "Six", 6);
	    AssertGetIsMostRecent(map, "Nine", 9);
	    AssertGetIsMostRecent(map, "Seven", 7);
	    AssertGetIsMostRecent(map, "Ten", 10);
	    AssertGetIsMostRecent(map, "One", 1);
	    Assertion.AssertEquals("Six", ((ArrayList)map.Keys)[map.Count-1]);

	    AssertSetIsMostRecent(map, "One", "Uno");
	    AssertSetIsMostRecent(map, "Two", "Dos");
	    Assertion.AssertEquals(5, map.Count);
	}

	private void AssertAddedFirst(LRUMap map, Object key, Object value) {
	    map.Add(key, value);
	    Assertion.AssertEquals(key, ((ArrayList)map.Keys)[0]);
	    Assertion.AssertEquals(value, ((ArrayList)map.Values)[0]);
	    Assertion.Assert(map.Count <= map.MaxSize);
	}

	private void AssertSetIsMostRecent(LRUMap map, Object key, Object value) {
	    map[key] = value;
	    Assertion.AssertEquals(key, ((ArrayList)map.Keys)[0]);
	    Assertion.AssertEquals(value, ((ArrayList)map.Values)[0]);
	    Assertion.Assert(map.Count <= map.MaxSize);
	}

	private void AssertGetIsMostRecent(LRUMap map, Object key, Object value) {
	    Object o = map[key];
	    Assertion.AssertEquals(value, o);
	    Assertion.AssertEquals(key, ((ArrayList)map.Keys)[0]);
	    Assertion.AssertEquals(value, ((ArrayList)map.Values)[0]);
	}

    }
}
