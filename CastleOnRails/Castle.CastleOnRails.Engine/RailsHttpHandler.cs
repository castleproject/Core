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
	using System;
	using System.Web;

	using Castle.CastleOnRails.Framework;
	using Castle.CastleOnRails.Framework.Views;

	/// <summary>
	/// Summary description for RailsHttpHandler.
	/// </summary>
	public class RailsHttpHandler : IHttpHandler
	{
		private String _requestType; 
		private String _url; 
		private String _pathTranslated;
		private String _controllersAssembly;
		private String _viewsPhysicalPath;
		private IControllerFactory _controllerFactory;
		private IViewEngine _viewEngine;

		public RailsHttpHandler( IViewEngine viewEngine, IControllerFactory controllerFactory, 
			String requestType, String url, String pathTranslated)
		{
			_viewEngine = viewEngine;
			_controllerFactory = controllerFactory;
			_requestType = requestType;
			_url = url;
			_pathTranslated = pathTranslated;
		}

		public String ControllersAssembly
		{
			get { return _controllersAssembly; }
			set { _controllersAssembly = value; }
		}

		public String ViewsPhysicalPath
		{
			get { return _viewsPhysicalPath; }
			set { _viewsPhysicalPath = value; }
		}

		public void ProcessRequest(HttpContext context)
		{
			UrlInfo info = UrlTokenizer.ExtractInfo(_url);

			Controller controller = _controllerFactory.GetController( info.Controller );

			try
			{
				controller.__Process( _url, 
					ViewsPhysicalPath, info.Controller, 
					info.Action, _viewEngine, context );
			}
			finally
			{
				_controllerFactory.Release(controller);
			}
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}
