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

namespace TestSiteNVelocity.Controllers
{
	using System;
	using Castle.MonoRail.Framework;

	[DynamicActionProvider(typeof(WizardActionProvider))]
	public class TestWizardConditionsController : Controller, IWizardController
	{
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
			return new WizardStepPage[] { new Page1() };
		}
		
		public class Page1 : WizardStepPage
		{
			protected override bool IsPreConditionSatisfied(IRailsEngineContext context)
			{
				bool isOk = Query["id"] == "1";
			
				if (!isOk)
				{
					Redirect("TestWizardConditions", "Index");
				}
			
				return isOk;
			}
		
			public void InnerAction()
			{
				Flash["InnerActionInvoked"] = true;

				RenderText("InnerAction contents");
			}

			public void InnerAction2()
			{
			}
		}
	}
}
