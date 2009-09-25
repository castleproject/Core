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

namespace Castle.MonoRail.Framework.Tests.Bugs
{
	using System;
	using Framework.Helpers;
	using NUnit.Framework;

	[TestFixture]
	public class MR_ISSUE_547
	{
		[Test]
		public void HtmlHelper_DateTime_Should_Generate_Increasing_Tab_Index_For_Select_Boxes()
		{
			HtmlHelper helper = new HtmlHelper();
			string result = helper.DateTime("dt", new DateTime(2010, 10, 1),
				DictHelper.Create("tabindex=1"));

			Assert.IsTrue(result.Contains("tabindex=\"1\""));
			Assert.IsTrue(result.Contains("tabindex=\"2\""));
			Assert.IsTrue(result.Contains("tabindex=\"3\""));
			Assert.IsFalse(result.Contains("tabindex=\"4\""));
		}
	}
}
