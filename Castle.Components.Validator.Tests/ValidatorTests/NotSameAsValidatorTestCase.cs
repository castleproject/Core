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
	public class NotSameAsValidatorTestCase
	{
		private NotSameAsValidator validator1, validator2, validator3;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			validator1 = new NotSameAsValidator("ComparableField1");
			validator1.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField1"));

			validator2 = new NotSameAsValidator("ComparableField2");
			validator2.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField2"));

			// Use public field instead of public property
			validator3 = new NotSameAsValidator("ComparableField3");
			validator3.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField1"));

			target = new TestTarget();
		}

		[Test]
		public void InvalidForString()
		{
			//Assert.IsFalse(validator1.IsValid(target, ""));
			Assert.IsFalse(validator1.IsValid(target, null));
			Assert.IsFalse(validator3.IsValid(target, null));
			Assert.IsFalse(validator1.IsValid(target, ""));
			Assert.IsFalse(validator3.IsValid(target, ""));

			target.ComparableField1 = "kenegozi";
			Assert.IsFalse(validator1.IsValid(target, "kenegozi"));

			target.ComparableField3 = "kenegozi";
			Assert.IsFalse(validator3.IsValid(target, "kenegozi"));
		}

		[Test]
		public void InvalidForInt()
		{
			target.ComparableField2 = 100;
			Assert.IsFalse(validator2.IsValid(target, 100));
		}

		[Test]
		public void ValidForString()
		{
			Assert.IsTrue(validator1.IsValid(target, "kenegozi"));

			target.ComparableField1 = "hammett";
			Assert.IsTrue(validator1.IsValid(target, null));
			Assert.IsTrue(validator1.IsValid(target, ""));

			target.ComparableField3 = "hammett";
			Assert.IsTrue(validator3.IsValid(target, null));
			Assert.IsTrue(validator3.IsValid(target, ""));
		}

		[Test]
		public void ValidForInt()
		{
			target.ComparableField2 = 100;
			Assert.IsTrue(validator2.IsValid(target, 200));
		}

		public class TestTarget
		{
			private string targetField1;
			private string comparableField1;
			private int targetField2;
			private int comparableField2;

			public string TargetField1
			{
				get { return targetField1; }
				set { targetField1 = value; }
			}

			public string ComparableField1
			{
				get { return comparableField1; }
				set { comparableField1 = value; }
			}

			public int TargetField2
			{
				get { return targetField2; }
				set { targetField2 = value; }
			}

			public int ComparableField2
			{
				get { return comparableField2; }
				set { comparableField2 = value; }
			}

			public string ComparableField3;
		}
	}
}
