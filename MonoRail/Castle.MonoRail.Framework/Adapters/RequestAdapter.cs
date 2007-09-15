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
	using System.Collections;
	using System.Collections.Specialized;
	using System.Web;

	/// <summary>
	/// This class adapts the <c>HttpRequest</c> to a MonoRail <c>IRequest</c>.
	/// </summary>
	public class RequestAdapter : IRequest
	{
		private HttpRequest request;
		private FileDictionaryAdapter files;

		/// <summary>
		/// Initializes a new instance of the <see cref="RequestAdapter"/> class.
		/// </summary>
		/// <param name="request">The request.</param>
		public RequestAdapter(HttpRequest request)
		{
			this.request = request;
		}

		/// <summary>
		/// Gets the Http headers.
		/// </summary>
		/// <value>The Http headers.</value>
		public NameValueCollection Headers
		{
			get { return request.Headers; }
		}

		/// <summary>
		/// Gets a value indicating whether this requeest is from a local address.
		/// </summary>
		/// <value><c>true</c> if this instance is local; otherwise, <c>false</c>.</value>
		public bool IsLocal
		{
			get { return request.Url.IsLoopback; }
		}

		/// <summary>
		/// Gets the HTTP method.
		/// </summary>
		/// <value>The HTTP method.</value>
		public string HttpMethod
		{
			get { return request.HttpMethod; }
		}

		/// <summary>
		/// Gets the URI.
		/// </summary>
		/// <value>The URI.</value>
		public Uri Uri
		{
			get { return request.Url; }
		}

		/// <summary>
		/// Gets the raw URL.
		/// </summary>
		/// <value>The raw URL.</value>
		public String RawUrl
		{
			get { return request.RawUrl; }
		}

		/// <summary>
		/// Gets the file path.
		/// </summary>
		/// <value>The file path.</value>
		public String FilePath
		{
			get { return request.FilePath; }
		}

		/// <summary>
		/// Gets the query string.
		/// </summary>
		/// <value>The query string.</value>
		public NameValueCollection QueryString
		{
			get { return request.QueryString; }
		}

		/// <summary>
		/// Gets the form.
		/// </summary>
		/// <value>The form.</value>
		public NameValueCollection Form
		{
			get { return request.Form; }
		}

		/// <summary>
		/// Reads the request data as a byte array.
		/// </summary>
		/// <param name="count">How many bytes.</param>
		/// <returns></returns>
		public byte[] BinaryRead(int count)
		{
			return request.BinaryRead(count);
		}

		/// <summary>
		/// Gets the param with the specified key.
		/// </summary>
		/// <value></value>
		public String this[String key]
		{
			get { return request[key]; }
		}

		/// <summary>
		/// Gets the <see cref="HttpPostedFile"/> per key.
		/// </summary>
		/// <value></value>
		public IDictionary Files
		{
			get
			{
				if (files == null)
				{
					files = new FileDictionaryAdapter(request.Files);
				}
				return files;
			}
		}

		/// <summary>
		/// Gets the params which accumulates headers, post, querystring and cookies.
		/// </summary>
		/// <value>The params.</value>
		public NameValueCollection Params
		{
			get { return request.Params; }
		}

		/// <summary>
		/// Gets the user languages.
		/// </summary>
		/// <value>The user languages.</value>
		public String[] UserLanguages
		{
			get { return request.UserLanguages; }
		}

		/// <summary>
		/// Gets the IP host address of the remote client.
		/// </summary>
		/// <value>The IP address of the remote client.</value>
		public string UserHostAddress
		{
			get { return request.UserHostAddress; }
		}

		/// <summary>
		/// Reads the cookie.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <returns></returns>
		public String ReadCookie(String name)
		{
			HttpCookie cookie = request.Cookies[name];
			if (cookie == null)
			{
				return null;
			}
			return cookie.Value;
		}

		/// <summary>
		/// Validates the input.
		/// </summary>
		public void ValidateInput()
		{
			request.ValidateInput();
		}
	}
}