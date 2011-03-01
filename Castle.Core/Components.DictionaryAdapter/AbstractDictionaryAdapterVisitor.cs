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
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;

	using Castle.Core;

	/// <summary>
	/// Abstract implementation of <see cref="IDictionaryAdapterVisitor"/>.
	/// </summary>
	public abstract class AbstractDictionaryAdapterVisitor : IDictionaryAdapterVisitor
	{
#if SL3 //Silverlight 3 does not have HashSet<T>
		private List<IDictionaryAdapter> visited;
#else
		private HashSet<IDictionaryAdapter> visited;
#endif
		protected AbstractDictionaryAdapterVisitor()
		{
#if SL3
		visited = new List<IDictionaryAdapter>();
#else
		visited = new HashSet<IDictionaryAdapter>(ReferenceEqualityComparer<IDictionaryAdapter>.Instance);
#endif			
		}

		protected AbstractDictionaryAdapterVisitor(AbstractDictionaryAdapterVisitor parent)
		{
			visited = parent.visited;
		}

		public virtual void VisitDictionaryAdapter(IDictionaryAdapter dictionaryAdapter)
		{
			VisitDictionaryAdapter(dictionaryAdapter, null);
		}

		public virtual void VisitDictionaryAdapter(IDictionaryAdapter dictionaryAdapter, Func<PropertyDescriptor, bool> selector)
		{
			if (ShouldVisit(dictionaryAdapter) == false)
			{
				return;
			}

			foreach (var property in dictionaryAdapter.This.Properties.Values)
			{
				if (selector != null && selector(property) == false)
				{
					continue;
				}

				Type collectionItemType;
				if (IsCollection(property, out collectionItemType))
				{
					VisitCollection(dictionaryAdapter, property, collectionItemType);
				}
				else if (property.PropertyType.IsInterface)
				{
					VisitInterface(dictionaryAdapter, property);
				}
				else
				{
					VisitProperty(dictionaryAdapter, property);
				}
			}
		}

		void IDictionaryAdapterVisitor.VisitProperty(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property)
		{
			VisitProperty(dictionaryAdapter, property);
		}

		protected virtual void VisitProperty(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property)
		{
		}

		void IDictionaryAdapterVisitor.VisitInterface(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property)
		{
			VisitInterface(dictionaryAdapter, property);
		}

		protected virtual void VisitInterface(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property)
		{
			VisitProperty(dictionaryAdapter, property);
		}

		void IDictionaryAdapterVisitor.VisitCollection(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property,
													   Type collectionItemType)
		{
			VisitCollection(dictionaryAdapter, property, collectionItemType);
		}

		protected virtual void VisitCollection(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property,
											   Type collectionItemType)
		{
			VisitProperty(dictionaryAdapter, property);
		}

		private bool ShouldVisit(IDictionaryAdapter dictionaryAdapter)
		{
#if SL3
			if (visited.Contains(dictionaryAdapter, ReferenceEqualityComparer<IDictionaryAdapter>.Instance))
			{
				return false;
			}
			visited.Add(dictionaryAdapter);
			return true;
#else
			return visited.Add(dictionaryAdapter);
#endif
		}

		private static bool IsCollection(PropertyDescriptor property, out Type collectionItemType)
		{
			collectionItemType = null;
			var propertyType = property.PropertyType;
			if (propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType))
			{
				if (propertyType.IsArray)
				{
					collectionItemType = propertyType.GetElementType();
				}
				else if (propertyType.IsGenericType)
				{
					var arguments = propertyType.GetGenericArguments();
					collectionItemType = arguments[0];
				}
				else
				{
					collectionItemType = typeof(object);
				}
				return true;
			}
			return false;
		}
	}
}
