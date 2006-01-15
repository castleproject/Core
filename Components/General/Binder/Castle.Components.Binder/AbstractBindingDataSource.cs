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

	public abstract class AbstractBindingDataSource : IBindingDataSourceNode
	{
		public abstract String GetEntryValue(String name);

		public abstract String GetMetaEntryValue(String name);

		public IBindingDataSourceNode ObtainNode(String name)
		{
			if (name == null) throw new ArgumentNullException("name");

			name = name.ToLower();

			DataSourceNode node = new DataSourceNode(name);

			int checkIndex = name.Length;

			foreach(String key in AllKeys)
			{
				if (key == null || key.Length <= name.Length || !key.ToLower().StartsWith(name)) continue;

				char checkChar = key[checkIndex];

				if (checkChar == '.')
				{
					node.ProcessEntry(key.Substring(checkIndex + 1), GetEntryValue(key));
				}
				else if (checkChar == '@')
				{
					node.ProcessMetaEntry(key.Substring(checkIndex + 1), GetEntryValue(key));
				}
				else if (checkChar == '[')
				{
					int closingBracket = key.IndexOf(']', checkIndex + 1);

					if (closingBracket == -1)
					{
						throw new BindingDataSourceException("malformed key {0}. It looks like an indexed value for the prefix {1}, but no closing bracket was found", key, name);
					}

					String indexString = key.Substring(checkIndex + 1, closingBracket - checkIndex - 1);
					int indexValue;

					try
					{
						indexValue = Convert.ToInt32(indexString);
					}
					catch(Exception)
					{
						throw new BindingDataSourceException("malformed key {0}. It looks like an indexed value for the prefix {1}, but the index could not be converted to int32", key, name);
					}

					bool isMeta = key[closingBracket + 1] == '@';

					if (isMeta)
					{
						// the offset 2 skips the '].' chars
						node.ProcessMetaIndexedEntry(indexValue, key.Substring(closingBracket + 2), GetEntryValue(key));
					}
					else
					{
						// the offset 2 skips the '].' chars
						node.ProcessIndexedEntry(indexValue, key.Substring(closingBracket + 2), GetEntryValue(key));
					}
				}
			}

			return node.IsEmpty ? null : node;
		}

		public virtual bool IsIndexed
		{
			get { throw new NotImplementedException(); }
		}

		public virtual IBindingDataSourceNode[] IndexedNodes
		{
			get { throw new NotImplementedException(); }
		}

		public virtual bool ShouldIgnore
		{
			get { throw new NotImplementedException(); }
		}

		protected abstract String[] AllKeys { get; }

		#region IDictionary implementation

		public bool Contains(object key)
		{
			throw new NotImplementedException();
		}

		public void Add(object key, object value)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public void Remove(object key)
		{
			throw new NotImplementedException();
		}

		public ICollection Keys
		{
			get { throw new NotImplementedException(); }
		}

		public ICollection Values
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public abstract object this[object key]
		{
			get; set;
		}

		#endregion

		#region ICollection

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public virtual int Count
		{
			get { throw new NotImplementedException(); }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		#endregion

		#region ICollection

		public virtual IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
