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
	/// <summary>
	/// Validates that the content has a different
	/// value from the property informed.
	/// </summary>
	public class ValidateNotSameAsAttribute : AbstractCrossReferenceValidationAttributre
	{
		private readonly string propertyToCompare;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateNotSameAsAttribute"/> class.
		/// </summary>
		/// <param name="propertyToCompare">The property to compare.</param>
		public ValidateNotSameAsAttribute(string propertyToCompare)
			: base(propertyToCompare)
		{
			this.propertyToCompare = propertyToCompare;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateNotSameAsAttribute"/> class.
		/// </summary>
		/// <param name="propertyToCompare">The property to compare.</param>
		/// <param name="errorMessage">The error message.</param>
		public ValidateNotSameAsAttribute(string propertyToCompare, string errorMessage) 
			: base(propertyToCompare, errorMessage)
		{
			this.propertyToCompare = propertyToCompare;
		}

		/// <summary>
		/// Constructs and configures an <see cref="IValidator"/>
		/// instance based on the properties set on the attribute instance.
		/// </summary>
		/// <returns></returns>
		public override IValidator Build()
		{
			IValidator validator = new NotSameAsValidator(propertyToCompare);

			ConfigureValidatorMessage(validator);

			return validator;
		}
	}
}
