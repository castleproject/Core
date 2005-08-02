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


	[TestFixture]
	public class TransactionScopeTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void TransactionScopeUsage()
		{
			ISession session1, session2, session3, session4;

			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );
			Recreate();

			using(new TransactionScope())
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
			}

			// Old behavior

			session1 = Blog.Holder.CreateSession( typeof(Blog) );
			session2 = Blog.Holder.CreateSession( typeof(Blog) );

			Assert.IsNotNull( session1 );
			Assert.IsNotNull( session2 );
			Assert.IsTrue( session1 != session2 );

			Blog.Holder.ReleaseSession(session1);
			Blog.Holder.ReleaseSession(session2);
		}

		[Test]
		public void RollbackVote()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			using(TransactionScope transaction = new TransactionScope())
			{
				Blog blog = new Blog();
				blog.Author = "hammett";
				blog.Name = "some name";
				blog.Save();

				Post post = new Post(blog, "title", "post contents", "Castle");
				post.Save();

				// pretend something went wrong

				transaction.VoteRollBack();
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual( 0, blogs.Length );

			Post[] posts = Post.FindAll();
			Assert.AreEqual( 0, posts.Length );
		}

		[Test]
		public void CommitVote()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			using(TransactionScope transaction = new TransactionScope())
			{
				Blog blog = new Blog();
				blog.Author = "hammett";
				blog.Name = "some name";
				blog.Save();

				Post post = new Post(blog, "title", "post contents", "Castle");
				post.Save();

				// Default to VoteCommit
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual( 1, blogs.Length );

			Post[] posts = Post.FindAll();
			Assert.AreEqual( 1, posts.Length );
		}

		[Test]
		public void RollbackUponException()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			using(TransactionScope transaction = new TransactionScope())
			{
				Blog blog = new Blog();
				blog.Author = "hammett";
				blog.Name = "some name";
				blog.Save();

				Post post = new Post(blog, "title", "post contents", "Castle");
				
				try
				{
					post.SaveWithException();
				}
				catch(Exception)
				{
					transaction.VoteRollBack();
				}
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual( 0, blogs.Length );

			Post[] posts = Post.FindAll();
			Assert.AreEqual( 0, posts.Length );
		}

		[Test]
		public void NestedTransactions()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			using(TransactionScope root = new TransactionScope())
			{
				Blog blog = new Blog();

				using(TransactionScope t1 = new TransactionScope(TransactionMode.Inherits))
				{
					blog.Author = "hammett";
					blog.Name = "some name";
					blog.Save();

					t1.VoteCommit();
				}

				using(TransactionScope t2 = new TransactionScope(TransactionMode.Inherits))
				{
					Post post = new Post(blog, "title", "post contents", "Castle");
				
					try
					{
						post.SaveWithException();
					}
					catch(Exception)
					{
						t2.VoteRollBack();
					}
				}
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual( 0, blogs.Length );

			Post[] posts = Post.FindAll();
			Assert.AreEqual( 0, posts.Length );
		}

		[Test]
		public void LotsOsNestedTransactionWithDifferentConfigurations()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			using(TransactionScope root = new TransactionScope())
			{
				using(TransactionScope t1 = new TransactionScope()) // Isolated
				{
					Blog blog = new Blog();

					Blog.FindAll(); // Just to force a session association

					using(TransactionScope t1n = new TransactionScope(TransactionMode.Inherits))
					{
						Blog.FindAll(); // Just to force a session association

						blog.Author = "hammett";
						blog.Name = "some name";
						blog.Save();
					}

					using(TransactionScope t1n = new TransactionScope(TransactionMode.Inherits))
					{
						Post post = new Post(blog, "title", "post contents", "Castle");
				
						post.Save();
					}

					t1.VoteRollBack();
				}

				Blog.FindAll(); // Cant be locked

				using(TransactionScope t2 = new TransactionScope()) 
				{
					Blog blog = new Blog();
					Blog.FindAll(); // Just to force a session association

					using(new TransactionScope())
					{
						Blog.FindAll(); // Just to force a session association

						blog.Author = "hammett";
						blog.Name = "some name";
						blog.Save();
					}

					using(TransactionScope t1n = new TransactionScope(TransactionMode.Inherits)) 
					{
						Post post = new Post(blog, "title", "post contents", "Castle");
				
						try{ post.SaveWithException(); } 
						catch(Exception) { t1n.VoteRollBack(); }
					}
				}

				root.VoteCommit();
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual( 1, blogs.Length );

			Post[] posts = Post.FindAll();
			Assert.AreEqual( 0, posts.Length );
		}

		[Test]
		public void MixingSessionScopeAndTransactionScopes()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			using(new SessionScope())
			{
			using(TransactionScope root = new TransactionScope())
			{
				using(TransactionScope t1 = new TransactionScope()) // Isolated
				{
					Blog blog = new Blog();

					Blog.FindAll(); // Just to force a session association

					using(new TransactionScope(TransactionMode.Inherits))
					{
						Blog.FindAll(); // Just to force a session association

						blog.Author = "hammett";
						blog.Name = "some name";
						blog.Save();
					}

					using(new TransactionScope(TransactionMode.Inherits))
					{
						Post post = new Post(blog, "title", "post contents", "Castle");
				
						post.Save();
					}

					t1.VoteRollBack();
				}

				Blog.FindAll(); // Cant be locked

				using(new TransactionScope()) 
				{
					Blog blog = new Blog();
					Blog.FindAll(); // Just to force a session association

					using(new TransactionScope())
					{
						Blog.FindAll(); // Just to force a session association

						blog.Author = "hammett";
						blog.Name = "some name";
						blog.Save();
					}

					using(TransactionScope t1n = new TransactionScope(TransactionMode.Inherits)) 
					{
						Post post = new Post(blog, "title", "post contents", "Castle");
				
						try{ post.SaveWithException(); } 
						catch(Exception) { t1n.VoteRollBack(); }
					}
				}

				root.VoteCommit();
			}
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual( 1, blogs.Length );

			Post[] posts = Post.FindAll();
			Assert.AreEqual( 0, posts.Length );
		}

		[Test]
		public void MixingSessionScopeAndTransactionScopes2()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(PostLazy), typeof(BlogLazy) );
			Recreate();

			PostLazy.DeleteAll();
			BlogLazy.DeleteAll();

			BlogLazy b = new BlogLazy();

			using(new SessionScope())
			{
				b.Name = "a";
				b.Author = "x";
				b.Save();

				using(new TransactionScope())
				{
					for(int i=1; i <= 10; i++)
					{
						PostLazy post = new PostLazy(b, "t", "c", "General");
						post.Save();
					}
				}
			}

			using(new SessionScope())
			{
				// We should load this outside the transaction scope

				b = BlogLazy.Find(b.Id);

				using(new TransactionScope())
				{
					int total = b.Posts.Count;
					
					foreach(PostLazy p in b.Posts)
					{
						p.Delete();
					}

					b.Delete();
				}
			}

			BlogLazy[] blogs = BlogLazy.FindAll();
			Assert.AreEqual( 0, blogs.Length );

			PostLazy[] posts = PostLazy.FindAll();
			Assert.AreEqual( 0, posts.Length );
		}

		[Test]
		public void MixingSessionScopeAndTransactionScopes3()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(PostLazy), typeof(BlogLazy) );
			Recreate();

			PostLazy.DeleteAll();
			BlogLazy.DeleteAll();

			BlogLazy b = new BlogLazy();

			using(new SessionScope())
			{
				b.Name = "a";
				b.Author = "x";
				b.Save();

				using(new TransactionScope())
				{
					for(int i=1; i <= 10; i++)
					{
						PostLazy post = new PostLazy(b, "t", "c", "General");
						post.Save();
					}
				}
			}

			using(new SessionScope())
			{
				// We should load this outside the transaction scope

				b = BlogLazy.Find(b.Id);

				using(TransactionScope transaction = new TransactionScope())
				{
					int total = b.Posts.Count;
					
					foreach(PostLazy p in b.Posts)
					{
						p.Delete();
					}

					b.Delete();

					transaction.VoteRollBack();
				}
			}

			BlogLazy[] blogs = BlogLazy.FindAll();
			Assert.AreEqual( 1, blogs.Length );

			PostLazy[] posts = PostLazy.FindAll();
			Assert.AreEqual( 10, posts.Length );
		}

	}
}
