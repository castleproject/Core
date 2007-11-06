// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

using Castle.Core.Logging;

namespace Castle.MonoRail.Framework.Routing
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Web;
	using Castle.MonoRail.Framework.Services.Utils;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Pendent
	/// </summary>
	public class RoutingModuleEx : IHttpModule
	{
		private static readonly RoutingEngine engine = new RoutingEngine();

		/// <summary>
		/// Inits the specified app.
		/// </summary>
		/// <param name="app">The app.</param>
		public void Init(HttpApplication app)
		{
			app.BeginRequest += OnBeginRequest;
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

			RouteMatch match = engine.FindMatch(request.Headers["host"], request.ApplicationPath, request.RawUrl);

			if (match != null)
			{
				string mrPath = CreateMrPath(match);
				string url = request.RawUrl;

				string paramsAsQueryString = ConvertToQueryString(match.Parameters, context.Server);

				int queryStringIndex = url.IndexOf('?');

				if (queryStringIndex != -1)
				{
					if (paramsAsQueryString.Length != 0)
					{
						// Concat
						paramsAsQueryString += url.Substring(queryStringIndex + 1);
					}
					else
					{
						paramsAsQueryString = url.Substring(queryStringIndex + 1);
					}
				}

				if (paramsAsQueryString.Length != 0)
				{
					context.RewritePath(mrPath, request.PathInfo, paramsAsQueryString);
				}
				else
				{
					context.RewritePath(mrPath);
				}
			}			
		}

		private static string ConvertToQueryString(Dictionary<string, string> parameters, HttpServerUtility serverUtil)
		{
			StringBuilder sb = new StringBuilder();

			foreach(KeyValuePair<string, string> pair in parameters)
			{
				sb.AppendFormat("{0}={1}&", serverUtil.UrlEncode(pair.Key), serverUtil.UrlEncode(pair.Value));
			}

			return sb.ToString();
		}

		private static string CreateMrPath(RouteMatch match)
		{
			ControllerDescriptor desc = ControllerInspectionUtil.Inspect(match.ControllerType);

			return "~/" + (string.IsNullOrEmpty(desc.Area) ? null : desc.Area + "/") + desc.Name + "/" + match.Action + MonoRailServiceContainer.MonoRailExtension;
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
