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

namespace Castle.MonoRail.Engine
{
	using System;
	using System.Web;
	using System.Configuration;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Views.Aspx;

	using Castle.MonoRail.Engine.Configuration;

	/// <summary>
	/// Coordinates the creation of new <see cref="RailsHttpHandler"/> 
	/// and uses the configuration to obtain the correct factories 
	/// instances.
	/// </summary>
	public class RailsHttpHandlerFactory : IHttpHandlerFactory
	{
		private RailsConfiguration _config;
		private IViewEngine _viewEngine;
		private IFilterFactory _filterFactory;
		private IControllerFactory _controllerFactory;

		public RailsHttpHandlerFactory()
		{
			ObtainConfiguration();
			InitializeControllerFactory();
			InitializeViewEngine();
			InitializeFilterFactory();
		}

		#region IHttpHandlerFactory

		public virtual IHttpHandler GetHandler(HttpContext context, 
			string requestType, String url, String pathTranslated)
		{
			return new RailsHttpHandler(url, _viewEngine, _controllerFactory, _filterFactory);
		}

		public virtual void ReleaseHandler(IHttpHandler handler)
		{
		}

		#endregion

		protected virtual void ObtainConfiguration()
		{
			_config = (RailsConfiguration) ConfigurationSettings.GetConfig("rails");

			if (_config == null)
			{
				throw new ApplicationException("Unfortunatelly you have to provide " + 
					"the configuration to use MonoRail");
			}
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
				_viewEngine = new AspNetViewEngine();
			}

			_viewEngine.ViewRootDir = _config.ViewsPhysicalPath;

			_viewEngine.Init();
		}

		protected virtual void InitializeFilterFactory()
		{
			if (_config.CustomFilterFactoryType != null)
			{
				_filterFactory = (IFilterFactory) 
					Activator.CreateInstance(_config.CustomFilterFactoryType);
			}
			else
			{
				_filterFactory = new DefaultFilterFactory();
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

				foreach(String assemblyName in _config.Assemblies)
				{
					factory.Inspect(assemblyName);
				}

				_controllerFactory = factory;
			}
		}
	}
}
