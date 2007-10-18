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

	/// <summary>
	/// Pendent
	/// </summary>
	public class RoutingEngine
	{
		private readonly IList rules = ArrayList.Synchronized(new ArrayList());

		/// <summary>
		/// Finds the match.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		public RouteMatch FindMatch(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentNullException("url", "url cannot be empty nor null");
			}

			if (url[0] == '/')
			{
				url = url.Substring(1);
			}

			foreach (IRoutingRule rule in rules)
			{
				RouteMatch match = new RouteMatch(rule.ControllerType, rule.RuleName, rule.Action);

				if (rule.Matches(url, match))
				{
					return match;
				}
			}

			return null;
		}

		/// <summary>
		/// Adds the specified rule.
		/// </summary>
		/// <param name="rule">The rule.</param>
		public void Add(IRoutingRule rule)
		{
			rules.Add(rule);
		}
	}
}
