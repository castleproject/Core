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
	using System.Threading;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Helpers;

	/// <summary>
	/// Implements the core functionality and expose the
	/// common methods for concrete controllers.
	/// </summary>
	public abstract class Controller
	{
		#region Static and Instance Fields

		/// <summary>
		/// TODO: Document why this is necessary
		/// </summary>
		public static readonly String ControllerContextKey = "rails.controller";

		/// <summary>
		/// TODO: Document why this is necessary
		/// </summary>
		internal static readonly String OriginalViewKey = "rails.original_view";

		/// <summary>
		/// The reference to the <see cref="IViewEngine"/> instance
		/// </summary>
		private IViewEngine _viewEngine;

		/// <summary>
		/// Holds the request/context information
		/// </summary>
		private IRailsEngineContext _context;

		/// <summary>
		/// Holds information to pass to the view
		/// </summary>
		private IDictionary _bag;
		
		/// <summary>
		/// Holds the filters associated with the action
		/// </summary>
		private FilterDescriptor[] _filters;
		
		/// <summary>
		/// Reference to the <see cref="IFilterFactory"/> instance
		/// </summary>
		private IFilterFactory _filterFactory;

		/// <summary>
		/// Reference to the <see cref="IResourceFactory"/> instance
		/// </summary>
		private IResourceFactory _resourceFactory;

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
		private IDictionary _resources = null;

		internal IDictionary _actions = new HybridDictionary(true);

		internal IDictionary _customActions = new HybridDictionary(true);

		private IScaffoldingSupport _scaffoldSupport;

		internal bool _directRenderInvoked;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a Controller
		/// </summary>
		public Controller()
		{
			_bag = new HybridDictionary();

			CollectActions();
		}

		#endregion

		#region Usefull Properties

		public IDictionary Resources
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
		/// Gets or set the controller's layout.
		/// </summary>
		public String LayoutName
		{
			get { return _layoutName; }
			set { _layoutName = value; }
		}

		/// <summary>
		/// Returns the name of the action being processed
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
		}

		/// <summary>
		/// Gets the context of this web execution.
		/// </summary>
		public IRailsEngineContext Context
		{
			get { return _context; }
		}

		/// <summary>
		/// Gets the Session dictionary
		/// </summary>
		protected IDictionary Session
		{
			get { return _context.Session; }
		}

		/// <summary>
		/// Access a dictionary of volative items.
		/// Ideal for showing success and failures messages
		/// </summary>
		protected IDictionary Flash
		{
			get { return _context.Flash; }
		}

		/// <summary>
		/// Gets the web context of ASP.Net API.
		/// </summary>
		protected HttpContext HttpContext
		{
			get { return _context.UnderlyingContext as HttpContext; }
		}

		/// <summary>
		/// Gets the request
		/// </summary>
		public IRequest Request
		{
			get { return Context.Request; }
		}

		/// <summary>
		/// Gets the response
		/// </summary>
		public IResponse Response
		{
			get { return Context.Response; }
		}

		#endregion

		#region Usefull operations

		/// <summary>
		/// Defines the View to be presented
		/// after the action has finished its processing. 
		/// </summary>
		/// <param name="name"></param>
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
		/// Renders a shared (a partial view shared 
		/// by others views and usually on the root folder
		/// of the view root directory)
		/// </summary>
		/// <param name="name"></param>
		public void RenderSharedView(String name)
		{
			_selectedViewName = name;
		}

		/// <summary>
		/// Defines the View to be presented
		/// after the action has finished its processing. 
		/// </summary>
		public void RenderView(String controller, String name)
		{
			_selectedViewName = Path.Combine(controller, name);
		}

		public void DirectRender(String contents)
		{
			CancelView();

			if (_directRenderInvoked)
			{
				throw new ControllerException("DirectRender should be called only once.");
			}

			_directRenderInvoked = true;

			_viewEngine.ProcessContents( _context, this, contents );
		}

		/// <summary>
		/// Redirects to the specified Url
		/// </summary>
		/// <param name="url"></param>
		public void Redirect(String url)
		{
			CancelView();

			_context.Response.Redirect(url);
		}

		/// <summary>
		/// Redirects to another controller and action.
		/// </summary>
		/// <param name="controller"></param>
		/// <param name="action"></param>
		public void Redirect(String controller, String action)
		{
			CancelView();

			_context.Response.Redirect(controller, action);
		}

		/// <summary>
		/// Redirects to another controller and action.
		/// </summary>
		/// <param name="area"></param>
		/// <param name="controller"></param>
		/// <param name="action"></param>
		public void Redirect(String area, String controller, String action)
		{
			CancelView();

			_context.Response.Redirect(area, controller, action);
		}

		/// <summary>
		/// Cancel the view presentation.
		/// </summary>
		public void CancelView()
		{
			_selectedViewName = null;
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

		#endregion

		#region Core members

		public IDictionary CustomActions
		{
			get { return _customActions; }
		}

		protected internal virtual void CollectActions()
		{
			MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);

			foreach (MethodInfo m in methods)
			{
				_actions[m.Name] = m;
			}

			ScreenCommonPublicMethods(_actions);
		}

		protected void ScreenCommonPublicMethods(IDictionary actions)
		{
			actions.Remove("ToString");
			actions.Remove("GetHashCode");
			actions.Remove("RenderView");
			actions.Remove("RenderText");
			actions.Remove("RenderSharedView");
			actions.Remove("Redirect");
			actions.Remove("Process");
			actions.Remove("Send");
			actions.Remove("PreSendView");
			actions.Remove("PostSendView");
		}

		/// <summary>
		/// Method invoked by the engine to start 
		/// the controller process. 
		/// </summary>
		public void Process(IRailsEngineContext context, IFilterFactory filterFactory, IResourceFactory resourceFactory,
		                    String areaName, String controllerName, String actionName, IViewEngine viewEngine, 
							IScaffoldingSupport scaffoldSupport)
		{
			_areaName = areaName;
			_controllerName = controllerName;
			_viewEngine = viewEngine;
			_context = context;
			_filterFactory = filterFactory;
			_resourceFactory = resourceFactory;
			_scaffoldSupport = scaffoldSupport;

			if (GetType().IsDefined(typeof (FilterAttribute), true))
			{
				_filters = CollectFilterDescriptor();
			}

			if (GetType().IsDefined(typeof (LayoutAttribute), true))
			{
				LayoutName = ObtainDefaultLayoutName();
			}

			if (GetType().IsDefined(typeof (ScaffoldingAttribute), false))
			{
				if (_scaffoldSupport == null)
				{
					String message = "You must enable scaffolding support on the " + 
						"configuration file, or, to use the standard ActiveRecord support, " + 
						"copy the necessary assemblies to the bin folder.";
					throw new Exception(message);
				}

				_scaffoldSupport.Process( this );
			}

			InternalSend(actionName);
		}

		/// <summary>
		/// Used to process the method associated
		/// with the action name specified.
		/// </summary>
		/// <param name="action"></param>
		public void Send(String action)
		{
			InternalSend(action);
		}

		/// <summary>
		/// Performs the Action, which means:
		/// <br/>
		/// 1. Define the default view name<br/>
		/// 2. Runs the Before Filters<br/>
		/// 3. Select the method related to the action name and invoke it<br/>
		/// 4. On error, executes the rescues if available<br/>
		/// 5. Runs the After Filters<br/>
		/// 6. Invoke the view engine<br/>
		/// </summary>
		/// <param name="action">Action name</param>
		protected virtual void InternalSend(String action)
		{
			_evaluatedAction = action;

			// Specifies the default view for this area/controller/action
			RenderView(action);

			if (HttpContext.Current != null)
			{
				if (!HttpContext.Current.Items.Contains(OriginalViewKey))
				{
					HttpContext.Current.Items[OriginalViewKey] = _selectedViewName;
				}
			}

			MethodInfo method = SelectMethod(action, _actions, _context.Request);

			CreateResources(method);

			IDynamicAction dynAction = null;

			if (method == null)
			{
				dynAction = CustomActions[ action ] as IDynamicAction;

				if (dynAction == null)
				{
					if (method == null)
					{
						throw new ControllerException(String.Format("No action for '{0}' found", action));
					}
				}
			}

			HybridDictionary filtersToSkip = new HybridDictionary();

			bool skipFilters = ShouldSkip(method, filtersToSkip);
			bool hasError = false;

			try
			{
				if (!skipFilters)
				{
					if (!ProcessFilters(ExecuteEnum.Before, filtersToSkip))
					{
						hasError = true;
					}
				}

				if (!hasError)
				{
					if (method != null)
					{
						InvokeMethod(method);
					}
					else
					{
						dynAction.Execute(this);
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

				if (!PerformRescue(method, GetType(), ex))
				{
					throw ex;
				}
			}
			finally
			{
				if (!skipFilters)
				{
					ProcessFilters(ExecuteEnum.After, filtersToSkip);
				}
				DisposeFilter();
			}

			if (!hasError)
			{
				ProcessView();
				ReleaseResources();
			}
		}

		protected virtual void CreateAndInitializeHelpers()
		{
			_helpers = new HybridDictionary();

			Attribute[] helpers = Attribute.GetCustomAttributes(this.GetType(), typeof (HelperAttribute));

			foreach (HelperAttribute helper in helpers)
			{
				object helperInstance = Activator.CreateInstance(helper.HelperType);

				IControllerAware aware = helperInstance as IControllerAware;

				if (aware != null)
				{
					aware.SetController(this);
				}

				_helpers.Add(helper.HelperType.Name, helperInstance);
			}

			// TODO: Unify Helper Instation
			// Default helpers 
			AjaxHelper ajaxHelper = new AjaxHelper();
			ajaxHelper.SetController(this);
			_helpers[typeof (AjaxHelper).Name] = ajaxHelper;

			EffectsFatHelper effectsFatHelper = new EffectsFatHelper();
			effectsFatHelper.SetController(this);
			_helpers[typeof (EffectsFatHelper).Name] = effectsFatHelper;

			_helpers[typeof (DateFormatHelper).Name] = new DateFormatHelper();

			HtmlHelper htmlHelper = new HtmlHelper();
			htmlHelper.SetController(this);
			_helpers[typeof (HtmlHelper).Name] = htmlHelper;
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
			_resources = new HybridDictionary(true);

			if (method == null) return;

			Assembly typeAssembly = this.GetType().Assembly;

			Attribute[] resources = Attribute.GetCustomAttributes(this.GetType(), typeof (AbstractResourceAttribute));

			foreach (AbstractResourceAttribute resource in resources)
				_resources.Add(resource.Name, _resourceFactory.Create(resource, typeAssembly));

			resources = Attribute.GetCustomAttributes(method, typeof (AbstractResourceAttribute));

			foreach (AbstractResourceAttribute resource in resources)
				_resources[resource.Name] = _resourceFactory.Create(resource, typeAssembly);
		}

		protected virtual void ReleaseResources()
		{
			foreach (IResource resource in _resources.Values)
				_resourceFactory.Release(resource);
		}

		#endregion

		#region Filters

		protected internal bool ShouldSkip(MethodInfo method, IDictionary filtersToSkip)
		{
			if (_filters == null)
			{
				// No filters, so skip 
				return true;
			}

			if (!method.IsDefined(typeof (SkipFilterAttribute), true))
			{
				// Nothing against filters declared for this action
				return false;
			}

			object[] skipInfo = method.GetCustomAttributes(typeof (SkipFilterAttribute), true);

			foreach (SkipFilterAttribute skipfilter in skipInfo)
			{
				// If the user declared a [SkipFilterAttribute] then skip all filters
				if (skipfilter.BlanketSkip) return true;

				filtersToSkip[skipfilter.FilterType] = String.Empty;
			}

			return false;
		}

		protected internal FilterDescriptor[] CollectFilterDescriptor()
		{
			object[] attrs = GetType().GetCustomAttributes(typeof (FilterAttribute), true);
			FilterDescriptor[] desc = new FilterDescriptor[attrs.Length];

			for (int i = 0; i < attrs.Length; i++)
			{
				desc[i] = new FilterDescriptor(attrs[i] as FilterAttribute);
			}

			return desc;
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

				if (typeof (IFilterAttributeAware).IsInstanceOfType(desc.FilterInstance))
					(desc.FilterInstance as IFilterAttributeAware).FilterAttribute = desc.Attribute;
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

		#endregion

		#region Views and Layout

		protected virtual String ObtainDefaultLayoutName()
		{
			object[] attrs = GetType().GetCustomAttributes(typeof (LayoutAttribute), true);

			if (attrs.Length == 1)
			{
				LayoutAttribute layoutDef = (LayoutAttribute) attrs[0];
				return layoutDef.LayoutName;
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

		protected virtual bool PerformRescue(MethodInfo method, Type controllerType, Exception ex)
		{
			if (ex is TargetInvocationException)
			{
				_context.LastException = ex.InnerException;
			}
			else
			{
				_context.LastException = ex;
			}

			RescueAttribute att = null;

			if (method != null && method.IsDefined(typeof (RescueAttribute), true))
			{
				att = method.GetCustomAttributes(
					typeof (RescueAttribute), true)[0] as RescueAttribute;
			}
			else if (controllerType.IsDefined(typeof (RescueAttribute), true))
			{
				att = controllerType.GetCustomAttributes(
					typeof (RescueAttribute), true)[0] as RescueAttribute;
			}

			if (att != null)
			{
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
			}

			return false;
		}

		#endregion

		#region Pre And Post view processing (overridables)

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
				((HttpContext) _context.UnderlyingContext).Items[ControllerContextKey] = this;
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
	}
}
