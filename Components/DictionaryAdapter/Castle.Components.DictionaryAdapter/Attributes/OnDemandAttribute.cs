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

		public object GetPropertyValue(IDictionaryAdapter dictionaryAdapter, 
			string key, object storedValue, PropertyDescriptor property)
		{
			if (storedValue == null)
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
						if (type.IsInterface)
						{
							if (!property.IsDynamicProperty)
							{
								storedValue = dictionaryAdapter.Create(property.PropertyType);
							}
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
							object[] args = null;

							if (property.IsDynamicProperty)
							{
								var constructor =
									(from ctor in property.PropertyType.GetConstructors()
									 let parms = ctor.GetParameters()
									 where parms.Length == 1 && 
										parms[0].ParameterType.IsAssignableFrom(dictionaryAdapter.Type)
									 select ctor).FirstOrDefault();

								if (constructor != null) args = new[] { dictionaryAdapter };
							}

							if (args != null)
							{
								storedValue = Activator.CreateInstance(type, args);
							}
							else
							{
								storedValue = Activator.CreateInstance(type);
							}
						}
					}
				}

				if (storedValue != null)
				{
					using (dictionaryAdapter.SupressNotificationsSection())
                    {
						if (storedValue is ISupportInitialize)
						{
							((ISupportInitialize)storedValue).BeginInit();
							((ISupportInitialize)storedValue).EndInit();
						}

						if (initializer != null)
						{
							initializer.Initialize(dictionaryAdapter, storedValue);
						}

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

		private Type GetInferredType(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property,
									 out IValueInitializer initializer)
		{
			initializer = null;
			var type = property.PropertyType;

			if (!typeof(IEnumerable).IsAssignableFrom(type))
			{
				return type;
			}

			Type collectionType = null;

			if (type.IsGenericType)
			{
				var genericDef = type.GetGenericTypeDefinition();
				var genericArg = type.GetGenericArguments()[0];
				bool isBindingList = genericDef == typeof(BindingList<>);

				if (isBindingList || genericDef == typeof(List<>))
				{
					if (dictionaryAdapter.CanEdit)
					{
						collectionType = isBindingList ? typeof(EditableBindingList<>) : typeof(EditableList<>);
					}

					if (isBindingList && genericArg.IsInterface)
					{
						initializer = (IValueInitializer)Activator.CreateInstance(
							typeof(BindingListInitializer<>).MakeGenericType(genericArg));
					}
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
				return dictionaryAdapter.CanEdit ? typeof(EditableList) : typeof(ArrayList);
			}

			return type;
		}
	}

	#region Initialization Helpers

	interface IValueInitializer
	{
		void Initialize(IDictionaryAdapter dictionaryAdapter, object value);
	}

	class BindingListInitializer<T> : IValueInitializer
	{
		public void Initialize(IDictionaryAdapter dictionaryAdapter, object value)
		{
			var bindingList = (BindingList<T>)value;
			bindingList.AddingNew += (sender, args) =>
			{
				args.NewObject = dictionaryAdapter.Create<T>();
			};
		}
	}

	#endregion
}
