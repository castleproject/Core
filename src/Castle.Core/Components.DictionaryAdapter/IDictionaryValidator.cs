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
	/// <summary>
	/// Contract for dictionary validation.
	/// </summary>
	public interface IDictionaryValidator
	{
		/// <summary>
		/// Determines if <see cref="IDictionaryAdapter"/> is valid.
		/// </summary>
		/// <param name="dictionaryAdapter">The dictionary adapter.</param>
		/// <returns>true if valid.</returns>
		bool IsValid(IDictionaryAdapter dictionaryAdapter);

		/// <summary>
		/// Validates the <see cref="IDictionaryAdapter"/>.
		/// </summary>
		/// <param name="dictionaryAdapter">The dictionary adapter.</param>
		/// <returns>The error summary information.</returns>
		string Validate(IDictionaryAdapter dictionaryAdapter);

		/// <summary>
		/// Validates the <see cref="IDictionaryAdapter"/> for a property.
		/// </summary>
		/// <param name="dictionaryAdapter">The dictionary adapter.</param>
		/// <param name="property">The property to validate.</param>
		/// <returns>The property summary information.</returns>
		string Validate(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property);

		/// <summary>
		/// Invalidates any results cached by the validator.
		/// </summary>
		/// <param name="dictionaryAdapter">The dictionary adapter.</param>
		void Invalidate(IDictionaryAdapter dictionaryAdapter);
	}
}
