// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Engine.Adapters
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Web;
	using System.Web.Caching;
	using System.Security.Principal;

	using Castle.CastleOnRails.Framework;

	/// <summary>
	/// Summary description for RailsEngineContextAdapter.
	/// </summary>
	public class RailsEngineContextAdapter : IRailsEngineContext
	{
		private HttpContext _context;
		private String _url;
		private String _requestType;
		private RequestAdapter _request;
		private ResponseAdapter _response;

		public RailsEngineContextAdapter(HttpContext context, String url, String requestType)
		{
			_context = context;
			_url = url;
			_requestType = requestType;
			_request = new RequestAdapter(context.Request);
			_response = new ResponseAdapter(context.Response);
		}

		public String RequestType
		{
			get { return _requestType; }
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

		public NameValueCollection Session
		{
			get { throw new NotImplementedException(); }
		}

		public IRequest Request
		{
			get { return _request; }
		}

		public IResponse Response
		{
			get { return _response; }
		}

		public Cache Cache
		{
			get { return _context.Cache; }
		}

		public IDictionary Items
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
		}
	}
}
