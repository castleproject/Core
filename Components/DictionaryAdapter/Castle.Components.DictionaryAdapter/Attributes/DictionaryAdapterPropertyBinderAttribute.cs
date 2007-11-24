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
	using System.Collections;

	/// <summary>
	/// Allows the user to convert the values in the dictionary into a different type on access.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class DictionaryAdapterPropertyBinderAttribute : 
		Attribute, IDictionaryPropertyGetter, IDictionaryPropertySetter
	{
		private readonly DictionaryAdapterPropertyBinder binder;

		/// <summary>
		/// Specifies a binder to perform conversion between the type currently stored in the 
		/// adapted dictionary, and the type the client code wishes to use via the interface.
		/// </summary>
		/// <param name="binderType"></param>
		public DictionaryAdapterPropertyBinderAttribute(Type binderType)
		{
			if (!typeof(DictionaryAdapterPropertyBinder).IsAssignableFrom(binderType))
				throw new ArgumentException("You may only supply DictionaryAdapterPropertyBinder types to the DictionaryAdapterPropertyBinderAttribute.");

			binder = (DictionaryAdapterPropertyBinder) Activator.CreateInstance(binderType);
		}

		#region IDictionaryPropertyGetter

		object IDictionaryPropertyGetter.GetPropertyValue(
			IDictionaryAdapterFactory factory, IDictionary dictionary,
			string key, object storedValue, PropertyDescriptor property)
		{
			return binder.ConvertFromDictionary(storedValue);
		}

		#endregion

		#region IDictionaryPropertySetter

		bool IDictionaryPropertySetter.SetPropertyValue(
			IDictionaryAdapterFactory factory, IDictionary dictionary,
			string key, ref object value, PropertyDescriptor property)
		{
			value = binder.ConvertFromInterface(value);
			return true;
		}

		#endregion
	}
}
