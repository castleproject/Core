// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Model
{
	using System;
	using System.Collections;

	using Castle.Model.Configuration;

	/// <summary>
	/// Collection of <see cref="ParameterModel"/>
	/// </summary>
	[Serializable]
	public class ParameterModelCollection : IEnumerable
	{
		private Hashtable dictionary;

		public ParameterModelCollection()
		{
			dictionary = new Hashtable(
				CaseInsensitiveHashCodeProvider.Default, 
				CaseInsensitiveComparer.Default);
		}

		public void Add(String name, String value)
		{
			dictionary.Add( name, new ParameterModel(name, value) );
		}

		public void Add(String name, IConfiguration configNode)
		{
			dictionary.Add( name, new ParameterModel(name, configNode) );
		}

		public bool Contains(object key)
		{
			return dictionary.Contains(key);
		}

		public void Add(object key, object value)
		{
			throw new NotImplementedException();
		}

		public void Clear()
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
			get { return dictionary.IsReadOnly; }
		}

		public bool IsFixedSize
		{
			get { return dictionary.IsFixedSize; }
		}

		public ParameterModel this[object key]
		{
			get { return (ParameterModel) dictionary[key]; }
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return dictionary.Count; }
		}

		public object SyncRoot
		{
			get { return dictionary.SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return dictionary.IsSynchronized; }
		}

		public IEnumerator GetEnumerator()
		{
			return dictionary.Values.GetEnumerator();
		}
	}
}
