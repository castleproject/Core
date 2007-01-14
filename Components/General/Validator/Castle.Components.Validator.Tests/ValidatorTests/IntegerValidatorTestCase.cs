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
			validator.Initialize(typeof(TestTarget).GetProperty("TargetField"));
			target = new TestTarget();
		}

		[Test]
		public void InvalidInteger()
		{
			Assert.IsFalse(validator.IsValid(target, "abc"));
			Assert.IsFalse(validator.IsValid(target, "100.11"));
		}

		[Test]
		public void ValidInteger()
		{
			Assert.IsTrue(validator.IsValid(target, "100"));
		}

		public class TestTarget
		{
			private int targetField;

			public int TargetField
			{
				get { return targetField; }
				set { targetField = value; }
			}
		}
	}
}
