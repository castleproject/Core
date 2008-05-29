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
	using System.Collections;

	/// <summary>
	/// Pendent
	/// </summary>
	public class RedirectRoute : IRoutingRule
	{
		private readonly string routeName, urlToMatch, redirectUrl;

		/// <summary>
		/// Initializes a new instance of the <see cref="RedirectRoute"/> class.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="redirectUrl">The redirect URL.</param>
		public RedirectRoute(string url, string redirectUrl)
		{
			routeName = null;
			this.urlToMatch = url;
			this.redirectUrl = redirectUrl;
		}

		/// <summary>
		/// Gets the name of the route.
		/// </summary>
		/// <value>The name of the route.</value>
		public string RouteName
		{
			get { return routeName; }
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string CreateUrl(IDictionary parameters)
		{
			throw new NotImplementedException("RedirectRoute.CreateUrl is not implemented");
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
			if (url.ToLowerInvariant() == urlToMatch.ToLowerInvariant())
			{
				context.Response.Redirect(redirectUrl);

				return 100;
			}

			return 0;
		}
	}
}
