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

	/// <summary>
	/// The default RouteContext
	/// </summary>
	public class RouteContext : IRouteContext
	{
		private readonly string applicationPath;
		private readonly string url;
		private readonly IRequest request;

		/// <summary>
		/// Creates a new RouteContext
		/// </summary>
		/// <param name="request"></param>
		/// <param name="applicationPath"></param>
		/// <param name="url"></param>
		public RouteContext(IRequest request, string applicationPath, string url)
		{
			this.request = request;
			this.applicationPath = applicationPath;
			this.url = url;
		}

		/// <summary>
		/// The ApplicationPath
		/// </summary>
		public string ApplicationPath
		{
			get { return applicationPath; }
		}

		/// <summary>
		/// The raw url with leading and trailing slashes stripped.
		/// </summary>
		public string Url
		{
			get { return url; }
		}

		/// <summary>
		/// The Http Request
		/// </summary>
		public IRequest Request
		{
			get { return request; }
		}

		/// <summary>
		/// Strips leading and trailing /'s from the url.
		/// </summary>
		/// <param name="url"></param>
		/// <returns>The input url, stripped from leading and trailing slashes</returns>
		public static string StripUrl(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentNullException("url", "url cannot be empty nor null");
			}

			return url
				.TrimEnd('/')
				.TrimStart('/');
		}
	}
}
