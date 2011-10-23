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
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class RangeValidatorTestCase
	{
		private RangeValidator validatorIntLow,
		                       validatorIntHigh,
		                       validatorIntLowOrHigh,
							   validatorLongLow,
							   validatorLongHigh,
							   validatorLongLowOrHigh,
							   validatorDecimalLow,
							   validatorDecimalHigh,
							   validatorDecimalLowOrHigh,
		                       validatorDateTimeLow,
		                       validatorDateTimeHigh,
		                       validatorDateTimeLowOrHigh,
		                       validatorStringLow,
		                       validatorStringHigh,
		                       validatorStringLowOrHigh,
							   validatorIntMessage;

		private TestTargetInt intTarget;
		private TestTargetLong longTarget;
		private TestTargetDecimal decimalTarget;
		private TestTargetDateTime dateTimeTarget;
		private TestTargetString stringTarget;
		private const string CustomErrorMessage = "Custom Error Message";

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			// Integer validation
			validatorIntLow = new RangeValidator(0, int.MaxValue);
			validatorIntLow.Initialize(new CachedValidationRegistry(), typeof(TestTargetInt).GetProperty("TargetField"));

			validatorIntHigh = new RangeValidator(int.MinValue, 0);
			validatorIntHigh.Initialize(new CachedValidationRegistry(), typeof(TestTargetInt).GetProperty("TargetField"));

			validatorIntLowOrHigh = new RangeValidator(RangeValidationType.Integer, "-1", "1");
			validatorIntLowOrHigh.Initialize(new CachedValidationRegistry(), typeof(TestTargetInt).GetProperty("TargetField"));

			intTarget = new TestTargetInt();

			// Long validation
			validatorLongLow = new RangeValidator(0, long.MaxValue);
			validatorLongLow.Initialize(new CachedValidationRegistry(), typeof(TestTargetLong).GetProperty("TargetField"));

			validatorLongHigh = new RangeValidator(long.MinValue, 0);
			validatorLongHigh.Initialize(new CachedValidationRegistry(), typeof(TestTargetLong).GetProperty("TargetField"));

			validatorLongLowOrHigh = new RangeValidator(RangeValidationType.Long, "-1", "1");
			validatorLongLowOrHigh.Initialize(new CachedValidationRegistry(), typeof(TestTargetLong).GetProperty("TargetField"));

			longTarget = new TestTargetLong();

			// decimal validation
			validatorDecimalLow = new RangeValidator(0, decimal.MaxValue);
			validatorDecimalLow.Initialize(new CachedValidationRegistry(), typeof(TestTargetInt).GetProperty("TargetField"));

			validatorDecimalHigh = new RangeValidator(decimal.MinValue, 0);
			validatorDecimalHigh.Initialize(new CachedValidationRegistry(), typeof(TestTargetInt).GetProperty("TargetField"));

			validatorDecimalLowOrHigh = new RangeValidator(RangeValidationType.Decimal, "-1", "1");
			validatorDecimalLowOrHigh.Initialize(new CachedValidationRegistry(), typeof(TestTargetInt).GetProperty("TargetField"));

			decimalTarget = new TestTargetDecimal();

			// DateTime validation
			validatorDateTimeLow = new RangeValidator(DateTime.Now, DateTime.MaxValue);
			validatorDateTimeLow.Initialize(new CachedValidationRegistry(), typeof(TestTargetDateTime).GetProperty("TargetField"));

			validatorDateTimeHigh = new RangeValidator(DateTime.MinValue, DateTime.Now);
			validatorDateTimeHigh.Initialize(new CachedValidationRegistry(), typeof(TestTargetDateTime).GetProperty("TargetField"));

			validatorDateTimeLowOrHigh = new RangeValidator(RangeValidationType.DateTime, "2000-01-01", "2099-12-31");
			validatorDateTimeLowOrHigh.Initialize(new CachedValidationRegistry(), typeof(TestTargetDateTime).GetProperty("TargetField"));

			dateTimeTarget = new TestTargetDateTime();

			// String validation
			validatorStringLow = new RangeValidator("bbb", String.Empty);
			validatorStringLow.Initialize(new CachedValidationRegistry(), typeof(TestTargetString).GetProperty("TargetField"));

			validatorStringHigh = new RangeValidator(String.Empty, "yyy");
			validatorStringHigh.Initialize(new CachedValidationRegistry(), typeof(TestTargetString).GetProperty("TargetField"));

			validatorStringLowOrHigh = new RangeValidator(RangeValidationType.String, 'b'.ToString(), 'y'.ToString());
			validatorStringLowOrHigh.Initialize(new CachedValidationRegistry(), typeof(TestTargetString).GetProperty("TargetField"));

			stringTarget = new TestTargetString();

			// Message validation
			validatorIntMessage = new RangeValidator(0, 1) { ErrorMessage = CustomErrorMessage };
			validatorIntMessage.Initialize(new CachedValidationRegistry(), typeof(TestTargetInt).GetProperty("TargetField"));
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

		public class TestTargetLong
		{
			private long targetField;

			public long TargetField
			{
				get { return targetField; }
				set { targetField = value; }
			}
		}

		public class TestTargetDecimal
		{
			private decimal targetField;

			public decimal TargetField
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

		public class TestTargetString
		{
			private string targetField;

			public string TargetField
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

		#region Long range tests

		[Test]
		public void RangeLongTooLowValidator()
		{
			//fail when compare to non-number
			Assert.IsFalse(validatorLongLow.IsValid(longTarget, "abc"));
			//fail when compare to number too low
			Assert.IsFalse(validatorLongLow.IsValid(longTarget, -1));
			//pass when compare to number not too low
			Assert.IsTrue(validatorLongLow.IsValid(longTarget, 1));
		}

		[Test]
		public void RangeLongTooHighValidator()
		{
			//fail when compare to non-number
			Assert.IsFalse(validatorLongHigh.IsValid(longTarget, "abc"));
			//fail when compare to number too high
			Assert.IsFalse(validatorLongHigh.IsValid(longTarget, 1));
			//pass when compare to number not too high
			Assert.IsTrue(validatorLongHigh.IsValid(longTarget, -1));
		}

		[Test]
		public void RangeLongTooLowOrHighValidator()
		{
			//fail when compare to non-number
			Assert.IsFalse(validatorLongLowOrHigh.IsValid(longTarget, "abc"));
			//fail when compare to number too low
			Assert.IsFalse(validatorLongLowOrHigh.IsValid(longTarget, -2));
			//fail when compare to number too high
			Assert.IsFalse(validatorLongLowOrHigh.IsValid(longTarget, 2));
			//pass when compare to number not too high
			Assert.IsTrue(validatorLongLowOrHigh.IsValid(longTarget, 0));
		}

		#endregion

		#region Decimal range tests

		[Test]
		public void RangeDecimalTooLowValidator()
		{
			//fail when compare to non-number
			Assert.IsFalse(validatorDecimalLow.IsValid(decimalTarget, "abc"));
			//fail when compare to number too low
			Assert.IsFalse(validatorDecimalLow.IsValid(decimalTarget, -1));
			//pass when compare to number not too low
			Assert.IsTrue(validatorDecimalLow.IsValid(decimalTarget, 1));
		}

		[Test]
		public void RangeDecimalTooHighValidator()
		{
			//fail when compare to non-number
			Assert.IsFalse(validatorDecimalHigh.IsValid(decimalTarget, "abc"));
			//fail when compare to number too high
			Assert.IsFalse(validatorDecimalHigh.IsValid(decimalTarget, 1));
			//pass when compare to number not too high
			Assert.IsTrue(validatorDecimalHigh.IsValid(decimalTarget, -1));
		}

		[Test]
		public void RangeDecimalTooLowOrHighValidator()
		{
			//fail when compare to non-number
			Assert.IsFalse(validatorDecimalLowOrHigh.IsValid(decimalTarget, "abc"));
			//fail when compare to number too low
			Assert.IsFalse(validatorDecimalLowOrHigh.IsValid(decimalTarget, -2));
			//fail when compare to number too high
			Assert.IsFalse(validatorDecimalLowOrHigh.IsValid(decimalTarget, 2));
			//pass when compare to number not too high
			Assert.IsTrue(validatorDecimalLowOrHigh.IsValid(decimalTarget, 0));
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
			//pass when compare to date not too low
			Assert.IsTrue(validatorDateTimeLow.IsValid(dateTimeTarget, DateTime.Now.AddDays(1)));
		}

		[Test]
		public void RangeDateTimeTooHighValidator()
		{
			//fail when compare to non-date
			Assert.IsFalse(validatorDateTimeHigh.IsValid(dateTimeTarget, "abc"));
			//fail when compare to number too high
			Assert.IsFalse(validatorDateTimeHigh.IsValid(dateTimeTarget, DateTime.Now.AddDays(1)));
			//pass when compare to date not too high
			Assert.IsTrue(validatorDateTimeHigh.IsValid(dateTimeTarget, DateTime.Now.AddDays(-1)));
		}

		[Test]
		public void RangeDateTimeTooLowOrHighValidator()
		{
			//fail when compare to non-date
			Assert.IsFalse(validatorDateTimeLowOrHigh.IsValid(dateTimeTarget, "abc"));
			//fail when compare to date too low
			Assert.IsFalse(validatorDateTimeLowOrHigh.IsValid(dateTimeTarget, new DateTime(1999, 01, 01)));
			//fail when compare to date too high
			Assert.IsFalse(validatorDateTimeLowOrHigh.IsValid(dateTimeTarget, new DateTime(2100, 01, 01)));
			//pass when compare to date not too low or high
			Assert.IsTrue(validatorDateTimeLowOrHigh.IsValid(dateTimeTarget, new DateTime(2007, 01, 01)));
		}

		#endregion

		#region String range tests

		[Test]
		public void RangeStringTooLowValidator()
		{
			//fail when compare to string too low
			Assert.IsFalse(validatorStringLow.IsValid(stringTarget, "aaa"));
			//pass when compare to string not too low
			Assert.IsTrue(validatorStringLow.IsValid(stringTarget, "ccc"));
		}

		[Test]
		public void RangeStringTooHighValidator()
		{
			//fail when compare to string too high
			Assert.IsFalse(validatorStringHigh.IsValid(stringTarget, "zzz"));
			//pass when compare to string not too high
			Assert.IsTrue(validatorStringHigh.IsValid(stringTarget, "xxx"));
		}

		[Test]
		public void RangeStringTooLowOrHighValidator()
		{
			//fail when compare to string too low
			Assert.IsFalse(validatorStringLowOrHigh.IsValid(stringTarget, "a"));
			//fail when compare to string too high
			Assert.IsFalse(validatorStringLowOrHigh.IsValid(stringTarget, "z"));
			//pass when compare to string not too low or high
			Assert.IsTrue(validatorStringLowOrHigh.IsValid(stringTarget, "m"));
		}

		#endregion

		#region Error message tests
		[Test]
		public void CustomMessageIsReturned()
		{
			Assert.IsFalse(validatorIntMessage.IsValid(intTarget, -2));
			Assert.AreEqual(CustomErrorMessage, validatorIntMessage.ErrorMessage);
		}

		[Test]
		public void CustomMessageIsSentToIBrowserValidationGenerator()
		{
			var mockery = new MockRepository();
			IBrowserValidationGenerator browserValidationGenerator = mockery.DynamicMock<IBrowserValidationGenerator>();

			using (mockery.Record())
			{
				browserValidationGenerator.SetValueRange("TargetField", 0, 1, CustomErrorMessage);
			}
			using (mockery.Playback())
			{
				Assert.IsFalse(validatorIntMessage.IsValid(intTarget, -2));
				validatorIntMessage.ApplyBrowserValidation(null, InputElementType.Text, browserValidationGenerator, new Hashtable(), "TargetField");
				Assert.AreEqual(CustomErrorMessage, validatorIntMessage.ErrorMessage);
			}
		}
		#endregion
	}
}