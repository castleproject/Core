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

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	using Castle.Core;
	using Castle.Core.Internal;

	public class XmlReferenceManager
	{
		private readonly        Dictionary<int,    Entry> entriesById;
		private readonly WeakKeyDictionary<object, Entry> entriesByValue;
		private readonly IXmlReferenceFormat format;
		private int nextId;

		public XmlReferenceManager(IXmlNode root, IXmlReferenceFormat format)
		{
			entriesById    = new        Dictionary<int,    Entry>();
			entriesByValue = new WeakKeyDictionary<object, Entry>(ReferenceEqualityComparer<object>.Instance);
			this.format    = format;
			this.nextId    = 1;

			Populate(root);
		}

		#region Populate

		private void Populate(IXmlNode node)
		{
			var references = new List<Reference>();
			var iterator   = node.SelectSubtree();

			while (iterator.MoveNext())
				PopulateFromNode(iterator, references);

			PopulateDeferredReferences(references);
		}

		private void PopulateFromNode(IXmlIterator node, ICollection<Reference> references)
		{
			int id;
			if (format.TryGetIdentity(node, out id))
				PopulateIdentity(id, node.Save());
			else if (format.TryGetReference(node, out id))
				PopulateReference(id, node.Save(), references);
		}

		private void PopulateIdentity(int id, IXmlNode node)
		{
			Entry entry;
			if (!entriesById.TryGetValue(id, out entry))
				entriesById.Add(id, new Entry(id, node));
			if (nextId <= id)
				nextId = ++id;
		}

		private void PopulateReference(int id, IXmlNode node, ICollection<Reference> references)
		{
			Entry entry;
			if (entriesById.TryGetValue(id, out entry))
				entry.AddReference(node);
			else
				references.Add(new Reference(id, node));
		}

		private void PopulateDeferredReferences(ICollection<Reference> references)
		{
			foreach (var reference in references)
			{
				Entry entry;
				if (entriesById.TryGetValue(reference.Id, out entry))
					entry.AddReference(reference.Node);
			}
		}

		#endregion

		public bool TryGet(object keyObject, out object inGraphObject)
		{
			Entry entry;
			if (entriesByValue.TryGetValue(keyObject, out entry))
			{
				inGraphObject = keyObject;
				TryGetCompatibleValue(entry, keyObject.GetComponentType(), ref inGraphObject);
				return true;
			}
			else
			{
				inGraphObject = null;
				return false;
			}
		}

		public void Add(IXmlNode node, object keyValue, object newValue, bool isInGraph)
		{
			if (keyValue == null)
				throw Error.ArgumentNull("keyValue");
			if (newValue == null)
				throw Error.ArgumentNull("newValue");

            var type = newValue.GetComponentType();
			if (ShouldExclude(type))
				return;
			if (entriesByValue.ContainsKey(newValue))
				return;

			Entry entry;
			if (entriesByValue.TryGetValue(keyValue, out entry))
			{
				if (newValue == keyValue)
					return;
			}
			else if (node != null)
			{
				bool reference;
				if (!TryGetEntry(node, out entry, out reference))
					entry = new Entry(node);
			}
			else return;

			AddValueCore(entry, type, newValue, isInGraph);
		}

		public bool OnGetStarting(ref IXmlNode node, ref object value, out object token)
		{
			Entry entry;
			bool isReference;

			var type = node.ClrType;
			if (ShouldExclude(type))
				{ token = null; return true; }

			if (!TryGetEntry(node, out entry, out isReference))
				{ token = CreateEntryToken; return true; }

			if (isReference)
				RedirectNode(ref node, entry);

			var proceed = ! TryGetCompatibleValue(entry, node.ClrType, ref value);

			token = proceed ? entry : null;
			return proceed;
		}

		public void OnGetCompleted(IXmlNode node, object value, object token)
		{
			if (value == null)
				return;

			var type = node.ClrType;
			if (ShouldExclude(type))
				return;

			if (entriesByValue.ContainsKey(value))
				return;

			var entry = (token == CreateEntryToken)
				? new Entry(node)
				: token as Entry;
			if (entry == null)
				return;

			AddValue(entry, type, value, null);
		}

		public bool OnAssigningNull(IXmlNode node, object oldValue)
		{
			object token, newValue = null;
			return OnAssigningValue(node, oldValue, ref newValue, out token);
		}

        public bool OnAssigningValue(IXmlNode node, object oldValue, ref object newValue, out object token)
        {
			if (newValue == oldValue && newValue != null)
				{ token = null; return false; }

			var oldEntry = OnReplacingValue(node, oldValue);

			if (newValue == null)
				return ShouldAssignmentProceed(oldEntry, null, token = null);

            var type = newValue.GetComponentType();
			if (ShouldExclude(type))
				return ShouldAssignmentProceed(oldEntry, null, token = null);

			var xmlAdapter = XmlAdapter.For(newValue, false);

			Entry newEntry;
            if (entriesByValue.TryGetValue(xmlAdapter ?? newValue, out newEntry))
            {
                // Value already present in graph; add reference
				TryGetCompatibleValue(newEntry, type, ref newValue);
				AddReference(node, newEntry);
				token = null;
            }
            else
            {
				// Value not present in graph; add as primary
				newEntry = oldEntry ?? new Entry(node);
				AddValue(newEntry, type, newValue, xmlAdapter);
				format.ClearIdentity (node);
				format.ClearReference(node);
				token = newEntry;
            }
			return ShouldAssignmentProceed(oldEntry, newEntry, token);
        }

		private bool ShouldAssignmentProceed(Entry oldEntry, Entry newEntry, object token)
		{
			if (oldEntry != null && oldEntry != newEntry && oldEntry.Id > 0)
				entriesById.Remove(oldEntry.Id); // Didn't reuse old entry; delete it

			return token    != null  // Expecting callback with a token, so proceed with set
				|| newEntry == null; // No reference tracking for this value; don't prevent assignment
		}

		private Entry OnReplacingValue(IXmlNode node, object oldValue)
		{
			Entry entry;
			bool isReference;

			if (oldValue == null)
			{
				if (!TryGetEntry(node, out entry, out isReference))
					return null;
			}
			else
			{
				if (!entriesByValue.TryGetValue(oldValue, out entry))
					return null;
				isReference = !entry.Node.PositionEquals(node);
			}

			if (isReference)
			{
				// Replacing reference
				entry.RemoveReference(node);
				ClearReference(entry, node);
				return null;
			}
			else if (entry.References != null)
			{
				// Replacing primary that has references
				// Relocate content to a referencing node (making it a new primary)
				node = entry.RemoveReference(0);
				ClearReference(entry, node);
				entry.Node.CopyTo(node);
				entry.Node.Clear();
				entry.Node = node;
				return null;
			}
			else
			{
				// Replaceing primary with no references; reuse entry
				PrepareForReuse(entry);
				return entry;
			}
		}

		public void OnAssignedValue(IXmlNode node, object givenValue, object storedValue, object token)
		{
			var entry = token as Entry;
			if (entry == null)
				return;

			if (ReferenceEquals(givenValue, storedValue))
				return;

			SetNotInGraph(entry, givenValue);

			if (entriesByValue.ContainsKey(storedValue))
				return;

			AddValue(entry, node.ClrType, storedValue, null);
		}

		private void AddReference(IXmlNode node, Entry entry)
		{
			if (!entry.Node.PositionEquals(node))
			{
				if (entry.References == null)
				{
					GenerateId(entry);
					format.SetIdentity(entry.Node, entry.Id);
				}
				node.Clear();
				entry.AddReference(node);
				format.SetReference(node, entry.Id);
			}
		}

		private void GenerateId(Entry entry)
		{
			if (entry.Id == 0)
			{
				entry.Id = nextId++;
				entriesById.Add(entry.Id, entry);
			}
		}

		private void AddValue(Entry entry, Type type, object value, XmlAdapter xmlAdapter)
		{
			if (xmlAdapter == null)
				xmlAdapter = XmlAdapter.For(value, false);

			AddValueCore(entry, type, value, true);

			if (xmlAdapter != null)
				AddValueCore(entry, typeof(XmlAdapter), xmlAdapter, true);
		}

		private void AddValueCore(Entry entry, Type type, object value, bool isInGraph)
		{
			entry.AddValue(type, value, isInGraph);
			entriesByValue.Add(value, entry);
		}

		private void ClearReference(Entry entry, IXmlNode node)
		{
			format.ClearReference(node);

			if (entry.References == null)
				format.ClearIdentity(entry.Node);
		}

		private void PrepareForReuse(Entry entry)
		{
			foreach (var item in entry.Values)
			{
				var value = item.Value.Target;
				if (null != value)
					entriesByValue.Remove(value);
			}
			entry.Values.Clear();

			format.ClearIdentity(entry.Node);
		}

		private bool TryGetEntry(IXmlNode node, out Entry entry, out bool reference)
		{
			int id;

			if (format.TryGetIdentity(node, out id))
				reference = false;
			else if (format.TryGetReference(node, out id))
				reference = true;
			else
			{
				reference = false;
				entry = null;
				return false;
			}

			if (!entriesById.TryGetValue(id, out entry))
				throw IdNotFoundError(id);
			return true;
		}

		private bool TryGetCompatibleValue(Entry entry, Type type, ref object value)
		{
			var values = entry.Values;
			if (values == null)
				return false;

			var dictionaryAdapter = null as IDictionaryAdapter;

			// Try to find in the graph a directly assignable value
			foreach (var item in values)
			{
				if (!item.IsInGraph)
					continue;

				var candidate = item.Value.Target;
				if (candidate == null)
					continue;

				if (type.IsAssignableFrom(item.Type))
					if (null != candidate)
						return Try.Success(out value, candidate);

				if (dictionaryAdapter == null)
					dictionaryAdapter = candidate as IDictionaryAdapter;
			}

			// Fall back to coercing a DA found in the graph
			if (dictionaryAdapter != null)
			{
				value = dictionaryAdapter.Coerce(type);
				entry.AddValue(type, value, true);
				return true;
			}

			return false;
		}

		private static void SetNotInGraph(Entry entry, object value)
		{
			var xmlAdapter = XmlAdapter.For(value, false);

			SetNotInGraphCore(entry, value);

			if (xmlAdapter != null)
				SetNotInGraphCore(entry, xmlAdapter);
		}

		private static bool ShouldExclude(Type type)
		{
			return type.GetTypeInfo().IsValueType
				|| type == StringType;
		}

		private static void SetNotInGraphCore(Entry entry, object value)
		{
			var values = entry.Values;
			for (int index = 0; index < values.Count; index++)
			{
				var item      = values[index];
				var candidate = item.Value.Target;

				if (ReferenceEquals(candidate, value))
				{
					item = new EntryValue(item.Type, item.Value, false);
					values[index] = item;
					return;
				}
			}
		}

		private static IXmlNode RedirectNode(ref IXmlNode node, Entry entry)
		{
			var cursor = entry.Node.SelectSelf(node.ClrType);
			cursor.MoveNext();
			return node = cursor;
		}

		public void UnionWith(XmlReferenceManager other)
		{
			var visited = null as HashSet<Entry>;

			foreach (var otherEntry in other.entriesByValue)
			{
				Entry thisEntry;
				if (entriesByValue.TryGetValue(otherEntry.Key, out thisEntry))
				{
					if (visited == null)
						visited = new HashSet<Entry>(ReferenceEqualityComparer<Entry>.Instance);
					else if (visited.Contains(thisEntry))
						continue;
					visited.Add(thisEntry);

					foreach (var otherValue in otherEntry.Value.Values)
					{
						var otherTarget = otherValue.Value.Target;
						if (otherTarget == null           ||
							otherTarget == otherEntry.Key ||
							entriesByValue.ContainsKey(otherTarget))
							{ continue; }
						AddValueCore(thisEntry, otherValue.Type, otherTarget, false);
					}
				}
			}
		}

		private static readonly Type
			StringType = typeof(string);

		private static readonly object
			CreateEntryToken = new object();

		private class Entry
		{
			public int Id;
			public IXmlNode  Node;
			private List<IXmlNode> references;
			private List<EntryValue> values;

			public Entry(IXmlNode node)
			{
				Node = node.Save();
			}

			public Entry(int id, IXmlNode node) : this(node)
			{
				Id = id;
			}

			public void AddReference(IXmlNode node)
			{
				if (references == null)
					references = new List<IXmlNode>();
				references.Add(node);
			}

			public IXmlNode RemoveReference(IXmlNode node)
			{
				for (var index = 0; index < references.Count; index++)
					if (references[index].PositionEquals(node))
						return RemoveReference(index);
				return node;
			}

			public IXmlNode RemoveReference(int index)
			{
				var node = references[index];
				references.RemoveAt(index);
				if (references.Count == 0)
					references = null;
				return node;
			}

			public void AddValue(Type type, object value, bool isInGraph)
			{
				if (values == null)
					values = new List<EntryValue>();
				values.Add(new EntryValue(type, value, isInGraph));
			}

			public List<IXmlNode> References
			{
				get { return references; }
			}

			public List<EntryValue> Values
			{
				get { return values; }
			}
		}

		private struct Reference
		{
			public readonly int      Id;
			public readonly IXmlNode Node;

			public Reference(int id, IXmlNode node)
			{
				Id   = id;
				Node = node;
			}
		}

		private struct EntryValue
		{
			public readonly Type          Type;
			public readonly WeakReference Value;
			public readonly	bool          IsInGraph;

			public EntryValue(Type type, object value, bool isInGraph)
				: this(type, new WeakReference(value), isInGraph) { }

			public EntryValue(Type type, WeakReference value, bool isInGraph)
			{
				Type      = type;
				Value     = value;
				IsInGraph = isInGraph;
			}
		}

		private static Exception IdNotFoundError(int id)
		{
			var message = string.Format
			(
				"The given ID ({0}) was not present in the underlying data.",
				id
			);
			return new KeyNotFoundException(message);
		}
	}
}
#endif
