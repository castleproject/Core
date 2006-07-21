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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System.Collections;
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
	}
}
