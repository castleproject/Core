// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Tests
{
	using System;
	using Castle.ActiveRecord.Tests.Model.NestedValidation;
	using Castle.Components.Validator;
	using NUnit.Framework;

	[TestFixture]
	public class NestedModelValidation : AbstractActiveRecordTest
	{
		[Test]
		public void DoesNestedPropertyValidateWithARVBGeneric()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(UserWithNestedAddress));
			Recreate();

			UserWithNestedAddress user = new UserWithNestedAddress();
			Assert.IsFalse(user.IsValid());
			user.Email = "someemail";

			Assert.IsFalse(user.IsValid(),"Nested class not validated");
			Assert.IsTrue(user.ValidationErrorMessages.Length == 3,"Both nested props are required and should have error messages");

			Assert.IsTrue(user.PropertiesValidationErrorMessages.Count == 2,"Two properties should be invalid");

			user.PostalAddress.AddressLine1 = "15st"; //to short
			user.PostalAddress.Country = "Brazil";
			user.Email = "12345";

			Assert.IsFalse(user.IsValid());
			Assert.IsTrue(user.ValidationErrorMessages.Length == 1,"Should be a too short error message");

			Assert.IsTrue(user.PropertiesValidationErrorMessages.Count ==1,"One property should be invalid") ;

			user.PostalAddress.AddressLine1 = "12345";
			Assert.IsTrue(user.IsValid());

			//setup another nested prop

			user.BillingAddress = new Address();

			user.BillingAddress.AddressLine1 = "12"; // to short
			user.BillingAddress.Country = "New Zealand";

			Assert.IsFalse(user.IsValid());

			Assert.IsTrue(user.ValidationErrorMessages.Length == 1, "Should be one error message about required length");

			Assert.IsTrue(user.PropertiesValidationErrorMessages.Count == 1, "One property should be invalid");

			user.BillingAddress.AddressLine1 = "12345";

			Assert.IsTrue(user.IsValid());
		}

		[Test]
		public void DoesNestedPropertValidateWithARVBNonGeneric()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(UserWithNestedAddressNonGeneric));
			Recreate();


			UserWithNestedAddressNonGeneric user = new UserWithNestedAddressNonGeneric();
			Assert.IsFalse(user.IsValid());
			user.Email = "someemail";

			Assert.IsFalse(user.IsValid(), "Nested class not validated");
			Assert.IsTrue(user.ValidationErrorMessages.Length == 3, "Both nested props are required and should have error messages");

			Assert.IsTrue(user.PropertiesValidationErrorMessages.Count == 2, "Two properties should be invalid");

			user.PostalAddress.AddressLine1 = "15st"; //to short
			user.PostalAddress.Country = "Brazil";
			user.Email = "12345";

			Assert.IsFalse(user.IsValid());
			Assert.IsTrue(user.ValidationErrorMessages.Length == 1, "Should be a too short error message");

			Assert.IsTrue(user.PropertiesValidationErrorMessages.Count == 1, "One property should be invalid");

			user.PostalAddress.AddressLine1 = "12345";
			Assert.IsTrue(user.IsValid());

			//setup another nested prop

			user.BillingAddress = new Address();

			user.BillingAddress.AddressLine1 = "12"; // to short
			user.BillingAddress.Country = "New Zealand";

			Assert.IsFalse(user.IsValid());

			Assert.IsTrue(user.ValidationErrorMessages.Length == 1, "Should be one error message about required length");

			Assert.IsTrue(user.PropertiesValidationErrorMessages.Count == 1, "One property should be invalid");

			user.BillingAddress.AddressLine1 = "12345";

			Assert.IsTrue(user.IsValid());

		}
	}
}
