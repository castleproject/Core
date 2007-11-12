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

	/// <summary>
	/// The exception throw when the property binder cannot convert a value
	/// from the dictionary type into the interface type.
	/// </summary>
	public class DictionaryAdapterPropertyBindingException : ApplicationException
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="binder">The type of binder being used.</param>
		/// <param name="value">The value that caused the exception.</param>
		/// <param name="interfaceType">The type the binder was attempting to convert the value into.</param>
		public DictionaryAdapterPropertyBindingException(Type binder, object value, Type interfaceType)
			: base(FormatMessage(binder, value, interfaceType))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="binder">The type of binder being used.</param>
		/// <param name="value">The value that caused the exception.</param>
		/// <param name="interfaceType">The type the binder was attempting to convert the value into.</param>
		/// <param name="innerException">An exception that occurred during conversion.</param>
		public DictionaryAdapterPropertyBindingException(Type binder, object value, Type interfaceType, Exception innerException)
			: base(FormatMessage(binder, value, interfaceType), innerException)
		{
		}

		private static string FormatMessage(Type binder, object value, Type interfaceType)
		{
			return string.Format("The value being bound to an interface by the DictionaryAdapterFactory " +
				"was not of the type expected by the {0}, or caused the conversion to fail. The type expected " +
				"was {1}. The value was '{2}' of type {3}.", binder, interfaceType, value, value.GetType());
		}
	}
}
