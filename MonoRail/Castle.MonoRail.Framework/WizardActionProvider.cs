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
		private WizardStepPage[] steps;
		private WizardStepPage currentStepInstance;

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

			steps = wizardController.GetSteps(controller.Context);

			if (steps == null || steps.Length == 0)
			{
				throw new RailsException("The controller {0} returned no WizardStepPage", 
					controller.Name);
			}

			SetUpWizardHelper(controller);

			IList stepList = new ArrayList();

			controller.DynamicActions["start"] = this;

			String action = controller.Context.UrlInfo.Action;

			String innerAction;
			String requestedAction = ObtainRequestedAction(action, out innerAction);

			foreach(WizardStepPage step in steps)
			{
				String actionName = step.ActionName;

				if (String.Compare(requestedAction, actionName, true) == 0)
				{
					currentStepInstance = step;
				}

				controller.DynamicActions[actionName] = 
					new DelegateDynamicAction(new ActionDelegate(OnStepActionRequested));

				stepList.Add(actionName);

				step.Initialize(controller);
			}

//			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			context.UnderlyingContext.Items["wizard.step.list"] = stepList;

			if (currentStepInstance != null && !HasRequiredSessionData(controller))
			{
				StartWizard(controller, false);
			}

			if (currentStepInstance != null && innerAction != null)
			{
				// Kinda hack to void to action on the wizard controller
				controller.DynamicActions[action] = 
					new DelegateDynamicAction(new ActionDelegate(EmptyAction));

				UrlInfo info = context.UrlInfo;

				currentStepInstance.Process(controller.Context, 
					controller.ServiceProvider, 
					info.Area, info.Controller, innerAction);
			}
		}

		/// <summary>
		/// Invoked as "start" action
		/// </summary>
		/// <param name="controller"></param>
		public void Execute(Controller controller)
		{
			StartWizard(controller, true);
		}

		/// <summary>
		/// Invoked when a step is accessed on the url, 
		/// i.e. http://host/mywizard/firststep.rails
		/// </summary>
		/// <param name="controller"></param>
		private void OnStepActionRequested(Controller controller)
		{
			controller.CancelView();

			IRailsEngineContext context = controller.Context;

			IWizardController wizController = (IWizardController) controller;

			// IList stepList = (IList) context.UnderlyingContext.Items["wizard.step.list"];

			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			String currentStep = (String) context.Session[wizardName + "currentstep"];

//			String doProcessFlag = context.Params["wizard.doprocess"];

			if (!wizController.OnBeforeStep(wizardName, currentStep, currentStepInstance))
			{
				return;
			}

			WizardUtils.RegisterCurrentStepInfo(controller, currentStepInstance.ActionName);

			currentStepInstance.RenderWizardView();

			wizController.OnAfterStep(wizardName, currentStep, currentStepInstance);
		}

		protected void EmptyAction(Controller controller)
		{
			controller.CancelView();
		}

		protected bool HasRequiredSessionData(Controller controller)
		{
			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			IRailsEngineContext context = controller.Context;

			return (context.Session.Contains(wizardName + "currentstepindex") &&
				context.Session.Contains(wizardName + "currentstep") );
		}

		protected void StartWizard(Controller controller, bool redirect)
		{
			ResetSteps(controller);

			IWizardController wizardController = controller as IWizardController;

			IRailsEngineContext context = controller.Context;

			IList stepList = (IList) context.UnderlyingContext.Items["wizard.step.list"];

			String firstStep = (String) stepList[0];

			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			context.Session[wizardName + "currentstepindex"] = 0;
			context.Session[wizardName + "currentstep"] = firstStep;

			wizardController.OnWizardStart();

			if (redirect)
			{
				context.Response.Redirect(controller.Name, firstStep);
			}

//			IList stepList = (IList) context.UnderlyingContext.Items["wizard.step.list"];
//
//			bool doRedirect = String.Compare((String) stepList[0], controller.Context.UrlInfo.Action, true) != 0;
//
//			String firstStep = (String) stepList[0];
//
//			context.Session[wizardName + "currentstepindex"] = 0;
//			context.Session[wizardName + "currentstep"] = firstStep;
//
//			wizardController.OnWizardStart();
//
//			if (doRedirect)
//			{
//				context.Response.Redirect(controller.Name, firstStep);
//			}
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

		private string ObtainRequestedAction(String action, out String innerAction)
		{
			innerAction = null;

			int index = action.IndexOf('-');

			if (index != -1)
			{
				innerAction = action.Substring(index + 1);

				return action.Substring(0, index);
			}

			return action;
		}

		private void SetUpWizardHelper(Controller controller)
		{
			WizardHelper helper = new WizardHelper();

			helper.SetController(controller);

			controller.Helpers["wizardhelper"] = helper;
		}
	}
}
