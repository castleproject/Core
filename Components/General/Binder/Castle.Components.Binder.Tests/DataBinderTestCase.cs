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

namespace Castle.Components.Binder.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Globalization;
	using System.Text;
	using System.Threading;

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

			Thread.CurrentThread.CurrentCulture	= en;
			Thread.CurrentThread.CurrentUICulture = en;
		}

		[Test]
		public void DateTimeBind()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("person.DOBday", 1.ToString());
			args.Add("person.DOBmonth", 12.ToString());
			args.Add("person.DOByear", 2005.ToString());

			object instance = binder.BindObject(typeof(Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Person p = (Person) instance;
			Assert.AreEqual(p.DOB, new DateTime(2005, 12, 1));
		}

		[Test]
		public void ExcludingProperty()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("person.name", "john");
			args.Add("person.age", "30");
			
			object instance = binder.BindObject(typeof(Person), "person", "Name", null, builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Person person = (Person) instance;
			Assert.IsNull(person.Name);
			Assert.AreEqual(30, person.Age);
		}

		[Test]
		public void DayTooBigDateTimeBind()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("person.DOBday", 31.ToString());
			args.Add("person.DOBmonth", 2.ToString());
			args.Add("person.DOByear", 2005.ToString());

			object instance = binder.BindObject(typeof(Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Person person = (Person) instance;
			Assert.AreEqual(1, binder.ErrorList.Count);
		}

		[Test]
		public void InvalidDateTimeBind()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("person.DOBday", 32.ToString());
			args.Add("person.DOBmonth", 2.ToString());
			args.Add("person.DOByear", 2005.ToString());

			binder.BindObject(typeof(Person), "person", builder.BuildSourceNode(args));
			Assert.AreEqual(1, binder.ErrorList.Count);
		}

		[Test]
		public void InvalidDateTime2Bind()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("person.DOBday", (-2).ToString());
			args.Add("person.DOBmonth", 2.ToString());
			args.Add("person.DOByear", 2005.ToString());
			
			binder.BindObject(typeof(Person), "person", builder.BuildSourceNode(args));
			Assert.AreEqual(1, binder.ErrorList.Count);
		}

		[Test]
		public void SimpleDataBind()
		{
			String name = "John";
			int age = 32;
			decimal assets = 100000;
			NameValueCollection args = new NameValueCollection();

			args.Add("Person.Name", name);
			args.Add("Person.Age", age.ToString());
			args.Add("Person.Assets", assets.ToString());
			
			object instance = binder.BindObject(typeof(Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Person person = instance as Person;
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
			NameValueCollection args = new NameValueCollection();

			args.Add("Test.Person.Name", name);
			args.Add("Test.Person.Age", age.ToString());
			args.Add("Test.Person.Assets", assets.ToString());
			
			object instance = binder.BindObject(typeof(Person), "test.person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Person person = instance as Person;
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
			NameValueCollection args = new NameValueCollection();

			args.Add("Person.Name", name);
			args.Add("Person.Age", age.ToString());
			args.Add("Person.Assets", assets.ToString());
			
			object instance = binder.BindObject(typeof(Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Person person = instance as Person;
			Assert.IsNotNull(person);
			Assert.AreEqual(person.Age, age);
			Assert.AreEqual(person.Name, name);
			Assert.AreEqual(person.Assets, assets);
		}

		[Test]
		public void NestedBinding()
		{
			string data = @"
				cust.Name = John
				cust.CustId = 1
				cust.address.street = r p l
				cust.address.number = 44
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);
			
			object instance = binder.BindObject(typeof(Customer), "cust", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Customer cust = instance as Customer;
			Assert.IsNotNull(cust);
			Assert.IsTrue(cust.Name == "John");
			Assert.AreEqual(1, cust.CustId);
			Assert.IsNotNull(cust.Address);
			Assert.AreEqual("r p l", cust.Address.Street);
			Assert.AreEqual(44, cust.Address.Number);
		}
		
		[Test]
		public void SimpleDataBindWithErrors()
		{
			string data = @"
				Person.Name = John
				Person.Age = Thirty Two?
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);
			
			object instance = binder.BindObject(typeof(Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Person person = instance as Person;
			Assert.IsNotNull(person);
			Assert.IsTrue(person.Name == "John");
			Assert.IsTrue(person.Age == 0);
			Assert.AreEqual(1, binder.ErrorList.Count);
			
			data = @"
				Person.Name = John
				Person.Age = " + long.MaxValue;

			args = TestUtils.ParseNameValueString(data);
			
			instance = binder.BindObject(typeof(Person), "person", builder.BuildSourceNode(args));

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
			NameValueCollection args = new NameValueCollection();

			args.Add("Comp.Type1", "validvalue");

			Comp instance = (Comp) binder.BindObject(typeof(Comp), "Comp", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Assert.IsNotNull(instance.Type1);
			Assert.IsNull(instance.Type2);

			args = new NameValueCollection();

			args.Add("Comp.Nonsense", "validvalue");

			binder = new DataBinder();
			instance = (Comp) binder.BindObject(typeof(Comp), "Comp", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Assert.IsNull(instance.Type1);
			Assert.IsNull(instance.Type2);
		}
	}

	#region Class Helpers
	
	class Address
	{
		private String street;
		private short number;
		private String state, city, countrycode;

		public string Street
		{
			get { return street; }
			set { street = value; }
		}

		public short Number
		{
			get { return number; }
			set { number = value; }
		}

		public string State
		{
			get { return state; }
			set { state = value; }
		}

		public string City
		{
			get { return city; }
			set { city = value; }
		}

		public string Countrycode
		{
			get { return countrycode; }
			set { countrycode = value; }
		}
	}
	
	class Customer : Person
	{
		private int custId;
		private Address address = new Address();
		private Address[] addresses;

		public int CustId
		{
			get { return custId; }
			set { custId = value; }
		}

		public Address Address
		{
			get { return address; }
		}

		public Address[] Addresses
		{
			get { return addresses; }
			set { addresses = value; }
		}
	}

	class Person
	{
		private string name;
		private Int32 age;
		private Decimal assets;
		private DateTime dob;
		private int[] months;

		public String Name
		{
			get { return name; }
			set { name = value; }
		}

		public Int32 Age
		{
			get { return age; }
			set { age = value; }
		}

		public Decimal Assets
		{
			get { return assets; }
			set { assets = value; }
		}

		public DateTime DOB
		{
			get { return dob; }
			set { dob = value; }
		}

		public int[] Months
		{
			get { return months; }
			set { months = value; }
		}
	}

	class Person2
	{
		private IList months;

		public IList Months
		{
			get { return months; }
			set { months = value; }
		}
	}

	class Comp
	{
		CustomType type1;
		CustomType type2;

		public CustomType Type1
		{
			get { return type1; }
			set { type1 = value; }
		}

		public CustomType Type2
		{
			get { return type2; }
			set { type2 = value; }
		}
	}

	[TypeConverter(typeof(TypeConverterHelper))]
	class CustomType
	{
	}

	[TypeConverter(typeof(TypeConverterHelper2))]
	class CustomType2
	{
	}

	class TypeConverterHelper : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value.ToString() == "validvalue")
			{
				return new CustomType();
			}
			else
			{
				throw new Exception("Invalid Value");
			}
		}
	}

	class TypeConverterHelper2 : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return false;
		}
	}
	
	#endregion
}
