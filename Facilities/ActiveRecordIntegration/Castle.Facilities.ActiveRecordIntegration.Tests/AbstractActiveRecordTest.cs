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

namespace Castle.Facilities.ActiveRecordIntegration.Tests
{
	using System;

	using Castle.Windsor;
	
	using Castle.ActiveRecord;

	using NUnit.Framework;

	using Castle.Facilities.ActiveRecordIntegration.Tests.Model;
	using Castle.Facilities.AutomaticTransactionManagement;


	public class AbstractActiveRecordTest
	{
		protected IWindsorContainer container;

		[TestFixtureSetUp]
		public void Init()
		{
			container = new WindsorContainer("configuration.xml");

			container.AddFacility("transactionfacility", new TransactionFacility() );
			container.AddFacility("arfacility", new ActiveRecordFacility() );

			container.AddComponent( "blog.service", typeof(BlogService) );
			container.AddComponent( "post.service", typeof(PostService) );
			container.AddComponent( "first.service", typeof(FirstService) );
			container.AddComponent( "wiring.service", typeof(WiringSession) );

			Recreate();
		}

		[TestFixtureTearDown]
		public void Terminate()
		{
			try
			{
				ActiveRecordStarter.DropSchema();
			}
			catch(Exception)
			{
				
			}

			container.Dispose();
		}

		protected void Recreate()
		{
			ActiveRecordStarter.CreateSchema();
		}
	}
}
