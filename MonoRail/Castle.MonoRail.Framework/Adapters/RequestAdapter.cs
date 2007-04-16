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

namespace Castle.MonoRail.Framework.Adapters
{
	using System;
	using System.Web;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.MonoRail.Framework;

	/// <summary>
	/// This class adapts the <c>HttpRequest</c> to a MonoRail <c>IRequest</c>.
	/// </summary>
	public class RequestAdapter : IRequest
	{
		private HttpRequest request;
		private FileDictionaryAdapter files;

		public RequestAdapter( HttpRequest request )
		{
			this.request = request;
		}

		public NameValueCollection Headers
		{
			get { return request.Headers; }
		}

		public bool IsLocal 
		{ 
			get { return request.Url.IsLoopback; } 
		}

		public string HttpMethod
		{
			get { return request.HttpMethod; }
		}
		
		public Uri Uri
		{
			get { return request.Url; }
		}

		public String RawUrl
		{
			get { return request.RawUrl; }
		}

		public String FilePath
		{
			get { return request.FilePath; }
		}

		public NameValueCollection QueryString
		{
			get { return request.QueryString; }
		}

		public NameValueCollection Form
		{
			get { return request.Form; }
		}

		public byte[] BinaryRead( int count )
		{
			return request.BinaryRead( count );
		}

		public String this[ String key ]
		{
			get { return request[ key ]; }
		}

		public IDictionary Files
		{
			get
			{
				if ( files == null )
				{
					files = new FileDictionaryAdapter(request.Files);
				}
				return files;
			}
		}

		public NameValueCollection Params
		{
			get { return request.Params; }
		}

		public String[] UserLanguages
		{
			get { return request.UserLanguages; }
		}


		public string UserHostAddress
		{
			get { return request.UserHostAddress; }
		}

		public String ReadCookie( String name )
		{
			HttpCookie cookie = request.Cookies[ name ];
			if ( cookie == null )
			{
				return null;
			}
			return cookie.Value;
		}

		public void ValidateInput()
		{
			request.ValidateInput();
		}
	}
}
