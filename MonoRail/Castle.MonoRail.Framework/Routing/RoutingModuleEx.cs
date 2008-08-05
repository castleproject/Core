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

namespace Castle.MonoRail.Framework.Routing
{
	using System;
	using System.IO;
	using System.Web;
	using System.Web.Configuration;
	using Castle.MonoRail.Framework.Adapters;

	/// <summary>
	/// Pendent
	/// </summary>
	public class RoutingModuleEx : IHttpModule
	{
		private static readonly RoutingEngine engine = new RoutingEngine();
		private string defaultUrlExtension = ".castle";

		/// <summary>
		/// Inits the specified app.
		/// </summary>
		/// <param name="app">The app.</param>
		public void Init(HttpApplication app)
		{
			app.BeginRequest += OnBeginRequest;

			try
			{
				HttpHandlersSection httpHandlersConfig =
					(HttpHandlersSection) WebConfigurationManager.GetSection("system.web/httpHandlers");

				foreach(HttpHandlerAction handlerAction in httpHandlersConfig.Handlers)
				{
					if (typeof(MonoRailHttpHandlerFactory).IsAssignableFrom(Type.GetType(handlerAction.Type)))
					{
						if (handlerAction.Path.StartsWith("*."))
						{
							defaultUrlExtension = handlerAction.Path.Substring(1);
							break;
						}
						else if (handlerAction.Path == "*")
						{
							defaultUrlExtension = string.Empty;
						}
					}
				}
			}
			catch 
			{
				//just swallow and keep on using .castle as the default.
			}

		}

		/// <summary>
		/// Disposes of the resources (other than memory) 
		/// used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Called when [begin request].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void OnBeginRequest(object sender, EventArgs e)
		{
			HttpContext context = HttpContext.Current;
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;

			if (File.Exists(request.PhysicalPath))
			{
				return; // Possibly requesting a static file, so we skip routing altogether
			}

			RouteMatch match =
				engine.FindMatch(StripAppPathFrom(request.FilePath, request.ApplicationPath) + request.PathInfo, 
					new RouteContext(new RequestAdapter(request), response, request.ApplicationPath, context.Items));

			if (match == null || response.StatusCode == 301 || response.StatusCode == 302)
			{
				return;
			}

			string mrPath = CreateMonoRailPath(match);
			string url = request.RawUrl;

			string paramsAsQueryString = "";

			int queryStringIndex = url.IndexOf('?');

			if (queryStringIndex != -1)
			{
				paramsAsQueryString = url.Substring(queryStringIndex + 1);
			}

			if (paramsAsQueryString.Length != 0)
			{
				context.RewritePath(mrPath, request.PathInfo, paramsAsQueryString);
			}
			else
			{
				context.RewritePath(mrPath);
			}

			context.Items.Add(RouteMatch.RouteMatchKey, match);
		}

		private string StripAppPathFrom(string path, string applicationPath)
		{
			if (applicationPath.Length != 1)
			{
				return path.Substring(applicationPath.Length);
			}
			return path;
		}

		private string CreateMonoRailPath(RouteMatch match)
		{
			if (!match.Parameters.ContainsKey("controller"))
			{
				throw new MonoRailException(
					"Parameter 'controller' is not optional. Check your routing rules to make " + 
					"sure this parameter is being added to the RouteMatch");
			}
			if (!match.Parameters.ContainsKey("action"))
			{
				throw new MonoRailException(
					"Parameter 'action' is not optional. Check your routing rules to make " + 
					"sure this parameter is being added to the RouteMatch");
			}

			string controller = match.Parameters["controller"];
			string area = match.Parameters.ContainsKey("area") ? match.Parameters["area"] : null;
			string action = match.Parameters["action"];

			if (area != null)
			{
				return "~/" + area + "/" + controller + "/" + action + defaultUrlExtension;
			}
			else
			{
				return "~/" + controller + "/" + action + defaultUrlExtension;
			}
		}

		/// <summary>
		/// Gets the engine.
		/// </summary>
		/// <value>The engine.</value>
		public static RoutingEngine Engine
		{
			get { return engine; }
		}
	}
}
