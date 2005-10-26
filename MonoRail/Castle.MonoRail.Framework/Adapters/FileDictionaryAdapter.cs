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

namespace Castle.MonoRail.Framework.Adapters
{
	using System;
	using System.Web;
	using System.Collections;

	public class FileDictionaryAdapter : IDictionary
	{
		private HttpFileCollection _fileCollection;

		public FileDictionaryAdapter(HttpFileCollection fileCollection)
		{
			_fileCollection = fileCollection;
		}

		public bool Contains(object key)
		{
			return _fileCollection[ (String)key ] != null;
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
			get { return _fileCollection.Keys; }
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
			get { return true; }
		}

		public object this[object key]
		{
			get { return _fileCollection[ (String)key ]; }
			set { throw new NotImplementedException(); }
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return _fileCollection.Count; }
		}

		public object SyncRoot
		{
			get { return this.SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public IEnumerator GetEnumerator()
		{
			return _fileCollection.GetEnumerator();
		}
	}
}
