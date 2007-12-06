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

namespace Castle.MonoRail.Framework.Tests.Routing.Stubs
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	public class StubRequest : IRequest
	{
		readonly Verb httpMethod;
		public StubRequest()
		{
			httpMethod = Verb.Get;
		}

		public StubRequest(Verb httpMethod)
		{
			this.httpMethod = httpMethod;
		}

		public NameValueCollection Headers
		{
			get { throw new NotImplementedException(); }
		}

		public IDictionary Files
		{
			get { throw new NotImplementedException(); }
		}

		public NameValueCollection Params
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsLocal
		{
			get { throw new NotImplementedException(); }
		}

		public string PathInfo
		{
			get { throw new NotImplementedException(); }
		}

		public string RawUrl
		{
			get { throw new NotImplementedException(); }
		}

		public Uri Uri
		{
			get { throw new NotImplementedException(); }
		}

		public string HttpMethod
		{
			get { return httpMethod.ToString(); }
		}

		public string FilePath
		{
			get { throw new NotImplementedException(); }
		}

		public byte[] BinaryRead(int count)
		{
			throw new NotImplementedException();
		}

		public string this[string key]
		{
			get { throw new NotImplementedException(); }
		}

		public string ReadCookie(string name)
		{
			throw new NotImplementedException();
		}

		public NameValueCollection QueryString
		{
			get { throw new NotImplementedException(); }
		}

		public NameValueCollection Form
		{
			get { throw new NotImplementedException(); }
		}

		public string[] UserLanguages
		{
			get { throw new NotImplementedException(); }
		}

		public string UserHostAddress
		{
			get { throw new NotImplementedException(); }
		}

		public void ValidateInput()
		{
			throw new NotImplementedException();
		}
	}
}
