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
	using System.ComponentModel;

	/// <summary>
	/// Support for on-demand value resolution.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = false)]
	public class DictionaryOnDemandAttribute : DictionaryBehaviorAttribute, IDictionaryPropertyGetter
	{
		public DictionaryOnDemandAttribute()
		{
		}

		public DictionaryOnDemandAttribute(Type type)
		{
			if (type.GetConstructor(Type.EmptyTypes) == null)
			{
				throw new ArgumentException("On-demand values must have a parameterless constructor");
			}

			Type = type;
		}

		public DictionaryOnDemandAttribute(object value)
		{
			Value = value;
		}

		public Type Type { get; private set; }

		public object Value { get; private set; }

		public object GetPropertyValue(IDictionaryAdapter dictionaryAdapter, 
			string key, object storedValue, PropertyDescriptor property)
		{
			if (storedValue == null)
			{
				if (Value != null)
				{
					storedValue = Value;
				}
				else
				{
					var type = Type ?? GetInferredType(dictionaryAdapter, property);

					if (IsAcceptedType(type))
					{
						if (type.IsInterface)
						{
							storedValue = dictionaryAdapter.Create(property.PropertyType);
						}
						else if (type.IsArray)
						{
							storedValue = Array.CreateInstance(type.GetElementType(), 0);
						}
						else if (type == typeof(Guid))
						{
							storedValue = Guid.NewGuid();
						}
						else
						{
							storedValue = Activator.CreateInstance(type);
						}
					}
				}

				if (storedValue != null)
				{
					using (dictionaryAdapter.SupressNotificationsSection())
                    {
						dictionaryAdapter.SetProperty(property.PropertyName, ref storedValue);	
                    }
				}
			}

			return storedValue;
		}

		private bool IsAcceptedType(Type type)
		{
			return type != null && type != typeof(String) && !type.IsPrimitive && !type.IsEnum;
		}

		private Type GetInferredType(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property)
		{
			var type = property.PropertyType;

			if (!typeof(IEnumerable).IsAssignableFrom(type))
			{
				return type;
			}

			Type collectionType = null;

			if (type.IsGenericType)
			{
				var genericDef = type.GetGenericTypeDefinition();

				if (genericDef == typeof(List<>) || genericDef == typeof(BindingList<>))
				{
					if (dictionaryAdapter.IsEditable)
					{
						collectionType = genericDef == typeof(List<>)
							? typeof(EditableList<>) : typeof(EditableBindingList<>);
					}
				}
				else if (genericDef == typeof(IList<>) || genericDef == typeof(ICollection<>))
				{
					collectionType = dictionaryAdapter.IsEditable ? typeof(EditableList<>) : typeof(List<>);
				}

				if (collectionType != null)
				{
					return collectionType.MakeGenericType(type.GetGenericArguments()[0]);
				}
			}
			else if (type == typeof(IList) || type == typeof(ICollection))
			{
				return dictionaryAdapter.IsEditable ? typeof(EditableList) : typeof(ArrayList);
			}

			return type;
		}
	}
}
