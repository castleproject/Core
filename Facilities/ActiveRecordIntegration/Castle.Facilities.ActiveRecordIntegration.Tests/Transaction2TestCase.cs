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

namespace Castle.Facilities.ActiveRecordIntegration.Tests
{
	using System;
	using Castle.ActiveRecord;
	using NUnit.Framework;

	using Castle.Facilities.ActiveRecordIntegration.Tests.Model;


	[TestFixture]
	public class Transaction2TestCase : AbstractActiveRecordTest
	{
		[Test]
		public void OneLevelTransaction()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			using(new SessionScope())
			{
				Blog.FindAll(); // side effects only

				BlogService service = (BlogService) container[ typeof(BlogService) ];
				service.Create( "name", "author" );
			}

			Assert.AreEqual( 1, Blog.FindAll().Length );
		}

		[Test]
		public void TransactionUsingParentSessionScope1()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			using(new SessionScope())
			{
				Blog.FindAll(); // side effects only

				BlogService service = (BlogService) container[ typeof(BlogService) ];
				Blog blog = service.Create( "name", "author" );

				try
				{
					service.ModifyAndThrowException( blog, "name2" );
					Assert.Fail("Exception expected");
				} catch(Exception) {}
			}

			Assert.AreEqual( 1, Blog.FindAll().Length );
			Blog blog2 = Blog.FindAll()[0];
			Assert.AreEqual( "name", blog2.Name );
		}

		[Test]
		public void TransactionUsingParentSessionScope2()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			using(new SessionScope())
			{
				Blog.FindAll(); // side effects only

				BlogService service = (BlogService) container[ typeof(BlogService) ];
				Blog blog = service.Create( "name", "author" );

				try
				{
					service.ModifyAndThrowException2( blog, "name2" );
					Assert.Fail("Exception expected");
				} 
				catch(Exception) {}
			}

			Assert.AreEqual( 1, Blog.FindAll().Length );
			Blog blog2 = Blog.FindAll()[0];
			Assert.AreEqual( "name", blog2.Name );
		}

		[Test]
		public void OneLevelTransactionRollback1()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			BlogService service = (BlogService) container[ typeof(BlogService) ];

			using(new SessionScope())
			{
				Blog.FindAll(); // side effects only

				try
				{
					service.CreateAndThrowException( "name", "author" );
				}
				catch(Exception)
				{
				}
			}

			Assert.AreEqual( 0, Blog.FindAll().Length );
		}

		[Test]
		public void OneLevelTransactionRollback2()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			BlogService service = (BlogService) container[ typeof(BlogService) ];

			using(new SessionScope())
			{
				Blog.FindAll(); // side effects only

				try
				{
					service.CreateAndThrowException2( "name", "author" );
				}
				catch(Exception)
				{
				}
			}

			Assert.AreEqual( 0, Blog.FindAll().Length );
		}

		[Test]
		public void TwoLevelTransaction()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			FirstService service = (FirstService) container[ typeof(FirstService) ];

			using(new SessionScope())
			{
				Blog.FindAll(); // side effects only

				service.CreateBlogAndPost();
			}

			Assert.AreEqual( 1, Blog.FindAll().Length );
			Assert.AreEqual( 1, Post.FindAll().Length );
		}

		[Test]
		public void TwoLevelTransaction2()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			FirstService service = (FirstService) container[ typeof(FirstService) ];

			using(new SessionScope())
			{
				Blog.FindAll(); // side effects only

				try
				{
					service.CreateBlogAndPost2();
				}
				catch(Exception)
				{
					
				}
			}

			Assert.AreEqual( 0, Blog.FindAll().Length );
			Assert.AreEqual( 0, Post.FindAll().Length );
		}
	}
}
