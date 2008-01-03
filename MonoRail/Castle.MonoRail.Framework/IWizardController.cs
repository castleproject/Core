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
	using System;

	/// <summary>
	/// Depicts the contract for wizard controllers. 
	/// </summary>
	/// 
	/// <seealso xref="WizardActionProvider"/>
	/// <seealso xref="WizardStepPage"/>
	/// 
	/// <example>
	/// The following code shows how to create a simple wizard with two pages.
	/// <code>
	/// [DynamicActionProvider(typeof(WizardActionProvider))]
	/// public class MyWizardController : Controller, IWizardController
	/// {
	///		public void OnWizardStart()
	///		{ }
	/// 
	///		public bool OnBeforeStep(String wizardName, String stepName, WizardStepPage step)
	///		{
	///			returtn true; 
	///		}
	///	
	/// 	public void OnAfterStep(String wizardName, String stepName, WizardStepPage step)
	///		{ }
	///		
	///		public WizardStepPage[] GetSteps(IHandlerContext context)
	///		{
	///			return new WizardStepPage[] { new MyPage1(), new MyPage2() }; 
	///		}
	/// }
	/// </code>
	/// </example>
	/// 
	/// <remarks>
	/// The interface members allow you to perform some logic on important
	/// events from a wizard lifecycle. The <see cref="GetSteps"/> must be used
	/// to return the steps the wizard has.
	/// </remarks>
	public interface IWizardController : IController
	{
		/// <summary>
		/// Called when the wizard starts. 
		/// </summary>
		/// <remarks>
		/// This is invoked only once per wizard lifecycle, but can 
		/// happen again if the data added by the infrastructure was not found on the session.
		/// </remarks>
		void OnWizardStart();

		/// <summary>
		/// Called before processing a step. Returning <c>false</c> tells 
		/// the infrastructure to stop the processing the request.
		/// </summary>
		/// <param name="wizardName">Name of the wizard.</param>
		/// <param name="stepName">Name of the step.</param>
		/// <param name="step">The step instance.</param>
		/// <returns><c>true</c> if the process should proceed, otherwise, <c>false</c></returns>
		bool OnBeforeStep(string wizardName, string stepName, IWizardStepPage step);

		/// <summary>
		/// Called after processing a step.
		/// </summary>
		/// <param name="wizardName">Name of the wizard.</param>
		/// <param name="stepName">Name of the step.</param>
		/// <param name="step">The step instance.</param>
		void OnAfterStep(string wizardName, string stepName, IWizardStepPage step);

		/// <summary>
		/// Implementors should return an array of steps that compose the wizard. 
		/// </summary>
		/// <remarks>
		/// This should be deterministic per session -- ie.
		/// always return the same instances for the same user session.
		/// </remarks>
		/// <param name="context">The web request context.</param>
		/// <returns>An array of steps</returns>
		IWizardStepPage[] GetSteps(IEngineContext context);
	}
}