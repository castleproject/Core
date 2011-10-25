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

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using Castle.Core;

	public class XmlReferenceManager
	{
		private readonly Dictionary<int,    Entry> entriesById;
		private readonly Dictionary<object, Entry> entriesByValue;
		private readonly IXmlReferenceFormat format;
		private int nextId;

		public XmlReferenceManager(IXmlNode root, IXmlReferenceFormat format)
		{
			entriesById    = new Dictionary<int,    Entry>();
			entriesByValue = new Dictionary<object, Entry>(ReferenceEqualityComparer<object>.Instance);
			this.format    = format;
			this.nextId    = 1;

			Populate(root);
		}

		private void Populate(IXmlNode node)
		{
			var references = new List<Reference>();
			var iterator   = node.SelectSubtree();

			while (iterator.MoveNext())
				PopulateFromNode(iterator, references);

			AddReferencesLate(references);
		}

		private void PopulateFromNode(IXmlNode node, ICollection<Reference> references)
		{
			int id;
			if (format.TryGetIdentity(node, out id))
				AddIdentity(id, node);
			else if (format.TryGetReference(node, out id))
				AddReferenceEarly(id, node, references);
		}

		private void AddIdentity(int id, IXmlNode node)
		{
			Entry entry;
			if (!entriesById.TryGetValue(id, out entry))
				entriesById.Add(id, new Entry(id, node));

			if (nextId <= id)
				nextId = ++id;
		}

		private void AddReferenceEarly(int id, IXmlNode node, ICollection<Reference> references)
		{
			Entry entry;
			if (entriesById.TryGetValue(id, out entry))
				entry.AddReference(node);
			else
				references.Add(new Reference(id, node));
		}

		private void AddReferencesLate(ICollection<Reference> references)
		{
			foreach (var reference in references)
			{
				Entry entry;
				if (entriesById.TryGetValue(reference.Id, out entry))
					entry.AddReference(reference.Node);
			}
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

			var entry = (token == CreateEntryToken)
				? CreateEntry(node)
				: token as Entry;
			if (entry == null)
				return;

			AddValue(entry, type, value, null);
		}

		public bool OnAssigningNull(IXmlNode node, object oldValue)
		{
			object newValue = null;
			object token;
			return OnAssigningValue(node, oldValue, ref newValue, out token);
		}

        public bool OnAssigningValue(IXmlNode node, object oldValue, ref object newValue, out object token)
        {
			if (newValue == oldValue)
				{ token = null; return newValue == null; }

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
				newEntry = oldEntry ?? CreateEntry(node);
				AddValue(newEntry, type, newValue, xmlAdapter);
				token = newEntry;
            }
			return ShouldAssignmentProceed(oldEntry, newEntry, token);
        }

		private bool ShouldAssignmentProceed(Entry oldEntry, Entry newEntry, object token)
		{
			if (oldEntry != null && oldEntry != newEntry)
				entriesById.Remove(oldEntry.Id); // Didn't reuse old entry; delete it

			return token    != null  // Expecting callback with a token, so proceed with set
				|| newEntry == null; // No reference tracking for this value; don't prevent assignment
		}

		private Entry OnReplacingValue(IXmlNode node, object oldValue)
		{
			Entry entry;

			if (oldValue == null)
				return null;

			if (!entriesByValue.TryGetValue(oldValue, out entry))
				return null;

			if (!entry.Node.PositionEquals(node))
			{
				// Replacing reference
				entry.RemoveReference(node);
				ClearReference(entry, node);
				return null;
			}
			else if (entry.References != null) // (&& !isReference)
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
			PrepareForReuse(entry);
			return entry;
		}

		public void OnAssignedValue(IXmlNode node, object givenValue, object storedValue, object token)
		{
			var entry = token as Entry;
			if (entry == null)
				return;

			if (ReferenceEquals(givenValue, storedValue))
				return;

			SetNotInGraph(entry, givenValue);
			AddValue(entry, node.ClrType, storedValue, null);
		}

		private Entry CreateEntry(IXmlNode node)
		{
			var entry = new Entry(nextId++, node);
			entriesById.Add(entry.Id, entry);
			return entry;
		}

		private void AddReference(IXmlNode node, Entry entry)
		{
			if (!entry.Node.PositionEquals(node))
			{
				if (entry.References == null)
					format.SetIdentity(entry.Node, entry.Id);

				entry.AddReference(node);
				format.SetReference(node, entry.Id);
			}
		}

		private void AddValue(Entry entry, Type type, object value, XmlAdapter xmlAdapter)
		{
			if (xmlAdapter == null)
				xmlAdapter = XmlAdapter.For(value, false);

			AddValueCore(entry, type, value);

			if (xmlAdapter != null)
				AddValueCore(entry, typeof(XmlAdapter), xmlAdapter);
		}

		private void AddValueCore(Entry entry, Type type, object value)
		{
			entry.AddValue(type, value, true);
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
				entriesByValue.Remove(item.Value);

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
			entry = GetEntry(id);
			return true;
		}

		private Entry GetEntry(int id)
		{
			Entry entry;
			if (!entriesById.TryGetValue(id, out entry))
				throw IdNotFoundError(id);
			return entry;
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

				if (type.IsAssignableFrom(item.Type))
					return Try.Success(out value, item.Value);

				var candidate = item.Value as IDictionaryAdapter;
				if (candidate != null)
					dictionaryAdapter = candidate;
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
			return type.IsValueType
				|| type == StringType;
		}

		private static void SetNotInGraphCore(Entry entry, object value)
		{
			var values = entry.Values;
			for (int index = 0; index < values.Count; index++)
			{
				if (!ReferenceEquals(values[index].Value, value))
					continue;

				var item = values[index];
				item = new EntryValue(item.Type, item.Value, false);
				values[index] = item;
			}
		}

		private static IXmlNode RedirectNode(ref IXmlNode node, Entry entry)
		{
			var cursor = entry.Node.SelectSelf(node.ClrType);
			cursor.MoveNext();
			return node = cursor;
		}

		private static readonly Type
			StringType = typeof(string);

		private static readonly object
			CreateEntryToken = new object();

		private class Entry
		{
			public readonly int Id;
			public IXmlNode  Node;
			private List<IXmlNode> references;
			private List<EntryValue> values;

			public Entry(int id, IXmlNode node)
			{
				Id     = id;
				Node   = node;
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
				if (references.Count == 1)
					references = null;
				else
					references.RemoveAt(index);
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
			public readonly Type   Type;
			public readonly object Value;
			public readonly	bool   IsInGraph;

			public EntryValue(Type type, object value, bool isInGraph)
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
