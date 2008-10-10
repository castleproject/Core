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

namespace Castle.MonoRail.Views.Brail.Tests
{
	using Castle.MonoRail.Views.Brail.TestSite.Components;
	using NUnit.Framework;

	[TestFixture]
	public class BrailRegressionTestFixture : BaseViewOnlyTestFixture
	{
		public BrailRegressionTestFixture()
			: base(ViewLocations.BrailTestsView)
		{

		}

	    [Test]
	    public void WillPropagateNullParameter()
	    {
	        PropertyBag["items"] = new Item[]
	                                   {
	                                       new Item("Item 1", null),
	                                       new Item("Item 2", new Item("SubItem 2.1", null)),
	                                   };
			ProcessView("regressions/NullPropagationTest");
            AssertReplyContains("SubItem 2.1");
	    }

		[Test]
		public void WillPropagateNullParameterInBraces()
		{
			PropertyBag["items"] = new Item[]
	                                   {
	                                       new Item("Item 1", null),
	                                       new Item("Item 2", new Item("SubItem 2.1", null)),
	                                   };
			ProcessView("regressions/NullPropagationWithBracesTest");
			AssertReplyContains("SubItem 2.1");
		}

		[Test]
		public void WillPropagateNullParameter_WithNullInArrayMiddle()
		{
			PropertyBag["items"] = new Item[]
	                                   {
	                                       new Item("Item 1", null),
										   null,
	                                       new Item("Item 1", new Item("SubItem 2.1", null)),
	                                   };
			ProcessView("regressions/NullPropagationTest");
			AssertReplyContains("SubItem 2.1");
		}

		[Test]
		public void WillPassRoutingInfoToView()
		{
			PropertyBag["myVariable"] = "Hello";
			ControllerContext.CustomActionParameters["id"] = 5;
			ProcessView("regressions/routingInfo");
			AssertReplyEqualTo("id = 5");
		}

		[Test]
		public void UnlessWithIsDefine()
		{
			PropertyBag["isWeb"] = true;
			ProcessView("regressions/UnlessWithIsDefine");
			AssertReplyEqualTo("");

			PropertyBag.Remove("isWeb");
			ProcessView("regressions/UnlessWithIsDefine");
			AssertReplyEqualTo("isWeb is defined");

		}

		[Test]
		public void CanCompareToNullableParameter()
		{
            PropertyBag["myVariable"] = "Hello";
			ProcessView("regressions/CanCompareToNullableParameter");
			AssertReplyEqualTo("Eq");
		}

		[Test]
		public void CanUseQuestionMarkOperatorInIfStatementToValidatePresenceOfParameter()
		{
			ProcessView("regressions/questionMarkOp_if");
            AssertReplyEqualTo("");
		}

		[Test]
		public void CanUseQuestionMarkOperatorInIfStatementToValidatePresenceOfParameter_WhenPassed()
		{
            PropertyBag["errorMsg"] = "Hello"; 
			ProcessView("regressions/questionMarkOp_if_whenPassed");
            AssertReplyEqualTo("Hello");
		}

		[Test]
		public void CanUseQuestionMarkOperatorInIfStatementToValidatePresenceOfParameter_WhenExists()
		{
			PropertyBag["Errors"] = new string[] {"Hello",}; 
			ProcessView("regressions/questionMarkOp_if_when_exists");
            AssertReplyEqualTo("Hello<br />\r\n");
		}

		
		[Test]
		public void CanUseQuestionMarkOperatorInIfStatementToValidatePresenceOfParameter_WhenMissing()
		{
			ProcessView("regressions/questionMarkOp_if_when_missing");
		    AssertReplyEqualTo("");
		}

		[Test]
		public void CanUseQuestionMarkOperatorInUnlessStatementToValidatePresenceOfParameter()
		{
			string view = ProcessView("regressions/questionMarkOp_unless");
			Assert.AreEqual("\r\nError does not exist\r\n", view);
		}

        [Test]
        public void CanUseQuestionMarkOperatorInUnlessStatementToValidatePresenceOfParameter_whenPassed()
        {
			
            PropertyBag["errorMsg"] = "Hello";
            string view = ProcessView("regressions/questionMarkOp_unless_whenPassed");
            Assert.AreEqual("", view);
        }


		[Test]
		public void HtmlEncodingStringInterpolation()
		{
            PropertyBag["htmlCode"] = "<script>alert('a');</script>";
			string view = ProcessView("regressions/HtmlEncodingStringInterpolation");
			Assert.AreEqual("&lt;script&gt;alert('a');&lt;/script&gt;", view);
		}

		[Test]
		public void StringInterpolationInCodeBlockWillNotBeEscaped()
		{
            PropertyBag["htmlCode"] = "<script>alert('a');</script>";
			string view = ProcessView("regressions/StringInterpolationInCodeBlockWillNotBeEscaped");
			Assert.AreEqual("<script>alert('a');</script>", view);
		}

		[Test]
		public void CanUseViewFromResource()
		{
			string view = ProcessView("login/welcome");
			Assert.AreEqual("Hi there, anonymous user!", view);
		}

		[Test]
		public void MR_415_CaptureForInNestedComponentsLoosesItsContextVars()
		{
			this.ViewComponentFactory.Inspect(typeof(ComponentUsingCaptureFor).Assembly);
			PropertyBag["HelpMessage"] = "SOS";
			string view = ProcessView("bugs/mr_415");
			Assert.AreEqual(2, CountOccurancesOf("<div class=\"tooltip_content\">SOS</div>", view));
		}

		[Test]
		public void CanUseIgnoreNullInsideCaptureFor()
		{
			string view = ProcessView("regressions/UsingIgnoreNullWithinCaptureFor");
			Assert.AreEqual(@"

	<h2 id=""H1"">Create new webcast</h2>


Hibernating Rhinos 
	
		- Create new webcast
	
", view);
		}

		private static int CountOccurancesOf(string stringOccuring, string view)
		{
			int count = 0;
			int index = 0;
			while((index = view.IndexOf(stringOccuring, index))!=-1)
			{
				index++;
				count++;
			}
			return count;
		}

        private class Item
        {
            public string Name;
            public Item SubItem;

            public Item(string name, Item subItem)
            {
                this.Name = name;
                this.SubItem = subItem;
            }
        }

	}
}
