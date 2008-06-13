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

namespace Castle.Components.Validator.Tests.ValidatorTests
{
	using System;
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;

	[TestFixture]
	public class SetValidatorTestCase
	{
		private SetValidator validatorStringArray, validatorStrings, validatorEnum, validatorFlagsEnum;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			// set from string array
			string[] valstrings = new string[] { "abc", "xyz" };
			validatorStringArray = new SetValidator(valstrings);
			validatorStringArray.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField"));

			// set from strings
			validatorStrings = new SetValidator("abc", "xyz");
			validatorStrings.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField"));

			// set from enum
			validatorEnum = new SetValidator(typeof(System.DayOfWeek));
			validatorEnum.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField"));

			// set from flags enum
			validatorFlagsEnum = new SetValidator(typeof(System.AttributeTargets));
			validatorFlagsEnum.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField"));

			target = new TestTarget();
		}

		public class TestTarget
		{
			private string targetField;

			public string TargetField
			{
				get { return targetField; }
				set { targetField = value; }
			}
		}

		[Test]
		public void StringArraySetValidator()
		{
			//fail when string not in set
			Assert.IsFalse(validatorStringArray.IsValid(target, "jkl"));
			//pass when string is in set
			Assert.IsTrue(validatorStringArray.IsValid(target, "abc"));
			//pass when string is null
			Assert.IsTrue(validatorStringArray.IsValid(target, String.Empty));
		}

		[Test]
		public void StringParamsSetValidator()
		{
			//fail when string not in set
			Assert.IsFalse(validatorStrings.IsValid(target, "jkl"));
			//pass when string is in set
			Assert.IsTrue(validatorStrings.IsValid(target, "abc"));
			//pass when string is null
			Assert.IsTrue(validatorStrings.IsValid(target, String.Empty));
		}

		[Test]
		public void EnumSetValidator()
		{
			//fail when string not in set
			Assert.IsFalse(validatorEnum.IsValid(target, "Doomsday"));
			//pass when string is in set
			Assert.IsTrue(validatorEnum.IsValid(target, DayOfWeek.Thursday));
			//pass when string is null
			Assert.IsTrue(validatorEnum.IsValid(target, String.Empty));
		}

		[Test]
		public void FlagsEnumSetValidator()
		{
			//fail when string not in set
			Assert.IsFalse(validatorFlagsEnum.IsValid(target, "Doomsday"));
			//pass when enum is in set
			Assert.IsTrue(validatorFlagsEnum.IsValid(target, AttributeTargets.Assembly));
			//pass when enum is in set
			Assert.IsTrue(validatorFlagsEnum.IsValid(target, AttributeTargets.Assembly | AttributeTargets.Class));
			//pass when string is null
			Assert.IsTrue(validatorFlagsEnum.IsValid(target, String.Empty));
		}
	}
}
