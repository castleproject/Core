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
	public class IntegerValidatorTestCase
	{
		private IntegerValidator validator;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			validator = new IntegerValidator();
			target = new TestTarget();
		}

		[Test]
		public void InvalidInteger()
		{
			validator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField"));

			Assert.IsFalse(validator.IsValid(target, "abc"));
			Assert.IsFalse(validator.IsValid(target, "100.11"));
			Assert.IsFalse(validator.IsValid(target, "-99.8"));
		}

		[Test]
		public void ValidInteger()
		{
			validator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField"));

			Assert.IsTrue(validator.IsValid(target, "100"));
			Assert.IsTrue(validator.IsValid(target, "-99"));
			Assert.IsTrue(validator.IsValid(target, null));
			Assert.IsTrue(validator.IsValid(target, ""));
		}

		[Test]
		public void ValidInt16()
		{
			validator = new IntegerValidator();
			validator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField16"));

			Assert.IsTrue(validator.IsValid(target, "100"));
			Assert.IsTrue(validator.IsValid(target, "-99"));
			Assert.IsTrue(validator.IsValid(target, null));
			Assert.IsTrue(validator.IsValid(target, ""));
			Assert.IsTrue(validator.IsValid(target, short.MinValue));
			Assert.IsTrue(validator.IsValid(target, short.MaxValue));
		}

		[Test]
		public void ValidNullableInt16()
		{
			validator = new IntegerValidator();
			validator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField16Nullable"));

			Assert.IsTrue(validator.IsValid(target, "100"));
			Assert.IsTrue(validator.IsValid(target, "-99"));
			Assert.IsTrue(validator.IsValid(target, null));
			Assert.IsTrue(validator.IsValid(target, ""));
			Assert.IsTrue(validator.IsValid(target, short.MinValue));
			Assert.IsTrue(validator.IsValid(target, short.MaxValue));
		}

		[Test]
		public void ValidInt64()
		{
			validator = new IntegerValidator();
			validator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField64"));

			Assert.IsTrue(validator.IsValid(target, "100"));
			Assert.IsTrue(validator.IsValid(target, "-99"));
			Assert.IsTrue(validator.IsValid(target, null));
			Assert.IsTrue(validator.IsValid(target, ""));
			Assert.IsTrue(validator.IsValid(target, long.MinValue));
			Assert.IsTrue(validator.IsValid(target, long.MaxValue));
		}

		[Test]
		public void ValidNullableInt64()
		{
			validator = new IntegerValidator();
			validator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField64Nullable"));

			Assert.IsTrue(validator.IsValid(target, "100"));
			Assert.IsTrue(validator.IsValid(target, "-99"));
			Assert.IsTrue(validator.IsValid(target, null));
			Assert.IsTrue(validator.IsValid(target, ""));
			Assert.IsTrue(validator.IsValid(target, long.MinValue));
			Assert.IsTrue(validator.IsValid(target, long.MaxValue));
		}

		public class TestTarget
		{
			private int targetField;
			private long targetField64;
			private long? targetField64Nullable;
			private short targetField16;
			private short? targetField16Nullable;

			public int TargetField
			{
				get { return targetField; }
				set { targetField = value; }
			}

			public long TargetField64
			{
				get { return targetField64; }
				set { targetField64 = value; }
			}

			public long? TargetField64Nullable
			{
				get { return targetField64Nullable; }
				set { targetField64Nullable = value; }
			}

			public short TargetField16
			{
				get { return targetField16; }
				set { targetField16 = value; }
			}

			public short? TargetField16Nullable
			{
				get { return targetField16Nullable; }
				set { targetField16Nullable = value; }
			}
		}
	}
}
