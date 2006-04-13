// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Binder
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	public class DataSourceNode : AbstractBindingDataSource
	{
		private readonly string name;
		private bool isIndexed;
		private NameValueCollection entries = new NameValueCollection(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
		private SortedList indexedEntries = new SortedList(IndexComparer.Instance);
		private HybridDictionary metaEntries;

		public DataSourceNode(String name)
		{
			this.name = name;
		}

		public bool IsEmpty
		{
			get { return entries.Count == 0 && indexedEntries.Count == 0; }
		}

		public override bool ShouldIgnore
		{
			get
			{
				String ignore = GetMetaEntryValue("ignore");
				return ignore == "true" || ignore == "yes";
			}
		}

		public override bool IsIndexed
		{
			get { return isIndexed; }
		}

		public override String GetEntryValue(String name)
		{
			return entries[name];
		}

		public override String GetMetaEntryValue(String name)
		{
			if (metaEntries == null) return null;

			return (String) metaEntries[name];
		}

		public override Object GetEntryValue(String name, Type desiredType, out bool conversionSucceeded)
		{
			throw new NotImplementedException();
		}

		public void ProcessEntry(string key, string value)
		{
			if (isIndexed)
			{
				throw new BindingDataSourceException("Invalid state: the node {0} can not receive indexed and non-indexed data", name);
			}

			entries.Add(key, value);
		}

		public void ProcessMetaEntry(string key, string value)
		{
			if (metaEntries == null)
			{
				metaEntries = new HybridDictionary(true);
			}

			metaEntries.Add(key, value);
		}

		public void ProcessIndexedEntry(int index, string key, string value)
		{
			isIndexed = true;

			DataSourceNode node = ObtainNode(index);

			node.ProcessEntry(key, value);
		}

		public void ProcessMetaIndexedEntry(int index, string key, string value)
		{
			isIndexed = true;

			DataSourceNode node = ObtainNode(index);

			node.ProcessMetaEntry(key, value);
		}

		private DataSourceNode ObtainNode(int index)
		{
			String indexString = index.ToString();
	
			DataSourceNode node = (DataSourceNode) indexedEntries[indexString];
	
			if (node == null)
			{
				node = new DataSourceNode(indexString);
				indexedEntries.Add(indexString, node);
			}
			return node;
		}

		public override IBindingDataSourceNode[] IndexedNodes
		{
			get
			{
				IBindingDataSourceNode[] nodes = new IBindingDataSourceNode[indexedEntries.Count];
				indexedEntries.Values.CopyTo(nodes, 0);
				return nodes;
			}
		}

		protected override String[] AllKeys
		{
			get { return entries.AllKeys; }
		}

		public override object this[object key]
		{
			get { return entries[key.ToString()]; }
			set { throw new NotImplementedException(); }
		}

		public override IEnumerator GetEnumerator()
		{
			return entries.GetEnumerator();
		}
	}
}
