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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections;

	public abstract class WizardUtils
	{
		public static String ConstructWizardNamespace(Controller controller)
		{
			return String.Format("wizard.{0}", controller.Name);
		}

		public static bool HasPreviousStep(Controller controller)
		{
			IRailsEngineContext context = controller.Context;

			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			int currentIndex = (int) context.Session[wizardName + "currentstepindex"];

			return currentIndex > 0;
		}

		public static bool HasNextStep(Controller controller)
		{
			IRailsEngineContext context = controller.Context;

			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			IList stepList = (IList) context.UnderlyingContext.Items["wizard.step.list"];

			int currentIndex = (int) context.Session[wizardName + "currentstepindex"];
						
			return (currentIndex + 1) < stepList.Count;
		}

		public static String GetPreviousStepName(Controller controller)
		{
			IRailsEngineContext context = controller.Context;

			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			int curIndex = (int) context.Session[wizardName + "currentstepindex"];

			IList stepList = (IList) context.UnderlyingContext.Items["wizard.step.list"];

			if ((curIndex - 1) >= 0)
			{
				return (String) stepList[curIndex - 1];
			}

			return null;
		}

		public static String GetNextStepName(Controller controller)
		{
			IRailsEngineContext context = controller.Context;

			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			int curIndex = (int) context.Session[wizardName + "currentstepindex"];

			IList stepList = (IList) context.UnderlyingContext.Items["wizard.step.list"];

			if ((curIndex + 1) < stepList.Count)
			{
				return (String) stepList[curIndex + 1];
			}

			return null;
		}

		public static void RegisterCurrentStepInfo(Controller controller, String actionName)
		{
			IRailsEngineContext context = controller.Context;

			IList stepList = (IList) context.UnderlyingContext.Items["wizard.step.list"];

			for(int i=0; i < stepList.Count; i++)
			{
				String stepName = (String) stepList[i];

				if (actionName == stepName)
				{
					RegisterCurrentStepInfo(controller, i, stepName, false);

					break;
				}
			}
		}

		public static void RegisterCurrentStepInfo(Controller controller, 
			int stepIndex, string stepName, bool started)
		{
			IRailsEngineContext context = controller.Context;
			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			context.Session[wizardName + "currentstepindex"] = stepIndex;
			context.Session[wizardName + "currentstep"] = stepName;

			if (started)
			{
				context.Session[wizardName + "juststarted"] = true;
			}
		}
	}
}
