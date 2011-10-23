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
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;

	[TestFixture]
	public class NotSameValueValidatorTestCase
	{
		private NotSameValueValidator validator1, validator2, validator3;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			validator1 = new NotSameValueValidator("15");
			validator1.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField1"));

			validator2 = new NotSameValueValidator(15);
			validator2.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField2"));

			ValidateNotSameValueAttribute attribute = new ValidateNotSameValueAttribute(typeof(Guid), Guid.Empty.ToString());
			validator3 = (NotSameValueValidator) attribute.Build();
			validator3.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField3"));

			target = new TestTarget();
		}

		[Test]
		public void InvalidForString()
		{
			Assert.IsFalse(validator1.IsValid(target, "15"));
			Assert.IsTrue(validator1.IsValid(target, "abc"));
		}

		[Test]
		public void InvalidForInt()
		{
			Assert.IsFalse(validator2.IsValid(target, 15));
		}

		[Test]
		public void ValidForString()
		{
			Assert.IsTrue(validator1.IsValid(target, "not 15"));
		}

		[Test]
		public void ValidForInt()
		{
			Assert.IsTrue(validator2.IsValid(target, 100));
		}

		[Test]
		public void InvalidGuid()
		{
			Assert.IsFalse(validator3.IsValid(target, Guid.Empty));
		}

		public class TestTarget
		{
			private string targetField1;
			private int targetField2;
			private Guid targetField3;

			public string TargetField1
			{
				get { return targetField1; }
				set { targetField1 = value; }
			}

			public int TargetField2
			{
				get { return targetField2; }
				set { targetField2 = value; }
			}


			public Guid TargetField3
			{
				get { return targetField3; }
				set { targetField3 = value; }
			}
		}
	}
}