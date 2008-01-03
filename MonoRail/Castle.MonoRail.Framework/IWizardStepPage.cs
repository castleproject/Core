// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	/// <summary>
	/// Pendent
	/// </summary>
	public interface IWizardStepPage : IController
	{
		/// <summary>
		/// Allow the step to assert some condition 
		/// before being accessed. Returning <c>false</c>
		/// prevents the step from being processed but 
		/// before doing that you must send a redirect.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// 	<c>true</c> if [is pre condition satisfied] [the specified context]; otherwise, <c>false</c>.
		/// </returns>
		bool IsPreConditionSatisfied(IEngineContext context);

		/// <summary>
		/// Resets this instance.
		/// </summary>
		void Reset();

		/// <summary>
		/// Gets the name of the step.
		/// </summary>
		/// <value>The name of the step.</value>
		string ActionName { get; }

		/// <summary>
		/// Gets the wizard controller (parent).
		/// </summary>
		/// <value>The wizard controller.</value>
		IWizardController WizardController { get; set; }

		/// <summary>
		/// Gets the controller context.
		/// </summary>
		/// <value>The controller context.</value>
		IControllerContext WizardControllerContext { get; set; }
	}
}
