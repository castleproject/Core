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
	using System.IO;

	using com.db4o;

	using NUnit.Framework;

	using Castle.Windsor;
	using Castle.Facilities.AutomaticTransactionManagement;
	using Castle.Facilities.Db4oIntegration.Tests.Components;
	
	public class AbstractDb4oTestCase
	{
		private WindsorContainer _container;

		public AbstractDb4oTestCase()
		{
		}

		public WindsorContainer Container
		{
			get
			{
				return _container;
			}
			set
			{
				_container = value;
			}
		}

		[SetUp]
		public virtual void Init()
		{
			_container = new WindsorContainer("../Castle.Facilities.Db4oIntegration.Tests/Castle.Facilities.Db4oIntegration.Tests.config");	

			_container.AddFacility("transaction", new TransactionFacility());

			_container.AddComponent("beer.dao", typeof(BeerDao), typeof(BeerDao));
			_container.AddComponent("beer.dao.transactional", typeof(BeerTransactionalDao), typeof(BeerTransactionalDao));
			_container.AddComponent("beer.box", typeof(BeerBox), typeof(BeerBox));
		}

		[TearDown]
		public void Clean()
		{
			_container.Dispose();

			File.Delete("dump.yap");
		}
	}
}
