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
	using System.Collections;
	using System.Collections.Generic;

	public class MemberwiseEqualityHashCodeStrategy : DictionaryBehaviorAttribute,
		IDictionaryEqualityHashCodeStrategy, IDictionaryInitializer, IEqualityComparer<IDictionaryAdapter>
	{
		[ThreadStatic]
		private static Dictionary<object, int> visited;

		public bool Equals(IDictionaryAdapter adapter1, IDictionaryAdapter adapter2)
		{
			if (ReferenceEquals(adapter1, adapter2))
			{
				return true;
			}
			if (adapter1.Meta.Type != adapter2.Meta.Type)
			{
				return false;
			}
			return GetHashCode(adapter1) == GetHashCode(adapter2);
		}

		public int GetHashCode(IDictionaryAdapter adapter)
		{
			int hashCode;
			GetHashCode(adapter, out hashCode);
			return hashCode;
		}

		public bool GetHashCode(IDictionaryAdapter adapter, out int hashCode)
		{
			var clear = false;
			var visitedLocal = visited;
			try
			{
				if (visitedLocal == null)
				{
					visited = visitedLocal = new Dictionary<object, int>(ReferenceEqualityComparer<object>.Instance);
					clear = true;
				}
				else
				{
					if (visitedLocal.TryGetValue(this, out hashCode))
					{
						return true;
					}
					visitedLocal.Add(this, 0);
				}
				hashCode = 27;
				foreach (var property in adapter.Meta.Properties)
				{
					var value = adapter.GetProperty(property.Key, false);
					var valueHashCode = GetValueHashCode(value);
					unchecked
					{
						hashCode = (13 * hashCode) + property.Key.GetHashCode();
						hashCode = (13 * hashCode) + valueHashCode;
					}
				}
				visitedLocal[this] = hashCode;
				return true;
			}
			finally
			{
				if (clear)
				{
					visited = null;
				}
			}
		}

		private int GetValueHashCode(object value)
		{
			if (value == null)
			{
				return 0;
			}

			if (value is IDictionaryAdapter)
			{
				return GetHashCode((IDictionaryAdapter)value);
			}

			if ((value is IEnumerable) && (value is string) == false)
			{
				return GetEnumerableHashcode((IEnumerable)value);
			}

			return value.GetHashCode();
		}

		private int GetEnumerableHashcode(IEnumerable enumerable)
		{
			var hash = 0;
			foreach (var value in enumerable)
			{
				var valueHashCode = GetValueHashCode(value);
				unchecked
				{
					hash = (13 * hash) + valueHashCode;
				}
			}
			return hash;
		}

		void IDictionaryInitializer.Initialize(IDictionaryAdapter dictionaryAdapter, object[] behaviors)
		{
			dictionaryAdapter.This.EqualityHashCodeStrategy = this;
		}
	}
}
