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
	using System;
	using System.Globalization;
	using System.Threading;
	
	using NUnit.Framework;

	[TestFixture]
	public class BrailHelperTestCase : BaseViewOnlyTestFixture
	{
		[Test]
		public void DictHelperUsage()
		{
			ProcessView_StripRailsExtension("helper/DictHelperUsage.rails");
			string expected =
				"<input type=\"text\" name=\"name\" id=\"name\" value=\"value\" size=\"30\" maxlength=\"20\" style=\"something\" eol=\"true\" />";
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void InheritedHelpers()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-us");
			ProcessView_StripRailsExtension("helper/inheritedhelpers.rails");
			string expected = "Date formatted " + new DateTime(1979, 7, 16).ToShortDateString();
			AssertReplyEqualTo(expected);
		}
	}
}