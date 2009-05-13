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

namespace MoreComplexSample
{
	using System;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Config;

	public class App
	{
		public static void Main()
		{
			// Old way
//			Hashtable properties = new Hashtable();
//
//			properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
//			properties.Add("hibernate.dialect", "NHibernate.Dialect.MsSql2000Dialect");
//			properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
//			properties.Add("hibernate.connection.connection_string",
//			               "Data Source=.;Initial Catalog=bookdb;Integrated Security=SSPI");
//
//			InPlaceConfigurationSource source = new InPlaceConfigurationSource();
//			source.Add(typeof(ActiveRecordBase), properties);

			// New way
			InPlaceConfigurationSource config = InPlaceConfigurationSource.BuildForMSSqlServer(".", "test");

			ActiveRecordStarter.Initialize(config,
										   typeof(LineItem), typeof(Order),
										   typeof(Category), typeof(Product),
										   typeof(Customer));

			// Framework started, let's create the schema

			ActiveRecordStarter.CreateSchema();

			// Now let's play

			Customer invalid = new Customer();
			invalid.Name = "john"; // Less than the minimum
			invalid.Email = "someinvalidemail.com";
			
			if (!invalid.IsValid())
			{
				foreach(String msg in invalid.ValidationErrorMessages)
				{
					Console.WriteLine(msg);
				}
			}
			
			try
			{
				// This will fail
				invalid.Create();
			}
			catch(Exception ex)
			{
			}

			Order order;
			Product product;

			using(new SessionScope())
			{
				Category root = new Category("Petshot");
				root.Create();

				Category c1 = new Category("Dogs");
				c1.Parent = root;
				c1.Create();

				product = new Product();
				product.Name = "It";
				product.Price = 10f;
				product.Categories.Add(c1);
				product.Create();

				Customer customer = new Customer();
				customer.Name = "another customer";
				customer.Email = "foo@bar.com";
				customer.Create();

				order = new Order();
				order.Customer = customer;
				order.Total = 100f;
				order.Create();

				// Associate order and product (the hard way)
				LineItem item = new LineItem(order, product);
				item.Quantity = 10;
				item.Create();
			}

			using(new SessionScope())
			{
				// Change just the association
				LineItem item = LineItem.Find(order, product);
				item.Quantity = 12;
			}
		}
	}
}
