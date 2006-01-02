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
		public string passConfirmation;

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
	}
}
