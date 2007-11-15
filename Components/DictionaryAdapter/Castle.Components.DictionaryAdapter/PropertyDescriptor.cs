// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	/// Describes a dictionary property.
	/// </summary>
	public class PropertyDescriptor : IDictionaryKeyBuilder,
	                                  IDictionaryPropertyGetter,
	                                  IDictionaryPropertySetter
	{
		private readonly PropertyInfo property;
		private IDictionaryPropertyGetter getter;
		private IDictionaryPropertySetter setter;
		private ICollection<IDictionaryKeyBuilder> keyBuilders;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyDescriptor"/> class.
		/// </summary>
		/// <param name="property">The property.</param>
		public PropertyDescriptor(PropertyInfo property)
		{
			this.property = property;
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
		/// Gets or sets the key builders.
		/// </summary>
		/// <value>The key builders.</value>
		public ICollection<IDictionaryKeyBuilder> KeyBuilders
		{
			get { return keyBuilders; }
			set { keyBuilders = value; }
		}

		/// <summary>
		/// Gets or sets the setter.
		/// </summary>
		/// <value>The setter.</value>
		public IDictionaryPropertySetter Setter
		{
			get { return setter; }
			set { setter = value; }
		}

		/// <summary>
		/// Gets or sets the getter.
		/// </summary>
		/// <value>The getter.</value>
		public IDictionaryPropertyGetter Getter
		{
			get { return getter; }
			set { getter = value; }
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

			if (descriptor != null)
			{
				key = descriptor.GetKey(dictionary, key, null);
			}

			return key;
		}

		/// <summary>
		/// Adds the key builder.
		/// </summary>
		/// <param name="builder">The builder.</param>
		public void AddKeyBuilder(IDictionaryKeyBuilder builder)
		{
			if (keyBuilders == null)
			{
				keyBuilders = new IDictionaryKeyBuilder[] { builder };
			}
			else
			{
				keyBuilders.Add(builder);
			}
		}

		/// <summary>
		/// Adds the key builders.
		/// </summary>
		/// <param name="builders">The builders.</param>
		public void AddKeyBuilders(ICollection<IDictionaryKeyBuilder> builders)
		{
			if (keyBuilders == null)
			{
				keyBuilders = builders;
			}
			else if (builders != null)
			{
				foreach (IDictionaryKeyBuilder builder in builders)
				{
					keyBuilders.Add(builder);
				}
			}
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
			if (getter != null)
			{
				storedValue = getter.GetPropertyValue(
					factory, dictionary, key, storedValue, this);
			}

			if (descriptor != null)
			{
				storedValue = descriptor.GetPropertyValue(
					factory, dictionary, key, storedValue, null);
			}

			return storedValue;
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
		public object SetPropertyValue(
			IDictionaryAdapterFactory factory, IDictionary dictionary,
			string key, object value, PropertyDescriptor descriptor)
		{
			if (setter != null)
			{
				value = setter.SetPropertyValue(
					factory, dictionary, key, value, this);
			}

			if (descriptor != null)
			{
				value = descriptor.SetPropertyValue(
					factory, dictionary, key, value, null);
			}

			return value;
		}

		#endregion
	}
}