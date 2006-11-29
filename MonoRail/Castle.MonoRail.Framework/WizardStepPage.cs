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
	using System.Reflection;

	using Castle.Components.Binder;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Represents a wizard step. 
	/// </summary>
	/// <remarks>
	/// Implementors can optionally override <see cref="WizardStepPage.ActionName"/>
	/// to customize the accessible action name and 
	/// <see cref="WizardStepPage.RenderWizardView"/> in order to define which view 
	/// should be used (defaults to the step name)
	/// <para>
	/// Please note that an step might have actions as well, but it follows a different 
	/// convention to be accessed. You must use the wizard controller name, slash, the
	/// step name, hifen, the action name. For example <c>/MyWizard/AddressInformation-GetCountries.rails</c>
	/// Which would access the following action
	/// </para>
	/// <code>
	/// public class AddressInformation : WizardStepPage
	/// {
	///		public void GetCountries()
	///		{
	///		  ...
	///		}
	/// }
	/// </code>
	/// <para>Note that the RedirectToAction will always send to an internal action, so you should
	/// omit the controller name for that.</para>
	/// <para>
	/// You can use a family of redirect methods to go back and forward on the wizard's 
	/// steps.
	/// </para>
	/// </remarks>
	public abstract class WizardStepPage : SmartDispatcherController
	{
		#region Fields

		private Controller _wizardcontroller;

		#endregion

		#region Constructors

		public WizardStepPage()
		{			
		}

		public WizardStepPage(DataBinder binder) : base(binder)
		{			
		}

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

//			context = wizardController.Context;
//
//			UrlInfo urlInfo = context.UrlInfo;
//
//			InitializeFieldsFromServiceProvider(wizardController.Context);
//			InitializeControllerState(urlInfo.Area, urlInfo.Controller, urlInfo.Action);
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
			get
			{
				Type thisType = GetType();
				
				// Hack fix for "dynamic proxied" controllers
				if (thisType.Assembly.FullName.StartsWith("DynamicAssemblyProxyGen") ||
					thisType.Assembly.FullName.StartsWith("DynamicProxyGenAssembly2"))
				{
					return thisType.BaseType.Name;
				}
				
				return GetType().Name;
			}
		}

		/// <summary>
		/// Used to decide on which view to render.
		/// </summary>
		protected internal virtual void RenderWizardView()
		{
			RenderView(ActionName);
		}

		/// <summary>
		/// Allow the step to assert some condition 
		/// before being accessed. Returning <c>false</c>
		/// prevents the step from being processed but 
		/// before doing that you must send a redirect.
		/// </summary>
		/// <returns></returns>
		protected internal virtual bool IsPreConditionSatisfied(IRailsEngineContext context)
		{
			return true;
		}

		protected internal override MethodInfo SelectMethod(String action, IDictionary actions, IRequest request, params object[] actionArgs)
		{
			if (action == "RenderWizardView")
			{
				return typeof(WizardStepPage).GetMethod("RenderWizardView", BindingFlags.Instance|BindingFlags.NonPublic);
			}
			else
			{
				return base.SelectMethod(action, actions, request);
			}
		}

		#endregion

		#region DoNavigate and Redirects

		/// <summary>
		/// Navigates within the wizard steps using optionally a form parameter 
		/// to dictate to where it should go.
		/// </summary>
		/// <remarks>
		/// By default this will invoke <see cref="RedirectToNextStep(IDictionary)"/>
		/// however you can send a field form <c>navigate.to</c> to customize this.
		/// The possible values for <c>navigate.to</c> are:
		/// <list type="bullet">
		/// <item><term>previous</term>
		/// <description>Invokes <see cref="RedirectToPreviousStep()"/></description></item>
		/// <item><term>first</term>
		/// <description>Invokes <see cref="RedirectToFirstStep()"/></description></item>
		/// <item><term>step name</term>
		/// <description>A custom step name to navigate</description></item>
		/// </list>
		/// </remarks>
		protected void DoNavigate()
		{
			DoNavigate((IDictionary) null);
		}
		
		/// <summary>
		/// Navigates within the wizard steps using optionally a form parameter 
		/// to dictate to where it should go.
		/// </summary>
		/// <remarks>
		/// By default this will invoke <see cref="RedirectToNextStep(IDictionary)"/>
		/// however you can send a field form <c>navigate.to</c> to customize this.
		/// The possible values for <c>navigate.to</c> are:
		/// <list type="bullet">
		/// <item><term>previous</term>
		/// <description>Invokes <see cref="RedirectToPreviousStep()"/></description></item>
		/// <item><term>first</term>
		/// <description>Invokes <see cref="RedirectToFirstStep()"/></description></item>
		/// <item><term>step name</term>
		/// <description>A custom step name to navigate</description></item>
		/// </list>
		/// </remarks>
		/// <param name="queryStringParameters">Query string parameters to be on the URL</param>
		protected void DoNavigate(params String[] queryStringParameters)
		{
			DoNavigate(DictHelper.Create(queryStringParameters));
		}
		
		/// <summary>
		/// Navigates within the wizard steps using optionally a form parameter 
		/// to dictate to where it should go.
		/// </summary>
		/// <remarks>
		/// By default this will invoke <see cref="RedirectToNextStep(IDictionary)"/>
		/// however you can send a field form <c>navigate.to</c> to customize this.
		/// The possible values for <c>navigate.to</c> are:
		/// <list type="bullet">
		/// <item><term>previous</term>
		/// <description>Invokes <see cref="RedirectToPreviousStep()"/></description></item>
		/// <item><term>first</term>
		/// <description>Invokes <see cref="RedirectToFirstStep()"/></description></item>
		/// <item><term>step name</term>
		/// <description>A custom step name to navigate</description></item>
		/// </list>
		/// </remarks>
		/// <param name="queryStringParameters">Query string parameters to be on the URL</param>
		protected void DoNavigate(IDictionary queryStringParameters)
		{
			string uriPrefix = "uri:";

			String navigateTo = Params["navigate.to"];

			if (navigateTo == "previous")
			{
				RedirectToPreviousStep(queryStringParameters);
			}
			else if (navigateTo == null || navigateTo == String.Empty || navigateTo == "next")
			{
				RedirectToNextStep(queryStringParameters);
			}
			else if (navigateTo.StartsWith(uriPrefix))
			{
				Redirect(navigateTo.Substring(uriPrefix.Length), queryStringParameters);
			}
			else if (navigateTo == "first")
			{
				RedirectToFirstStep(queryStringParameters);
			}
			else
			{
				RedirectToStep(navigateTo, queryStringParameters);
			}
		}
		
		/// <summary>
		/// Sends a redirect to the next wizard step (if it exists)
		/// </summary>
		/// <exception cref="RailsException">if no further step exists</exception>
		protected void RedirectToNextStep()
		{
			RedirectToNextStep((IDictionary) null);
		}

		/// <summary>
		/// Sends a redirect to the next wizard step (if it exists)
		/// </summary>
		/// <exception cref="RailsException">if no further step exists</exception>
		protected void RedirectToNextStep(params String[] queryStringParameters)
		{
			RedirectToNextStep(DictHelper.Create(queryStringParameters));
		}
		
		/// <summary>
		/// Sends a redirect to the next wizard step (if it exists)
		/// </summary>
		/// <exception cref="RailsException">if no further step exists</exception>
		protected void RedirectToNextStep(IDictionary queryStringParameters)
		{
			String wizardName = WizardUtils.ConstructWizardNamespace(_wizardcontroller);

			int currentIndex = (int) Context.Session[wizardName + "currentstepindex"];
			
			IList stepList = (IList) Context.UnderlyingContext.Items["wizard.step.list"];

			if ((currentIndex + 1) < stepList.Count)
			{
				int nextStepIndex = currentIndex + 1;

				String nextStep = (String) stepList[nextStepIndex];

				WizardUtils.RegisterCurrentStepInfo(_wizardcontroller, nextStepIndex, nextStep);
				
				InternalRedirectToStep(nextStepIndex, nextStep, queryStringParameters);
			}
			else
			{
				throw new RailsException("There is no next step available");
			}
		}

		/// <summary>
		/// Sends a redirect to the previous wizard step
		/// </summary>
		/// <exception cref="RailsException">
		/// if no previous step exists (ie. already in the first one)</exception>
		protected void RedirectToPreviousStep()
		{
			RedirectToPreviousStep((IDictionary) null);
		}
		
		/// <summary>
		/// Sends a redirect to the previous wizard step
		/// </summary>
		/// <exception cref="RailsException">
		/// if no previous step exists (ie. already in the first one)</exception>
		protected void RedirectToPreviousStep(params String[] queryStringParameters)
		{
			RedirectToPreviousStep(DictHelper.Create(queryStringParameters));
		}
		
		/// <summary>
		/// Sends a redirect to the previous wizard step
		/// </summary>
		/// <exception cref="RailsException">
		/// if no previous step exists (ie. already in the first one)</exception>
		protected void RedirectToPreviousStep(IDictionary queryStringParameters)
		{
			String wizardName = WizardUtils.ConstructWizardNamespace(_wizardcontroller);

			int currentIndex = (int) Context.Session[wizardName + "currentstepindex"];
			
			IList stepList = (IList) Context.UnderlyingContext.Items["wizard.step.list"];

			if ((currentIndex - 1) >= 0)
			{
				int prevStepIndex = currentIndex - 1;

				String prevStep = (String) stepList[prevStepIndex];

				InternalRedirectToStep(prevStepIndex, prevStep, queryStringParameters);
			}
			else
			{
				throw new RailsException("There is no previous step available");
			}
		}

		/// <summary>
		/// Sends a redirect to the first wizard step
		/// </summary>
		protected void RedirectToFirstStep()
		{
			RedirectToFirstStep((IDictionary) null);
		}
		
		/// <summary>
		/// Sends a redirect to the first wizard step
		/// </summary>
		protected void RedirectToFirstStep(params String[] queryStringParameters)
		{
			RedirectToFirstStep(DictHelper.Create(queryStringParameters));
		}
		
		/// <summary>
		/// Sends a redirect to the first wizard step
		/// </summary>
		protected void RedirectToFirstStep(IDictionary queryStringParameters)
		{
			IList stepList = (IList) Context.UnderlyingContext.Items["wizard.step.list"];

			String firstStep = (String) stepList[0];

			InternalRedirectToStep(0, firstStep, queryStringParameters);
		}

		/// <summary>
		/// Sends a redirect to a custom step (that must exists)
		/// </summary>
		protected bool RedirectToStep(String stepName)
		{
			return RedirectToStep(stepName, (IDictionary) null);
		}
		
		/// <summary>
		/// Sends a redirect to a custom step (that must exists)
		/// </summary>
		protected bool RedirectToStep(String stepName, params String[] queryStringParameters)
		{
			return RedirectToStep(stepName, DictHelper.Create(queryStringParameters));
		}
		
		/// <summary>
		/// Sends a redirect to a custom step (that must exists)
		/// </summary>
		protected bool RedirectToStep(String stepName, IDictionary queryStringParameters)
		{
			IList stepList = (IList) Context.UnderlyingContext.Items["wizard.step.list"];

			for(int index = 0; index < stepList.Count; index++)
			{
				String curStep = (String) stepList[index];

				if (curStep == stepName)
				{
					InternalRedirectToStep(index, stepName, queryStringParameters);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// For a wizard step, an internal action will always be named
		/// with the controller name as a prefix , plus an hifen and finally
		/// the action name. This implementation does exactly that.
		/// </summary>
		/// <param name="action">Raw action name</param>
		/// <returns>Properly formatted action name</returns>
		internal override String TransformActionName(String action)
		{
			return base.TransformActionName(ActionName + "-" + action);
		}

		private void InternalRedirectToStep(int stepIndex, String step, IDictionary queryStringParameters)
		{
			WizardUtils.RegisterCurrentStepInfo(_wizardcontroller, stepIndex, step);
	
			if (queryStringParameters != null && queryStringParameters.Count != 0)
			{
				Redirect(_wizardcontroller.Name, step, queryStringParameters);
			}
			else if (Context.Request.QueryString.HasKeys())
			{
				// We need to preserve any attribute from the QueryString
				// for example in case the url has an Id

				String url = UrlInfo.CreateAbsoluteRailsUrl( Context.ApplicationPath, 
								_wizardcontroller.Name, step, Context.UrlInfo.Extension ) + Context.Request.Uri.Query;
				
				Context.Response.Redirect(url);
			}
			else
			{
				Context.Response.Redirect(_wizardcontroller.Name, step);
			}
		}

		#endregion
	}
}
