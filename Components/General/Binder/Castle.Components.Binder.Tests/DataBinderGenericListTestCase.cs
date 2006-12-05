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
#if DOTNET2
	
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Threading;
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
		public void SimpleBind()
		{
			string data = @"
				months[0] = 1
				months[1] = 2
				months[2] = 3
				months[3] = 4
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof(List<int>), "months", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			List<int> result = instance as List<int>;
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

			object instance = binder.BindObject(typeof(Customer2), "cust", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Customer2 cust = instance as Customer2;
			Assert.IsNotNull(cust);

			Assert.IsNotNull(cust.Months);
			Assert.AreEqual(4, cust.Months.Count);
			
			Assert.AreEqual(1, cust.Months[0]);
			Assert.AreEqual(2, cust.Months[1]);
			Assert.AreEqual(3, cust.Months[2]);
			Assert.AreEqual(4, cust.Months[3]);
		}
		
		[Test]
		public void NestedBind()
		{
			string data = @"
				cust.addresses[0].street = st1
				cust.addresses[0].number = 1
				cust.addresses[1].street = st2
				cust.addresses[1].number = 2
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			object instance = binder.BindObject(typeof(Customer2), "cust", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Customer2 cust = instance as Customer2;
			Assert.IsNotNull(cust);
			Assert.IsNotNull(cust.Addresses);
			Assert.AreEqual(2, cust.Addresses.Count);
			Assert.AreEqual(1, cust.Addresses[0].Number);
			Assert.AreEqual(2, cust.Addresses[1].Number);
			Assert.AreEqual("st1", cust.Addresses[0].Street);
			Assert.AreEqual("st2", cust.Addresses[1].Street);
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

			object instance = binder.BindObject(typeof(Customer2), "cust", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Customer2 cust = instance as Customer2;
			Assert.IsNotNull(cust);

			Assert.IsNotNull(cust.Years);
			Assert.AreEqual(4, cust.Years.Count);

			Assert.AreEqual(2006, cust.Years[0]);
			Assert.AreEqual(2007, cust.Years[1]);
			Assert.AreEqual(2008, cust.Years[2]);
			Assert.AreEqual(2009, cust.Years[3]);
		}
	}

	class Customer2
	{
		private List<Address> addresses;

		private List<int> months;

		private List<int> years;

		public List<Address> Addresses
		{
			get { return addresses; }
			set { addresses = value; }
		}

		public List<int> Months
		{
			get { return months; }
			set { months = value; }
		}

		public IList<int> Years
		{
			get { return years; }
			set { years = new List<int>(value); }
		}
	}
	
#endif
}
