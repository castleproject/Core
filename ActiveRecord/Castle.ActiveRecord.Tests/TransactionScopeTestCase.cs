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
	using NUnit.Framework;
	using NHibernate;
	using Castle.ActiveRecord.Tests.Model;

	[TestFixture]
	public class TransactionScopeTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void TransactionScopeUsage()
		{
			ISession session1, session2, session3, session4;

			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
			Recreate();

			using(new TransactionScope())
			{
				session1 = Blog.Holder.CreateSession(typeof(Blog));
				session2 = Blog.Holder.CreateSession(typeof(Post));
				session3 = Blog.Holder.CreateSession(typeof(Blog));
				session4 = Blog.Holder.CreateSession(typeof(Post));

				Assert.IsNotNull(session1);
				Assert.IsNotNull(session2);
				Assert.IsNotNull(session3);
				Assert.IsNotNull(session3);

				Assert.IsTrue(session2 == session1);
				Assert.IsTrue(session3 == session1);
				Assert.IsTrue(session4 == session1);
			}

			// Old behavior

			session1 = Blog.Holder.CreateSession(typeof(Blog));
			session2 = Blog.Holder.CreateSession(typeof(Blog));

			Assert.IsNotNull(session1);
			Assert.IsNotNull(session2);
			Assert.IsTrue(session1 != session2);

			Blog.Holder.ReleaseSession(session1);
			Blog.Holder.ReleaseSession(session2);
		}

		[Test]
		public void RollbackVote()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
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
			Assert.AreEqual(0, blogs.Length);

			Post[] posts = Post.FindAll();
			Assert.AreEqual(0, posts.Length);
		}

		[Test]
		public void RollbackOnDispose()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			using(TransactionScope transaction = new TransactionScope(OnDispose.Rollback))
			{
				Blog blog = new Blog();
				blog.Author = "hammett";
				blog.Name = "some name";
				blog.Save();

				Post post = new Post(blog, "title", "post contents", "Castle");
				post.Save();
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual(0, blogs.Length);

			Post[] posts = Post.FindAll();
			Assert.AreEqual(0, posts.Length);
		}

		[Test]
		public void CommitVote()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
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
			Assert.AreEqual(1, blogs.Length);

			Post[] posts = Post.FindAll();
			Assert.AreEqual(1, posts.Length);
		}

		[Test]
		public void RollbackUponException()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
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
			Assert.AreEqual(0, blogs.Length);

			Post[] posts = Post.FindAll();
			Assert.AreEqual(0, posts.Length);
		}

		[Test]
		public void NestedTransactions()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
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
			Assert.AreEqual(0, blogs.Length);

			Post[] posts = Post.FindAll();
			Assert.AreEqual(0, posts.Length);
		}

		[Test]
		public void NestedTransactionScopesHaveCorrectTransactionContexts()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
			Recreate();
			Post.DeleteAll();
			Blog.DeleteAll();

			using (TransactionScope t1 = new TransactionScope())
			{
				Blog blog1 = new Blog();
				Blog.FindAll();

				ISession s1 = blog1.CurrentSession;
				ITransaction tx1 = s1.Transaction;
				Assert.IsNotNull(tx1);

				using (TransactionScope t2 = new TransactionScope())
				{
					Blog blog2 = new Blog();
					Blog.FindAll();
					ISession s2 = blog2.CurrentSession;
					ITransaction tx2 = s2.Transaction;

					Assert.IsNotNull(tx2);
					Assert.AreNotSame(tx1, tx2);
					
					// TransactionScope uses a new session!
					// Assert.AreSame(s1, s2);
				}

				using (TransactionScope t3 = new TransactionScope(TransactionMode.Inherits))
				{
					Blog blog3 = new Blog();
					Blog.FindAll();
					ITransaction tx3 = blog3.CurrentSession.Transaction;

					Assert.IsNotNull(tx3);
					Assert.AreSame(tx1, tx3);
				}

				Assert.IsTrue(tx1.IsActive);
			}

			using (new SessionScope())
			{
				Blog blog4 = new Blog();
				Blog.FindAll();

				using (new TransactionScope())
				{
					Blog blog5 = new Blog();

					Assert.AreSame(blog4.CurrentSession.Transaction, blog5.CurrentSession.Transaction);
				}
			}

			using (new SessionScope())
			{
				Blog blog6 = new Blog();
				ISession session = blog6.CurrentSession;

				Assert.IsNotNull(session.Transaction);
				ITransaction tx4 = session.Transaction;
				using (ITransaction tx5 = session.BeginTransaction())
				{
					Assert.AreSame(tx4, tx5);
					Blog.FindAll();

					using (ITransaction tx6 = session.BeginTransaction())
					{
						Assert.AreSame(tx5, tx6);
					}
				}
			}
		}

		[Test]
		public void LotsOfNestedTransactionWithDifferentConfigurations()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
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

						try
						{
							post.SaveWithException();
						}
						catch(Exception)
						{
							t1n.VoteRollBack();
						}
					}
				}

				root.VoteCommit();
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual(1, blogs.Length);

			Post[] posts = Post.FindAll();
			Assert.AreEqual(0, posts.Length);
		}

		[Test]
		public void MixingSessionScopeAndTransactionScopes()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
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

							try
							{
								post.SaveWithException();
							}
							catch(Exception)
							{
								t1n.VoteRollBack();
							}
						}
					}

					root.VoteCommit();
				}
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual(1, blogs.Length);

			Post[] posts = Post.FindAll();
			Assert.AreEqual(0, posts.Length);
		}

		[Test]
		public void MixingSessionScopeAndTransactionScopes2()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(PostLazy), typeof(BlogLazy));
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
					for(int i = 1; i <= 10; i++)
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
			Assert.AreEqual(0, blogs.Length);

			PostLazy[] posts = PostLazy.FindAll();
			Assert.AreEqual(0, posts.Length);
		}

		[Test]
		public void MixingSessionScopeAndTransactionScopes3()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(PostLazy), typeof(BlogLazy));
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
					for(int i = 1; i <= 10; i++)
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
			Assert.AreEqual(1, blogs.Length);

			PostLazy[] posts = PostLazy.FindAll();
			Assert.AreEqual(10, posts.Length);
		}

		[Test]
		public void MixingSessionScopeAndTransactionScopes4()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog b = new Blog();
			Post post = null;

			{
				b.Name = "a";
				b.Author = "x";
				b.Save();

				post = new Post(b, "t", "c", "General");
				post.Save();
			}

			using(new SessionScope())
			{
				using(new TransactionScope(TransactionMode.Inherits))
				{
					b = Blog.Find(b.Id);
					b.Name = "changed";
					b.Save();
				}

				{
					Post post2 = Post.Find(post.Id);
					b = Blog.Find(b.Id);
				}

				using(new TransactionScope(TransactionMode.Inherits))
				{
					Post post2 = Post.Find(post.Id);
					b = Blog.Find(b.Id);
				}
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual(1, blogs.Length);

			Post[] posts = Post.FindAll();
			Assert.AreEqual(1, posts.Length);
		}

		[Test]
		public void NestedTransactionWithRollbackOnDispose()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			using(new TransactionScope())
			{
				Blog blog = new Blog();

				using(TransactionScope t1 = new TransactionScope(TransactionMode.Inherits, OnDispose.Rollback))
				{
					blog.Author = "hammett";
					blog.Name = "some name";
					blog.Save();

					t1.VoteCommit();
				}

				using(TransactionScope t2 = new TransactionScope(TransactionMode.Inherits, OnDispose.Rollback))
				{
					Post post = new Post(blog, "title", "post contents", "Castle");

					try
					{
						post.SaveWithException();

						t2.VoteCommit(); // Will never be called
					}
					catch(Exception)
					{
						// t2.VoteRollBack();
					}
				}
			}

			Blog[] blogs = Blog.FindAll();
			Assert.AreEqual(0, blogs.Length);

			Post[] posts = Post.FindAll();
			Assert.AreEqual(0, posts.Length);
		}

		[Test]
		public void ReportedProblemOnForum()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), 
				typeof(Company), 
				typeof(Person),
				typeof(Blog),
				typeof(Post));
			Recreate();

			using(new TransactionScope())
			{
				Company comp1 = new Company("comp1");
				comp1.Create();

				Company comp2 = new Company("comp2");
				comp2.Create();
			}
		}

		[Test]
		public void ExplicitFlushInsideSecondTransactionProblem()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), 
				typeof(Company), 
				typeof(Person),
				typeof(Blog),
				typeof(Post));
			Recreate();

			Company comp1 = new Company("comp1");
			Company comp2 = new Company("comp2");
			using(new SessionScope())
			{
				comp1.Create();
				comp2.Create();
			}

			using(new SessionScope(FlushAction.Never))
			{
				using(TransactionScope tx = new TransactionScope(OnDispose.Rollback))
				{
					Company comp2a = Company.Find(comp2.Id);
					comp2a.Name = "changed";
					tx.VoteCommit();
				}

				using(new TransactionScope(OnDispose.Rollback))
				{
					Company[] changedCompanies = ActiveRecordMediator<Company>.FindAllByProperty("Name", "changed");
					Assert.AreEqual(1, changedCompanies.Length);
					Company e2a = changedCompanies[0];
					e2a.Delete();

					SessionScope.Current.Flush();

					Assert.AreEqual(0, ActiveRecordMediator<Company>.FindAllByProperty("Name", "changed").Length);
				}
			}
		}
	}
}
