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
	public class GroupNotEmptyValidatorTestCase
	{
		private GroupNotEmptyValidator validator;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			validator = new GroupNotEmptyValidator("Dummy");
			validator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("Foo"));
			validator.FriendlyName = "BAR";
			validator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("Bar"));
			target = new TestTarget();
		}

		[Test]
		public void BothEmptyStrings()
		{
			target.Bar = null;
			target.Foo = "";
			Assert.IsFalse(validator.IsValid(target));
		}

		[Test]
		public void BothNonEmptyStrings()
		{
			target.Bar = "Abva";
			target.Foo = "tdhf";
			Assert.IsTrue(validator.IsValid(target));
		}

		[Test]
		public void FirstEmptySecondFull()
		{
			target.Bar = "";
			target.Foo = " ";
			Assert.IsTrue(validator.IsValid(target));
		}


		[Test]
		public void FirstFullSecondEmpty()
		{
			target.Bar = "aa ";
			target.Foo = "";
			Assert.IsTrue(validator.IsValid(target));
		}

		[Test]
		public void ErrorMessage()
		{
			Assert.IsFalse(validator.IsValid(target));

			Assert.IsTrue(
				"At least one of the values should not be empty" == validator.ErrorMessage
				|| "At least one of the values should not be empty" == validator.ErrorMessage);
		}

		public class TestTarget
		{
			private string foo;
			private string bar;


			public string Bar
			{
				get { return bar; }
				set { bar = value; }
			}

			public string Foo
			{
				get { return foo; }
				set { foo = value; }
			}
		}
	}
}