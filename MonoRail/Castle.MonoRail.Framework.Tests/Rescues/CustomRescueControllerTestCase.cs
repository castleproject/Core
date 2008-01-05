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

namespace Castle.MonoRail.Framework.Tests.Rescues
{
	using System;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class CustomRescueControllerTestCase
	{
		private MockEngineContext engineContext;
		private ViewEngineManagerStub viewEngStub;
		private MockServices services;

		[SetUp]
		public void Init()
		{
			MockRequest request = new MockRequest();
			MockResponse response = new MockResponse();
			services = new MockServices();
			viewEngStub = new ViewEngineManagerStub();
			services.ViewEngineManager = viewEngStub;
			engineContext = new MockEngineContext(request, response, services, null);
		}

		[Test]
		public void ExecutesCustomController()
		{
			ControllerWithCustomRescue controller = new ControllerWithCustomRescue();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			controller.Process(engineContext, context);

			Assert.AreEqual("CustomRescue\\RescueMe", viewEngStub.TemplateRendered);
		}

		[Test]
		public void ExecutesCustomControllerThatImplementsIRescueController()
		{
			ControllerWithCustomRescue2 controller = new ControllerWithCustomRescue2();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			controller.Process(engineContext, context);

			Assert.AreEqual("CustomRescueControllerWithInterface\\Rescue", viewEngStub.TemplateRendered);
		}

		#region Controllers

		[Rescue(typeof(CustomRescueController), "RescueMe")]
		class ControllerWithCustomRescue : Controller
		{
			public void Index()
			{
				throw new InvalidOperationException();
			}
		}

		[Rescue(typeof(CustomRescueControllerWithInterface))]
		class ControllerWithCustomRescue2 : Controller
		{
			public void Index()
			{
				throw new InvalidOperationException();
			}
		}

		class CustomRescueControllerWithInterface : SmartDispatcherController, IRescueController
		{
			public void Rescue(Exception exception, IController controller, IControllerContext controllerContext)
			{
			}
		}

		class CustomRescueController : Controller
		{
			public void RescueMe()
			{
			}
		}

		#endregion
	}
}
