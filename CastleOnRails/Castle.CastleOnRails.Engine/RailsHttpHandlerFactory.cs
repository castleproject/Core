using System;
using Castle.CastleOnRails.Engine.ControllerFactory;
// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Engine
{
	using System.Web;
	using System.Configuration;

	using Castle.CastleOnRails.Engine.Configuration;

	using Castle.CastleOnRails.Framework.Views;
	using Castle.CastleOnRails.Framework.Views.Aspx;

	/// <summary>
	/// Summary description for RailsHttpHandlerFactory.
	/// </summary>
	public class RailsHttpHandlerFactory : IHttpHandlerFactory
	{
		private GeneralConfiguration _config;
		private IControllerFactory _controllerFactory;
		private IViewEngine _viewEngine;

		public RailsHttpHandlerFactory()
		{
			ObtainConfiguration();
			InitializeControllerFactory();
			InitializeViewEngine();
		}

		protected virtual void InitializeControllerFactory()
		{
			if (_config != null && _config.CustomControllerFactory != null)
			{
				String typeName = _config.CustomControllerFactory;
				Type controllerFactoryType = Type.GetType(typeName, false, false);

				if (controllerFactoryType == null)
				{
					String message = 
						String.Format("Could not obtain controller factory. Type not found {0}", typeName);
					throw new RailsException(message);
				}

				_controllerFactory = (IControllerFactory) 
					Activator.CreateInstance(controllerFactoryType);
			}
			else
			{
				ControllersCache cache = new ControllersCache();

				if (_config != null)
				{
					cache.Inspect( _config.ControllersAssembly );
				}
				
				_controllerFactory = new DefaultControllerFactory(cache);
			}
		}

		protected virtual void InitializeViewEngine()
		{
			// Default view engine
			_viewEngine = new AspNetViewEngine();
		}

		protected virtual void ObtainConfiguration()
		{
			_config = (GeneralConfiguration) 
				ConfigurationSettings.GetConfig("rails");
		}

		public virtual IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
		{
			RailsHttpHandler handler = new RailsHttpHandler( 
				_viewEngine, _controllerFactory, 
				requestType, url, pathTranslated);
			
			Configure(handler);

			return handler;
		}

		public virtual void ReleaseHandler(IHttpHandler handler)
		{
		}

		protected virtual void Configure(RailsHttpHandler handler)
		{
			GeneralConfiguration config = (GeneralConfiguration) 
				ConfigurationSettings.GetConfig("rails");

			handler.ControllersAssembly = config.ControllersAssembly;
			handler.ViewsPhysicalPath = config.ViewsPhysicalPath;
		}
	}
}
