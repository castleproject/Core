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
	using System.Collections.Generic;
	using System.Threading;
	using System.Web;
	using Adapters;
	using Castle.Core;
	using Castle.MonoRail.Framework.Container;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Descriptors;
	using Providers;
	using Routing;
	using Services;

	/// <summary>
	/// Coordinates the creation of new <see cref="MonoRailHttpHandler"/> 
	/// and uses the configuration to obtain the correct factories 
	/// instances.
	/// </summary>
	public class MonoRailHttpHandlerFactory : IHttpHandlerFactory
	{
		private static readonly string CurrentEngineContextKey = "currentmrengineinstance";
		private static readonly string CurrentControllerKey = "currentmrcontroller";
		private static readonly string CurrentControllerContextKey = "currentmrcontrollercontext";
		private static readonly ReaderWriterLock locker = new ReaderWriterLock();

		private static IMonoRailConfiguration configuration;
		private static IMonoRailContainer mrContainer;
		private static IUrlTokenizer urlTokenizer;
		private static IEngineContextFactory engineContextFactory;
		private static IServiceProviderLocator serviceProviderLocator;
		private static IControllerFactory controllerFactory;
		private static IControllerContextFactory controllerContextFactory;
		private static IStaticResourceRegistry staticResourceRegistry;

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailHttpHandlerFactory"/> class.
		/// </summary>
		public MonoRailHttpHandlerFactory()
			: this(ServiceProviderLocator.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailHttpHandlerFactory"/> class.
		/// </summary>
		/// <param name="serviceLocator">The service locator.</param>
		public MonoRailHttpHandlerFactory(IServiceProviderLocator serviceLocator)
		{
			serviceProviderLocator = serviceLocator;
		}

		/// <summary>
		/// Returns an instance of a class that implements 
		/// the <see cref="T:System.Web.IHttpHandler"></see> interface.
		/// </summary>
		/// <param name="context">An instance of the <see cref="T:System.Web.HttpContext"></see> class that provides references to intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		/// <param name="requestType">The HTTP data transfer method (GET or POST) that the client uses.</param>
		/// <param name="url">The <see cref="P:System.Web.HttpRequest.RawUrl"></see> of the requested resource.</param>
		/// <param name="pathTranslated">The <see cref="P:System.Web.HttpRequest.PhysicalApplicationPath"></see> to the requested resource.</param>
		/// <returns>
		/// A new <see cref="T:System.Web.IHttpHandler"></see> object that processes the request.
		/// </returns>
		public virtual IHttpHandler GetHandler(HttpContext context,
											   String requestType,
											   String url, String pathTranslated)
		{
			PerformOneTimeInitializationIfNecessary(context);

			EnsureServices();

			HttpRequest req = context.Request;

			RouteMatch routeMatch = (RouteMatch)context.Items[RouteMatch.RouteMatchKey] ?? new RouteMatch();

			UrlInfo urlInfo = urlTokenizer.TokenizeUrl(req.FilePath, req.PathInfo, req.Url, req.IsLocal, req.ApplicationPath);

			if (urlInfo.Area == "MonoRail" && urlInfo.Controller == "Files")
			{
				return new ResourceFileHandler(urlInfo, staticResourceRegistry);
			}

			// TODO: Identify requests for files (js files) and serve them directly bypassing the flow

			IEngineContext engineContext = engineContextFactory.Create(mrContainer, urlInfo, context, routeMatch);
			engineContext.AddService(typeof(IEngineContext), engineContext);
			context.Items[CurrentEngineContextKey] = engineContext;

			IController controller;

			try
			{
				controller = controllerFactory.CreateController(urlInfo.Area, urlInfo.Controller);
			}
			catch(ControllerNotFoundException)
			{
				return new NotFoundHandler(urlInfo.Area, urlInfo.Controller, engineContext);
			}
			catch(Exception ex)
			{
				HttpResponse response = context.Response;

				if (response.StatusCode == 200)
				{
					response.StatusCode = 500;
				}

				engineContext.LastException = ex;

				engineContext.Services.ExtensionManager.RaiseUnhandledError(engineContext);

				throw new MonoRailException("Error creating controller " + urlInfo.Controller, ex);
			}

			ControllerMetaDescriptor controllerDesc =
				mrContainer.ControllerDescriptorProvider.BuildDescriptor(controller);

			IControllerContext controllerContext =
				controllerContextFactory.Create(urlInfo.Area, urlInfo.Controller, urlInfo.Action, controllerDesc, routeMatch);

			engineContext.CurrentController = controller;
			engineContext.CurrentControllerContext = controllerContext;

			context.Items[CurrentControllerKey] = controller;
			context.Items[CurrentControllerContextKey] = controllerContext;

			if (IsAsyncAction(controllerContext) == false)
			{
				return CreateHandler(controllerDesc, engineContext, controller, controllerContext);
			}
			else
			{
				return CreateAsyncHandler(controllerDesc, engineContext, (IAsyncController)controller, controllerContext);
			}
		}

		/// <summary>
		/// Creates the handler.
		/// </summary>
		/// <param name="controllerDesc">The controller descriptor.</param>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns>
		/// A new <see cref="T:System.Web.IHttpHandler"></see> object that processes the request.
		/// </returns>
		protected virtual IHttpHandler CreateHandler(ControllerMetaDescriptor controllerDesc, IEngineContext engineContext,
													 IController controller, IControllerContext controllerContext)
		{
			if (IgnoresSession(controllerDesc.ControllerDescriptor))
			{
				return new SessionlessMonoRailHttpHandler(engineContext, controller, controllerContext);
			}
			return new MonoRailHttpHandler(engineContext, controller, controllerContext);
		}


		/// <summary>
		/// Creates the handler.
		/// </summary>
		/// <param name="controllerDesc">The controller descriptor.</param>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns>
		/// A new <see cref="T:System.Web.IHttpHandler"></see> object that processes the request.
		/// </returns>
		protected virtual IHttpAsyncHandler CreateAsyncHandler(ControllerMetaDescriptor controllerDesc,
															   IEngineContext engineContext, IAsyncController controller,
															   IControllerContext controllerContext)
		{
			if (IgnoresSession(controllerDesc.ControllerDescriptor))
			{
				return new AsyncSessionlessMonoRailHttpHandler(engineContext, controller, controllerContext);
			}
			return new AsyncMonoRailHttpHandler(engineContext, controller, controllerContext);
		}

		/// <summary>
		/// Enables a factory to reuse an existing handler instance.
		/// </summary>
		/// <param name="handler">The <see cref="T:System.Web.IHttpHandler"></see> object to reuse.</param>
		public virtual void ReleaseHandler(IHttpHandler handler)
		{
		}

		/// <summary>
		/// Resets the state (only used from test cases)
		/// </summary>
		public void ResetState()
		{
			configuration = null;
			mrContainer = null;
			urlTokenizer = null;
			engineContextFactory = null;
			serviceProviderLocator = null;
			controllerFactory = null;
			controllerContextFactory = null;
			staticResourceRegistry = null;
		}

		/// <summary>
		/// Gets or sets the configuration.
		/// </summary>
		/// <value>The configuration.</value>
		public IMonoRailConfiguration Configuration
		{
			get { return configuration; }
			set { configuration = value; }
		}

		/// <summary>
		/// Gets or sets the container.
		/// </summary>
		/// <value>The container.</value>
		public IMonoRailContainer Container
		{
			get { return mrContainer; }
			set { mrContainer = value; }
		}

		/// <summary>
		/// Gets or sets the service provider locator.
		/// </summary>
		/// <value>The service provider locator.</value>
		public IServiceProviderLocator ProviderLocator
		{
			get { return serviceProviderLocator; }
			set { serviceProviderLocator = value; }
		}

		/// <summary>
		/// Gets or sets the URL tokenizer.
		/// </summary>
		/// <value>The URL tokenizer.</value>
		public IUrlTokenizer UrlTokenizer
		{
			get { return urlTokenizer; }
			set { urlTokenizer = value; }
		}

		/// <summary>
		/// Gets or sets the engine context factory.
		/// </summary>
		/// <value>The engine context factory.</value>
		public IEngineContextFactory EngineContextFactory
		{
			get { return engineContextFactory; }
			set { engineContextFactory = value; }
		}

		/// <summary>
		/// Gets or sets the controller factory.
		/// </summary>
		/// <value>The controller factory.</value>
		public IControllerFactory ControllerFactory
		{
			get { return controllerFactory; }
			set { controllerFactory = value; }
		}

		/// <summary>
		/// Gets or sets the controller context factory.
		/// </summary>
		/// <value>The controller context factory.</value>
		public IControllerContextFactory ControllerContextFactory
		{
			get { return controllerContextFactory; }
			set { controllerContextFactory = value; }
		}

		/// <summary>
		/// Checks whether we should ignore session for the specified controller.
		/// </summary>
		/// <param name="controllerDesc">The controller desc.</param>
		/// <returns></returns>
		protected virtual bool IgnoresSession(ControllerDescriptor controllerDesc)
		{
			return controllerDesc.Sessionless;
		}

		/// <summary>
		/// Checks whether the target action is an async method.
		/// </summary>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns></returns>
		protected virtual bool IsAsyncAction(IControllerContext controllerContext)
		{
			if (controllerContext.ControllerDescriptor == null || controllerContext.Action == null)
			{
				return false;
			}
			return controllerContext.ControllerDescriptor.Actions[controllerContext.Action] is AsyncActionPair;
		}

		/// <summary>
		/// Creates the default service container.
		/// </summary>
		/// <param name="userServiceProvider">The user service provider.</param>
		/// <param name="appInstance">The app instance.</param>
		/// <returns></returns>
		protected virtual IMonoRailContainer CreateDefaultMonoRailContainer(IServiceProviderEx userServiceProvider,
																			HttpApplication appInstance)
		{
			DefaultMonoRailContainer container = new DefaultMonoRailContainer(userServiceProvider);

			container.UseServicesFromParent();
			container.InstallPrimordialServices();
			container.Configure(Configuration);

			FireContainerCreated(appInstance, container);

			// Too dependent on Http and MR surroundings services to be moved to Container class
			if (!container.HasService<IServerUtility>())
			{
				container.AddService<IServerUtility>(new ServerUtilityAdapter(appInstance.Context.Server));
			}
			if (!container.HasService<IRoutingEngine>())
			{
				container.AddService<IRoutingEngine>(RoutingModuleEx.Engine);
			}

			container.InstallMissingServices();
			container.StartExtensionManager();

			FireContainerInitialized(appInstance, container);

			return container;
		}

		#region Static accessors

		/// <summary>
		/// Gets the current engine context.
		/// </summary>
		/// <value>The current engine context.</value>
		public static IEngineContext CurrentEngineContext
		{
			get
			{
				if (HttpContext.Current == null)//for tests
					return null;
				return HttpContext.Current.Items[CurrentEngineContextKey] as IEngineContext;
			}
		}

		/// <summary>
		/// Gets the current controller.
		/// </summary>
		/// <value>The current controller.</value>
		public static IController CurrentController
		{
			get { return HttpContext.Current.Items[CurrentControllerKey] as IController; }
		}

		/// <summary>
		/// Gets the current controller context.
		/// </summary>
		/// <value>The current controller context.</value>
		public static IControllerContext CurrentControllerContext
		{
			get { return HttpContext.Current.Items[CurrentControllerContextKey] as IControllerContext; }
		}

		#endregion

		private void PerformOneTimeInitializationIfNecessary(HttpContext context)
		{
			locker.AcquireReaderLock(Timeout.Infinite);

			if (mrContainer != null)
			{
				locker.ReleaseReaderLock();
				return;
			}

			locker.UpgradeToWriterLock(Timeout.Infinite);

			if (mrContainer != null) // remember remember the race condition
			{
				locker.ReleaseWriterLock();
				return;
			}

			try
			{
				if (configuration == null)
				{
					configuration = ObtainConfiguration(context.ApplicationInstance);
				}

				IServiceProviderEx userServiceProvider = serviceProviderLocator.LocateProvider();

				mrContainer = CreateDefaultMonoRailContainer(userServiceProvider, context.ApplicationInstance);
			}
			finally
			{
				locker.ReleaseWriterLock();
			}
		}

		private void FireContainerCreated(HttpApplication instance, DefaultMonoRailContainer container)
		{
			IMonoRailContainerEvents events = instance as IMonoRailContainerEvents;

			if (events != null)
			{
				events.Created(container);
			}
		}

		private void FireContainerInitialized(HttpApplication instance, DefaultMonoRailContainer container)
		{
			IMonoRailContainerEvents events = instance as IMonoRailContainerEvents;

			if (events != null)
			{
				events.Initialized(container);
			}
		}

		private IMonoRailConfiguration ObtainConfiguration(HttpApplication appInstance)
		{
			IMonoRailConfiguration config = MonoRailConfiguration.GetConfig();

			IMonoRailConfigurationEvents events = appInstance as IMonoRailConfigurationEvents;

			if (events != null)
			{
				config = config ?? new MonoRailConfiguration();

				events.Configure(config);
			}

			if (config == null)
			{
				throw new ApplicationException("You have to provide a small configuration to use " +
											   "MonoRail. This can be done using the web.config or " +
											   "your global asax (your class that extends HttpApplication) " +
											   "through the method MonoRail_Configure(IMonoRailConfiguration config). " +
											   "Check the samples or the documentation.");
			}

			return config;
		}

		private void EnsureServices()
		{
			if (urlTokenizer == null)
			{
				urlTokenizer = mrContainer.UrlTokenizer;
			}
			if (engineContextFactory == null)
			{
				engineContextFactory = mrContainer.EngineContextFactory;
			}
			if (controllerFactory == null)
			{
				controllerFactory = mrContainer.ControllerFactory;
			}
			if (controllerContextFactory == null)
			{
				controllerContextFactory = mrContainer.ControllerContextFactory;
			}
			if (staticResourceRegistry == null)
			{
				staticResourceRegistry = mrContainer.StaticResourceRegistry;
			}
		}

		/// <summary>
		/// Handles the controller not found situation
		/// </summary>
		public class NotFoundHandler : IHttpHandler
		{
			private readonly string area;
			private readonly string controller;
			private readonly IEngineContext engineContext;

			/// <summary>
			/// Initializes a new instance of the <see cref="NotFoundHandler"/> class.
			/// </summary>
			/// <param name="area">The area.</param>
			/// <param name="controller">The controller.</param>
			/// <param name="engineContext">The engine context.</param>
			public NotFoundHandler(string area, string controller, IEngineContext engineContext)
			{
				this.area = area;
				this.controller = controller;
				this.engineContext = engineContext;
			}

			/// <summary>
			/// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
			/// </summary>
			/// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
			public void ProcessRequest(HttpContext context)
			{
				engineContext.Response.StatusCode = 404;
				engineContext.Response.StatusDescription = "Not found";

				if (engineContext.Services.ViewEngineManager.HasTemplate("rescues/404"))
				{
					Dictionary<string, object> parameters = new Dictionary<string, object>();

					engineContext.Services.ViewEngineManager.Process("rescues/404", null, engineContext.Response.Output, parameters);

					return; // gracefully handled
				}

				throw new ControllerNotFoundException(area, controller);
			}

			/// <summary>
			/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
			/// </summary>
			/// <value></value>
			/// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
			public bool IsReusable
			{
				get { return false; }
			}
		}
	}
}