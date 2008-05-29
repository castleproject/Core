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
	/// Depicts an url routing rule contract.
	/// </summary>
	/// <remarks>
	/// Implementors can use this interface to implement custom rules. 
	/// </remarks>
	public interface IRoutingRule
	{
		/// <summary>
		/// Gets the name of the route.
		/// </summary>
		/// <value>The name of the route.</value>
		string RouteName { get; }

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		string CreateUrl(IDictionary parameters);

		/// <summary>
		/// Determines if the specified URL matches the
		/// routing rule.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="context">The context</param>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		int Matches(string url, IRouteContext context, RouteMatch match);
	}
}
