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
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using Castle.MonoRail.Views.Brail.TestSite.Controllers;
	using Framework;
	using NUnit.Framework;

	[TestFixture]
	public class BrailSubViewTestCase : BaseViewOnlyTestFixture
	{
		private void SwitchViewImplementation()
		{
			Hashtable hashtable1 = (Hashtable) typeof (BooViewEngine).GetField("compilations", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(BooViewEngine);
			Hashtable hashtable2 = (Hashtable) typeof (BooViewEngine).GetField("constructors", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(BooViewEngine);
			hashtable1[@"subview\listItem.brail"] = typeof (DummySubView);
			Type[] typeArray1 = new Type[] {typeof (BooViewEngine), typeof (TextWriter), typeof (IEngineContext), typeof (Controller), typeof (IControllerContext)};
			hashtable2[typeof (DummySubView)] = typeof (DummySubView).GetConstructor(typeArray1);
		}

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

			SwitchViewImplementation();
			// if it was cached, we should get something else
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
			Layout = "master";
			ProcessView_StripRailsExtension("subview/index.rails");
			string expected = "\r\nWelcome!\r\n<p>View With SubView Content\r\nFrom SubView</p>\r\nFooter";
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void SubViewWithParameters()
		{
			ProcessView_StripRailsExtension("subview/CallSubViewWithParameters.rails");
			string expected = "View SubView Content With Parameters\r\nMonth: 0\r\nAllow Select: False";
			AssertReplyEqualTo(expected);
		}
	}
}