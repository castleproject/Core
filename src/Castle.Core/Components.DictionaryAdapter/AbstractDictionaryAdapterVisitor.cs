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
	using System.Reflection;

	using Castle.Core;

	/// <summary>
	/// Abstract implementation of <see cref="IDictionaryAdapterVisitor"/>.
	/// </summary>
	public abstract class AbstractDictionaryAdapterVisitor : IDictionaryAdapterVisitor
	{
		private readonly Dictionary<IDictionaryAdapter, int> scopes;

		protected AbstractDictionaryAdapterVisitor()
		{
			scopes = new Dictionary<IDictionaryAdapter, int>(ReferenceEqualityComparer<IDictionaryAdapter>.Instance);
		}

		protected AbstractDictionaryAdapterVisitor(AbstractDictionaryAdapterVisitor parent)
		{
			scopes = parent.scopes;
		}

		protected bool Cancelled { get; set; }

		public virtual bool VisitDictionaryAdapter(IDictionaryAdapter dictionaryAdapter, object state)
		{
			return VisitDictionaryAdapter(dictionaryAdapter, null, null);
		}

		public virtual bool VisitDictionaryAdapter(IDictionaryAdapter dictionaryAdapter, Func<PropertyDescriptor, bool> selector, object state)
		{
			if (PushScope(dictionaryAdapter) == false)
			{
				return false;
			}

			try
			{
				foreach (var property in dictionaryAdapter.This.Properties.Values)
				{
					if (Cancelled) break;

					if (selector != null && selector(property) == false)
					{
						continue;
					}

					Type collectionItemType;
					if (IsCollection(property, out collectionItemType))
					{
						VisitCollection(dictionaryAdapter, property, collectionItemType, state);
					}
					else if (property.PropertyType.GetTypeInfo().IsInterface)
					{
						VisitInterface(dictionaryAdapter, property, state);
					}
					else
					{
						VisitProperty(dictionaryAdapter, property, state);
					}
				}
			}
			finally
			{
				PopScope(dictionaryAdapter);
			}

			return true;
		}

		void IDictionaryAdapterVisitor.VisitProperty(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state)
		{
			VisitProperty(dictionaryAdapter, property, state);
		}

		protected virtual void VisitProperty(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state)
		{
		}

		void IDictionaryAdapterVisitor.VisitInterface(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state)
		{

			VisitInterface(dictionaryAdapter, property, state);
		}

		protected virtual void VisitInterface(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state)
		{
			VisitProperty(dictionaryAdapter, property, state);
		}

		void IDictionaryAdapterVisitor.VisitCollection(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, Type collectionItemType, object state)
		{
			VisitCollection(dictionaryAdapter, property, collectionItemType, state);
		}

		protected virtual void VisitCollection(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, Type collectionItemType, object state)
		{
			VisitProperty(dictionaryAdapter, property, state);
		}

		private bool PushScope(IDictionaryAdapter dictionaryAdapter)
		{
			if (scopes.ContainsKey(dictionaryAdapter))
			{
				return false;
			}
			scopes.Add(dictionaryAdapter, 0);
			return true;
		}

		private void PopScope(IDictionaryAdapter dictionaryAdapter)
		{
			scopes.Remove(dictionaryAdapter);
		}

		private static bool IsCollection(PropertyDescriptor property, out Type collectionItemType)
		{
			collectionItemType = null;
			var propertyType = property.PropertyType;
			if (propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType))
			{
				if (propertyType.GetTypeInfo().IsArray)
				{
					collectionItemType = propertyType.GetElementType();
				}
				else if (propertyType.GetTypeInfo().IsGenericType)
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
