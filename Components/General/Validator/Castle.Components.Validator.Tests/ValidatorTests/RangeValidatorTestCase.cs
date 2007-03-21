// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;

	[TestFixture]
	public class RangeValidatorTestCase
	{
		private RangeValidator validatorIntLow, validatorIntHigh, validatorIntLowOrHigh,
			validatorDateTimeLow, validatorDateTimeHigh, validatorDateTimeLowOrHigh;
		private TestTargetInt intTarget;
		private TestTargetDateTime dateTimeTarget;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			// Integer validation
			validatorIntLow = new RangeValidator(0, int.MaxValue);
			validatorIntLow.Initialize(typeof(TestTargetInt).GetProperty("TargetField"));

			validatorIntHigh = new RangeValidator(int.MinValue, 0);
			validatorIntHigh.Initialize(typeof(TestTargetInt).GetProperty("TargetField"));

			validatorIntLowOrHigh = new RangeValidator(-1, 1);
			validatorIntLowOrHigh.Initialize(typeof(TestTargetInt).GetProperty("TargetField"));

			intTarget = new TestTargetInt();

			// DateTime validation
			validatorDateTimeLow = new RangeValidator(DateTime.Now, DateTime.MaxValue);
			validatorDateTimeLow.Initialize(typeof(TestTargetDateTime).GetProperty("TargetField"));

			validatorDateTimeHigh = new RangeValidator(DateTime.MinValue, DateTime.Now);
			validatorDateTimeHigh.Initialize(typeof(TestTargetDateTime).GetProperty("TargetField"));

			validatorDateTimeLowOrHigh = new RangeValidator(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
			validatorDateTimeLowOrHigh.Initialize(typeof(TestTargetDateTime).GetProperty("TargetField"));

			dateTimeTarget = new TestTargetDateTime();
		}

		public class TestTargetInt
		{
			private int targetField;

			public int TargetField
			{
				get { return targetField; }
				set { targetField = value; }
			}
		}

		public class TestTargetDateTime
		{
			private DateTime targetField;

			public DateTime TargetField
			{
				get { return targetField; }
				set { targetField = value; }
			}
		}

		#region Integer range tests
		[Test]
		public void RangeIntTooLowValidator()
		{
			//fail when compare to non-number
			Assert.IsFalse(validatorIntLow.IsValid(intTarget, "abc"));
			//fail when compare to number too low
			Assert.IsFalse(validatorIntLow.IsValid(intTarget, -1));
			//pass when compare to number not too low
			Assert.IsTrue(validatorIntLow.IsValid(intTarget, 1));
		}

		[Test]
		public void RangeIntTooHighValidator()
		{
			//fail when compare to non-number
			Assert.IsFalse(validatorIntHigh.IsValid(intTarget, "abc"));
			//fail when compare to number too high
			Assert.IsFalse(validatorIntHigh.IsValid(intTarget, 1));
			//pass when compare to number not too high
			Assert.IsTrue(validatorIntHigh.IsValid(intTarget, -1));
		}

		[Test]
		public void RangeIntTooLowOrHighValidator()
		{
			//fail when compare to non-number
			Assert.IsFalse(validatorIntLowOrHigh.IsValid(intTarget, "abc"));
			//fail when compare to number too low
			Assert.IsFalse(validatorIntLowOrHigh.IsValid(intTarget, -2));
			//fail when compare to number too high
			Assert.IsFalse(validatorIntLowOrHigh.IsValid(intTarget, 2));
			//pass when compare to number not too high
			Assert.IsTrue(validatorIntLowOrHigh.IsValid(intTarget, 0));
		}
		#endregion

		#region DateTime range tests
		[Test]
		public void RangeDateTimeTooLowValidator()
		{
			//fail when compare to non-date
			Assert.IsFalse(validatorDateTimeLow.IsValid(dateTimeTarget, "abc"));
			//fail when compare to date too low
			Assert.IsFalse(validatorDateTimeLow.IsValid(dateTimeTarget, DateTime.Now.AddDays(-1)));
			//pass when compare to number not too low
			Assert.IsTrue(validatorDateTimeLow.IsValid(dateTimeTarget, DateTime.Now.AddDays(1)));
		}

		[Test]
		public void RangeDateTimeTooHighValidator()
		{
			//fail when compare to non-number
			Assert.IsFalse(validatorDateTimeHigh.IsValid(dateTimeTarget, "abc"));
			//fail when compare to number too high
			Assert.IsFalse(validatorDateTimeHigh.IsValid(dateTimeTarget, DateTime.Now.AddDays(1)));
			//pass when compare to number not too high
			Assert.IsTrue(validatorDateTimeHigh.IsValid(dateTimeTarget, DateTime.Now.AddDays(-1)));
		}

		[Test]
		public void RangeDateTimeTooLowOrHighValidator()
		{
			//fail when compare to non-number
			Assert.IsFalse(validatorDateTimeLowOrHigh.IsValid(dateTimeTarget, "abc"));
			//fail when compare to number too low
			Assert.IsFalse(validatorDateTimeLowOrHigh.IsValid(dateTimeTarget, DateTime.Now.AddDays(-2)));
			//fail when compare to number too high
			Assert.IsFalse(validatorDateTimeLowOrHigh.IsValid(dateTimeTarget, DateTime.Now.AddDays(2)));
			//pass when compare to number not too high
			Assert.IsTrue(validatorDateTimeLowOrHigh.IsValid(dateTimeTarget, DateTime.Now));
		}
		#endregion
	}
}
