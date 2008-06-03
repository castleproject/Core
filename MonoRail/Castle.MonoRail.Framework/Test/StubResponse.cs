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

namespace Castle.MonoRail.Framework.Test
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.IO;
	using System.Web;
	using Internal;
	using Routing;
	using Services;

	/// <summary>
	/// Represents a mock implementation of <see cref="IMockResponse"/> for unit test purposes.
	/// </summary>
	public class StubResponse : BaseResponse, IMockResponse
	{
		private readonly IDictionary<string, HttpCookie> cookies;
		private int statusCode = 200;
		private string statusDescription = "OK";
		private string contentType = "text/html";
		private string cacheControlHeader;
		private string charset = "ISO-8859-1";
		private string redirectedTo;
		private bool wasRedirected = false;
		private bool isClientConnected = false;
		private StringWriter output;
		private HttpCachePolicy cachePolicy;
		private NameValueCollection headers = new NameValueCollection();
		private Stream outputStream = new MemoryStream();

		/// <summary>
		/// Initializes a new instance of the <see cref="StubResponse"/> class.
		/// </summary>
		/// <param name="currentUrl">The current URL.</param>
		/// <param name="urlBuilder">The URL builder.</param>
		/// <param name="serverUtility">The server utility.</param>
		/// <param name="routeMatch">The route match.</param>
		/// <param name="referrer">The referrer.</param>
		public StubResponse(UrlInfo currentUrl, IUrlBuilder urlBuilder, IServerUtility serverUtility, RouteMatch routeMatch, string referrer)
			: base(currentUrl, urlBuilder, serverUtility, routeMatch, referrer)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StubResponse"/> class.
		/// </summary>
		/// <param name="currentUrl">The current URL.</param>
		/// <param name="urlBuilder">The URL builder.</param>
		/// <param name="serverUtility">The server utility.</param>
		/// <param name="routeMatch">The route match.</param>
		public StubResponse(UrlInfo currentUrl, IUrlBuilder urlBuilder, IServerUtility serverUtility, RouteMatch routeMatch)
			: this(currentUrl, urlBuilder, serverUtility, routeMatch, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StubResponse"/> class.
		/// </summary>
		/// <param name="cookies">The cookies.</param>
		/// <param name="info">Current url</param>
		public StubResponse(IDictionary<string, HttpCookie> cookies, UrlInfo info): this(
			info, new DefaultUrlBuilder(), new StubServerUtility(), new RouteMatch())
		{
			this.cookies = cookies;
			output = new StringWriter();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StubResponse"/> class.
		/// </summary>
		/// <param name="cookies">The cookies.</param>
		public StubResponse(IDictionary<string, HttpCookie> cookies)
			: this(cookies, new UrlInfo("", "controller", "action", "/", ".castle"))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StubResponse"/> class.
		/// </summary>
		public StubResponse(): this(new Dictionary<string, HttpCookie>(StringComparer.InvariantCultureIgnoreCase))
		{
		}

		/// <summary>
		/// Gets the urls the request was redirected to.
		/// </summary>
		/// <value>The redirected to.</value>
		public override string RedirectedTo
		{
			get { return redirectedTo; }
		}

		/// <summary>
		/// Gets the http headers.
		/// </summary>
		/// <value>The headers.</value>
		public NameValueCollection Headers
		{
			get { return headers; }
		}

		/// <summary>
		/// Determines whether a cookie is present on the response.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <returns>
		/// 	<c>true</c> if the cookie exists in the response; otherwise, <c>false</c>.
		/// </returns>
		public override bool IsCookiePresent(string name)
		{
			return cookies.ContainsKey(name);
		}

		/// <summary>
		/// Gets a cookie added through one of
		/// the <see cref="IResponse.CreateCookie(HttpCookie)"/> overloads.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <returns></returns>
		public override HttpCookie GetCookie(string name)
		{
			HttpCookie cookie;
			cookies.TryGetValue(name, out cookie);
			return cookie;
		}

		#region IResponse Related

		/// <summary>
		/// Appends the header.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public override void AppendHeader(string name, string value)
		{
			headers[name] = value;
		}

		/// <summary>
		/// Writes the buffer to the browser
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public override void BinaryWrite(byte[] buffer)
		{
//			output.Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Writes the stream to the browser
		/// </summary>
		/// <param name="stream">The stream.</param>
		public override void BinaryWrite(Stream stream)
		{
			byte[] buffer = new byte[stream.Length];

			stream.Read(buffer, 0, buffer.Length);

			BinaryWrite(buffer);
		}

		/// <summary>
		/// Clears the response (only works if buffered)
		/// </summary>
		public override void Clear()
		{
			output.GetStringBuilder().Length = 0;
		}

		/// <summary>
		/// Clears the response content (only works if buffered).
		/// </summary>
		public override void ClearContent()
		{
		}

		/// <summary>
		/// Writes the specified string.
		/// </summary>
		/// <param name="s">The string.</param>
		public override void Write(string s)
		{
			output.Write(s);
		}

		/// <summary>
		/// Writes the specified obj.
		/// </summary>
		/// <param name="obj">The obj.</param>
		public override void Write(object obj)
		{
			output.Write(obj);
		}

		/// <summary>
		/// Writes the specified char.
		/// </summary>
		/// <param name="ch">The char.</param>
		public override void Write(char ch)
		{
			output.Write(ch);
		}

		/// <summary>
		/// Writes the specified buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="index">The index.</param>
		/// <param name="count">The count.</param>
		public override void Write(char[] buffer, int index, int count)
		{
			output.Write(buffer, index, count);
		}

		/// <summary>
		/// Writes the file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		public override void WriteFile(string fileName)
		{
			// TODO: record file name
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="endProcess">if set to <c>true</c>, sends the redirect and
		/// kills the current request process.</param>
		public override void RedirectToUrl(string url, bool endProcess)
		{
			wasRedirected = true;
			redirectedTo = url;
		}

		/// <summary>
		/// Creates a http cookie.
		/// </summary>
		/// <param name="cookie">The cookie.</param>
		public override void CreateCookie(HttpCookie cookie)
		{
			if (cookie.Value == string.Empty)
			{
				cookies.Remove(cookie.Name);
			}
			else
			{
				cookies[cookie.Name] = cookie;
			}
		}

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public override int StatusCode
		{
			get { return statusCode; }
			set { statusCode = value; }
		}

		/// <summary>
		/// Gets or sets the status description.
		/// </summary>
		/// <value>The status code.</value>
		public override string StatusDescription
		{
			get { return statusDescription; }
			set { statusDescription = value; }
		}

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		public override string ContentType
		{
			get { return contentType; }
			set { contentType = value; }
		}

		/// <summary>
		/// Gets the caching policy (expiration time, privacy,
		/// vary clauses) of a Web page.
		/// </summary>
		/// <value></value>
		public override HttpCachePolicy CachePolicy
		{
			get { return cachePolicy; }
			set { cachePolicy = value; }
		}

		/// <summary>
		/// Sets the Cache-Control HTTP header to Public or Private.
		/// </summary>
		/// <value></value>
		public override string CacheControlHeader
		{
			get { return cacheControlHeader; }
			set { cacheControlHeader = value; }
		}

		/// <summary>
		/// Gets or sets the HTTP character set of the output stream.
		/// </summary>
		/// <value></value>
		public override string Charset
		{
			get { return charset; }
			set { charset = value; }
		}

		/// <summary>
		/// Gets the output.
		/// </summary>
		/// <value>The output.</value>
		public override TextWriter Output
		{
			get { return output; }
			set { output = (StringWriter) value; }
		}

		/// <summary>
		/// Gets the output stream.
		/// </summary>
		/// <value>The output stream.</value>
		public override Stream OutputStream
		{
			get { return outputStream; }
		}

		/// <summary>
		/// Gets a value indicating whether the response sent a redirect.
		/// </summary>
		/// <value><c>true</c> if was redirected; otherwise, <c>false</c>.</value>
		public override bool WasRedirected
		{
			get { return wasRedirected; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is client connected.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is client connected; otherwise, <c>false</c>.
		/// </value>
		public override bool IsClientConnected
		{
			get { return isClientConnected; }
		}

		/// <summary>
		/// Gets the output.
		/// </summary>
		/// <value>The output.</value>
		public override string OutputContent
		{
			get { return output.GetStringBuilder().ToString(); }
		}

		#endregion
	}
}
