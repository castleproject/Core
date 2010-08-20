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
	using System.Net;
	using System.Resources;
	using System.Threading;
	using NUnit.Framework;

	[TestFixture]
	public class IPAddressValidatorTestCase
	{
		private IPAddressValidator validator;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			validator = new IPAddressValidator();
			validator.Initialize(new CachedValidationRegistry(), typeof(TestTarget).GetProperty("TargetField"));
			target = new TestTarget();
		}

		[Test]
		public void Succeeds_On_Valid_Dotted_IPAddresses()
		{
			Assert.IsTrue(validator.IsValid(target, "10.0.0.0"));
			Assert.IsTrue(validator.IsValid(target, "192.168.1.1"));
			Assert.IsTrue(validator.IsValid(target, "255.255.255.255"));
			Assert.IsTrue(validator.IsValid(target, "192.168.0.0"));
			Assert.IsTrue(validator.IsValid(target, "67.199.26.208"));
		}

		[Test]
		public void Succeeds_On_Valid_Numeric_IPAddresses()
		{
			Assert.IsTrue(validator.IsValid(target, "167772160"));
			Assert.IsTrue(validator.IsValid(target, "3232235777"));
			Assert.IsTrue(validator.IsValid(target, "4294967294"));
			Assert.IsTrue(validator.IsValid(target, "3232235520"));
			Assert.IsTrue(validator.IsValid(target, "1137122000"));
		}

		[Test]
		public void Fails_On_Invalid_Dotted_IPAddresses()
		{
			Assert.IsFalse(validator.IsValid(target, "123.456.789"));
			Assert.IsFalse(validator.IsValid(target, "this.is.not.a.valid.ip.address"));
			Assert.IsFalse(validator.IsValid(target, "260.255.255.255"));
			Assert.IsFalse(validator.IsValid(target, "192.999.0.0"));
		}

		[Test]
		public void Fails_On_Invalid_Numeric_IPAddresses()
		{
			Assert.IsFalse(validator.IsValid(target, "0033996344"));
			Assert.IsFalse(validator.IsValid(target, "0033996351"));
			Assert.IsFalse(validator.IsValid(target, "0123456789"));
		}

		[Test]
		public void Fails_On_Invalid_Textual_IPAddresses()
		{
			Assert.IsFalse(validator.IsValid(target, "Invalid IP Address"));
			Assert.IsFalse(validator.IsValid(target, "this.is.not.a.valid.ip.address"));
		}

		[Test]
		public void NullOrEmptyStrings_Are_Valid()
		{
			Assert.IsTrue(validator.IsValid(target, null));
			Assert.IsTrue(validator.IsValid(target, ""));
		}

		[Test]
		public void Accepts_System_Net_IPAddress()
		{
			Assert.IsTrue(validator.IsValid(target, IPAddress.Parse("67.199.26.208")));
		}

		[Test, Ignore("Doesn't pass when run from command line - anyone?")]
		public void Localized_Error_Messages_Can_Be_Obtained()
		{
			ChangeCultureAndTest("en-us", (messageValidator) => Assert.AreEqual("Please enter a valid IP address.", messageValidator.ErrorMessage));
			ChangeCultureAndTest("nl-NL", (messageValidator) => Assert.AreEqual("Vul s.v.p. een geldig IP adres in.", messageValidator.ErrorMessage));
		}

		private static void ChangeCultureAndTest(string name, Action<AbstractValidator> test)
		{
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

			try
			{
				Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(name);

				var resourceManager = new ResourceManager("Castle.Components.Validator.Messages", typeof(CachedValidationRegistry).Assembly);

				var validator = new IPAddressValidator();
				validator.Initialize(new CachedValidationRegistry(resourceManager), typeof(TestTarget).GetProperty("TargetField"));

				test(validator);
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = currentCulture;
			}
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