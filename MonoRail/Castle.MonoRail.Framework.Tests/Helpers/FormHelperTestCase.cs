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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System;
	using System.Globalization;
	using System.Threading;

	using Castle.MonoRail.Framework.Helpers;
	
	using NUnit.Framework;

	[TestFixture]
	public class FormHelperTestCase
	{
		private FormHelper helper;

		[TestFixtureSetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture( "en" );

			Thread.CurrentThread.CurrentCulture	= en;
			Thread.CurrentThread.CurrentUICulture = en;

			helper = new FormHelper();
		}

		[Test]
		public void TextField()
		{
			Product product = new Product("memory card", 10, (decimal) 12.30);

			Assert.AreEqual("<input type=\"text\" id=\"Product_name\" name=\"Product.name\" value=\"memory card\" />", 
				helper.TextField(product, "name"));
			Assert.AreEqual("<input type=\"text\" id=\"Product_quantity\" name=\"Product.quantity\" value=\"10\" />", 
				helper.TextField(product, "quantity"));
		}

		[Test]
		public void TextFieldValue()
		{
			Product product = new Product("memory card", 10, (decimal) 12.30);

			Assert.AreEqual("<input type=\"text\" id=\"Product_price\" name=\"Product.price\" value=\"$12.30\" />", 
				helper.TextFieldValue(product, "price", product.Price.ToString("C")));
		}
	}

	public class Product
	{
		private String name;
		private int quantity;
		private Decimal price;

		public Product(string name, int quantity, decimal price)
		{
			this.name = name;
			this.quantity = quantity;
			this.price = price;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public int Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}

		public decimal Price
		{
			get { return price; }
			set { price = value; }
		}
	}
}
