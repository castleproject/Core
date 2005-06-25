// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Define the basic contract for validators
	/// </summary>
	public interface IValidator
	{
		/// <summary>
		/// Implementors should perform any initialization logic
		/// </summary>
		/// <param name="property">The target property</param>
		void Initialize(PropertyInfo property);

		/// <summary>
		/// Implementors should perform the actual validation upon
		/// the property value
		/// </summary>
		/// <param name="instance"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		bool Perform(object instance);

		/// <summary>
		/// Implementors should perform the actual validation upon
		/// the property value
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		bool Perform(object instance, object fieldValue);

		/// <summary>
		/// The target property
		/// </summary>
		PropertyInfo Property { get; }

		/// <summary>
		/// The error message to be displayed if the validation fails
		/// </summary>
		String ErrorMessage { get; set; }
	}
}
