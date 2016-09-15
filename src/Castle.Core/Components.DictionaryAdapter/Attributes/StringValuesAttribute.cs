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

	/// <summary>
	/// Converts all properties to strings.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class StringValuesAttribute : DictionaryBehaviorAttribute, IDictionaryPropertySetter
	{
		/// <summary>
		/// Gets or sets the format.
		/// </summary>
		/// <value>The format.</value>
		public string Format { get; set; }

		bool IDictionaryPropertySetter.SetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, ref object value, PropertyDescriptor property)
		{
			if (value != null)
			{
				value = GetPropertyAsString(property, value);
			}
			return true;
		}

		private string GetPropertyAsString(PropertyDescriptor property, object value)
		{
			if (string.IsNullOrEmpty(Format) == false)
			{
				return String.Format(Format, value);
			}

			var converter = property.TypeConverter;

			if (converter != null && converter.CanConvertTo(typeof(string)))
			{
				return (string) converter.ConvertTo(value, typeof(string));
			}

			return value.ToString();
		}
	}
}