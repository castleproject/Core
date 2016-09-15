// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License";
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
	using System.Collections.Generic;

	public class GenericDictionaryAdapter<TValue> : AbstractDictionaryAdapter
	{
		private readonly IDictionary<string, TValue> dictionary;

		public GenericDictionaryAdapter(IDictionary<string, TValue> dictionary)
		{
			this.dictionary = dictionary;
		}

		public override bool IsReadOnly
		{
			get { return dictionary.IsReadOnly; }
		}

		public override bool Contains(object key)
		{
			return dictionary.Keys.Contains(GetKey(key));
		}

		public override object this[object key]
		{
			get 
			{
				TValue value;
				return dictionary.TryGetValue(GetKey(key), out value) ? value : default(TValue);
			}
			set { dictionary[GetKey(key)] = (TValue)value; }
		}

		private static string GetKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return key.ToString();
		}
	}

	public static class GenericDictionaryAdapter
	{
		public static GenericDictionaryAdapter<TValue> ForDictionaryAdapter<TValue>(this IDictionary<string, TValue> dictionary)
		{
			return new GenericDictionaryAdapter<TValue>(dictionary);
		}
	}
}
