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

namespace Castle.MonoRail.Framework.Tests.Services
{
	using Castle.MonoRail.Framework.Services.AjaxProxyGenerator;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class PrototypeAjaxProxyGeneratorTestCase
	{
		private PrototypeAjaxProxyGenerator generator;
		private StubEngineContext engineContext;
		private StubViewEngineManager engStubViewEngineManager;
		private StubResponse response;

		[SetUp]
		public void Init()
		{
			StubRequest request = new StubRequest();
			response = new StubResponse();
			StubMonoRailServices services = new StubMonoRailServices();
			engStubViewEngineManager = new StubViewEngineManager();
			services.ViewEngineManager = engStubViewEngineManager;
			engineContext = new StubEngineContext(request, response, services, new UrlInfo("area", "controller", "action"));

			generator = new PrototypeAjaxProxyGenerator();
			generator.ControllerTree = services.ControllerTree;
			generator.ControllerDescriptorBuilder = services.ControllerDescriptorProvider;

			services.ControllerTree.AddController("area", "controller1", typeof(NoAjaxController));
			services.ControllerTree.AddController("", "controller2", typeof(AjaxController));
		}

		[Test]
		public void GenerateJSProxy_DoesNotGenerateAnythingForControllerThatHasNoAjaxAction()
		{
			string js = generator.GenerateJSProxy(engineContext, "proxyName", "area", "controller1");

			Assert.AreEqual("<script type=\"text/javascript\">\r\n" +
				"var proxyName =\r\n{};\r\n</script>", js);
		}

		[Test]
		public void GenerateJSProxy_GeneratesProxyOnlyForAjaxActions()
		{
			string js = generator.GenerateJSProxy(engineContext, "proxyName", "", "controller2");

			Assert.AreEqual("<script type=\"text/javascript\">\r\n" +
				"var proxyName =\r\n{\r\n\t" + 

				"action1: function(callback)\r\n\t" + 
				"{\r\n\t\t" +
				"var r=new Ajax.Request('/controller2/Action1.', " +
				"{method: 'get', asynchronous: !!callback, onComplete: callback, parameters: '_='}); \r\n\t\t" +
				"if(!callback) return r.transport.responseText;\r\n\t}\r\n,\r\n\t" +
				
				"action2: function(name, age, callback)\r\n\t{\r\n\t\t" +
				"var r=new Ajax.Request('/controller2/Action2.', {method: 'post', asynchronous: !!callback, onComplete: callback, " +
				"parameters: '_=&name='+name+'&age='+age+''}); \r\n\t\t" + 
				"if(!callback) return r.transport.responseText;\r\n\t}\r\n" +
				",\r\n" +
				"\tactionReturnJSON: function(, callback)\r\n" +
				"\t{\r\n" +
				"\t\tvar r=new Ajax.Request('/controller2/ActionReturnJSON.', {method: 'get', asynchronous: !!callback, onComplete: callback, parameters: '_=&='+Object.toJSON(test)+''}); \r\n" +
				"\t\tif(!callback) return r.transport.responseText;\r\n" +
				"\t}\r\n" +
				"};\r\n" +
				"</script>", js);
		}

		#region Controllers

		class NoAjaxController : Controller
		{
			public void Index()
			{
			}
		}

		class AjaxController : Controller
		{
			public void Index()
			{
			}

			[AjaxAction]
			public void Action1()
			{
			}

			[AjaxAction, AccessibleThrough(Verb.Post)]
			public void Action2(string name, int age)
			{
			}

			[AjaxAction]
			public void ActionReturnJSON([JSONBinder]string test) {
			}

		}

		#endregion
	}
}
