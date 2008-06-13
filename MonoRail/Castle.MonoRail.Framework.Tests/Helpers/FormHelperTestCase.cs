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
	using System.Collections.Specialized;
	using System.Globalization;
	using System.IO;
	using System.Reflection;
	using System.Threading;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Tests.Controllers;
	using NUnit.Framework;
	using Test;

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

			Thread.CurrentThread.CurrentCulture = en;
			Thread.CurrentThread.CurrentUICulture = en;

			helper = new FormHelper();

			subscription = new Subscription();
			months = new Month[] { new Month(1, "January"), new Month(1, "February") };
			product = new Product("memory card", 10, (decimal)12.30);

			user = new SimpleUser();

			HomeController controller = new HomeController();
			ControllerContext context = new ControllerContext();

			context.PropertyBag.Add("product", product);
			context.PropertyBag.Add("user", user);
			context.PropertyBag.Add("roles", new Role[] { new Role(1, "a"), new Role(2, "b"), new Role(3, "c") });
			context.PropertyBag.Add("sendemail", true);
			context.PropertyBag.Add("sendemailstringtrue", "true");
			context.PropertyBag.Add("sendemailstringfalse", "false");
			context.PropertyBag.Add("confirmation", "abc");
			context.PropertyBag.Add("fileaccess", FileAccess.Read);
			context.PropertyBag.Add("subscription", subscription);
			context.PropertyBag.Add("months", months);
			context.PropertyBag.Add("age", 26);

			helper.SetController(controller, context);
		}

		[Test]
		public void FormTagDoesNotMixUrlParametersWithFormElementParameters()
		{
			// No solution here. Id is ambiguous
			// helper.FormTag(DictHelper.Create("noaction=true"));
		}

		[Test]
		public void OverridingElementId()
		{
			Assert.AreEqual("<input type=\"text\" id=\"something\" name=\"product.name\" value=\"memory card\" />",
							helper.TextField("product.name", DictHelper.Create("id=something")));

			Assert.AreEqual("<input type=\"password\" id=\"something\" name=\"product.name\" value=\"memory card\" />",
							helper.PasswordField("product.name", DictHelper.Create("id=something")));

			Assert.AreEqual("<input type=\"password\" id=\"something\" name=\"product.quantity\" value=\"10\" onkeypress=\"return monorail_formhelper_numberonly(event, [], []);\" />",
							helper.PasswordNumberField("product.quantity", DictHelper.Create("id=something")));

			Assert.AreEqual("<input type=\"hidden\" id=\"something\" name=\"product.name\" value=\"memory card\" />",
							helper.HiddenField("product.name", DictHelper.Create("id=something")));

			product.IsAvailable = false;

			Assert.AreEqual("<input type=\"checkbox\" id=\"something\" name=\"product.isavailable\" value=\"true\" />" +
							"<input type=\"hidden\" id=\"somethingH\" name=\"product.isavailable\" value=\"false\" />",
							helper.CheckboxField("product.isavailable", DictHelper.Create("id=something")));

			user.IsActive = true;

			Assert.AreEqual(
				"<input type=\"radio\" id=\"something\" name=\"user.isactive\" value=\"True\" checked=\"checked\" />",
				helper.RadioField("user.isactive", true, DictHelper.Create("id=something")));

			Assert.AreEqual("<label for=\"something\">Name:</label>",
							helper.LabelFor("product.name", "Name:", DictHelper.Create("id=something")));

			ArrayList list = new ArrayList();

			list.Add("cat1");
			list.Add("cat2");

			Assert.AreEqual("<select id=\"something\" name=\"product.category.id\" >" + Environment.NewLine +
							"<option value=\"cat1\">cat1</option>" + Environment.NewLine + "<option value=\"cat2\">cat2</option>" +
							Environment.NewLine + "</select>",
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
		public void TextFieldWithAutoCompletion()
		{
			Assert.AreEqual("<input type=\"text\" id=\"product_name\" name=\"product.name\" value=\"memory card\" autocomplete=\"off\" /><div id=\"product_nameautocomplete\" class=\"auto_complete\"></div><script type=\"text/javascript\">new Ajax.Autocompleter('product_name', 'product_nameautocomplete', 'someurl',{})</script>",
							helper.TextFieldAutoComplete("product.name", "someurl", DictHelper.Create(), DictHelper.Create()));
			Assert.AreEqual("<input type=\"text\" id=\"product_name\" name=\"product.name\" value=\"memory card\" autocomplete=\"on\" /><div id=\"product_nameautocomplete\" class=\"auto_complete\"></div><script type=\"text/javascript\">new Ajax.Autocompleter('product_name', 'product_nameautocomplete', 'someurl',{})</script>",
							helper.TextFieldAutoComplete("product.name", "someurl", DictHelper.Create("autocomplete=on"), DictHelper.Create()));
			Assert.IsTrue(helper.TextFieldAutoComplete("product.name", "someurl", DictHelper.Create(), DictHelper.Create()).Contains(helper.TextField("product.name", DictHelper.Create("autocomplete=off"))));
		}

		[Test]
		public void NumberField()
		{
			Assert.AreEqual(
				"<input type=\"text\" id=\"product_quantity\" name=\"product.quantity\" value=\"10\" onkeypress=\"return monorail_formhelper_numberonly(event, [], []);\" />",
				helper.NumberField("product.quantity"));
			Assert.AreEqual(
				"<input type=\"text\" id=\"product_quantity\" name=\"product.quantity\" value=\"10\" onkeypress=\"return monorail_formhelper_numberonly(event, [1], []);\" />",
				helper.NumberField("product.quantity", DictHelper.Create("exceptions=1")));
			Assert.AreEqual(
				"<input type=\"text\" id=\"product_quantity\" name=\"product.quantity\" value=\"10\" onkeypress=\"return monorail_formhelper_numberonly(event, [1,2], []);\" />",
				helper.NumberField("product.quantity", DictHelper.Create("exceptions=1,2")));
		}

		[Test]
		public void MaskedNumberField()
		{
			Assert.AreEqual(
				"<input type=\"text\" id=\"product_quantity\" name=\"product.quantity\" value=\"10\" onkeypress=\"return monorail_formhelper_numberonly(event, [], []);\" " +
				"onblur=\"javascript:void(0);return monorail_formhelper_mask(event,this,'2,5','-');\" onkeyup=\"javascript:void(0);return monorail_formhelper_mask(event,this,'2,5','-');\" />",
				helper.NumberField("product.quantity", DictHelper.Create("mask=2,5")));
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
		public void PasswordFieldWithDoNotSetBehaviour()
		{
			Assert.AreEqual("<input type=\"password\" id=\"product_name\" name=\"product.name\" value=\"\" />",
							helper.PasswordField("product.name", ValueBehaviour.DoNotSet));
			Assert.AreEqual("<input type=\"password\" id=\"product_quantity\" name=\"product.quantity\" value=\"\" />",
							helper.PasswordField("product.quantity", ValueBehaviour.DoNotSet));
			Assert.AreEqual("<input type=\"password\" id=\"confirmation\" name=\"confirmation\" value=\"\" />",
							helper.PasswordField("confirmation", ValueBehaviour.DoNotSet));
		}

		[Test]
		public void PasswordNumberField()
		{
			Assert.AreEqual("<input type=\"password\" id=\"age\" name=\"age\" value=\"26\" onkeypress=\"return monorail_formhelper_numberonly(event, [], []);\" />",
							helper.PasswordNumberField("age"));
			Assert.AreEqual("<input type=\"password\" id=\"product_quantity\" name=\"product.quantity\" value=\"10\" onkeypress=\"return monorail_formhelper_numberonly(event, [], []);\" />",
							helper.PasswordNumberField("product.quantity"));

			Hashtable attributes = new Hashtable();
			attributes.Add("exceptions", "65,66,67,68"); // accept A,B,C and D
			attributes.Add("forbid", "48,57"); // ignore 0 and 9

			Assert.AreEqual("<input type=\"password\" id=\"age\" name=\"age\" value=\"26\" onkeypress=\"return monorail_formhelper_numberonly(event, [65,66,67,68], [48,57]);\" />",
							helper.PasswordNumberField("age", attributes));
		}

		[Test]
		public void PasswordNumberFieldWithDoNotSetBehaviour()
		{
			Assert.AreEqual(
				"<input type=\"password\" id=\"age\" name=\"age\" value=\"\" onkeypress=\"return monorail_formhelper_numberonly(event, [], []);\" />",
				helper.PasswordNumberField("age", ValueBehaviour.DoNotSet));
			Assert.AreEqual(
				"<input type=\"password\" id=\"product_quantity\" name=\"product.quantity\" value=\"\" onkeypress=\"return monorail_formhelper_numberonly(event, [], []);\" />",
				helper.PasswordNumberField("product.quantity", ValueBehaviour.DoNotSet));

			Hashtable attributes = new Hashtable();
			attributes.Add("exceptions", "65,66,67,68"); // accept A,B,C and D
			attributes.Add("forbid", "48,57"); // ignore 0 and 9
			Assert.AreEqual(
				"<input type=\"password\" id=\"age\" name=\"age\" value=\"\" onkeypress=\"return monorail_formhelper_numberonly(event, [65,66,67,68], [48,57]);\" />",
				helper.PasswordNumberField("age", attributes, ValueBehaviour.DoNotSet));
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
							helper.TextField("product.price", DictHelper.Create("textformat=C")));
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

			Assert.AreEqual(
				"<input type=\"checkbox\" id=\"product_isavailable\" name=\"product.isavailable\" value=\"true\" />" +
				"<input type=\"hidden\" id=\"product_isavailableH\" name=\"product.isavailable\" value=\"false\" />",
				helper.CheckboxField("product.isavailable"));

			product.IsAvailable = true;

			Assert.AreEqual(
				"<input type=\"checkbox\" id=\"product_isavailable\" name=\"product.isavailable\" value=\"true\" checked=\"checked\" />" +
				"<input type=\"hidden\" id=\"product_isavailableH\" name=\"product.isavailable\" value=\"false\" />",
				helper.CheckboxField("product.isavailable"));

			Assert.AreEqual(
				"<input type=\"checkbox\" id=\"sendemail\" name=\"sendemail\" value=\"true\" checked=\"checked\" />" +
				"<input type=\"hidden\" id=\"sendemailH\" name=\"sendemail\" value=\"false\" />",
				helper.CheckboxField("sendemail"));

			Assert.AreEqual(
				"<input type=\"checkbox\" id=\"sendemail\" name=\"sendemail\" value=\"true\" checked=\"checked\" />" +
				"<input type=\"hidden\" id=\"sendemailH\" name=\"sendemail\" value=\"0\" />",
				helper.CheckboxField("sendemail", new DictHelper().CreateDict("falseValue=0")));

			// Checkbox values are actually added as string values to the Flash
			// NameValueCollection of the Controller. Therefore a string "false"
			// must render an unchecked checkbox.
			Assert.AreEqual("<input type=\"checkbox\" id=\"sendemailstringtrue\" name=\"sendemailstringtrue\" value=\"true\" checked=\"checked\" />" +
				"<input type=\"hidden\" id=\"sendemailstringtrueH\" name=\"sendemailstringtrue\" value=\"false\" />",
				helper.CheckboxField("sendemailstringtrue"));

			Assert.AreEqual("<input type=\"checkbox\" id=\"sendemailstringfalse\" name=\"sendemailstringfalse\" value=\"true\" />" +
				"<input type=\"hidden\" id=\"sendemailstringfalseH\" name=\"sendemailstringfalse\" value=\"false\" />",
				helper.CheckboxField("sendemailstringfalse"));

		}

		[Test]
		public void RadioField()
		{
			user.IsActive = true;

			Assert.AreEqual(
				"<input type=\"radio\" id=\"user_isactive\" name=\"user.isactive\" value=\"True\" checked=\"checked\" />",
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
		public void LabelForAttributed()
		{
			IDictionary attrs = new ListDictionary();
			attrs.Add("class", "cssclass");
			Assert.AreEqual("<label for=\"product_name\" class=\"cssclass\" >Name:</label>",
							helper.LabelFor("product.name", "Name:", attrs));
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

		[Test]
		public void IndexedValueTextFieldInDotNet2()
		{
			subscription.Months4.Clear();
			subscription.Months4.Add(new Month(1, "Jan"));
			subscription.Months4.Add(new Month(2, "Feb"));

			Assert.AreEqual(
				"<input type=\"text\" id=\"subscription_months4_0_id\" name=\"subscription.months4[0].id\" value=\"1\" />",
				helper.TextField("subscription.months4[0].id"));
			Assert.AreEqual(
				"<input type=\"text\" id=\"subscription_months4_0_name\" name=\"subscription.months4[0].name\" value=\"Jan\" />",
				helper.TextField("subscription.months4[0].name"));
		}

		[Test, ExpectedException(typeof(MonoRailException))]
		public void InvalidIndex1()
		{
			helper.TextField("roles[a].Id");
		}

		[Test, ExpectedException(typeof(MonoRailException))]
		public void InvalidIndex2()
		{
			helper.TextField("roles[1a].Id");
		}

		[Test, ExpectedException(typeof(MonoRailException)), Ignore("The behavior for array access has changed")]
		public void InvalidIndex3()
		{
			helper.TextField("roles[10].Id");
		}

		[Test]
		public void ShouldDiscoverRootTypeOnCollectionWhenIndexedRootInPropertyBag()
		{
			FormHelperEx sut = new FormHelperEx();
			sut.SetController(new HomeController(), new ControllerContext());

			//this is what current code requires to discover
			sut.ControllerContext.PropertyBag["items[0]type"] = typeof(Product);
			PropertyInfo prop = sut.ObtainTargetProperty(RequestContext.PropertyBag, "items[0].Name");
			PropertyInfo expect = typeof(Product).GetProperty("Name");
			Assert.AreEqual(expect, prop);
		}

		[Test]
		public void ShouldDiscoverRootTypeOnCollection()
		{
			FormHelperEx sut = new FormHelperEx();
			sut.SetController(new HomeController(), new ControllerContext());

			sut.ControllerContext.PropertyBag["itemstype"] = typeof(Product); //no need to pass indexer
			PropertyInfo prop = sut.ObtainTargetProperty(RequestContext.PropertyBag, "items[0].Name");
			PropertyInfo expect = typeof(Product).GetProperty("Name");
			Assert.AreEqual(expect, prop);
		}

		[Test]
		public void SessionAndFlashLessController() {
			FormHelper sut = new FormHelper();
			HomeController homeController = new HomeController();
			StubEngineContext context = new StubEngineContext();
			context.Session = null;
			context.Flash = null;
			ControllerContext controllerContext = new ControllerContext();
			
			homeController.Contextualize(context, controllerContext );
			sut.SetController(homeController, controllerContext);
			sut.SetContext(context);
           
			string area = sut.TextArea("item.Name");
			Assert.AreEqual(@"<textarea id=""item_Name"" name=""item.Name"" ></textarea>",area);
		}

		[Test]
		public void ShouldFailToDiscoverRootTypeOnCollectionWhenNoTypeInPropertyBag()
		{
			FormHelperEx sut = new FormHelperEx();
			sut.SetController(new HomeController(), new ControllerContext());

			PropertyInfo prop = sut.ObtainTargetProperty(RequestContext.PropertyBag, "items[0].Name");
			Assert.IsNull(prop);
		}

		[Test]
		public void TargetValueCanBeObtainedForOverridenProperties()
		{
			helper.ControllerContext.PropertyBag["december"] = new December();
			helper.TextField("december.Name");
		}

//		[Test]
//		public void TargetValueCanBeObtainedForOverridenProxiedProperties()
//		{
//			ProxyGenerator generator = new ProxyGenerator();
//			object proxy = generator.CreateClassProxy(typeof(Month), new StandardInterceptor(), 12, "December");
//			helper.ControllerContext.PropertyBag["december"] = proxy;
//			helper.TextField("december.Name");
//		}

		[Test]
		public void TargetValueCanBeObtainedForOverridenGenericProperties()
		{
			ClassThatOverridesGenericProperty mr424 = new ClassThatOverridesGenericProperty();
			helper.ControllerContext.PropertyBag.Add("mr424", mr424);

			mr424.Prop = null;
			Assert.AreEqual("<input type=\"text\" id=\"mr424_prop\" name=\"mr424.prop\" value=\"(unknown)\" />",
							helper.TextField("mr424.prop"));

			mr424.Prop = "propvalue";
			Assert.AreEqual("<input type=\"text\" id=\"mr424_prop\" name=\"mr424.prop\" value=\"propvalue\" />",
							helper.TextField("mr424.prop"));
		}

		public class FormHelperEx : FormHelper
		{
			public PropertyInfo ObtainTargetProperty(RequestContext context, string target)
			{
				return ObtainTargetProperty(context, target, null);
			}
		}
        
	}

	#region Classes skeletons

	public class BaseClassWithGenericProperty<T>
	{
		private T prop;

		public virtual T Prop
		{
			get { return prop; }
			set { prop = value; }
		}
	}

	public class ClassThatOverridesGenericProperty : BaseClassWithGenericProperty<string>
	{
		public override string Prop
		{
			get { return base.Prop ?? "(unknown)"; }
			set { base.Prop = value; }
		}
	}
	public class Month
	{
		private int id;
		private String name;

		public Month(int id, string name)
		{
			this.id = id;
			this.name = name;
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}

//	public class NullInterceptor : IInterceptor
//	{
//		public object Intercept(IInvocation invocation, params object[] args)
//		{
//			return invocation.Proceed(args);
//		}
//	}

	public class December : Month
	{
		public December() : base(12, "December")
		{
		}

		public override string Name
		{
			get { return base.Name; }
			set { throw new InvalidOperationException(); }
		}
	}

	public class Subscription
	{
		private int[] months;
		private IList months2 = new ArrayList();
		private Month[] months3;
		private IList<Month> months4 = new CustomList<Month>();

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

		public IList<Month> Months4
		{
			get { return months4; }
			set { months4 = value; }
		}
	}

	public class Product
	{
		private string name;
		private int quantity;
		private bool isAvailable;
		private decimal price;
		private ProductCategory category = new ProductCategory();

		public Product()
		{
		}

		public Product(string name, int quantity, decimal price)
		{
			this.name = name;
			this.quantity = quantity;
			this.price = price;
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual int Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}

		public virtual decimal Price
		{
			get { return price; }
			set { price = value; }
		}

		public virtual bool IsAvailable
		{
			get { return isAvailable; }
			set { isAvailable = value; }
		}

		public virtual ProductCategory Category
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

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual String Name
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

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual String Name
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
			identification = id;
			this.name = name;
		}

		public virtual int Identification
		{
			get { return identification; }
			set { identification = value; }
		}

		public virtual String Name
		{
			get { return name; }
			set { name = value; }
		}
	}

	public class SimpleUser
	{
		public enum RegistrationEnum
		{
			unregistered = 1,
			pending = 2,
			registered = 6
		}

		private int id;
		private String name;
		private ArrayList roles = new ArrayList();
		private bool isActive;
		private RegistrationEnum registration = RegistrationEnum.registered;

		public SimpleUser()
		{
		}

		public SimpleUser(int id, bool isActive)
		{
			this.id = id;
			this.isActive = isActive;
		}

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

		public RegistrationEnum Registration
		{
			get { return registration; }
			set { registration = value; }
		}

		public Role[] RolesAsArray
		{
			get { return (Role[])roles.ToArray(typeof(Role)); }
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

		public virtual Month DobMonth
		{
			get { return dobMonth; }
			set { dobMonth = value; }
		}
	}

	public class CustomList<T> : IList<T>
	{
		readonly List<T> innerList = new List<T>();

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

	public interface IInterfacedList
	{
		int Id { get; set; }
		string Name { get; set; }
	}

	public class InterfacedClassA : IInterfacedList
	{
		private int id;
		private string name;

		public InterfacedClassA(int id, string name)
		{
			this.id = id;
			this.name = name;
		}

		#region IInterfacedList Members

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

		#endregion
	}

	public class InterfacedClassB : IInterfacedList
	{
		private int id;
		private string name;

		public InterfacedClassB(int id, string name)
		{
			this.id = id;
			this.name = name;
		}

		#region IInterfacedList Members

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

		#endregion
	}

	public class Key
	{
		private int id;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public Key(int id)
		{
			this.id = id;
		}
	}

	public class ClassWithCompositKey
	{
		private string name;
		private Key key;

		public ClassWithCompositKey(int id, string name)
		{
			this.name = name;
			key = new Key(id);
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public Key Key
		{
			get { return key; }
			set { key = value; }
		}
	}

	#endregion
}
