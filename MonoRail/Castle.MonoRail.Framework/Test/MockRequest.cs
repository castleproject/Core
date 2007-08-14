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

	public class MockRequest : IRequest
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

		public MockRequest(IDictionary cookies)
		{
			this.cookies = cookies;
		}

		public virtual byte[] BinaryRead(int count)
		{
			throw new NotImplementedException();
		}
		
		public virtual string ReadCookie(string name)
		{
			return (string) cookies[name];
		}

		public virtual void ValidateInput()
		{
			throw new NotImplementedException();
		}

		public virtual NameValueCollection Headers
		{
			get { return headers; }
		}

		public virtual IDictionary Files
		{
			get { return files; }
		}

		public virtual NameValueCollection Params
		{
			get { return @params; }
		}

		public virtual bool IsLocal
		{
			get { return isLocal; }
		}

		public virtual string RawUrl
		{
			get { return rawUrl; }
		}

		public virtual Uri Uri
		{
			get { return uri; }
		}

		public virtual string HttpMethod
		{
			get { return httpMethod; }
		}

		public virtual string FilePath
		{
			get { return filePath; }
		}

		public virtual string this[string key]
		{
			get { throw new NotImplementedException(); }
		}

		public virtual NameValueCollection QueryString
		{
			get { return queryString; }
		}

		public virtual NameValueCollection Form
		{
			get { return form; }
		}

		public virtual string[] UserLanguages
		{
			get { return userLanguages; }
		}

		public virtual string UserHostAddress
		{
			get { return userHostAddress; }
		}
	}
}
