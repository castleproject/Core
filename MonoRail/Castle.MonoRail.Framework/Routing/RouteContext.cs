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

	/// <summary>
	/// The default RouteContext
	/// </summary>
	public class RouteContext : IRouteContext
	{
		private readonly string applicationPath;
		private readonly IRequest request;

		/// <summary>
		/// Creates a new RouteContext
		/// </summary>
		/// <param name="request"></param>
		/// <param name="applicationPath"></param>
		public RouteContext(IRequest request, string applicationPath)
		{
			this.request = request;
			this.applicationPath = applicationPath;
		}

		/// <summary>
		/// The ApplicationPath
		/// </summary>
		public string ApplicationPath
		{
			get { return applicationPath; }
		}

		/// <summary>
		/// The Http Request
		/// </summary>
		public IRequest Request
		{
			get { return request; }
		}
	}
}
