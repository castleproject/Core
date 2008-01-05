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

namespace Castle.MonoRail.TestSupport
{
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Base test for wizard steps.
	/// </summary>
	/// <typeparam name="W">The wizard step page type</typeparam>
	/// <typeparam name="C">The wizard controller -- the one that implements <see cref="IWizardController"/></typeparam>
	public class GenericBaseWizardStepTest<W, C> : GenericBaseControllerTest<C> 
		where W : WizardStepPage 
		where C : Controller
	{
		/// <summary>
		/// The step typed field
		/// </summary>
		protected W wizardStep;

		/// <summary>
		/// Runs the page pre-condition
		/// </summary>
		/// <returns></returns>
		protected bool RunIsPreConditionSatisfied()
		{
			object[] args = new object[] { Context };
			return (bool) ReflectionHelper.RunInstanceMethod(typeof(WizardStepPage), 
				wizardStep, "IsPreConditionSatisfied", ref args);
		}

		/// <summary>
		/// Runs the step render method.
		/// </summary>
		protected void RunRenderWizardView()
		{
			ReflectionHelper.RunInstanceMethod(typeof(WizardStepPage), wizardStep, "RenderWizardView");
		}
	}
}
