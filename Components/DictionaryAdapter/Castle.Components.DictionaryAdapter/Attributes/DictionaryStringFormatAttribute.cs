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
	/// Provides simple formatting form existing properties.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class DictionaryStringFormatAttribute : DictionaryBehaviorAttribute, IDictionaryPropertyGetter
	{
		public DictionaryStringFormatAttribute(string format, params string[] properties)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}

			Format = format;
			Properties = properties;
		}

		/// <summary>
		/// Gets the string format.
		/// </summary>
		public string Format { get; private set; }

		/// <summary>
		/// Gets the format properties.
		/// </summary>
		public string[] Properties { get; private set; }

		#region IDictionaryPropertyGetter

		object IDictionaryPropertyGetter.GetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, object storedValue, PropertyDescriptor property)
		{
			return string.Format(Format, GetFormatArguments(dictionaryAdapter, property.Property.Name));
		}

		#endregion

		private object[] GetFormatArguments(IDictionaryAdapter dictionaryAdapter, string formattedPropertyName)
		{
			var arguments = new object[Properties.Length];
			for (int i = 0; i < Properties.Length; ++i)
			{
				var propertyName = Properties[i];
				if (propertyName != formattedPropertyName)
				{
					arguments[i] = dictionaryAdapter.GetProperty(propertyName);
				}
				else
				{
					arguments[i] = "(recursive)";
				}
			}
			return arguments;
		}
	}
}