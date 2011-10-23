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

namespace ValidatorTutorial.Tests.Validators
{
	using System;
	using System.Globalization;
	using System.Threading;
	using Castle.Components.Validator;
	using NUnit.Framework;

	[TestFixture]
	public class IsLesserValidatorTestCase
	{
		private IsLesserValidator lesserIntValidator;
		private IsLesserValidator lesserDateValidator;
    private IsLesserValidator nullableLesserDateValidator;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			lesserIntValidator = new IsLesserValidator(IsLesserValidationType.Integer, "IntCompareField");
			lesserIntValidator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("IntTarget"));

			lesserDateValidator = new IsLesserValidator(IsLesserValidationType.Date, "DateCompareField");
			lesserDateValidator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("DateTarget"));

      nullableLesserDateValidator = new IsLesserValidator(IsLesserValidationType.Date, "NullableDateCompareField");
      nullableLesserDateValidator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("NullableDateTarget"));

			target = new TestTarget();
		}

		[Test]
		public void IntegerIsLesserValid()
		{
			target.IntCompareField = 100;
			Assert.IsTrue(lesserIntValidator.IsValid(target, 99));
		}

		[Test]
		public void DateIsLesserValid()
		{
			target.DateCompareField = DateTime.Today;

			Assert.IsTrue(lesserDateValidator.IsValid(target, DateTime.Today.AddDays(-5)));
			Assert.IsTrue(lesserDateValidator.IsValid(target, DateTime.Today.AddHours(-1)));
		}

		[Test]
		public void DateIsLesserNotValid()
		{
			target.DateCompareField = DateTime.Today;

			Assert.IsFalse(lesserDateValidator.IsValid(target, DateTime.Today));
			Assert.IsFalse(lesserDateValidator.IsValid(target, DateTime.Today.AddHours(1)));
			Assert.IsFalse(lesserDateValidator.IsValid(target, DateTime.Today.AddDays(5)));
		}

    [Test]
    public void FieldValueNullIsValid()
    {
      Assert.IsTrue(nullableLesserDateValidator.IsValid(target, null));
      Assert.IsTrue(nullableLesserDateValidator.IsValid(target, ""));
    }

    [Test]
    public void ReferenceValueNullIsValid()
    {
      Assert.IsTrue(nullableLesserDateValidator.IsValid(target, DateTime.Today));
    }

		public class TestTarget
		{
			private int intTarget;
			private int intCompareField;

			private DateTime dateTarget;
			private DateTime dateCompareField;
		  private DateTime? nullableDateCompareField;
		  private DateTime? nullableDateTarget;

			public int IntTarget
			{
				get { return intTarget; }
				set { intTarget = value; }
			}

			public int IntCompareField
			{
				get { return intCompareField; }
				set { intCompareField = value; }
			}

			public DateTime DateTarget
			{
				get { return dateTarget; }
				set { dateTarget = value; }
			}

			public DateTime DateCompareField
			{
				get { return dateCompareField; }
				set { dateCompareField = value; }
			}

      public DateTime? NullableDateTarget
      {
        get { return nullableDateTarget; }
        set { nullableDateTarget = value; }
      }

		  public DateTime? NullableDateCompareField
		  {
        get { return nullableDateCompareField; }
        set { nullableDateCompareField = value; }
		  }
		}
	}
}
