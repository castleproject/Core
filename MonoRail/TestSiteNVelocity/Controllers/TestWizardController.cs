// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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


	[DynamicActionProvider( typeof(WizardActionProvider) )]
	public class TestWizardController : Controller, IWizardController
	{
		// wizard unrelated action
		public void Index()
		{
			
		}

		public WizardStepPage[] GetSteps(IRailsEngineContext context)
		{
			return new WizardStepPage[] { new Page1(), new Page2() };
		}
	}


	public class Page1 : WizardStepPage
	{
		protected override bool Process()
		{
			Flash["ProcessInvoked"] = true;

			String name = Params["name"];

			if (name == null || name.Length == 0)
			{
				return false;
			}

			return true;
		}
	}

	public class Page2 : WizardStepPage
	{
		protected override void Show()
		{
			RenderText("A content rendered using RenderText");
		}
	}
}
