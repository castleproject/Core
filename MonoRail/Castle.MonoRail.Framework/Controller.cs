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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.IO;
	using System.Reflection;
	using System.Web;
	using Castle.Components.Binder;
	using Castle.Components.Common.EmailSender;
	using Castle.Components.Validator;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Descriptors;
	using Castle.MonoRail.Framework.Resources;
	using Castle.MonoRail.Framework.Internal;
	using Core;
	using Helpers;

	/// <summary>
	/// Implements the core functionality and exposes the
	/// common methods for concrete controllers.
	/// </summary>
	public abstract class Controller : IAsyncController, IValidatorAccessor, IRedirectSupport
	{
		private IEngineContext engineContext;
		private IControllerContext context;
		private ILogger logger = NullLogger.Instance;
		private bool directRenderInvoked;
		private bool isContextualized;

		private IHelperFactory helperFactory;
		private IServiceInitializer serviceInitializer;
		private IFilterFactory filterFactory;
		private IViewEngineManager viewEngineManager;
		private IActionSelector actionSelector;
		private IScaffoldingSupport scaffoldSupport;
		private FilterDescriptor[] filters = new FilterDescriptor[0];
		private ValidatorRunner validatorRunner;
		private Dictionary<object, ErrorSummary> validationSummaryPerInstance;
		private Dictionary<object, ErrorList> boundInstances;
		private ErrorSummary simplerErrorList = new ErrorSummary();
		private RenderingSupport renderingSupport;
		private IDynamicActionProviderFactory dynamicActionProviderFactory;
		private DynamicActionProviderDescriptor[] dynamicActionProviders = new DynamicActionProviderDescriptor[0];

		#region IController

		/// <summary>
		/// Occurs just before the action execution.
		/// </summary>
		public event ControllerHandler BeforeAction;

		/// <summary>
		/// Occurs just after the action execution.
		/// </summary>
		public event ControllerHandler AfterAction;

		/// <summary>
		/// Sets the context for the controller
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="context">The controller context.</param>
		public virtual void Contextualize(IEngineContext engineContext, IControllerContext context)
		{
			this.context = context;
			SetEngineContext(engineContext);
			renderingSupport = new RenderingSupport(context, engineContext);
			isContextualized = true;
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
		/// <param name="engineContext">The engine context.</param>
		/// <param name="context">The controller context.</param>
		public virtual void Process(IEngineContext engineContext, IControllerContext context)
		{
			PrepareToExecuteAction(engineContext, context);
			Initialize();

			RunActionAndRenderView();
		}

		private void PrepareToExecuteAction(IEngineContext engineContext, IControllerContext context)
		{
			if (isContextualized == false)
			{
				Contextualize(engineContext, context);
			}

			ResolveLayout();
			CreateAndInitializeHelpers();
			CreateFiltersDescriptors();
			CreateDynamicActionProvidersDescriptors();
			ProcessDynamicActionProviders();
			ProcessScaffoldIfAvailable();
		}

		/// <summary>
		/// Invoked by the view engine to perform
		/// any logic before the view is sent to the client.
		/// </summary>
		/// <param name="view"></param>
		public virtual void PreSendView(object view)
		{
		}

		/// <summary>
		/// Invoked by the view engine to perform
		/// any logic after the view had been sent to the client.
		/// </summary>
		/// <param name="view"></param>
		public virtual void PostSendView(object view)
		{
		}

		/// <summary>
		/// Begin to perform the specified async action, which means:
		/// <br/>
		/// 1. Define the default view name<br/>
		/// 2. Run the before filters<br/>
		/// 3. Select the begin method related to the action name and invoke it<br/>
		/// 4. Return the result of the async method start and let ASP.Net wait on it
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="context">The controller context.</param>
		/// <returns></returns>
		/// <remarks>
		/// The async infomrmation about this call is pass using the controller context Async property
		/// </remarks>
		public IAsyncResult BeginProcess(IEngineContext engineContext, IControllerContext context)
		{
			PrepareToExecuteAction(engineContext, context);
			Initialize();

			IExecutableAction action = null;
			Exception actionException = null;
			bool cancel;

			try
			{
				action = SelectAction(Action, ActionType.AsyncBegin);

				if (action == null)
				{
					throw new MonoRailException(404, "Not Found", "Could not find action named " +
					                                              Action + " on controller " + AreaName + "\\" + Name);
				}

				EnsureActionIsAccessibleWithCurrentHttpVerb(action);

				RunBeforeActionFilters(action, out cancel);

				CreateControllerLevelResources();
				CreateActionLevelResources(action);
				ResolveLayout(action);

				if (cancel)
				{
					return new FailedToExecuteBeginActionAsyncResult();
				}

				if (BeforeAction != null)
				{
					BeforeAction(action, engineContext, this, context);
				}

				return (IAsyncResult) action.Execute(engineContext, this, context);
			}
			catch(MonoRailException ex)
			{
				if (Response.StatusCode == 200 && ex.HttpStatusCode.HasValue)
				{
					Response.StatusCode = ex.HttpStatusCode.Value;
					Response.StatusDescription = ex.HttpStatusDesc;
				}

				actionException = ex;

				RegisterExceptionAndNotifyExtensions(actionException);

				RunAfterActionFilters(action, out cancel);

				if (!ProcessRescue(action, actionException))
				{
					throw;
				}
				return new FailedToExecuteBeginActionAsyncResult();
			}
			catch(Exception ex)
			{
				if (Response.StatusCode == 200)
				{
					Response.StatusCode = 500;
					Response.StatusDescription = "Error processing action";
				}

				actionException = (ex is TargetInvocationException) ? ex.InnerException : ex;

				RegisterExceptionAndNotifyExtensions(actionException);

				RunAfterActionFilters(action, out cancel);

				if (!ProcessRescue(action, actionException))
				{
					throw;
				}
				return new FailedToExecuteBeginActionAsyncResult();
			}
		}

		/// <summary>
		/// Complete processing of the request:<br/>
		/// 1. Execute end method related to the action name<br/>
		/// 2. On error, execute the rescues if available<br/>
		/// 3. Run the after filters<br/>
		/// 4. Invoke the view engine<br/>
		/// </summary>
		/// <remarks>
		/// The async infomrmation about this call is pass using the controller context Async property
		/// </remarks>
		public void EndProcess()
		{
			PrepareToExecuteAction(engineContext, context);

			IExecutableAction action = null;
			Exception actionException = null;
			bool cancel;

			try
			{
				action = SelectAction(Action, ActionType.AsyncEnd);
				ResolveLayout(action);

				object actionRetValue = action.Execute(engineContext, this, context);

				// TO DO: review/refactor this code
				if (action.ReturnBinderDescriptor != null)
				{
					IReturnBinder binder = action.ReturnBinderDescriptor.ReturnTypeBinder;

					// Runs return binder and keep going
					binder.Bind(Context, this, ControllerContext, action.ReturnBinderDescriptor.ReturnType, actionRetValue);
				}

				// Action executed successfully, so it's safe to process the cache configurer
				if ((MetaDescriptor.CacheConfigurer != null || action.CachePolicyConfigurer != null) &&
				    !Response.WasRedirected && Response.StatusCode == 200)
				{
					ConfigureCachePolicy(action);
				}
			}
			catch(MonoRailException ex)
			{
				if (Response.StatusCode == 200 && ex.HttpStatusCode.HasValue)
				{
					Response.StatusCode = ex.HttpStatusCode.Value;
					Response.StatusDescription = ex.HttpStatusDesc;
				}

				actionException = ex;

				RegisterExceptionAndNotifyExtensions(actionException);

				RunAfterActionFilters(action, out cancel);

				if (!ProcessRescue(action, actionException))
				{
					throw;
				}
				return;
			}
			catch(Exception ex)
			{
				if (Response.StatusCode == 200)
				{
					Response.StatusCode = 500;
					Response.StatusDescription = "Error processing action";
				}

				actionException = (ex is TargetInvocationException) ? ex.InnerException : ex;

				RegisterExceptionAndNotifyExtensions(actionException);

				RunAfterActionFilters(action, out cancel);

				if (!ProcessRescue(action, actionException))
				{
					throw;
				}
				return;
			}
			finally
			{
				// AfterAction event: always executed
				if (AfterAction != null)
				{
					AfterAction(action, engineContext, this, context);
				}
			}

			RunAfterActionFilters(action, out cancel);
			if (cancel)
			{
				return;
			}

			if (engineContext.Response.WasRedirected) // No need to process view or rescue in this case
			{
				return;
			}

			if (context.SelectedViewName != null)
			{
				ProcessView();
				RunAfterRenderingFilters(action);
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			DisposeFilters();
			DisposeDynamicActionProviders();
		}

		#endregion

		#region IValidatorAccessor

		/// <summary>
		/// Gets the validator runner instance.
		/// </summary>
		/// <value>The validator instance.</value>
		public ValidatorRunner Validator
		{
			get
			{
				if (validatorRunner == null)
				{
					validatorRunner = CreateValidatorRunner(engineContext.Services.ValidatorRegistry);
				}
				return validatorRunner;
			}
			set { validatorRunner = value; }
		}

		/// <summary>
		/// Gets the bound instance errors. These are errors relative to
		/// the binding process performed for the specified instance, nothing more.
		/// </summary>
		/// <value>The bound instance errors.</value>
		public IDictionary<object, ErrorList> BoundInstanceErrors
		{
			get
			{
				if (boundInstances == null)
				{
					boundInstances = new Dictionary<object, ErrorList>();
				}
				return boundInstances;
			}
		}

		/// <summary>
		/// Populates the validator error summary with errors relative to the
		/// validation rules associated with the target type.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="binderUsedForBinding">The binder used for binding.</param>
		public void PopulateValidatorErrorSummary(object instance, ErrorSummary binderUsedForBinding)
		{
			if (validationSummaryPerInstance == null)
			{
				validationSummaryPerInstance = new Dictionary<object, ErrorSummary>();
			}
			validationSummaryPerInstance[instance] = binderUsedForBinding;
		}

		/// <summary>
		/// Gets the error summary associated with validation errors.
		/// <para>
		/// Will only work for instances populated by the <c>DataBinder</c>
		/// </para>
		/// </summary>
		/// <param name="instance">object instance</param>
		/// <returns>Error summary instance (can be null if the DataBinder wasn't configured to validate)</returns>
		protected ErrorSummary GetErrorSummary(object instance)
		{
			if (validationSummaryPerInstance == null)
			{
				return null;
			}
			return validationSummaryPerInstance.ContainsKey(instance) ? validationSummaryPerInstance[instance] : null;
		}

		/// <summary>
		/// Gets an error summary not associated with an object instance. This is useful 
		/// to register errors not associated with a data binding object.
		/// </summary>
		/// <value>The error summary.</value>
		public ErrorSummary SimpleErrorSummary
		{
			get { return simplerErrorList; }
		}

		/// <summary>
		/// Returns <c>true</c> if the given instance had 
		/// validation errors during binding.
		/// <para>
		/// Will only work for instances populated by the <c>DataBinder</c>
		/// </para>
		/// </summary>
		/// <param name="instance">object instance</param>
		/// <returns><c>true</c> if the validation had an error</returns>
		protected bool HasValidationError(object instance)
		{
			ErrorSummary summary = GetErrorSummary(instance);

			if (summary == null)
			{
				return false;
			}

			return summary.ErrorsCount != 0;
		}

		/// <summary>
		/// Gets a list of errors that were thrown during the 
		/// object process, like conversion errors.
		/// </summary>
		/// <param name="instance">The instance that was populated by a binder.</param>
		/// <returns>List of errors</returns>
		protected ErrorList GetDataBindErrors(object instance)
		{
			if (boundInstances != null && boundInstances.ContainsKey(instance))
			{
				return boundInstances[instance];
			}
			return null;
		}

		#endregion

		#region Useful Properties

		/// <summary>
		/// Gets the controller context.
		/// </summary>
		/// <value>The controller context.</value>
		public IControllerContext ControllerContext
		{
			get { return context; }
			set { context = value; }
		}

		/// <summary>
		/// Gets the view folder -- (areaname + 
		/// controllername) or just controller name -- that this controller 
		/// will use by default.
		/// </summary>
		public string ViewFolder
		{
			get { return context.ViewFolder; }
			set { context.ViewFolder = value; }
		}

		/// <summary>
		/// This is intended to be used by MonoRail infrastructure.
		/// </summary>
		public ControllerMetaDescriptor MetaDescriptor
		{
			get { return context.ControllerDescriptor; }
			set { context.ControllerDescriptor = value; }
		}

		/// <summary>
		/// Gets the actions available in this controller.
		/// </summary>
		/// <remarks>It is supposed to be used by MonoRail infrastructure only</remarks>
		/// <value>The actions.</value>
		public ICollection Actions
		{
			get { return MetaDescriptor.Actions.Values; }
		}

		/// <summary>
		/// Gets a dicitionary of name/<see cref="IResource"/>
		/// </summary>
		/// <remarks>It is supposed to be used by MonoRail infrastructure only</remarks>
		/// <value>The resources.</value>
		public IDictionary<string, IResource> Resources
		{
			get { return context.Resources; }
		}

		/// <summary>
		/// Gets a dictionary of name/helper instance
		/// </summary>
		/// <value>The helpers.</value>
		public IDictionary Helpers
		{
			get { return context.Helpers; }
		}

		/// <summary>
		/// Gets a value indicating whether the request is a post.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this request is a post; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsPost
		{
			get { return engineContext.Request.HttpMethod == "POST"; }
		}

		/// <summary>
		/// Gets a value indicating whether the request is a get.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this request is a get; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsGet
		{
			get { return engineContext.Request.HttpMethod == "GET"; }
		}

		/// <summary>
		/// Gets a value indicating whether the request is a put.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this request is a put; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsPut
		{
			get { return engineContext.Request.HttpMethod == "PUT"; }
		}

		/// <summary>
		/// Gets a value indicating whether the request is a head.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this request is a head; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsHead
		{
			get { return engineContext.Request.HttpMethod == "HEAD"; }
		}

		/// <summary>
		/// Gets the controller's name.
		/// </summary>
		public string Name
		{
			get { return context.Name; }
		}

		/// <summary>
		/// Gets the controller's area name.
		/// </summary>
		public string AreaName
		{
			get { return context.AreaName; }
		}

		/// <summary>
		/// Gets or set the layout being used.
		/// </summary>
		public string LayoutName
		{
			get { return (context.LayoutNames != null && context.LayoutNames.Length != 0) ? context.LayoutNames[0] : null; }
			set
			{
				if (value == null)
				{
					context.LayoutNames = null;
				}
				else
				{
					context.LayoutNames = new string[] {value};
				}
			}
		}

		/// <summary>
		/// Gets or set the layouts being used.
		/// </summary>
		public string[] LayoutNames
		{
			get { return context.LayoutNames; }
			set { context.LayoutNames = value; }
		}

		/// <summary>
		/// Gets the name of the action being processed.
		/// </summary>
		public string Action
		{
			get { return context.Action; }
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
			get { return context.SelectedViewName; }
			set { context.SelectedViewName = value; }
		}

		/// <summary>
		/// Gets the property bag, which is used
		/// to pass variables to the view.
		/// </summary>
		public IDictionary PropertyBag
		{
			get { return context.PropertyBag; }
			set { context.PropertyBag = value; }
		}

		/// <summary>
		/// Gets the context of this request execution.
		/// </summary>
		public IEngineContext Context
		{
			get { return engineContext; }
		}

		/// <summary>
		/// Gets the Session dictionary.
		/// </summary>
		protected IDictionary Session
		{
			get { return engineContext.Session; }
		}

		/// <summary>
		/// Gets a dictionary of volative items.
		/// Ideal for showing success and failures messages.
		/// </summary>
		public Flash Flash
		{
			get { return engineContext.Flash; }
		}

		/// <summary>
		/// Gets the web context of ASP.NET API.
		/// </summary>
		protected internal HttpContext HttpContext
		{
			get { return engineContext.UnderlyingContext; }
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

		/// <summary>
		/// Gets the dynamic actions dictionary. 
		/// <para>
		/// Can be used to insert dynamic actions on the controller instance.
		/// </para>
		/// </summary>
		/// <value>The dynamic actions dictionary.</value>
		public IDictionary<string, IDynamicAction> DynamicActions
		{
			get { return context.DynamicActions; }
		}

		/// <summary>
		/// Shortcut to 
		/// <see cref="IResponse.IsClientConnected"/>
		/// </summary>
		protected bool IsClientConnected
		{
			get { return engineContext.Response.IsClientConnected; }
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
				NameValueCollection fields = Params;
				return (fields["__VIEWSTATE"] != null) || (fields["__EVENTTARGET"] != null);
			}
		}

		#endregion

		#region Useful Operations

		/// <summary>
		/// Sets the engine context. Also initialize all required services by querying
		/// <see cref="IEngineContext.Services"/>
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		public virtual void SetEngineContext(IEngineContext engineContext)
		{
			this.engineContext = engineContext;

			helperFactory = engineContext.Services.HelperFactory; // should not be null
			serviceInitializer = engineContext.Services.ServiceInitializer; // should not be null
			filterFactory = engineContext.Services.FilterFactory; // should not be null
			viewEngineManager = engineContext.Services.ViewEngineManager; // should not be null
			actionSelector = engineContext.Services.ActionSelector; // should not be null
			scaffoldSupport = engineContext.Services.ScaffoldingSupport; // might be null
			dynamicActionProviderFactory = engineContext.Services.DynamicActionProviderFactory; // should not be null
		}

		#region From RenderingSupport

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="name">view template name (the file extension is optional)</param>
		public virtual void RenderView(string name)
		{
			renderingSupport.RenderView(name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		public virtual void RenderView(string name, bool skipLayout)
		{
			renderingSupport.RenderView(name, skipLayout);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		/// <param name="mimeType">The mime type to use on the reply</param>
		public virtual void RenderView(string name, bool skipLayout, string mimeType)
		{
			renderingSupport.RenderView(name, skipLayout, mimeType);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		public virtual void RenderView(string controller, string name)
		{
			renderingSupport.RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		public virtual void RenderView(string controller, string name, bool skipLayout)
		{
			renderingSupport.RenderView(controller, name, skipLayout);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		/// <param name="mimeType">The mime type to use on the reply</param>
		public virtual void RenderView(string controller, string name, bool skipLayout, string mimeType)
		{
			renderingSupport.RenderView(controller, name, skipLayout, mimeType);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="mimeType">The mime type to use on the reply</param>
		public virtual void RenderView(string controller, string name, string mimeType)
		{
			renderingSupport.RenderView(controller, name, mimeType);
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public virtual void RenderSharedView(string name)
		{
			renderingSupport.RenderSharedView(name);
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public virtual void RenderSharedView(string name, bool skipLayout)
		{
			renderingSupport.RenderSharedView(name, skipLayout);
		}

		/// <summary>
		/// Cancels the view processing.
		/// </summary>
		public virtual void CancelView()
		{
			renderingSupport.CancelView();
		}

		/// <summary>
		/// Cancels the layout processing.
		/// </summary>
		public virtual void CancelLayout()
		{
			renderingSupport.CancelLayout();
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		public virtual void RenderText(string contents)
		{
			renderingSupport.RenderText(contents);
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		public virtual void RenderText(string contents, params object[] args)
		{
			renderingSupport.RenderText(contents, args);
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		public virtual void RenderText(IFormatProvider formatProvider, string contents, params object[] args)
		{
			renderingSupport.RenderText(formatProvider, contents, args);
		}

		#endregion

		/// <summary>
		/// Specifies the view to be processed and results are written to System.IO.TextWriter. 
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name">The name of the view to process.</param>
		public void InPlaceRenderView(TextWriter output, string name)
		{
			viewEngineManager.Process(Path.Combine(ViewFolder, name), output, Context, this, context);
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
			viewEngineManager.Process(name, output, Context, this, context);
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

			viewEngineManager.RenderStaticWithinLayout(contents, engineContext, this, context);
		}

		/// <summary>
		/// Returns true if the specified template exists.
		/// </summary>
		/// <param name="templateName"></param>
		public bool HasTemplate(string templateName)
		{
			return viewEngineManager.HasTemplate(templateName);
		}

		#region Redirects

		/// <summary>
		/// Redirects to another action in the same controller.
		/// </summary>
		/// <param name="action">The action name</param>
		public void RedirectToAction(string action)
		{
			RedirectToAction(action, (NameValueCollection) null);
		}

		/// <summary>
		/// Redirects to another action in the same controller passing the specified querystring parameters.
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void RedirectToAction(string action, IDictionary queryStringParameters)
		{
			if (queryStringParameters != null)
			{
				Response.Redirect(AreaName, Name, TransformActionName(action), queryStringParameters);
			}
			else
			{
				Response.Redirect(AreaName, Name, TransformActionName(action));
			}
		}

		/// <summary>
		/// Redirects to another action in the same controller passing the specified querystring parameters.
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void RedirectToAction(string action, NameValueCollection queryStringParameters)
		{
			if (queryStringParameters != null)
			{
				Response.Redirect(AreaName, Name, TransformActionName(action), queryStringParameters);
			}
			else
			{
				Response.Redirect(AreaName, Name, TransformActionName(action));
			}
		}

		/// <summary>
		/// Redirects to another action in the same controller passing the specified querystring parameters.
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void RedirectToAction(string action, params string[] queryStringParameters)
		{
			RedirectToAction(action, DictHelper.Create(queryStringParameters));
		}

		/// <summary>
		/// Redirects to another action in the same controller passing the specified querystring parameters.
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="queryStringAnonymousDictionary">The querystring entries as an anonymous dictionary</param>
		public void RedirectToAction(string action, object queryStringAnonymousDictionary)
		{
			RedirectToAction(action, new ReflectionBasedDictionaryAdapter(queryStringAnonymousDictionary));
		}

		/// <summary>
		/// Redirects to url using referrer.
		/// </summary>
		public void RedirectToReferrer()
		{
			Response.RedirectToReferrer();
		}

		/// <summary>
		/// Redirects to the site root directory (<c>Context.ApplicationPath + "/"</c>).
		/// </summary>
		public void RedirectToSiteRoot()
		{
			Response.RedirectToSiteRoot();
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		public void RedirectToUrl(string url)
		{
			Response.RedirectToUrl(url);
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="endProcess">if set to <c>true</c>, sends the redirect and
		/// kills the current request process.</param>
		public void RedirectToUrl(string url, bool endProcess)
		{
			Response.RedirectToUrl(url, endProcess);
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void RedirectToUrl(string url, IDictionary queryStringParameters)
		{
			Response.RedirectToUrl(url, queryStringParameters);
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void RedirectToUrl(string url, NameValueCollection queryStringParameters)
		{
			Response.RedirectToUrl(url, queryStringParameters);
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void RedirectToUrl(string url, params string[] queryStringParameters)
		{
			Response.RedirectToUrl(url, queryStringParameters);
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="queryStringAnonymousDictionary">The querystring entries as an anonymous dictionary</param>
		public void RedirectToUrl(string url, object queryStringAnonymousDictionary)
		{
			Response.RedirectToUrl(url, queryStringAnonymousDictionary);
		}

		/// <summary>
		/// Redirects to another controller's action.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		public void Redirect(string controller, string action)
		{
			Response.Redirect(controller, action);
		}

		/// <summary>
		/// Redirects to another controller's action with the specified parameters.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void Redirect(string controller, string action, NameValueCollection queryStringParameters)
		{
			Response.Redirect(controller, action, queryStringParameters);
		}

		/// <summary>
		/// Redirects to another controller's action with the specified parameters.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void Redirect(string controller, string action, IDictionary queryStringParameters)
		{
			Response.Redirect(controller, action, queryStringParameters);
		}

		/// <summary>
		/// Redirects to another controller's action with the specified parameters.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringAnonymousDictionary">The querystring entries as an anonymous dictionary</param>
		public void Redirect(string controller, string action, object queryStringAnonymousDictionary)
		{
			Response.Redirect(controller, action, queryStringAnonymousDictionary);
		}

		/// <summary>
		/// Redirects to another controller's action in a different area.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		public void Redirect(string area, string controller, string action)
		{
			Response.Redirect(area, controller, action);
		}

		/// <summary>
		/// Redirects to another controller's action in a different area with the specified parameters.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void Redirect(string area, string controller, string action, IDictionary queryStringParameters)
		{
			Response.Redirect(area, controller, action, queryStringParameters);
		}

		/// <summary>
		/// Redirects to another controller's action in a different area with the specified parameters.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void Redirect(string area, string controller, string action, NameValueCollection queryStringParameters)
		{
			Response.Redirect(area, controller, action, queryStringParameters);
		}

		/// <summary>
		/// Redirects to another controller's action in a different area with the specified parameters.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringAnonymousDictionary">The querystring entries as an anonymous dictionary</param>
		public void Redirect(string area, string controller, string action, object queryStringAnonymousDictionary)
		{
			Response.Redirect(area, controller, action, queryStringAnonymousDictionary);
		}

		/// <summary>
		/// Redirects using a named route.
		/// The name must exists otherwise a <see cref="MonoRailException"/> will be thrown.
		/// </summary>
		/// <param name="routeName">Route name.</param>
		public void RedirectUsingNamedRoute(string routeName)
		{
			Response.RedirectUsingNamedRoute(routeName);
		}

		/// <summary>
		/// Redirects using a named route.
		/// The name must exists otherwise a <see cref="MonoRailException"/> will be thrown.
		/// </summary>
		/// <param name="routeName">Route name.</param>
		/// <param name="routeParameters">The route parameters.</param>
		public void RedirectUsingNamedRoute(string routeName, object routeParameters)
		{
			Response.RedirectUsingNamedRoute(routeName, routeParameters);
		}

		/// <summary>
		/// Redirects using a named route.
		/// The name must exists otherwise a <see cref="MonoRailException"/> will be thrown.
		/// </summary>
		/// <param name="routeName">Route name.</param>
		/// <param name="routeParameters">The route parameters.</param>
		public void RedirectUsingNamedRoute(string routeName, IDictionary routeParameters)
		{
			Response.RedirectUsingNamedRoute(routeName, routeParameters);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="useCurrentRouteParams">if set to <c>true</c> the current request matching route rules will be used.</param>
		public void RedirectUsingRoute(string action, bool useCurrentRouteParams)
		{
			Response.RedirectUsingRoute(Name, action, useCurrentRouteParams);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		public void RedirectUsingRoute(string action, IDictionary routeParameters)
		{
			Response.RedirectUsingRoute(Name, action, routeParameters);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		public void RedirectUsingRoute(string action, object routeParameters)
		{
			Response.RedirectUsingRoute(Name, action, routeParameters);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="useCurrentRouteParams">if set to <c>true</c> the current request matching route rules will be used.</param>
		public void RedirectUsingRoute(string controller, string action, bool useCurrentRouteParams)
		{
			Response.RedirectUsingRoute(controller, action, useCurrentRouteParams);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="useCurrentRouteParams">if set to <c>true</c> the current request matching route rules will be used.</param>
		public void RedirectUsingRoute(string area, string controller, string action, bool useCurrentRouteParams)
		{
			Response.RedirectUsingRoute(area, controller, action, useCurrentRouteParams);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		public void RedirectUsingRoute(string controller, string action, IDictionary routeParameters)
		{
			Response.RedirectUsingRoute(controller, action, routeParameters);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		public void RedirectUsingRoute(string controller, string action, object routeParameters)
		{
			Response.RedirectUsingRoute(controller, action, routeParameters);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		public void RedirectUsingRoute(string area, string controller, string action, IDictionary routeParameters)
		{
			Response.RedirectUsingRoute(area, controller, action, routeParameters);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		public void RedirectUsingRoute(string area, string controller, string action, object routeParameters)
		{
			Response.RedirectUsingRoute(area, controller, action, routeParameters);
		}

		#endregion

		#region Redirect utilities

		/// <summary>
		/// Creates a querystring string representation of the namevalue collection.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		protected string ToQueryString(NameValueCollection parameters)
		{
			return CommonUtils.BuildQueryString(Context.Server, parameters, false);
		}

		/// <summary>
		/// Creates a querystring string representation of the entries in the dictionary.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		protected string ToQueryString(IDictionary parameters)
		{
			return CommonUtils.BuildQueryString(Context.Server, parameters, false);
		}

		#endregion

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
		[Obsolete]
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
		[Obsolete]
		public Message RenderMailMessage(string templateName, bool doNotApplyLayout)
		{
			IEmailTemplateService templateService = engineContext.Services.EmailTemplateService;
			return templateService.RenderMailMessage(templateName, Context, this, ControllerContext, doNotApplyLayout);
		}

		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">Name of the template to load.
		/// Will look in Views/mail for that template file.</param>
		/// <param name="layoutName">Name of the layout.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>An instance of <see cref="Message"/></returns>
		public Message RenderMailMessage(string templateName, string layoutName, IDictionary parameters)
		{
			IEmailTemplateService templateService = engineContext.Services.EmailTemplateService;
			return templateService.RenderMailMessage(templateName, layoutName, parameters);
		}

		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">Name of the template to load.
		/// Will look in Views/mail for that template file.</param>
		/// <param name="layoutName">Name of the layout.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>An instance of <see cref="Message"/></returns>
		public Message RenderMailMessage(string templateName, string layoutName, IDictionary<string, object> parameters)
		{
			IEmailTemplateService templateService = engineContext.Services.EmailTemplateService;
			return templateService.RenderMailMessage(templateName, layoutName, parameters);
		}

		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">Name of the template to load.
		/// Will look in Views/mail for that template file.</param>
		/// <param name="layoutName">Name of the layout.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>An instance of <see cref="Message"/></returns>
		public Message RenderMailMessage(string templateName, string layoutName, object parameters)
		{
			IEmailTemplateService templateService = engineContext.Services.EmailTemplateService;
			return templateService.RenderMailMessage(templateName, layoutName, parameters);
		}

		/// <summary>
		/// Attempts to deliver the Message using the server specified on the web.config.
		/// </summary>
		/// <param name="message">The instance of System.Web.Mail.MailMessage that will be sent</param>
		public void DeliverEmail(Message message)
		{
			try
			{
				IEmailSender sender = engineContext.Services.EmailSender;
				sender.Send(message);
			}
			catch(Exception ex)
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error("Error sending e-mail", ex);
				}

				throw new MonoRailException("Error sending e-mail", ex);
			}
		}

		/// <summary>
		/// Renders and delivers the e-mail message.
		/// <seealso cref="DeliverEmail"/>
		/// </summary>
		/// <param name="templateName"></param>
		[Obsolete]
		public void RenderEmailAndSend(string templateName)
		{
			Message message = RenderMailMessage(templateName);
			DeliverEmail(message);
		}

		#endregion

		#region Lifecycle (overridables)

		/// <summary>
		/// Initializes this instance. Implementors 
		/// can use this method to perform initialization
		/// </summary>
		public virtual void Initialize()
		{
		}

		#endregion

		#region Resources/i18n

		/// <summary>
		/// Creates the controller level resources.
		/// </summary>
		protected virtual void CreateControllerLevelResources()
		{
			CreateResources(MetaDescriptor.Resources);
		}

		/// <summary>
		/// Creates the controller level resources.
		/// </summary>
		/// <param name="action">The action.</param>
		protected virtual void CreateActionLevelResources(IExecutableAction action)
		{
			CreateResources(action.Resources);
		}

		/// <summary>
		/// Creates the resources and adds them to the <see cref="IControllerContext.Resources"/>.
		/// </summary>
		/// <param name="resources">The resources.</param>
		protected virtual void CreateResources(ResourceDescriptor[] resources)
		{
			if (resources == null || resources.Length == 0)
			{
				return;
			}

			Assembly typeAssembly = GetType().Assembly;

			IResourceFactory resourceFactory = engineContext.Services.ResourceFactory;

			foreach(ResourceDescriptor resDesc in resources)
			{
				if (ControllerContext.Resources.ContainsKey(resDesc.Name))
				{
					throw new MonoRailException("There is a duplicated entry on the resource dictionary. Resource entry name: " +
					                            resDesc.Name);
				}

				ControllerContext.Resources.Add(resDesc.Name, resourceFactory.Create(resDesc, typeAssembly));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		protected virtual void CreateTransformsFilters(IExecutableAction action)
		{
			if (action == null)
			{
				return;
			}

			ITransformFilterFactory transformFilterFactory = engineContext.Services.TransformFilterFactory;

			foreach(TransformFilterDescriptor transformFilter in action.TransformFilters)
			{
				ITransformFilter filter = transformFilterFactory.Create(transformFilter.TransformFilterType,
				                                                        engineContext.UnderlyingContext.Response.Filter);
				engineContext.UnderlyingContext.Response.Filter = filter as Stream;
			}
		}

		#endregion

		/// <summary>
		/// Resolves the layout override for the specified action
		/// </summary>
		/// <param name="action">The action.</param>
		protected virtual void ResolveLayout(IExecutableAction action)
		{
			if (action.LayoutOverride != null && action.LayoutOverride.Length != 0)
			{
				context.LayoutNames = action.LayoutOverride;
			}
		}

		/// <summary>
		/// Gives a change to subclass 
		/// to override the layout resolution code
		/// </summary>
		protected virtual void ResolveLayout()
		{
			context.LayoutNames = ObtainDefaultLayoutName();
		}


		private void RunActionAndRenderView()
		{
			IExecutableAction action = null;
			Exception actionException = null;
			bool cancel;

			try
			{
				action = SelectAction(Action, ActionType.Sync);

				if (action == null)
				{
					throw new MonoRailException(404, "Not Found", "Could not find action named " +
					                                              Action + " on controller " + AreaName + "\\" + Name);
				}

				EnsureActionIsAccessibleWithCurrentHttpVerb(action);

				RunBeforeActionFilters(action, out cancel);

				if (cancel)
				{
					return;
				}

				CreateControllerLevelResources();
				CreateActionLevelResources(action);
				CreateTransformsFilters(action);
				ResolveLayout(action);

				if (cancel)
				{
					return;
				}

				if (BeforeAction != null)
				{
					BeforeAction(action, engineContext, this, context);
				}

				object actionRetValue = action.Execute(engineContext, this, context);

				// TO DO: review/refactor this code
				if (action.ReturnBinderDescriptor != null)
				{
					IReturnBinder binder = action.ReturnBinderDescriptor.ReturnTypeBinder;

					// Runs return binder and keep going
					binder.Bind(Context, this, ControllerContext, action.ReturnBinderDescriptor.ReturnType, actionRetValue);
				}

				// Action executed successfully, so it's safe to process the cache configurer
				if ((MetaDescriptor.CacheConfigurer != null || action.CachePolicyConfigurer != null) &&
				    !Response.WasRedirected && Response.StatusCode == 200)
				{
					ConfigureCachePolicy(action);
				}
			}
			catch(MonoRailException ex)
			{
				if (Response.StatusCode == 200 && ex.HttpStatusCode.HasValue)
				{
					Response.StatusCode = ex.HttpStatusCode.Value;
					Response.StatusDescription = ex.HttpStatusDesc;
				}

				actionException = ex;

				RegisterExceptionAndNotifyExtensions(actionException);

				RunAfterActionFilters(action, out cancel);

				if (!ProcessRescue(action, actionException))
				{
					throw;
				}
				return;
			}
			catch(Exception ex)
			{
				if (Response.StatusCode == 200)
				{
					Response.StatusCode = 500;
					Response.StatusDescription = "Error processing action";
				}

				actionException = (ex is TargetInvocationException) ? ex.InnerException : ex;

				RegisterExceptionAndNotifyExtensions(actionException);

				RunAfterActionFilters(action, out cancel);

				if (!ProcessRescue(action, actionException))
				{
					throw;
				}
				return;
			}
			finally
			{
				// AfterAction event: always executed
				if (AfterAction != null)
				{
					AfterAction(action, engineContext, this, context);
				}
			}

			RunAfterActionFilters(action, out cancel);
			if (cancel)
			{
				return;
			}

			if (engineContext.Response.WasRedirected) // No need to process view or rescue in this case
			{
				return;
			}

			if (context.SelectedViewName != null)
			{
				try
				{
					ProcessView();
					RunAfterRenderingFilters(action);
				}
				catch(Exception ex)
				{
					if (Response.StatusCode == 200)
					{
						Response.StatusCode = 500;
						Response.StatusDescription = "Error processing action";
					}

					actionException = (ex is TargetInvocationException) ? ex.InnerException : ex;

					RegisterExceptionAndNotifyExtensions(actionException);

					if (!ProcessRescue(action, actionException))
					{
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Configures the cache policy.
		/// </summary>
		/// <param name="action">The action.</param>
		protected virtual void ConfigureCachePolicy(IExecutableAction action)
		{
			ICachePolicyConfigurer configurer = action.CachePolicyConfigurer ?? MetaDescriptor.CacheConfigurer;

			configurer.Configure(Response.CachePolicy);
		}

		/// <summary>
		/// Selects the appropriate action.
		/// </summary>
		/// <param name="action">The action name.</param>
		/// <param name="actionType">Type of the action.</param>
		/// <returns></returns>
		protected virtual IExecutableAction SelectAction(string action, ActionType actionType)
		{
			// For backward compatibility purposes
			MethodInfo method = SelectMethod(action, MetaDescriptor.Actions, engineContext.Request,
			                                 context.CustomActionParameters, actionType);

			if (method != null)
			{
				ActionMetaDescriptor actionMeta = MetaDescriptor.GetAction(method);

				return new ActionMethodExecutorCompatible(method, actionMeta ?? new ActionMetaDescriptor(), InvokeMethod);
			}

			// New supported way
			return actionSelector.Select(engineContext, this, context, actionType);
		}

		/// <summary>
		/// Invokes the scaffold support if the controller
		/// is associated with a scaffold
		/// </summary>
		protected virtual void ProcessScaffoldIfAvailable()
		{
			if (MetaDescriptor.Scaffoldings.Count != 0)
			{
				if (scaffoldSupport == null)
				{
					String message = "You must enable scaffolding support on the " +
					                 "configuration file, or, to use the standard ActiveRecord support " +
					                 "copy the necessary assemblies to the bin folder.";

					throw new MonoRailException(message);
				}

				scaffoldSupport.Process(engineContext, this, context);
			}
		}

		/// <summary>
		/// Ensures the action is accessible with current HTTP verb.
		/// </summary>
		/// <param name="action">The action.</param>
		protected virtual void EnsureActionIsAccessibleWithCurrentHttpVerb(IExecutableAction action)
		{
			Verb allowedVerbs = action.AccessibleThroughVerb;

			if (allowedVerbs == Verb.Undefined)
			{
				return;
			}

			string method = engineContext.Request.HttpMethod;

			Verb currentVerb = (Verb) Enum.Parse(typeof(Verb), method, true);

			if ((allowedVerbs & currentVerb) != currentVerb)
			{
				throw new MonoRailException(403, "Forbidden",
				                            string.Format("Access to the action [{0}] " +
				                                          "on controller [{1}] is not allowed to the http verb [{2}].",
				                                          Action, Name, method));
			}
		}

		#region Views and Layout

		/// <summary>
		/// Obtains the name of the default layout.
		/// </summary>
		/// <returns></returns>
		protected virtual String[] ObtainDefaultLayoutName()
		{
			if (MetaDescriptor.Layout != null)
			{
				return MetaDescriptor.Layout.LayoutNames;
			}
			else
			{
				String defaultLayout = String.Format("layouts/{0}", Name);

				if (viewEngineManager.HasTemplate(defaultLayout))
				{
					return new String[] {Name};
				}
			}

			return null;
		}

		/// <summary>
		/// Processes the view.
		/// </summary>
		protected virtual void ProcessView()
		{
			if (context.SelectedViewName != null)
			{
				viewEngineManager.Process(context.SelectedViewName, engineContext.Response.Output, engineContext, this, context);
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Creates the and initialize helpers associated with a controller.
		/// </summary>
		public virtual void CreateAndInitializeHelpers()
		{
			IDictionary helpers = context.Helpers;

			// Custom helpers

			foreach(HelperDescriptor helper in MetaDescriptor.Helpers)
			{
				bool initialized;
				object helperInstance = helperFactory.Create(helper.HelperType, engineContext, out initialized);

				if (!initialized)
				{
					serviceInitializer.Initialize(helperInstance, engineContext);
				}

				if (helpers.Contains(helper.Name))
				{
					throw new ControllerException(String.Format("Found a duplicate helper " +
					                                            "attribute named '{0}' on controller '{1}'", helper.Name, Name));
				}

				helpers.Add(helper.Name, helperInstance);
			}

			CreateStandardHelpers();
		}

		/// <summary>
		/// Creates the standard helpers.
		/// </summary>
		public virtual void CreateStandardHelpers()
		{
			AbstractHelper[] builtInHelpers =
				new AbstractHelper[]
					{
						new AjaxHelper(engineContext), new BehaviourHelper(engineContext),
						new UrlHelper(engineContext), new TextHelper(engineContext),
						new EffectsFatHelper(engineContext), new ScriptaculousHelper(engineContext),
						new DateFormatHelper(engineContext), new HtmlHelper(engineContext),
						new ValidationHelper(engineContext), new DictHelper(engineContext),
						new PaginationHelper(engineContext), new FormHelper(engineContext),
						new JSONHelper(engineContext), new ZebdaHelper(engineContext)
					};

			foreach(AbstractHelper helper in builtInHelpers)
			{
				context.Helpers.Add(helper);

				if (helper is IServiceEnabledComponent)
				{
					serviceInitializer.Initialize(helper, engineContext);
				}
			}
		}

		#endregion

		#region Filters

		private void CreateFiltersDescriptors()
		{
			if (MetaDescriptor.Filters.Length != 0)
			{
				filters = CopyFilterDescriptors();
			}
		}

		private void RunBeforeActionFilters(IExecutableAction action, out bool cancel)
		{
			cancel = false;
			if (action.ShouldSkipAllFilters)
			{
				return;
			}

			if (!ProcessFilters(action, ExecuteWhen.BeforeAction))
			{
				cancel = true;
				return; // A filter returned false... so we stop
			}
		}

		private void RunAfterActionFilters(IExecutableAction action, out bool cancel)
		{
			cancel = false;
			if (action == null)
			{
				return;
			}

			if (action.ShouldSkipAllFilters)
			{
				return;
			}

			if (!ProcessFilters(action, ExecuteWhen.AfterAction))
			{
				cancel = true;
				return; // A filter returned false... so we stop
			}
		}

		private void RunAfterRenderingFilters(IExecutableAction action)
		{
			if (action.ShouldSkipAllFilters)
			{
				return;
			}

			ProcessFilters(action, ExecuteWhen.AfterRendering);
		}

		/// <summary>
		/// Identifies if no filter should run for the given action.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		protected virtual bool ShouldSkipFilters(IExecutableAction action)
		{
			if (filters == null)
			{
				// No filters, so skip 
				return true;
			}

			return action.ShouldSkipAllFilters;

			//			ActionMetaDescriptor actionMeta = MetaDescriptor.GetAction(method);
			//
			//			if (actionMeta.SkipFilters.Count == 0)
			//			{
			//				// Nothing against filters declared for this action
			//				return false;
			//			}
			//
			//			foreach(SkipFilterAttribute skipfilter in actionMeta.SkipFilters)
			//			{
			//				// SkipAllFilters handling...
			//				if (skipfilter.BlanketSkip)
			//				{
			//					return true;
			//				}
			//
			//				filtersToSkip[skipfilter.FilterType] = String.Empty;
			//			}
			//
			//			return false;
		}

		/// <summary>
		/// Clones all Filter descriptors, in order to get a writable copy.
		/// </summary>
		protected internal FilterDescriptor[] CopyFilterDescriptors()
		{
			FilterDescriptor[] clone = (FilterDescriptor[]) MetaDescriptor.Filters.Clone();

			for(int i = 0; i < clone.Length; i++)
			{
				clone[i] = (FilterDescriptor) clone[i].Clone();
			}

			return clone;
		}

		private bool ProcessFilters(IExecutableAction action, ExecuteWhen when)
		{
			foreach(FilterDescriptor desc in filters)
			{
				if (action.ShouldSkipFilter(desc.FilterType))
				{
					continue;
				}

				if ((desc.When & when) != 0)
				{
					if (!ProcessFilter(when, desc))
					{
						return false;
					}
				}
			}

			return true;
		}

		private bool ProcessFilter(ExecuteWhen when, FilterDescriptor desc)
		{
			if (desc.FilterInstance == null)
			{
				desc.FilterInstance = filterFactory.Create(desc.FilterType);

				IFilterAttributeAware filterAttAware = desc.FilterInstance as IFilterAttributeAware;

				if (filterAttAware != null)
				{
					filterAttAware.Filter = desc.Attribute;
				}
			}

			try
			{
				if (logger.IsDebugEnabled)
				{
					logger.DebugFormat("Running filter {0}/{1}", when, desc.FilterType.FullName);
				}

				return desc.FilterInstance.Perform(when, engineContext, this, context);
			}
			catch(Exception ex)
			{
				if (logger.IsErrorEnabled)
				{
					logger.ErrorFormat("Error processing filter " + desc.FilterType.FullName, ex);
				}

				throw;
			}
		}

		private void DisposeFilters()
		{
			if (filters == null)
			{
				return;
			}

			foreach(FilterDescriptor desc in filters)
			{
				if (desc.FilterInstance != null)
				{
					filterFactory.Release(desc.FilterInstance);
				}
			}
		}

		#endregion

		#region DynamicActionProviders

		/// <summary>
		/// Creates the dynamic action providers descriptors.
		/// </summary>
		private void CreateDynamicActionProvidersDescriptors()
		{
			if (MetaDescriptor.DynamicActionProviders.Length != 0)
			{
				dynamicActionProviders = CopyDynamicActionProviderDescriptors();
			}
		}

		/// <summary>
		/// Clones all Dynamic Action Provider descriptors, in order to get a writable copy.
		/// </summary>
		protected internal DynamicActionProviderDescriptor[] CopyDynamicActionProviderDescriptors()
		{
			DynamicActionProviderDescriptor[] clone =
				(DynamicActionProviderDescriptor[]) MetaDescriptor.DynamicActionProviders.Clone();

			for(int i = 0; i < clone.Length; i++)
			{
				clone[i] = (DynamicActionProviderDescriptor) clone[i].Clone();
			}

			return clone;
		}

		/// <summary>
		/// Processes the dynamic action providers.
		/// </summary>
		private void ProcessDynamicActionProviders()
		{
			foreach(DynamicActionProviderDescriptor desc in dynamicActionProviders)
			{
				if (desc.DynamicActionProviderInstance == null)
				{
					desc.DynamicActionProviderInstance =
						dynamicActionProviderFactory.Create(desc.DynamicActionProviderType);
				}
			}

			RegisterDynamicActions();
		}

		/// <summary>
		/// Registers the dynamic actions on the controller. Brought inside for meta descriptor reference.
		/// </summary>
		private void RegisterDynamicActions()
		{
			foreach(DynamicActionProviderDescriptor descriptor in dynamicActionProviders)
			{
				IDynamicActionProvider provider = descriptor.DynamicActionProviderInstance;
				provider.IncludeActions(engineContext, this, context);
			}
		}

		/// <summary>
		/// Disposes the dynamic action providers.
		/// </summary>
		private void DisposeDynamicActionProviders()
		{
			if (dynamicActionProviders == null)
			{
				return;
			}

			foreach(DynamicActionProviderDescriptor desc in dynamicActionProviders)
			{
				if (desc.DynamicActionProviderInstance != null)
				{
					dynamicActionProviderFactory.Release(desc.DynamicActionProviderInstance);
				}
			}
		}

		#endregion

		#region Rescue

		/// <summary>
		/// Performs the rescue.
		/// </summary>
		/// <param name="action">The action (can be null in the case of dynamic actions).</param>
		/// <param name="actionException">The exception.</param>
		/// <returns></returns>
		protected virtual bool ProcessRescue(IExecutableAction action, Exception actionException)
		{
			if (action != null && action.ShouldSkipRescues)
			{
				return false;
			}

			Type exceptionType = actionException.GetType();

			RescueDescriptor desc = action != null ? action.GetRescueFor(exceptionType) : null;

			if (desc == null)
			{
				desc = GetControllerRescueFor(exceptionType);
			}

			if (desc != null)
			{
				try
				{
					if (desc.RescueController != null)
					{
						CreateAndProcessRescueController(desc, actionException);
					}
					else
					{
						context.SelectedViewName = Path.Combine("rescues", desc.ViewName);

						ProcessView();
					}

					return true;
				}
				catch(Exception exception)
				{
					// In this situation, the rescue view could not be found
					// So we're back to the default error exibition

					if (logger.IsFatalEnabled)
					{
						logger.FatalFormat("Failed to process rescue view. View name " +
						                   context.SelectedViewName, exception);
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the best rescue that matches the exception type
		/// </summary>
		/// <param name="exceptionType">Type of the exception.</param>
		/// <returns></returns>
		protected virtual RescueDescriptor GetControllerRescueFor(Type exceptionType)
		{
			return RescueUtils.SelectBest(MetaDescriptor.Rescues, exceptionType);
		}

		private void CreateAndProcessRescueController(RescueDescriptor desc, Exception actionException)
		{
			IController rescueController = engineContext.Services.ControllerFactory.CreateController(desc.RescueController);

			ControllerMetaDescriptor rescueControllerMeta =
				engineContext.Services.ControllerDescriptorProvider.BuildDescriptor(rescueController);

			ControllerDescriptor rescueControllerDesc = rescueControllerMeta.ControllerDescriptor;

			IControllerContext rescueControllerContext = engineContext.Services.ControllerContextFactory.Create(
				rescueControllerDesc.Area, rescueControllerDesc.Name, desc.RescueMethod.Name,
				rescueControllerMeta);

			rescueControllerContext.CustomActionParameters["exception"] = actionException;
			rescueControllerContext.CustomActionParameters["controller"] = this;
			rescueControllerContext.CustomActionParameters["controllerContext"] = ControllerContext;

			rescueController.Process(engineContext, rescueControllerContext);
		}

		#endregion

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="actions">The actions.</param>
		/// <param name="request">The request.</param>
		/// <param name="actionArgs">The action args.</param>
		/// <param name="actionType">Type of the action.</param>
		/// <returns></returns>
		protected virtual MethodInfo SelectMethod(string action, IDictionary actions, IRequest request,
		                                          IDictionary<string, object> actionArgs, ActionType actionType)
		{
			return null;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="request">The request.</param>
		/// <param name="methodArgs">The method args.</param>
		protected virtual object InvokeMethod(MethodInfo method, IRequest request,
		                                      IDictionary<string, object> methodArgs)
		{
			return method.Invoke(this, new object[0]);
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
			if (validatorRegistry == null)
			{
				throw new ArgumentNullException("validatorRegistry");
			}

			return new ValidatorRunner(validatorRegistry);
		}

		/// <summary>
		/// Gives a chance to subclasses to format the action name properly
		/// </summary>
		/// <param name="action">Raw action name</param>
		/// <returns>Properly formatted action name</returns>
		protected virtual string TransformActionName(string action)
		{
			return action;
		}

		/// <summary>
		/// Registers the exception and notify extensions.
		/// </summary>
		/// <param name="exception">The exception.</param>
		protected void RegisterExceptionAndNotifyExtensions(Exception exception)
		{
			engineContext.LastException = exception;
			engineContext.Services.ExtensionManager.RaiseActionError(engineContext);
		}
	}
}