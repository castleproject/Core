// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord
{
	using System;

	using Castle.ActiveRecord.Framework.Validators;
	using Castle.Components.Validator;


	/// <summary>
	/// Validate that the property's value is unique in the database when saved
	/// </summary>
	[Serializable, CLSCompliant(false)]
	public class ValidateIsUniqueAttribute : AbstractValidationAttribute
	{
		private readonly IValidator validator;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateIsUniqueAttribute"/> class.
		/// </summary>
		public ValidateIsUniqueAttribute()
		{
			validator = new IsUniqueValidator();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateIsUniqueAttribute"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		public ValidateIsUniqueAttribute(String errorMessage)
			: base(errorMessage)
		{
			validator = new IsUniqueValidator();
		}

		/// <summary>
		/// Constructs and configures an <see cref="IValidator"/>
		/// instance based on the properties set on the attribute instance.
		/// </summary>
		/// <returns></returns>
		public override IValidator Build()
		{
			ConfigureValidatorMessage(validator);

			return validator;
		}
	}
}
