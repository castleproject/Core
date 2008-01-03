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

namespace Castle.MonoRail.Framework
{
	using System.Collections.Generic;
	using Castle.Components.Binder;
	using Castle.Components.Validator;

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IValidatorAccessor
	{
		/// <summary>
		/// Gets the validator.
		/// </summary>
		/// <value>The validator.</value>
		ValidatorRunner Validator { get; }

		/// <summary>
		/// Gets the bound instance errors. These are errors relative to 
		/// the binding process performed for the specified instance.
		/// </summary>
		/// <value>The bound instance errors.</value>
		IDictionary<object, ErrorList> BoundInstanceErrors { get; }

		/// <summary>
		/// Populates the validator error summary with errors relative to the 
		/// validation rules associated with the target type.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="binderUsedForBinding">The binder used for binding.</param>
		void PopulateValidatorErrorSummary(object instance, ErrorSummary binderUsedForBinding);
	}
}
