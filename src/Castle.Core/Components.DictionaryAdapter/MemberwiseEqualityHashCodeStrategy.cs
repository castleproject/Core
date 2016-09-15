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
		class HashCodeVisitor : AbstractDictionaryAdapterVisitor
		{
			private int hashCode;

			public int CalculateHashCode(IDictionaryAdapter dictionaryAdapter)
			{
				if (dictionaryAdapter == null)
					return 0;

				hashCode = 27;
				return VisitDictionaryAdapter(dictionaryAdapter, null) ? hashCode : 0;
			}

			protected override void VisitProperty(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state)
			{
				var value = dictionaryAdapter.GetProperty(property.PropertyName, true);
				CollectHashCode(property, GetValueHashCode(value));
			}

			protected override void VisitInterface(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state)
			{
				var nested = (IDictionaryAdapter)dictionaryAdapter.GetProperty(property.PropertyName, true);
				CollectHashCode(property, GetNestedHashCode(nested));
			}

			protected override void VisitCollection(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, Type collectionItemType, object state)
			{
				var collection = (IEnumerable)dictionaryAdapter.GetProperty(property.PropertyName, true);
				CollectHashCode(property, GetCollectionHashcode(collection));
			}

			private int GetValueHashCode(object value)
			{
				if (value == null)
				{
					return 0;
				}

				if (value is IDictionaryAdapter)
				{
					return GetNestedHashCode((IDictionaryAdapter)value);
				}

				if ((value is IEnumerable) && (value is string) == false)
				{
					return GetCollectionHashcode((IEnumerable)value);
				}

				return value.GetHashCode();
			}

			private int GetNestedHashCode(IDictionaryAdapter nested)
			{
				var currentHashCode = hashCode;
				var nestedHashCode = CalculateHashCode(nested);
				hashCode = currentHashCode;
				return nestedHashCode;
			}

			private int GetCollectionHashcode(IEnumerable collection)
			{
				if (collection == null)
				{
					return 0;
				}

				var collectionHashCode = 0;

				foreach (var value in collection)
				{
					var valueHashCode = GetValueHashCode(value);
					unchecked
					{
						collectionHashCode = (13 * collectionHashCode) + valueHashCode;
					}
				}

				return collectionHashCode;
			}

			private void CollectHashCode(PropertyDescriptor property, int valueHashCode)
			{
				unchecked
				{
					hashCode = (13 * hashCode) + property.PropertyName.GetHashCode();
					hashCode = (13 * hashCode) + valueHashCode;
				}
			}
		}

		public bool Equals(IDictionaryAdapter adapter1, IDictionaryAdapter adapter2)
		{
			if (ReferenceEquals(adapter1, adapter2))
			{
				return true;
			}

			if ((adapter1 == null) ^ (adapter2 == null))
			{
				return false;
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
			hashCode = new HashCodeVisitor().CalculateHashCode(adapter);
			return true;
		}

		void IDictionaryInitializer.Initialize(IDictionaryAdapter dictionaryAdapter, object[] behaviors)
		{
			dictionaryAdapter.This.EqualityHashCodeStrategy = this;
		}
	}
}
