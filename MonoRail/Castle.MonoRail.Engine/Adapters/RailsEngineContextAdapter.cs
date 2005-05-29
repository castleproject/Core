// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Engine.Adapters
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Web;
	using System.Web.Caching;
	using System.Security.Principal;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Adapter to expose a valid <see cref="IRailsEngineContext"/>
	/// implementation on top of <c>HttpContext</c>.
	/// </summary>
	public class RailsEngineContextAdapter : IRailsEngineContext
	{
		private String _url;
		private HttpContext _context;
		private RequestAdapter _request;
		private ResponseAdapter _response;
		private Exception _lastException;
		private SessionAdapter _session;
		private ServerUtilityAdapter _server;

		public RailsEngineContextAdapter(HttpContext context, String url)
		{
			_url = url;
			_context = context;
			_request = new RequestAdapter(context.Request);
			_response = new ResponseAdapter(context.Response, _url, ApplicationPath);
			_server = new ServerUtilityAdapter(context.Server);
		}

		public Exception LastException
		{
			get { return _lastException; }
			set { _lastException = value; }
		}

		public String RequestType
		{
			get { return _context.Request.RequestType; }
		}

		public String Url
		{
			get { return _url; }
		}

		public object UnderlyingContext
		{
			get { return _context; }
		}

		public NameValueCollection Params
		{
			get { return _context.Request.Params; }
		}

		public IDictionary Session
		{
			get
			{
				if (_session == null)
				{
					_session = new SessionAdapter(_context.Session);
				}
				return _session;
			}
		}

		public IRequest Request
		{
			get { return _request; }
		}

		public IResponse Response
		{
			get { return _response; }
		}

		public IServerUtility Server
		{
			get { return _server; }
		}

		public Cache Cache
		{
			get { return _context.Cache; }
		}

		public IDictionary Flash
		{
			get { return _context.Items; }
		}

		public void Transfer(String path, bool preserveForm)
		{
			_context.Server.Transfer(path, preserveForm);
		}

		public IPrincipal CurrentUser
		{
			get { return _context.User; }
			set { _context.User = value; }
		}

		/// <summary>
		/// Writes the message to the underlying tracing scheme
		/// </summary>
		/// <param name="message"></param>
		public void Trace(String message)
		{
			_context.Trace.Warn(message);
		}

		public String ApplicationPath
		{
			get
			{
				String path = null;

				if (HttpContext.Current != null)
				{
					path = HttpContext.Current.Request.ApplicationPath;
			
					if("/".Equals(path))
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
	}
}