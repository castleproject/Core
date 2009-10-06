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
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Threading;
	using Models;
	using NUnit.Framework;

	[TestFixture]
	public class DataBinderGenericListTestCase
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
		public void IListBind()
		{
			string data = @"
				cust.years = 2006
				cust.years = 2007
				cust.years = 2008
				cust.years = 2009
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Customer2), "cust", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var cust = instance as Customer2;
			Assert.IsNotNull(cust);

			Assert.IsNotNull(cust.Years);
			Assert.AreEqual(4, cust.Years.Count);

			Assert.AreEqual(2006, cust.Years[0]);
			Assert.AreEqual(2007, cust.Years[1]);
			Assert.AreEqual(2008, cust.Years[2]);
			Assert.AreEqual(2009, cust.Years[3]);
		}

		[Test]
		public void IListNestedBind()
		{
			string data =
				@"
				cust.addresses2[0].street = st1
				cust.addresses2[0].number = 1
				cust.addresses2[1].street = st2
				cust.addresses2[1].number = 2
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Customer2), "cust", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var cust = instance as Customer2;
			Assert.IsNotNull(cust);
			Assert.IsNotNull(cust.Addresses2);
			Assert.AreEqual(2, cust.Addresses2.Count);
			Assert.AreEqual(1, cust.Addresses2[0].Number);
			Assert.AreEqual(2, cust.Addresses2[1].Number);
			Assert.AreEqual("st1", cust.Addresses2[0].Street);
			Assert.AreEqual("st2", cust.Addresses2[1].Street);
		}

		[Test]
		public void InstantiatedListNestedBind()
		{
			string data =
				@"
				cust.addresses1[0].street = st1
				cust.addresses1[0].number = 1
				cust.addresses1[1].street = st2
				cust.addresses1[1].number = 2
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Customer2), "cust", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var cust = instance as Customer2;
			Assert.IsNotNull(cust);
			Assert.IsNotNull(cust.Addresses1);
			Assert.AreEqual(2, cust.Addresses1.Count);
			Assert.AreEqual(1, cust.Addresses1[0].Number);
			Assert.AreEqual(2, cust.Addresses1[1].Number);
			Assert.AreEqual("st1", cust.Addresses1[0].Street);
			Assert.AreEqual("st2", cust.Addresses1[1].Street);
		}

		[Test]
		public void NestedBind()
		{
			string data =
				@"
				cust.addresses[0].street = st1
				cust.addresses[0].number = 1
				cust.addresses[1].street = st2
				cust.addresses[1].number = 2
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Customer2), "cust", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var cust = instance as Customer2;
			Assert.IsNotNull(cust);
			Assert.IsNotNull(cust.Addresses);
			Assert.AreEqual(2, cust.Addresses.Count);
			Assert.AreEqual(1, cust.Addresses[0].Number);
			Assert.AreEqual(2, cust.Addresses[1].Number);
			Assert.AreEqual("st1", cust.Addresses[0].Street);
			Assert.AreEqual("st2", cust.Addresses[1].Street);
		}

		[Test]
		public void ShouldBindNullableListEvenWithEmptyParams()
		{
			var args = new NameValueCollection
			           	{
			           		{"months", "1"},
			           		{"months", "2"},
			           		{"months", ""},
			           		{"months", ""}
			           	};
			object instance = binder.BindObject(typeof (List<int?>), "months", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var list = instance as List<int?>;
			Assert.IsNotNull(list);

			Assert.AreEqual(2, list.Count);

			Assert.AreEqual(1, list[0]);
			Assert.AreEqual(2, list[1]);
		}

		[Test]
		public void ShouldBindNullablePrimitiveList()
		{
			var args = new NameValueCollection
			           	{
			           		{"months[0]", "1"},
			           		{"months[1]", "2"},
			           		{"months[2]", ""},
			           		{"months[3]", ""}
			           	};

			object instance = binder.BindObject(typeof (List<int?>), "months", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var result = instance as List<int?>;
			Assert.IsNotNull(result);

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);

			Assert.AreEqual(1, result[0]);
			Assert.AreEqual(2, result[1]);
		}

		[Test]
		public void ShouldBindPrimitiveListEvenWithEmptyParams()
		{
			var args = new NameValueCollection
			           	{
			           		{"months", "1"},
			           		{"months", "2"},
			           		{"months", ""},
			           		{"months", ""}
			           	};
			object instance = binder.BindObject(typeof (List<int>), "months", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var list = instance as List<int>;
			Assert.IsNotNull(list);

			Assert.AreEqual(2, list.Count);

			Assert.AreEqual(1, list[0]);
			Assert.AreEqual(2, list[1]);
		}

		[Test]
		public void ShouldBindPrimitiveListWithEmptyParams()
		{
			var args = new NameValueCollection
			           	{
			           		{"months[0]", "1"},
			           		{"months[1]", "2"},
			           		{"months[2]", ""},
			           		{"months[3]", ""}
			           	};

			object instance = binder.BindObject(typeof (List<int>), "months", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var result = instance as List<int>;
			Assert.IsNotNull(result);

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);

			Assert.AreEqual(1, result[0]);
			Assert.AreEqual(2, result[1]);
		}

		[Test]
		public void SimpleBind()
		{
			string data = @"
				months[0] = 1
				months[1] = 2
				months[2] = 3
				months[3] = 4
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (List<int>), "months", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var result = instance as List<int>;
			Assert.IsNotNull(result);

			Assert.IsNotNull(result);
			Assert.AreEqual(4, result.Count);

			Assert.AreEqual(1, result[0]);
			Assert.AreEqual(2, result[1]);
			Assert.AreEqual(3, result[2]);
			Assert.AreEqual(4, result[3]);
		}

		[Test]
		public void SimpleBind2()
		{
			string data = @"
				cust.months = 1
				cust.months = 2
				cust.months = 3
				cust.months = 4
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof (Customer2), "cust", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			var cust = instance as Customer2;
			Assert.IsNotNull(cust);

			Assert.IsNotNull(cust.Months);
			Assert.AreEqual(4, cust.Months.Count);

			Assert.AreEqual(1, cust.Months[0]);
			Assert.AreEqual(2, cust.Months[1]);
			Assert.AreEqual(3, cust.Months[2]);
			Assert.AreEqual(4, cust.Months[3]);
		}
	}

	internal class Customer2
	{
		private List<Address> addresses1 = new List<Address>();

		private List<int> years;

		public List<Address> Addresses { get; set; }

		public List<Address> Addresses1
		{
			get { return addresses1; }
			set { addresses1 = value; }
		}

		public IList<Address> Addresses2 { get; set; }

		public List<int> Months { get; set; }

		public IList<int> Years
		{
			get { return years; }
			set { years = new List<int>(value); }
		}
	}
}