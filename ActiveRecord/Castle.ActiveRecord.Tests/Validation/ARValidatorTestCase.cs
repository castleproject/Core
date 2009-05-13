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

namespace Castle.ActiveRecord.Tests.Validation
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.Reflection;
	using System.Threading;
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Validation.Model;
	using Castle.Components.Validator;

	[TestFixture]
	public class ARValidatorTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void IsValid()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));

			User user = new User();
			ActiveRecordValidator validator = new ActiveRecordValidator(user);

			Assert.IsFalse(validator.IsValid());

			user.Name = "hammett";
			user.Login = "hammett";
			user.Password = "123";
			user.ConfirmationPassword = "123";
			user.Email = "hammett@gmail.com";

			Assert.IsTrue(validator.IsValid());
		}

		[Test]
		public void HasErrorMessages()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));

			User user = new User();
			ActiveRecordValidator validator = new ActiveRecordValidator(user);

			Assert.IsFalse(validator.IsValid());
			Assert.AreEqual(5, validator.PropertiesValidationErrorMessages.Count);
			Assert.AreEqual(5, validator.ValidationErrorMessages.Length);

			user.Name = "hammett";
			//user.Login = "hammett";
			user.Password = "123";
			user.ConfirmationPassword = "123";
			//user.Email = "hammett@gmail.com";

			Assert.IsFalse(validator.IsValid());

			Assert.AreEqual(2, validator.PropertiesValidationErrorMessages.Count);
			Assert.AreEqual(2, validator.ValidationErrorMessages.Length);

			user.Login = "hammett";
			user.Email = "hammett@gmail.com";

			Assert.IsTrue(validator.IsValid());

			Assert.AreEqual(0, validator.PropertiesValidationErrorMessages.Count);
			Assert.AreEqual(0, validator.ValidationErrorMessages.Length);
		}

		[Test]
		public void HasCorrectErrorMessages()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));

			User user = new User();
			ActiveRecordValidator validator = new ActiveRecordValidator(user);

			PropertyInfo nameProp = user.GetType().GetProperty("Name");
			PropertyInfo loginProp = user.GetType().GetProperty("Login");
			PropertyInfo emailProp = user.GetType().GetProperty("Email");

			Assert.IsFalse(validator.IsValid());

			user.Name = "hammett";
			//user.Login = "hammett";
			user.Password = "123";
			user.ConfirmationPassword = "123";
			//user.Email = "hammett@gmail.com";

			Assert.IsFalse(validator.IsValid());

			Assert.IsFalse(validator.PropertiesValidationErrorMessages.Contains(nameProp));
			Assert.IsTrue(validator.PropertiesValidationErrorMessages.Contains(loginProp));
			Assert.IsTrue(validator.PropertiesValidationErrorMessages.Contains(loginProp));


			user.Login = "hammett";
			user.Email = "hammett@gmail.com";

			Assert.IsTrue(validator.IsValid());

			Assert.IsFalse(validator.PropertiesValidationErrorMessages.Contains(nameProp));
			Assert.IsFalse(validator.PropertiesValidationErrorMessages.Contains(loginProp));
			Assert.IsFalse(validator.PropertiesValidationErrorMessages.Contains(loginProp));
		}
	}
}
