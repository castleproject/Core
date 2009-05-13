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
	using System.Reflection;

	/// <summary>
	/// Describes a dictionary property.
	/// </summary>
	public class PropertyDescriptor : IDictionaryKeyBuilder,
	                                  IDictionaryPropertyGetter,
	                                  IDictionaryPropertySetter
	{
		private readonly PropertyInfo property;
		private List<IDictionaryPropertyGetter> getters;
		private List<IDictionaryPropertySetter> setters;
		private List<IDictionaryKeyBuilder> keyBuilders;
		private TypeConverter typeConverter;

		/// <summary>
		/// Initializes an empty <see cref="PropertyDescriptor"/> class.
		/// </summary>
		public PropertyDescriptor()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyDescriptor"/> class.
		/// </summary>
		/// <param name="property">The property.</param>
		public PropertyDescriptor(PropertyInfo property)
		{
			this.property = property;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyDescriptor"/> class
		/// with an existing property descriptor.
		/// </summary>
		/// <param name="other">The other descriptor.</param>
		public PropertyDescriptor(PropertyDescriptor other)
		{
			property = other.property;
			AddKeyBuilders(other.keyBuilders);
			AddGetters(other.getters);
			AddSetters(other.setters);
		}

		/// <summary>
		/// 
		/// </summary>
		public int ExecutionOrder
		{
			get { return 0; }
		}

		/// <summary>
		/// Gets the property name.
		/// </summary>
		public string PropertyName
		{
			get { return (Property != null) ? Property.Name : null; }
		}

		/// <summary>
		/// Gets the property type.
		/// </summary>
		public Type PropertyType
		{
			get { return (Property != null) ? Property.PropertyType : null; }
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <value>The property.</value>
		public PropertyInfo Property
		{
			get { return property; }
		}

		/// <summary>
		/// Gets the type converter.
		/// </summary>
		/// <value>The type converter.</value>
		public TypeConverter TypeConverter
		{
			get
			{
				if (typeConverter == null)
				{
					Type converterType = AttributesUtil.GetTypeConverter(property);

					if (converterType != null)
					{
						typeConverter = (TypeConverter) Activator.CreateInstance(converterType);
					}
					else
					{
						typeConverter = TypeDescriptor.GetConverter(PropertyType);
					}
				}

				return typeConverter;
			}
		}

		/// <summary>
		/// Gets or sets the key builders.
		/// </summary>
		/// <value>The key builders.</value>
		public ICollection<IDictionaryKeyBuilder> KeyBuilders
		{
			get { return keyBuilders; }
		}

		/// <summary>
		/// Gets or sets the setter.
		/// </summary>
		/// <value>The setter.</value>
		public ICollection<IDictionaryPropertySetter> Setters
		{
			get { return setters; }
		}

		/// <summary>
		/// Gets or sets the getter.
		/// </summary>
		/// <value>The getter.</value>
		public ICollection<IDictionaryPropertyGetter> Getters
		{
			get { return getters; }
		}

		#region IDictionaryKeyBuilder Members

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public string GetKey(IDictionary dictionary, string key,
		                     PropertyDescriptor descriptor)
		{
			if (keyBuilders != null)
			{
				foreach(IDictionaryKeyBuilder builder in keyBuilders)
				{
					key = builder.GetKey(dictionary, key, this);
				}
			}

			if (descriptor != null && descriptor.KeyBuilders != null)
			{
				foreach (IDictionaryKeyBuilder builder in descriptor.KeyBuilders)
				{
					key = builder.GetKey(dictionary, key, this);
				}
			}

			return key;
		}

		/// <summary>
		/// Adds the key builder.
		/// </summary>
		/// <param name="builders">The builder.</param>
		public PropertyDescriptor AddKeyBuilder(params IDictionaryKeyBuilder[] builders)
		{
			return AddKeyBuilders((ICollection<IDictionaryKeyBuilder>)builders);
		}

		/// <summary>
		/// Adds the key builders.
		/// </summary>
		/// <param name="builders">The builders.</param>
		public PropertyDescriptor AddKeyBuilders(ICollection<IDictionaryKeyBuilder> builders)
		{
			if (builders != null)
			{
				if (keyBuilders == null)
				{
					keyBuilders = new List<IDictionaryKeyBuilder>(builders);
				}
				else
				{
					keyBuilders.AddRange(builders);
				}
			}
			return this;
		}

		#endregion

		#region IDictionaryPropertyGetter Members

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="factory">The factory.</param>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="storedValue">The stored value.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public object GetPropertyValue(
			IDictionaryAdapterFactory factory, IDictionary dictionary,
			string key, object storedValue, PropertyDescriptor descriptor)
		{
			if (getters != null)
			{
				foreach(IDictionaryPropertyGetter getter in getters)
				{
					storedValue = getter.GetPropertyValue(
						factory, dictionary, key, storedValue, this);
				}
			}

			if (descriptor != null && descriptor.Getters != null)
			{
				foreach (IDictionaryPropertyGetter getter in descriptor.Getters)
				{
					storedValue = getter.GetPropertyValue(
						factory, dictionary, key, storedValue, this);
				}
			}

			return storedValue;
		}

		/// <summary>
		/// Adds the dictionary getter.
		/// </summary>
		/// <param name="getters">The getter.</param>
		public PropertyDescriptor AddGetter(params IDictionaryPropertyGetter[] getters)
		{
			return AddGetters((ICollection<IDictionaryPropertyGetter>)getters);
		}

		/// <summary>
		/// Adds the dictionary getters.
		/// </summary>
		/// <param name="gets">The getters.</param>
		public PropertyDescriptor AddGetters(ICollection<IDictionaryPropertyGetter> gets)
		{
			if (gets != null)
			{
				if (getters == null)
				{
					getters = new List<IDictionaryPropertyGetter>(gets);
				}
				else
				{
					getters.AddRange(gets);
				}
			}
			return this;
		}

		#endregion

		#region IDictionaryPropertySetter Members

		/// <summary>
		/// Sets the property value.
		/// </summary>
		/// <param name="factory">The factory.</param>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public bool SetPropertyValue(
			IDictionaryAdapterFactory factory, IDictionary dictionary,
			string key, ref object value, PropertyDescriptor descriptor)
		{
			bool consumed = false;

			if (setters != null)
			{
				foreach(IDictionaryPropertySetter setter in setters)
				{
					if (!setter.SetPropertyValue(
						factory, dictionary, key, ref value, this))
					{
						consumed = true;
					}
				}
			}

			if (descriptor != null && descriptor.Setters != null)
			{
				foreach (IDictionaryPropertySetter setter in descriptor.Setters)
				{
					if (!setter.SetPropertyValue(
						factory, dictionary, key, ref value, this))
					{
						consumed = true;
					}
				}
			}

			return !consumed;
		}

		/// <summary>
		/// Adds the dictionary setter.
		/// </summary>
		/// <param name="setters">The setter.</param>
		public PropertyDescriptor AddSetter(params IDictionaryPropertySetter[] setters)
		{
			return AddSetters((ICollection<IDictionaryPropertySetter>)setters);
		}

		/// <summary>
		/// Adds the dictionary setters.
		/// </summary>
		/// <param name="sets">The setters.</param>
		public PropertyDescriptor AddSetters(ICollection<IDictionaryPropertySetter> sets)
		{
			if (sets != null)
			{
				if (setters == null)
				{
					setters = new List<IDictionaryPropertySetter>(sets);
				}
				else
				{
					setters.AddRange(sets);
				}
			}
			return this;
		}

		#endregion
	}
}