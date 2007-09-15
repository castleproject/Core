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

namespace Castle.MonoRail.Framework.Helpers.ValidationStrategy
{
	using System.Collections;
	using Castle.Components.Validator;

	/// <summary>
	/// Abstracts the approach to create javascript
	/// validation for forms.
	/// </summary>
	public interface IBrowserValidatorProvider
	{
		/// <summary>
		/// Implementors should attempt to read their specific configuration 
		/// from the <paramref name="parameters"/>, configure and return 
		/// a class that extends from <see cref="BrowserValidationConfiguration"/>
		/// </summary>
		/// <returns>An instance that extends from <see cref="BrowserValidationConfiguration"/></returns>
		BrowserValidationConfiguration CreateConfiguration(IDictionary parameters);

		/// <summary>
		/// Implementors should return their generator instance. 
		/// </summary>
		/// <returns>A generator instance</returns>
		IBrowserValidationGenerator CreateGenerator(BrowserValidationConfiguration config,
		                                            InputElementType inputType, IDictionary attributes);
	}
}