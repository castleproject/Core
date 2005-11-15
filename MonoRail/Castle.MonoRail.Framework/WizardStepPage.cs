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

	using Castle.MonoRail.Framework.Internal;


	/// <summary>
	/// Represents a wizard step. 
	/// </summary>
	/// <remarks>
	/// Implementors are required to implement 
	/// <see cref="WizardStepPage.Process"/> in order 
	/// to perform the step processment.
	/// They can optionally also override <see cref="WizardStepPage.ActionName"/>
	/// to customize the accessible action name and 
	/// <see cref="WizardStepPage.RenderWizardView"/> in order to define which view 
	/// should be used (defaults to the step name)
	/// </remarks>
	public abstract class WizardStepPage : SmartDispatcherController//, IDynamicAction
	{
		#region Fields

		private Controller _wizardcontroller;

		#endregion

		#region Useful Properties

		public Controller WizardController
		{
			get { return _wizardcontroller; }
		}

		#endregion

		#region Core Lifecycle methods

		/// <summary>
		/// Invoked by <see cref="WizardActionProvider"/>. 
		/// </summary>
		/// <remarks>
		/// This can be overriden but it's important to invoke the base 
		/// implementation.
		/// </remarks>
		/// <param name="wizardController"></param>
		protected internal virtual void Initialize(Controller wizardController)
		{
			_wizardcontroller = wizardController;

			_context = wizardController.Context;

			InitializeFieldsFromServiceProvider(wizardController.ServiceProvider);
		}

		/// <summary>
		/// Invoked when the wizard is being access from the start 
		/// action. Implementors should perform session clean up (if 
		/// they actually use the session) to avoid stale data on forms.
		/// </summary>
		protected internal virtual void Reset()
		{
			
		}

		/// <summary>
		/// Returns the action name that will be used 
		/// to represent this step.
		/// </summary>
		public virtual String ActionName
		{
			get { return GetType().Name; }
		}

		/// <summary>
		/// Used to decide on which view to render.
		/// </summary>
		protected internal virtual void RenderWizardView()
		{
			_wizardcontroller.RenderView( ActionName );
		}

		#endregion

		protected void RedirectToNextStep()
		{
			String wizardName = WizardUtils.ConstructWizardNamespace(_wizardcontroller);

			int currentIndex = (int) Context.Session[wizardName + "currentstepindex"];
			
			IList stepList = (IList) Context.UnderlyingContext.Items["wizard.step.list"];

			if ((currentIndex + 1) < stepList.Count)
			{
				int nextStepIndex = currentIndex + 1;

				String nextStep = (String) stepList[nextStepIndex];

				WizardUtils.RegisterCurrentStepInfo(_wizardcontroller, nextStepIndex, nextStep);
				
				// We need to preserve any attribute from the QueryString
				// for example in case the url has an Id
				if( Context.Request.QueryString.HasKeys() )
				{							
					string url = UrlInfo.CreateAbsoluteRailsUrl( Context.ApplicationPath, 
						_wizardcontroller.Name, nextStep, Context.UrlInfo.Extension )
								+ Context.Request.Uri.Query;
					
					Context.Response.Redirect( url );
				}
				else
				{
					Context.Response.Redirect(_wizardcontroller.Name, nextStep);
				}
			}
		}

		#region IDynamicAction implementation

		/// <summary>
		/// 
		/// </summary>
		/// <param name="controller"></param>
//		public void Execute(Controller controller)
//		{
//			IRailsEngineContext context = controller.Context;
//
//			IWizardController wizController = (IWizardController) controller;
//
//			IList stepList = (IList) context.UnderlyingContext.Items["wizard.step.list"];
//
//			String wizardName = WizardUtils.ConstructWizardNamespace(controller);
//
//			String currentStep = (String) context.Session[wizardName + "currentstep"];
//
//			String doProcessFlag = context.Params["wizard.doprocess"];
//
//			if (!wizController.OnBeforeStep(wizardName, currentStep, this))
//			{
//				return;
//			}
//
//			bool redirected = false;
//
//			// This is a repost/postback
//			// and the programmer wants to perform Process invocation
//			if (currentStep == ActionName && doProcessFlag == "true") 
//			{
//				if (Process())
//				{
//					wizController.OnAfterStep(wizardName, currentStep, this);
//
//					// Successful - it means that we can move forward
//					
//					int currentIndex = (int) context.Session[wizardName + "currentstepindex"];
//					
//					if ((currentIndex + 1) < stepList.Count)
//					{
//						int nextStepIndex = currentIndex + 1;
//
//						String nextStep = (String) stepList[nextStepIndex];
//
//						WizardUtils.RegisterCurrentStepInfo(controller, nextStepIndex, nextStep);
//						
//						// We need to preserve any attribute from the QueryString
//						// for example in case the url has an Id
//						if( context.Request.QueryString.HasKeys() )
//						{							
//							string url = UrlInfo.CreateAbsoluteRailsUrl( context.ApplicationPath, controller.Name, nextStep, context.UrlInfo.Extension )
//									   + context.Request.Uri.Query;
//							
//							redirected = true;
//
//							context.Response.Redirect( url );
//						}
//						else
//						{
//							redirected = true;
//
//							context.Response.Redirect(controller.Name, nextStep);
//						}
//					}
//				}
//			}
//
//			WizardUtils.RegisterCurrentStepInfo(controller, ActionName);
//
//			if (!redirected)
//				Show();
//			else
//				CancelView();
//
//			wizController.OnAfterStep(wizardName, currentStep, this);
//		}

		#endregion
	}
}
