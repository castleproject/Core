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

namespace Castle.MonoRail.Framework.Test
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Design;
	using Castle.Components.Common.EmailSender;
	using Castle.Components.Validator;
	using Providers;
	using Resources;
	using Services;

	/// <summary>
	/// Pendent
	/// </summary>
	public class StubMonoRailServices : IMonoRailServices, IServiceContainer
	{
		private IUrlTokenizer urlTokenizer;
		private IUrlBuilder urlBuilder;
		private ICacheProvider cacheProvider;
		private IEngineContextFactory engineContextFactory;
		private IControllerFactory controllerFactory;
		private IControllerContextFactory controllerContextFactory;
		private IControllerTree controllerTree;
		private IViewSourceLoader viewSourceLoader;
		private IFilterFactory filterFactory;
		private IControllerDescriptorProvider controllerDescriptorProvider;
		private IViewEngineManager viewEngineManager;
		private IValidatorRegistry validatorRegistry;
		private IActionSelector actionSelector;
		private IScaffoldingSupport scaffoldSupport;
		private IJSONSerializer jsonSerializer;
		private IStaticResourceRegistry staticResourceRegistry;
		private IEmailTemplateService emailTemplateService;
		private IEmailSender emailSender;
		private IResourceFactory resourceFactory;
		private ITransformFilterFactory transformFilterFactory;
		private ExtensionManager extensionManager;
		private readonly Dictionary<Type, object> service2Impl = new Dictionary<Type, object>();
		private IServiceInitializer serviceInitializer;
		private IHelperFactory helperFactory;
		private IDynamicActionProviderFactory dynamicActionProviderFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="StubMonoRailServices"/> class with default mock services.
		/// </summary>
		public StubMonoRailServices() : this(new DefaultUrlBuilder(new StubServerUtility(), new StubRoutingEngine()),
		                             new DefaultFilterFactory(),
		                             new StubViewEngineManager(),
		                             new DefaultActionSelector(),
									 new DefaultDynamicActionProviderFactory())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StubMonoRailServices"/> class.
		/// </summary>
		/// <param name="urlBuilder">The URL builder.</param>
		/// <param name="filterFactory">The filter factory.</param>
		/// <param name="viewEngineManager">The view engine manager.</param>
		/// <param name="actionSelector">The action selector.</param>
		/// <param name="dynamicActionProviderFactory">The dynamic action provider factory.</param>
		public StubMonoRailServices(IUrlBuilder urlBuilder, IFilterFactory filterFactory, IViewEngineManager viewEngineManager,
		                    IActionSelector actionSelector,IDynamicActionProviderFactory dynamicActionProviderFactory)
		{
			this.urlBuilder = urlBuilder;
			this.filterFactory = filterFactory;
			this.viewEngineManager = viewEngineManager;
			this.actionSelector = actionSelector;
			this.dynamicActionProviderFactory = dynamicActionProviderFactory;
			controllerTree = new DefaultControllerTree();
			controllerFactory = new DefaultControllerFactory(controllerTree);
			staticResourceRegistry = new DefaultStaticResourceRegistry();

			controllerContextFactory = new DefaultControllerContextFactory();

			controllerDescriptorProvider = new DefaultControllerDescriptorProvider(
				new DefaultHelperDescriptorProvider(),
				new DefaultFilterDescriptorProvider(),
				new DefaultLayoutDescriptorProvider(),
				new DefaultRescueDescriptorProvider(),
				new DefaultResourceDescriptorProvider(),
				new DefaultTransformFilterDescriptorProvider(), 
				new DefaultReturnBinderDescriptorProvider(),
				new DefaultDynamicActionProviderDescriptorProvider());

			resourceFactory = new DefaultResourceFactory();
			scaffoldSupport = new StubScaffoldingSupport();
			cacheProvider = new StubCacheProvider();
			helperFactory = new DefaultHelperFactory();
			serviceInitializer = new DefaultServiceInitializer();

			extensionManager = new ExtensionManager(this);

			validatorRegistry = new CachedValidationRegistry();

			jsonSerializer = new NewtonsoftJSONSerializer();
		}

		#region IServiceContainer

		/// <summary>
		/// Adds the specified service to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="serviceInstance">An instance of the service type to add. This object must implement or inherit from the type indicated by the <paramref name="serviceType"/> parameter.</param>
		public void AddService(Type serviceType, object serviceInstance)
		{
			service2Impl[serviceType] = serviceInstance;
		}

		/// <summary>
		/// Adds the specified service to the service container, and optionally promotes the service to any parent service containers.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="serviceInstance">An instance of the service type to add. This object must implement or inherit from the type indicated by the <paramref name="serviceType"/> parameter.</param>
		/// <param name="promote">true to promote this request to any parent service containers; otherwise, false.</param>
		public void AddService(Type serviceType, object serviceInstance, bool promote)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Adds the specified service to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="callback">A callback object that is used to create the service. This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Adds the specified service to the service container, and optionally promotes the service to parent service containers.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="callback">A callback object that is used to create the service. This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
		/// <param name="promote">true to promote this request to any parent service containers; otherwise, false.</param>
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes the specified service type from the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to remove.</param>
		public void RemoveService(Type serviceType)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes the specified service type from the service container, and optionally promotes the service to parent service containers.
		/// </summary>
		/// <param name="serviceType">The type of service to remove.</param>
		/// <param name="promote">true to promote this request to any parent service containers; otherwise, false.</param>
		public void RemoveService(Type serviceType, bool promote)
		{
			throw new NotImplementedException();
		}

		#endregion

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
		/// Gets or sets the URL builder.
		/// </summary>
		/// <value>The URL builder.</value>
		public IUrlBuilder UrlBuilder
		{
			get { return urlBuilder; }
			set { urlBuilder = value; }
		}

		/// <summary>
		/// Gets or sets the cache provider.
		/// </summary>
		/// <value>The cache provider.</value>
		public ICacheProvider CacheProvider
		{
			get { return cacheProvider; }
			set { cacheProvider = value; }
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
		/// Gets or sets the controller tree.
		/// </summary>
		/// <value>The controller tree.</value>
		public IControllerTree ControllerTree
		{
			get { return controllerTree; }
			set { controllerTree = value; }
		}

		/// <summary>
		/// Gets or sets the view source loader.
		/// </summary>
		/// <value>The view source loader.</value>
		public IViewSourceLoader ViewSourceLoader
		{
			get { return viewSourceLoader; }
			set { viewSourceLoader = value; }
		}

		/// <summary>
		/// Gets or sets the filter factory.
		/// </summary>
		/// <value>The filter factory.</value>
		public IFilterFactory FilterFactory
		{
			get { return filterFactory; }
			set { filterFactory = value; }
		}

		/// <summary>
		/// Gets or sets the dynamic action provider factory.
		/// </summary>
		/// <value>The dynamic action provider factory.</value>
		public IDynamicActionProviderFactory DynamicActionProviderFactory
		{
			get { return dynamicActionProviderFactory; }
			set { dynamicActionProviderFactory = value; }
		}

		/// <summary>
		/// Gets or sets the controller descriptor provider.
		/// </summary>
		/// <value>The controller descriptor provider.</value>
		public IControllerDescriptorProvider ControllerDescriptorProvider
		{
			get { return controllerDescriptorProvider; }
			set { controllerDescriptorProvider = value; }
		}

		/// <summary>
		/// Gets or sets the view engine manager.
		/// </summary>
		/// <value>The view engine manager.</value>
		public IViewEngineManager ViewEngineManager
		{
			get { return viewEngineManager; }
			set { viewEngineManager = value; }
		}

		/// <summary>
		/// Gets or sets the validator registry.
		/// </summary>
		/// <value>The validator registry.</value>
		public IValidatorRegistry ValidatorRegistry
		{
			get { return validatorRegistry; }
			set { validatorRegistry = value; }
		}

		/// <summary>
		/// Gets or sets the action selector.
		/// </summary>
		/// <value>The action selector.</value>
		public IActionSelector ActionSelector
		{
			get { return actionSelector; }
			set { actionSelector = value; }
		}

		/// <summary>
		/// Gets or sets the scaffold support.
		/// </summary>
		/// <value>The scaffold support.</value>
		public IScaffoldingSupport ScaffoldSupport
		{
			get { return scaffoldSupport; }
			set { scaffoldSupport = value; }
		}

		/// <summary>
		/// Gets or sets the JSON serializer.
		/// </summary>
		/// <value>The JSON serializer.</value>
		public IJSONSerializer JSONSerializer
		{
			get { return jsonSerializer; }
			set { jsonSerializer = value; }
		}

		/// <summary>
		/// Gets or sets the static resource registry service.
		/// </summary>
		/// <value>The static resource registry.</value>
		public IStaticResourceRegistry StaticResourceRegistry
		{
			get { return staticResourceRegistry; }
			set { staticResourceRegistry = value; }
		}

		/// <summary>
		/// Gets or sets the email template service.
		/// </summary>
		/// <value>The email template service.</value>
		public IEmailTemplateService EmailTemplateService
		{
			get { return emailTemplateService; }
			set { emailTemplateService = value; }
		}

		/// <summary>
		/// Gets or sets the email sender.
		/// </summary>
		/// <value>The email sender.</value>
		public IEmailSender EmailSender
		{
			get { return emailSender; }
			set { emailSender = value; }
		}

		/// <summary>
		/// Gets or sets the resource factory.
		/// </summary>
		/// <value>The resource factory.</value>
		public IResourceFactory ResourceFactory
		{
			get { return resourceFactory; }
			set { resourceFactory = value; }
		}

		/// <summary>
		/// Gets or sets the transformfilter factory.
		/// </summary>
		/// <value>The resource factory.</value>
		public ITransformFilterFactory TransformFilterFactory
		{
			get { return transformFilterFactory; }
			set { transformFilterFactory = value; }
		}

		/// <summary>
		/// Gets or sets the service initializer.
		/// </summary>
		/// <value>The service initializer.</value>
		public IServiceInitializer ServiceInitializer
		{
			get { return serviceInitializer; }
			set { serviceInitializer = value; }
		}

		/// <summary>
		/// Gets or sets the helper factory.
		/// </summary>
		/// <value>The helper factory.</value>
		public IHelperFactory HelperFactory
		{
			get { return helperFactory; }
			set { helperFactory = value; }
		}

		/// <summary>
		/// Gets or sets the extension manager.
		/// </summary>
		/// <value>The extension manager.</value>
		public ExtensionManager ExtensionManager
		{
			get { return extensionManager; }
			set { extensionManager = value; }
		}

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetService<T>() where T : class
		{
			return (T) service2Impl[typeof(T)];
		}

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		/// <param name="serviceType">An object that specifies the type of service object to get.</param>
		/// <returns>
		/// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
		/// </returns>
		public object GetService(Type serviceType)
		{
			object value;
			service2Impl.TryGetValue(serviceType, out value);
			return value;
		}
	}
}
