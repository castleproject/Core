#region Licence
/// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	using Castle.Services.Transaction;

	using Castle.Facilities.IBatisNetIntegration.Tests.Dao;
	using Castle.Facilities.IBatisNetIntegration.Tests.Domain;

	/// <summary>
	/// Description résumée de Service.
	/// </summary>
	/// <remarks>
	/// If no sql map key is defined by
	/// [Session("sqlMap.key")]
	/// on a method definition
	/// The first regsitration will be used
	/// </remarks>
	[Transactional]
	[UsesAutomaticSessionCreation]
	public class Service : IService
	{
		private IAccountDao _accountDao;

		public Service(IAccountDao accountDao)
		{
			_accountDao = accountDao;
		}

		#region IService Members

		public Account GetAccount(int id)
		{
			return _accountDao.GetAccount(id);
		}

		// Specify wich Data Mapper to use
		[Session(AbstractIBatisNetTestCase.DATA_MAPPER)]
		[Transaction(TransactionMode.Requires)]
		public Account GetAccountWithSpecificDataMapper(int id)
		{
			return _accountDao.GetAccount(id);
		}

		public void InsertAccount(Account account)
		{
			_accountDao.InsertAccount(account);
		}

		[Transaction(TransactionMode.Requires)]
		public void InsertTransactionalAccount(Account account)
		{
			_accountDao.InsertAccount(account);
		}

		public void ResetTableAccount()
		{
			_accountDao.ResetTableAccount();
		}

		[NoSession]
		public void DummyNoSession()
		{
			Console.WriteLine("No session will be open since we're using the NoSession attribute");
		}

		[Transaction(TransactionMode.Requires)]
		public void InsertTransactionalAccountWithError(Account account)
		{
			_accountDao.InsertAccount(account);

			throw new ApplicationException("Ugh!");
		}

		#endregion
	}
}
