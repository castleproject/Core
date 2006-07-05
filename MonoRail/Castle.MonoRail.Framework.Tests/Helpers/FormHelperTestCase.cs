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
	using System.IO;
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
		private Subscription subscription;
		private Month[] months;

		[SetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture("en");

			Thread.CurrentThread.CurrentCulture	= en;
			Thread.CurrentThread.CurrentUICulture = en;

			helper = new FormHelper();

			subscription = new Subscription();
			months = new Month[] {new Month(1, "January"), new Month(1, "February")};
			product = new Product("memory card", 10, (decimal) 12.30);
			user = new SimpleUser();

			HomeController controller = new HomeController();

			controller.PropertyBag.Add("product", product);
			controller.PropertyBag.Add("user", user);
			controller.PropertyBag.Add("roles", new Role[] { new Role(1, "a"), new Role(2, "b"), new Role(3, "c") });
			controller.PropertyBag.Add("sendemail", true);
			controller.PropertyBag.Add("confirmation", "abc");
			controller.PropertyBag.Add("fileaccess", FileAccess.Read);
			controller.PropertyBag.Add("subscription", subscription);
			controller.PropertyBag.Add("months", months);

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

		/// <summary>
		/// Tests the subscription.Months as null and an int array as source
		/// </summary>
		[Test]
		public void CheckboxFieldList1()
		{
			FormHelper.CheckboxList list = 
				helper.CreateCheckboxList("subscription.Months", new int[] {1,2,3,4,5,6});
			
			Assert.IsNotNull(list);
			
			int index = 0;
			
			foreach(Object item in list)
			{
				String content = list.Item();
				Assert.AreEqual("<input type=\"checkbox\" id=\"subscription_Months_" + index + 
					"_\" name=\"subscription.Months[" + index + "]\" value=\"" + item + "\" />", content);
				index++;
			}
		}

		/// <summary>
		/// Tests the subscription.Months as non null and an int array as source
		/// </summary>
		[Test]
		public void CheckboxFieldList2()
		{
			subscription.Months = new int[] { 1, 2, 3, 4, 5 };
			
			FormHelper.CheckboxList list = 
				helper.CreateCheckboxList("subscription.Months", new int[] {1,2,3,4,5});
			
			Assert.IsNotNull(list);
			
			int index = 0;

			foreach(Object item in list)
			{
				String content = list.Item();
				Assert.AreEqual("<input type=\"checkbox\" id=\"subscription_Months_" + index + 
					"_\" name=\"subscription.Months[" + index + "]\" value=\"" + item + "\" checked=\"checked\" />", content);
				index++;
			}
		}
		
		/// <summary>
		/// Tests the subscription.Months2 as null and using a <c>Month</c> array as data source
		/// </summary>
		[Test]
		public void CheckboxFieldList3()
		{
			FormHelper.CheckboxList list = 
				helper.CreateCheckboxList("subscription.Months2", months, DictHelper.Create("value=id"));
			
			Assert.IsNotNull(list);

			int index = 0;

			foreach(Month item in list)
			{
				String content = list.Item();
				Assert.AreEqual("<input type=\"checkbox\" id=\"subscription_Months2_" + index + 
					"_\" name=\"subscription.Months2[" + index + "].Id\" value=\"" + item.Id + "\" />", content);
				index++;
			}
		}
		
		/// <summary>
		/// Tests the subscription.Months2 as non null and using a <c>Month</c> array as data source
		/// </summary>
		[Test]
		public void CheckboxFieldList4()
		{
			subscription.Months2 = new Month[] {new Month(3, "March")};
			
			FormHelper.CheckboxList list = 
				helper.CreateCheckboxList("subscription.Months2", months, DictHelper.Create("value=id"));
			
			Assert.IsNotNull(list);

			int index = 0;

			foreach(Month item in list)
			{
				String content = list.Item();
				Assert.AreEqual("<input type=\"checkbox\" id=\"subscription_Months2_" + index + 
					"_\" name=\"subscription.Months2[" + index + "].Id\" value=\"" + item.Id + "\" />", content);
				index++;
			}
		}

		/// <summary>
		/// Tests the subscription.Months2 as non null and using a <c>Month</c> array as data source
		/// but with selection
		/// </summary>
		[Test]
		public void CheckboxFieldList5()
		{
			subscription.Months2 = new Month[] {new Month(1, "January"), new Month(2, "Feb") };
			
			FormHelper.CheckboxList list = 
				helper.CreateCheckboxList("subscription.Months2", months, DictHelper.Create("value=id"));
			
			Assert.IsNotNull(list);

			int index = 0;

			foreach(Month item in list)
			{
				String content = list.Item();
				Assert.AreEqual("<input type=\"checkbox\" id=\"subscription_Months2_" + index + 
					"_\" name=\"subscription.Months2[" + index + "].Id\" value=\"" + item.Id + "\" checked=\"checked\" />", content);
				index++;
			}
		}

		[Test]
		public void CheckboxField()
		{
			product.IsAvailable = false;

			Assert.AreEqual("<input type=\"checkbox\" id=\"product_isavailable\" name=\"product.isavailable\" value=\"true\" />" + 
				"<input type=\"hidden\" id=\"product_isavailableH\" name=\"product.isavailable\" value=\"false\" />", 
				helper.CheckboxField("product.isavailable"));

			product.IsAvailable = true;

			Assert.AreEqual("<input type=\"checkbox\" id=\"product_isavailable\" name=\"product.isavailable\" value=\"true\" checked=\"checked\" />" + 
				"<input type=\"hidden\" id=\"product_isavailableH\" name=\"product.isavailable\" value=\"false\" />", 
				helper.CheckboxField("product.isavailable"));

			Assert.AreEqual("<input type=\"checkbox\" id=\"sendemail\" name=\"sendemail\" value=\"true\" checked=\"checked\" />" + 
				"<input type=\"hidden\" id=\"sendemailH\" name=\"sendemail\" value=\"false\" />", 
				helper.CheckboxField("sendemail"));

			Assert.AreEqual("<input type=\"checkbox\" id=\"sendemail\" name=\"sendemail\" value=\"true\" checked=\"checked\" />" + 
				"<input type=\"hidden\" id=\"sendemailH\" name=\"sendemail\" value=\"0\" />", 
				helper.CheckboxField("sendemail", new DictHelper().CreateDict("falseValue=0")));
		}

		[Test]
		public void RadioField()
		{
			user.IsActive = true;

			Assert.AreEqual("<input type=\"radio\" id=\"user_isactive\" name=\"user.isactive\" value=\"True\" checked=\"checked\" />", 
				helper.RadioField("user.isactive", true));

			user.IsActive = false;

			Assert.AreEqual("<input type=\"radio\" id=\"user_isactive\" name=\"user.isactive\" value=\"True\" />", 
				helper.RadioField("user.isactive", true));
		}

		[Test]
		public void RadioFieldWithEnums()
		{
			Assert.AreEqual("<input type=\"radio\" id=\"fileaccess\" name=\"fileaccess\" value=\"Read\" checked=\"checked\" />", 
				helper.RadioField("fileaccess", FileAccess.Read));

			Assert.AreEqual("<input type=\"radio\" id=\"fileaccess\" name=\"fileaccess\" value=\"Read\" checked=\"checked\" />", 
				helper.RadioField("fileaccess", "Read"));

			Assert.AreEqual("<input type=\"radio\" id=\"fileaccess\" name=\"fileaccess\" value=\"Write\" />", 
				helper.RadioField("fileaccess", FileAccess.Write));

			Assert.AreEqual("<input type=\"radio\" id=\"fileaccess\" name=\"fileaccess\" value=\"Write\" />", 
				helper.RadioField("fileaccess", "Write"));
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
				"<option value=\"cat1\">cat1</option>\r\n<option value=\"cat2\">cat2</option>\r\n</select>",
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
				"<option value=\"cat1\">cat1</option>\r\n<option value=\"cat2\">cat2</option>\r\n</select>",
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
		public void SelectOnPrimitiveArrayWithoutValueAndText()
		{
			Assert.AreEqual("<select id=\"product_category_id\" name=\"product.category.id\" >\r\n" + 
				"<option value=\"1\">1</option>\r\n<option value=\"2\">2</option>\r\n<option value=\"3\">3</option>\r\n<option value=\"4\">4</option>\r\n<option selected=\"selected\" value=\"5\">5</option>\r\n</select>",
				helper.Select("product.category.id", 5, new int[] { 1, 2, 3, 4, 5}, DictHelper.Create() ));
		}

		[Test]
		public void SelectWithValueAndTextAndSelect()
		{
			ArrayList list = new ArrayList();
			list.Add(new ProductCategory(1, "cat1"));
			list.Add(new ProductCategory(2, "cat2"));

			product.Category.Id = 2;

			Assert.AreEqual("<select id=\"product_category_id\" name=\"product.category.id\" >\r\n" + 
				"<option value=\"1\">cat1</option>\r\n<option selected=\"selected\" value=\"2\">cat2</option>\r\n</select>",
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
				"<option selected=\"selected\" value=\"1\">role1</option>\r\n<option selected=\"selected\" value=\"2\">role2</option>\r\n</select>",
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
				"<option selected=\"selected\" value=\"1\">role1</option>\r\n<option value=\"2\">role2</option>\r\n</select>",
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
				"<option selected=\"selected\" value=\"1\">role1</option>\r\n<option selected=\"selected\" value=\"2\">role2</option>\r\n</select>",
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
				"<option selected=\"selected\" value=\"1\">role1</option>\r\n<option value=\"2\">role2</option>\r\n</select>",
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
				"<option selected=\"selected\" value=\"role1\">role1</option>\r\n<option selected=\"selected\" value=\"role2\">role2</option>\r\n</select>",
				helper.Select("user.RolesAsArray", list));
		}

		[Test]
		public void TextFieldWithIndex()
		{
			Assert.AreEqual("<input type=\"text\" id=\"roles_0_Id\" name=\"roles[0].Id\" value=\"1\" />",
				helper.TextField("roles[0].Id"));

			Assert.AreEqual("<input type=\"text\" id=\"roles_1_Name\" name=\"roles[1].Name\" value=\"b\" />",
				helper.TextField("roles[1].Name"));
		}

		[Test]
		public void TextFieldWithNestedIndex()
		{
			user.Roles.Add(new Role(1, "role1"));
			user.Roles.Add(new Role(2, "role2"));

			Assert.AreEqual("<input type=\"text\" id=\"user_roles_0_Id\" name=\"user.roles[0].Id\" value=\"1\" />",
				helper.TextField("user.roles[0].Id"));

			Assert.AreEqual("<input type=\"text\" id=\"user_roles_0_Name\" name=\"user.roles[0].Name\" value=\"role1\" />",
				helper.TextField("user.roles[0].Name"));

			Assert.AreEqual("<input type=\"text\" id=\"user_roles_1_Name\" name=\"user.roles[1].Name\" value=\"role2\" />",
				helper.TextField("user.roles[1].Name"));
		}

		[Test]
		public void TextFieldWithNestedIndexAndNullValues1()
		{
			user.Roles = null;

			Assert.AreEqual("<input type=\"text\" id=\"user_roles_0_Id\" name=\"user.roles[0].Id\" value=\"\" />",
				helper.TextField("user.roles[0].Id"));

			Assert.AreEqual("<input type=\"text\" id=\"user_roles_0_Name\" name=\"user.roles[0].Name\" value=\"\" />",
				helper.TextField("user.roles[0].Name"));

			Assert.AreEqual("<input type=\"text\" id=\"user_roles_1_Name\" name=\"user.roles[1].Name\" value=\"\" />",
				helper.TextField("user.roles[1].Name"));
		}

		[Test]
		public void TextFieldWithNestedIndexAndNullValues2()
		{
			user.Roles.Add(new Role(1, null));
			user.Roles.Add(new Role(2, null));

			Assert.AreEqual("<input type=\"text\" id=\"user_roles_0_Name\" name=\"user.roles[0].Name\" value=\"\" />",
				helper.TextField("user.roles[0].Name"));

			Assert.AreEqual("<input type=\"text\" id=\"user_roles_1_Name\" name=\"user.roles[1].Name\" value=\"\" />",
				helper.TextField("user.roles[1].Name"));
		}

		[Test, ExpectedException(typeof(RailsException))]
		public void InvalidIndex1()
		{
			helper.TextField("roles[a].Id");
		}

		[Test, ExpectedException(typeof(RailsException))]
		public void InvalidIndex2()
		{
			helper.TextField("roles[1a].Id");
		}

		[Test, ExpectedException(typeof(RailsException)), Ignore("The behavior for array access has changed")]
		public void InvalidIndex3()
		{
			helper.TextField("roles[10].Id");
		}
	}

	#region Classes skeletons
	
	public class Month
	{
		private int id;
		private String name;

		public Month(int id, string name)
		{
			this.id = id;
			this.name = name;
		}

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
	}
	
	public class Subscription
	{
		int[] months; IList months2 = new ArrayList();

		public int[] Months
		{
			get { return months; }
			set { months = value; }
		}

		public IList Months2
		{
			get { return months2; }
			set { months2 = value; }
		}
	}

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
		private bool isActive;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public bool IsActive
		{
			get { return isActive; }
			set { isActive = value; }
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

		public ArrayList Roles
		{
			get { return roles; }
			set { roles = value; }
		}
	}

	#endregion
}
