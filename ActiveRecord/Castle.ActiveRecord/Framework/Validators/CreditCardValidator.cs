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

namespace Castle.ActiveRecord.Framework.Validators
{
	using System;
	using System.Text;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Ensures that a property's string representation is within the desired length limitations.
	/// </summary>
	[Serializable]
	public class CreditCardValidator : AbstractValidator
	{
		private CardType allowedTypes = CardType.All;
		private string[] exceptions = new string[] {};

		public CreditCardValidator()
		{}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="allowedTypes">The card types to accept.</param>
		public CreditCardValidator(CardType allowedTypes)
		{
			this.allowedTypes = allowedTypes;
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="exceptions">An array of card numbers to skip checking for (eg. gateway test numbers). Only digits should be provided for the exceptions.</param>
		public CreditCardValidator(string[] exceptions)
		{
			this.exceptions = exceptions;
		}

		/// <summary>
		/// Initializes a new credit card validator.
		/// </summary>
		/// <param name="allowedTypes">The card types to accept.</param>
		/// <param name="exceptions">An array of card numbers to skip checking for (eg. gateway test numbers). Only digits should be provided for the exceptions.</param>
		public CreditCardValidator(CardType allowedTypes, string[] exceptions)
		{
			this.allowedTypes = allowedTypes;
			this.exceptions = exceptions;
		}

		public override bool Perform(object instance, object fieldValue)
		{
			//If the input is null then there's nothing to validate here
			if (fieldValue == null)
				return true;
			
			//Get the raw string
			string cardNumberRaw = fieldValue.ToString();
			
			//Strip any spaces or dashes
			string cardNumber = string.Empty;
			foreach (char digit in cardNumberRaw.ToCharArray())
			{
				if (char.IsNumber(digit))
				{
					//Keep the number
					cardNumber += digit.ToString();
				}
				else if (digit == ' ' || digit == '-')
				{
					//Skip the space or dash
				}
				else
				{
					//If it's not one of the above then it shouldn't be here
					return false;
				}
			}

			//Check if it's in the exceptions
			foreach (string exception in exceptions)
				if (cardNumber == exception)
					return true;

			//Check to see if it's in the allowed types list, has the correct initial digits and has the right number of digits
			if (! IsValidCardType(cardNumber))
				return false;

			//Check the LUHN output
			if (! IsLuhnValid(cardNumber))
				return false;

			return true;
		}

		private bool IsLuhnValid(string cardNumber)
		{
			int length = cardNumber.Length;

			int sum = 0;
			int offset = length % 2;
			byte[] digits = new ASCIIEncoding().GetBytes(cardNumber);

			for (int i = 0; i < length; i++)
			{
				digits[i] -= 48;
				if (((i + offset) % 2) == 0)
					digits[i] *= 2;

				sum += (digits[i] > 9) ? digits[i] - 9 : digits[i];
			}

			return (sum % 10 == 0);
		}

		private bool IsValidCardType(string cardNumber)
		{
			if ((allowedTypes & CardType.MasterCard) != 0)
				if (Regex.IsMatch(cardNumber, "^(51|52|53|54|55)"))
					return (cardNumber.Length == 16);

			if ((allowedTypes & CardType.VISA) != 0)
				if (Regex.IsMatch(cardNumber, "^(4)"))
					return (cardNumber.Length == 13 || cardNumber.Length == 16);

			if ((allowedTypes & CardType.Amex) != 0)
				if (Regex.IsMatch(cardNumber, "^(34|37)"))
					return (cardNumber.Length == 15);

			if ((allowedTypes & CardType.DinersClub) != 0)
				if (Regex.IsMatch(cardNumber, "^(300|301|302|303|304|305|36|38)"))
					return (cardNumber.Length == 14);

			if ((allowedTypes & CardType.enRoute) != 0)
				if (Regex.IsMatch(cardNumber, "^(2014|2149)"))
					return (cardNumber.Length == 15);

			if ((allowedTypes & CardType.Discover) != 0)
				if (Regex.IsMatch(cardNumber, "^(6011)"))
					return (cardNumber.Length == 16);

			if ((allowedTypes & CardType.JCB) != 0)
				if (Regex.IsMatch(cardNumber, "^(3)"))
					return (cardNumber.Length == 16);

			if ((allowedTypes & CardType.JCB) != 0)
				if (Regex.IsMatch(cardNumber, "^(2131|1800)"))
					return (cardNumber.Length == 15);

			if ((allowedTypes & CardType.Unknown) != 0)
				return true;

			return false;
		}

		protected override string BuildErrorMessage()
		{
			return String.Format("{0} does not appear to be a valid credit card number, or is of an unsupported type.", Property.Name);
		}

		[Flags, Serializable]
		public enum CardType
		{
			MasterCard = 0x0001,
			VISA = 0x0002,
			Amex = 0x0004,
			DinersClub = 0x0008,
			enRoute = 0x0010,
			Discover = 0x0020,
			JCB = 0x0040,
			Unknown = 0x0080,
			All = Amex | DinersClub | Discover | Discover | enRoute | JCB | MasterCard | VISA
		}
	}
}