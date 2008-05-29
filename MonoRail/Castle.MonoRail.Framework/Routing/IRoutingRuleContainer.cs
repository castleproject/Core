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
	/// 
	/// </summary>
	public delegate void RouteAction(IRouteContext context, RouteMatch match);

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IRoutingRuleContainer
	{
		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="rule">The rule.</param>
		void Add(IRoutingRule rule);

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="rule">The rule.</param>
		void AddFirst(IRoutingRule rule);

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="rule">The rule.</param>
		/// <param name="action">The action executed if the rule matches.</param>
		void Add(IRoutingRule rule, RouteAction action);

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="rule">The rule.</param>
		/// <param name="action">The action executed if the rule matches.</param>
		void AddFirst(IRoutingRule rule, RouteAction action);

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="routeName">Name of the route.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		string CreateUrl(string routeName, IDictionary parameters);

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		string CreateUrl(IDictionary parameters);

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="context">The routing context.</param>
		/// <returns></returns>
		RouteMatch FindMatch(string url, IRouteContext context);

		/// <summary>
		/// Gets a value indicating whether this container is empty.
		/// </summary>
		/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
		bool IsEmpty { get; }
	}
}
