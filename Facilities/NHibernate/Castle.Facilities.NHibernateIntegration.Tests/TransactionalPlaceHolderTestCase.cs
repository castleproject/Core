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

	using Castle.Windsor;
	using Castle.Facilities.AutomaticTransactionManagement;

	using NUnit.Framework;


	[TestFixture]
	public class TransactionalPlaceHolderTestCase : AbstractNHibernateTestCase
	{
		private ManualResetEvent _startEvent = new ManualResetEvent(false);
		private ManualResetEvent _stopEvent = new ManualResetEvent(false);
		private BlogDao _dao;

		[Test]
		public void BusinessLayerWithTransactions()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("transaction", new TransactionFacility());
			
			container.AddComponent("blogdao", typeof(BlogDao));
			container.AddComponent("business", typeof(MyBusinessClass));

			MyBusinessClass service = (MyBusinessClass) container[typeof(MyBusinessClass)];
			Blog blog = service.Create("myblog");

			BlogDao dao = (BlogDao) container["blogdao"];

			IList blogs = dao.ObtainBlogs();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Count );
		}

		[Test]
		[Ignore("Just for a while")]
		public void BusinessLayerWithTransactionsAndThreads()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("transaction", new TransactionFacility());
			
			container.AddComponent("blogdao", typeof(BlogDao));
			container.AddComponent("business", typeof(MyBusinessClass));
			
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

		[Test]
		[Ignore("Just for a while")]
		public void BusinessLayerWithTransactionsAndThreads2()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("transaction", new TransactionFacility());
			
			container.AddComponent("blogdao", typeof(BlogDaoTransactional));
			container.AddComponent("business", typeof(MyBusinessClass));
			
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
				Assert.IsTrue( blogs.Count > 0 );
			}
		}

		[Test]
		public void Rollback()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("transaction", new TransactionFacility());
			container.AddComponent("blogdao", typeof(BlogDao));
			container.AddComponent("business", typeof(MyBusinessClass));

			MyBusinessClass service = (MyBusinessClass) container[typeof(MyBusinessClass)];
			
			try
			{
				Blog blog = service.CreateWithError("myblog");
			}
			catch(Exception)
			{
				// Expected
			}

			BlogDao dao = (BlogDao) container["blogdao"];

			IList blogs = dao.ObtainBlogs();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Count );
		}
	}
}
