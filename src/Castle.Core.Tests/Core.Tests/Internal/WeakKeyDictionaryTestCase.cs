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

#if !SILVERLIGHT && !MONO
namespace CastleTests.Core.Tests.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;
	using Castle.Core.Internal;

	[TestFixture]
	public class WeakKeyDictionaryTestCase
	{
		private WeakKeyDictionary<TKey, TValue> Dictionary;
		private KeyValuePair<TKey, TValue> Item;

		private static readonly KeyValuePair<TKey, TValue>
			OtherItem = new KeyValuePair<TKey,TValue>(new TKey(), new TValue());

		[Test]
		public void AfterExplicitTrim_LiveObject()
		{
			CreateDictionary(); AddItem();

			GC.Collect();
			Dictionary.TrimDeadObjects();

			Assert.That(Dictionary.Count,      Is.EqualTo(1));
			Assert.That(Dictionary[Item.Key],  Is.SameAs(Item.Value));
		}

		[Test]
#if __MonoCS__
		[Ignore("Expected: 0  But was: 1")]
#endif
		public void AfterExplicitTrim_DeadObject()
		{
			CreateDictionary(); AddItem(); ResetItem();

			GC.Collect();
			Dictionary.TrimDeadObjects();

			Assert.That(Dictionary.Count, Is.EqualTo(0));
		}

		[Test]
		public void AfterAutomaticTrim_LiveObject()
		{
			CreateDictionary(); AddItem();

			GC.Collect();
			TriggerAutomaticTrim();

			Assert.That(Dictionary.Count,      Is.EqualTo(1));
			Assert.That(Dictionary[Item.Key],  Is.SameAs(Item.Value));
		}

		[Test]
		public void AfterAutomaticTrim_DeadObject()
		{
			CreateDictionary(); AddItem(); ResetItem();

			GC.Collect();
			TriggerAutomaticTrim();

			Assert.That(Dictionary.Count, Is.EqualTo(0));
		}

		[Test]
		public void Initially_Count()
		{
			CreateDictionary();

			Assert.That(Dictionary.Count, Is.EqualTo(0));
		}

		[Test]
		public void Initially_Keys_Count()
		{
			CreateDictionary();

			Assert.That(Dictionary.Keys.Count, Is.EqualTo(0));
		}

		[Test]
		public void Initially_Values_Count()
		{
			CreateDictionary();

			Assert.That(Dictionary.Values.Count, Is.EqualTo(0));
		}

		[Test]
		public void Initially_ContainsKey()
		{
			CreateDictionary();

			Assert.That(Dictionary.ContainsKey(OtherItem.Key), Is.False);
		}

		[Test]
		public void Initially_Contains()
		{
			CreateDictionary();

			Assert.That(Collection.Contains(OtherItem), Is.False);
		}

		[Test]
		public void Initially_Keys_Contains()
		{
			CreateDictionary();

			Assert.That(Dictionary.Keys.Contains(OtherItem.Key), Is.False);
		}

		[Test]
		public void Initially_Values_Contains()
		{
			CreateDictionary();

			Assert.That(Dictionary.Values.Contains(OtherItem.Value), Is.False);
		}

		[Test]
		public void Initially_Indexer()
		{
			CreateDictionary();

			Assert.Throws<KeyNotFoundException>(() =>
				{ var dummy = Dictionary[OtherItem.Key]; });
		}

		[Test]
		public void Initially_TryGetValue()
		{
			CreateDictionary();

			TValue value;
			var result = Dictionary.TryGetValue(OtherItem.Key, out value);

			Assert.That(result, Is.False);
			Assert.That(value,  Is.EqualTo(default(TValue)));
		}

		[Test]
		public void Initially_Enumerator_Generic()
		{
			CreateDictionary();

			using (var e = Dictionary.GetEnumerator())
			{
				Assert.That(e,            Is.Not.Null);
				Assert.That(e.MoveNext(), Is.False);
			}
		}

		[Test]
		public void Initially_Keys_Enumerator_Generic()
		{
			CreateDictionary();

			using (var e = Dictionary.Keys.GetEnumerator())
			{
				Assert.That(e,            Is.Not.Null);
				Assert.That(e.MoveNext(), Is.False);
			}
		}

		[Test]
		public void Initially_Values_Enumerator_Generic()
		{
			CreateDictionary();

			using (var e = Dictionary.Values.GetEnumerator())
			{
				Assert.That(e,            Is.Not.Null);
				Assert.That(e.MoveNext(), Is.False);
			}
		}

		[Test]
		public void Initially_Enumerator_Nongeneric()
		{
			CreateDictionary();

			var e = AsEnumerable(Dictionary).GetEnumerator();
			Assert.That(e,            Is.Not.Null);
			Assert.That(e.MoveNext(), Is.False);
		}

		[Test]
		public void Initially_Keys_Enumerator_Nongeneric()
		{
			CreateDictionary();

			var e = AsEnumerable(Dictionary.Keys).GetEnumerator();
			Assert.That(e,            Is.Not.Null);
			Assert.That(e.MoveNext(), Is.False);
		}

		[Test]
		public void Initially_Values_Enumerator_Nongeneric()
		{
			CreateDictionary();

			var e = AsEnumerable(Dictionary.Values).GetEnumerator();
			Assert.That(e,            Is.Not.Null);
			Assert.That(e.MoveNext(), Is.False);
		}

		[Test]
		public void Initially_CopyTo()
		{
			CreateDictionary();
			var original = new[] { OtherItem, OtherItem, OtherItem };
			var modified = new[] { OtherItem, OtherItem, OtherItem };

			Dictionary.CopyTo(modified, 1);

			Assert.That(modified, Is.EquivalentTo(original));
		}

		[Test]
		public void Initially_Keys_CopyTo()
		{
			CreateDictionary();
			var original = new[] { OtherItem.Key, OtherItem.Key, OtherItem.Key };
			var modified = new[] { OtherItem.Key, OtherItem.Key, OtherItem.Key };

			Dictionary.Keys.CopyTo(modified, 1);

			Assert.That(modified, Is.EquivalentTo(original));
		}

		[Test]
		public void Initially_Values_CopyTo()
		{
			CreateDictionary();
			var original = new[] { OtherItem.Value, OtherItem.Value, OtherItem.Value };
			var modified = new[] { OtherItem.Value, OtherItem.Value, OtherItem.Value };

			Dictionary.Values.CopyTo(modified, 1);

			Assert.That(modified, Is.EquivalentTo(original));
		}

		[Test]
		public void AfterAdd_Count()
		{
			CreateDictionary(); AddItem();

			Assert.That(Dictionary.Count, Is.EqualTo(1));
		}

		[Test]
		public void AfterAdd_Keys_Count()
		{
			CreateDictionary(); AddItem();

			Assert.That(Dictionary.Keys.Count, Is.EqualTo(1));
		}

		[Test]
		public void AfterAdd_Values_Count()
		{
			CreateDictionary(); AddItem();

			Assert.That(Dictionary.Values.Count, Is.EqualTo(1));
		}

		[Test]
		public void AfterAdd_ContainsKey()
		{
			CreateDictionary(); AddItem();

			Assert.That(Dictionary.ContainsKey(Item.Key), Is.True);
		}

		[Test]
		public void AfterAdd_Contains()
		{
			CreateDictionary(); AddItem();

			Assert.That(Collection.Contains(Item), Is.True);
		}

		[Test]
		public void AfterAdd_Keys_Contains()
		{
			CreateDictionary(); AddItem();

			Assert.That(Dictionary.Keys.Contains(Item.Key), Is.True);
		}

		[Test]
		public void AfterAdd_Values_Contains()
		{
			CreateDictionary(); AddItem();

			Assert.That(Dictionary.Values.Contains(Item.Value), Is.True);
		}

		[Test]
		public void AfterAdd_Indexer()
		{
			CreateDictionary(); AddItem();

			Assert.That(Dictionary[Item.Key], Is.EqualTo(Item.Value));
		}

		[Test]
		public void AfterAdd_TryGetValue()
		{
			CreateDictionary(); AddItem();

			TValue value;
			var result = Dictionary.TryGetValue(Item.Key, out value);

			Assert.That(result, Is.True);
			Assert.That(value,  Is.EqualTo(Item.Value));
		}

		[Test]
		public void AfterAdd_Enumerator_Generic()
		{
			CreateDictionary(); AddItem();

			using (var e = Dictionary.GetEnumerator())
			{
				Assert.That(e,            Is.Not.Null);
				Assert.That(e.MoveNext(), Is.True);
				Assert.That(e.Current,    Is.EqualTo(Item));
				Assert.That(e.MoveNext(), Is.False);
			}
		}

		[Test]
		public void AfterAdd_Keys_Enumerator_Generic()
		{
			CreateDictionary(); AddItem();

			using (var e = Dictionary.Keys.GetEnumerator())
			{
				Assert.That(e,            Is.Not.Null);
				Assert.That(e.MoveNext(), Is.True);
				Assert.That(e.Current,    Is.EqualTo(Item.Key));
				Assert.That(e.MoveNext(), Is.False);
			}
		}

		[Test]
		public void AfterAdd_Values_Enumerator_Generic()
		{
			CreateDictionary(); AddItem();

			using (var e = Dictionary.Values.GetEnumerator())
			{
				Assert.That(e,            Is.Not.Null);
				Assert.That(e.MoveNext(), Is.True);
				Assert.That(e.Current,    Is.EqualTo(Item.Value));
				Assert.That(e.MoveNext(), Is.False);
			}
		}

		[Test]
		public void AfterAdd_Enumerator_Nongeneric()
		{
			CreateDictionary(); AddItem();

			var e = AsEnumerable(Dictionary).GetEnumerator();
			Assert.That(e,            Is.Not.Null);
			Assert.That(e.MoveNext(), Is.True);
			Assert.That(e.Current,    Is.EqualTo(Item));
			Assert.That(e.MoveNext(), Is.False);
		}

		[Test]
		public void AfterAdd_Keys_Enumerator_Nongeneric()
		{
			CreateDictionary(); AddItem();

			var e = AsEnumerable(Dictionary.Keys).GetEnumerator();
			Assert.That(e,            Is.Not.Null);
			Assert.That(e.MoveNext(), Is.True);
			Assert.That(e.Current,    Is.EqualTo(Item.Key));
			Assert.That(e.MoveNext(), Is.False);
		}

		[Test]
		public void AfterAdd_Values_Enumerator_Nongeneric()
		{
			CreateDictionary(); AddItem();

			var e = AsEnumerable(Dictionary.Values).GetEnumerator();
			Assert.That(e,            Is.Not.Null);
			Assert.That(e.MoveNext(), Is.True);
			Assert.That(e.Current,    Is.EqualTo(Item.Value));
			Assert.That(e.MoveNext(), Is.False);
		}

		[Test]
		public void AfterAdd_CopyTo()
		{
			CreateDictionary(); AddItem();
			var original = new[] { OtherItem, OtherItem, OtherItem };
			var modified = new[] { OtherItem, OtherItem, OtherItem };

			Dictionary.CopyTo(modified, 1);

			Assert.That(modified[0], Is.EqualTo(original[0]));
			Assert.That(modified[1], Is.EqualTo(Item));
			Assert.That(modified[2], Is.EqualTo(original[2]));
		}

		[Test]
		public void AfterAdd_Keys_CopyTo()
		{
			CreateDictionary(); AddItem();
			var original = new[] { OtherItem.Key, OtherItem.Key, OtherItem.Key };
			var modified = new[] { OtherItem.Key, OtherItem.Key, OtherItem.Key };

			Dictionary.Keys.CopyTo(modified, 1);

			Assert.That(modified[0], Is.EqualTo(original[0]));
			Assert.That(modified[1], Is.EqualTo(Item.Key));
			Assert.That(modified[2], Is.EqualTo(original[2]));
		}

		[Test]
		public void AfterAdd_Values_CopyTo()
		{
			CreateDictionary(); AddItem();
			var original = new[] { OtherItem.Value, OtherItem.Value, OtherItem.Value };
			var modified = new[] { OtherItem.Value, OtherItem.Value, OtherItem.Value };

			Dictionary.Values.CopyTo(modified, 1);

			Assert.That(modified[0], Is.EqualTo(original[0]));
			Assert.That(modified[1], Is.EqualTo(Item.Value));
			Assert.That(modified[2], Is.EqualTo(original[2]));
		}

		[Test]
		public void AfterRemove_Count()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			Assert.That(Dictionary.Count, Is.EqualTo(0));
		}

		[Test]
		public void AfterRemove_Keys_Count()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			Assert.That(Dictionary.Keys.Count, Is.EqualTo(0));
		}

		[Test]
		public void AfterRemove_Values_Count()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			Assert.That(Dictionary.Values.Count, Is.EqualTo(0));
		}

		[Test]
		public void AfterRemove_ContainsKey()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			Assert.That(Dictionary.ContainsKey(Item.Key), Is.False);
		}

		[Test]
		public void AfterRemove_Contains()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			Assert.That(Collection.Contains(Item), Is.False);
		}

		[Test]
		public void AfterRemove_Keys_Contains()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			Assert.That(Dictionary.Keys.Contains(Item.Key), Is.False);
		}

		[Test]
		public void AfterRemove_Values_Contains()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			Assert.That(Dictionary.Values.Contains(Item.Value), Is.False);
		}

		[Test]
		public void AfterRemove_Indexer()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			Assert.Throws<KeyNotFoundException>(() =>
				{ var dummy = Dictionary[Item.Key]; });
		}

		[Test]
		public void AfterRemove_TryGetValue()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			TValue value;
			var result = Dictionary.TryGetValue(Item.Key, out value);

			Assert.That(result, Is.False);
			Assert.That(value,  Is.EqualTo(default(TValue)));
		}

		[Test]
		public void AfterRemove_Enumerator_Generic()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			using (var e = Dictionary.GetEnumerator())
			{
				Assert.That(e,            Is.Not.Null);
				Assert.That(e.MoveNext(), Is.False);
			}
		}

		[Test]
		public void AfterRemove_Keys_Enumerator_Generic()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			using (var e = Dictionary.Keys.GetEnumerator())
			{
				Assert.That(e,            Is.Not.Null);
				Assert.That(e.MoveNext(), Is.False);
			}
		}

		[Test]
		public void AfterRemove_Values_Enumerator_Generic()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			using (var e = Dictionary.Values.GetEnumerator())
			{
				Assert.That(e,            Is.Not.Null);
				Assert.That(e.MoveNext(), Is.False);
			}
		}

		[Test]
		public void AfterRemove_Enumerator_Nongeneric()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			var e = AsEnumerable(Dictionary).GetEnumerator();
			Assert.That(e,            Is.Not.Null);
			Assert.That(e.MoveNext(), Is.False);
		}

		[Test]
		public void AfterRemove_Keys_Enumerator_Nongeneric()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			var e = AsEnumerable(Dictionary.Keys).GetEnumerator();
			Assert.That(e,            Is.Not.Null);
			Assert.That(e.MoveNext(), Is.False);
		}

		[Test]
		public void AfterRemove_Values_Enumerator_Nongeneric()
		{
			CreateDictionary(); AddItem(); RemoveItem();

			var e = AsEnumerable(Dictionary.Values).GetEnumerator();
			Assert.That(e,            Is.Not.Null);
			Assert.That(e.MoveNext(), Is.False);
		}

		[Test]
		public void AfterRemove_CopyTo()
		{
			CreateDictionary(); AddItem(); RemoveItem();
			var original = new[] { OtherItem, OtherItem, OtherItem };
			var modified = new[] { OtherItem, OtherItem, OtherItem };

			Dictionary.CopyTo(modified, 1);

			Assert.That(modified, Is.EquivalentTo(original));
		}

		[Test]
		public void AfterRemove_Keys_CopyTo()
		{
			CreateDictionary(); AddItem(); RemoveItem();
			var original = new[] { OtherItem.Key, OtherItem.Key, OtherItem.Key };
			var modified = new[] { OtherItem.Key, OtherItem.Key, OtherItem.Key };

			Dictionary.Keys.CopyTo(modified, 1);

			Assert.That(modified, Is.EquivalentTo(original));
		}

		[Test]
		public void AfterRemove_Values_CopyTo()
		{
			CreateDictionary(); AddItem(); RemoveItem();
			var original = new[] { OtherItem.Value, OtherItem.Value, OtherItem.Value };
			var modified = new[] { OtherItem.Value, OtherItem.Value, OtherItem.Value };

			Dictionary.Values.CopyTo(modified, 1);

			Assert.That(modified, Is.EquivalentTo(original));
		}

		[Test]
		public void AfterCollect_Count()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			// Collected items are counted until TrimDeadObjects() is called
			Assert.That(Dictionary.Count, Is.EqualTo(2));
		}

		[Test]
		public void AfterCollect_Keys_Count()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			// Collected items are counted until TrimDeadObjects() is called
			Assert.That(Dictionary.Keys.Count, Is.EqualTo(2));
		}

		[Test]
		public void AfterCollect_Values_Count()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			// Collected items are counted until TrimDeadObjects() is called
			Assert.That(Dictionary.Values.Count, Is.EqualTo(2));
		}

		[Test]
		public void AfterCollect_ContainsKey()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			Assert.That(Dictionary.ContainsKey(Item.Key), Is.True);
		}

		[Test]
		public void AfterCollect_Contains()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			Assert.That(Collection.Contains(Item), Is.True);
		}

		[Test]
		public void AfterCollect_Keys_Contains()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			Assert.That(Dictionary.Keys.Contains(Item.Key), Is.True);
		}

		[Test]
		public void AfterCollect_Values_Contains()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			Assert.That(Dictionary.Values.Contains(Item.Value), Is.True);
		}

		[Test]
		public void AfterCollect_Indexer()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			Assert.That(Dictionary[Item.Key], Is.EqualTo(Item.Value));
		}

		[Test]
		public void AfterCollect_TryGetValue()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			TValue value;
			var result = Dictionary.TryGetValue(Item.Key, out value);

			Assert.That(result, Is.True);
			Assert.That(value,  Is.EqualTo(Item.Value));
		}

		[Test]
		public void AfterCollect_Enumerator_Generic()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			using (var e = Dictionary.GetEnumerator())
			{
				Assert.That(e,            Is.Not.Null);
				Assert.That(e.MoveNext(), Is.True);
				Assert.That(e.Current,    Is.EqualTo(Item));
				Assert.That(e.MoveNext(), Is.False);
			}
		}

		[Test]
		public void AfterCollect_Keys_Enumerator_Generic()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			using (var e = Dictionary.Keys.GetEnumerator())
			{
				Assert.That(e,            Is.Not.Null);
				Assert.That(e.MoveNext(), Is.True);
				Assert.That(e.Current,    Is.EqualTo(Item.Key));
				Assert.That(e.MoveNext(), Is.False);
			}
		}

		[Test]
		public void AfterCollect_Values_Enumerator_Generic()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			using (var e = Dictionary.Values.GetEnumerator())
			{
				// Values for collected keys are present until TrimDeadObjects() is called
				Assert.That(e,            Is.Not.Null);
				Assert.That(e.MoveNext(), Is.True);
				Assert.That(e.Current,    Is.Not.EqualTo(Item.Value));
				Assert.That(e.Current,    Is.Not.EqualTo(OtherItem.Value));
				Assert.That(e.MoveNext(), Is.True);
				Assert.That(e.Current,    Is.EqualTo(Item.Value));
				Assert.That(e.MoveNext(), Is.False);
			}
		}

		[Test]
		public void AfterCollect_Enumerator_Nongeneric()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			var e = AsEnumerable(Dictionary).GetEnumerator();
			Assert.That(e,            Is.Not.Null);
			Assert.That(e.MoveNext(), Is.True);
			Assert.That(e.Current,    Is.EqualTo(Item));
			Assert.That(e.MoveNext(), Is.False);
		}

		[Test]
		public void AfterCollect_Keys_Enumerator_Nongeneric()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			var e = AsEnumerable(Dictionary.Keys).GetEnumerator();
			Assert.That(e,            Is.Not.Null);
			Assert.That(e.MoveNext(), Is.True);
			Assert.That(e.Current,    Is.EqualTo(Item.Key));
			Assert.That(e.MoveNext(), Is.False);
		}

		[Test]
		public void AfterCollect_Values_Enumerator_Nongeneric()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();

			// Values for collected keys are present until TrimDeadObjects() is called
			var e = AsEnumerable(Dictionary.Values).GetEnumerator();
			Assert.That(e,            Is.Not.Null);
			Assert.That(e.MoveNext(), Is.True);
			Assert.That(e.Current,    Is.Not.EqualTo(Item.Value));
			Assert.That(e.Current,    Is.Not.EqualTo(OtherItem.Value));
			Assert.That(e.MoveNext(), Is.True);
			Assert.That(e.Current,    Is.EqualTo(Item.Value));
			Assert.That(e.MoveNext(), Is.False);
		}

		[Test]
		public void AfterCollect_CopyTo()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();
			var original = new[] { OtherItem, OtherItem, OtherItem };
			var modified = new[] { OtherItem, OtherItem, OtherItem };

			Dictionary.CopyTo(modified, 1);

			Assert.That(modified[0], Is.EqualTo(original[0]));
			Assert.That(modified[1], Is.EqualTo(Item));
			Assert.That(modified[2], Is.EqualTo(original[2]));
		}

		[Test]
		public void AfterCollect_Keys_CopyTo()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();
			var original = new[] { OtherItem.Key, OtherItem.Key, OtherItem.Key };
			var modified = new[] { OtherItem.Key, OtherItem.Key, OtherItem.Key };

			Dictionary.Keys.CopyTo(modified, 1);

			Assert.That(modified[0], Is.EqualTo(original[0]));
			Assert.That(modified[1], Is.EqualTo(Item.Key));
			Assert.That(modified[2], Is.EqualTo(original[2]));
		}

		[Test]
		public void AfterCollect_Values_CopyTo()
		{
			CreateDictionary(); AddItem(); ResetItem(); AddItem(); GC.Collect();
			var original = new[] { OtherItem.Value, OtherItem.Value, OtherItem.Value };
			var modified = new[] { OtherItem.Value, OtherItem.Value, OtherItem.Value };

			Dictionary.Values.CopyTo(modified, 1);

			// Values for collected keys are present until TrimDeadObjects() is called
			Assert.That(modified[0], Is    .EqualTo(original[0]));
			Assert.That(modified[1], Is.Not.EqualTo(original[1]));
			Assert.That(modified[1], Is.Not.EqualTo(Item.Value));
			Assert.That(modified[2], Is    .EqualTo(Item.Value));
		}

		[Test]
		public void IsReadOnly()
		{
			CreateDictionary();

			Assert.That(Collection.IsReadOnly, Is.False);
		}

		[Test]
		public void Keys_IsReadOnly()
		{
			CreateDictionary();

			Assert.That(Dictionary.Keys.IsReadOnly, Is.True);
		}

		[Test]
		public void Values_IsReadOnly()
		{
			CreateDictionary();

			Assert.That(Dictionary.Values.IsReadOnly, Is.True);
		}

		[Test]
		public void Keys_Add()
		{
			CreateDictionary();

			Assert.Throws<NotSupportedException>(() => Dictionary.Keys.Add(OtherItem.Key));
		}

		[Test]
		public void Values_Add()
		{
			CreateDictionary();

			Assert.Throws<NotSupportedException>(() => Dictionary.Values.Add(OtherItem.Value));
		}

		[Test]
		public void Keys_Remove()
		{
			CreateDictionary();

			Assert.Throws<NotSupportedException>(() => Dictionary.Keys.Remove(OtherItem.Key));
		}

		[Test]
		public void Values_Remove()
		{
			CreateDictionary();

			Assert.Throws<NotSupportedException>(() => Dictionary.Values.Remove(OtherItem.Value));
		}

		[Test]
		public void Keys_Clear()
		{
			CreateDictionary();

			Assert.Throws<NotSupportedException>(() => Dictionary.Keys.Clear());
		}

		[Test]
		public void Values_Clear()
		{
			CreateDictionary();

			Assert.Throws<NotSupportedException>(() => Dictionary.Values.Clear());
		}

		private void CreateDictionary()
		{
			Dictionary = new WeakKeyDictionary<TKey, TValue>();
		}

		private void AddItem()
		{
			Item = new KeyValuePair<TKey, TValue>(new TKey(), new TValue());
			Dictionary.Add(Item.Key, Item.Value);
		}

		private void RemoveItem()
		{
			Dictionary.Remove(Item.Key);
		}

		private void ResetItem()
		{
			Item = default(KeyValuePair<TKey, TValue>);
		}

		private void TriggerAutomaticTrim()
		{
			int dummy;
			for (var i = 0; i < 128; i++)
				dummy = Dictionary.Count;
		}

		[TearDown]
		public void TearDown()
		{
			Dictionary = null;
			ResetItem();
		}

		private ICollection<KeyValuePair<TKey, TValue>> Collection
		{
			get { return (ICollection<KeyValuePair<TKey, TValue>>) Dictionary; }
		}

		private static IEnumerable AsEnumerable<T>(ICollection<T> collection)
		{
			return (IEnumerable) collection;
		}

		private sealed class TKey   { }
		private sealed class TValue { }
	}
}
#endif