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
	using System.IO;
	using System.Web;
	using System.Collections;
	using System.Collections.Specialized;

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
	/// <see cref="WizardStepPage.Show"/> in order to define which view 
	/// should be used.
	/// </remarks>
	public abstract class WizardStepPage : IDynamicAction
	{
		#region Fields

		private Controller _controller;

		protected DataBinder binder;

		#endregion

		#region Constructors

		public WizardStepPage()
		{
			binder = new DataBinder();
		}

		#endregion

		#region Useful Properties

		protected Controller WizardController
		{
			get { return _controller; }
		}

		public DataBinder Binder
		{
			get { return binder; }
		}

		/// <summary>
		/// Gets the property bag, which is used
		/// to pass variables to the view.
		/// </summary>
		public IDictionary PropertyBag
		{
			get { return _controller.PropertyBag; }
		}

		/// <summary>
		/// Gets the context of this web execution.
		/// </summary>
		public IRailsEngineContext Context
		{
			get { return _controller.Context; }
		}

		/// <summary>
		/// Gets the Session dictionary.
		/// </summary>
		protected IDictionary Session
		{
			get { return Context.Session; }
		}

		/// <summary>
		/// Gets a dictionary of volative items.
		/// Ideal for showing success and failures messages.
		/// </summary>
		protected IDictionary Flash
		{
			get { return Context.Flash; }
		}

		/// <summary>
		/// Gets the web context of ASP.NET API.
		/// </summary>
		protected HttpContext HttpContext
		{
			get { return Context.UnderlyingContext; }
		}

		/// <summary>
		/// Gets the request object.
		/// </summary>
		public IRequest Request
		{
			get { return Context.Request; }
		}

		/// <summary>
		/// Gets the response object.
		/// </summary>
		public IResponse Response
		{
			get { return Context.Response; }
		}

		/// <summary>
		/// Shortcut to Request.Params
		/// </summary>
		public NameValueCollection Params
		{
			get { return Request.Params; }
		}

		#endregion

		#region Useful Operations

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		public void RenderView(String name)
		{
			_controller.RenderView(name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		public void RenderView(String name, bool skipLayout)
		{
			_controller.RenderView(name, skipLayout);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		public void RenderView(String controller, String name)
		{
			_controller.RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		public void RenderView(String controller, String name, bool skipLayout)
		{
			_controller.RenderView(controller, name, skipLayout);
		}

		/// <summary>
		/// Specifies the view to be processed and results are written to System.IO.TextWriter. 
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name">The name of the view to process.</param>
		public void InPlaceRenderView(TextWriter output, String name)
		{
			_controller.InPlaceRenderView(output, name);		
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public void RenderSharedView(String name)
		{
			_controller.RenderSharedView(name);
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public void RenderSharedView(String name, bool skipLayout)
		{
			_controller.RenderSharedView(name, skipLayout);
		}

		/// <summary>
		/// Specifies the shared view to be processed and results are written to System.IO.TextWriter.
		/// (A partial view shared by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name">The name of the view to process.</param>
		public void InPlaceRenderSharedView(TextWriter output, String name)
		{
			_controller.InPlaceRenderSharedView(output, name);
		}

		/// <summary>
		/// Cancels the view processing.
		/// </summary>
		public void CancelView()
		{
			_controller.CancelView();
		}

		/// <summary>
		/// Cancels the layout processing.
		/// </summary>
		public void CancelLayout()
		{
			_controller.CancelLayout();
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		/// <param name="contents"></param>
		public void RenderText(String contents)
		{
			_controller.RenderText(contents);
		}

		/// <summary>
		/// Returns true if the specified template exists.
		/// </summary>
		/// <param name="templateName"></param>
		public bool HasTemplate(String templateName)
		{
			return _controller.HasTemplate(templateName);
		}

		/// <summary>
		/// Redirects to the specified URL.
		/// </summary>
		/// <param name="url">Target URL</param>
		public void Redirect(String url)
		{
			_controller.Redirect(url);
		}

		/// <summary>
		/// Redirects to another controller and action.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		public void Redirect(String controller, String action)
		{
			_controller.Redirect(controller, action);
		}

		/// <summary>
		/// Redirects to another controller and action.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		public void Redirect(String area, String controller, String action)
		{
			_controller.Redirect(area, controller, action);
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(String controller, String action, NameValueCollection parameters)
		{
			_controller.Redirect(controller, action, parameters);
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(String area, String controller, String action, NameValueCollection parameters)
		{
			_controller.Redirect(area, controller, action, parameters);
		}

		#endregion

		#region Core Lifecycle methods

		/// <summary>
		/// Invoked when the wizard is being access from the start 
		/// action. Implementors should perform session clean up (if 
		/// they actually use the session) to avoid stale data on forms.
		/// </summary>
		public virtual void Reset()
		{
			
		}

		public virtual String ActionName
		{
			get { return GetType().Name; }
		}

		/// <summary>
		/// Process the data input from the wizard step and 
		/// return <c>true</c> if everything is OK.
		/// </summary>
		/// <remarks>
		/// Implementors should perform their logic and only return
		/// <c>true</c> if, for example, no validation failed. Returning
		/// <c>false</c> keep the user on the same step
		/// </remarks>
		/// <returns><c>true</c> if the user can go to the next step</returns>
		protected virtual bool Process()
		{
			return true;
		}

		/// <summary>
		/// Used to decide on which view to render.
		/// </summary>
		protected virtual void Show()
		{
			_controller.RenderView( GetType().Name );
		}

		#endregion

		#region IDynamicAction implementation

		/// <summary>
		/// 
		/// </summary>
		/// <param name="controller"></param>
		public void Execute(Controller controller)
		{
			IRailsEngineContext context = controller.Context;

			_controller = controller;

			IList stepList = (IList) context.UnderlyingContext.Items["wizard.step.list"];

			String wizardName = WizardUtils.ConstructWizardNamespace(controller);

			String currentStep = (String) context.Session[wizardName + "currentstep"];

			String doProcessFlag = context.Params["wizard.doprocess"];

			// This is a repost/postback
			// and the programmer wants to perform Process invocation
			if (currentStep == ActionName && doProcessFlag == "true") 
			{
				if (Process())
				{
					// Successful - it means that we can move forward
					
					int currentIndex = (int) context.Session[wizardName + "currentstepindex"];
					
					if ((currentIndex + 1) < stepList.Count)
					{
						int nextStepIndex = currentIndex + 1;

						String nextStep = (String) stepList[nextStepIndex];

						WizardUtils.RegisterCurrentStepInfo(controller, nextStepIndex, nextStep);

						context.Response.Redirect(controller.Name, nextStep);
					}
				}
			}

			WizardUtils.RegisterCurrentStepInfo(controller, ActionName);

			Show();
		}

		#endregion
	}
}
