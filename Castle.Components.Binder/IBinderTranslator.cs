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
// 
namespace Castle.Components.Binder
{
	using System;

	/// <summary>
	/// Provides a way to properties on the binder target
	/// be bound to a different key in the data source.
	/// </summary>
	public interface IBinderTranslator
	{
		/// <summary>
		/// Should return the key that gathers the value 
		/// to fill the property.
		/// </summary>
		/// <param name="instanceType">
		/// The type which is the target of the binder
		/// </param>
		/// <param name="paramName">
		/// The property name in  the target type
		/// </param>
		/// <returns>
		/// A name of the source data that should be used to populate the property
		/// </returns>
		String Translate(Type instanceType, String paramName);
	}
}