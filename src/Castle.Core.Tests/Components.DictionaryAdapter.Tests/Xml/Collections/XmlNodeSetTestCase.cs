// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if DOTNET40
namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;

	[TestFixture]
	public class XmlNodeSetTestCase : XmlAdapterTestCase
	{
		[Test]
		public void GetValue()
		{
			var s = GetSetOfStrings();

			Assert.IsNotNull(s);
			Assert.IsInstanceOf<XmlNodeSet<string>>(s);
		}

		[Test]
		public void Count()
		{
			var s = GetSetOfStrings();

			Assert.AreEqual(Strings.Count, s.Count);
		}

		[Test]
		public void Count_AfterAdd_Nonexisting()
		{
			var s = GetSetOfStrings();

			s.Add("added");

			Assert.AreEqual(Strings.Count + 1, s.Count);
		}

		[Test]
		public void Count_AfterAdd_Existing()
		{
			var s = GetSetOfStrings();

			s.Add(Strings.First());

			Assert.AreEqual(Strings.Count, s.Count);
		}

		[Test]
		public void Count_AfterRemove_Nonexisting()
		{
			var s = GetSetOfStrings();

			s.Remove("other");

			Assert.AreEqual(Strings.Count, s.Count);
		}

		[Test]
		public void Count_AfterRemove_Existing()
		{
			var s = GetSetOfStrings();

			s.Remove(Strings.First());

			Assert.AreEqual(Strings.Count - 1, s.Count);
		}

		[Test]
		public void Count_AfterClear()
		{
			var s = GetSetOfStrings();

			s.Clear();

			Assert.AreEqual(0, s.Count);
		}

		[Test]
		public void Contains_Populated_True()
		{
			var s = GetSetOfStrings();

			Assert.True(s.Contains(Strings[0]));
		}

		[Test]
		public void Contains_Populated_False()
		{
			var s = GetSetOfStrings();

			Assert.False(s.Contains("unknown"));
		}

		[Test]
		public void Contains_Added()
		{
			var s = GetSetOfStrings();

			s.Add("added");

			Assert.True(s.Contains("added"));
		}

		[Test]
		public void Contains_Changed()
		{
			var s = GetSetOfStrings();
			var l = (IList<string>) s;

			var v = l[0];
			l[0] = "other";

			Assert.True(s.Contains("other"));
			Assert.False(s.Contains(v));
		}

		[Test]
		public void Contains_Removed()
		{
			var s = GetSetOfStrings();

			s.Remove(Strings.First());

			Assert.False(s.Contains(Strings.First()));
		}

		[Test]
		public void Contains_RemovedAt()
		{
			var s = GetSetOfStrings();
			var l = (IList<string>) s;

			var v = l[0];
			l.RemoveAt(0);

			Assert.False(s.Contains(v));
		}

		[Test]
		public void Contains_Cleared()
		{
			var s = GetSetOfStrings();

			s.Clear();

			Assert.False(s.Contains(Strings.First()));
		}

		[Test]
		public void IsSubsetOf_True()
		{
			var s = GetSetOfStrings();
			var o = Strings;

			Assert.True(s.IsSubsetOf(o));
		}

		[Test]
		public void IsSubsetOf_False()
		{
			var s = GetSetOfStrings();
			var o = Strings.Skip(1);

			Assert.False(s.IsSubsetOf(o));
		}

		[Test]
		public void IsSupersetOf_True()
		{
			var s = GetSetOfStrings();
			var o = Strings;

			Assert.True(s.IsSupersetOf(o));
		}

		[Test]
		public void IsSupersetOf_False()
		{
			var s = GetSetOfStrings();
			var o = Strings.Concat(new[] { "other" });

			Assert.False(s.IsSupersetOf(o));
		}

		[Test]
		public void IsProperSubsetOf_True()
		{
			var s = GetSetOfStrings();
			var o = Strings.Concat(new[] { "other" });

			Assert.True(s.IsProperSubsetOf(o));
		}

		[Test]
		public void IsProperSubsetOf_False()
		{
			var s = GetSetOfStrings();
			var o = Strings;

			Assert.False(s.IsProperSubsetOf(o));
		}

		[Test]
		public void IsProperSupersetOf_True()
		{
			var s = GetSetOfStrings();
			var o = Strings.Skip(1);

			Assert.True(s.IsProperSupersetOf(o));
		}

		[Test]
		public void IsProperSupersetOf_False()
		{
			var s = GetSetOfStrings();
			var o = Strings;

			Assert.False(s.IsProperSupersetOf(o));
		}

		[Test]
		public void Overlaps_True()
		{
			var s = GetSetOfStrings();
			var o = Strings.Take(1).Concat(new[] { "other" });

			Assert.True(s.Overlaps(o));
		}

		[Test]
		public void Overlaps_False()
		{
			var s = GetSetOfStrings();
			var o = new[] { "other" };

			Assert.False(s.Overlaps(o));
		}

		[Test]
		public void SetEquals_True()
		{
			var s = GetSetOfStrings();
			var o = Strings.Reverse();

			Assert.True(s.SetEquals(o));
		}

		[Test]
		public void SetEquals_False()
		{
			var s = GetSetOfStrings();
			var o = Strings.Take(1);

			Assert.False(s.SetEquals(o));
		}

		[Test]
		public void GetEnumerator_Generic()
		{
			var s = GetSetOfStrings() as IEnumerable<string>;
			var x = new List<string>();

			using (var e = s.GetEnumerator())
				while (e.MoveNext())
					x.Add(e.Current);

			CollectionAssert.AreEquivalent(Strings, x);
		}

		[Test]
		public void GetEnumerator_NonGeneric()
		{
			var s = GetSetOfStrings() as IEnumerable;
			var x = new ArrayList();

			var e = s.GetEnumerator();
				while (e.MoveNext())
					x.Add(e.Current as string);

			CollectionAssert.AreEquivalent(Strings, x);
		}

		[Test]
		public void UnionWith()
		{
			var s = GetSetOfStrings();
			var t = new[] { "other" };

			s.UnionWith(t);

			CollectionAssert.AreEquivalent(Strings.Union(t), s.ToArray());
		}

		[Test]
		public void ExceptWith()
		{
			var s = GetSetOfStrings();
			var t = Strings.Take(1);

			s.ExceptWith(t);

			CollectionAssert.AreEquivalent(Strings.Except(t), s.ToArray());
		}

		[Test]
		public void IntersectWith()
		{
			var s = GetSetOfStrings();
			var t = Strings.Take(1).Concat(new[] { "other" });

			s.IntersectWith(t);

			CollectionAssert.AreEquivalent(Strings.Intersect(t), s.ToArray());
		}

		[Test]
		public void SymmetricExceptWith()
		{
			var s = GetSetOfStrings();
			var t = Strings.Take(1).Concat(new[] { "other" });

			s.SymmetricExceptWith(t);

			CollectionAssert.AreEquivalent(Strings.Skip(1).Concat(new[] { "other" }), s.ToArray());
		}

		private static readonly IList<string>
			Strings = Array.AsReadOnly(new[] { "a", "b" });

		private ISet<string> GetEmptySet()
		{
			var f = Create<IFoo>("<Foo/>");
			return f.Strings;
		}

		private ISet<string> GetSetOfStrings()
		{
			var f = Create<IFoo>
			(
				"<Foo>",
					"<Strings>",
						"<string>a</string>",
						"<string>b</string>",
					"</Strings>",
				"</Foo>"
			);

			return f.Strings;
		}

		public interface IFoo : IDictionaryAdapter
		{
			ISet<string> Strings { get; set; }
		}
	}
}
#endif
