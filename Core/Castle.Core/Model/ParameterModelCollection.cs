// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core
{
	using System;
	using System.Collections;
	using Castle.Core.Configuration;
	using System.Collections.Generic;

	/// <summary>
	/// Collection of <see cref="ParameterModel"/>
	/// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
	public class ParameterModelCollection : IEnumerable
	{
		private readonly IDictionary<string, ParameterModel> dictionary;
		private readonly object syncRoot = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterModelCollection"/> class.
		/// </summary>
		public ParameterModelCollection()
		{
			dictionary = new Dictionary<string, ParameterModel>(StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Adds the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public void Add(String name, String value)
		{
			dictionary.Add(name, new ParameterModel(name, value));
		}

		/// <summary>
		/// Adds the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="configNode">The config node.</param>
		public void Add(String name, IConfiguration configNode)
		{
			dictionary.Add(name, new ParameterModel(name, configNode));
		}

		/// <summary>
		/// Determines whether this collection contains the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// <c>true</c> if yes; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(object key)
		{
			return dictionary.ContainsKey((string) key);
		}

		/// <summary>
		/// Adds the specified key.
		/// </summary>
		/// <remarks>
		/// Not implemented
		/// </remarks>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "key"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void Add(object key, object value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		/// <remarks>
		/// Not implemented
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void Clear()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <remarks>
		/// Not implemented
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "key")]
		public void Remove(object key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the keys.
		/// </summary>
		/// <value>The keys.</value>
		/// <remarks>
		/// Not implemented
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public ICollection Keys
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the values.
		/// </summary>
		/// <value>The values.</value>
		/// <remarks>
		/// Not implemented
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public ICollection Values
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		public bool IsReadOnly
		{
			get { return dictionary.IsReadOnly; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is fixed size.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is fixed size; otherwise, <c>false</c>.
		/// </value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public bool IsFixedSize
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the <see cref="ParameterModel"/> with the specified key.
		/// </summary>
		/// <value></value>
		public ParameterModel this[object key]
		{
			get
			{
				ParameterModel result;
				dictionary.TryGetValue((string) key, out result);
				return result;
			}
		}

		/// <summary>
		/// Copy the content to the specified array
		/// </summary>
		/// <param name="array">target array</param>
		/// <param name="index">target index</param>
		/// <remarks>
		/// Not implemented
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "index"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "array")]
		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get { return dictionary.Count; }
		}

		/// <summary>
		/// Gets the sync root.
		/// </summary>
		/// <value>The sync root.</value>
		public object SyncRoot
		{
			get { return syncRoot; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is synchronized.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is synchronized; otherwise, <c>false</c>.
		/// </value>
		public bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// Returns an enumerator that can iterate through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/>
		/// that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return dictionary.Values.GetEnumerator();
		}
	}
}