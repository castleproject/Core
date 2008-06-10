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
	using System.Text;
	using System.IO;
	using System.Web;
	using Castle.MonoRail.Framework;
	using Internal;
	using Routing;
	using Services;

	/// <summary>
	/// Adapts the <see cref="IResponse"/> to
	/// an <see cref="HttpResponse"/> instance.
	/// </summary>
	public class ResponseAdapter : BaseResponse
	{
		/// <summary>
		/// Store original <see cref="HttpResponse"/> class.
		/// </summary>
		protected readonly HttpResponse response;

		/// <summary>
		/// Initializes a new instance of the <see cref="ResponseAdapter"/> class.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <param name="currentUrl">The current URL.</param>
		/// <param name="urlBuilder">The URL builder.</param>
		/// <param name="serverUtility">The server utility.</param>
		/// <param name="routeMatch">The route match.</param>
		/// <param name="referrer">The referrer.</param>
		public ResponseAdapter(HttpResponse response, UrlInfo currentUrl, 
			IUrlBuilder urlBuilder, IServerUtility serverUtility,
			RouteMatch routeMatch, string referrer)
			: base(currentUrl, urlBuilder, serverUtility, routeMatch, referrer)
		{
			this.response = response;
		}

		/// <summary>
		/// Gets the caching policy (expiration time, privacy, 
		/// vary clauses) of a Web page.
		/// </summary>
		public override HttpCachePolicy CachePolicy
		{
			get { return response.Cache; }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Sets the Cache-Control HTTP header to Public or Private.
		/// </summary>
		public override string CacheControlHeader
		{
			get { return response.CacheControl; }
			set { response.CacheControl = value; }
		}

		/// <summary>
		/// Gets or sets the HTTP character set of the output stream.
		/// </summary>
		public override Encoding ContentEncoding
		{
			get { return response.ContentEncoding; }
			set { response.ContentEncoding = value; }
		}

		/// <summary>
		/// Gets or sets the HTTP character set of the output stream.
		/// </summary>
		public override string Charset
		{
			get { return response.Charset; }
			set { response.Charset = value; }
		}

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public override int StatusCode
		{
			get { return response.StatusCode; }
			set { response.StatusCode = value; }
		}

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public override string StatusDescription
		{
			get { return response.StatusDescription; }
			set { response.StatusDescription = value; }
		}

		/// <summary>
		/// Gets or sets the content type.
		/// </summary>
		/// <value>The type of the content.</value>
		public override string ContentType
		{
			get { return response.ContentType; }
			set { response.ContentType = value; }
		}

		/// <summary>
		/// Appends the header.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="headerValue">The header value.</param>
		public override void AppendHeader(string name, string headerValue)
		{
			response.AppendHeader(name, headerValue);
		}

		/// <summary>
		/// Gets the output.
		/// </summary>
		/// <value>The output.</value>
		public override TextWriter Output
		{
			get { return response.Output; }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the output stream.
		/// </summary>
		/// <value>The output stream.</value>
		public override Stream OutputStream
		{
			get { return response.OutputStream; }
		}

		/// <summary>
		/// Writes the buffer to the browser
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public override void BinaryWrite(byte[] buffer)
		{
			response.BinaryWrite(buffer);
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
			response.Clear();
		}

		/// <summary>
		/// Clears the response content (only works if buffered).
		/// </summary>
		public override void ClearContent()
		{
			response.ClearContent();
		}

		/// <summary>
		/// Writes the specified string.
		/// </summary>
		/// <param name="s">The string.</param>
		public override void Write(string s)
		{
			response.Write(s);
		}

		/// <summary>
		/// Writes the specified obj.
		/// </summary>
		/// <param name="obj">The obj.</param>
		public override void Write(object obj)
		{
			response.Write(obj);
		}

		/// <summary>
		/// Writes the specified char.
		/// </summary>
		/// <param name="ch">The char.</param>
		public override void Write(char ch)
		{
			response.Write(ch);
		}

		/// <summary>
		/// Writes the specified buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="index">The index.</param>
		/// <param name="count">The count.</param>
		public override void Write(char[] buffer, int index, int count)
		{
			response.Write(buffer, index, count);
		}

		/// <summary>
		/// Writes the file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		public override void WriteFile(string fileName)
		{
			response.WriteFile(fileName);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is client connected.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is client connected; otherwise, <c>false</c>.
		/// </value>
		public override bool IsClientConnected
		{
			get { return response.IsClientConnected; }
		}

		/// <summary>
		/// Gets a value indicating whether the response sent a redirect.
		/// </summary>
		/// <value><c>true</c> if was redirected; otherwise, <c>false</c>.</value>
		public override bool WasRedirected
		{
			get { return redirected; }
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="endProcess">if set to <c>true</c>, sends the redirect and
		/// kills the current request process.</param>
		public override void RedirectToUrl(string url, bool endProcess)
		{
			redirected = true;
			response.Redirect(url, endProcess);
		}

		/// <summary>
		/// Creates the cookie.
		/// </summary>
		/// <param name="cookie">The cookie.</param>
		public override void CreateCookie(HttpCookie cookie)
		{
			response.Cookies.Add(cookie);
		}
	}
}
