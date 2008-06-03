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

namespace Castle.MonoRail.Framework.Tests.Actions
{
	using NUnit.Framework;
	using Castle.MonoRail.Framework.Test;

	[TestFixture]
	public class DynamicActionExecutorTestCase
	{
		[Test]
		public void DelegatesToDynamicAction()
		{
			ActionStub dynAction = new ActionStub();

			DynamicActionExecutor executor = new DynamicActionExecutor(dynAction);

			StubRequest req = new StubRequest();
			StubResponse res = new StubResponse();
			StubMonoRailServices services = new StubMonoRailServices();
			IEngineContext engineContext = new StubEngineContext(req, res, services, new UrlInfo("area", "controller", "action"));

			object retVal = executor.Execute(engineContext, new DummyController(), new ControllerContext());
			Assert.IsTrue(dynAction.WasExecuted);
			Assert.AreEqual(3, retVal);
		}

		public class ActionStub : IDynamicAction
		{
			private bool wasExecuted;

			public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
			{
				wasExecuted = true;
				return 3;
			}

			public bool WasExecuted
			{
				get { return wasExecuted; }
			}
		}

		public class DummyController : Controller
		{
		}
	}
}
