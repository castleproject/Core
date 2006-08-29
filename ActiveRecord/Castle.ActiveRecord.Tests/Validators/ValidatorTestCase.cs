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

namespace Castle.ActiveRecord.Tests.Validators
{
	using System;

	using NUnit.Framework;

	using Castle.ActiveRecord.Framework.Validators;

	[TestFixture]
	public class ValidatorTestCase
	{
		[Test]
		public void EmailValidatorTest()
		{
			EmailValidator validator = new EmailValidator();

			Assert.IsFalse( validator.Perform(this, "hammett") );
			Assert.IsTrue( validator.Perform(this, null) );
			Assert.IsTrue( validator.Perform(this, "hammett@gmail.com") );
			Assert.IsTrue( validator.Perform(this, "hammett@aol.com.br") );
		}

		[Test]
		public void NullCheckTest()
		{
			NullCheckValidator validator = new NullCheckValidator();

			Assert.IsTrue( validator.Perform(this, "hammett") );
			Assert.IsFalse( validator.Perform(this, null) );
			Assert.IsFalse( validator.Perform(this, "") );
			Assert.IsTrue( validator.Perform(this, "hammett@gmail.com") );
			Assert.IsTrue( validator.Perform(this, "hammett@aol.com.br") );
		}

		#region Confirmation Validator Test

		public string passConfirmation;

		[Test]
		public void ConfirmationValidatorTest()
		{
			ConfirmationValidator validator = new ConfirmationValidator("passConfirmation");

			passConfirmation = "123";
			Assert.IsTrue( validator.Perform(this, "123") );

			passConfirmation = "123x";
			Assert.IsFalse( validator.Perform(this, "123") );

			validator = new ConfirmationValidator("PassConfirmation");

			passConfirmation = "123";
			Assert.IsTrue( validator.Perform(this, "123") );

			passConfirmation = "321";
			Assert.IsFalse( validator.Perform(this, "123") );

			passConfirmation = "";
			Assert.IsFalse( validator.Perform(this, "123") );

			passConfirmation = null;
			Assert.IsFalse( validator.Perform(this, "123") );
		}

		public string PassConfirmation
		{
			get { return passConfirmation; }
		}
		
		#endregion

		#region Length Validator Test

		[Test]
		public void LengthValidatorTest()
		{
			Assert.IsTrue(LengthValidatorTest("", 0));
			Assert.IsTrue(LengthValidatorTest(null, 0));
			Assert.IsTrue(LengthValidatorTest(null, 100));
			Assert.IsTrue(LengthValidatorTest("abc", 3));
			Assert.IsFalse(LengthValidatorTest("abcd", 3));

			Assert.IsTrue(LengthValidatorTest("", 0, 5));
			Assert.IsTrue(LengthValidatorTest(null, 0, 5));
			Assert.IsTrue(LengthValidatorTest(null, 100, 110));
			Assert.IsTrue(LengthValidatorTest("abc", 3, 3));
			Assert.IsFalse(LengthValidatorTest("a", 2, 4));
			Assert.IsTrue(LengthValidatorTest("ab", 2, 4));
			Assert.IsTrue(LengthValidatorTest("abc", 2, 4));
			Assert.IsTrue(LengthValidatorTest("abcd", 2, 4));
			Assert.IsFalse(LengthValidatorTest("abcde", 2, 4));

			// minimum only
			Assert.IsTrue(LengthValidatorTest("abcde", 2, int.MaxValue));
			Assert.IsFalse(LengthValidatorTest("abcde", 6, int.MaxValue));

			// maximum only
			Assert.IsFalse(LengthValidatorTest("abcde", int.MinValue, 2));
			Assert.IsTrue(LengthValidatorTest("abcde", int.MinValue, 6));
		}

		private bool LengthValidatorTest(string input, int exactLength)
		{
			LengthValidator validator = new LengthValidator(exactLength);
			return validator.Perform(this, input);
		}

		private bool LengthValidatorTest(string input, int minLength, int maxLength)
		{
			LengthValidator validator = new LengthValidator(minLength, maxLength);
			return validator.Perform(this, input);
		}

		#endregion

		#region Credit Card Validator Test

		[Test]
		public void CreditCardValidatorTest()
		{
			Assert.IsTrue(CreditCardValidatorTest(null, CreditCardValidator.CardType.All, new string[] { }));
			Assert.IsTrue(CreditCardValidatorTest("2323-2005 77663 554", CreditCardValidator.CardType.Unknown, new string[] { }));
			Assert.IsFalse(CreditCardValidatorTest("3323-2005 77663 554", CreditCardValidator.CardType.Unknown, new string[] { }));
			Assert.IsFalse(CreditCardValidatorTest("3323-2005 77663 554", CreditCardValidator.CardType.Unknown, new string[] { "3323-2005-7766-3554" }));
			Assert.IsTrue(CreditCardValidatorTest("3323-2005 77663 554", CreditCardValidator.CardType.Unknown, new string[] { "3323200577663554" }));
			Assert.IsTrue(CreditCardValidatorTest("4111-1111 1111 1111", CreditCardValidator.CardType.VISA, new string[] {}));
			Assert.IsTrue(CreditCardValidatorTest("3400-0000 0000009", CreditCardValidator.CardType.Amex, new string[] {}));
			Assert.IsTrue(CreditCardValidatorTest("3400-0000 0000 009", CreditCardValidator.CardType.Amex, new string[] {}));
			Assert.IsTrue(CreditCardValidatorTest("6011-0000 00000004", CreditCardValidator.CardType.Discover, new string[] {}));
			Assert.IsTrue(CreditCardValidatorTest("5500-0000 000 00004", CreditCardValidator.CardType.MasterCard, new string[] {}));
		}

		private bool CreditCardValidatorTest(string input, CreditCardValidator.CardType allowedTypes, string[] exceptions)
		{
			CreditCardValidator validator = new CreditCardValidator(allowedTypes, exceptions);
			return validator.Perform(this, input);
		}
		
		#endregion
	}
}
