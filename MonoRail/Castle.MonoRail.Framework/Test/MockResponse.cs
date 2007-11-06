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
	using System.IO;
	using System.Web;
	using Internal;

	/// <summary>
	/// Represents a mock implementation of <see cref="IMockResponse"/> for unit test purposes.
	/// </summary>
	public class MockResponse : IMockResponse
	{
		private readonly IDictionary cookies;
		private int statusCode = 400;
		private string contentType = "text/html";
		private string cacheControlHeader = null;
		private string charset = "ISO-8859-1";
		private string redirectedTo;
		private bool wasRedirected = false;
		private bool isClientConnected = false;
		private TextWriter output = new StringWriter();
		private Stream outputStream = new MemoryStream();
		private TextWriter outputStreamWriter;
		private HttpCachePolicy cachePolicy = null;
		private NameValueCollection headers = new NameValueCollection();

		/// <summary>
		/// Initializes a new instance of the <see cref="MockResponse"/> class.
		/// </summary>
		/// <param name="cookies">The cookies.</param>
		public MockResponse(IDictionary cookies)
		{
			this.cookies = cookies;
			outputStreamWriter = new StreamWriter(outputStream);
		}

		/// <summary>
		/// Gets the urls the request was redirected to.
		/// </summary>
		/// <value>The redirected to.</value>
		public virtual string RedirectedTo
		{
			get { return redirectedTo; }
		}

		/// <summary>
		/// Gets the http headers.
		/// </summary>
		/// <value>The headers.</value>
		public virtual NameValueCollection Headers
		{
			get { return headers; }
		}

		#region IResponse Related

		/// <summary>
		/// Appends the header.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public void AppendHeader(string name, string value)
		{
			headers[name] = value;
		}

		/// <summary>
		/// Writes the buffer to the browser
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public virtual void BinaryWrite(byte[] buffer)
		{
			outputStream.Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Writes the stream to the browser
		/// </summary>
		/// <param name="stream">The stream.</param>
		public virtual void BinaryWrite(Stream stream)
		{
			byte[] buffer = new byte[stream.Length];

			stream.Read(buffer, 0, buffer.Length);

			BinaryWrite(buffer);
		}

		/// <summary>
		/// Clears the response (only works if buffered)
		/// </summary>
		public virtual void Clear()
		{
			outputStream.SetLength(0);
		}

		/// <summary>
		/// Clears the response content (only works if buffered).
		/// </summary>
		public virtual void ClearContent()
		{
			outputStreamWriter.Flush();
		}

		/// <summary>
		/// Writes the specified string.
		/// </summary>
		/// <param name="s">The string.</param>
		public virtual void Write(string s)
		{
			outputStreamWriter.Write(s);
		}

		/// <summary>
		/// Writes the specified obj.
		/// </summary>
		/// <param name="obj">The obj.</param>
		public virtual void Write(object obj)
		{
			outputStreamWriter.Write(obj);
		}

		/// <summary>
		/// Writes the specified char.
		/// </summary>
		/// <param name="ch">The char.</param>
		public virtual void Write(char ch)
		{
			outputStreamWriter.Write(ch);
		}

		/// <summary>
		/// Writes the specified buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="index">The index.</param>
		/// <param name="count">The count.</param>
		public virtual void Write(char[] buffer, int index, int count)
		{
			outputStreamWriter.Write(buffer, index, count);
		}

		/// <summary>
		/// Writes the file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		public virtual void WriteFile(string fileName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Redirects the specified controller.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		public virtual void Redirect(string controller, string action)
		{
			Redirect(BuildMockUrl(null, controller, action));
		}

		/// <summary>
		/// Redirects the specified area.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		public virtual void Redirect(string area, string controller, string action)
		{
			Redirect(BuildMockUrl(area, controller, action));
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string controller, string action, NameValueCollection parameters)
		{
			Redirect(BuildMockUrl(controller, action, parameters));
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
			Redirect(BuildMockUrl(area, controller, action, parameters));
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(string controller, string action, IDictionary parameters)
		{
			Redirect(BuildMockUrl(controller, action, parameters));
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
			Redirect(BuildMockUrl(area, controller, action, parameters));
		}

		/// <summary>
		/// Redirects the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		public virtual void Redirect(string url)
		{
			wasRedirected = true;
			redirectedTo = url;
		}

		/// <summary>
		/// Redirects the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="endProcess">if set to <c>true</c> [end process].</param>
		public virtual void Redirect(string url, bool endProcess)
		{
			Redirect(url);
		}

		/// <summary>
		/// Creates a cookie.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public virtual void CreateCookie(string name, string value)
		{
			cookies.Add(name, value);
		}

		/// <summary>
		/// Creates a cookie.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <param name="expiration">The expiration.</param>
		public virtual void CreateCookie(string name, string value, DateTime expiration)
		{
			CreateCookie(name, value);
		}

		/// <summary>
		/// Creates a cookie.
		/// </summary>
		/// <param name="cookie">The cookie.</param>
		public virtual void CreateCookie(HttpCookie cookie)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Removes a cookie.
		/// </summary>
		/// <param name="name">The name.</param>
		public virtual void RemoveCookie(string name)
		{
			cookies.Remove(name);
		}

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public int StatusCode
		{
			get { return statusCode; }
			set { statusCode = value; }
		}

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		public string ContentType
		{
			get { return contentType; }
			set { contentType = value; }
		}

		/// <summary>
		/// Gets the caching policy (expiration time, privacy,
		/// vary clauses) of a Web page.
		/// </summary>
		/// <value></value>
		public HttpCachePolicy CachePolicy
		{
			get { return cachePolicy; }
		}

		/// <summary>
		/// Sets the Cache-Control HTTP header to Public or Private.
		/// </summary>
		/// <value></value>
		public string CacheControlHeader
		{
			get { return cacheControlHeader; }
			set { cacheControlHeader = value; }
		}

		/// <summary>
		/// Gets or sets the HTTP character set of the output stream.
		/// </summary>
		/// <value></value>
		public string Charset
		{
			get { return charset; }
			set { charset = value; }
		}

		/// <summary>
		/// Gets the output.
		/// </summary>
		/// <value>The output.</value>
		public virtual TextWriter Output
		{
			get { return output; }
		}

		/// <summary>
		/// Gets the output stream.
		/// </summary>
		/// <value>The output stream.</value>
		public virtual Stream OutputStream
		{
			get { return outputStream; }
		}

		/// <summary>
		/// Gets a value indicating whether the response sent a redirect.
		/// </summary>
		/// <value><c>true</c> if was redirected; otherwise, <c>false</c>.</value>
		public virtual bool WasRedirected
		{
			get { return wasRedirected; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is client connected.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is client connected; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsClientConnected
		{
			get { return isClientConnected; }
		}

		#endregion

		private static string BuildMockUrl(string area, string controller, string action, string querystring)
		{
			string mockUrl = "/";

			if (area != null)
			{
				mockUrl += area + "/";
			}

			mockUrl += controller + "/" + action + ".rails";

			if (querystring != null)
			{
				mockUrl += "?" + querystring;
			}

			return mockUrl;
		}

		private static string BuildMockUrl(string area, string controller, string action)
		{
			return BuildMockUrl(area, controller, action, (string) null);
		}

		private static string BuildMockUrl(string area, string controller, string action, IDictionary parameters)
		{
			return BuildMockUrl(area, controller, action, ToQueryString(parameters));
		}

		private static string BuildMockUrl(string controller, string action, IDictionary parameters)
		{
			return BuildMockUrl(controller, action, ToQueryString(parameters));
		}

		private static string BuildMockUrl(string area, string controller, string action, NameValueCollection parameters)
		{
			return BuildMockUrl(area, controller, action, ToQueryString(parameters));
		}

		private static string BuildMockUrl(string controller, string action, NameValueCollection parameters)
		{
			return BuildMockUrl(controller, action, ToQueryString(parameters));
		}

		/// <summary>
		/// Creates a querystring string representation of the namevalue collection.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		private static string ToQueryString(NameValueCollection parameters)
		{
			return CommonUtils.BuildQueryString(new MockServerUtility(), parameters, false);
		}

		/// <summary>
		/// Creates a querystring string representation of the entries in the dictionary.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		private static string ToQueryString(IDictionary parameters)
		{
			return CommonUtils.BuildQueryString(new MockServerUtility(), parameters, false);
		}
	}
}