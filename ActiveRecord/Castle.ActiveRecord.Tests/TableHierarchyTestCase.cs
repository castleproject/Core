<<<<<<< .mine
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
	using System.Collections;
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model;
	using Iesi.Collections;

	[TestFixture]
	public class TableHierarchyTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void CompanyFirmAndClient()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), 
				typeof(Company), typeof(Client), typeof(Firm), typeof(Person) );

			Client.DeleteAll();
			Firm.DeleteAll();
			Company.DeleteAll();
			Person.DeleteAll();

			Firm firm = new Firm("keldor");
			firm.Save();

			Client client = new Client("castle", firm);
			client.Save();

			Client[] clients = Client.FindAll();
			Assert.AreEqual( 1, clients.Length );

			Firm[] firms = Firm.FindAll();
			Assert.AreEqual( 1, firms.Length );

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
		
		[Test]
		public void ManyToManyUsingSet()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), 
				typeof(Order), typeof(LineItem), typeof(Product));
			
			Order.DeleteAll();
			Product.DeleteAll();
			
			Order myOrder = new Order();
			myOrder.OrderedDate = DateTime.Parse("05/09/2004");
			Product coolGadget = new Product();
			coolGadget.Name = "PSP";
			coolGadget.Price = 250.39f;
			
			using (new SessionScope())
			{
				coolGadget.Save();
				ISet products = new ListSet();
				products.Add(coolGadget);
				myOrder.Products = products;
				myOrder.Save();
			}
			
			Order secondRef2Order = Order.Find(myOrder.ID);
			Assert.IsFalse(secondRef2Order.Products.IsEmpty);
			
			Product secondRef2Product = Product.Find(coolGadget.ID);
			Assert.AreEqual(1, secondRef2Product.Orders.Count);
			
		}

		[Test]
		public void ManyToManyUsingIDBag()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), 
				typeof(OrderWithIDBag), typeof(ProductWithIDBag));
			
			OrderWithIDBag.DeleteAll();
			ProductWithIDBag.DeleteAll();
			
			OrderWithIDBag myOrder = new OrderWithIDBag();
			myOrder.OrderedDate = new DateTime(2006, 12, 25);
			ProductWithIDBag coolGadget = new ProductWithIDBag();
			coolGadget.Name = "Xbox 2";
			coolGadget.Price = 330.23f;
			
			using (new SessionScope())
			{
				coolGadget.Save();
				IList products = new ArrayList();
				products.Add(coolGadget);
				myOrder.Products = products;
				myOrder.Save();
			}
			
			OrderWithIDBag secondRef2Order = OrderWithIDBag.Find(myOrder.ID);
			Assert.IsTrue(secondRef2Order.Products.Count > 0);
			
			ProductWithIDBag secondRef2Product = ProductWithIDBag.Find(coolGadget.ID);
			Assert.AreEqual(1, secondRef2Product.Orders.Count);
		}
	}
}
=======
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
	using System.Collections;
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model;
	using Iesi.Collections;

	[TestFixture]
	public class TableHierarchyTestCase : AbstractActiveRecordTest
	{
		[Test]
		// [Ignore("NHibernate will fix the problem soon")]
		public void CompanyFirmAndClient()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), 
				typeof(Company), typeof(Client), typeof(Firm), typeof(Person) );

			Client.DeleteAll();
			Firm.DeleteAll();
			Company.DeleteAll();
			Person.DeleteAll();

			Firm firm = new Firm("keldor");
			firm.Save();

			Client client = new Client("castle", firm);
			client.Save();

			Client[] clients = Client.FindAll();
			Assert.AreEqual( 1, clients.Length );

			Firm[] firms = Firm.FindAll();
			Assert.AreEqual( 1, firms.Length );

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
		
		[Test]
		public void ManyToManyUsingSet()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), 
				typeof(Order), typeof(LineItem), typeof(Product));
			
			Order.DeleteAll();
			Product.DeleteAll();
			
			Order myOrder = new Order();
			myOrder.OrderedDate = DateTime.Parse("05/09/2004");
			Product coolGadget = new Product();
			coolGadget.Name = "PSP";
			coolGadget.Price = 250.39f;
			
			using (new SessionScope())
			{
				coolGadget.Save();
				ISet products = new ListSet();
				products.Add(coolGadget);
				myOrder.Products = products;
				myOrder.Save();
			}
			
			Order secondRef2Order = Order.Find(myOrder.ID);
			Assert.IsFalse(secondRef2Order.Products.IsEmpty);
			
			Product secondRef2Product = Product.Find(coolGadget.ID);
			Assert.AreEqual(1, secondRef2Product.Orders.Count);
			
		}

		[Test]
		public void ManyToManyUsingIDBag()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), 
				typeof(OrderWithIDBag), typeof(ProductWithIDBag));
			
			OrderWithIDBag.DeleteAll();
			ProductWithIDBag.DeleteAll();
			
			OrderWithIDBag myOrder = new OrderWithIDBag();
			myOrder.OrderedDate = new DateTime(2006, 12, 25);
			ProductWithIDBag coolGadget = new ProductWithIDBag();
			coolGadget.Name = "Xbox 2";
			coolGadget.Price = 330.23f;
			
			using (new SessionScope())
			{
				coolGadget.Save();
				IList products = new ArrayList();
				products.Add(coolGadget);
				myOrder.Products = products;
				myOrder.Save();
			}
			
			OrderWithIDBag secondRef2Order = OrderWithIDBag.Find(myOrder.ID);
			Assert.IsTrue(secondRef2Order.Products.Count > 0);
			
			ProductWithIDBag secondRef2Product = ProductWithIDBag.Find(coolGadget.ID);
			Assert.AreEqual(1, secondRef2Product.Orders.Count);
		}
	}
}
>>>>>>> .r637
