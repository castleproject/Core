// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	public class IsGreaterValidatorTestCase
	{
		private IsGreaterValidator greaterIntValidator;
		private IsGreaterValidator greaterDateValidator;

		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			greaterIntValidator = new IsGreaterValidator(IsGreaterValidationType.Integer, "IntCompareField");
			greaterIntValidator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("IntTarget"));

			greaterDateValidator = new IsGreaterValidator(IsGreaterValidationType.Date, "DateCompareField");
			greaterDateValidator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("DateTarget"));

			target = new TestTarget();
		}

		[Test]
		public void IntegerIsGreaterValid()
		{
			target.IntCompareField = 100;
			Assert.IsTrue(greaterIntValidator.IsValid(target, 101));
		}

		[Test]
		public void DateIsGreaterValid()
		{
			target.DateCompareField = DateTime.Today;

			Assert.IsTrue(greaterDateValidator.IsValid(target, DateTime.Today.AddDays(5)));
			Assert.IsTrue(greaterDateValidator.IsValid(target, DateTime.Today.AddHours(1)));
		}

		[Test]
		public void DateIsGreaterNotValid()
		{
			target.DateCompareField = DateTime.Today;

			Assert.IsFalse(greaterDateValidator.IsValid(target, DateTime.Today));
			Assert.IsFalse(greaterDateValidator.IsValid(target, DateTime.Today.AddHours(-1)));
			Assert.IsFalse(greaterDateValidator.IsValid(target, DateTime.Today.AddDays(-5)));
		}

		public class TestTarget
		{
			private int intTarget;
			private int intCompareField;

			private DateTime dateTarget;
			private DateTime dateCompareField;

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
		}
	}
}
