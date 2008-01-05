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

namespace Castle.MonoRail.Framework.Adapters
{
	using System;
	using System.Security.Principal;
	using System.Web;
	using System.Collections;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Container;

	/// <summary>
	/// Adapter to expose a valid <see cref="IEngineContext"/> 
	/// implementation on top of <c>HttpContext</c>.
	/// </summary>
	public class DefaultEngineContext : AbstractServiceContainer, IEngineContext
	{
		private readonly IMonoRailContainer container;
		private readonly HttpContext context;
		private readonly UrlInfo urlInfo;
		private readonly IServerUtility server;
		private readonly IRequest request;
		private readonly IResponse response;
		private Flash flash;
		private IDictionary session;
		private ITrace trace;
		private Exception lastException;
//		private IDictionary _session;
//		private String _url;
//		private ICacheProvider _cache;
//		private IServiceProvider container;
//		private bool customSessionSet;
		private IController currentController;
		private IControllerContext controllerContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultEngineContext"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="urlInfo">Url information</param>
		/// <param name="context">The context.</param>
		/// <param name="server">The server.</param>
		/// <param name="request">The request.</param>
		/// <param name="response">The response.</param>
		/// <param name="trace">The trace.</param>
		/// <param name="session">The session.</param>
		public DefaultEngineContext(IMonoRailContainer container, UrlInfo urlInfo,
		                            HttpContext context, IServerUtility server, 
			IRequest request, IResponse response, ITrace trace, 
		                            IDictionary session)
			: base(container)
		{
			this.container = container;
			this.context = context;
			this.urlInfo = urlInfo;
			this.request = request;
			this.response = response;
			this.session = session;
			this.server = server;
			this.trace = trace;
		}

		/// <summary>
		/// Gets the underlying context of the API being used.
		/// </summary>
		/// <value></value>
		public HttpContext UnderlyingContext
		{
			get { return context; }
		}

		/// <summary>
		/// Gets a reference to the MonoRail services.
		/// </summary>
		/// <value>The services.</value>
		public IMonoRailServices Services
		{
			get { return container; }
		}

		/// <summary>
		/// Gets the last exception raised during
		/// the execution of an action.
		/// </summary>
		/// <value></value>
		public Exception LastException
		{
			get { return lastException; }
			set { lastException = value; }
		}

//		/// <summary>
//		/// Gets the request type (GET, POST, etc)
//		/// </summary>
//		/// <value></value>
//		public String RequestType
//		{
//			get { return _context.Request.RequestType; }
//		}
//
//		/// <summary>
//		/// Gets the request URL.
//		/// </summary>
//		/// <value></value>
//		public String Url
//		{
//			get { return _url; }
//			set { _url = value; }
//		}

		/// <summary>
		/// Access the session objects.
		/// </summary>
		/// <value></value>
		public IDictionary Session
		{
			get { return session; }
			set { session = value; }
		}

		/// <summary>
		/// Gets the request object.
		/// </summary>
		/// <value></value>
		public IRequest Request
		{
			get { return request; }
		}

		/// <summary>
		/// Gets the response object.
		/// </summary>
		/// <value></value>
		public IResponse Response
		{
			get { return response; }
		}

		/// <summary>
		/// Gets the trace object.
		/// </summary>
		/// <value></value>
		public ITrace Trace
		{
			get { return trace; }
		}

		/// <summary>
		/// Returns an <see cref="IServerUtility"/>.
		/// </summary>
		/// <value></value>
		public IServerUtility Server
		{
			get { return server; }
		}

//		/// <summary>
//		/// Access the Cache associated with this
//		/// web execution context.
//		/// </summary>
//		/// <value></value>
//		public ICacheProvider Cache
//		{
//			get { return _cache; }
//		}

		/// <summary>
		/// Access a dictionary of volative items.
		/// </summary>
		/// <value></value>
		public Flash Flash
		{
			get { return flash; }
			set { flash = value; }
		}

//		/// <summary>
//		/// Transfer the execution to another resource.
//		/// </summary>
//		/// <param name="path"></param>
//		/// <param name="preserveForm"></param>
//		public void Transfer(String path, bool preserveForm)
//		{
//			_context.Server.Transfer(path, preserveForm);
//		}

		/// <summary>
		/// Gets or sets the current user.
		/// </summary>
		/// <value></value>
		public IPrincipal CurrentUser
		{
			get { return context.User; }
			set { context.User = value; }
		}

		/// <summary>
		/// Returns the <see cref="UrlInfo"/> of the the current request.
		/// </summary>
		/// <value></value>
		public UrlInfo UrlInfo
		{
			get { return urlInfo; }
		}

		/// <summary>
		/// Returns the application path.
		/// </summary>
		/// <value></value>
		public String ApplicationPath
		{
			get
			{
				String path;

				if (UnderlyingContext != null)
				{
					path = UnderlyingContext.Request.ApplicationPath;

					if ("/".Equals(path))
					{
						path = String.Empty;
					}
				}
				else
				{
					path = AppDomain.CurrentDomain.BaseDirectory;
				}

				return path;
			}
		}

//		/// <summary>
//		/// Returns the physical application path.
//		/// </summary>
//		public String ApplicationPhysicalPath
//		{
//			get 
//			{ 
//				String path;
//
//				if (UnderlyingContext != null)
//				{
//					path = UnderlyingContext.Request.ApplicationPath;
//				}
//				else
//				{
//					path = AppDomain.CurrentDomain.BaseDirectory;
//				}
//
//				return _context.Server.MapPath(path);
//			}
//		}

		/// <summary>
		/// Returns the Items collection from the current HttpContext.
		/// </summary>
		/// <value></value>
		public IDictionary Items
		{
			get { return UnderlyingContext.Items; }
		}

		/// <summary>
		/// Gets or sets the current controller.
		/// </summary>
		/// <value>The current controller.</value>
		public IController CurrentController
		{
			get { return currentController; }
			set { currentController = value; }
		}

		/// <summary>
		/// Gets or sets the current controller context.
		/// </summary>
		/// <value>The current controller context.</value>
		public IControllerContext CurrentControllerContext
		{
			get { return controllerContext; }
			set { controllerContext = value; }
		}

//		/// <summary>
//		/// If a container is available for the app, this 
//		/// property exposes its instance.
//		/// </summary>
//		public IServiceProvider Container
//		{
//			get { return container; }
//		}
	}
}