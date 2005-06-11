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
	using System.Collections;
	using System.Web;

	using Castle.MonoRail.Engine.Configuration;

	/// <summary>
	/// Provides routing services in response to rules defined in <see cref="MonoRailConfiguration.RoutingRules"/>.
	/// </summary>
	public class RoutingModule : IHttpModule
	{
		internal static readonly String OriginalPathKey = "rails.original_path";

		private IList routingRules;

		public RoutingModule()
		{
		}

		public void Init(HttpApplication context)
		{
			//Subcribe to BeginRequest
			context.BeginRequest += new EventHandler(OnBeginRequest);

			//Load the rules
			routingRules = MonoRailConfiguration.GetConfig().RoutingRules;
		}

		public void Dispose()
		{
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			if (routingRules.Count == 0) return;

			HttpContext context = HttpContext.Current;
			HttpRequest request = context.Request;

			string virtualPath = request.FilePath;
			string newPath;

			//Store the original path in case this needs to be used later
			context.Items.Add(OriginalPathKey, virtualPath);

			if(FindMatchAndReplace(virtualPath, out newPath))
			{
				//Handle things differently depending on wheter we need to keep a query string or not
				int queryStringIndex = newPath.IndexOf('?');
				if (queryStringIndex == -1)
				{
					context.RewritePath(newPath);
				}
				else
				{
					string path = newPath.Substring(0, queryStringIndex);
					string queryString = newPath.Substring(queryStringIndex + 1);
					context.RewritePath(path, request.PathInfo, queryString);
				}
			}
		}

		private bool FindMatchAndReplace(string currentPath, out string newPath)
		{
			newPath = String.Empty;

			foreach(RoutingRule rule in routingRules)
			{
				if (rule.CompiledRule.IsMatch(currentPath))
				{
					newPath = rule.CompiledRule.Replace(currentPath, rule.Replace);

					//Append the query string
					string queryString = HttpContext.Current.Request.Url.Query;
					if(queryString.Length > 0)
					{
						//If we already have some query string params on the new path...
						bool hasParams = (newPath.LastIndexOf("?") != -1);
						if(hasParams)
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
	}
}
