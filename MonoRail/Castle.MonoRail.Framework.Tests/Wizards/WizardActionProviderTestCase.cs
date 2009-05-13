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

namespace Castle.MonoRail.Framework.Tests.Wizards
{
	using System;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class WizardActionProviderTestCase
	{
		#region Setup/Teardown

		[SetUp]
		public void Init()
		{
			request = new StubRequest();
			response = new StubResponse();
			services = new StubMonoRailServices();
			engStubViewEngineManager = new StubViewEngineManager();
			services.ViewEngineManager = engStubViewEngineManager;
			engineContext = new StubEngineContext(request, response, services, null);
			actionProvider = new WizardActionProvider();
		}

		#endregion

		private StubEngineContext engineContext;
		private StubViewEngineManager engStubViewEngineManager;
		private StubMonoRailServices services;
		private StubRequest request;
		private StubResponse response;
		private WizardActionProvider actionProvider;

		private class NotAWizardController : Controller
		{
		}

		private class WizardWithNoSteps : Controller, IWizardController
		{
			#region IWizardController Members

			public void OnWizardStart()
			{
				throw new NotImplementedException();
			}

			public bool OnBeforeStep(string wizardName, string stepName, IWizardStepPage step)
			{
				throw new NotImplementedException();
			}

			public void OnAfterStep(string wizardName, string stepName, IWizardStepPage step)
			{
				throw new NotImplementedException();
			}

			public IWizardStepPage[] GetSteps(IEngineContext context)
			{
				return null;
			}

			public bool UseCurrentRouteForRedirects
			{
				get { throw new NotImplementedException(); }
			}

			#endregion
		}

		[Test,
		 ExpectedException(typeof(MonoRailException),
		 	ExpectedMessage = "The controller home must implement the interface IWizardController to be used as a wizard")]
		public void RejectsControllerThatDoesNotImplementIWizardController()
		{
			NotAWizardController controller = new NotAWizardController();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			actionProvider.IncludeActions(engineContext, controller, context);
		}

		[Test,
		 ExpectedException(typeof(MonoRailException), ExpectedMessage = "The controller home returned no WizardStepPage")]
		public void ThrowsExceptionIfNoStepsAreReturned()
		{
			WizardWithNoSteps controller = new WizardWithNoSteps();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			actionProvider.IncludeActions(engineContext, controller, context);
		}
	}
}
