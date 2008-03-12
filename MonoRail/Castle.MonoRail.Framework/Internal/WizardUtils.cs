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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections;

	/// <summary>
	/// Utility class for wizard related queries and operations
	/// </summary>
	public static class WizardUtils
	{
		/// <summary>
		/// Constructs the wizard namespace.
		/// </summary>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns></returns>
		public static String ConstructWizardNamespace(IControllerContext controllerContext)
		{
			return String.Format("wizard.{0}", controllerContext.Name);
		}

		/// <summary>
		/// Determines whether the current request is within a wizard context.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns>
		/// 	<c>true</c> if is on wizard context; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsOnWizard(IEngineContext engineContext, IControllerContext controllerContext)
		{
			String wizardName = WizardUtils.ConstructWizardNamespace(controllerContext);
			return engineContext.Session.Contains(wizardName + "currentstepindex");
		}

		/// <summary>
		/// Determines whether the current wizard has a previous step.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns>
		/// 	<c>true</c> if has previous step; otherwise, <c>false</c>.
		/// </returns>
		public static bool HasPreviousStep(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			if (!IsOnWizard(engineContext, controllerContext))
			{
				return false;
			}

			String wizardName = WizardUtils.ConstructWizardNamespace(controllerContext);

			int currentIndex = (int) engineContext.Session[wizardName + "currentstepindex"];

			return currentIndex > 0;
		}

		/// <summary>
		/// Determines whether the current wizard has a next step.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns>
		/// 	<c>true</c> if has next step; otherwise, <c>false</c>.
		/// </returns>
		public static bool HasNextStep(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			if (!IsOnWizard(engineContext, controllerContext))
			{
				return false;
			}

			String wizardName = WizardUtils.ConstructWizardNamespace(controllerContext);

			IList stepList = (IList) engineContext.Items["wizard.step.list"];

			int currentIndex = (int) engineContext.Session[wizardName + "currentstepindex"];

			return (currentIndex + 1) < stepList.Count;
		}

		/// <summary>
		/// Gets the index of the current step.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns></returns>
		public static int GetCurrentStepIndex(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			if (!IsOnWizard(engineContext, controllerContext))
			{
				return -1;
			}

			String wizardName = WizardUtils.ConstructWizardNamespace(controllerContext);

			int curIndex = (int) engineContext.Session[wizardName + "currentstepindex"];

			return curIndex;
		}

		/// <summary>
		/// Gets the name of the current step.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns></returns>
		public static String GetCurrentStepName(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			if (!IsOnWizard(engineContext, controllerContext))
			{
				return null;
			}

			String wizardName = WizardUtils.ConstructWizardNamespace(controllerContext);

			int curIndex = (int) engineContext.Session[wizardName + "currentstepindex"];

			IList stepList = (IList) engineContext.Items["wizard.step.list"];

			return (String) stepList[curIndex];
		}

		/// <summary>
		/// Gets the name of the previous step.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns></returns>
		public static String GetPreviousStepName(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			if (!IsOnWizard(engineContext, controllerContext))
			{
				return null;
			}

			String wizardName = WizardUtils.ConstructWizardNamespace(controllerContext);

			int curIndex = (int) engineContext.Session[wizardName + "currentstepindex"];

			IList stepList = (IList) engineContext.Items["wizard.step.list"];

			if ((curIndex - 1) >= 0)
			{
				return (String) stepList[curIndex - 1];
			}

			return null;
		}

		/// <summary>
		/// Gets the name of the next step.
		/// </summary>
		/// <param name="index">The step index.</param>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns></returns>
		public static string GetStepName(int index, IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			if (!IsOnWizard(engineContext, controllerContext))
			{
				return null;
			}

			IList stepList = (IList) engineContext.Items["wizard.step.list"];

			if ((index) < stepList.Count)
			{
				return (String) stepList[index];
			}

			return null;
		}

		/// <summary>
		/// Gets the name of the next step.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns></returns>
		public static String GetNextStepName(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			if (!IsOnWizard(engineContext, controllerContext))
			{
				return null;
			}

			String wizardName = WizardUtils.ConstructWizardNamespace(controllerContext);

			int curIndex = (int) engineContext.Session[wizardName + "currentstepindex"];

			IList stepList = (IList) engineContext.Items["wizard.step.list"];

			if ((curIndex + 1) < stepList.Count)
			{
				return (String) stepList[curIndex + 1];
			}

			return null;
		}

		/// <summary>
		/// Registers the current step info/state.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="actionName">Name of the action.</param>
		public static void RegisterCurrentStepInfo(IEngineContext engineContext, IController controller,
												   IControllerContext controllerContext, String actionName)
		{
			IList stepList = (IList) engineContext.Items["wizard.step.list"];

			for(int i = 0; i < stepList.Count; i++)
			{
				String stepName = (String) stepList[i];

				if (actionName == stepName)
				{
					RegisterCurrentStepInfo(engineContext, controller, controllerContext, i, stepName);
					break;
				}
			}
		}

		/// <summary>
		/// Registers the current step info/state.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="stepIndex">Index of the step.</param>
		/// <param name="stepName">Name of the step.</param>
		public static void RegisterCurrentStepInfo(IEngineContext engineContext, IController controller,
		                                           IControllerContext controllerContext, int stepIndex, String stepName)
		{
			String wizardName = WizardUtils.ConstructWizardNamespace(controllerContext);

			engineContext.Session[wizardName + "currentstepindex"] = stepIndex;
			engineContext.Session[wizardName + "currentstep"] = stepName;
		}
	}
}