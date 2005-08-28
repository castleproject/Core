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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	public class MockRequest : IRequest
	{
		internal NameValueCollection _params = new NameValueCollection();
		internal NameValueCollection _query = new NameValueCollection();
		internal NameValueCollection _form = new NameValueCollection();
		internal NameValueCollection _headers = new NameValueCollection();

		public MockRequest()
		{
		}

		public NameValueCollection Headers
		{
			get { return _headers; }
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

		public void ValidateInput()
		{
			throw new NotImplementedException();
		}

		public NameValueCollection QueryString
		{
			get { return _query; }
		}

		public NameValueCollection Form
		{
			get { return _form; }
		}

		public string[] UserLanguages
		{
			get { throw new NotImplementedException(); }
		}
	}
}
