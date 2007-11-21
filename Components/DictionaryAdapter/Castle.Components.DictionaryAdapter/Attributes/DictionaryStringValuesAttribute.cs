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
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Reflection;

	/// <summary>
	/// Converts all properties to strings.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property,
		AllowMultiple = false, Inherited = true)]
	public class DictionaryStringValuesAttribute : Attribute, IDictionaryPropertySetter
	{
		private string format;

		/// <summary>
		/// Gets or sets the format.
		/// </summary>
		/// <value>The format.</value>
		public string Format
		{
			get { return format; }
			set { format = value; }
		}

		#region IDictionaryPropertySetter Members

		object IDictionaryPropertySetter.SetPropertyValue(
			IDictionaryAdapterFactory factory, IDictionary dictionary,
			string key, object value, PropertyDescriptor property)
		{
			if (value != null)
			{
				return GetPropertyAsString(property.Property, value);
			}
			return value;
		}

		#endregion

		private string GetPropertyAsString(PropertyInfo property, object value)
		{
			TypeConverter converter = GetTypeConverter(property);
			if (converter != null)
			{
				if (converter.CanConvertTo(typeof(string)))
				{
					return (string) converter.ConvertTo(value, typeof(string));
				}
			}

			if (!string.IsNullOrEmpty(format))
			{
				return String.Format(format, value);
			}
			return value.ToString();
		}

		private TypeConverter GetTypeConverter(PropertyInfo property)
		{
			Type converterType = AttributesUtil.GetTypeConverter(property);
			if (converterType != null)
			{
				return (TypeConverter) Activator.CreateInstance(converterType);
			}
			return null;
		}
	}
}