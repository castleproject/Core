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
#if DOTNET2
	using System.Collections.Generic;
#endif
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
		public void OverridingElementId()
		{
			Assert.AreEqual("<input type=\"text\" id=\"something\" name=\"product.name\" value=\"memory card\" />", 
				helper.TextField("product.name", DictHelper.Create("id=something")));
		
			Assert.AreEqual("<input type=\"password\" id=\"something\" name=\"product.name\" value=\"memory card\" />", 
				helper.PasswordField("product.name", DictHelper.Create("id=something")));
			
			Assert.AreEqual("<input type=\"hidden\" id=\"something\" name=\"product.name\" value=\"memory card\" />", 
				helper.HiddenField("product.name", DictHelper.Create("id=something")));
			
			product.IsAvailable = false;

			Assert.AreEqual("<input type=\"checkbox\" id=\"something\" name=\"product.isavailable\" value=\"true\" />" + 
				"<input type=\"hidden\" id=\"somethingH\" name=\"product.isavailable\" value=\"false\" />", 
				helper.CheckboxField("product.isavailable", DictHelper.Create("id=something")));

			user.IsActive = true;

			Assert.AreEqual("<input type=\"radio\" id=\"something\" name=\"user.isactive\" value=\"True\" checked=\"checked\" />", 
				helper.RadioField("user.isactive", true, DictHelper.Create("id=something")));

			Assert.AreEqual("<label for=\"something\">Name:</label>", 
				helper.LabelFor("product.name", "Name:", DictHelper.Create("id=something")));

			ArrayList list = new ArrayList();

			list.Add("cat1");
			list.Add("cat2");

			Assert.AreEqual("<select id=\"something\" name=\"product.category.id\" >\r\n" + 
				"<option value=\"cat1\">cat1</option>\r\n<option value=\"cat2\">cat2</option>\r\n</select>",
				helper.Select("product.category.id", list, DictHelper.Create("id=something")));
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

#if DOTNET2
		
		[Test]
		public void IndexedValueTextFieldInDotNet2()
		{
			subscription.Months4.Clear();
			subscription.Months4.Add(new Month(1, "Jan"));
			subscription.Months4.Add(new Month(2, "Feb"));

			Assert.AreEqual("<input type=\"text\" id=\"subscription_months4_0_id\" name=\"subscription.months4[0].id\" value=\"1\" />",
				helper.TextField("subscription.months4[0].id"));
			Assert.AreEqual("<input type=\"text\" id=\"subscription_months4_0_name\" name=\"subscription.months4[0].name\" value=\"Jan\" />",
				helper.TextField("subscription.months4[0].name"));
		}

#endif

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
		int[] months; 
		IList months2 = new ArrayList();
		Month[] months3;
#if DOTNET2
		IList<Month> months4 = new CustomList<Month>();
#endif

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

		public Month[] Months3
		{
			get { return months3; }
			set { months3 = value; }
		}

#if DOTNET2

		public IList<Month> Months4
		{
			get { return this.months4; }
			set { this.months4 = value; }
		}

#endif
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
	
	public class Role2
	{
		private int identification;
		private String name;

		public Role2(int id, string name)
		{
			this.identification = id;
			this.name = name;
		}

		public int Identification
		{
			get { return identification; }
			set { identification = value; }
		}

		public String Name
		{
			get { return name; }
			set { name = value; }
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
	
	public class Contact
	{
		private Month dobMonth;

		public Month DobMonth
		{
			get { return this.dobMonth; }
			set { this.dobMonth = value; }
		}
	}
	
#if DOTNET2

	public class CustomList<T> : IList<T>
	{
		private List<T> innerList = new List<T>();
		
		public int IndexOf(T item)
		{
			return innerList.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			innerList.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			innerList.RemoveAt(index);
		}

		public T this[int index]
		{
			get { return innerList[index]; }
			set { innerList[index] = value; }
		}

		public void Add(T item)
		{
			innerList.Add(item);
		}

		public void Clear()
		{
			innerList.Clear();
		}

		public bool Contains(T item)
		{
			return innerList.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return innerList.Count; }
		}

		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public IEnumerator GetEnumerator()
		{
			return innerList.GetEnumerator();
		}
	}

#endif

	#endregion
}
