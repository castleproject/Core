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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Web;
	using Castle.MonoRail.Framework.Configuration;

	/// <summary>
	/// Provides routing basic services in response to rules defined in 
	/// <see cref="MonoRailConfiguration.RoutingRules"/>.
	/// <remarks>
	/// This class delegates the resolving of the path that will be evaluated
	/// to derivided classes.
	/// </remarks>
	/// </summary>
	public class RoutingModule : IHttpModule
	{
		internal static readonly String OriginalPathKey = "rails.original_path";
		private IList routingRules;

		/// <summary>
		/// Initializes a module and prepares it to handle requests.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpApplication"></see> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(OnBeginRequest);

			routingRules = MonoRailConfiguration.GetConfig().RoutingRules;
		}

		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"></see>.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Called when [begin request].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnBeginRequest(object sender, EventArgs e)
		{
			if (routingRules.Count == 0) return;

			HttpContext context = HttpContext.Current;
			HttpRequest request = context.Request;

			String newPath;

			String sourcePath = GetSourcePath();

			SaveOriginalPath(context, sourcePath);

			if (FindMatchAndReplace(sourcePath, out newPath))
			{
				// Handle things differently depending on wheter we need 
				// to keep a query string or not

				int queryStringIndex = newPath.IndexOf('?');

				if (queryStringIndex == -1)
				{
					context.RewritePath(newPath);
				}
				else
				{
					String path = newPath.Substring(0, queryStringIndex);
					String queryString = newPath.Substring(queryStringIndex + 1);
					context.RewritePath(path, request.PathInfo, queryString);
				}
			}
		}

		/// <summary>
		/// Gets the source path.
		/// </summary>
		/// <returns></returns>
		protected string GetSourcePath()
		{
			if (ShouldUseHostAndPath())
			{
				return GetHostNameAndPath();
			}
			else
			{
				return GetPath();
			}
		}

		private bool ShouldUseHostAndPath()
		{
			return MonoRailConfiguration.GetConfig().MatchHostNameAndPath;
		}

		private string GetHostNameAndPath()
		{
			HttpContext context = HttpContext.Current;
			HttpRequest request = context.Request;

			String host = request.Headers["host"];

			if (String.IsNullOrEmpty(host))
			{
				return request.FilePath;
			}
			else
			{
				return host + request.FilePath;
			}
		}

		private static string GetPath()
		{
			return HttpContext.Current.Request.FilePath;
		}

		private bool FindMatchAndReplace(String currentPath, out String newPath)
		{
			newPath = String.Empty;

			foreach(RoutingRule rule in routingRules)
			{
				if (rule.CompiledRule.IsMatch(currentPath))
				{
					newPath = rule.CompiledRule.Replace(currentPath, rule.Replace);

					//Append the query string
					String queryString = HttpContext.Current.Request.Url.Query;

					if (queryString.Length > 0)
					{
						//If we already have some query string params on the new path...
						bool hasParams = (newPath.LastIndexOf("?") != -1);

						if (hasParams)
						{
							//...make sure we append the query string nicely rather than adding another ?
							queryString = queryString.Replace("?", "&");
						}

						newPath += queryString;
					}

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns the original path 
		/// (before rewriting occured), or <c>null</c> 
		/// if rewriting didn't occur on this request.
		/// </summary>
		public static String OriginalPath
		{
			get
			{
				HttpContext context = HttpContext.Current;

				if (context.Items.Contains(OriginalPathKey))
				{
					return context.Items[OriginalPathKey] as String;
				}

				return null;
			}
		}

		private static void SaveOriginalPath(HttpContext context, String virtualPath)
		{
			context.Items.Add(OriginalPathKey, virtualPath);
		}
	}
}