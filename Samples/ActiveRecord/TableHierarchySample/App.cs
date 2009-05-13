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

namespace TableHierarchySample
{
	using System.Diagnostics;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Config;

	public class App
	{
		public static void Main()
		{
			ActiveRecordStarter.Initialize( 
				new XmlConfigurationSource("../appconfig.xml"), 
				typeof(Company), typeof(Client), typeof(Firm), typeof(Person) );

			// If you want to let AR to create the schema

			ActiveRecordStarter.CreateSchema();

			// Common usage

			Client.DeleteAll();
			Firm.DeleteAll();
			Company.DeleteAll();

			using(new SessionScope())
			{
				Company company = new Company("Stronghold");
				company.Create();

				Firm firm = new Firm("Johnson & Norman");
				firm.Create();

				Client client = new Client("hammett", firm);
				client.Create();
			}

			// Now let's load

			using(new SessionScope())
			{
				Company[] companies = Company.FindAll();
				Debug.Assert(companies.Length == 3);

				Firm[] firms = Firm.FindAll();
				Debug.Assert(firms.Length == 1);

				Client[] clients = Client.FindAll();
				Debug.Assert(clients.Length == 1);

			}

			// Drop the schema if you want
			// ActiveRecordStarter.DropSchema();
		}
	}
}
