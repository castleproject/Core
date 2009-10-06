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
// 
namespace Castle.Components.Binder.Tests
{
	using System;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Threading;
	using Models;
	using NUnit.Framework;

	[TestFixture]
	public class DataBinderTestCase
	{
		private IDataBinder binder;
		private TreeBuilder builder;

		[TestFixtureSetUp]
		public void Init()
		{
			binder = new DataBinder();
			builder = new TreeBuilder();

			CultureInfo en = CultureInfo.CreateSpecificCulture("en");

			Thread.CurrentThread.CurrentCulture = en;
			Thread.CurrentThread.CurrentUICulture = en;
		}

		private static void AssertExcludingProperty(object instance)
		{
			Assert.IsNotNull(instance);
			var person = (Person) instance;
			Assert.IsNull(person.Name);
			Assert.AreEqual(30, person.Age);
		}

		private static void AssertExcludingNestedObjectProperty(object instance)
		{
			Assert.IsNotNull(instance);
			var cust = instance as Customer;
			Assert.IsNotNull(cust);
			Assert.IsTrue(cust.Name == "John");
			Assert.AreEqual(1, cust.CustId);
			Assert.IsNotNull(cust.Address);
			Assert.IsNull(cust.Address.Street);
			Assert.AreEqual(44, cust.Address.Number);
		}

		[Test]
		public void DateTimeBind()
		{
			var args = new NameValueCollection();

			args.Add("person.DOBday", 1.ToString());
			args.Add("person.DOBmonth", 12.ToString());
			args.Add("person.DOByear", 2005.ToString());

			object instance = binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var p = (Person) instance;
			Assert.AreEqual(new DateTime(2005, 12, 1), p.DOB);

			args.Clear();
			args.Add("person.DOBday", 2.ToString());
			args.Add("person.DOBmonth", 1.ToString());
			args.Add("person.DOByear", 2005.ToString());

			instance = binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			p = (Person) instance;
			Assert.AreEqual(new DateTime(2005, 1, 2), p.DOB);
		}

		[Test]
		public void DayTooBigDateTimeBind()
		{
			var args = new NameValueCollection();

			args.Add("person.DOBday", 31.ToString());
			args.Add("person.DOBmonth", 2.ToString());
			args.Add("person.DOByear", 2005.ToString());

			object instance = binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var person = (Person) instance;
			Assert.AreEqual(1, binder.ErrorList.Count);
		}

		[Test]
		public void ExcludingNestedObjectProperty()
		{
			string data =
				@"
				cust.Name = John
				cust.CustId = 1
				cust.address.street = r p l
				cust.address.number = 44
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Customer), "cust", "cust.address.street", null,
			                                    builder.BuildSourceNode(args));

			AssertExcludingNestedObjectProperty(instance);

			instance = binder.BindObject(typeof (Customer), "cust", null,
			                             "cust.address, cust.address.number, cust.CustId, cust.Name",
			                             builder.BuildSourceNode(args));

			AssertExcludingNestedObjectProperty(instance);
		}

		[Test]
		public void ExcludingProperty()
		{
			var args = new NameValueCollection();

			args.Add("person.name", "john");
			args.Add("person.age", "30");

			object instance = binder.BindObject(typeof (Person), "person", "person.Name", null, builder.BuildSourceNode(args));

			AssertExcludingProperty(instance);

			instance = binder.BindObject(typeof (Person), "person", null, "person.Age", builder.BuildSourceNode(args));

			AssertExcludingProperty(instance);
		}

		[Test]
		public void InvalidDateTime2Bind()
		{
			var args = new NameValueCollection();

			args.Add("person.DOBday", (-2).ToString());
			args.Add("person.DOBmonth", 2.ToString());
			args.Add("person.DOByear", 2005.ToString());

			binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));
			Assert.AreEqual(1, binder.ErrorList.Count);
		}

		[Test]
		public void InvalidDateTimeBind()
		{
			var args = new NameValueCollection();

			args.Add("person.DOBday", 32.ToString());
			args.Add("person.DOBmonth", 2.ToString());
			args.Add("person.DOByear", 2005.ToString());

			binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));
			Assert.AreEqual(1, binder.ErrorList.Count);
		}

		[Test]
		public void NestedBinding()
		{
			string data =
				@"
				cust.Name = John
				cust.CustId = 1
				cust.address.street = r p l
				cust.address.number = 44
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Customer), "cust", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var cust = instance as Customer;
			Assert.IsNotNull(cust);
			Assert.IsTrue(cust.Name == "John");
			Assert.AreEqual(1, cust.CustId);
			Assert.IsNotNull(cust.Address);
			Assert.AreEqual("r p l", cust.Address.Street);
			Assert.AreEqual(44, cust.Address.Number);
		}

		[Test]
		public void SimpleDataBind()
		{
			String name = "John";
			int age = 32;
			decimal assets = 100000;
			var args = new NameValueCollection();

			args.Add("Person.Name", name);
			args.Add("Person.Age", age.ToString());
			args.Add("Person.Assets", assets.ToString());

			object instance = binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var person = instance as Person;
			Assert.IsNotNull(person);
			Assert.AreEqual(person.Age, age);
			Assert.AreEqual(person.Name, name);
			Assert.AreEqual(person.Assets, assets);
		}

		[Test]
		public void SimpleDataBindDotPath()
		{
			String name = "John";
			int age = 32;
			decimal assets = 100000;
			var args = new NameValueCollection();

			args.Add("Test.Person.Name", name);
			args.Add("Test.Person.Age", age.ToString());
			args.Add("Test.Person.Assets", assets.ToString());

			object instance = binder.BindObject(typeof (Person), "test.person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var person = instance as Person;
			Assert.IsNotNull(person);
			Assert.AreEqual(person.Age, age);
			Assert.AreEqual(person.Name, name);
			Assert.AreEqual(person.Assets, assets);
		}

		[Test]
		public void SimpleDataBindWithEmptyField()
		{
			String name = "";
			int age = 32;
			decimal assets = 100000;
			var args = new NameValueCollection();

			args.Add("Person.Name", name);
			args.Add("Person.Age", age.ToString());
			args.Add("Person.Assets", assets.ToString());

			object instance = binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var person = instance as Person;
			Assert.IsNotNull(person);
			Assert.AreEqual(person.Age, age);
			Assert.AreEqual(person.Name, null);
			Assert.AreEqual(person.Assets, assets);
		}

		[Test]
		public void SimpleDataBindWithErrors()
		{
			string data = @"
				Person.Name = John
				Person.Age = Thirty Two?
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var person = instance as Person;
			Assert.IsNotNull(person);
			Assert.IsTrue(person.Name == "John");
			Assert.IsTrue(person.Age == 0);
			Assert.AreEqual(1, binder.ErrorList.Count);

			data = @"
				Person.Name = John
				Person.Age = " + long.MaxValue;

			args = TestUtils.ParseNameValueString(data);

			instance = binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			person = instance as Person;
			Assert.IsNotNull(person);
			Assert.IsTrue(person.Name == "John");
			Assert.IsTrue(person.Age == 0);
			Assert.AreEqual(1, binder.ErrorList.Count);
		}

		[Test]
		public void TypeConverterBinding()
		{
			var args = new NameValueCollection();

			args.Add("Comp.Type1", "validvalue");

			var instance = (Comp) binder.BindObject(typeof (Comp), "Comp", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Assert.IsNotNull(instance.Type1);
			Assert.IsNull(instance.Type2);

			args = new NameValueCollection();

			args.Add("Comp.Nonsense", "validvalue");

			binder = new DataBinder();
			instance = (Comp) binder.BindObject(typeof (Comp), "Comp", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Assert.IsNull(instance.Type1);
			Assert.IsNull(instance.Type2);
		}
	}

	#region Class Helpers

	#endregion
}