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

namespace Castle.MonoRail.Framework.Tests.Controllers
{
	using System.Collections;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class SmartDispatcherSimpleBindingTestCase
	{
		private StubEngineContext engineContext;
		private StubMonoRailServices services;
		private StubResponse response;
		private StubRequest request;

		[SetUp]
		public void Init()
		{
			request = new StubRequest();
			response = new StubResponse();
			services = new StubMonoRailServices();
			engineContext = new StubEngineContext(request, response, services, null);
		}

		[Test]
		public void CanInvokeWithoutParamsToFill()
		{
			SDController controller = new SDController();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "stringparam", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			controller.Process(engineContext, context);

			Assert.IsTrue(controller.parameters.Count != 0);
			Assert.IsNull(controller.parameters[0]);
		}

		[Test]
		public void CanFillSimpleNameParametersWithDataFromParams()
		{
			SDController controller = new SDController();

			request.Params.Add("name", "hammett");

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "stringparam", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			controller.Process(engineContext, context);

			Assert.IsTrue(controller.parameters.Count != 0);
			Assert.AreEqual("hammett", controller.parameters[0]);
		}

		[Test]
		public void CanFillSimpleNameParametersWithDataFromCustomParams()
		{
			SDController controller = new SDController();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "stringparam", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			context.CustomActionParameters["name"] = "hammett";

			controller.Process(engineContext, context);

			Assert.IsTrue(controller.parameters.Count != 0);
			Assert.AreEqual("hammett", controller.parameters[0]);
		}

		[Test]
		public void CustomParamsHasPrecedenceOverParams()
		{
			SDController controller = new SDController();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "stringparam", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			context.CustomActionParameters["name"] = "hammett";
			request.Params.Add("name", "john doe");

			controller.Process(engineContext, context);

			Assert.IsTrue(controller.parameters.Count != 0);
			Assert.AreEqual("hammett", controller.parameters[0]);
		}

		[Test]
		public void CanConvertSimpleParameter()
		{
			SDController controller = new SDController();

			request.Params.Add("age", "1");

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "IntParam", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			controller.Process(engineContext, context);

			Assert.IsTrue(controller.parameters.Count != 0);
			Assert.AreEqual(1, controller.parameters[0]);
		}

		#region Controllers

		class SDController : SmartDispatcherController
		{
			public ArrayList parameters = new ArrayList();

			public void StringParam(string name)
			{
				parameters.Add(name);
			}

			public void IntParam(int age)
			{
				parameters.Add(age);
			}
		}

		#endregion
	}
}
