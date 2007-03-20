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

	/// <summary>
	/// This fixture asserts that Poco are accept as controllers, although
	/// this use presents some design differences
	/// </summary>
	[TestFixture, Category(TestCategories.Core)]
	public class PocoControllerSupportTestCase
	{
		private PocoController controller;
		private IExecutionContext testContext;

		[SetUp]
		public void CreateObjects()
		{
			controller = new PocoController();
			testContext = new TestContext(new UrlInfo("area", "controller", "index"));
		}

		[Test]
		public void ExecutorDoesNotFailToAcceptPoco()
		{
			new ControllerExecutor(controller, testContext);

			// How Can I write an assert for that?
		}

		[Test]
		public void ActionIsSelectedAndRun()
		{
			ControllerExecutor executor = new ControllerExecutor(controller, testContext);

			ActionExecutor actionExec = executor.SelectAction();

			Assert.IsNotNull(actionExec);
			Assert.AreEqual(ActionType.Method, actionExec.ActionType);
			Assert.AreEqual("Index", actionExec.Name);

			executor.Execute(actionExec);

			Assert.IsTrue(controller.wasRun);
		}

		public class PocoController
		{
			public bool wasRun;

			public void Index()
			{
				wasRun = true;
			}
		}
	}
}
