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

namespace Castle.MonoRail.Framework.Adapters
{
	using System;
	using System.ComponentModel.Design;
	using System.Web;
	using System.Web.SessionState;
	using System.Security.Principal;
	using System.Collections;
	using System.Collections.Specialized;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Services;
	using Castle.Core;

	/// <summary>
	/// Adapter to expose a valid <see cref="IRailsEngineContext"/> 
	/// implementation on top of <c>HttpContext</c>.
	/// </summary>
	public class DefaultRailsEngineContext : AbstractServiceContainer, IRailsEngineContext
	{
		private HttpContext _context;
		private RequestAdapter _request;
		private ResponseAdapter _response;
		private TraceAdapter _trace;
		private Exception _lastException;
		private IDictionary _session;
		private ServerUtilityAdapter _server;
		private Flash _flash;
		private UrlInfo _urlInfo;
		private String _url;
		private ICacheProvider _cache;
		private Controller currentController;
		private IServiceProvider container;
		private bool customSessionSet;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultRailsEngineContext"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="urlInfo">Url information</param>
		/// <param name="context">The context.</param>
		/// <param name="container">External container instance</param>
		public DefaultRailsEngineContext(IServiceContainer parent, UrlInfo urlInfo,
										 HttpContext context, IServiceProvider container)
			: base(parent)
		{
			_urlInfo = urlInfo;
			_context = context;
			_request = new RequestAdapter(context.Request);
			_trace = new TraceAdapter(context.Trace);
			_server = new ServerUtilityAdapter(context.Server);
			_response = new ResponseAdapter(context.Response, this, ApplicationPath);
			_url = _context.Request.RawUrl;
			_cache = parent.GetService(typeof(ICacheProvider)) as ICacheProvider;
			this.container = container;
		}

		/// <summary>
		/// Gets the last exception raised during
		/// the execution of an action.
		/// </summary>
		/// <value></value>
		public Exception LastException
		{
			get { return _lastException; }
			set { _lastException = value; }
		}

		/// <summary>
		/// Gets the request type (GET, POST, etc)
		/// </summary>
		/// <value></value>
		public String RequestType
		{
			get { return _context.Request.RequestType; }
		}

		/// <summary>
		/// Gets the request URL.
		/// </summary>
		/// <value></value>
		public String Url
		{
			get { return _url; }
			set { _url = value; }
		}

		/// <summary>
		/// Gets the referring URL.
		/// </summary>
		/// <value></value>
		public String UrlReferrer
		{
			get
			{
				Uri referrer = _context.Request.UrlReferrer;

				if (referrer != null)
				{
					return referrer.ToString();
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the underlying context of the API being used.
		/// </summary>
		/// <value></value>
		public HttpContext UnderlyingContext
		{
			get { return _context; }
		}

		/// <summary>
		/// Access the params (Query, Post, headers and Cookies)
		/// </summary>
		/// <value></value>
		public NameValueCollection Params
		{
			get { return _context.Request.Params; }
		}

		/// <summary>
		/// Access the session objects.
		/// </summary>
		/// <value></value>
		public IDictionary Session
		{
			get { return _session; }
			set
			{
				customSessionSet = true;
				_session = value;
			}
		}

		/// <summary>
		/// Gets the request object.
		/// </summary>
		/// <value></value>
		public IRequest Request
		{
			get { return _request; }
		}

		/// <summary>
		/// Gets the response object.
		/// </summary>
		/// <value></value>
		public IResponse Response
		{
			get { return _response; }
		}

		/// <summary>
		/// Gets the trace object.
		/// </summary>
		/// <value></value>
		public ITrace Trace
		{
			get { return _trace; }
		}

		/// <summary>
		/// Returns an <see cref="IServerUtility"/>.
		/// </summary>
		/// <value></value>
		public IServerUtility Server
		{
			get { return _server; }
		}

		/// <summary>
		/// Access the Cache associated with this
		/// web execution context.
		/// </summary>
		/// <value></value>
		public ICacheProvider Cache
		{
			get { return _cache; }
		}

		/// <summary>
		/// Access a dictionary of volative items.
		/// </summary>
		/// <value></value>
		public Flash Flash
		{
			get
			{
				if (_flash == null && Session != null)
				{
					_flash = new Flash((Flash) Session[Flash.FlashKey]);
				}

				return _flash;
			}
		}

		/// <summary>
		/// Transfer the execution to another resource.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="preserveForm"></param>
		public void Transfer(String path, bool preserveForm)
		{
			_context.Server.Transfer(path, preserveForm);
		}

		/// <summary>
		/// Gets or sets the current user.
		/// </summary>
		/// <value></value>
		public IPrincipal CurrentUser
		{
			get { return _context.User; }
			set { _context.User = value; }
		}

		/// <summary>
		/// Returns the <see cref="UrlInfo"/> of the the current request.
		/// </summary>
		/// <value></value>
		public UrlInfo UrlInfo
		{
			get { return _urlInfo; }
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

		/// <summary>
		/// Returns the physical application path.
		/// </summary>
		public String ApplicationPhysicalPath
		{
			get 
			{ 
				String path;

				if (UnderlyingContext != null)
				{
					path = UnderlyingContext.Request.ApplicationPath;
				}
				else
				{
					path = AppDomain.CurrentDomain.BaseDirectory;
				}

				return _context.Server.MapPath(path);
			}
		}

		/// <summary>
		/// Resolves the request session.
		/// </summary>
		public void ResolveRequestSession()
		{
			// Someone set a custom session, so we skip this
			if (customSessionSet) return;
			
			object session;

			if (_context.Items["AspSession"] != null)
			{
				// Windows and Testing
				session = _context.Items["AspSession"];
			}
			else
			{
				// Mono
				session = _context.Session;
			}

			if (session is HttpSessionState)
			{
				_session = new SessionAdapter(session as HttpSessionState);
			}
			else
			{
				_session = (IDictionary) session;
			}			
		}

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
		public Controller CurrentController
		{
			get { return currentController; }
			set { currentController = value; }
		}

		/// <summary>
		/// If a container is available for the app, this 
		/// property exposes its instance.
		/// </summary>
		public IServiceProvider Container
		{
			get { return container; }
		}
	}
}
