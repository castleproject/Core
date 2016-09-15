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
	using System.ComponentModel;
	using System.Reflection;

	/// <summary>
	/// Manages conversion between property values.
	/// </summary>
	public class DefaultPropertyGetter : IDictionaryPropertyGetter
	{
		private readonly TypeConverter converter;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultPropertyGetter"/> class.
		/// </summary>
		/// <param name="converter">The converter.</param>
		public DefaultPropertyGetter(TypeConverter converter)
		{
			this.converter = converter;
		}

		/// <summary>
		/// 
		/// </summary>
		public int ExecutionOrder
		{
			get { return DictionaryBehaviorAttribute.LastExecutionOrder; }
		}

		/// <summary>
		/// Gets the effective dictionary value.
		/// </summary>
		/// <param name="dictionaryAdapter">The dictionary adapter.</param>
		/// <param name="key">The key.</param>
		/// <param name="storedValue">The stored value.</param>
		/// <param name="property">The property.</param>
		/// <param name="ifExists">true if return only existing.</param>
		/// <returns>The effective property value.</returns>
		public object GetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, object storedValue, PropertyDescriptor property, bool ifExists)
		{
			var propertyType = property.PropertyType;

			if (storedValue != null && propertyType.IsInstanceOfType(storedValue) == false)
			{
				if (converter != null && converter.CanConvertFrom(storedValue.GetType()))
				{
					return converter.ConvertFrom(storedValue);
				}
			}

			return storedValue;
		}

		public IDictionaryBehavior Copy()
		{
			return this;
		}
	}
}
