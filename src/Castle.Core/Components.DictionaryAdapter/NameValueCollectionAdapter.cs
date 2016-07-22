// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Collections.Specialized;
	using System.Linq;

	/// <summary>
	/// 
	/// </summary>
	public class NameValueCollectionAdapter : AbstractDictionaryAdapter
	{
		private readonly NameValueCollection nameValues;

		/// <summary>
		/// Initializes a new instance of the <see cref="NameValueCollectionAdapter"/> class.
		/// </summary>
		/// <param name="nameValues">The name values.</param>
		public NameValueCollectionAdapter(NameValueCollection nameValues)
		{
			this.nameValues = nameValues;
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"></see> object is read-only.
		/// </summary>
		/// <value></value>
		/// <returns>true if the <see cref="T:System.Collections.IDictionary"></see> object is read-only; otherwise, false.</returns>
		public override bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.IDictionary"></see> object contains an element with the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary"></see> object.</param>
		/// <returns>
		/// true if the <see cref="T:System.Collections.IDictionary"></see> contains an element with the key; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">key is null. </exception>
		public override bool Contains(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			//Getting a value out is O(1), so in the case that the collection contains a non-null value for this key
			//we can skip the O(n) key lookup.
			if (this[key] != null)
			{
				return true;
			}

			return nameValues.AllKeys.Contains(key.ToString(), StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Gets or sets the <see cref="Object"/> with the specified key.
		/// </summary>
		/// <value></value>
		public override object this[object key]
		{
			get { return nameValues[key.ToString()]; }
			set
			{
				String val = (value != null) ? value.ToString() : null;
				nameValues[key.ToString()] = val;
			}
		}

		/// <summary>
		/// Adapts the specified name values.
		/// </summary>
		/// <param name="nameValues">The name values.</param>
		/// <returns></returns>
		public static NameValueCollectionAdapter Adapt(NameValueCollection nameValues)
		{
			return new NameValueCollectionAdapter(nameValues);
		}
	}
}