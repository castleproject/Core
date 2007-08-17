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
	using System.Net;
	using System.Web;

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

		public MockResponse(IDictionary cookies)
		{
			this.cookies = cookies;
		    outputStreamWriter = new StreamWriter (outputStream);
		}

		public virtual string RedirectedTo
		{
			get { return redirectedTo; }
		}

		public virtual NameValueCollection Headers
		{
			get { return headers; }
		}

		#region IResponse Related

		public void AppendHeader(string name, string value)
		{
			headers[name] = value;
		}

		public virtual void BinaryWrite(byte[] buffer)
		{
			outputStream.Write (buffer, 0, buffer.Length);
		}

		public virtual void BinaryWrite(Stream stream)
		{
			byte[] buffer = new byte[stream.Length];

			stream.Read (buffer, 0, buffer.Length);

			BinaryWrite (buffer);
		}

		public virtual void Clear()
		{
			outputStream.SetLength (0);
		}

		public virtual void ClearContent()
		{
			outputStreamWriter.Flush ();
		}

		public virtual void Write(string s)
		{
			outputStreamWriter.Write (s);
		}

		public virtual void Write(object obj)
		{
			outputStreamWriter.Write (obj);
		}       	

		public virtual void Write(char ch)
		{
			outputStreamWriter.Write (ch);
		}

		public virtual void Write(char[] buffer, int index, int count)
		{
			outputStreamWriter.Write (buffer, index, count);
		}

		public virtual void WriteFile(string fileName)
		{
			throw new NotImplementedException();
		}

		public virtual void Redirect(string controller, string action)
		{
			Redirect(BuildMockUrl(null, controller, action));
		}

		public virtual void Redirect(string area, string controller, string action)
		{
			Redirect(BuildMockUrl(area, controller, action));
		}

		public virtual void Redirect(string url)
		{
			wasRedirected = true;
			redirectedTo = url;
		}

		public virtual void Redirect(string url, bool endProcess)
		{
			Redirect(url);
		}

		public virtual void CreateCookie(string name, string value)
		{
			cookies.Add(name,value);
		}

		public virtual void CreateCookie(string name, string value, DateTime expiration)
		{
			CreateCookie(name,value);
		}

		public virtual void CreateCookie(HttpCookie cookie)
		{
			throw new NotSupportedException();
		}	

		public virtual void RemoveCookie(string name)
		{
			cookies.Remove(name);
		}

		public int StatusCode
		{
			get { return statusCode; }
			set { statusCode = value; }
		}

		public string ContentType
		{
			get { return contentType; }
			set { contentType = value; }
		}

		public HttpCachePolicy CachePolicy
		{
			get { return cachePolicy; }
		}

		public string CacheControlHeader
		{
			get { return cacheControlHeader; }
			set { cacheControlHeader = value; }
		}

		public string Charset
		{
			get { return charset; }
			set { charset = value; }
		}

		public virtual TextWriter Output
		{
			get { return output; }
		}

		public virtual Stream OutputStream
		{
			get { return outputStream; }
		}

		public virtual bool WasRedirected
		{
			get { return wasRedirected; }
		}

		public virtual bool IsClientConnected
		{
			get { return isClientConnected; }
		}

		#endregion

		private string BuildMockUrl(string area, string controller, string action)
		{
			string mockUrl = "/";

			if (area != null)
			{
				mockUrl += area + "/";
			}

			mockUrl += controller + "/" + action + ".rails";

			return mockUrl;
		}
	}
}
