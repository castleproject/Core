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
	/// Properties decorated with this attribute will be validated to ensure that they represent a valid
	/// credit card number.
	/// <see ref="CreditCardValidator"/> for more details.
	/// </summary>
	[Serializable, CLSCompliant(false)]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Parameter, AllowMultiple = true)]
	public class ValidateCreditCardAttribute : AbstractValidationAttribute
	{
		private readonly IValidator validator;

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		public ValidateCreditCardAttribute()
		{
			validator = new CreditCardValidator();
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		public ValidateCreditCardAttribute(String errorMessage)
			: base (errorMessage)
		{
			validator = new CreditCardValidator();
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="allowedTypes">The card types to accept.</param>
		public ValidateCreditCardAttribute(CreditCardValidator.CardType allowedTypes)
		{
			validator = new CreditCardValidator(allowedTypes);
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="allowedTypes">The card types to accept.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateCreditCardAttribute(CreditCardValidator.CardType allowedTypes, String errorMessage)
			: base(errorMessage)
		{
			validator = new CreditCardValidator(allowedTypes);
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="exceptions">An array of card numbers to skip checking for (eg. gateway test numbers). Only digits should be provided for the exceptions.</param>
		public ValidateCreditCardAttribute(string[] exceptions)
		{
			validator = new CreditCardValidator(exceptions);
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="exceptions">An array of card numbers to skip checking for (eg. gateway test numbers). Only digits should be provided for the exceptions.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateCreditCardAttribute(string[] exceptions, String errorMessage)
			: base(errorMessage)
		{
			validator = new CreditCardValidator(exceptions);
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="allowedTypes">The card types to accept.</param>
		/// <param name="exceptions">An array of card numbers to skip checking for (eg. gateway test numbers). Only digits should be provided for the exceptions.</param>
		public ValidateCreditCardAttribute(CreditCardValidator.CardType allowedTypes, string[] exceptions)
		{
			validator = new CreditCardValidator(allowedTypes, exceptions);
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="allowedTypes">The card types to accept.</param>
		/// <param name="exceptions">An array of card numbers to skip checking for (eg. gateway test numbers). Only digits should be provided for the exceptions.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateCreditCardAttribute(CreditCardValidator.CardType allowedTypes, string[] exceptions, String errorMessage)
			: base(errorMessage)
		{
			validator = new CreditCardValidator(allowedTypes, exceptions);
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
