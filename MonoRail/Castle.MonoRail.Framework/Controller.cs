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
	using System.IO;
	using System.Web;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.Components.Common.EmailSender;
	using Castle.Components.Validator;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Implements the core functionality and exposes the
	/// common methods for concrete controllers.
	/// </summary>
	public abstract class Controller
	{
		#region Fields

		/// <summary>
		/// Holds the request/context information
		/// </summary>
		internal IRailsEngineContext context;

		/// <summary>
		/// The reference to the <see cref="IViewEngineManager"/> instance
		/// </summary>
		internal IViewEngineManager viewEngineManager;

		/// <summary>
		/// Logger instance. Should never be null
		/// </summary>
		internal ILogger logger = NullLogger.Instance;

		/// <summary>
		/// Holds information to pass to the view
		/// </summary>
		private IDictionary bag = new HybridDictionary();

		/// <summary>
		/// The area name which was used to access this controller
		/// </summary>
		private string _areaName;

		/// <summary>
		/// The controller name which was used to access this controller
		/// </summary>
		private string _controllerName;

		/// <summary>
		/// The view name selected to be rendered after the execution 
		/// of the action
		/// </summary>
		internal string _selectedViewName;

		/// <summary>
		/// The layout name that the view engine should use
		/// </summary>
		private string _layoutName;

		/// <summary>
		/// The original action requested
		/// </summary>
		private string _evaluatedAction;

		/// <summary>
		/// True if any Controller.Send operation was called.
		/// </summary>
		private bool _resetIsPostBack;
		
		/// <summary>
		/// The helper instances collected
		/// </summary>
		internal IDictionary helpers = null;

		/// <summary>
		/// The resources associated with this controller
		/// </summary>
		internal ResourceDictionary resources = null;

		/// <summary>
		/// Reference to the <see cref="IResourceFactory"/> instance
		/// </summary>
		// internal IResourceFactory resourceFactory;
		internal IDictionary _dynamicActions = new HybridDictionary(true);

		internal bool directRenderInvoked;

		internal ControllerMetaDescriptor metaDescriptor;

		internal IGenericServiceProvider serviceProvider;

		internal ValidatorRunner validator;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a Controller
		/// </summary>
		public Controller()
		{
		}

		#endregion

		#region Useful Properties

		/// <summary>
		/// This is intended to be used by MonoRail infrastructure.
		/// </summary>
		public ControllerMetaDescriptor MetaDescriptor
		{
			get { return metaDescriptor; }
		}

		/// <summary>
		/// Gets the actions available in this controller.
		/// </summary>
		/// <remarks>It is supposed to be used by MonoRail infrastructure only</remarks>
		/// <value>The actions.</value>
		public ICollection Actions
		{
			get { return metaDescriptor.Actions.Values; }
		}

		/// <summary>
		/// Gets a dicitionary of name/<see cref="IResource"/>
		/// </summary>
		/// <remarks>It is supposed to be used by MonoRail infrastructure only</remarks>
		/// <value>The resources.</value>
		public ResourceDictionary Resources
		{
			get { return resources; }
		}

		/// <summary>
		/// Gets a dictionary of name/helper instance
		/// </summary>
		/// <value>The helpers.</value>
		public IDictionary Helpers
		{
			get { return helpers; }
		}

		/// <summary>
		/// Gets a value indicating whether the request is a post.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this request is a post; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsPost
		{
			get { return context.Request.HttpMethod == "POST"; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the request is a get.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this request is a get; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsGet
		{
			get { return context.Request.HttpMethod == "GET"; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the request is a put.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this request is a put; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsPut
		{
			get { return context.Request.HttpMethod == "PUT"; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the request is a head.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this request is a head; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsHead
		{
			get { return context.Request.HttpMethod == "HEAD"; }
		}
		
		/// <summary>
		/// Gets the controller's name.
		/// </summary>
		public string Name
		{
			get { return _controllerName; }
		}

		/// <summary>
		/// Gets the controller's area name.
		/// </summary>
		public string AreaName
		{
			get { return _areaName; }
		}

		/// <summary>
		/// Gets or set the layout being used.
		/// </summary>
		public string LayoutName
		{
			get { return _layoutName; }
			set { _layoutName = value; }
		}

		/// <summary>
		/// Gets the name of the action being processed.
		/// </summary>
		public string Action
		{
			get { return _evaluatedAction; }
		}

		/// <summary>
		/// Logger for the controller
		/// </summary>
		public ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		/// <summary>
		/// Gets or sets the view which will be rendered by this action.
		/// </summary>
		public string SelectedViewName
		{
			get { return _selectedViewName; }
			set { _selectedViewName = value; }
		}

		/// <summary>
		/// Gets the property bag, which is used
		/// to pass variables to the view.
		/// </summary>
		public IDictionary PropertyBag
		{
			get { return bag; }
			set { bag = value; }
		}

		/// <summary>
		/// Gets the context of this request execution.
		/// </summary>
		public IRailsEngineContext Context
		{
			get { return context; }
		}

		/// <summary>
		/// Gets the Session dictionary.
		/// </summary>
		protected IDictionary Session
		{
			get { return context.Session; }
		}

		/// <summary>
		/// Gets a dictionary of volative items.
		/// Ideal for showing success and failures messages.
		/// </summary>
		public Flash Flash
		{
			get { return context.Flash; }
		}

		/// <summary>
		/// Gets the web context of ASP.NET API.
		/// </summary>
		protected internal HttpContext HttpContext
		{
			get { return context.UnderlyingContext; }
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
		/// Shortcut to <see cref="IRequest.Params"/> 
		/// </summary>
		public NameValueCollection Params
		{
			get { return Request.Params; }
		}
		
		/// <summary>
		/// Shortcut to <see cref="IRequest.Form"/> 
		/// </summary>
		public NameValueCollection Form
		{
			get { return Request.Form; }
		}

		/// <summary>
		/// Shortcut to <see cref="IRequest.QueryString"></see>
		/// </summary>
		public NameValueCollection Query
		{
			get { return Request.QueryString; }
		}

		public IDictionary DynamicActions
		{
			get { return _dynamicActions; }
		}

		[Obsolete("Use the DynamicActions property instead")]
		public IDictionary CustomActions
		{
			get { return _dynamicActions; }
		}

		/// <summary>
		/// Gets the validator runner instance.
		/// </summary>
		/// <value>The validator instance.</value>
		public ValidatorRunner Validator
		{
			get { return validator; }
			set { validator = value; }		
		}

		/// <summary>
		/// Gets the URL builder instance.
		/// </summary>
		/// <value>The URL builder.</value>
		public IUrlBuilder UrlBuilder
		{
			get { return (IUrlBuilder) serviceProvider.GetService(typeof(IUrlBuilder)); }
		}

		/// <summary>
		/// Shortcut to 
		/// <see cref="IResponse.IsClientConnected"/>
		/// </summary>
		protected bool IsClientConnected
		{
			get { return context.Response.IsClientConnected; }
		}

 		/// <summary>
		/// Indicates that the current Action resulted from an ASP.NET PostBack.
		/// As a result, this property is only relavent to controllers using 
		/// WebForms views.  It is placed on the base Controller for convenience 
		/// only to avoid the need to extend the Controller or provide additional 
		/// helper classes.  It is marked virtual to better support testing.
		/// </summary>
		protected virtual bool IsPostBack
		{
			get
			{
				if (_resetIsPostBack) return false;
				
				NameValueCollection fields = Context.Params;
				return (fields["__VIEWSTATE"] != null) || (fields["__EVENTTARGET"] != null);
			}
		}

		#endregion

		#region Useful Operations

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="name">view template name (the file extension is optional)</param>
		public void RenderView(string name)
		{
			string basePath = _controllerName;

			if (_areaName != null && _areaName.Length > 0)
			{
				basePath = Path.Combine(_areaName, _controllerName);
			}

			_selectedViewName = Path.Combine(basePath, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		public void RenderView(string name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();

			RenderView(name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		/// <param name="mimeType">The mime type to use on the reply</param>
		public void RenderView(string name, bool skipLayout, string mimeType)
		{
			if (skipLayout) CancelLayout();
			Response.ContentType = mimeType;

			RenderView(name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		public void RenderView(string controller, string name)
		{
			_selectedViewName = Path.Combine(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		public void RenderView(string controller, string name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();

			RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		/// <param name="mimeType">The mime type to use on the reply</param>
		public void RenderView(string controller, string name, bool skipLayout, string mimeType)
		{
			if (skipLayout) CancelLayout();
			Response.ContentType = mimeType;

			RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="mimeType">The mime type to use on the reply</param>
		public void RenderView(string controller, string name, string mimeType)
		{
			Response.ContentType = mimeType;

			RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed and results are written to System.IO.TextWriter. 
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name">The name of the view to process.</param>
		public void InPlaceRenderView(TextWriter output, string name)
		{
			string basePath = _controllerName;

			if (_areaName != null && _areaName.Length > 0)
			{
				basePath = Path.Combine(_areaName, _controllerName);
			}

			viewEngineManager.Process(output, Context, this, Path.Combine(basePath, name));			
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public void RenderSharedView(string name)
		{
			_selectedViewName = name;
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public void RenderSharedView(string name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();
			
			RenderSharedView(name);
		}

		/// <summary>
		/// Specifies the shared view to be processed and results are written to System.IO.TextWriter.
		/// (A partial view shared by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name">The name of the view to process.</param>
		public void InPlaceRenderSharedView(TextWriter output, string name)
		{
			viewEngineManager.Process(output, Context, this, name);
		}

		/// <summary>
		/// Cancels the view processing.
		/// </summary>
		public void CancelView()
		{
			_selectedViewName = null;
		}

		/// <summary>
		/// Cancels the layout processing.
		/// </summary>
		public void CancelLayout()
		{
			LayoutName = null;
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		public void RenderText(string contents)
		{
			CancelView();

			Response.Write(contents);
		}
		
		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		public void RenderText(string contents, params object[] args)
		{
			RenderText(String.Format(contents, args));
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		public void RenderText(IFormatProvider formatProvider, string contents, params object[] args)
		{
			RenderText(String.Format(formatProvider, contents, args));
		}

		/// <summary>
		/// Sends raw contents to be rendered directly by the view engine.
		/// It's up to the view engine just to apply the layout and nothing else.
		/// </summary>
		/// <param name="contents">Contents to be rendered.</param>
		public void DirectRender(string contents)
		{
			CancelView();

			if (directRenderInvoked)
			{
				throw new ControllerException("DirectRender should be called only once.");
			}

			directRenderInvoked = true;

			viewEngineManager.ProcessContents(context, this, contents);
		}

		/// <summary>
		/// Returns true if the specified template exists.
		/// </summary>
		/// <param name="templateName"></param>
		public bool HasTemplate(string templateName)
		{
			return viewEngineManager.HasTemplate(templateName);
		}

		#region RedirectToAction

		/// <summary> 
		/// Redirects to another action in the same controller.
		/// </summary>
		/// <param name="action">The action name</param>
		protected void RedirectToAction(string action)
		{
			RedirectToAction(action, (NameValueCollection) null);
		}

		/// <summary> 
		/// Redirects to another action in the same controller.
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="queryStringParameters">list of key/value pairs. Each string is supposed
		/// to have the format "key=value" that will be converted to a proper 
		/// query string</param>
		protected void RedirectToAction(string action, params String[] queryStringParameters)
		{
			RedirectToAction(action, DictHelper.Create(queryStringParameters));
		}

		/// <summary> 
		/// Redirects to another action in the same controller.
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="queryStringParameters">Query string entries</param>
		protected void RedirectToAction(string action, IDictionary queryStringParameters)
		{
			if (queryStringParameters != null)
			{
				Redirect(AreaName, Name, TransformActionName(action), queryStringParameters);
			}
			else
			{
				Redirect(AreaName, Name, TransformActionName(action));
			}
		}

		/// <summary> 
		/// Redirects to another action in the same controller.
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="queryStringParameters">Query string entries</param>
		protected void RedirectToAction(string action, NameValueCollection queryStringParameters)
		{
			if (queryStringParameters != null)
			{
				Redirect(AreaName, Name, TransformActionName(action), queryStringParameters);
			}
			else
			{
				Redirect(AreaName, Name, TransformActionName(action));
			}
		}

		#endregion

		/// <summary>
		/// Redirects to the referrer action, according to the "HTTP_REFERER" header (<c>Context.UrlReferrer</c>).
		/// </summary>
		[Obsolete("Use RedirectToReferrer")]
		protected void RedirectToReferer()
		{
			RedirectToReferrer();
		}

		/// <summary>
		/// Redirects to the referrer action, according to the "HTTP_REFERER" header (<c>Context.UrlReferrer</c>).
		/// </summary>
		protected void RedirectToReferrer()
		{
			Redirect(Context.UrlReferrer);
		}

		/// <summary> 
		/// Redirects to the site root directory (<c>Context.ApplicationPath + "/"</c>).
		/// </summary>
		public void RedirectToSiteRoot()
		{
			Redirect(Context.ApplicationPath + "/");
		}

		/// <summary>
		/// Redirects to the specified URL. All other Redirects call this one.
		/// </summary>
		/// <param name="url">Target URL</param>
		public virtual void Redirect(string url)
		{
			CancelView();

			context.Response.Redirect(url);
		}

		/// <summary>
		/// Redirects to the specified URL. 
		/// </summary>
		/// <param name="url">Target URL</param>
		/// <param name="parameters">URL parameters</param>
		public virtual void Redirect(string url, IDictionary parameters)
		{
			if (parameters != null && parameters.Count != 0)
			{
				if (url.IndexOf('?') != -1)
				{
					url = url + '&' + ToQueryString(parameters);
				}
				else
				{
					url = url + '?' + ToQueryString(parameters);
				}
			}

			Redirect(url);
		}

		/// <summary>
		/// Redirects to the specified URL. 
		/// </summary>
		/// <param name="url">Target URL</param>
		/// <param name="parameters">URL parameters</param>
		public virtual void Redirect(string url, NameValueCollection parameters)
		{
			if (parameters != null && parameters.Count != 0)
			{
				if (url.IndexOf('?') != -1)
				{
					url = url + '&' + ToQueryString(parameters);
				}
				else
				{
					url = url + '?' + ToQueryString(parameters);
				}
			}

			Redirect(url);
		}

		/// <summary>
		/// Redirects to another controller and action.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		public void Redirect(string controller, string action)
		{
			Redirect(UrlBuilder.BuildUrl(Context.UrlInfo, controller, action));
		}

		/// <summary>
		/// Redirects to another controller and action.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		public void Redirect(string area, string controller, string action)
		{
			Redirect(UrlBuilder.BuildUrl(Context.UrlInfo, area, controller, action));
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string controller, string action, NameValueCollection parameters)
		{
			Redirect(UrlBuilder.BuildUrl(Context.UrlInfo, controller, action, parameters));
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string area, string controller, string action, NameValueCollection parameters)
		{
			Redirect(UrlBuilder.BuildUrl(Context.UrlInfo, area, controller, action, parameters));
		}
		
		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string controller, string action, IDictionary parameters)
		{
			Redirect(UrlBuilder.BuildUrl(Context.UrlInfo, controller, action, parameters));
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string area, string controller, string action, IDictionary parameters)
		{
			Redirect(UrlBuilder.BuildUrl(Context.UrlInfo, area, controller, action, parameters));
		}

		protected string ToQueryString(NameValueCollection parameters)
		{
			return CommonUtils.BuildQueryString(Context.Server, parameters, false);
		}

		protected string ToQueryString(IDictionary parameters)
		{
			return CommonUtils.BuildQueryString(Context.Server, parameters, false);
		}

		#endregion

		#region Core members

		public void InitializeFieldsFromServiceProvider(IRailsEngineContext context)
		{
			serviceProvider = context;

			viewEngineManager = (IViewEngineManager) serviceProvider.GetService(typeof(IViewEngineManager));

			IControllerDescriptorProvider controllerDescriptorBuilder = (IControllerDescriptorProvider)
				serviceProvider.GetService( typeof(IControllerDescriptorProvider) );

			metaDescriptor = controllerDescriptorBuilder.BuildDescriptor(this);

			ILoggerFactory loggerFactory = (ILoggerFactory) context.GetService(typeof(ILoggerFactory));
			
			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(GetType().Name);
			}
			
			this.context = context;

			Initialize();
		}

		/// <summary>
		/// Initializes the state of the controller.
		/// </summary>
		/// <param name="areaName">Name of the area.</param>
		/// <param name="controllerName">Name of the controller.</param>
		/// <param name="actionName">Name of the action.</param>
		public void InitializeControllerState(string areaName, string controllerName, string actionName)
		{
			SetEvaluatedAction(actionName);
			_areaName = areaName;
			_controllerName = controllerName;
		}

		/// <summary>
		/// Sets the evaluated action.
		/// </summary>
		/// <param name="actionName">Name of the action.</param>
		internal void SetEvaluatedAction(string actionName)
		{
			_evaluatedAction = actionName;
		}

		/// <summary>
		/// Gets the service provider.
		/// </summary>
		/// <value>The service provider.</value>
		protected internal IServiceProvider ServiceProvider
		{
			get { return serviceProvider; }
		}

		/// <summary>
		/// Performs the specified action, which means:
		/// <br/>
		/// 1. Define the default view name<br/>
		/// 2. Run the before filters<br/>
		/// 3. Select the method related to the action name and invoke it<br/>
		/// 4. On error, execute the rescues if available<br/>
		/// 5. Run the after filters<br/>
		/// 6. Invoke the view engine<br/>
		/// </summary>
		/// <param name="action">Action name</param>
		public void Send(string action)
		{
			ResetIsPostback();
			InternalSend(action, null);
		}

		/// <summary>
		/// Performs the specified action with arguments.
		/// </summary>
		/// <param name="action">Action name</param>
		/// <param name="actionArgs">Action arguments</param>
		public void Send(string action, IDictionary actionArgs)
		{
			ResetIsPostback();
			InternalSend(action, actionArgs);
		}
	    
		/// <summary>
		/// Performs the specified action, which means:
		/// <br/>
		/// 1. Define the default view name<br/>
		/// 2. Run the before filters<br/>
		/// 3. Select the method related to the action name and invoke it<br/>
		/// 4. On error, execute the rescues if available<br/>
		/// 5. Run the after filters<br/>
		/// 6. Invoke the view engine<br/>
		/// </summary>
		/// <param name="action">Action name</param>
		/// <param name="actionArgs">Action arguments</param>
		protected virtual void InternalSend(string action, IDictionary actionArgs)
		{
			// If a redirect was sent there's no point in
			// wasting processor cycles
			
			if (Response.WasRedirected) return;
			
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("InternalSend for action '{0}'", action);
			}

			bool checkWhetherClientHasDisconnected = ShouldCheckWhetherClientHasDisconnected;

			// Nothing to do if the peer disconnected
			if (checkWhetherClientHasDisconnected && !IsClientConnected) return;

			IControllerLifecycleExecutor executor =
				(IControllerLifecycleExecutor) context.Items[ControllerLifecycleExecutor.ExecutorEntry];

			if (!executor.SelectAction(action, Name, actionArgs))
			{
				executor.PerformErrorHandling();

				executor.Dispose();
				
				return;
			}
			
			executor.ProcessSelectedAction(actionArgs);
		}


		/// <summary>
		/// Gives a chance to subclasses to format the action name properly
		/// <seealso cref="WizardStepPage"/>
		/// </summary>
		/// <param name="action">Raw action name</param>
		/// <returns>Properly formatted action name</returns>
		internal virtual string TransformActionName(string action)
		{
			return action;
		}

		private bool ShouldCheckWhetherClientHasDisconnected
		{
			get
			{
				MonoRailConfiguration conf = (MonoRailConfiguration)
											 context.GetService(typeof(MonoRailConfiguration));

				return conf.CheckClientIsConnected;
			}
		}

		/// <summary>
		/// To preserve standard Action semantics when using ASP.NET Views,
		/// the event handlers in the CodeBehind typically call <see cref="Send(String)"/>.
		/// As a result, the <see cref="IsPostBack"/> property must be logically 
		/// cleared to allow the Action to behave as if it was called directly.
		/// </summary>
		private void ResetIsPostback()
		{
			_resetIsPostBack = true;	
		}
		
		#endregion

		#region Action Invocation

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="action"></param>
		/// <param name="actions"></param>
		/// <param name="request"></param>
		/// <param name="actionArgs"></param>
		/// <returns></returns>
		protected internal virtual MethodInfo SelectMethod(string action, IDictionary actions, 
		                                          IRequest request, IDictionary actionArgs)
		{
			return actions[action] as MethodInfo;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="method"></param>
		/// <param name="methodArgs"></param>
		protected internal virtual void InvokeMethod(MethodInfo method, IDictionary methodArgs)
		{
			InvokeMethod(method, context.Request, methodArgs);
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="method"></param>
		/// <param name="request"></param>
		/// <param name="methodArgs"></param>
		protected internal virtual void InvokeMethod(MethodInfo method, IRequest request, IDictionary methodArgs)
		{
 			method.Invoke(this, new object[0]);
		}
	    
		#endregion

		#region Lifecycle (overridables)

		/// <summary>
		/// Initializes this instance. Implementors 
		/// can use this method to perform initialization
		/// </summary>
		protected virtual void Initialize()
		{
			IValidatorRegistry validatorRegistry = 
				(IValidatorRegistry) serviceProvider.GetService(typeof(IValidatorRegistry));

			validator = CreateValidatorRunner(validatorRegistry);
		}

		/// <summary>
		/// Creates the default validator runner. 
		/// </summary>
		/// <param name="validatorRegistry">The validator registry.</param>
		/// <returns></returns>
		/// <remarks>
		/// You can override this method to create a runner
		/// with some different configuration
		/// </remarks>
		protected virtual ValidatorRunner CreateValidatorRunner(IValidatorRegistry validatorRegistry)
		{
			return new ValidatorRunner(validatorRegistry);
		}

		/// <summary>
		/// Invoked by the view engine to perform
		/// any logic before the view is sent to the client.
		/// </summary>
		/// <param name="view"></param>
		public virtual void PreSendView(object view)
		{
			if (view is IControllerAware)
			{
				(view as IControllerAware).SetController(this);
			}

			if (context.Items != null)
			{
				context.Items[Constants.ControllerContextKey] = this;
			}
		}

		/// <summary>
		/// Invoked by the view engine to perform
		/// any logic after the view had been sent to the client.
		/// </summary>
		/// <param name="view"></param>
		public virtual void PostSendView(object view)
		{
		}

		#endregion

		#region Email operations

		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">
		/// Name of the template to load. 
		/// Will look in Views/mail for that template file.
		/// </param>
		/// <returns>An instance of <see cref="Message"/></returns>
		public Message RenderMailMessage(string templateName)
		{
			return RenderMailMessage(templateName, false);
		}

		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">
		/// Name of the template to load. 
		/// Will look in Views/mail for that template file.
		/// </param>
		/// <param name="doNotApplyLayout">If <c>true</c>, it will skip the layout</param>
		/// <returns>An instance of <see cref="Message"/></returns>
		public Message RenderMailMessage(string templateName, bool doNotApplyLayout)
		{
			IEmailTemplateService templateService = (IEmailTemplateService)
				ServiceProvider.GetService(typeof(IEmailTemplateService));

			return templateService.RenderMailMessage(templateName, Context, this, doNotApplyLayout);
		}

		/// <summary>
		/// Attempts to deliver the Message using the server specified on the web.config.
		/// </summary>
		/// <param name="message">The instance of System.Web.Mail.MailMessage that will be sent</param>
		public void DeliverEmail(Message message)
		{
			try
			{
				IEmailSender sender = (IEmailSender) ServiceProvider.GetService( typeof(IEmailSender) );

				sender.Send(message);
			}
			catch(Exception ex)
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error("Error sending e-mail", ex);
				}
				
				throw new RailsException("Error sending e-mail", ex);
			}
		}

		/// <summary>
		/// Renders and delivers the e-mail message.
		/// <seealso cref="DeliverEmail"/>
		/// </summary>
		/// <param name="templateName"></param>
		public void RenderEmailAndSend(string templateName)
		{
			Message message = RenderMailMessage(templateName);
			DeliverEmail(message);
		}

		#endregion

		internal class EmptyController : Controller
		{
			public EmptyController(IRailsEngineContext context)
			{
				InitializeFieldsFromServiceProvider(context);
			}
		}
	}
}
