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

namespace WizardSampleSite.Controllers
{
	using System;

	using Castle.MonoRail.Framework;

	[DynamicActionProvider( typeof(WizardActionProvider) )]
	public class SimpleWizardController : Controller, IWizardController
	{
		public WizardStepPage[] GetSteps(IRailsEngineContext context)
		{
			return new WizardStepPage[]
				{
					new IntroductionStep(), new MainInfoStep(), 
					new SubscribeStep(), new ConfirmationStep(), 
					new ResultStep()
				};
		}
	}

	/// <summary>
	/// Presents a small introduction
	/// </summary>
	class IntroductionStep : WizardStepPage
	{
	}

	class MainInfoStep : WizardStepPage
	{
		protected override bool Process()
		{
			return true;
		}

		/// <summary>
		/// Note that you can override 
		/// the Show method and render a different 
		/// view (the default behavior is to render 
		/// the step name, i.e. MainInfoStep)
		/// </summary>
		protected override void Show()
		{
			base.Show();
		}
	}

	class SubscribeStep : WizardStepPage
	{
	}

	class ConfirmationStep : WizardStepPage
	{
	}

	class ResultStep : WizardStepPage
	{
	}
}
