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

namespace Castle.MonoRail.Framework.Test
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Web;

	/// <summary>
	/// Represents a mock implementation of <see cref="IRequest"/> for unit test purposes.
	/// </summary>
	public class MockRequest : IMockRequest
	{
		private NameValueCollection form = new NameValueCollection();
		private NameValueCollection headers = new NameValueCollection();
		private NameValueCollection queryString = new NameValueCollection();
		private NameValueCollection @params = new NameValueCollection();
		private IDictionary cookies;
		private IDictionary files = new Hashtable();

		private bool isLocal = true;
		private string rawUrl = null;
		private string httpMethod = "GET";
		private string filePath = null;
		private Uri uri = null;

		private string[] userLanguages = new string[] { "en-ES", "pt-BR" };
		private string userHostAddress = "127.0.0.1";
		private string pathInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="MockRequest"/> class.
		/// </summary>
		/// <param name="cookies">The cookies.</param>
		public MockRequest(IDictionary cookies)
		{
			this.cookies = cookies;
		}

		/// <summary>
		/// Reads the request data as a byte array.
		/// </summary>
		/// <param name="count">How many bytes.</param>
		/// <returns></returns>
		public virtual byte[] BinaryRead(int count)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Reads the cookie.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <returns></returns>
		public virtual string ReadCookie(string name)
		{
			return (string) cookies[name];
		}

		/// <summary>
		/// Validates the input.
		/// </summary>
		public virtual void ValidateInput()
		{
		}

		/// <summary>
		/// Gets the Http headers.
		/// </summary>
		/// <value>The Http headers.</value>
		public virtual NameValueCollection Headers
		{
			get { return headers; }
		}

		/// <summary>
		/// Gets the <see cref="HttpPostedFile"/> per key.
		/// </summary>
		/// <value></value>
		public virtual IDictionary Files
		{
			get { return files; }
		}

		/// <summary>
		/// Gets the params which accumulates headers, post, querystring and cookies.
		/// </summary>
		/// <value>The params.</value>
		public virtual NameValueCollection Params
		{
			get { return @params; }
		}

		/// <summary>
		/// Gets a value indicating whether this requeest is from a local address.
		/// </summary>
		/// <value><c>true</c> if this instance is local; otherwise, <c>false</c>.</value>
		public virtual bool IsLocal
		{
			get { return isLocal; }
			set { isLocal = value; }
		}

		/// <summary>
		/// Gets additional path information for
		/// a resource with a URL extension.
		/// </summary>
		/// <value>The path info.</value>
		public virtual string PathInfo
		{
			get { return pathInfo; }
			set { pathInfo = value; }
		}

		/// <summary>
		/// Gets the raw URL.
		/// </summary>
		/// <value>The raw URL.</value>
		public virtual string RawUrl
		{
			get { return rawUrl; }
			set { rawUrl = value; }
		}

		/// <summary>
		/// Gets the URI.
		/// </summary>
		/// <value>The URI.</value>
		public virtual Uri Uri
		{
			get { return uri; }
			set { uri = value; }
		}

		/// <summary>
		/// Gets the HTTP method.
		/// </summary>
		/// <value>The HTTP method.</value>
		public virtual string HttpMethod
		{
			get { return httpMethod; }
			set { httpMethod = value; }
		}

		/// <summary>
		/// Gets the file path.
		/// </summary>
		/// <value>The file path.</value>
		public virtual string FilePath
		{
			get { return filePath; }
			set { filePath = value; }
		}

		/// <summary>
		/// Gets the param with the specified key.
		/// </summary>
		/// <value></value>
		public virtual string this[string key]
		{
			get { return @params[key]; }
		}

		/// <summary>
		/// Gets the query string.
		/// </summary>
		/// <value>The query string.</value>
		public virtual NameValueCollection QueryString
		{
			get { return queryString; }
		}

		/// <summary>
		/// Gets the form.
		/// </summary>
		/// <value>The form.</value>
		public virtual NameValueCollection Form
		{
			get { return form; }
		}

		/// <summary>
		/// Gets the user languages.
		/// </summary>
		/// <value>The user languages.</value>
		public virtual string[] UserLanguages
		{
			get { return userLanguages; }
		}

		/// <summary>
		/// Gets the IP host address of the remote client.
		/// </summary>
		/// <value>The IP address of the remote client.</value>
		public virtual string UserHostAddress
		{
			get { return userHostAddress; }
		}
	}
}
