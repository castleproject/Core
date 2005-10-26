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

namespace Castle.MonoRail.Framework.Adapters
{
	using System;
	using System.Web;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.MonoRail.Framework;

	/// <summary>
	/// Summary description for RequestAdapter.
	/// </summary>
	public class RequestAdapter : IRequest
	{
		private HttpRequest _request;
		private FileDictionaryAdapter _files;

		public RequestAdapter(HttpRequest request)
		{
			_request = request;
		}

		public NameValueCollection Headers
		{
			get { return _request.Headers; }
		}

		public bool IsLocal 
		{ 
			get { return _request.Url.IsLoopback; } 
		}

		public Uri Uri
		{
			get { return _request.Url; }
		}

		public NameValueCollection QueryString
		{
			get { return _request.QueryString; }
		}

		public NameValueCollection Form
		{
			get { return _request.Form; }
		}

		public byte[] BinaryRead(int count)
		{
			return _request.BinaryRead(count);
		}

		public String this[String key]
		{
			get { return _request[key]; }
		}

		public IDictionary Files
		{
			get
			{
				if (_files == null)
				{
					_files = new FileDictionaryAdapter(_request.Files);
				}
				return _files;
			}
		}

		public NameValueCollection Params
		{
			get { return _request.Params; }
		}

		public string[] UserLanguages
		{
			get { return _request.UserLanguages; }
		}

		public String ReadCookie(String name)
		{
			HttpCookie cookie = _request.Cookies[name];
			if (cookie == null)
			{
				return null;
			}
			return cookie.Value;
		}

		public void ValidateInput()
		{
			_request.ValidateInput();
		}
	}
}
