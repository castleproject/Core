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
	public class EmailValidatorTestCase
	{
		private EmailValidator validator;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			validator = new EmailValidator();
			validator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField"));
			target = new TestTarget();
		}

		[Test]
		public void InvalidEmail()
		{
			Assert.IsFalse(validator.IsValid(target, "abc"));
			Assert.IsFalse(validator.IsValid(target, "ham@ham@ham"));
		}

		[Test]
		public void ValidEmail()
		{
			Assert.IsTrue(validator.IsValid(target, "hammett@gmail.com"));
			Assert.IsTrue(validator.IsValid(target, "hammett@uol.com.br"));
			Assert.IsTrue(validator.IsValid(target, "hammett@apache.org"));
			Assert.IsTrue(validator.IsValid(target, "hamilton.verissimo@something.com.br"));
			Assert.IsTrue(validator.IsValid(target, null));
			Assert.IsTrue(validator.IsValid(target, ""));
		}

		[Test]
		public void COMP54_ApostrophesShouldBeAllowed()
		{
			Assert.IsTrue(validator.IsValid(target, "hammett'@gmail.com"));
		}

		[Test]
		public void COMP99_PlusShouldBeAllowed()
		{
			Assert.IsTrue(validator.IsValid(target, "test+flowers@gmail.com"));
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
