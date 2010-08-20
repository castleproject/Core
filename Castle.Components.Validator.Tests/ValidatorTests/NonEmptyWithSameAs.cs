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
// 
namespace Castle.Components.Validator.Tests.ValidatorTests
{
	using Models;
	using NUnit.Framework;

	[TestFixture]
	public class NonEmptyWithSameAs
	{
		private ValidatorRunner runner;

		[TestFixtureSetUp]
		public void Init()
		{
			runner = new ValidatorRunner(new CachedValidationRegistry());
		}

		[Test]
		public void COMP87_Valid()
		{
			Admin admin = new Admin();
			admin.Password = "test";
			admin.PasswordConfirmation = "test";

			Assert.IsTrue(runner.IsValid(admin));
		}

		[Test]
		public void COMP87_Invalid()
		{
			Admin admin = new Admin();
			admin.Password = string.Empty;
			admin.PasswordConfirmation = "test";

			Assert.IsFalse(runner.IsValid(admin));
			Assert.AreEqual(2, runner.GetErrorSummary(admin).ErrorsCount);
			Assert.AreEqual(1, runner.GetErrorSummary(admin).GetErrorsForProperty("Password").Length);
			Assert.AreEqual(1, runner.GetErrorSummary(admin).GetErrorsForProperty("PasswordConfirmation").Length);

			admin.Password = "test";
			admin.PasswordConfirmation = string.Empty;

			Assert.IsFalse(runner.IsValid(admin));
			Assert.AreEqual(1, runner.GetErrorSummary(admin).ErrorsCount);
			Assert.AreEqual(1, runner.GetErrorSummary(admin).GetErrorsForProperty("PasswordConfirmation").Length);
		}
	}

	public class Admin : User
	{
	}

	public class User
	{
		[ValidateNonEmpty]
		public string Password { get; set; }


		[ValidateSameAs("Password")]
		public string PasswordConfirmation { get; set; }
	} 
}