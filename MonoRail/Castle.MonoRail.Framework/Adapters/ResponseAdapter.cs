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
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using System.Web;
	using Castle.MonoRail.Framework;
	using Core;
	using Services;

	/// <summary>
	/// Adapts the <see cref="IResponse"/> to
	/// an <see cref="HttpResponse"/> instance.
	/// </summary>
	public class ResponseAdapter : IResponse
	{
		private readonly HttpResponse response;
		private readonly UrlInfo currentUrl;
		private readonly IUrlBuilder urlBuilder;
		private bool redirected;

		/// <summary>
		/// Initializes a new instance of the <see cref="ResponseAdapter"/> class.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <param name="currentUrl">The current URL.</param>
		/// <param name="urlBuilder">The URL builder.</param>
		public ResponseAdapter(HttpResponse response, UrlInfo currentUrl, IUrlBuilder urlBuilder)
		{
			this.response = response;
			this.currentUrl = currentUrl;
			this.urlBuilder = urlBuilder;
		}

		/// <summary>
		/// Gets the caching policy (expiration time, privacy, 
		/// vary clauses) of a Web page.
		/// </summary>
		public HttpCachePolicy CachePolicy
		{
			get { return response.Cache; }
		}

		/// <summary>
		/// Sets the Cache-Control HTTP header to Public or Private.
		/// </summary>
		public String CacheControlHeader
		{
			get { return response.CacheControl; }
			set { response.CacheControl = value; }
		}

		/// <summary>
		/// Gets or sets the HTTP character set of the output stream.
		/// </summary>
		public String Charset
		{
			get { return response.Charset; }
			set { response.Charset = value; }
		}

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public int StatusCode
		{
			get { return response.StatusCode; }
			set { response.StatusCode = value; }
		}

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public string StatusDescription
		{
			get { return response.StatusDescription; }
			set { response.StatusDescription = value; }
		}

		/// <summary>
		/// Gets or sets the content type.
		/// </summary>
		/// <value>The type of the content.</value>
		public String ContentType
		{
			get { return response.ContentType; }
			set { response.ContentType = value; }
		}

		/// <summary>
		/// Appends the header.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="headerValue">The header value.</param>
		public void AppendHeader(String name, String headerValue)
		{
			response.AppendHeader(name, headerValue);
		}

		/// <summary>
		/// Gets the output.
		/// </summary>
		/// <value>The output.</value>
		public TextWriter Output
		{
			get { return response.Output; }
		}

		/// <summary>
		/// Gets the output stream.
		/// </summary>
		/// <value>The output stream.</value>
		public Stream OutputStream
		{
			get { return response.OutputStream; }
		}

//		/// <summary>
//		/// Writes the buffer to the browser
//		/// </summary>
//		/// <param name="buffer">The buffer.</param>
//		public void BinaryWrite(byte[] buffer)
//		{
//			response.BinaryWrite(buffer);
//		}
//
//		/// <summary>
//		/// Writes the stream to the browser
//		/// </summary>
//		/// <param name="stream">The stream.</param>
//		public void BinaryWrite(Stream stream)
//		{
//			byte[] buffer = new byte[stream.Length];
//
//			stream.Read(buffer, 0, buffer.Length);
//
//			BinaryWrite(buffer);
//		}

		/// <summary>
		/// Clears the response (only works if buffered)
		/// </summary>
		public void Clear()
		{
			response.Clear();
		}

		/// <summary>
		/// Clears the response content (only works if buffered).
		/// </summary>
		public void ClearContent()
		{
			response.ClearContent();
		}

		/// <summary>
		/// Writes the specified string.
		/// </summary>
		/// <param name="s">The string.</param>
		public void Write(String s)
		{
			response.Write(s);
		}

		/// <summary>
		/// Writes the specified obj.
		/// </summary>
		/// <param name="obj">The obj.</param>
		public void Write(object obj)
		{
			response.Write(obj);
		}

		/// <summary>
		/// Writes the specified char.
		/// </summary>
		/// <param name="ch">The char.</param>
		public void Write(char ch)
		{
			response.Write(ch);
		}

		/// <summary>
		/// Writes the specified buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="index">The index.</param>
		/// <param name="count">The count.</param>
		public void Write(char[] buffer, int index, int count)
		{
			response.Write(buffer, index, count);
		}

//		/// <summary>
//		/// Writes the file.
//		/// </summary>
//		/// <param name="fileName">Name of the file.</param>
//		public void WriteFile(String fileName)
//		{
//			response.WriteFile(fileName);
//		}

		/// <summary>
		/// Redirects the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		public void RedirectToUrl(String url)
		{
			redirected = true;

			response.Redirect(url, false);
		}

		/// <summary>
		/// Redirects the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="endProcess">if set to <c>true</c> [end process].</param>
		public void RedirectToUrl(String url, bool endProcess)
		{
			redirected = true;

			response.Redirect(url, endProcess);
		}

		/// <summary>
		/// Redirects the specified controller.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		public void Redirect(String controller, String action)
		{
			redirected = true;

			response.Redirect(urlBuilder.BuildUrl(currentUrl, controller, action), false);
		}

		/// <summary>
		/// Redirects the specified controller.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		public void Redirect(object parameters)
		{
			redirected = true;

			response.Redirect(urlBuilder.BuildUrl(currentUrl, new ReflectionBasedDictionaryAdapter(parameters)), false);
		}

		/// <summary>
		/// Redirects the specified area.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		public void Redirect(String area, String controller, String action)
		{
			redirected = true;

			response.Redirect(urlBuilder.BuildUrl(currentUrl, area, controller, action), false);
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string controller, string action, NameValueCollection parameters)
		{
			redirected = true;

			response.Redirect(urlBuilder.BuildUrl(currentUrl, controller, action, parameters), false);
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string area, string controller, string action, NameValueCollection parameters)
		{
			redirected = true;

			response.Redirect(urlBuilder.BuildUrl(currentUrl, area, controller, action, parameters), false);
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string controller, string action, IDictionary parameters)
		{
			redirected = true;

			response.Redirect(urlBuilder.BuildUrl(currentUrl, controller, action, parameters), false);
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string area, string controller, string action, IDictionary parameters)
		{
			redirected = true;

			response.Redirect(urlBuilder.BuildUrl(currentUrl, area, controller, action, parameters), false);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is client connected.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is client connected; otherwise, <c>false</c>.
		/// </value>
		public bool IsClientConnected
		{
			get { return response.IsClientConnected; }
		}

		/// <summary>
		/// Gets a value indicating whether the response sent a redirect.
		/// </summary>
		/// <value><c>true</c> if was redirected; otherwise, <c>false</c>.</value>
		public bool WasRedirected
		{
			get { return redirected; }
		}

		/// <summary>
		/// Creates the cookie.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cookieValue">The cookie value.</param>
		public void CreateCookie(String name, String cookieValue)
		{
			CreateCookie(new HttpCookie(name, cookieValue));
		}

		/// <summary>
		/// Creates the cookie.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cookieValue">The cookie value.</param>
		/// <param name="expiration">The expiration.</param>
		public void CreateCookie(String name, String cookieValue, DateTime expiration)
		{
			HttpCookie cookie = new HttpCookie(name, cookieValue);

			cookie.Expires = expiration;
			cookie.Path = SafeAppPath();

			CreateCookie(cookie);
		}

		/// <summary>
		/// Creates the cookie.
		/// </summary>
		/// <param name="cookie">The cookie.</param>
		public void CreateCookie(HttpCookie cookie)
		{
			response.Cookies.Add(cookie);
		}

		/// <summary>
		/// Removes the cookie.
		/// </summary>
		/// <param name="name">The name.</param>
		public void RemoveCookie(string name)
		{
			HttpCookie cookie = new HttpCookie(name, "");
			
			cookie.Expires = DateTime.Now.AddYears(-10);
			cookie.Path = SafeAppPath();
			
			CreateCookie(cookie);
		}

		private string SafeAppPath()
		{
			return currentUrl.AppVirtualDir == string.Empty ? "/" : currentUrl.AppVirtualDir;
		}
	}
}