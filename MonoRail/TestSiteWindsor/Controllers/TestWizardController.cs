// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace TestSiteWindsor.Controllers
{
	using System;

	using Castle.MicroKernel;
	using Castle.MonoRail.Framework;


	[DynamicActionProvider( typeof(WizardActionProvider) ), Serializable]
	public class TestWizardController : Controller, IWizardController
	{
		private readonly IKernel kernel;

		public TestWizardController(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public void Index()
		{
			RenderText("Hello!");
		}

		public void OnWizardStart()
		{
		}

		public bool OnBeforeStep(String wizardName, String stepName, WizardStepPage step)
		{
			return true;
		}

		public void OnAfterStep(String wizardName, String stepName, WizardStepPage step)
		{
		}

		public WizardStepPage[] GetSteps(IRailsEngineContext context)
		{
			return new WizardStepPage[]
			{
				(WizardStepPage) kernel[typeof(Page1)],
				(WizardStepPage) kernel[typeof(Page2)],
				(WizardStepPage) kernel[typeof(Page3)],
				(WizardStepPage) kernel[typeof(Page4)],
			};
		}
	}

	public class Page1 : WizardStepPage
	{
		public void InnerAction()
		{
			Flash["InnerActionInvoked"] = true;

			RenderText("InnerAction contents");
		}

		public void InnerAction2()
		{
		}
	}

	public class Page2 : WizardStepPage
	{
		protected override void RenderWizardView()
		{
			RenderText("A content rendered using RenderText");
		}
	}

	public class Page3 : WizardStepPage
	{
		protected override void RenderWizardView()
		{
			RenderView("page3");
		}
	}

	public class Page4 : WizardStepPage
	{
		public void InnerAction()
		{
			Flash["InnerActionInvoked"] = true;

			DoNavigate();
		}
	}
}
