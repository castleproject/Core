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
	/// Validate that this property has the required length (either exact or in a range)
	/// </summary>
	[Serializable, CLSCompliant(false)]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Parameter, AllowMultiple = true)]
	public class ValidateRangeAttribute : AbstractValidationAttribute
	{
		private readonly IValidator validator;

		/// <summary>
		/// Initializes an integer-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>int.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>int.MaxValue</c> if this should not be tested.</param>
		public ValidateRangeAttribute(int min, int max)
		{
			validator = new RangeValidator(min, max);
		}

		/// <summary>
		/// Initializes an integer-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>int.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>int.MaxValue</c> if this should not be tested.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateRangeAttribute(int min, int max, string errorMessage) : base(errorMessage)
		{
			validator = new RangeValidator(min, max);
		}

		/// <summary>
		/// Initializes an decimal-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>decimal.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>decimal.MaxValue</c> if this should not be tested.</param>
		public ValidateRangeAttribute(decimal min, decimal max)
		{
			validator = new RangeValidator(min, max);
		}

		/// <summary>
		/// Initializes an decimal-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>decimal.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>decimal.MaxValue</c> if this should not be tested.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateRangeAttribute(decimal min, decimal max, string errorMessage)
			: base(errorMessage)
		{
			validator = new RangeValidator(min, max);
		}


		/// <summary>
		/// Initializes a string-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>String.Empty</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>String.Empty</c> if this should not be tested.</param>
		public ValidateRangeAttribute(string min, string max)
		{
			validator = new RangeValidator(min, max);
		}

		/// <summary>
		/// Initializes a string-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>String.Empty</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>String.Empty</c> if this should not be tested.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateRangeAttribute(string min, string max, string errorMessage) : base(errorMessage)
		{
			validator = new RangeValidator(min, max);
		}

		/// <summary>
		/// Initializes a DateTime-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>DateTime.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>DateTime.MaxValue</c> if this should not be tested.</param>
		public ValidateRangeAttribute(DateTime min, DateTime max)
		{
			validator = new RangeValidator(min, max);
		}

		/// <summary>
		/// Initializes a DateTime-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>DateTime.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>DateTime.MaxValue</c> if this should not be tested.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateRangeAttribute(DateTime min, DateTime max, string errorMessage) : base(errorMessage)
		{
			validator = new RangeValidator(min, max);
		}

		/// <summary>
		/// Initializes a range validator of a specified type.
		/// </summary>
		/// <param name="type">The data type to be used by the range validator.</param>
		/// <param name="min">The minimum value, or <c>DateTime.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>DateTime.MaxValue</c> if this should not be tested.</param>
		public ValidateRangeAttribute(RangeValidationType type, object min, object max) : base()
		{
			validator = new RangeValidator(type, min, max);
		}

		/// <summary>
		/// Initializes a range validator of a specified type.
		/// </summary>
		/// <param name="type">The data type to be used by the range validator.</param>
		/// <param name="min">The minimum value, or <c>DateTime.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>DateTime.MaxValue</c> if this should not be tested.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateRangeAttribute(RangeValidationType type, object min, object max, string errorMessage)
			: base(errorMessage)
		{
			validator = new RangeValidator(type, min, max);
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
