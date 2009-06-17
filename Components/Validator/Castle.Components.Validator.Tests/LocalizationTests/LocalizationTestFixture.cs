//  Copyright 2004-2009 Castle Project - http://www.castleproject.org/
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

namespace Castle.Components.Validator.Tests.LocalizationTests
{
	using System;
	using System.Globalization;
	using System.Reflection;
	using System.Threading;
	using NUnit.Framework;

	[TestFixture]
	public class ErrorMessageAndFriendlyNameResourceLocalizationTestCase
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			runner = new ValidatorRunner(new CachedValidationRegistry());
		}

		#endregion

		private ValidatorRunner runner;
		private const string This_is_a_required_field = "This is a required field";

		private static IDisposable SwitchUICulture(CultureInfo culture)
		{
			return new UICultureSwitcher(culture);
		}

		[Test]
		public void LocalizedValidation_ThrowsInvalidOperationException_When_NoLocalizationExists()
		{
			var foo = new IncorrectlyLocalized();

			try
			{
				runner.IsValid(foo);

				Assert.Fail("Should have gotten exception");
			}
			catch( InvalidOperationException ex)
			{
				Assert.AreEqual("The resource type 'Castle.Components.Validator.Tests.Messages' does not have a publicly visible static property named 'key_that_does_not_exists'. You probably marked the resources as internal, to fix this change the 'Access modifier' dropdown to 'Public' in the VS resources editor.", ex.Message);
			}
		}

		[Test]
		public void LocalizedValidation_ThrowsArgumentException_When_ResourceTypeNull()
		{
			var foo = new ResourceTypeNull();

			try
			{
				runner.IsValid(foo);

				Assert.Fail("Should have gotten exception");
			}
			catch (CustomAttributeFormatException ex)
			{
				ArgumentException argumentException = (ArgumentException)ex.InnerException.InnerException;
				Assert.AreEqual("Value cannot be null.\r\nParameter name: value", argumentException.Message);
			}
		}

		[Test]
		public void LocalizedValidation_ThrowsArgumentException_When_ResourceTypeNotSet()
		{
			var foo = new ResourceTypeNotSet();

			try
			{
				runner.IsValid(foo);

				Assert.Fail("Should have gotten exception");
			}
			catch (ArgumentException ex)
			{
				Assert.AreEqual("You have set ErrorMessageKey and/or FriendlyNameKey but have not specified the ResourceType to use for lookup.", ex.Message);
			}
		}

		[Test]
		public void LocalizedValidation_ThrowsArgumentException_When_ErrorMessageKeyNullOrEmpty()
		{
			var foos = new object [] { new ErrorMessageKeyNull(), new ErrorMessageKeyEmpty() };

			foreach (var foo in foos)
			{
				try
				{
					runner.IsValid(foo);

					Assert.Fail("Should have gotten exception");
				}
				catch (CustomAttributeFormatException ex)
				{
					ArgumentException argumentException = (ArgumentException)ex.InnerException.InnerException;
					Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: value", argumentException.Message);
				}
			}
		}

		[Test]
		public void LocalizedValidation_ThrowsArgumentException_When_FriendlyNameKeyNullOrEmpty()
		{
			var foos = new object [] { new FriendlyNameKeyNull(), new FriendlyNameKeyEmpty() };

			foreach (var foo in foos)
			{

				try
				{
					runner.IsValid(foo);

					Assert.Fail("Should have gotten exception");
				}
				catch (CustomAttributeFormatException ex)
				{
					ArgumentException argumentException = (ArgumentException) ex.InnerException.InnerException;
					Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: value", argumentException.Message);
				}
			}
		}

		[Test, Ignore("Doesn't pass when run from command line - anyone?")]
		public void LocalizedValidation_Uses_CurrentUICultureLocalization() {
			var bar = new OnlyLocalizedForLithuaniaBar();
			using (SwitchUICulture(CultureInfo.CreateSpecificCulture("lt-LT")))
			{
				runner.IsValid(bar);
				ErrorSummary summary = runner.GetErrorSummary(bar);

				Assert.IsNotNull(summary);
				Assert.AreEqual("lt-Bar_BazNonEmpty", summary.ErrorMessages[0]);
				Assert.AreEqual("lt-Bar_Baz", summary.InvalidProperties[0]);
			}
		}

		[Test]
		public void LocalizedValidation_IsOk_WhenMessagesExists()
		{
			var description = new CorrectlyLocalized();

			runner.IsValid(description);
			ErrorSummary summary = runner.GetErrorSummary(description);

			Assert.IsNotNull(summary);
			Assert.AreEqual(Messages.CorrectlyLocalized_DescriptionValidateNonEmpty, summary.ErrorMessages[0]);
			Assert.AreEqual(Messages.CorrectlyLocalized_Description, summary.InvalidProperties[0]);
		}

		[Test]
		public void LocalizedValidation_Uses_DefaultCulture_When_MessageDoesNotExists_In_LocalCulture()
		{
			var description = new CorrectlyLocalized();

			using (SwitchUICulture(CultureInfo.CreateSpecificCulture("mk-MK")))
			{
				runner.IsValid(description);
				ErrorSummary summary = runner.GetErrorSummary(description);

				Assert.IsNotNull(summary);
				Assert.AreEqual(Messages.CorrectlyLocalized_DescriptionValidateNonEmpty, summary.ErrorMessages[0]);
				Assert.AreEqual(Messages.CorrectlyLocalized_Description, summary.InvalidProperties[0]);
			}
		}

		[Test]
		public void LocalizedValidation_Uses_DefaultCulture_When_NoLocalIsAvailable()
		{
			var description = new CorrectlyLocalized();

			using (SwitchUICulture(CultureInfo.CreateSpecificCulture("ru-RU")))
			{

				runner.IsValid(description);
				ErrorSummary summary = runner.GetErrorSummary(description);

				Assert.IsNotNull(summary);
				Assert.AreEqual(Messages.CorrectlyLocalized_DescriptionValidateNonEmpty, summary.ErrorMessages[0]);
				Assert.AreEqual(Messages.CorrectlyLocalized_Description, summary.InvalidProperties[0]);
			}

		}

	}

	internal class UICultureSwitcher : IDisposable
	{
		private readonly CultureInfo previousUICulture;

		public UICultureSwitcher(CultureInfo newCulture)
		{
			previousUICulture = Thread.CurrentThread.CurrentUICulture;

			ThreadCurrentUICulture = newCulture;
		}

		private static CultureInfo ThreadCurrentUICulture
		{
			set { Thread.CurrentThread.CurrentUICulture = value; }
		}

		#region IDisposable Members

		public void Dispose()
		{
			ThreadCurrentUICulture = previousUICulture;
		}

		#endregion
	}

	public class CorrectlyLocalized
	{
		[ValidateNonEmpty(
			FriendlyNameKey = "CorrectlyLocalized_Description",
			ErrorMessageKey = "CorrectlyLocalized_DescriptionValidateNonEmpty",
			ResourceType = typeof(Messages)
			)]
		public string Description { get; set; }
	}

	public class IncorrectlyLocalized
	{
		[ValidateNonEmpty(
			FriendlyNameKey = "key_that_does_not_exists",
			ErrorMessageKey = "key_that_does_not_exists",
			ResourceType = typeof(Messages))]
		public string Bar { get; set; }
	}

	public class OnlyLocalizedForLithuaniaBar
	{
		[ValidateNonEmpty(
			FriendlyNameKey = "Bar_Baz",
			ErrorMessageKey = "Bar_BazNonEmpty",
			ResourceType = typeof(Messages))]
		public string Baz { get; set; }
	}

	public class ResourceTypeNull
	{
		[ValidateNonEmpty(
			ErrorMessageKey = "errormessage",
			FriendlyNameKey = "friendlyname",
			ResourceType = null)]
		public string Bar { get; set; }
	}

	public class ResourceTypeNotSet
	{
		[ValidateNonEmpty(
			ErrorMessageKey = "errormessage",
			FriendlyNameKey = "friendlyname")]
		public string Bar { get; set; }
	}
    
	public class ErrorMessageKeyNull
	{
		[ValidateNonEmpty(
			ErrorMessageKey = null,
			FriendlyNameKey = "friendlyname",
			ResourceType = typeof(Messages))]
		public string Bar { get; set; }
	}

	public class ErrorMessageKeyEmpty
	{
		[ValidateNonEmpty(
			ErrorMessageKey = "",
			FriendlyNameKey = "friendlyname",
			ResourceType = typeof(Messages))]
		public string Bar { get; set; }
	}


	public class FriendlyNameKeyNull
	{
		[ValidateNonEmpty(
			FriendlyNameKey = null,
			ErrorMessageKey = "errormessage",
			ResourceType = typeof(Messages))]
		public string Bar { get; set; }
	}

	public class FriendlyNameKeyEmpty
	{
		[ValidateNonEmpty(
			FriendlyNameKey = "",
			ErrorMessageKey = "errormessage",
			ResourceType = typeof(Messages))]
		public string Bar { get; set; }
	}
}
