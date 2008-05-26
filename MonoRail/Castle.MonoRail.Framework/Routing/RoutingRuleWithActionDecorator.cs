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
	using System.Collections;

	/// <summary>
	/// Pendent
	/// </summary>
	public class RoutingRuleWithActionDecorator : IRoutingRule
	{
		private readonly IRoutingRule inner;
		private readonly RouteAction action;

		/// <summary>
		/// Initializes a new instance of the <see cref="RoutingRuleWithActionDecorator"/> class.
		/// </summary>
		/// <param name="inner">The real routing rule.</param>
		/// <param name="action">The action.</param>
		public RoutingRuleWithActionDecorator(IRoutingRule inner, RouteAction action)
		{
			this.inner = inner;
			this.action = action;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="hostname">The hostname.</param>
		/// <param name="virtualPath">The virtual path.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string CreateUrl(string hostname, string virtualPath, IDictionary parameters)
		{
			return inner.CreateUrl(hostname, virtualPath, parameters);
		}

		/// <summary>
		/// Determines if the specified URL matches the
		/// routing rule.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="context">The context</param>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		public int Matches(string url, IRouteContext context, RouteMatch match)
		{
			return inner.Matches(url, context, match);
//			bool matches = inner.Matches(url, context, match);
//
//			if (matches)
//			{
//				action(context, match);
//			}
//
//			return matches;
		}

		/// <summary>
		/// Gets the name of the route.
		/// </summary>
		/// <value>The name of the route.</value>
		public string RouteName
		{
			get { return inner.RouteName; }
		}
	}
}
