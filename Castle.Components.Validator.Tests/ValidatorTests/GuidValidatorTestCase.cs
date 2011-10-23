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
	using Validators;
	using NUnit.Framework;

	[TestFixture]
	public class GuidValidatorTestCase
	{
		private GuidValidator validator;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			setup(true);
		}
        
		private void setup(bool acceptEmptyGuid)
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			validator = new GuidValidator(acceptEmptyGuid);
			
			validator.Initialize(new CachedValidationRegistry(), typeof (TestTarget).GetProperty("TargetField"));
			target = new TestTarget();
		}

        [Test]
		public void InvalidGuids()
		{
			Assert.IsFalse(validator.IsValid(target, "abc"));
			Assert.IsFalse(validator.IsValid(target, "1"));
			Assert.IsFalse(validator.IsValid(target, "71dfee03-102c-433a-9cbf-bb38e8d9b45c-"));
			Assert.IsFalse(validator.IsValid(target, "71dfee03-102c-433a-9cbf-bb38e8d9b45-"));
			Assert.IsFalse(validator.IsValid(target, "71dfee03-102c-433a-9cbf-bb38e8d9b45"));
			Assert.IsFalse(validator.IsValid(target, "1dfee03-102c-433a-9cbf-bb38e8d9b45c"));

			Assert.IsFalse(validator.IsValid(target, "71dfee03102c-433a-9cbf-bb38e8d9b45c"));
			Assert.IsFalse(validator.IsValid(target, "71dfee03-102c433a-9cbf-bb38e8d9b45c"));
			Assert.IsFalse(validator.IsValid(target, "71dfee03-102c-433a9cbf-bb38e8d9b45c"));
			Assert.IsFalse(validator.IsValid(target, "71dfee03-102c-433a-9cbfbb38e8d9b45c"));
		}

		[Test]
		public void ValidGuids()
		{
			Assert.IsTrue(validator.IsValid(target, null));
			Assert.IsTrue(validator.IsValid(target, ""));
			Assert.IsTrue(validator.IsValid(target, "5e9a2e18-438f-473b-bbb1-895814e2d497"));
			Assert.IsTrue(validator.IsValid(target, "00000000-0000-0000-0000-000000000000"));
		}

		[Test]
		public void EmptyGuidIsRejectedWhenWeSaySo()
		{
			setup(false);
			Assert.IsFalse(validator.IsValid(target, "00000000-0000-0000-0000-000000000000"));
		}

		public class TestTarget
		{
			public string TargetField { get; set; }
		}
	}
}
