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
	using System.Collections;

	using NUnit.Framework;

	[TestFixture]
	public class BrailLayoutsTestCase_TestSite : BaseViewOnlyTestFixture
	{
		[Test]
		public void CanUseLayoutThatIsNotInLayoutsFolder()
		{
			Layout = "/layoutable/notInLayouts";
			ProcessView_StripRailsExtension("layoutable/CustomLayoutLocation.rails");
			AssertReplyEqualTo("not in layouts");
		}
	}

	[TestFixture]
	public class BrailLayoutsTestCase_TestViews : BaseViewOnlyTestFixture
	{
		public BrailLayoutsTestCase_TestViews()
			: base(ViewLocations.BrailTestsView)
		{

		}

		[Test]
		public void CanUseNestedViews()
		{
			Layouts = new string[] { "/layouts/master", "/layouts/secondary" };

			string view = ProcessView("home/index");
			const string expected = @"Master Layout Header
Secondary Layout Header
Brail is wonderful
Secondary Layout Footer
Master Layout Footer";
			Assert.AreEqual(expected, view);
		}
	}
}
