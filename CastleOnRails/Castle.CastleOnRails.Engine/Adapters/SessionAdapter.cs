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

namespace Castle.CastleOnRails.Engine.Adapters
{
	using System;
	using System.Collections;
	using System.Web.SessionState;
	
	/// <summary>
	/// Summary description for SessionAdapter.
	/// </summary>
	public class SessionAdapter : IDictionary
	{
		private HttpSessionState _session;

		public SessionAdapter(HttpSessionState session)
		{
			_session = session;
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public ICollection Keys
		{
			get { return _session.Keys; }
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return _session.Count; }
		}

		public object SyncRoot
		{
			get { return _session.SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return _session.IsSynchronized; }
		}

		public IEnumerator GetEnumerator()
		{
			return _session.GetEnumerator();
		}

		public bool Contains(object key)
		{
			return _session[(String) key] != null;
		}

		public void Add(object key, object value)
		{
			_session.Add((String) key, value);
		}

		public void Clear()
		{
			_session.Clear();
		}

		public void Remove(object key)
		{
			_session.Remove((String) key);
		}

		public ICollection Values
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { return _session.IsReadOnly; }
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public object this[object key]
		{
			get { return _session[(String) key]; }
			set
			{
				_session[(String) key] = value;
			}
		}
	}
}
