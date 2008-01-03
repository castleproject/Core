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
	using System.Collections.Generic;

	/// <summary>
	/// Pendent
	/// </summary>
	[Serializable]
	public class RouteMatch
	{
		/// <summary>
		/// Key used to store the route match on the HttpContext.Items
		/// </summary>
		public static readonly string RouteMatchKey = "route.match";
		private readonly Dictionary<string, string> parameters = new Dictionary<string, string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="RouteMatch"/> class.
		/// </summary>
		public RouteMatch()
		{
		}

		/// <summary>
		/// Gets the parameters.
		/// </summary>
		/// <value>The parameters.</value>
		public Dictionary<string, string> Parameters
		{
			get { return parameters; }
		}

		/// <summary>
		/// Adds the named.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public void AddNamed(string name, string value)
		{
			parameters[name] = value;
		}
	}
}