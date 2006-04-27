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

namespace Castle.Facilities.AutomaticTransactionManagement.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.MicroKernel.SubSystems.Configuration;
	
	using Castle.Windsor;

	using Castle.Services.Transaction;

	/// <summary>
	/// Summary description for TransactionTestCase.
	/// </summary>
	[TestFixture]
	public class FacilityBasicTestCase
	{
		[Test]
		public void TestReportedBug()
		{
			WindsorContainer container = new WindsorContainer( new DefaultConfigurationStore() );

			container.AddFacility( "transactionmanagement", new TransactionFacility() );

			container.AddComponent( "transactionmanager", 
				typeof(ITransactionManager), typeof(MockTransactionManager) );

			container.AddComponent( "comp", typeof(SubTransactionalComp) );

			SubTransactionalComp service = (SubTransactionalComp) container["comp"];

			service.BaseMethod();

			MockTransactionManager transactionManager = (MockTransactionManager) 
				container["transactionmanager"];

			Assert.AreEqual(1, transactionManager.TransactionCount);
			Assert.AreEqual(1, transactionManager.CommittedCount);
			Assert.AreEqual(0, transactionManager.RolledBackCount);
		}

		[Test]
		public void TestBasicOperations()
		{
			WindsorContainer container = new WindsorContainer( new DefaultConfigurationStore() );

			container.AddFacility( "transactionmanagement", new TransactionFacility() );

			container.AddComponent( "transactionmanager", 
				typeof(ITransactionManager), typeof(MockTransactionManager) );

			container.AddComponent( "services.customer", typeof(CustomerService) );

			CustomerService service = (CustomerService) container["services.customer"];

			service.Insert( "TestCustomer", "Rua P Leite, 33" );

			MockTransactionManager transactionManager = (MockTransactionManager) 
				container["transactionmanager"];

			Assert.AreEqual(1, transactionManager.TransactionCount);
			Assert.AreEqual(1, transactionManager.CommittedCount);
			Assert.AreEqual(0, transactionManager.RolledBackCount);

			try
			{
				service.Delete( 1 );
			}
			catch(Exception)
			{
				// Expected
			}

			Assert.AreEqual(2, transactionManager.TransactionCount);
			Assert.AreEqual(1, transactionManager.CommittedCount);
			Assert.AreEqual(1, transactionManager.RolledBackCount);
		}

		[Test]
		public void TestBasicOperationsWithInterfaceService()
		{
			WindsorContainer container = new WindsorContainer( new DefaultConfigurationStore() );

			container.AddFacility( "transactionmanagement", new TransactionFacility() );

			container.AddComponent( "transactionmanager", 
				typeof(ITransactionManager), typeof(MockTransactionManager) );

			container.AddComponent( "services.customer", typeof(ICustomerService), typeof(AnotherCustomerService) );

			ICustomerService service = (ICustomerService) container["services.customer"];

			service.Insert( "TestCustomer", "Rua P Leite, 33" );

			MockTransactionManager transactionManager = (MockTransactionManager) 
				container["transactionmanager"];

			Assert.AreEqual(1, transactionManager.TransactionCount);
			Assert.AreEqual(1, transactionManager.CommittedCount);
			Assert.AreEqual(0, transactionManager.RolledBackCount);

			try
			{
				service.Delete( 1 );
			}
			catch(Exception)
			{
				// Expected
			}

			Assert.AreEqual(2, transactionManager.TransactionCount);
			Assert.AreEqual(1, transactionManager.CommittedCount);
			Assert.AreEqual(1, transactionManager.RolledBackCount);
		}
	}
}
