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

namespace Castle.ActiveRecord.Tests
{
	using System;
	using System.Data.SqlClient;
	
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Scopes;
	using Castle.ActiveRecord.Tests.Model;
	using Castle.Model.Configuration;
	
	using NHibernate;
	
	using NUnit.Framework;

	[TestFixture]
	public class DifferentDatabaseScopeTestCase : AbstractActiveRecordTest
	{
		[SetUp]
		public void Setup()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), 
				typeof(Blog), typeof(Post), 
				typeof(OtherDbBlog), typeof(OtherDbPost), typeof(Test2ARBase) );
			Recreate();
		}

		[Test, Category("mssql")]
		public void SimpleCase()
		{
			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			SqlConnection conn = CreateSqlConnection();

			using(conn)
			{
				conn.Open();

				using(new DifferentDatabaseScope(conn))
				{
					blog.Create();
				}
			}

			Blog[] blogs = Blog.FindAll();
			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );

			OtherDbBlog[] blogs2 = OtherDbBlog.FindAll();
			Assert.IsNotNull( blogs2 );
			Assert.AreEqual( 1, blogs2.Length );
		}

		[Test, Category("mssql")]
		public void UsingSessionScope()
		{
			ISession session1, session2;

			using(new SessionScope())
			{
				Blog blog = new Blog();
				blog.Name = "hammett's blog";
				blog.Author = "hamilton verissimo";
				blog.Save();

				session1 = blog.CurrentSession;
				Assert.IsNotNull(session1);

				SqlConnection conn = CreateSqlConnection();

				using(conn)
				{
					conn.Open();

					using(new DifferentDatabaseScope(conn))
					{
						blog.Create();

						session2 = blog.CurrentSession;
						Assert.IsNotNull(session2);

						Assert.IsFalse( Object.ReferenceEquals(session1, session2) );
					}
				}
			}

			Blog[] blogs = Blog.FindAll();
			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );

			OtherDbBlog[] blogs2 = OtherDbBlog.FindAll();
			Assert.IsNotNull( blogs2 );
			Assert.AreEqual( 1, blogs2.Length );
		}

		[Test, Category("mssql")]
		public void UsingTransactionScope()
		{
			SqlConnection conn = CreateSqlConnection();
			ISession session1, session2;

			using(new TransactionScope())
			{
				Blog blog = new Blog();
				blog.Name = "hammett's blog";
				blog.Author = "hamilton verissimo";
				blog.Save();
				
				session1 = blog.CurrentSession;
				Assert.IsNotNull(session1);
				Assert.IsNotNull(session1.Transaction);
				Assert.IsFalse(session1.Transaction.WasCommitted);
				Assert.IsFalse(session1.Transaction.WasRolledBack);

				conn.Open();

				using(new DifferentDatabaseScope(conn))
				{
					blog.Create();

					session2 = blog.CurrentSession;
					Assert.IsNotNull(session2);

					Assert.IsFalse( Object.ReferenceEquals(session1, session2) );

					Assert.IsNotNull(session2.Transaction);
					Assert.IsFalse(session2.Transaction.WasCommitted);
					Assert.IsFalse(session2.Transaction.WasRolledBack);
				}
			}

			Assert.IsFalse(session1.IsOpen);
			Assert.IsFalse(session2.IsOpen);
			Assert.IsTrue(session1.Transaction.WasCommitted);
			Assert.IsFalse(session1.Transaction.WasRolledBack);
			Assert.IsTrue(session2.Transaction.WasCommitted);
			Assert.IsFalse(session2.Transaction.WasRolledBack);

			Blog[] blogs = Blog.FindAll();
			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );

			OtherDbBlog[] blogs2 = OtherDbBlog.FindAll();
			Assert.IsNotNull( blogs2 );
			Assert.AreEqual( 1, blogs2.Length );
		}

		[Test, Category("mssql")]
		public void UsingTransactionScopeWithRollback()
		{
			SqlConnection conn = CreateSqlConnection();
			ISession session1, session2;

			using(TransactionScope scope = new TransactionScope())
			{
				Blog blog = new Blog();
				blog.Name = "hammett's blog";
				blog.Author = "hamilton verissimo";
				blog.Save();
				
				session1 = blog.CurrentSession;
				Assert.IsNotNull(session1);
				Assert.IsNotNull(session1.Transaction);
				Assert.IsFalse(session1.Transaction.WasCommitted);
				Assert.IsFalse(session1.Transaction.WasRolledBack);

				conn.Open();

				using(new DifferentDatabaseScope(conn))
				{
					blog.Create();

					session2 = blog.CurrentSession;
					Assert.IsNotNull(session2);

					Assert.IsFalse( Object.ReferenceEquals(session1, session2) );

					Assert.IsNotNull(session2.Transaction);
					Assert.IsFalse(session2.Transaction.WasCommitted);
					Assert.IsFalse(session2.Transaction.WasRolledBack);

					scope.VoteRollBack();
				}
			}

			Assert.IsFalse(session1.IsOpen);
			Assert.IsFalse(session2.IsOpen);
			Assert.IsFalse(session1.Transaction.WasCommitted);
			Assert.IsTrue(session1.Transaction.WasRolledBack);
			Assert.IsFalse(session2.Transaction.WasCommitted);
			Assert.IsTrue(session2.Transaction.WasRolledBack);

			Blog[] blogs = Blog.FindAll();
			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Length );

			OtherDbBlog[] blogs2 = OtherDbBlog.FindAll();
			Assert.IsNotNull( blogs2 );
			Assert.AreEqual( 0, blogs2.Length );
		}

		[Test, Category("mssql")]
		public void UsingSessionAndTransactionScope()
		{
			SqlConnection conn = CreateSqlConnection();
			ISession session1, session2;

			using(new SessionScope())
			{
				using(TransactionScope scope = new TransactionScope())
				{
					Blog blog = new Blog();
					blog.Name = "hammett's blog";
					blog.Author = "hamilton verissimo";
					blog.Save();
					
					session1 = blog.CurrentSession;
					Assert.IsNotNull(session1);
					Assert.IsNotNull(session1.Transaction);
					Assert.IsFalse(session1.Transaction.WasCommitted);
					Assert.IsFalse(session1.Transaction.WasRolledBack);

					conn.Open();

					using(new DifferentDatabaseScope(conn))
					{
						blog = new Blog();
						blog.Name = "hammett's blog";
						blog.Author = "hamilton verissimo";
						blog.Save();

						session2 = blog.CurrentSession;
						Assert.IsNotNull(session2);

						Assert.IsFalse( Object.ReferenceEquals(session1, session2) );

						Assert.IsNotNull(session2.Transaction);
						Assert.IsFalse(session2.Transaction.WasCommitted);
						Assert.IsFalse(session2.Transaction.WasRolledBack);
					}

					using(new DifferentDatabaseScope(conn))
					{
						blog = new Blog();
						blog.Name = "hammett's blog";
						blog.Author = "hamilton verissimo";
						blog.Save();

						session2 = blog.CurrentSession;
						Assert.IsNotNull(session2);

						Assert.IsFalse( Object.ReferenceEquals(session1, session2) );

						Assert.IsNotNull(session2.Transaction);
						Assert.IsFalse(session2.Transaction.WasCommitted);
						Assert.IsFalse(session2.Transaction.WasRolledBack);
					}
				}
			}

			Assert.IsFalse(session1.IsOpen);
			Assert.IsFalse(session2.IsOpen);
			Assert.IsTrue(session1.Transaction.WasCommitted);
			Assert.IsFalse(session1.Transaction.WasRolledBack);
			Assert.IsTrue(session2.Transaction.WasCommitted);
			Assert.IsFalse(session2.Transaction.WasRolledBack);

			Blog[] blogs = Blog.FindAll();
			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );

			OtherDbBlog[] blogs2 = OtherDbBlog.FindAll();
			Assert.IsNotNull( blogs2 );
			Assert.AreEqual( 2, blogs2.Length );
		}

		[Test, Category("mssql")]
		public void SequenceOfTransactions()
		{
			SqlConnection conn = CreateSqlConnection();
			ISession session1, session2;

			using(new SessionScope())
			{
				using(TransactionScope scope = new TransactionScope())
				{
					Blog blog = new Blog();
					blog.Name = "hammett's blog";
					blog.Author = "hamilton verissimo";
					blog.Save();
					
					session1 = blog.CurrentSession;
					Assert.IsNotNull(session1);
					Assert.IsNotNull(session1.Transaction);
					Assert.IsFalse(session1.Transaction.WasCommitted);
					Assert.IsFalse(session1.Transaction.WasRolledBack);

					conn.Open();

					using(new DifferentDatabaseScope(conn))
					{
						blog = new Blog();
						blog.Name = "hammett's blog";
						blog.Author = "hamilton verissimo";
						blog.Save();

						session2 = blog.CurrentSession;
						Assert.IsNotNull(session2);

						Assert.IsFalse( Object.ReferenceEquals(session1, session2) );

						Assert.IsNotNull(session2.Transaction);
						Assert.IsFalse(session2.Transaction.WasCommitted);
						Assert.IsFalse(session2.Transaction.WasRolledBack);
					}

					using(new DifferentDatabaseScope(conn))
					{
						blog = new Blog();
						blog.Name = "hammett's blog";
						blog.Author = "hamilton verissimo";
						blog.Save();

						session2 = blog.CurrentSession;
						Assert.IsNotNull(session2);

						Assert.IsFalse( Object.ReferenceEquals(session1, session2) );

						Assert.IsNotNull(session2.Transaction);
						Assert.IsFalse(session2.Transaction.WasCommitted);
						Assert.IsFalse(session2.Transaction.WasRolledBack);
					}
				}

				conn.Close();

				using(TransactionScope scope = new TransactionScope())
				{
					Blog blog = new Blog();
					blog.Name = "another blog";
					blog.Author = "erico verissimo";
					blog.Save();
				}
			}

			Assert.IsFalse(session1.IsOpen);
			Assert.IsFalse(session2.IsOpen);
			Assert.IsTrue(session1.Transaction.WasCommitted);
			Assert.IsFalse(session1.Transaction.WasRolledBack);
			Assert.IsTrue(session2.Transaction.WasCommitted);
			Assert.IsFalse(session2.Transaction.WasRolledBack);

			Blog[] blogs = Blog.FindAll();
			Assert.IsNotNull( blogs );
			Assert.AreEqual( 2, blogs.Length );

			OtherDbBlog[] blogs2 = OtherDbBlog.FindAll();
			Assert.IsNotNull( blogs2 );
			Assert.AreEqual( 2, blogs2.Length );
		}

		private SqlConnection CreateSqlConnection()
		{
			IConfigurationSource config = GetConfigSource();
	
			IConfiguration db2 = config.GetConfiguration( typeof(Test2ARBase) );
	
			SqlConnection conn = null;
	
			foreach(IConfiguration child in db2.Children)
			{
				if (child.Name == "hibernate.connection.connection_string")
				{
					conn = new SqlConnection(child.Value);
				}
			}

			return conn;
		}
	}
}
