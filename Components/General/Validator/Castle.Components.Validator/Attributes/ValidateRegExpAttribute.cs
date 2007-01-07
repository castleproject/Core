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

namespace Castle.Components.Validator
{
	using System;

	/// <summary>
	/// Validate that the property match the given regular expression
	/// </summary>
	[Serializable, CLSCompliant(false)]
	public class ValidateRegExpAttribute : AbstractValidationAttribute
	{
		private readonly string pattern;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateRegExpAttribute"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		public ValidateRegExpAttribute(String pattern)
		{
			this.pattern = pattern;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateRegExpAttribute"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="errorMessage">The error message.</param>
		public ValidateRegExpAttribute(String pattern, String errorMessage) : base(errorMessage)
		{
			this.pattern = pattern;
		}

		public override IValidator Build()
		{
			IValidator validator = new RegularExpressionValidator(pattern);

			ConfigureValidatorMessage(validator);

			return validator;
		}
	}
}