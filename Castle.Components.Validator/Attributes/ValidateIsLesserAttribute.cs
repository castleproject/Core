// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Validator
{
	using System;

	/// <summary>
	/// Validate a field value is greater than another one.
	/// </summary>
	[Serializable, CLSCompliant(false)]
	public class ValidateIsLesserAttribute : AbstractCrossReferenceValidationAttributre
	{
		private readonly IValidator validator;

		#region Contructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateIsLesserAttribute"/> class.
		/// </summary>
		/// <param name="type"><see cref="IsLesserValidationType"/>The data type to compare.</param>
		/// <param name="propertyToCompare">Target property to compare</param>
		public ValidateIsLesserAttribute( IsLesserValidationType type, string propertyToCompare )
			: base(propertyToCompare)
		{
			validator = new IsLesserValidator(type, propertyToCompare);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateIsLesserAttribute"/> class.
		/// </summary>
		/// <param name="type"><see cref="IsLesserValidationType"/>The data type to compare.</param>
		/// <param name="propertyToCompare">Target property to compare</param>
		/// <param name="errorMessage">The error message.</param>
		public ValidateIsLesserAttribute( IsLesserValidationType type, string propertyToCompare, string errorMessage )
			: base(propertyToCompare, errorMessage)
		{
			validator = new IsLesserValidator( type, propertyToCompare );
		}

		#endregion

		#region Object Overrides

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

		#endregion
	}
}
