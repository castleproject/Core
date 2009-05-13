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
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model;

	[TestFixture]
	public class JoinedTableTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void JoinedTable()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), 
				typeof(Post), 
				typeof(Blog), 
				typeof(Company), 
				typeof(Person));

			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();
			Company.DeleteAll();
			Person.DeleteAll();

			Blog blog = new Blog();
			blog.Name = "Ronalodo's blog";
			blog.Author = "Christiano Ronalodo";
			blog.Save();

			Person person = new Person();
			person.Name = "Ronaldo";
			person.FullName = new FullName();
			person.FullName.First = "Christiano";
			person.FullName.Last = "Ronaldo";
			person.Address = "123 Nutmeg";
			person.City = "Lisbon";
			person.Blog = blog;
			person.Save();

			Person[] people = Person.FindAll();
			Assert.AreEqual( 1, people.Length );

			Person personLoaded = people[0];

			Assert.AreEqual(person.Name, personLoaded.Name);
			Assert.AreEqual(person.FullName.First, personLoaded.FullName.First);
			Assert.AreEqual(person.FullName.Last, personLoaded.FullName.Last);
			Assert.AreEqual(person.Address, personLoaded.Address);
			Assert.AreEqual(person.City, personLoaded.City);
			Assert.AreEqual(blog.Id, personLoaded.Blog.Id);
			Assert.AreEqual(blog.Name, personLoaded.Blog.Name);

			personLoaded.FullName.Middle = "Goal";
			personLoaded.Address = "200 Hatrick";
			personLoaded.Update();

			people = Person.FindAll();
			Assert.AreEqual(1, people.Length);

			Person personUpdated = people[0];
			Assert.AreEqual(personLoaded.FullName.Middle, personUpdated.FullName.Middle);
			Assert.AreEqual(personLoaded.Address, personUpdated.Address);
		}
	}
}
