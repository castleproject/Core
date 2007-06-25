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
		private IDictionary files = new Hashtable();
		private NameValueCollection form = new NameValueCollection();
		private NameValueCollection headers = new NameValueCollection();
		private NameValueCollection queryString = new NameValueCollection();
		private NameValueCollection @params = new NameValueCollection();

		private bool isLocal = true;
		private string rawUrl = null;
		private string httpMethod = "GET";
		private string filePath = null;
		private Uri uri = null;

		private string[] userLanguages = new string[] { "en-ES", "pt-BR" };
		private string userHostAddress = "127.0.0.1";

		public virtual byte[] BinaryRead(int count)
		{
			throw new NotImplementedException();
		}

		public virtual string ReadCookie(string name)
		{
			throw new NotImplementedException();
		}

		public virtual void ValidateInput()
		{
			throw new NotImplementedException();
		}

		public NameValueCollection Headers
		{
			get { return headers; }
		}

		public IDictionary Files
		{
			get { return files; }
		}

		public NameValueCollection Params
		{
			get { return @params; }
		}

		public bool IsLocal
		{
			get { return isLocal; }
		}

		public string RawUrl
		{
			get { return rawUrl; }
		}

		public Uri Uri
		{
			get { return uri; }
		}

		public string HttpMethod
		{
			get { return httpMethod; }
		}

		public string FilePath
		{
			get { return filePath; }
		}

		public virtual string this[string key]
		{
			get { throw new NotImplementedException(); }
		}

		public NameValueCollection QueryString
		{
			get { return queryString; }
		}

		public NameValueCollection Form
		{
			get { return form; }
		}

		public string[] UserLanguages
		{
			get { return userLanguages; }
		}

		public string UserHostAddress
		{
			get { return userHostAddress; }
		}
	}
}
