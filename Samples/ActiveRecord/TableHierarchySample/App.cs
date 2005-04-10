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

namespace TableHierarchySample
{
	using System;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Config;


	public class App
	{
		public App()
		{
		}

		public static void Main()
		{
			ActiveRecordStarter.Initialize( 
				new XmlConfigurationSource("../appconfig.xml"), 
				typeof(Company), typeof(Client), typeof(Firm), typeof(Person) );

			// Common usage

			Client.DeleteAll();
			Firm.DeleteAll();
			Company.DeleteAll();

			Company company = new Company("Keldor");
			company.Save();

			Firm firm = new Firm("Johnson & Norman");
			firm.Save();

			Client client = new Client("hammett", firm);
			client.Save();
		}
	}
}
