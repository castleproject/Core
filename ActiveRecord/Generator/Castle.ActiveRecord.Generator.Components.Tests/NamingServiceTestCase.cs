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

namespace Castle.ActiveRecord.Generator.Components.Tests
{
	using System;

	using NUnit.Framework;



	[TestFixture]
	public class NamingServiceTestCase : AbstractContainerTestCase
	{
		[SetUp]
		public void SetUpServices()
		{
			base.Init();
			Kernel.AddComponent("namingservice", typeof(INamingService), typeof(NamingService));
		}

		[Test]
		public void CreateClassName()
		{
			INamingService service = (INamingService) Kernel[ typeof(INamingService) ];

			Assert.AreEqual( "Author", service.CreateClassName("authors") );
			Assert.AreEqual( "Tax", service.CreateClassName("Taxes") );
			Assert.AreEqual( "Child", service.CreateClassName("children") );
			Assert.AreEqual( "Order", service.CreateClassName("Orders") );
			Assert.AreEqual( "Order", service.CreateClassName("Order") );
			Assert.AreEqual( "Order", service.CreateClassName("_Order") );
			Assert.AreEqual( "Order", service.CreateClassName("tb_Order") );
			Assert.AreEqual( "Order", service.CreateClassName("tb_Orders") );
		}

		[Test]
		public void CreateFieldName()
		{
			INamingService service = (INamingService) Kernel[ typeof(INamingService) ];

			Assert.AreEqual( "_orderName", service.CreateFieldName("order_name") );
			Assert.AreEqual( "_name", service.CreateFieldName("name") );
			Assert.AreEqual( "_customerName", service.CreateFieldName("customerName") );
			Assert.AreEqual( "_customerName", service.CreateFieldName("customer_name") );
			Assert.AreEqual( "_customerName", service.CreateFieldName("customer_Name") );
			Assert.AreEqual( "_customerName", service.CreateFieldName("CustomerName") );
			Assert.AreEqual( "_name", service.CreateFieldName("_name") );
		}

		[Test]
		public void CreatePropertyName()
		{
			INamingService service = (INamingService) Kernel[ typeof(INamingService) ];

			Assert.AreEqual( "OrderName", service.CreatePropertyName("order_name") );
			Assert.AreEqual( "Name", service.CreatePropertyName("name") );
			Assert.AreEqual( "CustomerName", service.CreatePropertyName("customerName") );
			Assert.AreEqual( "CustomerName", service.CreatePropertyName("customer_Name") );
			Assert.AreEqual( "CustomerName", service.CreatePropertyName("Customer_name") );
			Assert.AreEqual( "Name", service.CreatePropertyName("_name") );
		}

		[Test]
		public void CreateRelationName()
		{
			INamingService service = (INamingService) Kernel[ typeof(INamingService) ];

			Assert.AreEqual( "Posts", service.CreateRelationName("Post") );
			Assert.AreEqual( "Children", service.CreateRelationName("child") );
			Assert.AreEqual( "Posts", service.CreateRelationName("posts") );
			Assert.AreEqual( "Posts", service.CreateRelationName("post") );
			Assert.AreEqual( "Orders", service.CreateRelationName("order") );
			Assert.AreEqual( "Facilities", service.CreateRelationName("facility") );
		}
	}
}
