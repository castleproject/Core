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

namespace Castle.Facilities.Db4oIntegration.Tests
{
	using System;
	using System.Threading;
	
	using NUnit.Framework;

	using Castle.Facilities.Db4oIntegration.Tests.Components;
	
	[TestFixture]
	public class DaoTestCase : AbstractDb4oTestCase
	{
		private BeerDao _dao;

		int InvocationsNumber = 20;
		int ThreadsNumber = 10;

		public DaoTestCase()
		{
		}

		[SetUp]
		public override void Init()
		{
			base.Init();

			_dao = (BeerDao) Container["beer.dao"];
		}

		[Test]
		public void CommonUsage()
		{
			Beer beer = new Beer(Guid.NewGuid());

			_dao.Create(beer);

			Assert.AreEqual(beer.Id, _dao.Load(beer.Id).Id);
		}

		[Test]
		public void MultiThreaded()
		{
			Thread[] threads = new Thread[ThreadsNumber];

			for (int i = 0; i < threads.Length; i++)
			{
				threads[i] = new Thread(new ThreadStart(BeerFactory));
			}

			Start(threads);
			Join(threads);

			Assert.AreEqual(InvocationsNumber * ThreadsNumber, _dao.FindAll().size());
		}

		private void Join(Thread[] threads)
		{
			foreach (Thread t in threads)
			{
				t.Join();
			}
		}

		private void Start(Thread[] threads)
		{
			foreach (Thread t in threads)
			{
				t.Start();
			}
		}

		public void BeerFactory()
		{
			for (int i = 0; i < InvocationsNumber; i++)
			{
				CommonUsage();
			}
		}
	}
}
