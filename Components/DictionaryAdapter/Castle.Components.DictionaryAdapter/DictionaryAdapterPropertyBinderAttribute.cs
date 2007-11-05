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
	/// Allows the user to convert the values in the dictionary into a different type on access.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class DictionaryAdapterPropertyBinderAttribute : Attribute
	{
		private readonly Type binder;

		/// <summary>
		/// Specifies a binder to perform conversion between the type currently stored in the 
		/// adapted dictionary, and the type the client code wishes to use via the interface.
		/// </summary>
		/// <param name="binder"></param>
		public DictionaryAdapterPropertyBinderAttribute(Type binder)
		{
			if (!typeof(DictionaryAdapterPropertyBinder).IsAssignableFrom(binder))
				throw new ArgumentException("You may only supply DictionaryAdapterPropertyBinder types to the DictionaryAdapterPropertyBinderAttribute.");

			this.binder = binder;
		}

		/// <summary>
		/// The binder performing the conversion.
		/// </summary>
		public Type Binder
		{
			get { return binder; }
		}
	}
}
