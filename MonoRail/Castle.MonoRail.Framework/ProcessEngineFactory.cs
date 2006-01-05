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
	using System.ComponentModel;
	using System.Collections;

	using Castle.Components.Common.EmailSender;
	using Castle.Components.Common.EmailSender.SmtpEmailSender;

	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Views.Aspx;


	public class ProcessEngineFactory
	{
		protected MonoRailConfiguration monoRailConfiguration;
		protected IViewEngine viewEngine;
		protected IFilterFactory filterFactory;
		protected IResourceFactory resourceFactory;
		protected IControllerFactory controllerFactory;
		protected IScaffoldingSupport scaffoldingSupport;
		protected IViewComponentFactory viewCompFactory;
		protected IMonoRailExtension[] extensions;
		protected IEmailSender emailSender;
		protected ControllerDescriptorBuilder controllerDescriptorBuilder;

		public ProcessEngineFactory() : this(null)
		{
			
		}

		public ProcessEngineFactory(MonoRailConfiguration config)
		{
			if (config == null)
			{
				ObtainConfiguration();
			}
			else
			{
				this.monoRailConfiguration = config;
			}
			
			controllerDescriptorBuilder = new ControllerDescriptorBuilder();
			
			InitializeExtensions();
			InitializeControllerFactory();
			InitializeViewComponentFactory();
			InitializeFilterFactory();
			InitializeResourceFactory();
			InitializeViewEngine();
			InitializeScaffoldingSupport();
			InitializeEmailSender();

			ConnectViewComponentFactoryToViewEngine();
		}

		public ProcessEngine Create()
		{
			return new ProcessEngine(controllerFactory, controllerDescriptorBuilder, viewEngine, 
				filterFactory, resourceFactory, scaffoldingSupport, 
				viewCompFactory, extensions, emailSender);
		}

		public MonoRailConfiguration Config
		{
			get { return monoRailConfiguration; }
			set { monoRailConfiguration = value; }
		}

		protected virtual void ObtainConfiguration()
		{
			monoRailConfiguration = MonoRailConfiguration.GetConfig();
		}

		protected virtual void CreateDefaultConfig()
		{
			monoRailConfiguration = new MonoRailConfiguration();
		}

		protected virtual void InitializeViewEngine()
		{
			if (monoRailConfiguration.CustomViewEngineType != null)
			{
				viewEngine = (IViewEngine) 
					Activator.CreateInstance(monoRailConfiguration.CustomViewEngineType);
			}
			else
			{
				// If nothing was specified, we use the default view engine 
				// based on webforms
				viewEngine = new AspNetViewEngine();
			}

			viewEngine.ViewRootDir = monoRailConfiguration.ViewsPhysicalPath;
			viewEngine.XhtmlRendering = monoRailConfiguration.ViewsXhtmlRendering;

			viewEngine.Init();
		}

		protected virtual void InitializeFilterFactory()
		{
			if (monoRailConfiguration.CustomFilterFactoryType != null)
			{
				filterFactory = (IFilterFactory) Activator.CreateInstance(monoRailConfiguration.CustomFilterFactoryType);
			}
			else
			{
				filterFactory = new DefaultFilterFactory();
			}
		}

		protected virtual void InitializeViewComponentFactory()
		{
			if (monoRailConfiguration.CustomViewComponentFactoryType != null)
			{
				viewCompFactory = (IViewComponentFactory) 
					Activator.CreateInstance(monoRailConfiguration.CustomViewComponentFactoryType);
			}
			else
			{
				DefaultViewComponentFactory compFactory = new DefaultViewComponentFactory();

				foreach(String assemblyName in monoRailConfiguration.ComponentsAssemblies)
				{
					compFactory.Inspect(assemblyName);
				}

				viewCompFactory = compFactory;
			}
		}

		protected virtual void InitializeResourceFactory()
		{
			if (monoRailConfiguration.CustomResourceFactoryType != null)
			{
				resourceFactory = (IResourceFactory) 
					Activator.CreateInstance(monoRailConfiguration.CustomResourceFactoryType);
			}
			else
			{
				resourceFactory = new DefaultResourceFactory();
			}
		}
		
		protected virtual void InitializeScaffoldingSupport()
		{
			if (monoRailConfiguration.ScaffoldingType != null)
			{
				scaffoldingSupport = (IScaffoldingSupport) 
					Activator.CreateInstance(monoRailConfiguration.ScaffoldingType);
			}
		}

		protected virtual void InitializeControllerFactory()
		{
			if (monoRailConfiguration.CustomControllerFactoryType != null)
			{
				controllerFactory = (IControllerFactory) 
					Activator.CreateInstance(monoRailConfiguration.CustomControllerFactoryType);
			}
			else
			{
				DefaultControllerFactory factory = new DefaultControllerFactory();

				foreach(String assemblyName in monoRailConfiguration.ControllerAssemblies)
				{
					factory.Inspect(assemblyName);
				}

				controllerFactory = factory;
			}
		}

		protected virtual void InitializeExtensions()
		{
			ArrayList extensionList = new ArrayList();

			foreach(Type extensionType in Config.Extensions)
			{
				IMonoRailExtension extension = 
					Activator.CreateInstance( extensionType ) as IMonoRailExtension;

				extension.Init(monoRailConfiguration);

				extensionList.Add(extension);
			}

			extensions = (IMonoRailExtension[]) 
				extensionList.ToArray( typeof(IMonoRailExtension) );
		}

		protected void InitializeEmailSender()
		{
			// TODO: allow user to customize this

			emailSender = new SmtpSender( monoRailConfiguration.SmtpConfig.Host );

			ISupportInitialize initializer = emailSender as ISupportInitialize;

			if (initializer != null)
			{
				initializer.BeginInit();
				initializer.EndInit();
			}
		}
		
		private void ConnectViewComponentFactoryToViewEngine()
		{
			viewCompFactory.ViewEngine = viewEngine;
			viewEngine.ViewComponentFactory = viewCompFactory;
		}
	}
}
