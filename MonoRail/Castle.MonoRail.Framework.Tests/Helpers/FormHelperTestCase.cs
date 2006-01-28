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
	using System.Collections;
	using System.Globalization;
	using System.Threading;

	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Tests.Controllers;
	
	using NUnit.Framework;

	[TestFixture]
	public class FormHelperTestCase
	{
		private FormHelper helper;
		private Product product;
		private SimpleUser user;

		[SetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture("en");

			Thread.CurrentThread.CurrentCulture	= en;
			Thread.CurrentThread.CurrentUICulture = en;

			helper = new FormHelper();

			product = new Product("memory card", 10, (decimal) 12.30);
			user = new SimpleUser();

			HomeController controller = new HomeController();

			controller.PropertyBag.Add("product", product);
			controller.PropertyBag.Add("user", user);
			controller.PropertyBag.Add("sendemail", true);
			controller.PropertyBag.Add("confirmation", "abc");

			helper.SetController(controller);
		}

		[Test]
		public void TextField()
		{
			Assert.AreEqual("<input type=\"text\" id=\"product_name\" name=\"product.name\" value=\"memory card\" />", 
				helper.TextField("product.name"));
			Assert.AreEqual("<input type=\"text\" id=\"product_quantity\" name=\"product.quantity\" value=\"10\" />", 
				helper.TextField("product.quantity"));
		}

		[Test]
		public void PasswordField()
		{
			Assert.AreEqual("<input type=\"password\" id=\"product_name\" name=\"product.name\" value=\"memory card\" />", 
				helper.PasswordField("product.name"));
			Assert.AreEqual("<input type=\"password\" id=\"product_quantity\" name=\"product.quantity\" value=\"10\" />", 
				helper.PasswordField("product.quantity"));
			Assert.AreEqual("<input type=\"password\" id=\"confirmation\" name=\"confirmation\" value=\"abc\" />", 
				helper.PasswordField("confirmation"));
		}

		[Test]
		public void TextFieldValue()
		{
			Assert.AreEqual("<input type=\"text\" id=\"product_price\" name=\"product.price\" value=\"$12.30\" />", 
				helper.TextFieldValue("product.price", product.Price.ToString("C")));
		}

		[Test]
		public void TextFieldFormat()
		{
			Assert.AreEqual("<input type=\"text\" id=\"product_price\" name=\"product.price\" value=\"$12.30\" />", 
				helper.TextFieldFormat("product.price", "C"));
		}

		[Test]
		public void HiddenField()
		{
			Assert.AreEqual("<input type=\"hidden\" id=\"product_name\" name=\"product.name\" value=\"memory card\" />", 
				helper.HiddenField("product.name"));
			Assert.AreEqual("<input type=\"hidden\" id=\"product_quantity\" name=\"product.quantity\" value=\"10\" />", 
				helper.HiddenField("product.quantity"));
		}

		[Test]
		public void CheckboxField()
		{
			product.IsAvailable = false;

			Assert.AreEqual("<input type=\"checkbox\" id=\"product_isavailable\" name=\"product.isavailable\" value=\"true\" />", 
				helper.CheckboxField("product.isavailable"));

			product.IsAvailable = true;

			Assert.AreEqual("<input type=\"checkbox\" id=\"product_isavailable\" name=\"product.isavailable\" value=\"true\" checked />", 
				helper.CheckboxField("product.isavailable"));

			Assert.AreEqual("<input type=\"checkbox\" id=\"sendemail\" name=\"sendemail\" value=\"true\" checked />", 
				helper.CheckboxField("sendemail"));
		}

		[Test]
		public void LabelFor()
		{
			Assert.AreEqual("<label for=\"product_name\">Name:</label>", 
				helper.LabelFor("product.name", "Name:"));
		}

		[Test]
		public void SimplisticSelect()
		{
			ArrayList list = new ArrayList();

			list.Add("cat1");
			list.Add("cat2");

			Assert.AreEqual("<select id=\"product_category_id\" name=\"product.category.id\" >\r\n" + 
				"<option>cat1</option>\r\n<option>cat2</option>\r\n</select>",
				helper.Select("product.category.id", list));
		}

		[Test]
		public void SelectWithFirstOption()
		{
			ArrayList list = new ArrayList();
			list.Add("cat1");
			list.Add("cat2");

			Assert.AreEqual("<select id=\"product_category_id\" name=\"product.category.id\" >\r\n" + 
				"<option value=\"0\">Please select</option>\r\n" + 
				"<option>cat1</option>\r\n<option>cat2</option>\r\n</select>",
				helper.Select("product.category.id", list, DictHelper.Create("firstoption=Please select") ));
		}

		[Test]
		public void SelectWithValueAndText()
		{
			ArrayList list = new ArrayList();
			list.Add(new ProductCategory(1, "cat1"));
			list.Add(new ProductCategory(2, "cat2"));

			Assert.AreEqual("<select id=\"product_category_id\" name=\"product.category.id\" >\r\n" + 
				"<option value=\"1\">cat1</option>\r\n<option value=\"2\">cat2</option>\r\n</select>",
				helper.Select("product.category.id", list, DictHelper.Create("value=id", "text=name") ));
		}

		[Test]
		public void SelectWithValueAndTextAndSelect()
		{
			ArrayList list = new ArrayList();
			list.Add(new ProductCategory(1, "cat1"));
			list.Add(new ProductCategory(2, "cat2"));

			product.Category.Id = 2;

			Assert.AreEqual("<select id=\"product_category_id\" name=\"product.category.id\" >\r\n" + 
				"<option value=\"1\">cat1</option>\r\n<option selected value=\"2\">cat2</option>\r\n</select>",
				helper.Select("product.category.id", list, DictHelper.Create("value=id", "text=name") ));
		}

		[Test]
		public void SelectWithNoSelection()
		{
			ArrayList list = new ArrayList();
			list.Add(new Role(1, "role1"));
			list.Add(new Role(2, "role2"));

			Assert.AreEqual("<select id=\"user_roles\" name=\"user.roles\" >\r\n" + 
				"<option value=\"1\">role1</option>\r\n<option value=\"2\">role2</option>\r\n</select>",
				helper.Select("user.roles", list, DictHelper.Create("value=id", "text=name") ));
		}

		[Test]
		public void SelectWithSelection()
		{
			ArrayList list = new ArrayList();
			list.Add(new Role(1, "role1"));
			list.Add(new Role(2, "role2"));

			user.Roles.Add(new Role(1, "role1"));
			user.Roles.Add(new Role(2, "role2"));

			Assert.AreEqual("<select id=\"user_roles\" name=\"user.roles\" >\r\n" + 
				"<option selected value=\"1\">role1</option>\r\n<option selected value=\"2\">role2</option>\r\n</select>",
				helper.Select("user.roles", list, DictHelper.Create("value=id", "text=name") ));
		}

		[Test]
		public void SelectWithSelection2()
		{
			ArrayList list = new ArrayList();
			list.Add(new Role(1, "role1"));
			list.Add(new Role(2, "role2"));

			user.Roles.Add(new Role(1, "role1"));

			Assert.AreEqual("<select id=\"user_roles\" name=\"user.roles\" >\r\n" + 
				"<option selected value=\"1\">role1</option>\r\n<option value=\"2\">role2</option>\r\n</select>",
				helper.Select("user.roles", list, DictHelper.Create("value=id", "text=name") ));
		}

		[Test]
		public void SelectWithArraySelection()
		{
			ArrayList list = new ArrayList();
			list.Add(new Role(1, "role1"));
			list.Add(new Role(2, "role2"));

			user.Roles.Add(new Role(1, "role1"));
			user.Roles.Add(new Role(2, "role2"));

			Assert.AreEqual("<select id=\"user_RolesAsArray\" name=\"user.RolesAsArray\" >\r\n" + 
				"<option selected value=\"1\">role1</option>\r\n<option selected value=\"2\">role2</option>\r\n</select>",
				helper.Select("user.RolesAsArray", list, DictHelper.Create("value=id", "text=name") ));
		}

		[Test]
		public void SelectWithArraySelection2()
		{
			ArrayList list = new ArrayList();
			list.Add(new Role(1, "role1"));
			list.Add(new Role(2, "role2"));

			user.Roles.Add(new Role(1, "role1"));

			Assert.AreEqual("<select id=\"user_RolesAsArray\" name=\"user.RolesAsArray\" >\r\n" + 
				"<option selected value=\"1\">role1</option>\r\n<option value=\"2\">role2</option>\r\n</select>",
				helper.Select("user.RolesAsArray", list, DictHelper.Create("value=id", "text=name") ));
		}

		[Test]
		public void SelectWithArraySelectionNoId()
		{
			ArrayList list = new ArrayList();
			list.Add(new Role(1, "role1"));
			list.Add(new Role(2, "role2"));

			user.Roles.Add(new Role(1, "role1"));
			user.Roles.Add(new Role(2, "role2"));

			Assert.AreEqual("<select id=\"user_RolesAsArray\" name=\"user.RolesAsArray\" >\r\n" + 
				"<option selected>role1</option>\r\n<option selected>role2</option>\r\n</select>",
				helper.Select("user.RolesAsArray", list));
		}
	}

	#region Classes skeletons

	public class Product
	{
		private String name;
		private int quantity;
		private Decimal price;
		private bool isAvailable;
		private ProductCategory category = new ProductCategory();

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

		public bool IsAvailable
		{
			get { return isAvailable; }
			set { isAvailable = value; }
		}

		public ProductCategory Category
		{
			get { return category; }
			set { category = value; }
		}
	}

	public class ProductCategory
	{
		private int id;
		private String name;

		public ProductCategory()
		{
		}

		public ProductCategory(int id, String name)
		{
			this.id = id;
			this.name = name;
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public String Name
		{
			get { return name; }
			set { name = value; }
		}
	}

	public class Role
	{
		private int id;
		private String name;

		public Role(int id, string name)
		{
			this.id = id;
			this.name = name;
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public String Name
		{
			get { return name; }
			set { name = value; }
		}

		public override bool Equals(object obj)
		{
			Role other = obj as Role;
			
			if (other != null)
			{
				return other.Id == Id;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return id;
		}

		public override string ToString()
		{
			return name;
		}
	}

	public class SimpleUser
	{
		private int id;
		private String name;
		private ArrayList roles = new ArrayList();

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public Role[] RolesAsArray
		{
			get { return (Role[]) roles.ToArray(typeof(Role)); }
		}

		public IList Roles
		{
			get { return roles; }
		}
	}

	#endregion
}
