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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;

	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Provide easy to use Wizard-like support.
	/// </summary>
	/// <remarks>
	/// We use the DynamicAction infrastructure to provide 
	/// wizard support. By doing we dont get force 
	/// the programmer to extend from a specific Controller 
	/// which can be quite undesirable in common business projects
	/// situations. 
	/// <para>
	/// Nevetheless we do require that the programmer 
	/// implements <see cref="IWizardController"/> on the wizard controller.
	/// </para>
	/// </remarks>
	public class WizardActionProvider : IDynamicActionProvider, IDynamicAction
	{
		public WizardActionProvider()
		{
		}

		public void IncludeActions(Controller controller)
		{
			IRailsEngineContext context = controller.Context;

			IWizardController wizardController = controller as IWizardController;

			if (wizardController == null)
			{
				throw new RailsException("The controller {0} must implement the interface " + 
					"IWizardController to be used as a wizard", controller.Name);
			}

			WizardStepPage[] steps = wizardController.GetSteps(controller.Context);

			if (steps == null || steps.Length == 0)
			{
				throw new RailsException("The controller {0} returned no WizardStepPage ", 
					controller.Name);
			}

			SetUpWizardHelper(controller);

			IList stepList = new ArrayList();

			controller.CustomActions["start"] = this;

			foreach(WizardStepPage step in steps)
			{
				String actionName = step.ActionName;

				controller.CustomActions[actionName] = step;

				stepList.Add(actionName);
			}

			context.UnderlyingContext.Items["wizard.step.list"] = stepList;

			if (!HasRequiredSessionData(controller))
			{
				StartWizard(controller);
			}
		}

		/// <summary>
		/// Invoked as "start" action
		/// </summary>
		/// <param name="controller"></param>
		public void Execute(Controller controller)
		{
			StartWizard(controller);
		}

		protected bool HasRequiredSessionData(Controller controller)
		{
			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			IRailsEngineContext context = controller.Context;

			return (context.Session.Contains(wizardName + "currentstepindex") &&
				context.Session.Contains(wizardName + "currentstep") );
		}

		protected void StartWizard(Controller controller)
		{
			ResetSteps(controller);

			IRailsEngineContext context = controller.Context;

			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			IList stepList = (IList) context.UnderlyingContext.Items["wizard.step.list"];

			String firstStep = (String) stepList[0];

			context.Session[wizardName + "currentstepindex"] = 0;
			context.Session[wizardName + "currentstep"] = firstStep;

			context.Response.Redirect(controller.Name, firstStep);
		}

		protected void ResetSteps(Controller controller)
		{
			IWizardController wizardController = controller as IWizardController;

			WizardStepPage[] steps = wizardController.GetSteps(controller.Context);

			foreach(WizardStepPage step in steps)
			{
				step.Reset();
			}
		}

		private void SetUpWizardHelper(Controller controller)
		{
			WizardHelper helper = new WizardHelper();

			helper.SetController(controller);

			controller.Helpers["wizardhelper"] = helper;
		}
	}
}
