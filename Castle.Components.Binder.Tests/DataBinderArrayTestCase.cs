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
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Threading;
	using Models;
	using NUnit.Framework;

	[TestFixture]
	public class DataBinderArrayTestCase
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

		[Test]
		public void CanHandleProtoTypeSimpleArray()
		{
			string data = @"abc[]=Foo,Bar";
			NameValueCollection args = TestUtils.ParseNameValueString(data);
			object instance = binder.BindObject(typeof (string[]), "abc", builder.BuildSourceNode(args));
			Assert.IsNotNull(instance);
			var sc = instance as string[];
			Assert.IsNotNull(sc);
			Assert.AreEqual(2, sc.Length);
			Assert.AreEqual("Foo", sc[0]);
			Assert.AreEqual("Bar", sc[1]);
		}

		[Test]
		public void CanHandleProtoTypeSimpleArrayWhenItIsEmpty()
		{
			var args = new NameValueCollection();
			args.Add("abc[]", "");
			object instance = binder.BindObject(typeof (string[]), "abc", builder.BuildSourceNode(args));
			Assert.IsNotNull(instance);
			var sc = instance as string[];
			Assert.IsNotNull(sc);
			Assert.AreEqual(0, sc.Length);
		}

		[Test]
		public void CanHandleProtoTypeSimpleArrayWhenOnlyOneElementIsThere()
		{
			string data = @"abc[]=Foo";
			NameValueCollection args = TestUtils.ParseNameValueString(data);
			object instance = binder.BindObject(typeof (string[]), "abc", builder.BuildSourceNode(args));
			Assert.IsNotNull(instance);
			var sc = instance as string[];
			Assert.IsNotNull(sc);
			Assert.AreEqual(1, sc.Length);
			Assert.AreEqual("Foo", sc[0]);
		}

		[Test]
		public void CanHandleProtoTypeSimpleArrayWithoutSplitting()
		{
			var args = new NameValueCollection();
			args.Add("abc[]", "foo");
			args.Add("abc[]", "bar");
			object instance = binder.BindObject(typeof (string[]), "abc", builder.BuildSourceNode(args));
			Assert.IsNotNull(instance as string[]);
			var sc = instance as string[];
			Assert.IsNotNull(sc);
			Assert.AreEqual(2, sc.Length);
			Assert.AreEqual("foo", sc[0]);
			Assert.AreEqual("bar", sc[1]);
		}

		[Test]
		public void CanHandleProtoTypeSimpleArrayWithoutSplittingWhenEmpty()
		{
			var args = new NameValueCollection();
			args.Add("abc[]", null);
			object instance = binder.BindObject(typeof (string[]), "abc", builder.BuildSourceNode(args));
			Assert.IsNotNull(instance as string[]);
			var sc = instance as string[];
			Assert.IsNotNull(sc);
			Assert.AreEqual(0, sc.Length);
		}

		[Test]
		public void CanHandleProtoTypeSimpleArrayWithoutSplittingWithOneElement()
		{
			var args = new NameValueCollection();
			args.Add("abc[]", "foo");
			object instance = binder.BindObject(typeof (string[]), "abc", builder.BuildSourceNode(args));
			Assert.IsNotNull(instance as string[]);
			var sc = instance as string[];
			Assert.IsNotNull(sc);
			Assert.AreEqual(1, sc.Length);
			Assert.AreEqual("foo", sc[0]);
		}

		[Test]
		public void IndexedArray()
		{
			string data = @" 
				abc[0].Name = John
				abc[0].Age  = 32
				abc[1].Name = Mary
				abc[1].Age  = 16
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Person[]), "abc", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var sc = instance as Person[];

			Assert.IsNotNull(sc);
			Assert.IsTrue(sc.Length == 2);

			Assert.AreEqual(32, sc[0].Age);
			Assert.AreEqual("John", sc[0].Name);
			Assert.AreEqual(16, sc[1].Age);
			Assert.AreEqual("Mary", sc[1].Name);
		}

		[Test, Ignore("Behavior changed")] // Expected Exception
		public void InvalidData()
		{
			string data =
				@" 
				person.Months[1] = 10
				person.Months[2].Id = 20
				person.Months[3].Name = 30
				person.Months[4].Some = 40
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));
		}

		[Test]
		public void NestedArrayBinding()
		{
			string data =
				@"
				cust.addresses[0].street = st1
				cust.addresses[0].number = 1
				cust.addresses[1].street = st2
				cust.addresses[1].number = 2
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Customer), "cust", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var cust = instance as Customer;
			Assert.IsNotNull(cust);
			Assert.IsNotNull(cust.Addresses);
			Assert.AreEqual(1, cust.Addresses[0].Number);
			Assert.AreEqual(2, cust.Addresses[1].Number);
			Assert.AreEqual("st1", cust.Addresses[0].Street);
			Assert.AreEqual("st2", cust.Addresses[1].Street);
		}

		[Test]
		public void SimpleArrayDataBind()
		{
			string data =
				@" 
				Person[0].Name = John
				Person[0].Age  = 32
				Person[1].Name = Mary
				Person[1].Age  = 16
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Person[]), "Person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var sc = instance as Person[];
			Assert.IsNotNull(sc);
			Assert.IsTrue(sc.Length == 2);
			Assert.AreEqual(sc[0].Age, 32);
			Assert.AreEqual(sc[0].Name, "John");
			Assert.AreEqual(sc[1].Age, 16);
			Assert.AreEqual(sc[1].Name, "Mary");
		}

		/// <summary>
		/// Binds to array with an indexed node
		/// </summary>
		[Test]
		public void SimpleArrayDataBind2()
		{
			string data =
				@" 
				person.Months[1] = 10
				person.Months[2] = 20
				person.Months[3] = 30
				person.Months[4] = 40
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var person = instance as Person;
			Assert.IsNotNull(person);
			Assert.IsNotNull(person.Months);
			Assert.AreEqual(4, person.Months.Length);
			Assert.AreEqual(10, person.Months[0]);
			Assert.AreEqual(20, person.Months[1]);
			Assert.AreEqual(30, person.Months[2]);
			Assert.AreEqual(40, person.Months[3]);
		}

		[Test]
		public void SimpleArrayDataBind4()
		{
			string data =
				@" 
				person.Months = 10
				person.Months = 20
				person.Months = 30
				person.Months = 40
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var person = instance as Person;
			Assert.IsNotNull(person);
			Assert.IsNotNull(person.Months);
			Assert.AreEqual(4, person.Months.Length);
			Assert.AreEqual(10, person.Months[0]);
			Assert.AreEqual(20, person.Months[1]);
			Assert.AreEqual(30, person.Months[2]);
			Assert.AreEqual(40, person.Months[3]);
		}
	}
}