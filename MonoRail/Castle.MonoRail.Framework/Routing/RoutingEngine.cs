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

namespace Castle.MonoRail.Framework.Routing
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Pendent
	/// </summary>
	public class RoutingEngine : IRoutingEngine
	{
		private readonly IList rules = ArrayList.Synchronized(new ArrayList());
		private readonly Dictionary<string,IRoutingRule> name2Rule = new Dictionary<string,IRoutingRule>();

		/// <summary>
		/// Adds the specified rule.
		/// </summary>
		/// <param name="rule">The rule.</param>
		public void Add(IRoutingRule rule)
		{
			rules.Add(rule);

			// For really fast access
			name2Rule[rule.RouteName] = rule;
		}

		/// <summary>
		/// Creates the URL.
		/// </summary>
		/// <param name="routeName">Name of the route.</param>
		/// <param name="hostname">The hostname.</param>
		/// <param name="virtualPath">The virtual path.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string CreateUrl(string routeName, string hostname, string virtualPath, IDictionary parameters)
		{
			IRoutingRule rule;

			if (!name2Rule.TryGetValue(routeName, out rule))
			{
				throw new RailsException("Could not find named route: " + routeName);
			}

			return rule.CreateUrl(hostname, virtualPath, parameters);
		}

		/// <summary>
		/// Finds the match.
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="virtualPath"></param>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		public RouteMatch FindMatch(string hostname, string virtualPath, string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentNullException("url", "url cannot be empty nor null");
			}

			if (url[url.Length - 1] == '/')
			{
				url = url.Substring(0, url.Length - 1);
			}

			if (url[0] == '/')
			{
				url = url.Substring(1);
			}

			foreach(IRoutingRule rule in rules)
			{
				RouteMatch match = new RouteMatch(rule.ControllerType, rule.RouteName, rule.Action);

				if (rule.Matches(hostname, virtualPath, url, match))
				{
					return match;
				}
			}

			return null;
		}
	}
}
