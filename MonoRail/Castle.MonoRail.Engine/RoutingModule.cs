using System.Collections;
using Castle.MonoRail.Engine.Configuration;
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

	/// <summary>
	/// 
	/// </summary>
	public class RoutingModule : IHttpModule
	{
		private IList routingRules;

		public RoutingModule()
		{
		}

		public void Init(HttpApplication context)
		{
			// Subcribe to BeginRequest

			context.BeginRequest += new EventHandler(OnBeginRequest);

			// Load the rules

			routingRules = MonoRailConfiguration.GetConfig().RoutingRules;
		}

		public void Dispose()
		{
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			if (routingRules.Count == 0) return;

			HttpRequest request = HttpContext.Current.Request;

			String newPath;

			if(FindMatchAndReplace(request.FilePath, out newPath))
			{
				int qsIndex = newPath.IndexOf('?');

				if (qsIndex == -1)
				{
					HttpContext.Current.RewritePath(newPath);
				}
				else
				{
					String path = newPath.Substring(0, qsIndex);
					String qs = newPath.Substring(qsIndex + 1);
					HttpContext.Current.RewritePath(path, request.PathInfo, qs);
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

					// Append the query string
					string queryString = HttpContext.Current.Request.Url.Query;

					if(queryString.Length > 0)
					{
						bool hasParams = (newPath.LastIndexOf("?") != -1);

						if(hasParams)
						{
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
