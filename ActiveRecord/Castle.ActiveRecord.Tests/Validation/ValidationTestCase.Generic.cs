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
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;
	using Castle.ActiveRecord.Tests.Validation.Model.GenericModel;
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Validation.GenericModel;
	using Castle.Components.Validator;

	[TestFixture]
	public class ValidationTestCaseGeneric : AbstractActiveRecordTest
	{
		[Test]
		public void IsValid()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(User) );

			User user = new User();

			Assert.IsFalse(user.IsValid()); 

			user.Name = "hammett";
			user.Login = "hammett";
			user.Password = "123";
			user.ConfirmationPassword = "123";
			user.Email = "hammett@gmail.com";

			Assert.IsTrue(user.IsValid()); 
		}

		[Test]
		public void ErrorMessages()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));

			User user = new User();
			Type type = user.GetType();
			PropertyInfo info;
			ArrayList propertyMessages;

			Assert.IsFalse(user.IsValid());
			Assert.AreEqual(5, user.ValidationErrorMessages.Length);
			Assert.AreEqual("This is a required field", user.ValidationErrorMessages[0]);
			Assert.AreEqual("This is a required field", user.ValidationErrorMessages[1]);
			Assert.AreEqual("This is a required field", user.ValidationErrorMessages[2]);
			Assert.AreEqual("This is a required field", user.ValidationErrorMessages[3]);
			Assert.AreEqual("This is a required field", user.ValidationErrorMessages[4]);

			Assert.AreEqual(5, user.PropertiesValidationErrorMessages.Count);

			info = type.GetProperty("Login");
			Assert.IsTrue(user.PropertiesValidationErrorMessages.Contains(info));
			propertyMessages = (ArrayList)user.PropertiesValidationErrorMessages[info];
			Assert.AreEqual(1, propertyMessages.Count);
			Assert.AreEqual("This is a required field", propertyMessages[0]);

			info = type.GetProperty("Name");
			Assert.IsTrue(user.PropertiesValidationErrorMessages.Contains(info));
			propertyMessages = (ArrayList)user.PropertiesValidationErrorMessages[info];
			Assert.AreEqual(1, propertyMessages.Count);
			Assert.AreEqual("This is a required field", propertyMessages[0]);

			info = type.GetProperty("Email");
			Assert.IsTrue(user.PropertiesValidationErrorMessages.Contains(info));
			propertyMessages = (ArrayList)user.PropertiesValidationErrorMessages[info];
			Assert.AreEqual(1, propertyMessages.Count);
			Assert.AreEqual("This is a required field", propertyMessages[0]);

			info = type.GetProperty("Password");
			Assert.IsTrue(user.PropertiesValidationErrorMessages.Contains(info));
			propertyMessages = (ArrayList)user.PropertiesValidationErrorMessages[info];
			Assert.AreEqual(1, propertyMessages.Count);
			Assert.AreEqual("This is a required field", propertyMessages[0]);

			info = type.GetProperty("ConfirmationPassword");
			Assert.IsTrue(user.PropertiesValidationErrorMessages.Contains(info));
			propertyMessages = (ArrayList)user.PropertiesValidationErrorMessages[info];
			Assert.AreEqual(1, propertyMessages.Count);
			Assert.AreEqual("This is a required field", propertyMessages[0]);

			user.Name = "hammett";
			user.Login = "hammett";
			user.Email = "hammett@gmail.com";
			user.Password = "123x";
			user.ConfirmationPassword = "123";

			Assert.IsFalse(user.IsValid());
			Assert.AreEqual(1, user.ValidationErrorMessages.Length);
			Assert.AreEqual("Fields do not match", user.ValidationErrorMessages[0]);

			info = type.GetProperty("Password");
			Assert.IsTrue(user.PropertiesValidationErrorMessages.Contains(info));
			propertyMessages = (ArrayList)user.PropertiesValidationErrorMessages[info];
			Assert.AreEqual(1, propertyMessages.Count);
			Assert.AreEqual("Fields do not match", propertyMessages[0]);

			user.Password = "123";

			Assert.IsTrue(user.IsValid());
			Assert.AreEqual(0, user.ValidationErrorMessages.Length);
		}

		[Test, ExpectedException(typeof(ActiveRecordValidationException))]
		public void InvalidClassIsNotPersisted()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(User) );
			Recreate();

			int id;

			using(new SessionScope())
			{
				User user = new User();

				user.Name = "hammett";
				user.Login = "hammett";
				user.Email = "hammett@gmail.com";
				user.ConfirmationPassword = "123";
				user.Password = "123";

				user.Save();

				id = user.Id;
			}

			{
				User user = (User) 
					ActiveRecordMediator.FindByPrimaryKey(typeof(User), id, true);

				Assert.AreEqual("hammett@gmail.com", user.Email);
				Assert.AreEqual("123", user.Password);
				Assert.AreEqual("123", user.ConfirmationPassword);
			}

			using(new SessionScope())
			{
				User user = (User) 
					ActiveRecordMediator.FindByPrimaryKey(typeof(User), id, true);

				user.Email = "wrong";
				user.ConfirmationPassword = "123x";
				user.Password = "123y";
			}
		}

		[Test]
		[ExpectedException(typeof(ActiveRecordValidationException), ExpectedMessage = "Can't save or update as there is one (or more) field that has not passed the validation test")]
		public void IsUnique()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Blog2) );
			Recreate();

			Blog2.DeleteAll();

			Blog2 blog = new Blog2();
			blog.Name = "hammett";
			blog.Save();

			blog = new Blog2();
			blog.Name = "hammett";
			
			String[] messages = blog.ValidationErrorMessages;
			Assert.IsTrue(messages.Length == 1);
			Assert.AreEqual("Name is currently in use. Please pick up a new Name.", messages[0]);

			blog.Save();
		}

		[Test]
		public void IsUniqueDoesNotDeadlockOnAutoflushTransaction()
		{
			InPlaceConfigurationSource source = (InPlaceConfigurationSource)GetConfigSource();
			DefaultFlushType originalType = source.DefaultFlushType;
			try
			{
				ActiveRecordStarter.Initialize(source, typeof(Blog2));
				Recreate();
				source.DefaultFlushType = DefaultFlushType.Auto;

				using (new TransactionScope())
				{
					Blog2.DeleteAll();
					Blog2 blog = new Blog2();
					blog.Name = "FooBar";
					blog.Save();
				}
			}
			finally
			{
				source.DefaultFlushType = originalType;
			}
		}

		[Test]
		public void Hierarchy()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Person), typeof(Customer) );
			// Recreate();

			Person p1 = new Person();
			
			Assert.IsFalse( p1.IsValid() );

			p1.Name = "hammett";
			p1.Age = 25;

			Assert.IsTrue( p1.IsValid() );

			Customer c1 = new Customer();

			Assert.IsFalse( c1.IsValid() );

			c1.ContactName = "someone";
			c1.Phone = "11";

			Assert.IsFalse( c1.IsValid() );

			c1.Name = "hammett";

			Assert.IsTrue( c1.IsValid() );
		}

		[Test]
		public void ValidateIsUnique()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(TimeSlotFixedDate));
			Recreate();

			TimeSlotFixedDate timeDate = new TimeSlotFixedDate();
			timeDate.Hour = 1;
			timeDate.Name = null;

			Assert.IsFalse(timeDate.IsValid());
		} 
	}
}
