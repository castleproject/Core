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
	using System.ComponentModel;
	using System.Configuration;
	using System.Web;

	using Castle.Core;
	using Castle.MonoRail.Framework.Adapters;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Views.Aspx;

	/// <summary>
	/// Provides the services used and shared by the framework. Also 
	/// is in charge of creating an implementation of <see cref="IRailsEngineContext"/>
	/// upon the start of a new request.
	/// </summary>
	public class EngineContextModule : AbstractServiceContainer, IHttpModule 
	{
		internal static readonly String RailsContextKey = "rails.context";
		
		private static bool initialized;
		
		/// <summary>The only one Extension Manager</summary>
		private ExtensionManager extensionManager;
		
		/// <summary>Prevents GC to collect the extensions</summary>
		private IList extensions = new ArrayList();

		public void Init(HttpApplication context)
		{
			InitConfiguration();
			
			MonoRailConfiguration config = ObtainConfiguration();

			InitExtensions(config);
			InitServices(config);
			InitApplicationHooks(context);

			initialized = true;
		}

		public void Dispose()
		{
			extensions.Clear();
		}
		
		/// <summary>
		/// Reads the configuration and initializes
		/// registered extensions.
		/// </summary>
		/// <param name="config">The configuration object</param>
		private void InitExtensions(MonoRailConfiguration config)
		{
			extensionManager = new ExtensionManager(this);
			
			foreach(ExtensionEntry entry in config.ExtensionEntries)
			{
				AssertImplementsService(typeof(IMonoRailExtension), entry.ExtensionType);
				
				IMonoRailExtension extension = (IMonoRailExtension) ActivateService(entry.ExtensionType);
				
				extension.SetExtensionConfigNode(entry.ExtensionNode);
				
				extensions.Add(extension);
			}
		}

		/// <summary>
		/// Coordinates the instantiation, registering and initialization (lifecycle-wise)
		/// of the services used by MonoRail.
		/// </summary>
		/// <param name="config">The configuration object</param>
		private void InitServices(MonoRailConfiguration config)
		{
			AddService(typeof(ExtensionManager), extensionManager);
			AddService(typeof(MonoRailConfiguration), config);
			
			IList services = InstantiateAndRegisterServices(config.ServiceEntries);
			
			LifecycleService(services);
			LifecycleService(extensions);

			LifecycleInitialize(services);
			LifecycleInitialize(extensions);
		}

		/// <summary>
		/// Checks for services that implements <see cref="IInitializable"/>
		/// or <see cref="ISupportInitialize"/> and initialize them through the interface
		/// </summary>
		/// <param name="services">List of MonoRail's services</param>
		private void LifecycleInitialize(IList services)
		{
			foreach(object instance in services)
			{
				IInitializable initializable = instance as IInitializable;
				
				if (initializable != null)
				{
					initializable.Initialize();
				}
				
				ISupportInitialize suppInitialize = instance as ISupportInitialize;
				
				if (suppInitialize != null)
				{
					suppInitialize.BeginInit();
					suppInitialize.EndInit();
				}
			}
		}

		/// <summary>
		/// Checks for services that implements <see cref="IServiceEnabledComponent"/>
		/// and invoke <see cref="IServiceEnabledComponent.Service"/> on them
		/// </summary>
		/// <param name="services">List of MonoRail's services</param>
		private void LifecycleService(IList services)
		{
			foreach(object instance in services)
			{
				IServiceEnabledComponent serviceEnabled = instance as IServiceEnabledComponent;
				
				if (serviceEnabled != null)
				{
					serviceEnabled.Service(this);
				}
			}
		}

		/// <summary>
		/// Instantiates and registers the services used by MonoRail.
		/// </summary>
		/// <param name="services">The service's registry</param>
		/// <returns>List of service's instances</returns>
		private IList InstantiateAndRegisterServices(ServiceEntryCollection services)
		{
			IList instances = new ArrayList();
			
			// Builtin services
			
			foreach(DictionaryEntry entry in services.ServiceImplMap)
			{
				Type service = (Type) entry.Key;
				Type impl = (Type) entry.Value;
				
				AssertImplementsService(service, impl);
				
				object instance = ActivateService(impl);
				
				AddService(service, instance);
				
				instances.Add(instance);
			}

			// Custom services
			
			foreach(Type type in services.CustomServices)
			{
				object instance = ActivateService(type);
				
				AddService(type, instance);
				
				instances.Add(instance);
			}

			return instances;
		}

		/// <summary>
		/// Registers to <c>HttpApplication</c> events
		/// </summary>
		/// <param name="context">The application instance</param>
		private void InitApplicationHooks(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(OnBeginRequest);
			context.EndRequest += new EventHandler(OnEndRequest);
			context.AcquireRequestState += new EventHandler(OnAcquireRequestState);
			context.ReleaseRequestState += new EventHandler(OnReleaseRequestState);
			context.PreRequestHandlerExecute += new EventHandler(OnPreRequestHandlerExecute);
			context.PostRequestHandlerExecute += new EventHandler(OnPostRequestHandlerExecute);
			context.Error += new EventHandler(OnError);		    
		}

		/// <summary>
		/// Registers the default implementation of services, if 
		/// they are not registered
		/// </summary>
		private void InitConfiguration()
		{
			MonoRailConfiguration config = MonoRailConfiguration.GetConfig();

			RegisterMissingServices(config);
		}

		/// <summary>
		/// Checks whether non-optional services were supplied 
		/// through the configuration, and if not, register the 
		/// default implementation.
		/// </summary>
		/// <param name="config">The configuration object</param>
		private void RegisterMissingServices(MonoRailConfiguration config)
		{
			ServiceEntryCollection services = config.ServiceEntries;

			if (!services.HasService(ServiceIdentification.ViewSourceLoader))
			{
				services.RegisterService(ServiceIdentification.ViewSourceLoader, 
				                         typeof(FileAssemblyViewSourceLoader));
			}
			if (!services.HasService(ServiceIdentification.ViewEngine))
			{
				Type viewEngineType = config.ViewEngineConfig.CustomEngine;
				
				if (viewEngineType == null)
				{
					viewEngineType = typeof(WebFormsViewEngine);
				}
				
				services.RegisterService(ServiceIdentification.ViewEngine, viewEngineType);
			}
			if (!services.HasService(ServiceIdentification.ScaffoldingSupport))
			{
				Type defaultScaffoldingType =
					TypeLoadUtil.GetType(
						TypeLoadUtil.GetEffectiveTypeName(
							"Castle.MonoRail.ActiveRecordScaffold.ScaffoldingSupport, Castle.MonoRail.ActiveRecordScaffold"), true);
				
				if (defaultScaffoldingType != null)
				{
					services.RegisterService(ServiceIdentification.ScaffoldingSupport, defaultScaffoldingType);
				}
			}
			if (!services.HasService(ServiceIdentification.ControllerFactory))
			{
				if (config.ControllersConfig.CustomControllerFactory != null)
				{
					services.RegisterService(ServiceIdentification.ControllerFactory, 
					                         config.ControllersConfig.CustomControllerFactory);
				}
				else
				{
					services.RegisterService(ServiceIdentification.ControllerFactory, 
					                         typeof(DefaultControllerFactory));
				}
			}
			if (!services.HasService(ServiceIdentification.ViewComponentFactory))
			{
				if (config.ViewComponentsConfig.CustomViewComponentFactory != null)
				{
					services.RegisterService(ServiceIdentification.ViewComponentFactory, 
					                         config.ViewComponentsConfig.CustomViewComponentFactory);
				}
				else
				{
					services.RegisterService(ServiceIdentification.ViewComponentFactory, 
					                         typeof(DefaultViewComponentFactory));
				}
			}
			if (!services.HasService(ServiceIdentification.FilterFactory))
			{
				if (config.CustomFilterFactory != null)
				{
					services.RegisterService(ServiceIdentification.FilterFactory, 
					                         config.CustomFilterFactory);
				}
				else
				{
					services.RegisterService(ServiceIdentification.FilterFactory, 
					                         typeof(DefaultFilterFactory));
				}
			}
			if (!services.HasService(ServiceIdentification.ResourceFactory))
			{
				services.RegisterService(ServiceIdentification.ResourceFactory, typeof(DefaultResourceFactory));
			}
			if (!services.HasService(ServiceIdentification.EmailSender))
			{
				services.RegisterService(ServiceIdentification.EmailSender, typeof(MonoRailSmtpSender));
			}
			if (!services.HasService(ServiceIdentification.ControllerDescriptorProvider))
			{
				services.RegisterService(ServiceIdentification.ControllerDescriptorProvider, typeof(DefaultControllerDescriptorProvider));
			}
			if (!services.HasService(ServiceIdentification.ResourceDescriptorProvider))
			{
				services.RegisterService(ServiceIdentification.ResourceDescriptorProvider, typeof(DefaultResourceDescriptorProvider));
			}
			if (!services.HasService(ServiceIdentification.RescueDescriptorProvider))
			{
				services.RegisterService(ServiceIdentification.RescueDescriptorProvider, typeof(DefaultRescueDescriptorProvider));
			}
			if (!services.HasService(ServiceIdentification.LayoutDescriptorProvider))
			{
				services.RegisterService(ServiceIdentification.LayoutDescriptorProvider, typeof(DefaultLayoutDescriptorProvider));
			}
			if (!services.HasService(ServiceIdentification.HelperDescriptorProvider))
			{
				services.RegisterService(ServiceIdentification.HelperDescriptorProvider, typeof(DefaultHelperDescriptorProvider));
			}
			if (!services.HasService(ServiceIdentification.FilterDescriptorProvider))
			{
				services.RegisterService(ServiceIdentification.FilterDescriptorProvider, typeof(DefaultFilterDescriptorProvider));
			}
			if (!services.HasService(ServiceIdentification.EmailTemplateService))
			{
				services.RegisterService(ServiceIdentification.EmailTemplateService, typeof(EmailTemplateService));
			}
			if (!services.HasService(ServiceIdentification.ControllerTree))
			{
				services.RegisterService(ServiceIdentification.ControllerTree, typeof(DefaultControllerTree));
			}
			if (!services.HasService(ServiceIdentification.CacheProvider))
			{
				services.RegisterService(ServiceIdentification.CacheProvider, typeof(DefaultCacheProvider));
			}
		}

		private MonoRailConfiguration ObtainConfiguration()
		{
			return MonoRailConfiguration.GetConfig();
		}

		#region Hooks dispatched to extensions

		private void OnBeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication) sender;
			HttpContext context = app.Context;

			IRailsEngineContext mrContext = CreateRailsEngineContext(context);

			extensionManager.RaiseContextCreated(mrContext);
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			extensionManager.RaiseContextDisposed(mrContext);
		}

		private void OnAcquireRequestState(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			extensionManager.RaiseAcquireRequestState(mrContext);
		}

		private void OnReleaseRequestState(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			extensionManager.RaiseReleaseRequestState(mrContext);
		}

		private void OnPreRequestHandlerExecute(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			extensionManager.RaisePreProcess(mrContext);
		}

		private void OnPostRequestHandlerExecute(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			extensionManager.RaisePostProcess(mrContext);
		}

		private void OnError(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			mrContext.LastException = mrContext.UnderlyingContext.Server.GetLastError();

			extensionManager.RaiseUnhandledError(mrContext);
		}

		private static IRailsEngineContext ObtainContextFromApplication(object sender)
		{
			HttpApplication app = (HttpApplication) sender;
			HttpContext context = app.Context;

			return ObtainRailsEngineContext(context);
		}

		#endregion
		
		private IRailsEngineContext CreateRailsEngineContext(HttpContext context)
		{
			IRailsEngineContext mrContext = ObtainRailsEngineContext(context);

			if (mrContext == null)
			{
				mrContext = new DefaultRailsEngineContext(this, context);

				context.Items[RailsContextKey] = mrContext;
			}

			return mrContext;
		}
		
		private object ActivateService(Type type)
		{
			try
			{
				return Activator.CreateInstance(type);
			}
			catch(Exception ex)
			{
				throw new ConfigurationException(String.Format("Initialization Exception: " + 
					"Could not instantiate {0}", type.FullName), ex);
			}
		}

		private void AssertImplementsService(Type service, Type impl)
		{
			if (!service.IsAssignableFrom(impl))
			{
				throw new ConfigurationException(String.Format("Initialization Exception: " + 
					"Service {0} does not implement or extend {1}", impl.FullName, service.FullName));
			}
		}

		internal static bool Initialized
		{
			get { return initialized; }
		}

		internal static IRailsEngineContext ObtainRailsEngineContext(HttpContext context)
		{
			return (IRailsEngineContext) context.Items[RailsContextKey];
		}
	}
}
