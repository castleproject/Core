// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Tests
{
	using Castle.MonoRail.Test;
	using NUnit.Framework;

	[TestFixture, Category(TestCategories.Core)]
	public class ControllerExecutorTestCase
	{
		private SimpleController controller;
		private IExecutionContext testContext;

		[SetUp]
		public void CreateObjects()
		{
			controller = new SimpleController();
			testContext = new TestContext(new UrlInfo("area", "controller", "index"));
		}

		[Test]
		public void ControllerStateIsProperlyInitialized()
		{
			new ControllerExecutor(controller, testContext);

			Assert.AreEqual("area", controller.AreaName);
			Assert.AreEqual("controller", controller.Name);
			Assert.AreEqual("index", controller.ActionName);
		}

		[Test]
		public void ActionIsSelectedBasedOnUrlActionPiece()
		{
			ControllerExecutor executor = new ControllerExecutor(controller, testContext);

			ActionExecutor actionExec = executor.SelectAction();

			Assert.IsNotNull(actionExec);
			Assert.AreEqual(ActionType.Method, actionExec.ActionType);
			Assert.AreEqual("Index", actionExec.Name);
		}

		[Test]
		public void ActionIsExecuted()
		{
			ControllerExecutor executor = new ControllerExecutor(controller, testContext);

			executor.Execute(executor.SelectAction());

			Assert.IsTrue(controller.wasRun, "ControllerExecutor failed to execute the action");
		}

		public class SimpleController : Controller
		{
			public bool wasRun;

			public void Index()
			{
				wasRun = true;
			}
		}
	}
}
