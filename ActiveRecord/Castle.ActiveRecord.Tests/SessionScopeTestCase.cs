// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	using NUnit.Framework;

	using NHibernate;

	using Castle.ActiveRecord.Tests.Model.LazyModel;


	[TestFixture]
	public class SessionScopeTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void OneDatabaseSameSession()
		{
			ISession session1, session2, session3, session4;

			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );
			Recreate();

			// No scope here
			// So no optimization, thus different sessions

			session1 = Blog.Holder.CreateSession( typeof(Blog) );
			session2 = Blog.Holder.CreateSession( typeof(Blog) );

			Assert.IsNotNull( session1 );
			Assert.IsNotNull( session2 );
			Assert.IsTrue( session1 != session2 );

			Blog.Holder.ReleaseSession(session1);
			Blog.Holder.ReleaseSession(session2);

			// With scope

			using(new SessionScope())
			{
				session1 = Blog.Holder.CreateSession( typeof(Blog) );
				session2 = Blog.Holder.CreateSession( typeof(Post) );
				session3 = Blog.Holder.CreateSession( typeof(Blog) );
				session4 = Blog.Holder.CreateSession( typeof(Post) );

				Assert.IsNotNull( session1 );
				Assert.IsNotNull( session2 );
				Assert.IsNotNull( session3 );
				Assert.IsNotNull( session3 );

				Assert.IsTrue( session2 == session1 );
				Assert.IsTrue( session3 == session1 );
				Assert.IsTrue( session4 == session1 );

				Blog.Holder.ReleaseSession(session1);
				Blog.Holder.ReleaseSession(session2);
				Blog.Holder.ReleaseSession(session3);
				Blog.Holder.ReleaseSession(session4);

				session1 = Blog.Holder.CreateSession( typeof(Post) );
				session2 = Blog.Holder.CreateSession( typeof(Post) );
				session3 = Blog.Holder.CreateSession( typeof(Blog) );
				session4 = Blog.Holder.CreateSession( typeof(Blog) );

				Assert.IsNotNull( session1 );
				Assert.IsNotNull( session2 );
				Assert.IsNotNull( session3 );
				Assert.IsNotNull( session3 );

				Assert.IsTrue( session2 == session1 );
				Assert.IsTrue( session3 == session1 );
				Assert.IsTrue( session4 == session1 );
			}

			// Back to the old behavior

			session1 = Blog.Holder.CreateSession( typeof(Blog) );
			session2 = Blog.Holder.CreateSession( typeof(Blog) );

			Assert.IsNotNull( session1 );
			Assert.IsNotNull( session2 );
			Assert.IsTrue( session1 != session2 );

			Blog.Holder.ReleaseSession(session1);
			Blog.Holder.ReleaseSession(session2);
		}

		[Test]
		public void SessionScopeUsage()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			using(new SessionScope())
			{
				Blog blog = new Blog();
				blog.Author = "hammett";
				blog.Name = "some name";
				blog.Save();

				Post post = new Post(blog, "title", "post contents", "Castle");
				post.Save();
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual( 1, blogs.Length );

			Post[] posts = Post.FindAll();
			Assert.AreEqual( 1, posts.Length );
		}

		[Test]
		public void NestedSessionScopeUsage()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			using(new SessionScope())
			{
				Blog blog = new Blog();

				using(new SessionScope())
				{
					blog.Author = "hammett";
					blog.Name = "some name";
					blog.Save();
				}

				using(new SessionScope())
				{
					Post post = new Post(blog, "title", "post contents", "Castle");
					post.Save();
				}
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual( 1, blogs.Length );

			Post[] posts = Post.FindAll();
			Assert.AreEqual( 1, posts.Length );
		}

		[Test]
		public void NestedSessionScopeAndLazyLoad()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(ProductLazy), typeof(CategoryLazy) );
			Recreate();

			ProductLazy product = new ProductLazy();

			product.Categories.Add( new CategoryLazy("x") );
			product.Categories.Add( new CategoryLazy("y") );
			product.Categories.Add( new CategoryLazy("z") );

			foreach(CategoryLazy cat in product.Categories)
			{
				cat.Save();
			}

			product.Save();

			using(new SessionScope())
			{
				ProductLazy product1 = ProductLazy.Find(product.Id);
				Assert.AreEqual( 3, product1.Categories.Count );

				foreach(CategoryLazy cat in product1.Categories)
				{
					object dummy = cat.Name;
				}

				ProductLazy product2 = ProductLazy.Find(product.Id);
				Assert.AreEqual( 3, product2.Categories.Count );

				using(new SessionScope())
				{
					foreach(CategoryLazy cat in product2.Categories)
					{
						object dummy = cat.Name;
					}
				}

				using(new SessionScope())
				{
					ProductLazy product3 = ProductLazy.Find(product.Id);
					Assert.AreEqual( 3, product3.Categories.Count );

					foreach(CategoryLazy cat in product3.Categories)
					{
						object dummy = cat.Name;
					}
				}
			}
		}
	}
}
