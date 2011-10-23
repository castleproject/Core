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
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;

	[TestFixture]
	public class NonEmptyValidatorTestCase
	{
		private NonEmptyValidator validator;
		private NonEmptyValidator nullableTypeValidator;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			validator = new NonEmptyValidator();
			validator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField"));
			nullableTypeValidator = new NonEmptyValidator();
			nullableTypeValidator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("NullableTargetField"));
			target = new TestTarget();
		}

		[Test]
		public void EmptyStrings()
		{
			Assert.IsFalse(validator.IsValid(target, null));
			Assert.IsFalse(validator.IsValid(target, ""));
		}

		[Test]
		public void NonEmptyStrings()
		{
			Assert.IsTrue(validator.IsValid(target, "abc"));
			Assert.IsTrue(validator.IsValid(target, "  "));	
		}

		[Test]
		public void NullableTypes()
		{
			int? nullValue = null;
			int? notNullValue = 0;

			Assert.IsTrue(nullableTypeValidator.IsValid(target, notNullValue));
			Assert.IsFalse(nullableTypeValidator.IsValid(target, nullValue));
		}

		public class TestTarget
		{
			private string targetField;
			private int? nullableTargetField;

			public string TargetField
			{
				get { return targetField; }
				set { targetField = value; }
			}

			public int? NullableTargetField
			{
				get { return nullableTargetField; }
				set { nullableTargetField = value; }
			}
		}
	}
}
