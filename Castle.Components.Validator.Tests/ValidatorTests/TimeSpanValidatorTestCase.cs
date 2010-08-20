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
	public class TimeSpanValidatorTestCase
	{
		private TimeSpanValidator validator;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			setup();
		}
        
		private void setup()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			validator = new TimeSpanValidator();
			
			validator.Initialize(new CachedValidationRegistry(), typeof (TestTarget).GetProperty("TargetField"));
			target = new TestTarget();
		}

        [Test]
		public void InvalidTimeSpans()
		{
			Assert.IsFalse(validator.IsValid(target, "abc"));
			Assert.IsFalse(validator.IsValid(target, "l"));
			Assert.IsFalse(validator.IsValid(target, "b23:56"));
			Assert.IsFalse(validator.IsValid(target, "32:56f"));
			Assert.IsFalse(validator.IsValid(target, "00:70"));
			Assert.IsFalse(validator.IsValid(target, "-00:70"));
#if DOTNET35
			Assert.IsFalse(validator.IsValid(target, "50:00:50"));
			Assert.IsFalse(validator.IsValid(target, "50:05:50"));
			Assert.IsFalse(validator.IsValid(target, "50:3:50"));
#endif
		}

		[Test]
		public void ValidTimeSpans()
		{
			Assert.IsTrue(validator.IsValid(target, null));
			Assert.IsTrue(validator.IsValid(target, ""));
			Assert.IsTrue(validator.IsValid(target, "0"));
			Assert.IsTrue(validator.IsValid(target, "00:50"));
			Assert.IsTrue(validator.IsValid(target, "5:00:50"));
			Assert.IsTrue(validator.IsValid(target, "05:00:50"));
			Assert.IsTrue(validator.IsValid(target, "1.05:00:50"));
			Assert.IsTrue(validator.IsValid(target, "1.05:00:50.894"));
		}

		public class TestTarget
		{
			public string TargetField { get; set; }
		}
	}
}
