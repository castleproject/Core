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

namespace BlogSample
{
	using System.Windows.Forms;

	using BlogSample.UI;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Config;

	public class App
	{
		public static void Main()
		{
			// 1. Step Configure and Initialize ActiveRecord:
			
			// If you want to use the InPlaceConfigurationSource:
			// Hashtable properties = new Hashtable();
			// properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
			// properties.Add("hibernate.dialect", "NHibernate.Dialect.MsSql2000Dialect");
			// properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			// properties.Add("hibernate.connection.connection_string", "Data Source=.;Initial Catalog=test;Integrated Security=SSPI");
			// InPlaceConfigurationSource source = new InPlaceConfigurationSource();
			// source.Add(typeof(ActiveRecordBase), properties);

			// We are using XmlConfigurationSource:
			XmlConfigurationSource source = new XmlConfigurationSource("../appconfig.xml");

			ActiveRecordStarter.Initialize( source, typeof(Blog), typeof(Post), typeof(User) );

			// 2. Create the schema 
			
			// If you want to let AR to create the schema
			if (MessageBox.Show("Do you want to let ActiveRecord create the database tables?", "Schema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				ActiveRecordStarter.CreateSchema();
			}

			// 3. Create the first user (so you can log in)
			
			if (User.GetUsersCount() == 0)
			{
				User user = new User("admin", "123");
			
				user.Create();
			}
			
			// 4. Bring the Login Form
			
			using(LoginForm login = new LoginForm())
			{
				if (login.ShowDialog() != DialogResult.OK)
				{
					return;
				}
			}
			
			// 5. Show the main form
			
			using(BlogManagement mainForm = new BlogManagement())
			{
				Application.Run(mainForm);
			}
		}
	}
}
