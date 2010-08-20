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
	public class LengthValidatorTestCase
	{
		private LengthValidator validator1, validator2;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			validator1 = new LengthValidator(5);
			validator1.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField"));

			validator2 = new LengthValidator(4, 6);
			validator2.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField"));

			target = new TestTarget();
		}

		[Test]
		public void ExactLengthValidation()
		{
			Assert.IsFalse(validator1.IsValid(target, "abc"));
			Assert.IsFalse(validator1.IsValid(target, "abcabc"));
			Assert.IsTrue(validator1.IsValid(target, "12345"));
			Assert.IsTrue(validator1.IsValid(target, string.Empty));
		}

		[Test]
		public void RangeValidation()
		{
			Assert.IsFalse(validator2.IsValid(target, "abc"));
			Assert.IsFalse(validator2.IsValid(target, "1234567"));
			Assert.IsTrue(validator2.IsValid(target, "1234"));
			Assert.IsTrue(validator2.IsValid(target, "123456"));
			Assert.IsTrue(validator2.IsValid(target, string.Empty));
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
	}
}
