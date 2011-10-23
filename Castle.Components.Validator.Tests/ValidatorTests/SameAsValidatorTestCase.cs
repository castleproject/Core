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
using System;

namespace Castle.Components.Validator.Tests.ValidatorTests
{
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;

	[TestFixture]
	public class SameAsValidatorTestCase
	{
		private SameAsValidator validator1, validator2, validator3, validator4;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			validator1 = new SameAsValidator("ComparableField1");
			validator1.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField1"));

			validator2 = new SameAsValidator("ComparableField2");
			validator2.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField2"));

			// Use public field instead of public property
			validator3 = new SameAsValidator("ComparableField3");
			validator3.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField1"));

			validator4 = new SameAsValidator("NestedField.ComparableField1");
			validator4.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField1"));

			target = new TestTarget();
			target.NestedField = new NestedTargetClass();
		}

		[Test]
		public void InvalidForString()
		{
			//Assert.IsFalse(validator1.IsValid(target, ""));
			Assert.IsFalse(validator1.IsValid(target, "abc"));
			Assert.IsFalse(validator3.IsValid(target, "abc"));

			target.ComparableField1 = "abc";
			Assert.IsFalse(validator1.IsValid(target, null));
		}

		[Test]
		public void InvalidForInt()
		{
			Assert.IsFalse(validator2.IsValid(target, 1));
			Assert.IsFalse(validator2.IsValid(target, 2));
		}

		[Test]
		public void ValidForString()
		{
			Assert.IsTrue(validator1.IsValid(target, null));

			target.ComparableField1 = "hammett";
			Assert.IsTrue(validator1.IsValid(target, "hammett"));

			target.ComparableField3 = "hammett";
			Assert.IsTrue(validator3.IsValid(target, "hammett"));
		}

		[Test]
		public void ValidForInt()
		{
			target.ComparableField2 = 100;
			Assert.IsTrue(validator2.IsValid(target, 100));
		}

		[Test]
		public void ValidForNestedTarget()
		{
			target.NestedField.ComparableField1 = "craig";
			Assert.IsTrue(validator4.IsValid(target, "craig"));	
		}

		public class TestTarget
		{
			private string targetField1;
			private string comparableField1;
			private int targetField2;
			private int comparableField2;
			private NestedTargetClass nestedField;

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

			public NestedTargetClass NestedField
			{
				get { return nestedField; }
				set { nestedField = value; }
			}
		}
	}

	public class NestedTargetClass
	{
		private string comparableField1;

		public NestedTargetClass()
		{
		}

		public string ComparableField1
		{
			get { return comparableField1; }
			set { comparableField1 = value; }
		}
	}
}
