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

	/// <summary>
	/// Abstract implementation of <see cref="IDictionaryAdapterVisitor"/>.
	/// </summary>
	public abstract class AbstractDictionaryAdapterVisitor : IDictionaryAdapterVisitor
	{
		public virtual void VisitDictionaryAdapter(IDictionaryAdapter dictionaryAdapter)
		{
			foreach (var property in dictionaryAdapter.Meta.Properties.Values)
			{
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
