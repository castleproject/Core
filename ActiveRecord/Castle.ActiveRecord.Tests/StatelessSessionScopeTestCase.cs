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
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;
	using Castle.ActiveRecord.Framework.Scopes;
	using Castle.ActiveRecord.Tests.Model;
	using Castle.ActiveRecord.Tests.Model.LazyModel;
	using NHibernate;
	using NHibernate.Criterion;
	using NUnit.Framework;
	using System.Collections;
	using Castle.ActiveRecord.Queries;

	[TestFixture]
	public class StatelessSessionScopeTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void SessionIsStateless()
		{
			Initialize();
			using (new StatelessSessionScope())
			{
				Assert.IsAssignableFrom(typeof(StatelessSessionWrapper), Blog.Holder.CreateSession(typeof(Blog)));
			}
		}

		[Test]
		public void Unsupported_actions_should_have_squattery_exceptions()
		{
			Initialize();
			using (new StatelessSessionScope())
			{
				try
				{
					Blog.Holder.CreateSession(typeof(Blog)).Merge(null);
					Assert.Fail();
				}
				catch (NotImplementedException ex)
				{
					Assert.AreEqual(@"The called method is not supported.
ActiveRecord is currently running within a StatelessSessionScope. Stateless sessions are faster than normal sessions, but they do not support all methods and properties that a normal session allows. 
Please check the stacktrace and change your code accordingly.", ex.Message);
				}
			}
		}

		[Test]
		public void A_simple_object_can_be_created()
		{
			Initialize();
			using (new StatelessSessionScope())
				CreateBlog();

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual(1, blogs.Length);
			Assert.AreEqual("Mort", blogs[0].Author);
		}

		[Test]
		public void A_simple_object_can_be_read()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Ship));
			Recreate();

			using (new SessionScope())
				ActiveRecordMediator<Ship>.Create(new Ship { Name = "Andrea Doria" });

			using (new StatelessSessionScope())
			{
				Assert.IsTrue(ActiveRecordMediator<Ship>.Exists(1));
				Assert.AreEqual("Andrea Doria",ActiveRecordMediator<Ship>.FindByPrimaryKey(1).Name);
			}
		}

		[Test]
		public void Get_with_lazy_classes_does_work()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(BlogLazy), typeof(PostLazy));
			Recreate();

			using (new SessionScope())
				new BlogLazy { Author = "Mort", Name = "Hourglass" }.Create();

			using (new StatelessSessionScope())
			{
				Assert.AreEqual("Mort", BlogLazy.Find(1).Author);
				// The assert below cannot work, stateless sessions cannot serve proxies.
				// Assert.AreEqual(0, BlogLazy.Find(1).Posts.Count);
			}
		}

		[Test]
		public void Get_with_nonlazy_classes_does_not_work()
		{
			Initialize();

			using (new SessionScope())
				new Blog { Author = "Mort", Name = "Hourglass" }.Create();

			using (new StatelessSessionScope())
			{
				try
				{
					Blog.Find(1);
					Assert.Fail();
				}
				catch(ActiveRecordException ex)
				{
					Assert.AreEqual(typeof(SessionException),ex.InnerException.GetType());
					Assert.AreEqual("collections cannot be fetched by a stateless session", ex.InnerException.Message);
				}
			}
		}

		[Test]
		public void Updating_stateless_fetched_entities_works()
		{
			InitializeLazy();

			using (new SessionScope())
				CreateLazyBlog();

			using (new StatelessSessionScope())
			{
				var blog = BlogLazy.Find(1);
				Assert.AreEqual("Hourglass", blog.Name);
				blog.Name = "HOURGLASS";
				blog.Update();
			}

			Assert.AreEqual("HOURGLASS", BlogLazy.Find(1).Name);
		}

		[Test]
		public void Updating_detached_entities_works()
		{
			Initialize();
			Blog blog;

			using (new SessionScope())
			{
				CreateBlog();
				blog = Blog.Find(1);
			}

			using (new StatelessSessionScope())
			{
				Assert.AreEqual("Hourglass", blog.Name);
				blog.Name = "HOURGLASS";
				blog.Update();
			}

			Assert.AreEqual("HOURGLASS", Blog.Find(1).Name);
		}

		[Test]
		public void Inversively_adding_to_a_detached_entitys_collections_works()
		{
			Initialize();
			Blog blog;

			using (new SessionScope())
			{
				CreateBlog();
				blog = Blog.Find(1);
			}

			using (new StatelessSessionScope())
			{
				for (int i = 0; i < 10; i++)
				{
					var post = new Post() { Blog = blog, Title = "Post" + i, Created = DateTime.Now };
					post.Create();
				}
			}

			Assert.AreEqual(10, Blog.Find(1).Posts.Count);
		}


		[Test]
		public void Updating_detached_entities_collections_does_not_work()
		{
			Initialize();
			Blog blog;

			using (new SessionScope())
			{
				CreateBlog();
				blog = Blog.Find(1);
			}

			using (new StatelessSessionScope())
			{
				blog.Posts = new ArrayList();

				for (int i = 0; i < 10; i++)
				{
					var post = new Post() { Title = "Post" + i, Created = DateTime.Now};
					post.Create();
					blog.Posts.Add(post);
				}

				blog.Update();
			}

			Assert.AreEqual(0, Blog.Find(1).Posts.Count);
		}

		[Test]
		public void Transactions_are_supported()
		{
			Initialize();

			using (new StatelessSessionScope())
			using (new TransactionScope())
			{
				CreateBlog();
			}

			Assert.AreEqual("Mort", Blog.Find(1).Author);
		}

		[Test]
		public void Querying_works_with_HQL()
		{
			InitializeLazy();
			using (new SessionScope())
				CreateLazyBlog();

			var query = new SimpleQuery<BlogLazy>("from BlogLazy b where b.Author = ?", "Mort");
			using (new StatelessSessionScope())
				Assert.AreEqual(1, query.Execute().Length);

		}

		[Test]
		public void Enumerating_with_queries_doesnt_work()
		{
			InitializeLazy();
			using (new SessionScope())
				CreateLazyBlog();

			var query = new SimpleQuery<BlogLazy>("from BlogLazy b where b.Author = ?", "Mort");
			using (new StatelessSessionScope())
				query.Enumerate();
		}

		[Test]
		public void Can_delete_instances()
		{
			InitializeLazy();
			using (new SessionScope())
				CreateLazyBlog();

			var query = new SimpleQuery<BlogLazy>("from BlogLazy b where b.Author = ?", "Mort");
			using (new StatelessSessionScope())
			{
				foreach (var blog in query.Execute())
					blog.Delete();
			}

			Assert.AreEqual(0, BlogLazy.FindAll().Length);
		}

		[Test]
		public void Querying_works_with_Detached_Criteria()
		{
			InitializeLazy();
			using (new SessionScope())
				CreateLazyBlog();

			var crit = DetachedCriteria.For<BlogLazy>().Add(Expression.Eq("Author", "Mort"));
			using (new StatelessSessionScope())
				Assert.AreEqual(1, ActiveRecordMediator<BlogLazy>.FindAll(crit).Length);

		}

		[Test]
		public void Collections_can_be_fetched_with_queries()
		{
			InitializeLazy();

			var blog = new BlogLazy { Author = "Mort", Name = "Hourglass" };
			var post = new PostLazy { Blog = blog, Title = "...", Created = DateTime.Now };

			blog.Save();
			post.Save();

			var query = new SimpleQuery<BlogLazy>("from BlogLazy b join fetch b.Posts");

			using (new StatelessSessionScope())
			{
				var result = query.Execute();
				Assert.AreEqual(1, result.Length);
				Assert.AreEqual(1, result[0].Posts.Count);
				var queriedPost = result[0].Posts[0] as PostLazy;
				Assert.AreEqual("...", queriedPost.Title);
			}
		}

		[Test]
		public void Nonlazy_collections_can_be_fetched_with_queries()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(SimpleBlog), typeof(SimplePost));
			Recreate();

			var blog = new SimpleBlog { Name = "Blog" };
			var post = new SimplePost { Blog = blog, Title = "Post" };
			blog.Save();
			post.Save();

			var query = new SimpleQuery<SimpleBlog>("from SimpleBlog b join fetch b.Posts");

			using (new StatelessSessionScope())
			{
				var result = query.Execute();
				Assert.AreEqual(1, result.Length);
				Assert.AreEqual(1, result[0].Posts.Count);
				Assert.AreEqual("Post", result[0].Posts[0].Title);
				Assert.AreSame(result[0], result[0].Posts[0].Blog);
			}
		}

		private void Initialize()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
			Recreate();
		}

		private void InitializeLazy()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(PostLazy), typeof(BlogLazy));
			Recreate();
		}

		private void CreateBlog()
		{
			new Blog { Author = "Mort", Name = "Hourglass" }.Create();
		}


		private void CreateLazyBlog()
		{
			new BlogLazy { Author = "Mort", Name = "Hourglass" }.Create();
		}
	}
}
