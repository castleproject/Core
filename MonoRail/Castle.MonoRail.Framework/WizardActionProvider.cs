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
	/// wizard support as we dont force 
	/// the programmer to inherit from a specific Controller 
	/// which can be quite undesirable in common business projects
	/// situations. 
	/// <para>
	/// Nevertheless we do require that the programmer 
	/// implements <see cref="IWizardController"/> on the wizard controller.
	/// </para>
	/// </remarks>
	public class WizardActionProvider : IDynamicActionProvider, IDynamicAction
	{
		private WizardStepPage[] steps;
		private WizardStepPage currentStepInstance;
		private IControllerLifecycleExecutor currentStepExecutor;
		private String rawAction;
		private String innerAction;
		private String requestedAction;
		private UrlInfo urlInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="WizardActionProvider"/> class.
		/// </summary>
		public WizardActionProvider()
		{
		}

		/// <summary>
		/// Implementation of IDynamicActionProvider.
		/// <para>
		/// Grab all steps related to the wizard 
		/// and register them as dynamic actions.
		/// </para>
		/// </summary>
		/// <param name="controller">Wizard controller (must implement <see cref="IWizardController"/></param>
		public void IncludeActions(Controller controller)
		{
			IRailsEngineContext context = controller.Context;
			
			// Primordial assert

			IWizardController wizardController = controller as IWizardController;

			if (wizardController == null)
			{
				throw new RailsException("The controller {0} must implement the interface " + 
					"IWizardController to be used as a wizard", controller.Name);
			}
			
			// Grab all Wizard Steps

			steps = wizardController.GetSteps(controller.Context);

			if (steps == null || steps.Length == 0)
			{
				throw new RailsException("The controller {0} returned no WizardStepPage", controller.Name);
			}

			IList stepList = new ArrayList();
			
			// Include the "start" dynamic action, which resets the wizard state

			controller.DynamicActions["start"] = this;

			// Find out the action request (and possible inner action)
			//   Each action will be a step name, or maybe the step name + action (ie Page1-Save)
			
			urlInfo = controller.Context.UrlInfo;

			rawAction = urlInfo.Action;

			requestedAction = ObtainRequestedAction(rawAction, out innerAction);

			// If no inner action was found, fallback to 'RenderWizardView'
			
			if (innerAction == null || innerAction == String.Empty)
			{
				innerAction = "RenderWizardView";
			}
			
			IControllerLifecycleExecutorFactory execFactory = 
				(IControllerLifecycleExecutorFactory) context.GetService(typeof(IControllerLifecycleExecutorFactory));

			// Initialize all steps and while we are at it, 
			// discover the current step
			
			foreach(WizardStepPage step in steps)
			{
				String actionName = step.ActionName;
				
				if (String.Compare(requestedAction, actionName, true) == 0)
				{
					currentStepInstance = step;

					if (innerAction != null)
					{
						// If there's an inner action, we invoke it as a step too
						controller.DynamicActions[rawAction] = 
							new DelegateDynamicAction(new ActionDelegate(OnStepActionRequested));
					}
				}
				
				controller.DynamicActions[actionName] = 
					new DelegateDynamicAction(new ActionDelegate(OnStepActionRequested));

				stepList.Add(actionName);

				IControllerLifecycleExecutor stepExec = execFactory.CreateExecutor(step, context);
				stepExec.InitializeController(controller.AreaName, controller.Name, innerAction);
				step.Initialize(controller);
				
				if (currentStepInstance == step)
				{
					currentStepExecutor = stepExec;

					if (!stepExec.SelectAction(innerAction, controller.Name))
					{
						stepExec.PerformErrorHandling();
							
						return;
					}
					
					if (!stepExec.RunStartRequestFilters())
					{
						return;
					}
				}
			}

			context.UnderlyingContext.Items["wizard.step.list"] = stepList;

			SetUpWizardHelper(controller);
			SetUpWizardHelper(currentStepInstance);
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
		/// i.e. http://host/mywizard/firststep.rails and 
		/// when an inner action is invoked like http://host/mywizard/firststep-save.rails
		/// </summary>
		/// <param name="controller"></param>
		private void OnStepActionRequested(Controller controller)
		{
			if (currentStepInstance != null && !HasRequiredSessionData(controller))
			{
				StartWizard(controller, false);
			}
			
			controller.CancelView();

			IRailsEngineContext context = controller.Context;

			IWizardController wizController = (IWizardController) controller;

			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			String currentStep = (String) context.Session[wizardName + "currentstep"];

			// The step will inherit the controller property bag,
			// this way filters can pass values to the step property without having to know it
			currentStepInstance.PropertyBag = controller.PropertyBag;

			// If OnBeforeStep returns false we stop
			if (!wizController.OnBeforeStep(wizardName, currentStep, currentStepInstance))
			{
				return;
			}
			
			// Initialize step data so instance members can be used
			// executor.InitializeController(urlInfo.Area, urlInfo.Controller, innerAction);

			// Record the step we're working with
			WizardUtils.RegisterCurrentStepInfo(controller, currentStepInstance.ActionName);

			// The step cannot be accessed in the current state of matters
			if (!currentStepInstance.IsPreConditionSatisfied(controller.Context))
			{
				return;
			}
			
			// Dispatch process
			try
			{
				// TODO: Invoke Whole step here
				// currentStepInstance.Process(controller.Context, 
				//	urlInfo.Area, urlInfo.Controller, innerAction);
				currentStepExecutor.ProcessSelectedAction();
			}
			finally
			{
				wizController.OnAfterStep(wizardName, currentStep, currentStepInstance);

				currentStepExecutor.Dispose();
			}
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
			        context.Session.Contains(wizardName + "currentstep"));
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
				context.Response.Redirect(controller.AreaName, controller.Name, firstStep);
			}
		}

		protected void ResetSteps(Controller controller)
		{
			IWizardController wizardController = controller as IWizardController;

			WizardStepPage[] steps = wizardController.GetSteps(controller.Context);

			foreach(WizardStepPage step in steps)
			{
				step.InitializeFieldsFromServiceProvider(controller.Context);
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

		private void SetUpWizardHelper(Controller controller)
		{
			if (controller == null) return;

			WizardHelper helper = new WizardHelper();

			helper.SetController(controller);

			controller.Helpers["wizardhelper"] = helper;
		}
	}
}
