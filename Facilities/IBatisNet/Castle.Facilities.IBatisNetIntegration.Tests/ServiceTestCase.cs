#region License

/// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
///  
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///  
/// http://www.apache.org/licenses/LICENSE-2.0
///  
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// -- 
/// 
/// This facility was a contribution kindly 
/// donated by Gilles Bayon <gilles.bayon@gmail.com>
/// 
/// --

#endregion

namespace Castle.Facilities.IBatisNetIntegration.Tests
{
	using System;
	using Castle.Facilities.AutomaticTransactionManagement;
	using Castle.Facilities.IBatisNetIntegration.Tests.Dao;
	using Castle.Facilities.IBatisNetIntegration.Tests.Domain;
	using NUnit.Framework;

	[TestFixture]
	public class ServiceTestCase : AbstractIBatisNetTestCase
	{
		[Test]
		public void CommonUsage()
		{
			container = CreateConfiguredContainer();
			container.AddFacility("IBatisNet", new IBatisNetFacility());

			container.AddComponent("AccountDao", typeof (IAccountDao), typeof (AccountDao));
			container.AddComponent("Service", typeof (IService), typeof (Service));

			ResetDatabase();
			IService service = container["Service"] as IService;

			Account account = service.GetAccount(5);

			AssertAccount5(account);
		}

		[Test]
		public void TransactionalUsage()
		{
			container = CreateConfiguredContainer();
			container.AddFacility("IBatisNet", new IBatisNetFacility());
			container.AddFacility("transaction", new TransactionFacility());

			container.AddComponent("AccountDao", typeof (IAccountDao), typeof (AccountDao));
			container.AddComponent("Service", typeof (IService), typeof (Service));

			ResetDatabase();
			IService service = container["Service"] as IService;

			Account account = new Account();
			account.Id = 999;
			account.EmailAddress = "ibatis@somewhere.com";
			account.FirstName = "Gilles";
			account.LastName = "Bayon";

			service.InsertTransactionalAccount(account);
			account = null;
			account = service.GetAccountWithSpecificDataMapper(999) as Account;

			Assert.IsNotNull(account);
			Assert.AreEqual(999, account.Id, "account.Id");
		}


		[Test]
		public void NoSessionOpen()
		{
			container = CreateConfiguredContainer();
			container.AddFacility("IBatisNet", new IBatisNetFacility());
			container.AddFacility("transaction", new TransactionFacility());

			container.AddComponent("AccountDao", typeof (IAccountDao), typeof (AccountDao));
			container.AddComponent("Service", typeof (IService), typeof (Service));

			IService service = container["Service"] as IService;
			service.DummyNoSession();
		}

		[Test]
		public void Rollback()
		{
			container = CreateConfiguredContainer();
			container.AddFacility("IBatisNet", new IBatisNetFacility());
			container.AddFacility("transaction", new TransactionFacility());

			container.AddComponent("AccountDao", typeof (IAccountDao), typeof (AccountDao));
			container.AddComponent("Service", typeof (IService), typeof (Service));

			ResetDatabase();
			IService service = container["Service"] as IService;

			Account account = new Account();
			account.Id = 999;
			account.EmailAddress = "ibatis@somewhere.com";
			account.FirstName = "Transaction";
			account.LastName = "Error";

			try
			{
				service.InsertTransactionalAccountWithError(account);
				Assert.Fail("Exception was expected");
			}
			catch (Exception)
			{
				// Expected
			}

			account = null;
			account = service.GetAccountWithSpecificDataMapper(999) as Account;

			Assert.IsNull(account);
		}
	}
}