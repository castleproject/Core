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
	using System.Collections.Specialized;
	using System.IO;
	using System.Reflection;
	using System.Threading;
	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// 
	/// </summary>
	public class ControllerLifecycleExecutor : IControllerLifecycleExecutor, IServiceEnabledComponent, IDisposable
	{
		public const String ExecutorEntry = "mr.executor";

		private readonly Controller controller;
		private readonly IRailsEngineContext context;

		private ControllerMetaDescriptor metaDescriptor;
		private IServiceProvider serviceProvider;
		private ILogger logger = NullLogger.Instance;

		/// <summary>
		/// The reference to the <see cref="IViewEngine"/> instance
		/// </summary>
		private IViewEngine viewEngine;

		private IResourceFactory resourceFactory;
		private IScaffoldingSupport scaffoldSupport;

		/// <summary>
		/// Reference to the <see cref="IFilterFactory"/> instance
		/// </summary>
		private IFilterFactory filterFactory;

		/// <summary>
		/// Holds the filters associated with the action
		/// </summary>
		private FilterDescriptor[] filters;

		private IDynamicAction dynAction;
		private MethodInfo actionMethod;

		private bool skipFilters;
		private bool hasError;
		private bool hasConfiguredCache;
		private Exception exceptionToThrow;
		private IDictionary filtersToSkip;
		private ActionMetaDescriptor actionDesc;

		/// <summary>
		/// Initializes a new instance of 
		/// the <see cref="ControllerLifecycleExecutor"/> class.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		public ControllerLifecycleExecutor(Controller controller, IRailsEngineContext context)
		{
			this.controller = controller;
			this.context = context;
		}

		#region IServiceEnabledComponent

		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public void Service(IServiceProvider provider)
		{
			viewEngine = (IViewEngine) provider.GetService(typeof(IViewEngine));
			filterFactory = (IFilterFactory) provider.GetService(typeof(IFilterFactory));
			resourceFactory = (IResourceFactory) provider.GetService(typeof(IResourceFactory));
			scaffoldSupport = (IScaffoldingSupport) provider.GetService(typeof(IScaffoldingSupport));

			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(ControllerLifecycleExecutor));
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			DisposeFilters();

			ReleaseResources();
		}

		#endregion

		#region IControllerLifecycleExecutor

		public void InitializeController(string areaName, string controllerName, string actionName)
		{
			InitializeControllerFieldsFromServiceProvider();
			controller.InitializeControllerState(areaName, controllerName, actionName);
			controller.LayoutName = ObtainDefaultLayoutName();
			CreateAndInitializeHelpers();
			CreateFiltersDescriptors();
			ProcessScaffoldIfPresent();
			ActionProviderUtil.RegisterActions(controller);
			controller.DetermineIfPostBack();

			// Record the action
			controller.SetEvaluatedAction(actionName);

			// Record the default view for this area/controller/action
			controller.RenderView(actionName);

			controller.InternalInitialize();

			// If we have an HttpContext available, store the original view name
			if (controller.HttpContext != null)
			{
				if (!controller.HttpContext.Items.Contains(Constants.OriginalViewKey))
				{
					controller.HttpContext.Items[Constants.OriginalViewKey] = controller._selectedViewName;
				}
			}
		}

		public bool SelectAction(string actionName, string controllerName)
		{
			try
			{
				// Look for the target method

				actionMethod = controller.SelectMethod(actionName, metaDescriptor.Actions, context.Request);

				// If we couldn't find a method for this action, look for a dynamic action
				dynAction = null;

				if (actionMethod == null)
				{
					dynAction = controller.DynamicActions[actionName] as IDynamicAction;

					if (dynAction == null)
					{
						actionMethod = FindOutDefaultMethod(new object[0]);

						if (actionMethod == null)
						{
							throw new ControllerException(
								String.Format("Unable to locate action [{0}] on controller [{1}].",
								              actionName, controllerName));
						}
					}
				}
				else
				{
					actionDesc = metaDescriptor.GetAction(actionMethod);

					// Overrides the current layout, if the action specifies one
					if (actionDesc.Layout != null)
					{
						controller.LayoutName = actionDesc.Layout.LayoutName;
					}

					if (actionDesc.AccessibleThrough != null)
					{
						string verbName = actionDesc.AccessibleThrough.Verb.ToString();
						string requestType = context.RequestType;

						if (String.Compare(verbName, requestType, true) != 0)
						{
							throw new ControllerException(string.Format("Access to the action [{0}] " +
							                                            "on controller [{1}] is not allowed by the http verb [{2}].",
							                                            actionName, controllerName, requestType));
						}
					}
				}

				// Record the action
				controller.SetEvaluatedAction(actionName);

				// Record the default view for this area/controller/action
				controller.RenderView(actionName);
				
				// Compute the filters that should be skipped for this action/method
				filtersToSkip = new HybridDictionary();
				skipFilters = ShouldSkip(actionMethod);

				return true;
			}
			catch(Exception ex)
			{
				// There was an exception selecting the method

				hasError = true;

				exceptionToThrow = ex;

				return false;
			}
		}

		/// <summary>
		/// Executes the method or the dynamic action
		/// </summary>
		/// <param name="actionArgs">The action args.</param>
		public void ProcessSelectedAction(params object[] actionArgs)
		{			
			try
			{
				if (RunBeforeActionFilters())
				{
					PrepareResources();
					
					if (actionMethod != null)
					{
						controller.InvokeMethod(actionMethod, actionArgs);
					}
					else
					{
						dynAction.Execute(controller);
					}
					
					if (!hasConfiguredCache && 
					    !context.Response.WasRedirected &&
						actionDesc != null &&
						context.Response.StatusCode == 200)
					{
						// We need to prevent that a controller.Send
						// ends up configuring the cache for a different URL
						hasConfiguredCache = true;
						
						foreach(ICachePolicyConfigurer configurer in actionDesc.CacheConfigurers)
						{
							configurer.Configure(context.Response.CachePolicy);
						}
					}
				}
			}
			catch(ThreadAbortException)
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error("ThreadAbortException, process aborted");
				}

				hasError = true;
			}
			catch(Exception ex)
			{
				exceptionToThrow = ex;

				hasError = true;

				if (logger.IsErrorEnabled)
				{
					logger.Error("Exception during action process", ex);
				}

				if (context.Response.WasRedirected) return;
				
				PerformErrorHandling();
			}

			if (context.Response.WasRedirected) return;
			
			RunAfterActionFilters();

			// If we haven't failed anywhere and no redirect was issued
			if (!hasError && !context.Response.WasRedirected)
			{
				// Render the actual view then cleanup
				ProcessView();
			}
			
			RunAfterRenderFilters();
		}

		/// <summary>
		/// Performs the error handling:
		/// <para>
		/// - Tries to run the rescue page<br/>
		/// - Throws the exception<br/>
		/// </para>
		/// </summary>
		public void PerformErrorHandling()
		{
			if (context.Response.WasRedirected) return;
			
			// Try and perform the rescue
			if (PerformRescue(actionMethod, exceptionToThrow))
			{
				exceptionToThrow = null;
			}

			RaiseOnActionExceptionOnExtension();

			if (exceptionToThrow != null)
			{
				throw exceptionToThrow;
			}
		}

		/// <summary>
		/// Runs the start request filters.
		/// </summary>
		/// <returns><c>false</c> if the process should be stopped</returns>
		public bool RunStartRequestFilters()
		{
			hasError = false;
			exceptionToThrow = null;

			try
			{
				// If we are supposed to run the filters...
				if (!skipFilters)
				{
					// ...run them. If they fail...
					if (!ProcessFilters(ExecuteEnum.StartRequest))
					{
						// Record that they failed.
						hasError = true;
					}
				}
			}
			catch(ThreadAbortException)
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error("ThreadAbortException, process aborted");
				}

				hasError = true;
			}
			catch(Exception ex)
			{
				hasError = true;

				if (logger.IsErrorEnabled)
				{
					logger.Error("Exception during filter process", ex);
				}

				exceptionToThrow = ex;
			}

			return ! hasError;
		}

		/// <summary>
		/// Gets a value indicating whether an error has happened during controller processing
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if has error; otherwise, <see langword="false"/>.
		/// </value>
		public bool HasError
		{
			get { return hasError; }
		}

		/// <summary>
		/// Gets the controller.
		/// </summary>
		/// <value>The controller.</value>
		public Controller Controller
		{
			get { return controller; }
		}

		#endregion

		internal void InitializeControllerFieldsFromServiceProvider()
		{
			controller.InitializeFieldsFromServiceProvider(context);

			serviceProvider = context;

			metaDescriptor = controller.metaDescriptor;

			controller.viewEngine = viewEngine;

			ILoggerFactory loggerFactory = (ILoggerFactory) context.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				controller.logger = loggerFactory.Create(controller.GetType().Name);
			}
		}

		protected void CreateAndInitializeHelpers()
		{
			HybridDictionary helpers = new HybridDictionary();

			// Custom helpers

			foreach(HelperDescriptor helper in metaDescriptor.Helpers)
			{
				object helperInstance = Activator.CreateInstance(helper.HelperType);

				IControllerAware aware = helperInstance as IControllerAware;

				if (aware != null)
				{
					aware.SetController(controller);
				}

				PerformAdditionalHelperInitialization(helperInstance);

				if (helpers.Contains(helper.Name))
				{
					throw new ControllerException(String.Format("Found a duplicate helper " +
					                                            "attribute named '{0}' on controller '{1}'", helper.Name,
					                                            controller.Name));
				}

				helpers.Add(helper.Name, helperInstance);
			}

			CreateStandardHelpers(helpers);

			controller.helpers = helpers;
		}

		/// <summary>
		/// Runs the after view rendering filters.
		/// </summary>
		/// <returns><c>false</c> if the process should be stopped</returns>
		private bool RunBeforeActionFilters()
		{
			try
			{
				// If we are supposed to run the filters...
				if (!skipFilters)
				{
					// ...run them. If they fail...
					if (!ProcessFilters(ExecuteEnum.BeforeAction))
					{
						// Record that they failed.
						hasError = true;
					}
				}
			}
			catch(ThreadAbortException)
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error("ThreadAbortException, process aborted");
				}

				hasError = true;
			}
			catch(Exception ex)
			{
				hasError = true;

				if (logger.IsErrorEnabled)
				{
					logger.Error("Exception during filter process", ex);
				}

				exceptionToThrow = ex;
			}

			return ! hasError;
		}

		/// <summary>
		/// Runs the after action filters.
		/// </summary>
		private void RunAfterActionFilters()
		{
			if (skipFilters) return;

			ProcessFilters(ExecuteEnum.AfterAction);
		}

		/// <summary>
		/// Runs the after view rendering filters.
		/// </summary>
		private void RunAfterRenderFilters()
		{
			if (skipFilters) return;

			ProcessFilters(ExecuteEnum.AfterRendering);
		}

		private void CreateFiltersDescriptors()
		{
			if (metaDescriptor.Filters.Length != 0)
			{
				filters = CopyFilterDescriptors();
			}
		}

		private void CreateStandardHelpers(HybridDictionary helpers)
		{
			AbstractHelper[] builtInHelpers =
				new AbstractHelper[]
					{
						new AjaxHelper(),
						new EffectsFatHelper(), new Effects2Helper(),
						new DateFormatHelper(), new HtmlHelper(),
						new ValidationHelper(), new DictHelper(),
						new PaginationHelper(), new FormHelper()
					};

			foreach(AbstractHelper helper in builtInHelpers)
			{
				helper.SetController(controller);

				String helperName = helper.GetType().Name;

				if (!helpers.Contains(helperName))
				{
					helpers[helperName] = helper;
				}

				PerformAdditionalHelperInitialization(helper);
			}
		}

		/// <summary>
		/// Performs the additional helper initialization
		/// checking if the helper instance implements <see cref="IServiceEnabledComponent"/>.
		/// </summary>
		/// <param name="helperInstance">The helper instance.</param>
		private void PerformAdditionalHelperInitialization(object helperInstance)
		{
			IServiceEnabledComponent serviceEnabled = helperInstance as IServiceEnabledComponent;

			if (serviceEnabled != null)
			{
				serviceEnabled.Service(serviceProvider);
			}
		}

		/// <summary>
		/// Invokes the scaffold support if the controller
		/// is associated with a scaffold
		/// </summary>
		private void ProcessScaffoldIfPresent()
		{
			if (metaDescriptor.Scaffoldings.Count != 0)
			{
				if (scaffoldSupport == null)
				{
					String message = "You must enable scaffolding support on the " +
					                 "configuration file, or, to use the standard ActiveRecord support " +
					                 "copy the necessary assemblies to the bin folder.";

					throw new RailsException(message);
				}

				scaffoldSupport.Process(controller);
			}
		}

		private bool IsClientConnected
		{
			get { return context.Response.IsClientConnected; }
		}

		#region Resources

		private void PrepareResources()
		{
			CreateResources(actionMethod);
		}
		
		protected void CreateResources(MethodInfo method)
		{
			controller.resources = new ResourceDictionary();

			Assembly typeAssembly = controller.GetType().Assembly;

			foreach(ResourceDescriptor resource in metaDescriptor.Resources)
			{
				controller.resources.Add(resource.Name, resourceFactory.Create(resource, typeAssembly));
			}

			if (method == null) return;

			ActionMetaDescriptor actionMeta = metaDescriptor.GetAction(method);

			foreach(ResourceDescriptor resource in actionMeta.Resources)
			{
				controller.resources[resource.Name] = resourceFactory.Create(resource, typeAssembly);
			}
		}

		protected void ReleaseResources()
		{
			if (controller.resources == null) return;

			foreach(IResource resource in controller.resources.Values)
			{
				resourceFactory.Release(resource);
			}
		}

		#endregion

		#region Filters

		protected internal bool ShouldSkip(MethodInfo method)
		{
			if (method == null)
			{
				// Dynamic Action, run the filters if we have any
				return (filters == null);
			}

			if (filters == null)
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

			foreach(SkipFilterAttribute skipfilter in actionMeta.SkipFilters)
			{
				// SkipAllFilters handling...
				if (skipfilter.BlanketSkip)
				{
					return true;
				}

				filtersToSkip[skipfilter.FilterType] = String.Empty;
			}

			return false;
		}

		/// <summary>
		/// Clones all Filter descriptors, in order to get a writable copy.
		/// </summary>
		protected internal FilterDescriptor[] CopyFilterDescriptors()
		{
			FilterDescriptor[] clone = (FilterDescriptor[]) metaDescriptor.Filters.Clone();

			for(int i = 0; i < clone.Length; i++)
			{
				clone[i] = (FilterDescriptor) clone[i].Clone();
			}

			return clone;
		}

		private bool ProcessFilters(ExecuteEnum when)
		{
			foreach(FilterDescriptor desc in filters)
			{
				if (filtersToSkip.Contains(desc.FilterType))
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

		private bool ProcessFilter(ExecuteEnum when, FilterDescriptor desc)
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
					logger.Debug("Running filter {0}/{1}", when, desc.FilterType.FullName);
				}

				return desc.FilterInstance.Perform(when, context, controller);
			}
			catch(Exception ex)
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error("Error processing filter " + desc.FilterType.FullName, ex);
				}

				throw;
			}
		}

		private void DisposeFilters()
		{
			if (filters == null) return;

			foreach(FilterDescriptor desc in filters)
			{
				if (desc.FilterInstance != null)
				{
					filterFactory.Release(desc.FilterInstance);
				}
			}
		}

		#endregion

		#region Views and Layout

		protected String ObtainDefaultLayoutName()
		{
			if (metaDescriptor.Layout != null)
			{
				return metaDescriptor.Layout.LayoutName;
			}
			else
			{
				String defaultLayout = String.Format("layouts/{0}", controller.Name);

				if (controller.HasTemplate(defaultLayout))
				{
					return controller.Name;
				}
			}

			return null;
		}

		private void ProcessView()
		{
			if (controller._selectedViewName != null)
			{
				viewEngine.Process(context, controller, controller._selectedViewName);
			}
		}

		#endregion

		#region Rescue

		/// <summary>
		/// Performs the rescue.
		/// </summary>
		/// <param name="method">The action (can be null).</param>
		/// <param name="ex">The exception.</param>
		/// <returns></returns>
		protected bool PerformRescue(MethodInfo method, Exception ex)
		{
			context.LastException = (ex is TargetInvocationException) ? ex.InnerException : ex;

			// Dynamic action 
			if (method == null) return false;

			Type exceptionType = context.LastException.GetType();

			ActionMetaDescriptor actionMeta = metaDescriptor.GetAction(method);

			if (actionMeta.SkipRescue != null) return false;

			RescueDescriptor att = GetRescueFor(actionMeta.Rescues, exceptionType);

			if (att == null)
			{
				att = GetRescueFor(metaDescriptor.Rescues, exceptionType);

				if (att == null) return false;
			}

			try
			{
				controller._selectedViewName = Path.Combine("rescues", att.ViewName);
				ProcessView();
				return true;
			}
			catch(Exception exception)
			{
				// In this situation, the rescue view could not be found
				// So we're back to the default error exibition

				if (logger.IsFatalErrorEnabled)
				{
					logger.FatalError("Failed to process rescue view. View name " +
					                  controller._selectedViewName, exception);
				}
			}

			return false;
		}

		protected RescueDescriptor GetRescueFor(IList rescues, Type exceptionType)
		{
			if (rescues == null || rescues.Count == 0) return null;

			RescueDescriptor bestCandidate = null;

			foreach(RescueDescriptor rescue in rescues)
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

		#region Extension

		protected void RaiseOnActionExceptionOnExtension()
		{
			ExtensionManager manager =
				(ExtensionManager) serviceProvider.GetService(typeof(ExtensionManager));

			manager.RaiseActionError(context);
		}

		#endregion

		/// <summary>
		/// The following lines were added to handle _default processing
		/// if present look for and load _default action method
		/// <seealso cref="DefaultActionAttribute"/>
		/// </summary>
		private MethodInfo FindOutDefaultMethod(object[] methodArgs)
		{
			if (metaDescriptor.DefaultAction != null)
			{
				return controller.SelectMethod(
					metaDescriptor.DefaultAction.DefaultAction,
					metaDescriptor.Actions, context.Request, methodArgs);
			}

			return null;
		}
	}
}