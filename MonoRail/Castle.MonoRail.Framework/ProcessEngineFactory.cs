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
	using System.ComponentModel;
	using System.Collections;

	using Castle.Components.Common.EmailSender;
	using Castle.Components.Common.EmailSender.SmtpEmailSender;

	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Views.Aspx;


	public class ProcessEngineFactory
	{
		protected MonoRailConfiguration _config;
		protected IViewEngine _viewEngine;
		protected IFilterFactory _filterFactory;
		protected IResourceFactory _resourceFactory;
		protected IControllerFactory _controllerFactory;
		protected IScaffoldingSupport _scaffoldingSupport;
		protected IViewComponentFactory _viewCompFactory;
		protected IMonoRailExtension[] extensions;
		protected IEmailSender emailSender;

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
				_config = config;
			}

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
			return new ProcessEngine(_controllerFactory, _viewEngine, 
				_filterFactory, _resourceFactory, _scaffoldingSupport, 
				_viewCompFactory, extensions, emailSender);
		}

		public MonoRailConfiguration Config
		{
			get { return _config; }
			set { _config = value; }
		}

		protected virtual void ObtainConfiguration()
		{
			_config = MonoRailConfiguration.GetConfig();
		}

		protected virtual void CreateDefaultConfig()
		{
			_config = new MonoRailConfiguration();
		}

		protected virtual void InitializeViewEngine()
		{
			if (_config.CustomViewEngineType != null)
			{
				_viewEngine = (IViewEngine) 
					Activator.CreateInstance(_config.CustomViewEngineType);
			}
			else
			{
				// If nothing was specified, we use the default view engine 
				// based on webforms
				_viewEngine = new AspNetViewEngine();
			}

			_viewEngine.ViewRootDir = _config.ViewsPhysicalPath;
			_viewEngine.XhtmlRendering = _config.ViewsXhtmlRendering;

			_viewEngine.Init();
		}

		protected virtual void InitializeFilterFactory()
		{
			if (_config.CustomFilterFactoryType != null)
			{
				_filterFactory = (IFilterFactory) Activator.CreateInstance(_config.CustomFilterFactoryType);
			}
			else
			{
				_filterFactory = new DefaultFilterFactory();
			}
		}

		protected virtual void InitializeViewComponentFactory()
		{
			if (_config.CustomViewComponentFactoryType != null)
			{
				_viewCompFactory = (IViewComponentFactory) 
					Activator.CreateInstance(_config.CustomViewComponentFactoryType);
			}
			else
			{
				DefaultViewComponentFactory compFactory = new DefaultViewComponentFactory();

				foreach(String assemblyName in _config.ComponentsAssemblies)
				{
					compFactory.Inspect(assemblyName);
				}

				_viewCompFactory = compFactory;
			}
		}

		protected virtual void InitializeResourceFactory()
		{
			if (_config.CustomResourceFactoryType != null)
			{
				_resourceFactory = (IResourceFactory) 
					Activator.CreateInstance(_config.CustomResourceFactoryType);
			}
			else
			{
				_resourceFactory = new DefaultResourceFactory();
			}
		}
		
		protected virtual void InitializeScaffoldingSupport()
		{
			if (_config.ScaffoldingType != null)
			{
				_scaffoldingSupport = (IScaffoldingSupport) 
					Activator.CreateInstance(_config.ScaffoldingType);
			}
		}

		protected virtual void InitializeControllerFactory()
		{
			if (_config.CustomControllerFactoryType != null)
			{
				_controllerFactory = (IControllerFactory) 
					Activator.CreateInstance(_config.CustomControllerFactoryType);
			}
			else
			{
				DefaultControllerFactory factory = new DefaultControllerFactory();

				foreach(String assemblyName in _config.ControllerAssemblies)
				{
					factory.Inspect(assemblyName);
				}

				_controllerFactory = factory;
			}
		}

		protected virtual void InitializeExtensions()
		{
			ArrayList extensionList = new ArrayList();

			foreach(Type extensionType in Config.Extensions)
			{
				IMonoRailExtension extension = 
					Activator.CreateInstance( extensionType ) as IMonoRailExtension;

				extension.Init(_config);

				extensionList.Add(extension);
			}

			extensions = (IMonoRailExtension[]) 
				extensionList.ToArray( typeof(IMonoRailExtension) );
		}

		protected void InitializeEmailSender()
		{
			// TODO: allow user to customize this

			emailSender = new SmtpSender( _config.SmtpConfig.Host );

			ISupportInitialize initializer = emailSender as ISupportInitialize;

			if (initializer != null)
			{
				initializer.BeginInit();
				initializer.EndInit();
			}
		}
		
		private void ConnectViewComponentFactoryToViewEngine()
		{
			_viewCompFactory.ViewEngine = _viewEngine;
			_viewEngine.ViewComponentFactory = _viewCompFactory;
		}
	}
}
