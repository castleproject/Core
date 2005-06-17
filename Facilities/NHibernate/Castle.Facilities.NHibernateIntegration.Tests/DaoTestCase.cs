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
	using System.Threading;

	using NUnit.Framework;

	using Castle.Windsor;

	using Castle.Facilities.AutomaticTransactionManagement;


	[TestFixture]
	public class DaoTestCase : AbstractNHibernateTestCase
	{
		private ManualResetEvent  _startEvent = new ManualResetEvent(false);
		private ManualResetEvent  _stopEvent = new ManualResetEvent(false);
		private BlogDao _dao;

		[Test]
		public void CommonUsage()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("nhibernate", new NHibernateFacility());
			container.AddComponent("blogdao", typeof(BlogDao));
			
			BlogDao dao = (BlogDao) container["blogdao"];
			dao.CreateBlog("my blog");

			IList blogs = dao.ObtainBlogs();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Count );
		}

		[Test]
		public void CommonUsageMultithread()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("nhibernate", new NHibernateFacility());
			container.AddComponent("blogdao", typeof(BlogDao));
			_dao = (BlogDao) container["blogdao"];
			
			const int threadCount = 10;

			Thread[] threads = new Thread[threadCount];
			
			for(int i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(new ThreadStart(ExecuteMethodUntilSignal));
				threads[i].Start();
			}

			_startEvent.Set();

			Thread.CurrentThread.Join(1 * 2000);

			_stopEvent.Set();
		}

		public void ExecuteMethodUntilSignal()
		{
			_startEvent.WaitOne(int.MaxValue, false);

			while (!_stopEvent.WaitOne(1, false))
			{
				_dao.CreateBlog("my blog");
				IList blogs = _dao.ObtainBlogs();
				Assert.IsNotNull( blogs );
			}
		}

		[Test]
		public void TransactionalUsage()
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
		}

		[Test]
		public void TransactionalUsageMultithread()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("nhibernate", new NHibernateFacility());
			container.AddFacility("transaction", new TransactionFacility());

			container.AddComponent("blogdao", typeof(BlogDaoTransactional));
			_dao = (BlogDao) container["blogdao"];
			
			const int threadCount = 10;

			Thread[] threads = new Thread[threadCount];
			
			for(int i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(new ThreadStart(ExecuteMethodUntilSignal));
				threads[i].Start();
			}

			_startEvent.Set();

			Thread.CurrentThread.Join(1 * 2000);

			_stopEvent.Set();
		}
	}
}
