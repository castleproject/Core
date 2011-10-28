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

			Assert.That(s, Is.Not.Null & Is.InstanceOf<XmlNodeSet<string>>());
		}

		[Test]
		public void Count()
		{
			var s = GetSetOfStrings();

			Assert.That(s.Count, Is.EqualTo(Strings.Count));
		}

		[Test]
		public void Count_AfterAdd_Nonexisting()
		{
			var s = GetSetOfStrings();

			s.Add("added");

			Assert.That(s.Count, Is.EqualTo(Strings.Count + 1));
		}

		[Test]
		public void Count_AfterAdd_Existing()
		{
			var s = GetSetOfStrings();

			s.Add(Strings.First());

			Assert.That(s.Count, Is.EqualTo(Strings.Count));
		}

		[Test]
		public void Count_AfterRemove_Nonexisting()
		{
			var s = GetSetOfStrings();

			s.Remove("other");

			Assert.That(s.Count, Is.EqualTo(Strings.Count));
		}

		[Test]
		public void Count_AfterRemove_Existing()
		{
			var s = GetSetOfStrings();

			s.Remove(Strings.First());

			Assert.That(s.Count, Is.EqualTo(Strings.Count - 1));
		}

		[Test]
		public void Count_AfterClear()
		{
			var s = GetSetOfStrings();

			s.Clear();

			Assert.That(s.Count, Is.EqualTo(0));
		}

		[Test]
		public void Contains_Populated_True()
		{
			var s = GetSetOfStrings();

			Assert.That(s.Contains(Strings[0]), Is.True);
		}

		[Test]
		public void Contains_Populated_False()
		{
			var s = GetSetOfStrings();

			Assert.That(s.Contains("unknown"), Is.False);
		}

		[Test]
		public void Contains_Added()
		{
			var s = GetSetOfStrings();

			s.Add("added");

			Assert.That(s.Contains("added"), Is.True);
		}

		[Test]
		public void Contains_Changed()
		{
			var s = GetSetOfStrings();
			var l = (IList<string>) s;

			var v = l[0];
			l[0] = "other";

			Assert.That(s.Contains("other"), Is.True);
			Assert.That(s.Contains(v),       Is.False);
		}

		[Test]
		public void Contains_Removed()
		{
			var s = GetSetOfStrings();

			s.Remove(Strings.First());

			Assert.That(s.Contains(Strings.First()), Is.False);
		}

		[Test]
		public void Contains_RemovedAt()
		{
			var s = GetSetOfStrings();
			var l = (IList<string>) s;

			var v = l[0];
			l.RemoveAt(0);

			Assert.That(s.Contains(v), Is.False);
		}

		[Test]
		public void Contains_Cleared()
		{
			var s = GetSetOfStrings();

			s.Clear();

			Assert.That(s.Contains(Strings.First()), Is.False);
		}

		[Test]
		public void IsSubsetOf_True()
		{
			var s = GetSetOfStrings();
			var o = Strings;

			Assert.That(s.IsSubsetOf(o), Is.True);
		}

		[Test]
		public void IsSubsetOf_False()
		{
			var s = GetSetOfStrings();
			var o = Strings.Skip(1);

			Assert.That(s.IsSubsetOf(o), Is.False);
		}

		[Test]
		public void IsSupersetOf_True()
		{
			var s = GetSetOfStrings();
			var o = Strings;

			Assert.That(s.IsSupersetOf(o), Is.True);
		}

		[Test]
		public void IsSupersetOf_False()
		{
			var s = GetSetOfStrings();
			var o = Strings.Concat(new[] { "other" });

			Assert.That(s.IsSupersetOf(o), Is.False);
		}

		[Test]
		public void IsProperSubsetOf_True()
		{
			var s = GetSetOfStrings();
			var o = Strings.Concat(new[] { "other" });

			Assert.That(s.IsProperSubsetOf(o), Is.True);
		}

		[Test]
		public void IsProperSubsetOf_False()
		{
			var s = GetSetOfStrings();
			var o = Strings;

			Assert.That(s.IsProperSubsetOf(o), Is.False);
		}

		[Test]
		public void IsProperSupersetOf_True()
		{
			var s = GetSetOfStrings();
			var o = Strings.Skip(1);

			Assert.That(s.IsProperSupersetOf(o), Is.True);
		}

		[Test]
		public void IsProperSupersetOf_False()
		{
			var s = GetSetOfStrings();
			var o = Strings;

			Assert.That(s.IsProperSupersetOf(o), Is.False);
		}

		[Test]
		public void Overlaps_True()
		{
			var s = GetSetOfStrings();
			var o = Strings.Take(1).Concat(new[] { "other" });

			Assert.That(s.Overlaps(o), Is.True);
		}

		[Test]
		public void Overlaps_False()
		{
			var s = GetSetOfStrings();
			var o = new[] { "other" };

			Assert.That(s.Overlaps(o), Is.False);
		}

		[Test]
		public void SetEquals_True()
		{
			var s = GetSetOfStrings();
			var o = Strings.Reverse();

			Assert.That(s.SetEquals(o), Is.True);
		}

		[Test]
		public void SetEquals_False()
		{
			var s = GetSetOfStrings();
			var o = Strings.Take(1);

			Assert.That(s.SetEquals(o), Is.False);
		}

		[Test]
		public void GetEnumerator_Generic()
		{
			var s = GetSetOfStrings() as IEnumerable<string>;
			var x = new List<string>();

			using (var e = s.GetEnumerator())
				while (e.MoveNext())
					x.Add(e.Current);

			Assert.That(x, Is.EquivalentTo(Strings));
		}

		[Test]
		public void GetEnumerator_NonGeneric()
		{
			var s = GetSetOfStrings() as IEnumerable;
			var x = new ArrayList();

			var e = s.GetEnumerator();
				while (e.MoveNext())
					x.Add(e.Current as string);

			Assert.That(x, Is.EquivalentTo(Strings));
		}

		[Test]
		public void UnionWith()
		{
			var s = GetSetOfStrings();
			var t = new[] { "other" };

			s.UnionWith(t);

			Assert.That(s.ToArray(), Is.EquivalentTo(Strings.Union(t)));
		}

		[Test]
		public void ExceptWith()
		{
			var s = GetSetOfStrings();
			var t = Strings.Take(1);

			s.ExceptWith(t);

			Assert.That(s.ToArray(), Is.EquivalentTo(Strings.Except(t)));
		}

		[Test]
		public void IntersectWith()
		{
			var s = GetSetOfStrings();
			var t = Strings.Take(1).Concat(new[] { "other" });

			s.IntersectWith(t);

			Assert.That(s.ToArray(), Is.EquivalentTo(Strings.Intersect(t)));
		}

		[Test]
		public void SymmetricExceptWith()
		{
			var s = GetSetOfStrings();
			var t = Strings.Take(1).Concat(new[] { "other" });

			s.SymmetricExceptWith(t);

			var expected = Strings.Skip(1).Concat(new[] { "other" });
			Assert.That(s.ToArray(), Is.EquivalentTo(expected));
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
