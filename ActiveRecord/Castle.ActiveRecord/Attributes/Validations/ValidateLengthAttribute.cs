// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

	/// <summary>
	/// Validate that this property has the required length (either exact or in range)
	/// </summary>
	[Serializable, CLSCompliant(false)]
	public class ValidateLengthAttribute : AbstractValidationAttribute
	{
		/// <summary>
		/// Initializes a new exact length validator.
		/// </summary>
		/// <param name="exactLength">The exact length required.</param>
		public ValidateLengthAttribute(int exactLength) : base(new LengthValidator(exactLength))
		{
		}

		/// <summary>
		/// Initializes a new exact length validator.
		/// </summary>
		/// <param name="exactLength">The exact length required.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateLengthAttribute(int exactLength, String errorMessage) : base(new LengthValidator(exactLength), errorMessage)
		{
		}

		/// <summary>
		/// Initializes a new range based length validator.
		/// </summary>
		/// <param name="minLength">The minimum length, or <c>int.MinValue</c> if this should not be tested.</param>
		/// <param name="maxLength">The maximum length, or <c>int.MaxValue</c> if this should not be tested.</param>
		public ValidateLengthAttribute(int minLength, int maxLength) : base(new LengthValidator(minLength, maxLength))
		{
		}

		/// <summary>
		/// Initializes a new range based length validator.
		/// </summary>
		/// <param name="minLength">The minimum length, or <c>int.MinValue</c> if this should not be tested.</param>
		/// <param name="maxLength">The maximum length, or <c>int.MaxValue</c> if this should not be tested.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateLengthAttribute(int minLength, int maxLength, String errorMessage) : base(new LengthValidator(minLength, maxLength), errorMessage)
		{
		}
	}
}