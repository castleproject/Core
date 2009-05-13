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
	public class ValidationTestCase : AbstractActiveRecordTest
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

			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(User) );

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
		public void CreateFail1()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));
			Recreate();

			User user = new User();

			user.Create();
		}

		[Test, ExpectedException(typeof(ActiveRecordValidationException))]
		public void CreateFail2()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));
			Recreate();

			User user = new User();

			user.Save();
		}

		[Test, ExpectedException(typeof(ActiveRecordValidationException))]
		public void CreateFail3()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));
			Recreate();

			using (new SessionScope())
			{
				User user = new User();

				user.Save();
			}
		}

		[Test, ExpectedException(typeof(ActiveRecordValidationException))]
		public void CreateFail4()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));
			Recreate();

			using (new TransactionScope())
			{
				User user = new User();

				user.Save();
			}
		}

		[Test, ExpectedException(typeof(ActiveRecordValidationException))]
		public void UpdateFail1()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));
			Recreate();

			int id = CreateNewUser();

			User user = (User)
				ActiveRecordMediator.FindByPrimaryKey(typeof(User), id, true);

			user.Name = "";
			user.Update();
		}

		[Test, ExpectedException(typeof(ActiveRecordValidationException))]
		public void UpdateFail2()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));
			Recreate();

			int id = CreateNewUser();

			User user = (User)
				ActiveRecordMediator.FindByPrimaryKey(typeof(User), id, true);

			user.Name = "";
			user.Save();
		}

		[Test, ExpectedException(typeof(ActiveRecordValidationException))]
		public void UpdateFail3()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));
			Recreate();

			int id = CreateNewUser();

			User user = (User)
				ActiveRecordMediator.FindByPrimaryKey(typeof(User), id, true);

			user.Name = "";
			user.UpdateAndFlush();
		}

		[Test, ExpectedException(typeof(ActiveRecordValidationException))]
		public void UpdateFail4()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));
			Recreate();

			int id = CreateNewUser();

			User user = (User)
				ActiveRecordMediator.FindByPrimaryKey(typeof(User), id, true);

			user.Name = "";
			user.SaveAndFlush();
		}

		[Test, ExpectedException(typeof(ActiveRecordValidationException))]
		public void DeleteFail1()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));
			Recreate();

			int id1 = CreateNewUser();
			int id2 = CreateNewUser();

			using (new SessionScope())
			{
				User user1 = (User) ActiveRecordMediator.FindByPrimaryKey(typeof(User), id1, true);
				User user2 = (User) ActiveRecordMediator.FindByPrimaryKey(typeof(User), id2, true);

				user1.Name = "";
				user2.DeleteAndFlush();
			}
		}

		[Test]
		public void CheckScope()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(User));
			Recreate();

			int id1 = CreateNewUser();
			int id2 = CreateNewUser();

			try
			{
				using (new SessionScope())
				{
					User user1 = (User)ActiveRecordMediator.FindByPrimaryKey(typeof(User), id1, true);
					User user2 = (User)ActiveRecordMediator.FindByPrimaryKey(typeof(User), id2, true);

					user1.Name = "dng";
					user2.Name = "";
				}
			}
			catch (ValidationException)
			{
			}

			User user = (User)ActiveRecordMediator.FindByPrimaryKey(typeof(User), id1, true);
			Assert.AreNotEqual("dng", user.Name);
		}

		[Test, ExpectedException(typeof(ActiveRecordValidationException))]
		public void InvalidClassIsNotPersisted()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(User) );
			Recreate();

			int id = CreateNewUser();

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
			blog.Create();

			blog = new Blog2();
			blog.Name = "hammett";
			
			String[] messages = blog.ValidationErrorMessages;
			Assert.IsTrue(messages.Length == 1);
			Assert.AreEqual("Name is currently in use. Please pick up a new Name.", messages[0]);

			blog.Create();
		}
		
		[Test(Description = "Reproduces AR-136")]
		public void IsUniqueWithInvalidData()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Blog2), typeof(Blog2B) );
			Recreate();

			Blog2B.DeleteAll();

			Blog2B blog = new Blog2B();
			blog.Name = "hammett";
			blog.Create();
			
			blog = new Blog2B();
			blog.Name = "hammett";
			blog.Create();
			
			Blog2 blog2 = new Blog2();
			blog2.Name = blog.Name;
			
			Assert.IsFalse(blog2.IsValid());
		}
		

		[Test]
		[ExpectedException(typeof(ActiveRecordValidationException), ExpectedMessage = "Can't save or update as there is one (or more) field that has not passed the validation test")]
		public void IsUniqueWithNullKey()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Blog4));
			Recreate();

			Blog4.DeleteAll();

			Blog4 blog = new Blog4();
			blog.Name = "hammett";
			blog.Create();

			blog = new Blog4();
			blog.Name = "hammett";

			String[] messages = blog.ValidationErrorMessages;
			Assert.IsTrue(messages.Length == 1);
			Assert.AreEqual("Name is currently in use. Please pick up a new Name.", messages[0]);

			blog.Create();
		}

		[Test]
		public void IsUniqueWithSessionScope()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof (Blog2));
			Recreate();

			Blog2.DeleteAll();

			Blog2 blog = new Blog2();
			blog.Name = "hammett";
			blog.Create();

			using (new SessionScope())
			{
				Blog2 fromDb = Blog2.Find(blog.Id);
				fromDb.Name = "foo";
				fromDb.Save();
			}
		}

		[Test]
		[ExpectedException(typeof(ActiveRecordValidationException), ExpectedMessage = "Can't save or update as there is one (or more) field that has not passed the validation test")]
		public void IsUnique2()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Blog3) );
			Recreate();

			Blog3.DeleteAll();

			Blog3 blog = new Blog3();
			blog.Id = "assignedKey";
			blog.Name = "First Blog";
			blog.Create();

			blog = new Blog3();
			blog.Id = "assignedKey";
			blog.Name = "Second Blog";

			String[] messages = blog.ValidationErrorMessages;
			Assert.IsTrue(messages.Length == 1);
			Assert.AreEqual("The ID you specified already exists.", messages[0]);

			blog.Create();
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
		public void ValidateIsUniqueWithinTransactionScope()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Blog2) );
			Recreate();
			
			// The IsUniqueValidator was created a new SessionScope and causing an
			// error when used inside TransactionScope
			
			using (TransactionScope scope = new TransactionScope())
			{
				Blog2 blog = new Blog2();
				blog.Name = "A cool blog";
				blog.Create();
					
				blog = new Blog2();
				blog.Name = "Another cool blog";
				blog.Create();
				
				scope.VoteCommit();
			}
		}

		[Test, ExpectedException(typeof(ActiveRecordValidationException))]
		public void IsUniqueTimeoutExpiredBug()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Blog2), typeof(Blog5));
			Recreate();

			for (int i = 0; i < 5; i++)
			{
				Blog5 blog = new Blog5();
				blog.Name = "A cool blog";
				blog.Create();
			}

			Blog5 theBlog = new Blog5();
			theBlog.Name = "A cool blog";
			theBlog.Create();

			Blog5 anotherBlog = new Blog5();
			anotherBlog.Name = "A cool blog";
			anotherBlog.Create();
				
			anotherBlog.Name = "A very cool blog";
			anotherBlog.Update();

			Assert.IsFalse(Blog2.Find(theBlog.Id).IsValid());

			Blog2 weblog = new Blog2();
			weblog.Name = theBlog.Name;

			Assert.IsFalse(weblog.IsValid());
			
			weblog.Create();
		}

		private int CreateNewUser()
		{
			int id;

			using (new SessionScope())
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

			return id;
		}
	}
}
