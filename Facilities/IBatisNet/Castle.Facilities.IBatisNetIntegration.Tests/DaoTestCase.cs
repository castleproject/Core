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
	using System.Threading;

	using NUnit.Framework;

	using Castle.Facilities.AutomaticTransactionManagement;
	using Castle.Facilities.IBatisNetIntegration.Tests.Dao;
	using Castle.Facilities.IBatisNetIntegration.Tests.Domain;

	[TestFixture]
	public class DaoTestCase : AbstractIBatisNetTestCase
	{
		private ManualResetEvent  _startEvent = new ManualResetEvent(false);
		private ManualResetEvent  _stopEvent = new ManualResetEvent(false);
		private IAccountDao _dao = null;

		[Test]
		public void CommonUsage()
		{
			container = CreateConfiguredContainer();
			container.AddFacility("IBatisNet", new IBatisNetFacility());

			container.AddComponent("AccountDao", typeof(IAccountDao), typeof(AccountDao));
			
			ResetDatabase();

			IAccountDao dao = container["AccountDao"] as AccountDao;
			Account account = new Account();
			account.Id = 99;
			account.EmailAddress = "ibatis@somewhere.com";
			account.FirstName = "Gilles";
			account.LastName = "Bayon";

			dao.InsertAccount(account);
			account = null;
			account = dao.GetAccount(99) as Account;

			Assert.IsNotNull( account );
			Assert.AreEqual(99, account.Id, "account.Id");
		}

		[Test]
		public void CommonUsageMultithread()
		{
			container = CreateConfiguredContainer();
			container.AddFacility("IBatisNet", new IBatisNetFacility());

			container.AddComponent("AccountDao", typeof(IAccountDao), typeof(AccountDao));
			
			ResetDatabase();

			_dao = container["AccountDao"] as AccountDao;
			
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
			System.Random _random = new Random();
			_startEvent.WaitOne(int.MaxValue, false);

			while (!_stopEvent.WaitOne(1, false))
			{
				int id = _random.Next();
				Account account = new Account();
				account.Id = id;
				account.EmailAddress = "ibatis@somewhere.com";
				account.FirstName = "Gilles";
				account.LastName = "Bayon";

				_dao.InsertAccount(account);
				account = null;
				account = _dao.GetAccount(id);
				Assert.IsNotNull( account );
			}
		}

		[Test]
		public void TransactionalUsageMultithread()
		{
			container = CreateConfiguredContainer();
			container.AddFacility("IBatisNet", new IBatisNetFacility());
			container.AddFacility("transaction", new TransactionFacility());

			container.AddComponent("AccountDao", typeof(IAccountDao), typeof(AccountDao));
			
			_dao = container["AccountDao"] as AccountDao;
			
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
