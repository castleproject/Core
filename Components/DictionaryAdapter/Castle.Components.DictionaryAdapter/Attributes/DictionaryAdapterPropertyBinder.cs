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
	using System.Reflection;

	/// <summary>
	/// Base class for property binders which defines the bidirectional conversion methods.
	/// </summary>
	public abstract class DictionaryAdapterPropertyBinder
	{
		/// <summary>
		/// Performs the dictionary -> interface conversion for when client code is attempting to read out
		/// of the "adapted" dictionary.
		/// </summary>
		/// <param name="value">The string value to convert to the property type.</param>
		/// <returns>The converted value.</returns>
		public object ConvertFromDictionary(object value)
		{
			AssertDictionaryValueHasExpectedType(value, DictionaryType);
			return PerformConversionFromDictionary(value);
		}


		/// <summary>
		/// Gets the type of the dictionary.
		/// </summary>
		/// <value>The type of the dictionary.</value>
		protected abstract Type DictionaryType { get; }

		/// <summary>
		/// Performs the conversion from dictionary.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		protected abstract object PerformConversionFromDictionary(object value);

		/// <summary>
		/// Performs the interface -> dictionary conversion for when the client code is attempting to write
		/// into the "adapted" dictionary.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public abstract object ConvertFromInterface(object value);

		/// <summary>
		/// Asserts that the type in the dictionary is the type expected, and if not, throws a meaningful
		/// exception rather than InvalidCastException.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="interfaceType"></param>
		protected void AssertDictionaryValueHasExpectedType(object value, Type interfaceType)
		{
			if (value.GetType() != interfaceType)
			{
				throw new DictionaryAdapterPropertyBindingException(GetType(), value, interfaceType);
			}
		}
	}
}