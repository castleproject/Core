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
	using System.Text;
	
	using NUnit.Framework;

	[TestFixture]
	public class BrailSubViewTestCase : BaseViewOnlyTestFixture
	{
		[Test]
		public void BrailWillCacheSubViewsWhenUsingForwardSlash()
		{
			ProcessView_StripRailsExtension("subview/useLotsOfSubViews.rails");
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 50; i++)
			{
				sb.Append("real");
			}
			string expected = sb.ToString();
			AssertReplyEqualTo(expected);

			// Here we tell the controller to replace the subview with a dummy implementation
			// if brail doesn't cache it, it will use the real implementation, and the test will fail.
			sb = new StringBuilder();
			for (int i = 0; i < 50; i++)
			{
				sb.Append("dummy");
			}
			ProcessView_StripRailsExtension("subview/useLotsOfSubViews.rails");
			expected = sb.ToString();
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void CanCallSubViews()
		{
			ProcessView_StripRailsExtension("subview/index.rails");
			string expected = "View With SubView Content\r\nFrom SubView";
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void CanCallSubViewWithPath()
		{
			ProcessView_StripRailsExtension("subview/SubViewWithPath.rails");
			string expected = "View With SubView Content\r\nContents for heyhello View";
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void SubViewWithLayout()
		{
			ProcessView_StripRailsExtension("subview/SubViewWithLayout.rails");
			string expected = "\r\nWelcome!\r\n<p>View With SubView Content\r\nFrom SubView</p>\r\nFooter";
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void SubViewWithParameters()
		{
			ProcessView_StripRailsExtension("subview/SubViewWithParameters.rails");
			string expected = "View SubView Content With Parameters\r\nMonth: 0\r\nAllow Select: False";
			AssertReplyEqualTo(expected);
		}
	}
}