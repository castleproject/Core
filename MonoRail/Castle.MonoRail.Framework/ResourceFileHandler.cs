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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Web;
	using Services;

	/// <summary>
	/// Handles the service of static files (usually js files)
	/// </summary>
	public class ResourceFileHandler : IHttpHandler
	{
		private readonly UrlInfo urlInfo;
		private readonly IStaticResourceRegistry staticResourceRegistry;

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceFileHandler"/> class.
		/// </summary>
		/// <param name="urlInfo">The URL info.</param>
		/// <param name="staticResourceRegistry">The static resource registry.</param>
		public ResourceFileHandler(UrlInfo urlInfo, IStaticResourceRegistry staticResourceRegistry)
		{
			this.urlInfo = urlInfo;
			this.staticResourceRegistry = staticResourceRegistry;
		}

		/// <summary>
		/// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		public void ProcessRequest(HttpContext context)
		{
			string name = context.Request.QueryString["name"];
			string location = context.Request.QueryString["location"];
			string version = context.Request.QueryString["version"];

			if (name == null)
			{
				name = urlInfo.Action;
			}

			try
			{
				if (!staticResourceRegistry.Exists(name, location, version))
				{
					context.Response.StatusCode = 404;

					return;
				}

				string mimeType;
				string content = staticResourceRegistry.GetResource(name, location, version, out mimeType);

				context.Response.ContentType = mimeType;
				context.Response.Write(content);
			}
			catch(Exception)
			{
				context.Response.StatusCode = 500;

				throw;
			}
		}

		/// <summary>
		/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
		/// </summary>
		/// <value></value>
		/// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
		public bool IsReusable
		{
			get { return false; }
		}
	}
}
