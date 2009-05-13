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
	using Castle.ActiveRecord.Framework;
	using Model.MultipleDBWithMediator;
	using NUnit.Framework;

	[TestFixture]
	public class MultipleDBMediatorTest : AbstractActiveRecordTest
	{
		[SetUp]
		public void Setup()
		{
			Init();

			ActiveRecordStarter.Initialize(GetConfigSource(),
										   typeof(Blog), typeof(Author), typeof(UserDB));

			Recreate();
		}

		[Test]
		public void SessionsAreDifferent()
		{
			using (new SessionScope())
			{
				Blog blog = new Blog();
				Author author = new Author();

				ActiveRecordMediator<Blog>.FindAll();
				ActiveRecordMediator<Author>.FindAll();

				Assert.AreNotSame(blog.CurrentSession, author.CurrentSession);
				Assert.AreNotEqual(
					blog.CurrentSession.Connection.ConnectionString,
					author.CurrentSession.Connection.ConnectionString);
			}
		}

		[Test]
		public void OperateOne()
		{
			Blog[] blogs = ActiveRecordMediator<Blog>.FindAll();

			Assert.AreEqual(0, blogs.Length);

			CreateBlog();

			blogs = ActiveRecordMediator<Blog>.FindAll();
			Assert.AreEqual(1, blogs.Length);
		}

		private static void CreateBlog()
		{
			Blog blog = new Blog();
			blog.Name = "Senseless";
			ActiveRecordMediator<Blog>.Save(blog);
		}

		[Test]
		public void OperateTheOtherOne()
		{
			Author[] authors = ActiveRecordMediator<Author>.FindAll();

			Assert.AreEqual(0, authors.Length);

			CreateAuthor();

			authors = ActiveRecordMediator<Author>.FindAll();
			Assert.AreEqual(1, authors.Length);
		}

		private static void CreateAuthor()
		{
			Author author = new Author();
			author.Name = "Dr. Who";
			ActiveRecordMediator<Author>.Save(author);
		}

		[Test]
		public void OperateBoth()
		{
			Blog[] blogs = ActiveRecordMediator<Blog>.FindAll();
			Author[] authors = ActiveRecordMediator<Author>.FindAll();

			Assert.AreEqual(0, blogs.Length);
			Assert.AreEqual(0, authors.Length);

			CreateBlog();
			CreateAuthor();

			blogs = ActiveRecordMediator<Blog>.FindAll();
			authors = ActiveRecordMediator<Author>.FindAll();

			Assert.AreEqual(1, blogs.Length);
			Assert.AreEqual(1, authors.Length);
		}
	}
}