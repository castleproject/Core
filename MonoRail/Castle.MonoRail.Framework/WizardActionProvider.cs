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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Descriptors;

	/// <summary>
	/// Provide easy to use Wizard-like support.
	/// </summary>
	/// <seealso cref="IWizardController"/>
	/// <remarks>
	/// MonoRail uses the DynamicAction infrastructure to provide 
	/// wizard support so we dont force 
	/// the programmer to inherit from a specific Controller 
	/// which can be quite undesirable in real world projects. 
	/// <para>
	/// Nevertheless we do require that the programmer 
	/// implements <see cref="IWizardController"/> on the wizard controller.
	/// </para>
	/// </remarks>
	public class WizardActionProvider : IDynamicActionProvider, IDynamicAction
	{
		private IWizardStepPage[] steps;
		private IWizardStepPage currentStepInstance;
		private String rawAction;
		private String innerAction;
		private String requestedAction;
		private UrlInfo urlInfo;

		/// <summary>
		/// Implementation of IDynamicActionProvider.
		/// <para>
		/// Grab all steps related to the wizard
		/// and register them as dynamic actions.
		/// </para>
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">Wizard controller (must implement <see cref="IWizardController"/></param>
		/// <param name="controllerContext">The controller context.</param>
		public void IncludeActions(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			// Primordial assert

			IWizardController wizardController = controller as IWizardController;

			if (wizardController == null)
			{
				throw new MonoRailException("The controller {0} must implement the interface " +
					"IWizardController to be used as a wizard", controllerContext.Name);
			}
			
			// Grab all Wizard Steps

			steps = wizardController.GetSteps(engineContext);

			if (steps == null || steps.Length == 0)
			{
				throw new MonoRailException("The controller {0} returned no WizardStepPage", controllerContext.Name);
			}

			List<string> stepList = new List<string>();
			
			// Include the "start" dynamic action, which resets the wizard state

			controllerContext.DynamicActions["start"] = this;

			// Find out the action request (and possible inner action)
			//   Each action will be a step name, or maybe the step name + action (ie Page1-Save)
			
			urlInfo = engineContext.UrlInfo;

			rawAction = urlInfo.Action;

			requestedAction = ObtainRequestedAction(rawAction, out innerAction);

			// If no inner action was found, fallback to 'RenderWizardView'

			if (string.IsNullOrEmpty(innerAction))
			{
				innerAction = "RenderWizardView";
			}

			engineContext.Items["wizard.step.list"] = stepList;

			SetUpWizardHelper(engineContext, controller, controllerContext);

			// Initialize all steps and while we are at it, 
			// discover the current step
			
			foreach(IWizardStepPage step in steps)
			{
				string actionName = step.ActionName;

				step.WizardController = wizardController;
				step.WizardControllerContext = controllerContext;
				
				if (string.Compare(requestedAction, actionName, true) == 0)
				{
					currentStepInstance = step;

					if (innerAction != null)
					{
						// If there's an inner action, we invoke it as a step too
						controllerContext.DynamicActions[rawAction] = new DelegateDynamicAction(OnStepActionRequested);
					}
					
					engineContext.CurrentController = step;
				}
				else
				{
					controllerContext.DynamicActions[actionName] = new DelegateDynamicAction(OnStepActionRequested);
				}

				stepList.Add(actionName);
			}

			SetUpWizardHelper(engineContext, controller, controllerContext);
		}

		/// <summary>
		/// Invoked as "start" action
		/// </summary>
		/// <returns></returns>
		public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			StartWizard(engineContext, controller, controllerContext, true);
			return null;
		}

		/// <summary>
		/// Invoked when a step is accessed on the url,
		/// i.e. http://host/mywizard/firststep.rails and
		/// when an inner action is invoked like http://host/mywizard/firststep-save.rails
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		private object OnStepActionRequested(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			if (currentStepInstance != null && !HasRequiredSessionData(engineContext, controller, controllerContext))
			{
				StartWizard(engineContext, controller, controllerContext, false);
			}

			controllerContext.SelectedViewName = null;

			IWizardController wizController = (IWizardController) controller;

			String wizardName = WizardUtils.ConstructWizardNamespace(controllerContext);
			String currentStep = (String) engineContext.Session[wizardName + "currentstep"];

			// If OnBeforeStep returns false we stop
			if (!wizController.OnBeforeStep(wizardName, currentStep, currentStepInstance))
			{
				return null;
			}

			ControllerMetaDescriptor stepMetaDescriptor =
				engineContext.Services.ControllerDescriptorProvider.BuildDescriptor(currentStepInstance);

			// Record the step we're working with
			WizardUtils.RegisterCurrentStepInfo(engineContext, controller, controllerContext, currentStepInstance.ActionName);

			// The step cannot be accessed in the current state of matters
			if (!currentStepInstance.IsPreConditionSatisfied(engineContext))
			{
				return null;
			}

			try
			{
				IControllerContext stepContext =
					engineContext.Services.ControllerContextFactory.Create(
						controllerContext.AreaName, controllerContext.Name, innerAction, 
						stepMetaDescriptor, controllerContext.RouteMatch);
				stepContext.PropertyBag = controllerContext.PropertyBag;

				SetUpWizardHelper(engineContext, currentStepInstance, stepContext);

				currentStepInstance.Process(engineContext, stepContext);

				return null;
			}
			finally
			{
				wizController.OnAfterStep(wizardName, currentStep, currentStepInstance);
			}
		}

		/// <summary>
		/// Represents an empty (no-op) action.
		/// </summary>
		/// <param name="controller">The controller.</param>
		protected void EmptyAction(Controller controller)
		{
			controller.CancelView();
		}

		/// <summary>
		/// Determines whether all wizard specific information is on the user session.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns>
		/// 	<c>true</c> if has session data; otherwise, <c>false</c>.
		/// </returns>
		protected bool HasRequiredSessionData(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			String wizardName = WizardUtils.ConstructWizardNamespace(controllerContext);

			return (engineContext.Session.Contains(wizardName + "currentstepindex") &&
					engineContext.Session.Contains(wizardName + "currentstep"));
		}

		/// <summary>
		/// Starts the wizard by adding the required information to the
		/// session and invoking <see cref="IWizardController.OnWizardStart"/>
		/// and detecting the first step.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="redirect">if set to <c>true</c>, a redirect
		/// will be issued to the first step.</param>
		protected void StartWizard(IEngineContext engineContext, IController controller, IControllerContext controllerContext, bool redirect)
		{
			ResetSteps(engineContext, controller);

			IWizardController wizardController = (IWizardController) controller;

			IList stepList = (IList) engineContext.Items["wizard.step.list"];

			String firstStep = (String) stepList[0];

			String wizardName = WizardUtils.ConstructWizardNamespace(controllerContext);

			engineContext.Session[wizardName + "currentstepindex"] = 0;
			engineContext.Session[wizardName + "currentstep"] = firstStep;

			wizardController.OnWizardStart();

			if (redirect)
			{
				engineContext.Response.Redirect(controllerContext.AreaName, controllerContext.Name, firstStep);
			}
		}

		/// <summary>
		/// Resets the steps by invoking <see cref="IWizardStepPage.Reset"/>
		/// on all steps instances.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		protected void ResetSteps(IEngineContext engineContext, IController controller)
		{
			IWizardController wizardController = (IWizardController) controller;

			IWizardStepPage[] steps = wizardController.GetSteps(engineContext);

			foreach(IWizardStepPage step in steps)
			{
				step.Reset();
			}
		}

		private String ObtainRequestedAction(String action, out String innerAction)
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

		private void SetUpWizardHelper(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			if (controller == null) return;

			WizardHelper helper = new WizardHelper();

			helper.SetContext(engineContext);
			helper.SetController(controller, controllerContext);

			controllerContext.Helpers.Add(helper);
		}
	}
}
