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

namespace PetStore.Model.Tests
{
	using System.Linq;
	using Castle.ActiveRecord;
	using Castle.Components.Validator;
	using NUnit.Framework;

	[TestFixture]
	public class BasicUserTests : TestBase
	{
		[Test]
		public void RunsInASessionScope()
		{
			Assert.That(SessionScope.Current, Is.Not.Null);
		}

		[Test]
		public void ShouldBeAbleToCreateAnUser()
		{
			var user = new User()
			           	{
			           		Login = "bluechip1",
			           		Email = "purchasing@bluechip.com",
			           		Name = "John D. Farrow",
			           		Password = "mommy"
			           	};
			ActiveRecordMediator<User>.Save(user);
		}

		[Test]
		public void CannotCreateAnUserWithoutEmail()
		{
			Assert.Throws<ValidationException>(() =>
			                                   ActiveRecordMediator<User>.Save(
			                                   	new User {
			                                   	         	Login = "", Name = "A. N. Onymous", 
			                                   	         	Password = "",Email=""}
			                                   	));
		}

		[Test]
		public void AnUserWithoutEmailIsInvalid()
		{
			IValidatorRegistry registry = new CachedValidationRegistry();
			IValidatorRunner runner = new ValidatorRunner(registry);
			var user = new User
			           	{
			           		Login = "", Name = "A. N. Onymous", 
			           		Password = "", Email =""
			           	};
			Assert.That(runner.IsValid(user), Is.False);
		}

		[Test]
		public void SupportsActiveRecordSemantics()
		{
			var user = new User()
			           	{
			           		Login = "bluechip1",
			           		Email = "purchasing@bluechip.com",
			           		Name = "John D. Farrow",
			           		Password = "mommy"
			           	};

			user.Save();

			var users = (from u in Storage<User>.Linq() 
			             where u.Email.Contains("blue")
			             select u).ToList();
			Assert.That(users.Count,Is.EqualTo(1));
		}
	}
}