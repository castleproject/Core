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

	using Castle.CastleOnRails.Engine.Adapters;

	using Castle.CastleOnRails.Framework;

	/// <summary>
	/// Summary description for RailsHttpHandler.
	/// </summary>
	public class RailsHttpHandler : ProcessEngine, IHttpHandler
	{
//		private HttpContext _context;
		private String _url; 
//		private String _pathTranslated;
		private String _requestType;

		public RailsHttpHandler( IViewEngine viewEngine, IControllerFactory controllerFactory, 
			String requestType, String url) : 
			base(controllerFactory, viewEngine)
		{
			_url = url;
			_requestType = requestType;
		}

		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			IRailsEngineContext railsContext = new RailsEngineContextAdapter(context, _url, _requestType);

			base.Process(railsContext);
		}

		public bool IsReusable
		{
			get { return false; }
		}

		#endregion
	}
}
