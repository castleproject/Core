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

using System.Globalization;

namespace Castle.Components.Validator.Tests
{
	using Castle.Components.Validator.Tests.Models;
	using NUnit.Framework;

	[TestFixture]
	public class ValidatorRunnerTestCase
	{
		private ValidatorRunner runner;

		[SetUp]
		public virtual void Init()
		{
			runner = new ValidatorRunner(new CachedValidationRegistry());
		}

		[Test]
		public void IsValidForEverything()
		{
			Person person = new Person();
			Assert.IsFalse(runner.IsValid(person));

			person = new Person(1, 27, "hammett", "100, street");
			Assert.IsTrue(runner.IsValid(person));
		}

		[Test]
		public void IsValidForInsertUpdate()
		{
			InsertUpdateClass obj = new InsertUpdateClass();

			Assert.IsFalse(runner.IsValid(obj, RunWhen.Insert));

			obj.Prop1 = "value";
			obj.Prop2 = "value";

			Assert.IsTrue(runner.IsValid(obj, RunWhen.Insert));

			Assert.IsFalse(runner.IsValid(obj, RunWhen.Update));

			obj.Prop3 = "value";
			obj.Prop4 = "value";

			Assert.IsTrue(runner.IsValid(obj, RunWhen.Update));
		}

		[Test]
		public void InheritanceTest()
		{
			Client client = new Client();
			Assert.IsFalse(runner.IsValid(client));
			client = new Client(1, 27, "hammett", "100, street", "hammett@gmail.com", "123", "123");
			Assert.IsTrue(runner.IsValid(client));
		}

		[Test]
		public void GroupValidation()
		{
			Client client = new Client();
			client.Email = "foo@bar.com";
			Assert.IsFalse(runner.IsValid(client));

			Assert.AreEqual(0, runner.GetErrorSummary(client).GetErrorsForProperty("Email").Length);
			Assert.AreEqual(0, runner.GetErrorSummary(client).GetErrorsForProperty("Password").Length);
		}

		[Test]
		public void ExecutesCustomContributors()
		{
			ValidatorRunner runnerWithContributor = new ValidatorRunner(true, new CachedValidationRegistry(),
				new IValidationContributor[] {new AlwaysErrorContributor()});

			object target = new object();
			Assert.IsFalse(runnerWithContributor.IsValid(target));
			ErrorSummary errors = runnerWithContributor.GetErrorSummary(target);
			Assert.IsTrue(errors.HasError);
			Assert.AreEqual(1, errors.ErrorsCount);
			string[] errorsForKey = errors.GetErrorsForProperty("someKey");
			Assert.AreEqual(1, errorsForKey.Length);
			Assert.AreEqual("error", errorsForKey[0]);
		}

		[Test]
		public void ExecutesSelfValidationByDefault()
		{
			SelfValidationTestTarget target = new SelfValidationTestTarget();
			Assert.IsFalse(runner.IsValid(target));
			ErrorSummary errors = runner.GetErrorSummary(target);
			Assert.IsTrue(errors.HasError);
			Assert.AreEqual(1, errors.ErrorsCount);
			string[] errorsForKey = errors.GetErrorsForProperty("errorKey");
			Assert.AreEqual(1, errorsForKey.Length);
			Assert.AreEqual("errorMessage", errorsForKey[0]);
		}

		private class SelfValidationTestTarget
		{
			[ValidateSelf]
			public void Validate(ErrorSummary errors)
			{
				errors.RegisterErrorMessage("errorKey", "errorMessage");
			}
		}

		private class AlwaysErrorContributor : IValidationContributor
		{
			public ErrorSummary IsValid(object instance, RunWhen runWhen)
			{
				ErrorSummary errors =  new ErrorSummary();
				errors.RegisterErrorMessage("someKey", "error");
				return errors;
			}
		}

	}

	[TestFixture]
	public class ValidatorRunnerTestCaseForComp58 : ValidatorRunnerTestCase 
	{
		private CultureInfo backupculture, backupuiculture;

		[SetUp]
		public override void Init() 
		{
			backupculture = System.Threading.Thread.CurrentThread.CurrentCulture;
			backupuiculture = System.Threading.Thread.CurrentThread.CurrentUICulture;

			// switch to a culture for which message resource is available but messages are not defined
			System.Threading.Thread.CurrentThread.CurrentUICulture =
				System.Threading.Thread.CurrentThread.CurrentCulture =
				new CultureInfo("mk-MK");

			base.Init();
		}	

		[TearDown]
		public void DeInit() 
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = backupculture;
			System.Threading.Thread.CurrentThread.CurrentUICulture = backupuiculture;
		}
	}
}
