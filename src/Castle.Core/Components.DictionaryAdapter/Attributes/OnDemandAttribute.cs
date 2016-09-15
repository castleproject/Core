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
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// Support for on-demand value resolution.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = false)]
	public class OnDemandAttribute : DictionaryBehaviorAttribute, IDictionaryPropertyGetter
	{
		public OnDemandAttribute()
		{
		}

		public OnDemandAttribute(Type type)
		{
			if (type.GetConstructor(Type.EmptyTypes) == null)
			{
				throw new ArgumentException("On-demand values must have a parameterless constructor");
			}

			Type = type;
		}

		public OnDemandAttribute(object value)
		{
			Value = value;
		}

		public Type Type { get; private set; }

		public object Value { get; private set; }

		public object GetPropertyValue(IDictionaryAdapter dictionaryAdapter, string key,
									   object storedValue, PropertyDescriptor property, bool ifExists)
		{
			if (storedValue == null && ifExists == false)
			{
				IValueInitializer initializer = null;

				if (Value != null)
				{
					storedValue = Value;
				}
				else
				{
					var type = Type ?? GetInferredType(dictionaryAdapter, property, out initializer);

					if (IsAcceptedType(type))
					{
						if (type.GetTypeInfo().IsInterface)
						{
							if (property.IsDynamicProperty == false)
							{
								if (storedValue == null)
								{
									storedValue = dictionaryAdapter.Create(property.PropertyType);
								}
							}
						}
						else if (type.GetTypeInfo().IsArray)
						{
							storedValue = Array.CreateInstance(type.GetElementType(), 0);
						}
						else
						{
							if (storedValue == null)
							{
								object[] args = null;
								ConstructorInfo constructor = null;

								if (property.IsDynamicProperty)
								{
									constructor = 
										(from ctor in type.GetConstructors()
										 let parms = ctor.GetParameters()
										 where parms.Length == 1 &&
										       parms[0].ParameterType.IsAssignableFrom(dictionaryAdapter.Meta.Type)
										  select ctor).FirstOrDefault();

									if (constructor != null) args = new[] { dictionaryAdapter };
								}

								if (constructor == null)
								{
									constructor = type.GetConstructor(Type.EmptyTypes);
								}

								if (constructor != null)
								{
									storedValue = constructor.Invoke(args);
								}
							}
						}
					}
				}

				if (storedValue != null)
				{
					using (dictionaryAdapter.SuppressNotificationsBlock())
					{
#if FEATURE_ISUPPORTINITIALIZE
						if (storedValue is ISupportInitialize)
						{
							((ISupportInitialize)storedValue).BeginInit();
							((ISupportInitialize)storedValue).EndInit();
						}
#endif
						if (initializer != null)
						{
							initializer.Initialize(dictionaryAdapter, storedValue);
						}

						property.SetPropertyValue(dictionaryAdapter, property.PropertyName,
												  ref storedValue, dictionaryAdapter.This.Descriptor);
					}
				}
			}

			return storedValue;
		}

		private static bool IsAcceptedType(Type type)
		{
			return type != null && type != typeof(string) && !type.GetTypeInfo().IsPrimitive && !type.GetTypeInfo().IsEnum;
		}

		private static Type GetInferredType(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, out IValueInitializer initializer)
		{
			Type type = null;
			initializer = null;

			type = property.PropertyType;
			if (typeof(IEnumerable).IsAssignableFrom(type) == false)
			{
				return type;
			}

			Type collectionType = null;

			if (type.GetTypeInfo().IsGenericType)
			{
				var genericDef = type.GetGenericTypeDefinition();
				var genericArg = type.GetGenericArguments()[0];
				bool isBindingList =
#if !FEATURE_BINDINGLIST
					false;
#else
					genericDef == typeof(System.ComponentModel.BindingList<>);
#endif

				if (isBindingList || genericDef == typeof(List<>))
				{
					if (dictionaryAdapter.CanEdit)
					{
#if !FEATURE_BINDINGLIST
						collectionType =  typeof(EditableList<>);
#else
						collectionType = isBindingList ? typeof(EditableBindingList<>) : typeof(EditableList<>);
#endif
					}

#if FEATURE_BINDINGLIST
					if (isBindingList && genericArg.GetTypeInfo().IsInterface)
					{
						Func<object> addNew = () => dictionaryAdapter.Create(genericArg);
						initializer = (IValueInitializer)Activator.CreateInstance(
							typeof(BindingListInitializer<>).MakeGenericType(genericArg),
							null, addNew, null, null, null);
					}
#endif
				}
				else if (genericDef == typeof(IList<>) || genericDef == typeof(ICollection<>))
				{
					collectionType = dictionaryAdapter.CanEdit ? typeof(EditableList<>) : typeof(List<>);
				}

				if (collectionType != null)
				{
					return collectionType.MakeGenericType(genericArg);
				}
			}
			else if (type == typeof(IList) || type == typeof(ICollection))
			{
				return dictionaryAdapter.CanEdit ? typeof(EditableList) : typeof(List<object>);
			}

			return type;
		}
	}
}
