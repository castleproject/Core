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

namespace Castle.CastleOnRails.Framework.Tests
{
	using System;
	using System.IO;
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Security.Principal;
	using System.Web.Caching;

	/// <summary>
	/// Summary description for RailsEngineContextImpl.
	/// </summary>
	public class RailsEngineContextImpl : IRailsEngineContext
	{
		private object _context = new object();
		private String _url;
		private String _requestType;
		private RequestImpl _request = new RequestImpl();
		private ResponseImpl _response = new ResponseImpl();
		private Exception _lastException;
		private Hashtable _session = new Hashtable();
		private Hashtable _flashItems = new Hashtable();
		private IPrincipal _user;

		public RailsEngineContextImpl(String url) : this(url, "GET")
		{
		}

		public RailsEngineContextImpl(String url, String requestType)
		{
			_url = url;
			_requestType = requestType;
		}

		public void AddRequestParam(String name, String value)
		{
			_request._params.Add(name, value);
		}

		public object Output
		{
			get { return _response._contents.ToString(); }
		}

		#region IRailsEngineContext

		public Exception LastException
		{
			get { return _lastException; }
			set { _lastException = value; }
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
			get { return _request._params; }
		}

		public IDictionary Session
		{
			get { return _session; }
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
			get { throw new NotImplementedException(); }
		}

		public IDictionary Flash
		{
			get { return _flashItems; }
		}

		public void Transfer(String path, bool preserveForm)
		{
			throw new NotImplementedException();
		}

		public IPrincipal CurrentUser
		{
			get { return _user; }
			set { _user = value; }
		}
		
		#endregion
	}

	public class RequestImpl : IRequest
	{
		internal NameValueCollection _params = new NameValueCollection();

		public RequestImpl()
		{
		}

		#region IRequest Members

		public NameValueCollection Headers
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsLocal
		{
			get { return true; }
		}

		public String this[string key]
		{
			get { return _params[key]; }
		}

		public IDictionary Files
		{
			get { return null; }
		}

		public byte[] BinaryRead(int count)
		{
			return null;
		}

		public NameValueCollection Params
		{
			get { return _params; }
		}

		public Uri Uri
		{
			get { throw new NotImplementedException(); }
		}

		public String ReadCookie(String name)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class ResponseImpl : IResponse
	{
		private String _contentType;
		private int _statusCode;
		private StringWriter _writer;
		
		internal StringBuilder _contents = new StringBuilder();

		public ResponseImpl()
		{
			_writer = new StringWriter(_contents);
		}

		public int StatusCode
		{
			get { return _statusCode; }
			set { _statusCode = value; }
		}

		public String ContentType
		{
			get { return _contentType; }
			set { _contentType = value; }
		}

		public void AppendHeader(String name, String value)
		{
			throw new NotImplementedException();
		}

		public System.IO.TextWriter Output
		{
			get { return _writer; }
		}

		public System.IO.Stream OutputStream
		{
			get { throw new NotImplementedException(); }
		}

		public void Write(String s)
		{
			_writer.Write(s);
		}

		public void Write(object obj)
		{
			_writer.Write(obj);
		}

		public void Write(char ch)
		{
			_writer.Write(ch);
		}

		public void Write(char[] buffer, int index, int count)
		{
			_writer.Write(buffer, index, count);
		}

		public void Redirect(String url)
		{
			throw new NotImplementedException();
		}

		public void Redirect(String url, bool endProcess)
		{
			throw new NotImplementedException();
		}

		public void Redirect(String controller, String action)
		{
			throw new NotImplementedException();
		}

		public void Redirect(String area, String controller, String action)
		{
			throw new NotImplementedException();
		}

		public void CreateCookie(String name, String value)
		{
			throw new NotImplementedException();
		}
	}
}