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

namespace Castle.ActiveRecord.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model;


	[TestFixture]
	public class TableHierarchyTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void CompanyFirmAndClient()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), 
				typeof(Company), typeof(Client), typeof(Firm), typeof(Person) );

			Company.DeleteAll();
			Client.DeleteAll();
			Firm.DeleteAll();
			Person.DeleteAll();

			Firm firm = new Firm("keldor");
			firm.Save();

			Client client = new Client("castle", firm);
			client.Save();

			Firm[] firms = Firm.FindAll();
			Assert.AreEqual( 1, firms.Length );

			Client[] clients = Client.FindAll();
			Assert.AreEqual( 1, clients.Length );
			
			Assert.AreEqual( firm.Id, firms[0].Id );
			Assert.AreEqual( client.Id, clients[0].Id );

			Assert.IsNotNull( clients[0].Firm );
			Assert.AreEqual( firm.Id, clients[0].Firm.Id );
		}

		[Test]
		public void ManyToMany()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), 
				typeof(Company), typeof(Client), typeof(Firm), typeof(Person) );

			Company.DeleteAll();
			Client.DeleteAll();
			Firm.DeleteAll();
			Person.DeleteAll();

			Firm firm = new Firm("keldor");
			Client client = new Client("castle", firm);
			Company company = new Company("vs");

			using(new SessionScope())
			{
				firm.Save();
				client.Save();
				company.Save();

				Person person = new Person();
				person.Name = "hammett";

				person.Companies.Add( firm );
				person.Companies.Add( client );
				person.Companies.Add( company );
				person.Save();
			}

			company = Company.Find( company.Id );
			Assert.AreEqual(1, company.People.Count );
		}
	}
}
