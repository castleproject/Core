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

	using NUnit.Framework;

	[TestFixture]
	public class SelfValidationContributorTestCase
	{
		private SelfValidationContributor contributor;

		[SetUp]
		public void Init()
		{
			contributor = new SelfValidationContributor();
		}

		[Test]
		public void ObjectValidator()
		{
			TestTarget target = new TestTarget();
			target.ItemSKUs = new string[] { "ITEM-1" };
			target.Total = 10000.00;
			Assert.IsTrue(contributor.IsValid(target, RunWhen.Everytime).HasError);

			target.Total = 10.00;
			Assert.IsFalse(contributor.IsValid(target, RunWhen.Everytime).HasError);
		}

		[Test, ExpectedException(typeof(ValidationException),
			ExpectedMessage = "The class Castle.Components.Validator.Tests.ContributorsTests.SelfValidationContributorTestCase+InvalidTestTarget " +
							  "wants to use self validation, however the methods must be only taking one parameter of type ErrorSummary. Please " + 
							  "correct the following methods: Invalid, AnotherInvalid")]
		public void ThrowsOnInvalidTarget()
		{
			InvalidTestTarget target = new InvalidTestTarget();
			contributor.IsValid(target, RunWhen.Everytime);
		}

		[Test]
		public void HonorsRunWhen()
		{
			TestTarget target = new TestTarget();
			target.ItemSKUs = new string[] { "ITEM-1" };
			target.Total = 10000.00;
			Assert.IsTrue(contributor.IsValid(target, RunWhen.Everytime).HasError);
			Assert.IsTrue(target.ValidateOnUpdateRan);

			target = new TestTarget();
			target.ItemSKUs = new string[] { "ITEM-1" };
			target.Total = 10000.00;
			Assert.IsTrue(contributor.IsValid(target, RunWhen.Custom).HasError);
			Assert.IsFalse(target.ValidateOnUpdateRan);
		}

		public class TestTarget
		{
			private string[] itemSKUs;
			private double total;
			private bool _validationOnUpdateRan;

			public TestTarget()
			{
			}

			public TestTarget(string[] itemSKUs, double total)
			{
				this.itemSKUs = itemSKUs;
				this.total = total;
			}

			public string[] ItemSKUs
			{
				get { return itemSKUs; }
				set { itemSKUs = value; }
			}

			public double Total
			{
				get { return total; }
				set { total = value; }
			}

			public bool ValidateOnUpdateRan
			{
				get { return _validationOnUpdateRan; }
				set { _validationOnUpdateRan = value; }
			}

			[ValidateSelf]
			public void Validate(ErrorSummary errorSummary)
			{
				if (total > 1000 && ItemSKUs.Length == 1)
					//TODO Make it easy to register an error message with a key
					errorSummary.RegisterErrorMessage("total", "Customers cannot purchase 1 item if it is more than $1000 dollars.");
			}

			[ValidateSelf(RunWhen = RunWhen.Update)]
			public void ValidateOnUpdate(ErrorSummary errorSummary)
			{
				ValidateOnUpdateRan = true;
			}
		}

		public class InvalidTestTarget
		{
			[ValidateSelf]
			public void Valid(ErrorSummary errorSummary)
			{
			}

			[ValidateSelf]
			public void Invalid()
			{
			}

			[ValidateSelf]
			public void AnotherInvalid(out ErrorSummary errors)
			{
				errors = new ErrorSummary();
			}
		}
	}
}
