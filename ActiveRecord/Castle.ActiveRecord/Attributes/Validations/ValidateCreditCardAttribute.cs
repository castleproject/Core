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

	using Framework.Validators;

	/// <summary>
	/// Properties decorated with this attribute will be validated to ensure that they represent a valid
	/// credit card number.
	/// <see ref="CreditCardValidator"/> for more details.
	/// </summary>
	/// <remarks>
	/// Note that this merely check the validity of the format of the value, not whatever this it an existing and valid one
	/// </remarks>
	/// <example>
	/// //just validate that this is a credit card of any type
	/// [ValidateCreditCard("You didn't fill the credit card infomration correctly.")]
	/// 
	/// //Validate that this is a VISA or Discover card
	/// [ValidateCreditCard(CreditCardValidator.CardType.VISA | CreditCardValidator.CardType.Discover, 
	///		"You need to specify a VISA or Discover card")]
	/// </example>
	[Serializable, CLSCompliant(false)]
	public class ValidateCreditCardAttribute : AbstractValidationAttribute
	{
		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		public ValidateCreditCardAttribute()
			: base(new CreditCardValidator())
		{
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		public ValidateCreditCardAttribute(String errorMessage)
			: base(new CreditCardValidator(), errorMessage)
		{
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="allowedTypes">The card types to accept.</param>
		public ValidateCreditCardAttribute(CreditCardValidator.CardType allowedTypes)
			: base(new CreditCardValidator(allowedTypes))
		{
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="allowedTypes">The card types to accept.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateCreditCardAttribute(CreditCardValidator.CardType allowedTypes, String errorMessage)
			: base(new CreditCardValidator(allowedTypes), errorMessage)
		{
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="exceptions">An array of card numbers to skip checking for (eg. gateway test numbers). Only digits should be provided for the exceptions.</param>
		public ValidateCreditCardAttribute(string[] exceptions)
			: base(new CreditCardValidator(exceptions))
		{
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="exceptions">An array of card numbers to skip checking for (eg. gateway test numbers). Only digits should be provided for the exceptions.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateCreditCardAttribute(string[] exceptions, String errorMessage)
			: base(new CreditCardValidator(exceptions), errorMessage)
		{
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="allowedTypes">The card types to accept.</param>
		/// <param name="exceptions">An array of card numbers to skip checking for (eg. gateway test numbers). Only digits should be provided for the exceptions.</param>
		public ValidateCreditCardAttribute(CreditCardValidator.CardType allowedTypes, string[] exceptions)
			: base(new CreditCardValidator(allowedTypes, exceptions))
		{
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="allowedTypes">The card types to accept.</param>
		/// <param name="exceptions">An array of card numbers to skip checking for (eg. gateway test numbers). Only digits should be provided for the exceptions.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateCreditCardAttribute(CreditCardValidator.CardType allowedTypes, string[] exceptions, String errorMessage)
			: base(new CreditCardValidator(allowedTypes, exceptions), errorMessage)
		{
		}
	}
}