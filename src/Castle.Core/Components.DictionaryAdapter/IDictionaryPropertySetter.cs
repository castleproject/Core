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
	/// Defines the contract for updating dictionary values.
	/// </summary>
	public interface IDictionaryPropertySetter : IDictionaryBehavior
	{
		/// <summary>
		/// Sets the stored dictionary value.
		/// </summary>
		/// <param name="dictionaryAdapter">The dictionary adapter.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The stored value.</param>
		/// <param name="property">The property.</param>
		/// <returns>true if the property should be stored.</returns>
		bool SetPropertyValue(IDictionaryAdapter dictionaryAdapter, string key, ref object value, 
							  PropertyDescriptor property);
	}
}