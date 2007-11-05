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

using System;

namespace Castle.Components.DictionaryAdapter
{
	/// <summary>
	/// Performs property binding for converting string values into ints.
	/// </summary>
	public class DictionaryAdapterStringToNullableIntBinder : DictionaryAdapterPropertyBinder
	{
		/// <summary>
		/// Performs the dictionary -> interface conversion for when client code is attempting to read out
		/// of the "adapted" dictionary.
		/// </summary>
		/// <param name="value">The string value to convert to the property type.</param>
		/// <returns>The converted value.</returns>
		protected override object PerformConversionFromDictionary(object value)
		{
			return value == null ? null : (int?) int.Parse((string) value);
		}

		/// <summary>
		/// Performs the interface -> dictionary conversion for when the client code is attempting to write
		/// into the "adapted" dictionary.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public override object ConvertFromInterface(object value)
		{
			return value.ToString();
		}

		/// <summary>
		/// The type expected for the value obtained from the dictionary.
		/// </summary>
		protected override Type DictionaryType
		{
			get { return typeof (string); }
		}
	}
}
