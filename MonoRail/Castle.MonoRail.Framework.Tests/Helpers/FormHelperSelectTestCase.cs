// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.IO;
	using System.Threading;

	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Tests.Controllers;
	
	using NUnit.Framework;

	[TestFixture]
	public class FormHelperSelectTestCase
	{
		private FormHelper helper;
		private Product product;
		private SimpleUser user;
		private Subscription subscription;
		private Month[] months;
		private Contact contact;
		private DataTable workTable;

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
			contact = new Contact();

			HomeController controller = new HomeController();
			ControllerContext context = new ControllerContext();

			context.PropertyBag.Add("product", product);
			context.PropertyBag.Add("user", user);
			context.PropertyBag.Add("roles", new Role[] { new Role(1, "a"), new Role(2, "b"), new Role(3, "c") });
			context.PropertyBag.Add("sendemail", true);
			context.PropertyBag.Add("confirmation", "abc");
			context.PropertyBag.Add("fileaccess", FileAccess.Read);
			context.PropertyBag.Add("subscription", subscription);
			context.PropertyBag.Add("months", months);
			context.PropertyBag.Add("contact", contact);

			workTable = new DataTable("Customers");
			DataColumn workCol = workTable.Columns.Add("CustID", typeof(Int32));
			workCol.AllowDBNull = false;
			workCol.Unique = true;
			workTable.Columns.Add("Name", typeof(String));

			DataRow row = workTable.NewRow();
			row[0] = 1;
			row[1] = "chris rocks";
			workTable.Rows.Add(row);
			row = workTable.NewRow();
			row[0] = 2;
			row[1] = "will ferrell";
			workTable.Rows.Add(row);

			helper.SetController(controller, context);
		}

		[Test]
		public void SimplisticSelect()
		{
			ArrayList list = new ArrayList();

			list.Add("cat1");
			list.Add("cat2");

			Assert.AreEqual("<select id=\"product_category_id\" name=\"product.category.id\" >" + Environment.NewLine + 
				"<option value=\"cat1\">cat1</option>" + Environment.NewLine + "<option value=\"cat2\">cat2</option>" + Environment.NewLine + "</select>",
				helper.Select("product.category.id", list));
		}

		[Test]
		public void SelectWithFirstOption()
		{
			ArrayList list = new ArrayList();
			list.Add("cat1");
			list.Add("cat2");

			Assert.AreEqual("<select id=\"product_category_id\" name=\"product.category.id\" >" + Environment.NewLine + 
				"<option value=\"0\">Please select</option>" + Environment.NewLine + 
				"<option value=\"cat1\">cat1</option>" + Environment.NewLine + "<option value=\"cat2\">cat2</option>" + Environment.NewLine + "</select>",
				helper.Select("product.category.id", list, DictHelper.Create("firstoption=Please select") ));
		}

		[Test]
		public void SelectWithValueAndText()
		{
			ArrayList list = new ArrayList();
			list.Add(new ProductCategory(1, "cat1"));
			list.Add(new ProductCategory(2, "cat2"));

			Assert.AreEqual("<select id=\"product_category_id\" name=\"product.category.id\" >" + Environment.NewLine + 
				"<option value=\"1\">cat1</option>" + Environment.NewLine + "<option value=\"2\">cat2</option>" + Environment.NewLine + "</select>",
				helper.Select("product.category.id", list, DictHelper.Create("value=id", "text=name") ));
		}

		[Test]
		public void SelectOnPrimitiveArrayWithoutValueAndText()
		{
			Assert.AreEqual("<select id=\"product_category_id\" name=\"product.category.id\" >" + Environment.NewLine +
				"<option value=\"1\">1</option>" + Environment.NewLine + "<option value=\"2\">2</option>" + Environment.NewLine + "<option value=\"3\">3</option>" + Environment.NewLine + "<option value=\"4\">4</option>" + Environment.NewLine + "<option selected=\"selected\" value=\"5\">5</option>" + Environment.NewLine + "</select>",
				helper.Select("product.category.id", 5, new int[] { 1, 2, 3, 4, 5}, DictHelper.Create() ));
		}

		[Test]
		public void SelectWithValueAndTextAndSelect()
		{
			ArrayList list = new ArrayList();
			list.Add(new ProductCategory(1, "cat1"));
			list.Add(new ProductCategory(2, "cat2"));

			product.Category.Id = 2;

			Assert.AreEqual("<select id=\"product_category_id\" name=\"product.category.id\" >" + Environment.NewLine +
				"<option value=\"1\">cat1</option>" + Environment.NewLine + "<option selected=\"selected\" value=\"2\">cat2</option>" + Environment.NewLine + "</select>",
				helper.Select("product.category.id", list, DictHelper.Create("value=id", "text=name") ));
		}

		[Test]
		public void SelectWithNoSelection()
		{
			ArrayList list = new ArrayList();
			list.Add(new Role(1, "role1"));
			list.Add(new Role(2, "role2"));

			Assert.AreEqual("<select id=\"user_roles\" name=\"user.roles\" >" + Environment.NewLine + 
				"<option value=\"1\">role1</option>" + Environment.NewLine + "<option value=\"2\">role2</option>" + Environment.NewLine + "</select>",
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

			Assert.AreEqual("<select id=\"user_roles\" name=\"user.roles\" >" + Environment.NewLine + 
				"<option selected=\"selected\" value=\"1\">role1</option>" + Environment.NewLine + "<option selected=\"selected\" value=\"2\">role2</option>" + Environment.NewLine + "</select>",
				helper.Select("user.roles", list, DictHelper.Create("value=id", "text=name") ));
		}

		[Test]
		public void SelectWithSelection2()
		{
			ArrayList list = new ArrayList();
			list.Add(new Role(1, "role1"));
			list.Add(new Role(2, "role2"));

			user.Roles.Add(new Role(1, "role1"));

			Assert.AreEqual("<select id=\"user_roles\" name=\"user.roles\" >" + Environment.NewLine + 
				"<option selected=\"selected\" value=\"1\">role1</option>" + Environment.NewLine + "<option value=\"2\">role2</option>" + Environment.NewLine + "</select>",
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

			Assert.AreEqual("<select id=\"user_RolesAsArray\" name=\"user.RolesAsArray\" >" + Environment.NewLine + 
				"<option selected=\"selected\" value=\"1\">role1</option>" + Environment.NewLine + "<option selected=\"selected\" value=\"2\">role2</option>" + Environment.NewLine + "</select>",
				helper.Select("user.RolesAsArray", list, DictHelper.Create("value=id", "text=name") ));
		}

		[Test]
		public void SelectWithArraySelection2()
		{
			ArrayList list = new ArrayList();
			list.Add(new Role(1, "role1"));
			list.Add(new Role(2, "role2"));

			user.Roles.Add(new Role(1, "role1"));

			Assert.AreEqual("<select id=\"user_RolesAsArray\" name=\"user.RolesAsArray\" >" + Environment.NewLine + 
				"<option selected=\"selected\" value=\"1\">role1</option>" + Environment.NewLine + "<option value=\"2\">role2</option>" + Environment.NewLine + "</select>",
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

			Assert.AreEqual("<select id=\"user_RolesAsArray\" name=\"user.RolesAsArray\" >" + Environment.NewLine + 
				"<option selected=\"selected\" value=\"role1\">role1</option>" + Environment.NewLine + "<option selected=\"selected\" value=\"role2\">role2</option>" + Environment.NewLine + "</select>",
				helper.Select("user.RolesAsArray", list));
		}

		[Test]
		public void UsingDataTable()
		{
			Assert.AreEqual(
				"<select id=\"user_id\" name=\"user.id\" >" + Environment.NewLine + 
				"<option value=\"1\">chris rocks</option>" + Environment.NewLine + "<option value=\"2\">will ferrell</option>" + Environment.NewLine + "</select>",
				helper.Select("user.id", workTable.Rows, DictHelper.Create("value=custid", "text=name", "sourceProperty=id")));
		}

		[Test]
		public void UsingEnums()
		{
			Assert.AreEqual(
				"<select id=\"user_registration\" name=\"user.registration\" >" + Environment.NewLine + 
				"<option value=\"1\">unregistered</option>" + Environment.NewLine + 
				"<option value=\"2\">pending</option>" + Environment.NewLine + 
				"<option selected=\"selected\" value=\"6\">registered</option>" + Environment.NewLine + 
				"</select>",
				helper.Select("user.registration", Enum.GetValues(typeof(SimpleUser.RegistrationEnum))));
		}

		[Test]
		public void UsingEnumsAsText()
		{
			Assert.AreEqual(
				"<select id=\"user_registration\" name=\"user.registration\" >" + Environment.NewLine +
				"<option value=\"unregistered\">unregistered</option>" + Environment.NewLine +
				"<option value=\"pending\">pending</option>" + Environment.NewLine +
				"<option selected=\"selected\" value=\"registered\">registered</option>" + Environment.NewLine + 
				"</select>",
				helper.Select("user.registration", Enum.GetNames(typeof(SimpleUser.RegistrationEnum))));
		}	


		[Test]
		public void UsingInterface()
		{
			List<IInterfacedList> list = new List<IInterfacedList>();

			list.Add(new InterfacedClassA(1, "ernst"));
			list.Add(new InterfacedClassB(2, "enix"));

			Assert.AreEqual("<select id=\"user_name\" name=\"user.name\" >" + Environment.NewLine +
				"<option value=\"1\">ernst</option>" + Environment.NewLine + "<option value=\"2\">enix</option>" + Environment.NewLine + "</select>",
				helper.Select("user.name", list, DictHelper.Create("value=id", "text=name")));
		}

		[Test]
		public void BasicFunctionalityInDotNet2()
		{
			List<Month> list = new List<Month>();
			list.Add(new Month(1, "Jan"));
			list.Add(new Month(2, "Feb"));

			Assert.AreEqual("<select id=\"contact_dobmonth_id\" name=\"contact.dobmonth.id\" >" + Environment.NewLine + 
				"<option value=\"1\">Jan</option>" + Environment.NewLine + "<option value=\"2\">Feb</option>" + Environment.NewLine + "</select>",
				helper.Select("contact.dobmonth.id", list, DictHelper.Create("value=id", "text=name")));
		}

		[Test]
		public void SelectWithCompositKey()
		{
			ArrayList list = new ArrayList();
			list.Add(new ClassWithCompositKey(1, "cat1"));
			list.Add(new ClassWithCompositKey(2, "cat2"));

			Assert.AreEqual("<select id=\"product_category_id\" name=\"product.category.id\" >" + Environment.NewLine +
				"<option value=\"1\">cat1</option>" + Environment.NewLine + "<option value=\"2\">cat2</option>" + Environment.NewLine + "</select>",
				helper.Select("product.category.id", list, DictHelper.Create("value=Key.Id", "text=name")));
		}
	}
}
