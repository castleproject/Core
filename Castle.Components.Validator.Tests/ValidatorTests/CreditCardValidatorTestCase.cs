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

namespace Castle.Components.Validator.Tests.ValidatorTests
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using NUnit.Framework;

	[TestFixture]
	public class CreditCardValidatorTestCase
	{
		#region Credit Card Validator Test

		[Test]
		public void CreditCardValidatorTest()
		{
			Assert.IsTrue(CreditCardValidatorTest(null, CreditCardValidator.CardType.All, new string[] { }));
			Assert.IsTrue(CreditCardValidatorTest("2323-2005 77663 554", CreditCardValidator.CardType.Unknown, new string[] { }));
			Assert.IsFalse(CreditCardValidatorTest("3323-2005 77663 554", CreditCardValidator.CardType.Unknown, new string[] { }));
			Assert.IsFalse(CreditCardValidatorTest("3323-2005 77663 554", CreditCardValidator.CardType.Unknown, new string[] { "3323-2005-7766-3554" }));
			Assert.IsTrue(CreditCardValidatorTest("3323-2005 77663 554", CreditCardValidator.CardType.Unknown, new string[] { "3323200577663554" }));
			Assert.IsTrue(CreditCardValidatorTest("4111-1111 1111 1111", CreditCardValidator.CardType.VISA, new string[] { }));
			Assert.IsTrue(CreditCardValidatorTest("3400-0000 0000009", CreditCardValidator.CardType.Amex, new string[] { }));
			Assert.IsTrue(CreditCardValidatorTest("3400-0000 0000 009", CreditCardValidator.CardType.Amex, new string[] { }));
			Assert.IsTrue(CreditCardValidatorTest("6011-0000 00000004", CreditCardValidator.CardType.Discover, new string[] { }));
			Assert.IsTrue(CreditCardValidatorTest("5500-0000 000 00004", CreditCardValidator.CardType.MasterCard, new string[] { }));
		}

		private bool CreditCardValidatorTest(string input, CreditCardValidator.CardType allowedTypes, string[] exceptions)
		{
			CreditCardValidator validator = new CreditCardValidator(allowedTypes, exceptions);
			return validator.IsValid(this, input);
		}

		#endregion
	}
}
