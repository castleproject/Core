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
	using Framework.Helpers;
	using Framework.Services;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class MR_ISSUE_535
	{
		[Test]
		public void UrlHelper_ButtonLink_Should_JavaScript_Escape_URLs()
		{
			var controllerContext = new ControllerContext();
			var engineContext = new StubEngineContext { CurrentControllerContext = controllerContext, UrlInfo = new UrlInfo("", "home", "index", "", "") };

			var helper = new UrlHelper(engineContext);
			helper.UrlBuilder = new DefaultUrlBuilder(new StubServerUtility(), null);
			helper.UrlBuilder.UseExtensions = false;
			controllerContext.Helpers.Add(helper);
			
			Assert.AreEqual("<button type=\"button\" onclick=\"javascript:window.location.href = '/MyController/MyAction?ProductName=Jack\\'s+Mine'\">MyButton</button>", helper.ButtonLink("MyButton", DictHelper.CreateN("controller", "MyController").N("action", "MyAction").N("queryString", "ProductName=Jack's Mine")));
		}
	}
}
