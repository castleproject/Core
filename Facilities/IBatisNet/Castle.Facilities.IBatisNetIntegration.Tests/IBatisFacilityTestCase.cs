#region License
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
	using NUnit.Framework;
	
	using Castle.Facilities.IBatisNetIntegration.Tests.Domain;
	using Castle.Windsor;

	using IBatisNet.Common;
	using IBatisNet.DataMapper;

	[TestFixture]
	public class IBatisNetFacilityTestCase : AbstractIBatisNetTestCase
	{
		[Test]
		public void Usage()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("IBatisNet", new IBatisNetFacility());

			SqlMapper sqlMap = container[AbstractIBatisNetTestCase.DATA_MAPPER] as SqlMapper;

			using (IDalSession session = sqlMap.OpenConnection())
			{
				Account account = new Account();
				account.Id = 99;
				account.EmailAddress = "ibatis@somewhere.com";
				account.FirstName = "Gilles";
				account.LastName = "Bayon";

				sqlMap.Insert("InsertAccount", account);
				account = null;
				account = sqlMap.QueryForObject("GetAccount",99) as Account;

				Assert.AreEqual(99, account.Id, "account.Id");
			}

		}

	}
}
