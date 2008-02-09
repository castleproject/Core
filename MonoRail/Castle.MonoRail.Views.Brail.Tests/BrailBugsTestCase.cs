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
	using Castle.MonoRail.Views.Brail.TestSite.Controllers;
	using NUnit.Framework;

	[TestFixture]
	public class BrailBugsTestCase : BaseViewOnlyTestFixture
	{
		[Test]
		public void MR_233_TagInDoubleQuotes()
		{
			PropertyBag["page_id"] = 123;
			ProcessView_StripRailsExtension("bugs/mr_233.rails");
			AssertReplyEqualTo("<body id=\"123\">");
		}

		[Test]
		public void MR_294_CaptureForInSubViewDoesNotPropogateUpward()
		{
			ProcessView_StripRailsExtension("bugs/MR_294_CaptureForInSubViewDoesNotPropogateUpward.rails");
			AssertReplyEqualTo("ayende");
		}

		[Test]
		public void MR_371_OutputComponentInSectionTooManyTimes()
		{
			ViewComponentFactory.Inspect(typeof(BugsController).Assembly);
			ProcessView_StripRailsExtension("bugs/mr_371.rails");
			AssertReplyEqualTo("123ayende 0<br/>123ayende 1<br/>123ayende 2<br/>");
		}

		[Test]
		public void MR_262_DynamicComponents()
		{
			ViewComponentFactory.Inspect(typeof(BugsController).Assembly);
			 PropertyBag["components"] = new string[]
                {
                    "SimpleInlineViewComponent3", 
                    "SimpleInlineViewComponent2"
                };
			ProcessView_StripRailsExtension("usingcomponents/DynamicComponents.rails");
			AssertReplyEqualTo("default component view picked up automaticallyThis is a view used by a component");
		}

		[Test]
		public void MR_285_ViewName_Is_Reserved_CompilerKeyword()
		{
			ProcessView_StripRailsExtension("bugs/add.rails");
			AssertReplyContains("Success");
		}


	    [Test]
		public void MR_299_Inline_SubView()
		{
			ProcessView_StripRailsExtension("bugs/inlineSubView.rails");
			AssertReplyContains("Success");
		}

	    [Test]
	    public void MS_378_AccessingIndexers()
	    {
	        PropertyBag["Data"] = new Data();
	        ProcessView("bugs/mr_378");
	    }

        public class Data
        {
            public string[] array = new string[] { "one", "two", "three" };

            public string[] Items
            {
                get { return array; }
            }
        } 
	}
}