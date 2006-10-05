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

#if DOTNET2
namespace Castle.ActiveRecord.Tests
{
	using System;
	using System.Text;
	using System.Reflection;
	using System.Collections.Generic;
	using Castle.ActiveRecord.Framework.Queries;
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model.GenericModel;
    using Castle.ActiveRecord.Queries;

	using NHibernate.Expression;

	[TestFixture]
	public class ActiveRecordGenericsTestCase : AbstractActiveRecordTest
	{
		[SetUp]
		public void Setup()
		{
			ActiveRecordStarter.ResetInitializationFlag();
			
			ActiveRecordStarter.Initialize(GetConfigSource(),
				typeof(Blog),
				typeof(Post),
				typeof(Company),
				typeof(Award),
				typeof(Employee),
				typeof(Person));
			Recreate();

            Post.DeleteAll();
            Blog.DeleteAll();
            Company.DeleteAll();
            Award.DeleteAll();
            Employee.DeleteAll();
		}

		[Test]
		public void SimpleOperations()
		{
            Blog[] blogs = Blog.FindAll();

			Assert.IsNotNull(blogs);
			Assert.AreEqual(0, blogs.Length);

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

            blogs = Blog.FindAll();

			Assert.IsNotNull(blogs);
			Assert.AreEqual(1, blogs.Length);

			Blog retrieved = blogs[0];
			Assert.IsNotNull(retrieved);

			Assert.AreEqual(blog.Name, retrieved.Name);
			Assert.AreEqual(blog.Author, retrieved.Author);
		}

        [Test]
        public void ExistsTest()
        {
            Blog[] blogs = Blog.FindAll();

            Assert.IsNotNull(blogs);
            Assert.AreEqual(0, blogs.Length);

            Blog blog = new Blog();
            blog.Name = "hammett's blog";
            blog.Author = "hamilton verissimo";
            blog.Save();

            Assert.IsTrue(blog.Id > 0);
            Assert.IsTrue(Blog.Exists<int>(blog.Id));

            blog = new Blog();
            blog.Name = "chad's blog";
            blog.Author = "chad humphries";
            blog.Save();

            Assert.IsTrue(Blog.Exists<int>(blog.Id));

            Assert.IsFalse(Blog.Exists<int>(1000));
        }

		[Test]
		public void SlicedOperation()
		{
			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			Post post1 = new Post(blog, "title1", "contents", "category1");
			Post post2 = new Post(blog, "title2", "contents", "category2");
			Post post3 = new Post(blog, "title3", "contents", "category3");

			post1.Save();
			post2.Save();
			post3.Published = true;
			post3.Save();

			Post[] posts = Post.SlicedFindAll(1, 2, Expression.Eq("Blog", blog));
			Assert.AreEqual(2, posts.Length);
		}

		[Test]
		public void SimpleOperations2()
		{
			Blog[] blogs = Blog.FindAll();

			Assert.IsNotNull(blogs);
			Assert.AreEqual(0, blogs.Length);

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Create();

			blogs = Blog.FindAll();
			Assert.AreEqual(blog.Name, blogs[0].Name);
			Assert.AreEqual(blog.Author, blogs[0].Author);

			Assert.IsNotNull(blogs);
			Assert.AreEqual(1, blogs.Length);

			blog.Name = "something else1";
			blog.Author = "something else2";
			blog.Update();

			blogs = Blog.FindAll();

			Assert.IsNotNull(blogs);
			Assert.AreEqual(1, blogs.Length);
			Assert.AreEqual(blog.Name, blogs[0].Name);
			Assert.AreEqual(blog.Author, blogs[0].Author);
		}

      
		[Test]
		public void ComponentAttribute()
		{
			Company company = new Company("Castle Corp.");
			company.Address = new PostalAddress(
				"Embau St., 102", "Sao Paulo", "SP", "040390-060");
			company.Save();

			Company[] companies = Company.FindAll();
			Assert.IsNotNull(companies);
			Assert.AreEqual(1, companies.Length);

			Company corp = companies[0];
			Assert.IsNotNull(corp.Address);
			Assert.AreEqual(corp.Address.Address, company.Address.Address);
			Assert.AreEqual(corp.Address.City, company.Address.City);
			Assert.AreEqual(corp.Address.State, company.Address.State);
			Assert.AreEqual(corp.Address.ZipCode, company.Address.ZipCode);
		}

		[Test]
		public void RelationsOneToMany()
		{
			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			Post post1 = new Post(blog, "title1", "contents", "category1");
			Post post2 = new Post(blog, "title2", "contents", "category2");

			post1.Save();
			post2.Save();

			blog = Blog.Find(blog.Id);

			Assert.IsNotNull(blog);
			Assert.IsNotNull(blog.Posts, "posts collection is null");
			Assert.AreEqual(2, blog.Posts.Count);

			foreach (Post post in blog.Posts)
			{
				Assert.AreEqual(blog.Id, post.Blog.Id);
			}
		}

		[Test]
		public void RelationsOneToManyWithWhereAndOrder()
		{
			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			Post post1 = new Post(blog, "title1", "contents", "category1");
			Post post2 = new Post(blog, "title2", "contents", "category2");
			Post post3 = new Post(blog, "title3", "contents", "category3");

			post1.Save();
			System.Threading.Thread.Sleep(1000); // Its a smalldatetime (small precision)
			post2.Save();
			System.Threading.Thread.Sleep(1000); // Its a smalldatetime (small precision)
			post3.Published = true;
			post3.Save();

            blog = Blog.Find(blog.Id);

			Assert.IsNotNull(blog);
			Assert.AreEqual(2, blog.UnPublishedPosts.Count);
			Assert.AreEqual(1, blog.PublishedPosts.Count);

			Assert.AreEqual(3, blog.RecentPosts.Count);
			Assert.AreEqual(post3.Id, (blog.RecentPosts[0] as Post).Id);
			Assert.AreEqual(post2.Id, (blog.RecentPosts[1] as Post).Id);
			Assert.AreEqual(post1.Id, (blog.RecentPosts[2] as Post).Id);
		}


        [Test]
        public void TestExpressionQuerySubProperty() {
            Blog blog = new Blog();
            blog.Name = "hammett's blog";
            blog.Author = "hamilton verissimo";
            blog.Save();

            Blog blog2 = new Blog();
            blog2.Name = "hammett's blog other blog";
            blog2.Author = "hamilton verissimo 2";
            blog2.Save();

            Post post1 = new Post(blog, "title1", "contents", "category1");
            Post post2 = new Post(blog, "title2", "contents", "category2");
            Post post3 = new Post(blog, "title3", "contents", "category3");

            Post post21 = new Post(blog2, "title21", "contents", "category21");
            Post post22 = new Post(blog2, "title22", "contents", "category22");
            Post post23 = new Post(blog2, "title23", "contents", "category23");

            post1.Save();
            post2.Save();
            post3.Save();
            
            post21.Save();
            post22.Save();
            post23.Save();

            //no idea how to make this style of query work. 
            //Post[] posts = Post.FindAll(
            //            Expression.Eq("Blog.Name", blog2.Name)
            //        );
            //Assert.IsTrue(posts.Length > 0);

			SimpleQuery<Post> q = new SimpleQuery<Post>(typeof(Post), "from Post p where p.Id = ? or p.Blog.Name = ?", 1, "hammett's blog other blog");

            Post[] p = q.Execute();

            Assert.IsTrue(p.Length > 0);

        }

		[Test]
		public void RelationsOneToOne()
		{
			Employee emp = new Employee();
			emp.FirstName = "john";
			emp.LastName = "doe";
			emp.Save();

			Assert.AreEqual(1, Employee.FindAll().Length);

			Award award = new Award(emp);
			award.Description = "Invisible employee";
			award.Save();

            Assert.AreEqual(1, Award.FindAll().Length);

            Employee emp2 = Employee.Find(emp.ID);
			Assert.IsNotNull(emp2);
			Assert.IsNotNull(emp2.Award);
			Assert.AreEqual(emp.FirstName, emp2.FirstName);
			Assert.AreEqual(emp.LastName, emp2.LastName);
			Assert.AreEqual(award.Description, emp2.Award.Description);
		}

		[Test]
		[ExpectedException(typeof(NotFoundException))]
		public void FindLoad()
		{
            Blog.Find(1);
		}

		[Test]
		public void SaveUpdate()
		{
			Blog[] blogs = Blog.FindAll();

			Assert.IsNotNull(blogs);
			Assert.AreEqual(0, blogs.Length);

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			blogs = Blog.FindAll();

			Assert.IsNotNull(blogs);
			Assert.AreEqual(1, blogs.Length);

			blog.Name = "Something else";
			blog.Author = "changed too";
			blog.Save();

			blogs = Blog.FindAll();

			Assert.IsNotNull(blogs);
			Assert.AreEqual(1, blogs.Length);

			Assert.AreEqual(blog.Name, blogs[0].Name);
			Assert.AreEqual(blog.Author, blogs[0].Author);
		}

		[Test]
		public void Delete()
		{
			Blog[] blogs = Blog.FindAll();

			Assert.IsNotNull(blogs);
			Assert.AreEqual(0, blogs.Length);

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			blogs = Blog.FindAll();

			Assert.IsNotNull(blogs);
			Assert.AreEqual(1, blogs.Length);

			blog.Delete();

			blogs = Blog.FindAll();

			Assert.IsNotNull(blogs);
			Assert.AreEqual(0, blogs.Length);
		}

		[Test]
		public void ProjectionQueryTest()
		{
			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			ProjectionQuery<Blog, int> proj = new ProjectionQuery<Blog, int>(Projections.RowCount());
			int rowCount = proj.Execute();
			Assert.AreEqual(1, rowCount);
		}

		[Test]
		public void UseBlogWithGenericPostCollection()
		{
			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			
			blog.Save();

			Post p = new Post(blog, "a", "b", "c");
			blog.Posts.Add(p);
			
			p.Save();

			Blog fromDB = Blog.Find(blog.Id);
			Assert.AreEqual(1, fromDB.Posts.Count);
		}
	}
}
#endif
