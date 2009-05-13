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

namespace Castle.Facilities.Db4oIntegration.Tests
{
	using System;
	using System.Threading;
	using Db4objects.Db4o;
	using NUnit.Framework;

	using Castle.Facilities.Db4oIntegration.Tests.Components;


	[TestFixture]
	public class TransactionalTestCase : AbstractDb4oTestCase
	{
		private BeerBox _box;

		private IObjectContainer _objContainer;

		[SetUp]
		public override void Init()
		{
			base.Init();

			_box = (BeerBox) Container["beer.box"];

			_objContainer = (IObjectContainer)Container[typeof(IObjectContainer)];
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
	}
}
