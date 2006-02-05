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
	using System.IO;
	using System.Text;
	using System.Web;
	using System.Reflection;
	using System.Threading;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Text.RegularExpressions;

	using Castle.Components.Common.EmailSender;
	
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
		/// The reference to the <see cref="IViewEngine"/> instance
		/// </summary>
		internal IViewEngine _viewEngine;

		/// <summary>
		/// Holds the request/context information
		/// </summary>
		internal IRailsEngineContext _context;

		/// <summary>
		/// Holds information to pass to the view
		/// </summary>
		private IDictionary _bag = new HybridDictionary();

		/// <summary>
		/// Holds the filters associated with the action
		/// </summary>
		private FilterDescriptor[] _filters;

		/// <summary>
		/// Reference to the <see cref="IFilterFactory"/> instance
		/// </summary>
		internal IFilterFactory _filterFactory;

		/// <summary>
		/// Reference to the <see cref="IResourceFactory"/> instance
		/// </summary>
		internal IResourceFactory _resourceFactory;

		/// <summary>
		/// The area name which was used to access this controller
		/// </summary>
		private String _areaName;

		/// <summary>
		/// The controller name which was used to access this controller
		/// </summary>
		private String _controllerName;

		/// <summary>
		/// The view name selected to be rendered after the execution 
		/// of the action
		/// </summary>
		private String _selectedViewName;

		/// <summary>
		/// The layout name that the view engine should use
		/// </summary>
		private String _layoutName;

		/// <summary>
		/// The original action requested
		/// </summary>
		private String _evaluatedAction;

		/// <summary>
		/// The helper instances collected
		/// </summary>
		private IDictionary _helpers = null;

		/// <summary>
		/// The resources associated with this controller
		/// </summary>
		private ResourceDictionary _resources = null;

		internal IDictionary _dynamicActions = new HybridDictionary(true);

		internal IScaffoldingSupport _scaffoldSupport;

		internal bool _directRenderInvoked;

		private ControllerMetaDescriptor metaDescriptor;

		private IServiceProvider serviceProvider;

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

		public ICollection Actions
		{
			get { return metaDescriptor.Actions.Values; }
		}

		public ResourceDictionary Resources
		{
			get { return _resources; }
		}

		public IDictionary Helpers
		{
			get
			{
				if (_helpers == null) CreateAndInitializeHelpers();

				return _helpers;
			}
		}

		/// <summary>
		/// Gets the controller's name.
		/// </summary>
		public String Name
		{
			get { return _controllerName; }
		}

		/// <summary>
		/// Gets the controller's area name.
		/// </summary>
		public String AreaName
		{
			get { return _areaName; }
		}

		/// <summary>
		/// Gets or set the layout being used.
		/// </summary>
		public String LayoutName
		{
			get { return _layoutName; }
			set { _layoutName = value; }
		}

		/// <summary>
		/// Gets the name of the action being processed.
		/// </summary>
		public String Action
		{
			get { return _evaluatedAction; }
		}

		/// <summary>
		/// Gets the property bag, which is used
		/// to pass variables to the view.
		/// </summary>
		public IDictionary PropertyBag
		{
			get { return _bag; }
			set { _bag = value; }
		}

		/// <summary>
		/// Gets the context of this web execution.
		/// </summary>
		public IRailsEngineContext Context
		{
			get { return _context; }
		}

		/// <summary>
		/// Gets the Session dictionary.
		/// </summary>
		protected IDictionary Session
		{
			get { return _context.Session; }
		}

		/// <summary>
		/// Gets a dictionary of volative items.
		/// Ideal for showing success and failures messages.
		/// </summary>
		protected Flash Flash
		{
			get { return _context.Flash; }
		}

		/// <summary>
		/// Gets the web context of ASP.NET API.
		/// </summary>
		protected HttpContext HttpContext
		{
			get { return _context.UnderlyingContext; }
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

		public IDictionary DynamicActions
		{
			get { return _dynamicActions; }
		}

		[Obsolete("Use the DynamicActions property instead")]
		public IDictionary CustomActions
		{
			get { return _dynamicActions; }
		}

		#endregion

		#region Useful operations

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		public void RenderView(String name)
		{
			String basePath = _controllerName;

			if (_areaName != null)
			{
				basePath = Path.Combine(_areaName, _controllerName);
			}

			_selectedViewName = Path.Combine(basePath, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		public void RenderView(String name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();

			RenderView(name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		public void RenderView(String controller, String name)
		{
			_selectedViewName = Path.Combine(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		public void RenderView(String controller, String name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();

			RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed and results are written to System.IO.TextWriter. 
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name">The name of the view to process.</param>
		public void InPlaceRenderView(TextWriter output, String name)
		{
			String basePath = _controllerName;

			if (_areaName != null)
			{
				basePath = Path.Combine(_areaName, _controllerName);
			}

			_viewEngine.Process(output, Context, this, Path.Combine(basePath, name));			
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public void RenderSharedView(String name)
		{
			_selectedViewName = name;
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public void RenderSharedView(String name, bool skipLayout)
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
		public void InPlaceRenderSharedView(TextWriter output, String name)
		{
			_viewEngine.Process(output, Context, this, name);
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
		/// <param name="contents"></param>
		public void RenderText(String contents)
		{
			CancelView();

			Response.Write(contents);
		}

		/// <summary>
		/// Sends raw contents to be rendered directly by the view engine.
		/// It's up to the view engine just to apply the layout and nothing else.
		/// </summary>
		/// <param name="contents">Contents to be rendered.</param>
		public void DirectRender(String contents)
		{
			CancelView();

			if (_directRenderInvoked)
			{
				throw new ControllerException("DirectRender should be called only once.");
			}

			_directRenderInvoked = true;

			_viewEngine.ProcessContents(_context, this, contents);
		}

		/// <summary>
		/// Returns true if the specified template exists.
		/// </summary>
		/// <param name="templateName"></param>
		public bool HasTemplate(String templateName)
		{
			return _viewEngine.HasTemplate(templateName);
		}

		#region RedirectToAction
		
		/// <summary> 
		/// Redirects to another action in the same controller.
		/// </summary>
		/// <param name="action">The action name</param>
		protected void RedirectToAction(String action)
		{
			RedirectToAction(action, (NameValueCollection) null);
		}

		/// <summary> 
		/// Redirects to another action in the same controller.
		/// </summary>
		protected void RedirectToAction(String action, params String[] queryStringParameters)
		{
			RedirectToAction(action, new DictHelper().CreateDict(queryStringParameters));
		}

		/// <summary> 
		/// Redirects to another action in the same controller.
		/// </summary>
		protected void RedirectToAction(String action, IDictionary queryStringParameters)
		{
			NameValueCollection copy = null;
			
			if (queryStringParameters != null && queryStringParameters.Count > 0) 
			{
				copy = new NameValueCollection(queryStringParameters.Count);
				
				foreach (Object key in queryStringParameters.Keys)
				{
					copy.Add(Convert.ToString(key), Convert.ToString(queryStringParameters[key]));
				}
			}

			RedirectToAction(action, copy);
		}

		/// <summary> 
		/// Redirects to another action in the same controller.
		/// </summary>
		protected void RedirectToAction(String action, NameValueCollection queryStringParameters)
		{
			if (queryStringParameters != null)
			{
				Redirect(AreaName, Name, action, queryStringParameters);
			}
			else
			{
				Redirect(AreaName, Name, action);
			}
		}

		#endregion

		protected String CreateAbsoluteRailsUrl(String area, String controller, String action)
		{
			return UrlInfo.CreateAbsoluteRailsUrl(Context.ApplicationPath, area, controller, action, Context.UrlInfo.Extension);
		}
		
		protected String CreateAbsoluteRailsUrl(String controller, String action)
		{
			return UrlInfo.CreateAbsoluteRailsUrl(Context.ApplicationPath, controller, action, Context.UrlInfo.Extension);
		}
		
		protected String CreateAbsoluteRailsUrlForAction(String action)
		{
			return UrlInfo.CreateAbsoluteRailsUrl(Context.ApplicationPath, this.AreaName, this.Name, action, Context.UrlInfo.Extension);
		}
		
		/// <summary>
		/// Redirects to the specified URL. All other Redirects call this one.
		/// </summary>
		/// <param name="url">Target URL</param>
		public virtual void Redirect(String url)
		{
			CancelView();

			_context.Response.Redirect(url);
		}

		/// <summary>
		/// Redirects to another controller and action.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		public void Redirect(String controller, String action)
		{
			Redirect( CreateAbsoluteRailsUrl(controller, action) );
		}

		/// <summary>
		/// Redirects to another controller and action.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		public void Redirect(String area, String controller, String action)
		{
			Redirect( CreateAbsoluteRailsUrl(area, controller, action) );
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(String controller, String action, NameValueCollection parameters)
		{
			String querystring = ToQueryString(parameters);
			String url = CreateAbsoluteRailsUrl(controller, action);

			Redirect(String.Format("{0}?{1}", url, querystring));
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
			String querystring = ToQueryString(parameters);
			String url = CreateAbsoluteRailsUrl(area, controller, action);

			Redirect(String.Format("{0}?{1}", url, querystring));
		}
		
		protected String ToQueryString(NameValueCollection parameters)
		{
			StringBuilder buffer = new StringBuilder();
			HttpServerUtility serverUtility = HttpContext.Server;
	
			foreach (String key in parameters.Keys)
			{
				buffer.Append( serverUtility.HtmlEncode(key) )
					  .Append('=')
					  .Append( serverUtility.HtmlEncode(parameters[key]) )
					  .Append('&');
			}

			return buffer.Length == 0 ? 
						String.Empty : 
						buffer.Remove(buffer.Length -1, 1).ToString(); // removing extra &
		}

		#endregion

		#region Core members

		protected internal IServiceProvider ServiceProvider
		{
			get { return serviceProvider; }
		}

		internal void InitializeFieldsFromServiceProvider(IRailsEngineContext context)
		{
			serviceProvider = context;
			
			_viewEngine = (IViewEngine) serviceProvider.GetService( typeof(IViewEngine) );
			_filterFactory = (IFilterFactory) serviceProvider.GetService( typeof(IFilterFactory) );
			_resourceFactory = (IResourceFactory) serviceProvider.GetService( typeof(IResourceFactory) );
			_scaffoldSupport = (IScaffoldingSupport) serviceProvider.GetService( typeof(IScaffoldingSupport) );

			ControllerDescriptorBuilder controllerDescriptorBuilder = (ControllerDescriptorBuilder)
				serviceProvider.GetService( typeof(ControllerDescriptorBuilder) );

			metaDescriptor = controllerDescriptorBuilder.BuildDescriptor(this);

			_context = context;
		}

		internal void InitializeControllerState(String areaName, String controllerName, String actionName)
		{
			SetEvaluatedAction(actionName);
			_areaName = areaName;
			_controllerName = controllerName;
		}

		internal void SetEvaluatedAction(String actionName)
		{
			_evaluatedAction = actionName;
		}

		/// <summary>
		/// Method invoked by the engine to start 
		/// the controller process. 
		/// </summary>
		internal void Process(IRailsEngineContext context, String areaName, 
			String controllerName, String actionName)
		{
			InitializeFieldsFromServiceProvider(context);

			InitializeControllerState(areaName, controllerName, actionName);
			
#if ALLOWTEST
			HttpContext.Items["mr.flash"] = Flash;
			HttpContext.Items["mr.session"] = Session;
			HttpContext.Items["mr.propertybag"] = PropertyBag;
#endif

			if (metaDescriptor.Filters.Count != 0)
			{
				_filters = CollectFilterDescriptors();
			}

			LayoutName = ObtainDefaultLayoutName();

			if (metaDescriptor.Scaffoldings.Count != 0)
			{
				if (_scaffoldSupport == null)
				{
					String message = "You must enable scaffolding support on the " +
						"configuration file, or, to use the standard ActiveRecord support " +
						"copy the necessary assemblies to the bin folder.";

					throw new RailsException(message);
				}

				_scaffoldSupport.Process(this);
			}

			ActionProviderUtil.RegisterActions(this);

			Initialize();

			InternalSend(actionName);
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
		public void Send(String action)
		{
			InternalSend(action);
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
		protected virtual void InternalSend(String action)
		{
			// If a redirect was sent there's no point in
			// wasting processor cycles
			if (Response.WasRedirected) return;

			// Record the action
			SetEvaluatedAction(action);

			// Record the default view for this area/controller/action
			RenderView(action);

			// If we have an HttpContext available, store the original view name
			if (HttpContext != null)
			{
				if (!HttpContext.Items.Contains(Constants.OriginalViewKey))
				{
					HttpContext.Items[Constants.OriginalViewKey] = _selectedViewName;
				}
			}

			// Look for the target method
			MethodInfo method = SelectMethod(action, MetaDescriptor.Actions, _context.Request);

			// If we couldn't find a method for this action, look for a dynamic action
			IDynamicAction dynAction = null;
			if (method == null)
			{
				dynAction = DynamicActions[action] as IDynamicAction;

				if (dynAction == null)
				{
					method = FindOutDefaultMethod();

					if (method == null)
					{
						throw new ControllerException(String.Format("No action for '{0}' found", action));
					}
				}
			}

			// Overrides the current layout, if the action specifies one
			if (method != null)
			{
				ActionMetaDescriptor ac = MetaDescriptor.GetAction(method);
				
				if (ac.Layout != null)
				{
					this.LayoutName = ac.Layout.LayoutName;
				}
			}

			HybridDictionary filtersToSkip = new HybridDictionary();

			bool skipFilters = ShouldSkip(method, filtersToSkip);
			bool hasError = false;
			Exception exceptionToThrow = null;

			try
			{
				// If we are supposed to run the filters...
				if (!skipFilters)
				{
					// ...run them. If they fail...
					if (!ProcessFilters(ExecuteEnum.BeforeAction, filtersToSkip))
					{
						// Record that they failed.
						hasError = true;
					}
				}

				// If we haven't failed anywhere yet...
				if (!hasError)
				{
					CreateResources(method);

					// Execute the method / dynamic action
					if (method != null)
					{
						InvokeMethod(method);
					}
					else
					{
						dynAction.Execute(this);
					}

					if (!skipFilters)
					{
						// ...run the AfterAction filters. If they fail...
						if (!ProcessFilters(ExecuteEnum.AfterAction, filtersToSkip))
						{
							// Record that they failed.
							hasError = true;
						}
					}
				}
			}
			catch (ThreadAbortException)
			{
				hasError = true;
			}
			catch (Exception ex)
			{
				hasError = true;

				// Try and perform the rescue
				if (!PerformRescue(method, ex))
				{
					exceptionToThrow = ex;
				}

				RaiseOnActionExceptionOnExtension();
			}
			
			try
			{
				// If we haven't failed anywhere and no redirect was issued
				if (!hasError && !Response.WasRedirected)
				{
					// Render the actual view then cleanup
					ProcessView();
				}

				if (exceptionToThrow != null)
				{
					throw new RailsException("Unhandled Exception while rendering view", exceptionToThrow);
				}
			}
			finally
			{			
				// Run the filters if required
				if (!skipFilters)
				{
					ProcessFilters(ExecuteEnum.AfterRendering, filtersToSkip);
				}

				DisposeFilter();

				ReleaseResources();

				AddFlashToSessionIfUsed();
			}
		}

		/// <summary>
		/// The following lines were added to handle _default processing
		/// if present look for and load _default action method
		/// <seealso cref="DefaultActionAttribute"/>
		/// </summary>
		private MethodInfo FindOutDefaultMethod()
		{
			if (metaDescriptor.DefaultAction != null)
			{
				return SelectMethod(metaDescriptor.DefaultAction.DefaultAction, MetaDescriptor.Actions, _context.Request);
			}

			return null;
		}

		protected virtual void CreateAndInitializeHelpers()
		{
			_helpers = new HybridDictionary();

			foreach (HelperAttribute helper in metaDescriptor.Helpers)
			{
				object helperInstance = Activator.CreateInstance(helper.HelperType);

				IControllerAware aware = helperInstance as IControllerAware;

				if (aware != null)
				{
					aware.SetController(this);
				}

				_helpers.Add(helper.Name, helperInstance);
			}

			AbstractHelper[] builtInHelpers =
				new AbstractHelper[]
					{
						new AjaxHelper(), new AjaxHelperOld(),
						new EffectsFatHelper(), new Effects2Helper(),
						new DateFormatHelper(), new HtmlHelper(),
						new ValidationHelper(), new DictHelper(),
						new PaginationHelper(), new FormHelper()
					};

			foreach (AbstractHelper helper in builtInHelpers)
			{
				helper.SetController(this);

				String helperName = helper.GetType().Name;

				if (!_helpers.Contains(helperName))
				{
					_helpers[helperName] = helper;
				}
			}
		}

		private void AddFlashToSessionIfUsed()
		{
			
		}

		#endregion

		#region Action Invocation

		protected virtual MethodInfo SelectMethod(String action, IDictionary actions, IRequest request)
		{
			return actions[action] as MethodInfo;
		}

		private void InvokeMethod(MethodInfo method)
		{
			InvokeMethod(method, _context.Request);
		}

		protected virtual void InvokeMethod(MethodInfo method, IRequest request)
		{
			method.Invoke(this, new object[0]);
		}

		#endregion

		#region Resources

		protected virtual void CreateResources(MethodInfo method)
		{
			_resources = new ResourceDictionary();

			Assembly typeAssembly = this.GetType().Assembly;

			foreach (ResourceAttribute resource in metaDescriptor.Resources)
			{
				_resources.Add(resource.Name, _resourceFactory.Create(resource, typeAssembly));
			}

			if (method == null) return;

			ActionMetaDescriptor actionMeta = metaDescriptor.GetAction(method);

			foreach (ResourceAttribute resource in actionMeta.Resources)
			{
				_resources[resource.Name] = _resourceFactory.Create(resource, typeAssembly);
			}
		}

		protected virtual void ReleaseResources()
		{
			if (_resources == null) return;

			foreach (IResource resource in _resources.Values)
			{
				_resourceFactory.Release(resource);
			}
		}

		#endregion

		#region Filters

		protected internal bool ShouldSkip(MethodInfo method, IDictionary filtersToSkip)
		{
			if (method == null)
			{
				// Dynamic Action, run the filters if we have any
				return (_filters == null);
			}

			if (_filters == null)
			{
				// No filters, so skip 
				return true;
			}

			ActionMetaDescriptor actionMeta = metaDescriptor.GetAction(method);

			if (actionMeta.SkipFilters.Count == 0)
			{
				// Nothing against filters declared for this action
				return false;
			}

			foreach (SkipFilterAttribute skipfilter in actionMeta.SkipFilters)
			{
				// If the user declared a [SkipFilterAttribute] then skip all filters
				if (skipfilter.BlanketSkip) return true;

				filtersToSkip[skipfilter.FilterType] = String.Empty;
			}

			return false;
		}

		protected internal FilterDescriptor[] CollectFilterDescriptors()
		{
			IList attrs = metaDescriptor.Filters;

			FilterDescriptor[] descriptors = new FilterDescriptor[attrs.Count];

			for (int i = 0; i < attrs.Count; i++)
			{
				descriptors[i] = new FilterDescriptor(attrs[i] as FilterAttribute);
			}

			if (attrs.Count > 1)
			{
				SortFilterDescriptors(descriptors);
			}

			return descriptors;
		}

		private void SortFilterDescriptors(FilterDescriptor[] descriptors)
		{
			Array.Sort(descriptors, FilterDescriptorComparer.Instance);
		}

		private bool ProcessFilters(ExecuteEnum when, IDictionary filtersToSkip)
		{
			foreach (FilterDescriptor desc in _filters)
			{
				if (filtersToSkip.Contains(desc.FilterType)) continue;

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

		private bool ProcessFilter(ExecuteEnum when, FilterDescriptor desc)
		{
			if (desc.FilterInstance == null)
			{
				desc.FilterInstance = _filterFactory.Create(desc.FilterType);

				IFilterAttributeAware filterAttAware = desc.FilterInstance as IFilterAttributeAware;

				if (filterAttAware != null)
				{
					filterAttAware.FilterAttribute = desc.Attribute;
				}
			}

			return desc.FilterInstance.Perform(when, _context, this);
		}

		private void DisposeFilter()
		{
			if (_filters == null) return;

			foreach (FilterDescriptor desc in _filters)
			{
				if (desc.FilterInstance != null)
				{
					_filterFactory.Release(desc.FilterInstance);
				}
			}
		}

		class FilterDescriptorComparer : IComparer
		{
			private static readonly FilterDescriptorComparer instance = new FilterDescriptorComparer();
			
			private FilterDescriptorComparer()
			{
			}

			public static FilterDescriptorComparer Instance
			{
				get { return instance; }
			}

			public int Compare(object x, object y)
			{
				return ((FilterDescriptor)x).ExecutionOrder - ((FilterDescriptor)y).ExecutionOrder;
			}
		}

		#endregion

		#region Views and Layout

		protected virtual String ObtainDefaultLayoutName()
		{
			if (metaDescriptor.Layout != null)
			{
				return metaDescriptor.Layout.LayoutName;
			}
			else
			{
				String defaultLayout = String.Format("layouts/{0}", Name);
				
				if ( HasTemplate(defaultLayout) )
				{
					return Name;
				}
			}
			return null;
		}

		private void ProcessView()
		{
			if (_selectedViewName != null)
			{
				_viewEngine.Process(_context, this, _selectedViewName);
			}
		}

		#endregion

		#region Rescue

		protected virtual bool PerformRescue(MethodInfo method, Exception ex)
		{		
			_context.LastException = (ex is TargetInvocationException) ? ex.InnerException : ex;
			
			// Dynamic action 
			if (method == null) return false;

			Type exceptionType = _context.LastException.GetType();

			ActionMetaDescriptor actionMeta = metaDescriptor.GetAction(method);
			
			if (actionMeta.SkipRescue != null) return false;
			
			RescueAttribute att = GetRescueAttributeFor(actionMeta.Rescues, exceptionType);
			
			if (att == null)
			{
				att = GetRescueAttributeFor(metaDescriptor.Rescues, exceptionType);

				if (att == null) return false;
			}
				
			try
			{
				_selectedViewName = String.Format("rescues\\{0}", att.ViewName);
				ProcessView();
				return true;
			}
			catch (Exception e)
			{
				// In this situation, the rescue view could not be found
				// So we're back to the default error exibition
				Console.WriteLine(e);
			}

			return false;
		}

		protected virtual RescueAttribute GetRescueAttributeFor(IList rescues, Type exceptionType)
		{
			if (rescues == null || rescues.Count == 0) return null;
			
			RescueAttribute bestCandidate = null;
			
			foreach(RescueAttribute rescue in rescues)
			{
				if (rescue.ExceptionType == exceptionType)
				{
					return rescue;
				}
				else if (rescue.ExceptionType != null && 
					rescue.ExceptionType.IsAssignableFrom(exceptionType))
				{
					bestCandidate = rescue;
				}
			}
			
			return bestCandidate;
		}

		#endregion

		#region Lifecycle (overridables)

		protected virtual void Initialize()
		{
			
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

			if (_context.UnderlyingContext != null)
			{
				_context.UnderlyingContext.Items[Constants.ControllerContextKey] = this;
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
		public Message RenderMailMessage(String templateName)
		{
			// create a message object
			Message message = new Message();

			// use the template engine to generate the body of the message
			StringWriter writer = new StringWriter();
			InPlaceRenderSharedView(writer, Path.Combine(Constants.TemplatePath, templateName));
			String body = writer.ToString();
			
			// process delivery addresses from template.
			MatchCollection matches1 = Constants.readdress.Matches(body);
			for(int i=0; i< matches1.Count; i++)
			{
				String header	 = matches1[i].Groups[Constants.HeaderKey].ToString().ToLower();
				String address = matches1[i].Groups[Constants.ValueKey].ToString();

				switch(header)
				{
					case Constants.To :
						message.To = address;
						break;
					case Constants.Cc :
						message.Cc = address;
						break;
					case Constants.Bcc :
						message.Bcc = address;
						break;
				}
			}
			body = Constants.readdress.Replace(body, String.Empty);

			// process from address from template
			Match match = Constants.refrom.Match(body);
			if(match.Success)
			{
				message.From = match.Groups[Constants.ValueKey].ToString();
				body = Constants.refrom.Replace(body, String.Empty);
			}

			// process subject and X headers from template
			MatchCollection matches2 = Constants.reheader.Matches(body);
			for(int i=0; i< matches2.Count; i++)
			{
				String header	= matches2[i].Groups[Constants.HeaderKey].ToString();
				String strval	= matches2[i].Groups[Constants.ValueKey].ToString();

				if(header.ToLower() == Constants.Subject)
				{
					message.Subject = strval;
				}
				else
				{
					message.Headers.Add(header, strval);
				}
			}
			body = Constants.reheader.Replace(body, String.Empty);

			message.Body = body;

			// a little magic to see if the body is html
			if(message.Body.ToLower().IndexOf(Constants.HtmlTag) > -1)
			{
				message.Format = Format.Html;
			}
			
			return message;
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
				throw new RailsException("Error sending e-mail", ex);
			}
		}

		/// <summary>
		/// Renders and delivers the e-mail message.
		/// <seealso cref="RenderMailMessage"/>
		/// <seealso cref="DeliverEmail"/>
		/// </summary>
		/// <param name="templateName"></param>
		public void RenderEmailAndSend(String templateName)
		{
			Message message = RenderMailMessage(templateName);
			DeliverEmail(message);
		}

		#endregion

		#region Extension 

		protected void RaiseOnActionExceptionOnExtension()
		{
			ExtensionManager manager = (ExtensionManager) 
				ServiceProvider.GetService( typeof(ExtensionManager) );

			manager.RaiseActionError(Context);
		}

		#endregion
	}
}
