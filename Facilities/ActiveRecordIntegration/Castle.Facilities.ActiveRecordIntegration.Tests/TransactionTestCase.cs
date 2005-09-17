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

	using NUnit.Framework;

	using Castle.ActiveRecord;
	using Castle.Facilities.ActiveRecordIntegration.Tests.Model;


	[TestFixture]
	public class TransactionTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void OneLevelTransaction()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			BlogService service = (BlogService) container[ typeof(BlogService) ];

			service.Create( "name", "author" );

			Assert.AreEqual( 1, Blog.FindAll().Length );
		}

		[Test]
		public void DicardingChanges()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			SessionScope scope = new SessionScope();
			
			Blog.FindAll(); // side effects only

			BlogService service = (BlogService) container[ typeof(BlogService) ];
			Blog blog = service.Create( "name", "author" );
			
			Assert.AreEqual( 1, Blog.FindAll().Length );

			blog.Name = "joe developer";

			scope.Dispose(true);

			Assert.AreEqual( "name", Blog.FindAll()[0].Name );
		}

		[Test]
		public void DicardingChanges2()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			SessionScope scope = new SessionScope();
			
			Blog.FindAll(); // side effects only

			BlogService service = (BlogService) container[ typeof(BlogService) ];
			Blog blog = service.Create( "name", "author" );
			
			Assert.AreEqual( 1, Blog.FindAll().Length );

			blog.Name = "joe developer";

			Assert.AreEqual( 1, Blog.FindAll().Length );

			scope.Dispose(true);

			Assert.AreEqual( "name", Blog.FindAll()[0].Name );
		}

		[Test]
		public void OneLevelTransactionRollback1()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			BlogService service = (BlogService) container[ typeof(BlogService) ];

			try
			{
				service.CreateAndThrowException( "name", "author" );
			}
			catch(Exception)
			{
			}

			Assert.AreEqual( 0, Blog.FindAll().Length );
		}

		[Test]
		public void OneLevelTransactionRollback2()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			BlogService service = (BlogService) container[ typeof(BlogService) ];

			try
			{
				service.CreateAndThrowException2( "name", "author" );
			}
			catch(Exception)
			{
			}

			Assert.AreEqual( 0, Blog.FindAll().Length );
		}

		[Test]
		public void TwoLevelTransaction()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			FirstService service = (FirstService) container[ typeof(FirstService) ];

			service.CreateBlogAndPost();

			Assert.AreEqual( 1, Blog.FindAll().Length );
			Assert.AreEqual( 1, Post.FindAll().Length );
		}

		[Test]
		public void TwoLevelTransaction2()
		{
			Post.DeleteAll();
			Blog.DeleteAll();

			FirstService service = (FirstService) container[ typeof(FirstService) ];

			try
			{
				service.CreateBlogAndPost2();
			}
			catch(Exception)
			{
				
			}

			Assert.AreEqual( 0, Blog.FindAll().Length );
			Assert.AreEqual( 0, Post.FindAll().Length );
		}
	}
}
