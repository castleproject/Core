// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
#if dotNet2
namespace Castle.ActiveRecord.Tests.Validation
{
	using System;

	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Validation.GenericModel;

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
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(User) );

			User user = new User();

			Assert.IsFalse(user.IsValid()); 
			Assert.AreEqual(5, user.ValidationErrorMessages.Length);
			Assert.AreEqual("Login is not optional.", user.ValidationErrorMessages[0]);
			Assert.AreEqual("Name is not optional.", user.ValidationErrorMessages[1]);
			Assert.AreEqual("Email is not optional.", user.ValidationErrorMessages[2]);
			Assert.AreEqual("Password is not optional.", user.ValidationErrorMessages[3]);
			Assert.AreEqual("ConfirmationPassword is not optional.", user.ValidationErrorMessages[4]);

			user.Name = "hammett";
			user.Login = "hammett";
			user.Email = "hammett@gmail.com";
			user.Password = "123x";
			user.ConfirmationPassword = "123";
			
			Assert.IsFalse(user.IsValid()); 
			Assert.AreEqual(1, user.ValidationErrorMessages.Length);
			Assert.AreEqual("Field Password doesn't match with confirmation.", user.ValidationErrorMessages[0]);

			user.Password = "123";

			Assert.IsTrue(user.IsValid()); 
			Assert.AreEqual(0, user.ValidationErrorMessages.Length);
		}

		[Test, ExpectedException(typeof(ValidationException))]
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
		[ExpectedException(typeof(ValidationException), "Can't save or update as there is one (or more) field that has not passed the validation test")]
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
	}
}
#endif