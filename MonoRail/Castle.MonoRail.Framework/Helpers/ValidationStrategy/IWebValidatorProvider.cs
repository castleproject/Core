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
	/// Pendent
	/// </summary>
	public interface IWebValidatorProvider
	{
		/// <summary>
		/// Pendent
		/// </summary>
		/// <returns></returns>
		WebValidationConfiguration CreateConfiguration(IDictionary parameters);

		/// <summary>
		/// Pendent
		/// </summary>
		/// <returns></returns>
		IWebValidationGenerator CreateGenerator(InputElementType inputType, IDictionary attributes);
	}
}
