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
	using System;
	using System.Globalization;
	using System.IO;
	using System.Threading;

	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Tests.Controllers;
	
	using NUnit.Framework;

	[TestFixture]
	public class FormHelperCheckboxFieldListTestCase
	{
		class MockClass
		{
			private int[] values;
			
			public int[] Values
			{
				get { return values; }
				set { values = value; }
			}
		}
		
		private FormHelper helper;
		private Product product;
		private SimpleUser user;
		private Subscription subscription;
		private Month[] months;
		private MockClass mock;

		[SetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture("en");

			Thread.CurrentThread.CurrentCulture	= en;
			Thread.CurrentThread.CurrentUICulture = en;

			helper = new FormHelper();

			subscription = new Subscription();
			mock = new MockClass();
			months = new Month[] {new Month(1, "January"), new Month(1, "February")};
			product = new Product("memory card", 10, (decimal) 12.30);
			user = new SimpleUser();
			mock.Values = new int[] { 2, 3 };

			HomeController controller = new HomeController();

			controller.PropertyBag.Add("product", product);
			controller.PropertyBag.Add("user", user);
			controller.PropertyBag.Add("roles", new Role[] { new Role(1, "a"), new Role(2, "b"), new Role(3, "c") });
			controller.PropertyBag.Add("sendemail", true);
			controller.PropertyBag.Add("confirmation", "abc");
			controller.PropertyBag.Add("fileaccess", FileAccess.Read);
			controller.PropertyBag.Add("subscription", subscription);
			controller.PropertyBag.Add("months", months);
			controller.PropertyBag.Add("mock", mock);

			helper.SetController(controller);
		}

		/// <summary>
		/// Code posted on the mailing list
		/// </summary>
		[Test]
		public void BugReport1()
		{
			FormHelper.CheckboxList list = 
				helper.CreateCheckboxList("mock.Values", new int[] {1,2,3,4});
			
			Assert.IsNotNull(list);
			
			int index = 0;
			
			foreach(Object item in list)
			{
				String content = list.Item();
				if (index == 1 || index == 2)
				{
					Assert.AreEqual("<input type=\"checkbox\" id=\"mock_Values_" + index + 
						"_\" name=\"mock.Values[" + index + "]\" value=\"" + item + "\" checked=\"checked\" />", content);
				}
				else
				{
					Assert.AreEqual("<input type=\"checkbox\" id=\"mock_Values_" + index + 
						"_\" name=\"mock.Values[" + index + "]\" value=\"" + item + "\" />", content);
				}
				index++;
			}

		}
		
		/// <summary>
		/// Tests the subscription.Months as null and an int array as source
		/// </summary>
		[Test]
		public void CheckboxFieldList1()
		{
			subscription.Months = null;
			
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
				helper.CreateCheckboxList("subscription.Months2", months, DictHelper.Create("value=id", "suffix=Id"));
			
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
		/// Tests the subscription.Months2 as null and using a <c>Month</c> array as data source
		/// </summary>
		[Test]
		public void CheckboxFieldList3a()
		{
			subscription.Months3 = null;
			
			FormHelper.CheckboxList list = 
				helper.CreateCheckboxList("subscription.Months3", months, DictHelper.Create("value=id", "suffix=Id"));
			
			Assert.IsNotNull(list);

			int index = 0;

			foreach(Month item in list)
			{
				String content = list.Item();
				Assert.AreEqual("<input type=\"checkbox\" id=\"subscription_Months3_" + index + 
					"_\" name=\"subscription.Months3[" + index + "].Id\" value=\"" + item.Id + "\" />", content);
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
	}
}
