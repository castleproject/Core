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

namespace Castle.MonoRail.Framework.Tests.MockObjectTests
{
	using Castle.MonoRail.Framework.Test;
	using NUnit.Framework;
	using Castle.MonoRail.TestSupport;
	
	[TestFixture]
	public class StubEngineContextTest : BaseControllerTest
	{
		private DummyController controller;

		[Test]
		public void AddMailTemplateRendered_Expect_A_Copy_Of_Properties()
		{
			controller = new DummyController();
			PrepareController(controller);
			controller.Context.Services.EmailTemplateService = new StubEmailTemplateService((StubEngineContext)controller.Context);
			string templateName = "welcome";

			controller.PropertyBag["Value"] = "One";
			controller.RenderMailMessage(templateName, null, controller.PropertyBag);
			controller.PropertyBag["Value"] = "Two";
			controller.RenderMailMessage(templateName, null, controller.PropertyBag); 

			Assert.AreEqual(RenderedEmailTemplates[0].Parameters["Value"], "One");
			Assert.AreEqual(RenderedEmailTemplates[1].Parameters["Value"], "Two");
		}

		private class DummyController : Controller {}
	}
}
