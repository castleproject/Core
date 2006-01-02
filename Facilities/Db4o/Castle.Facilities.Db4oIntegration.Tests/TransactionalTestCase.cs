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

namespace Castle.Facilities.Db4oIntegration.Tests
{
	using System;
	using System.Threading;

	using NUnit.Framework;

	using com.db4o;

	using Castle.Facilities.Db4oIntegration.Tests.Components;


	[TestFixture]
	public class TransactionalTestCase : AbstractDb4oTestCase
	{
		private BeerBox _box;

		private const int InvocationsNumber = 20;
		private const int ThreadsNumber = 10;

		private ObjectContainer _objContainer;

		public TransactionalTestCase()
		{
		}

		[SetUp]
		public override void Init()
		{
			base.Init();

			_box = (BeerBox) Container["beer.box"];

			_objContainer = (ObjectContainer)Container[typeof(ObjectContainer)];
		}

		[Test]
		public void EnsureCommit()
		{
			Beer beer = AddToBox();

			DoRollback();

			Assert.AreEqual(beer.Id, _box.Load(beer.Id).Id);
		}

		[Test]
		public void EnsureRollback()
		{
			Beer beer = new Beer(Guid.NewGuid());

			try
			{
				_box.AddAndBroke(beer);
			}
			catch (ApplicationException)
			{
			}

			Assert.IsNull(_box.Load(beer.Id));
		}

		[Test]
		public void SimpleUsage()
		{
			Beer beer = AddToBox();

			Assert.AreEqual(beer.Id, _box.Load(beer.Id).Id);
		}

		private Beer AddToBox()
		{
			Beer beer = new Beer(Guid.NewGuid());
	
			_box.AddBeer(beer);

			return beer;
		}

		private void DoRollback()
		{
			_objContainer.Rollback();
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

			Assert.AreEqual(InvocationsNumber * ThreadsNumber, _box.GetAll().Size());
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
				AddToBox();
			}
		}
	}
}
