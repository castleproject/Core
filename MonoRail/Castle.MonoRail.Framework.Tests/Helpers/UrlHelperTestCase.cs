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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System.Collections.Generic;
	using Framework.Services;
	using NUnit.Framework;
	using Framework.Helpers;
	using Test;

	[TestFixture]
	public class UrlHelperTestCase
	{
		private UrlHelper helper;

		[SetUp]
		public void Setup()
		{
			var controllerContext = new ControllerContext();
			var engineContext = new StubEngineContext { CurrentControllerContext = controllerContext, UrlInfo = new UrlInfo("", "home", "index", "", "") };

			helper = new UrlHelper(engineContext) { UrlBuilder = new DefaultUrlBuilder() };
			helper.UrlBuilder.UseExtensions = false;
			controllerContext.Helpers.Add(helper);
		}

		[Test]
		public void Create_ButtonLink_With_Attributes_Should_Not_Generate_Incorrect_HTML()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("action", "MyAction");
			parameters.Add("controller", "MyController");

			Dictionary<string, string> buttonAttributes = new Dictionary<string, string>();
			buttonAttributes.Add("class", "MyClass");

			Assert.AreEqual("<button type=\"button\" class=\"MyClass\" onclick=\"javascript:window.location.href = '/MyController/MyAction'\">Test</button>", helper.ButtonLink("Test", parameters, buttonAttributes));
		}

		[Test]
		public void Create_ButtonLink_Without_Attributes_Should_Not_Generate_Incorrect_HTML()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("action", "MyAction");
			parameters.Add("controller", "MyController");

			Assert.AreEqual("<button type=\"button\" onclick=\"javascript:window.location.href = '/MyController/MyAction'\">Test</button>", helper.ButtonLink("Test", parameters));
		}
	}
}