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

namespace Castle.Facilities.NHibernateIntegration.Tests
{
	using System;
	using System.Collections;
	using Castle.Facilities.AutomaticTransactionManagement;
	using Castle.Windsor;

	using NUnit.Framework;


	[TestFixture]
	public class SessionFlushTestCase : AbstractNHibernateTestCase
	{
		[Test]
		public void DeletionNeedsFlush()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("nhibernate", new NHibernateFacility());
			container.AddFacility("transaction", new TransactionFacility());

			container.AddComponent("blogdao", typeof(BlogDao));
			
			BlogDao dao = (BlogDao) container["blogdao"];
			dao.CreateBlog("my blog");

			IList blogs = dao.ObtainBlogs();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Count );

			dao.DeleteAll();

			blogs = dao.ObtainBlogs();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Count );
		}

		[Test]
		public void FlushAndTransaction()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("nhibernate", new NHibernateFacility());
			container.AddFacility("transaction", new TransactionFacility());

			container.AddComponent("blogdao", typeof(BlogDaoTransactional));
			
			BlogDao dao = (BlogDao) container["blogdao"];
			dao.CreateBlog("my blog");

			IList blogs = dao.ObtainBlogs();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Count );

			dao.DeleteAll();

			blogs = dao.ObtainBlogs();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Count );
		}
	}
}
