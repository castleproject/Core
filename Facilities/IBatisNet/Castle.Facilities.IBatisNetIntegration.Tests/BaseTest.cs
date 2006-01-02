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
	using Castle.Windsor;

	using Castle.Facilities.IBatisNetIntegration.Tests.Domain;
	using Castle.Facilities.IBatisNetIntegration.Tests.Dao;

	using NUnit.Framework;

	public class BaseTest
	{
		protected IWindsorContainer container = null;

		/// <summary>
		/// SetUp
		/// </summary>
		[SetUp] 
		public void Init() 
		{
			container = null;
		}


		public void ResetDatabase() 
		{
			IAccountDao dao = container["AccountDao"] as AccountDao;

			dao.ResetTableAccount();
		}

		/// <summary>
		/// Verify that the input account is equal to the account(id=5).
		/// </summary>
		/// <param name="account">An account object</param>
		protected void AssertAccount1(Account account) 
		{
			Assert.AreEqual(1, account.Id, "account.Id");
			Assert.AreEqual("Joe", account.FirstName, "account.FirstName");
			Assert.AreEqual("Dalton", account.LastName, "account.LastName");
			Assert.AreEqual("Joe.Dalton@somewhere.com", account.EmailAddress, "account.EmailAddress");
		}

		/// <summary>
		/// Verify that the input account is equal to the account(id=1).
		/// </summary>
		/// <param name="account">An account object</param>
		protected void AssertAccount5(Account account) 
		{
			Assert.AreEqual(5, account.Id, "account.Id");
			Assert.AreEqual("Gilles", account.FirstName, "account.FirstName");
			Assert.AreEqual("Bayon", account.LastName, "account.LastName");
			Assert.AreEqual("no_email@provided.com", account.EmailAddress, "account.EmailAddress");
		}
	}
}
