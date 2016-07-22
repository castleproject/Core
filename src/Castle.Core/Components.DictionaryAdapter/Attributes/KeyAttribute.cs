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

	/// <summary>
	/// Assigns a specific dictionary key.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class KeyAttribute : DictionaryBehaviorAttribute, IDictionaryKeyBuilder
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="KeyAttribute"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		public KeyAttribute(String key)
		{
			Key = key;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KeyAttribute"/> class.
		/// </summary>
		/// <param name="keys">The compound key.</param>
		public KeyAttribute(String[] keys)
		{
			Key = string.Join(",", keys);
		}

		public String Key { get; private set; }

		String IDictionaryKeyBuilder.GetKey(IDictionaryAdapter dictionaryAdapter, String key, PropertyDescriptor property)
		{
			return Key;
		}
	}
}