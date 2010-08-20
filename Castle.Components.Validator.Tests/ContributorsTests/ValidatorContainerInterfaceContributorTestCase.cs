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

namespace Castle.Components.Validator.Tests.ContributorsTests
{
	using Castle.Components.Validator.Attributes;
	using Castle.Components.Validator.Contibutors;
	using NUnit.Framework;

	[TestFixture]
	public class ValidatorContainerInterfaceContributorTestCase
	{
		private ValidatorContainerInterfaceValidationContributor contributor;

		#region test case model
		class Form1
			: IForm1Validation1
			  , IForm1Validation2
			  , IForm1Validation3
			 , IForm1Validation4
		{
			private string userId_, password_, passwordConfirmation_, magicNumber_;
			public string UserId { get { return userId_; } set { userId_ = value; } }
			public string Password { get { return password_; } set { password_ = value; } }
			public string PasswordConfirmation { get { return passwordConfirmation_; } set { passwordConfirmation_ = value; } }
			public string MagicNumber { get { return magicNumber_; } set { magicNumber_ = value;} }
		}

		[ValidatorContainerInterfaceFlag]
		interface IForm1Validation1
			: IValidatorContainerInterface
		{
			[ValidateNonEmpty]
			[ValidateLength(1)]
			string UserId { get; }
			[ValidateNonEmpty]
			[ValidateLength(8)]
			string Password { get; }
		}

		[ValidatorContainerInterfaceFlag]
		interface IForm1Validation2
			: IValidatorContainerInterface
		{
			[ValidateSameAs("Password")]
			string PasswordConfirmation { get; }
		}

		interface IForm1Validation3
			: IValidatorContainerInterface
		{
			[ValidateSameAs("Password")]
			string PasswordConfirmation { get; }
		}

		[ValidatorContainerInterfaceFlag]
		interface IForm1Validation4
			: IValidatorContainerInterface {
			[ValidateInteger(RunWhen = RunWhen.Update)]
			string MagicNumber { get; }
		}

		#endregion


		[SetUp]
		public void Init() {
			contributor = new ValidatorContainerInterfaceValidationContributor();

			new ValidatorRunner(new IValidationContributor[] {contributor}, new CachedValidationRegistry());
		}



		[Test]
		public void AssertErrorsAreFoundInImplementedInterfaceMembers()
		{
			Form1 model = new Form1();
			ErrorSummary errorsummary = contributor.IsValid(model, RunWhen.Everytime);
			
			// validation should fail because there are ValidateNonEmpty
			// and ValidateLength attributes mismatching on base interface
			// of Form1
			Assert.IsTrue(errorsummary.HasError);
			
			model.UserId = "a";
			model.PasswordConfirmation = model.Password = "12345678";
			errorsummary = contributor.IsValid(model, RunWhen.Everytime);
			Assert.IsFalse(errorsummary.HasError);
			
		}

		[Test]
		public void HonorsRunWhen()
		{
			Form1 model = new Form1();
			model.UserId = "a";
			model.PasswordConfirmation = model.Password = "12345678";

			model.MagicNumber = "i";
			ErrorSummary errorsummary = contributor.IsValid(model, RunWhen.Update);

			// validation should fail MagicNumber is invalid on update
			Assert.IsTrue(errorsummary.HasError);

			errorsummary = contributor.IsValid(model, RunWhen.Insert);
			Assert.IsFalse(errorsummary.HasError);

			model.MagicNumber = "1";
			errorsummary = contributor.IsValid(model, RunWhen.Update);
			Assert.IsFalse(errorsummary.HasError);
			
		}

	}
}
