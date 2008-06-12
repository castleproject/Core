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

using Castle.MonoRail.Framework;

namespace Castle.MonoRail.Framework.Adapters
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using System.Web;
	using Castle.Components.Binder;

	/// <summary>
	/// This class adapts the <c>HttpRequest</c> to a MonoRail <c>IRequest</c>.
	/// </summary>
	public class RequestAdapter : IRequest
	{
		private TreeBuilder treeBuilder = new TreeBuilder();
		private HttpRequest request;
		private FileDictionaryAdapter files;

		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="Controller.Params"/>
		/// </summary>
		protected CompositeNode paramsCompositeNode;

		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="IRequest.Form"/>
		/// </summary>
		protected CompositeNode formCompositeNode;

		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="IRequest.QueryString"/>
		/// </summary>
		protected CompositeNode queryStringCompositeNode;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="RequestAdapter"/> class.
		/// </summary>
		/// <param name="request">The request.</param>
		public RequestAdapter(HttpRequest request)
		{
			this.request = request;
		}

		/// <summary>
		/// Gets the request URL.
		/// </summary>
		/// <value></value>
		public string Url
		{
			get { return RawUrl; }
		}

		/// <summary>
		/// Gets the accept header.
		/// </summary>
		/// <value>The accept header.</value>
		public string AcceptHeader
		{
			get { return request.Headers["Accept"]; }
		}

		/// <summary>
		/// Gets a string array of client-supported MIME accept types.
		/// </summary>
		/// <value>A string array of client-supported MIME accept types.</value>
		public string[] AcceptTypes
		{
			get { return request.AcceptTypes; }
		}

		/// <summary>
		/// Gets the referring URL.
		/// </summary>
		/// <value></value>
		public String UrlReferrer
		{
			get
			{
				Uri referrer = request.UrlReferrer;

				if (referrer != null)
				{
					return referrer.ToString();
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this requeest is from a local address.
		/// </summary>
		/// <value><c>true</c> if this instance is local; otherwise, <c>false</c>.</value>
		public bool IsLocal
		{
			get { return request.Url.IsLoopback; }
		}

		/// <summary>
		/// Gets the contents of the incoming HTTP entity body.
		/// </summary>
		/// <value></value>
		public Stream InputStream
		{
			get { return request.InputStream; }
		}

		/// <summary>
		/// Gets the size of the input stream (if you plan to read it directly as a byte[]).
		/// </summary>
		/// <value>The size of the input stream.</value>
		public int InputStreamSize
		{
			get { return request.TotalBytes; }
		}

		/// <summary>
		/// Gets or sets the MIME content type of the incoming request.
		/// </summary>
		/// <value></value>
		public string ContentType
		{
			get { return request.ContentType; }
			set { request.ContentType = value; }
		}
		
		/// <summary>
		/// Gets the request type (GET, POST, etc)
		/// </summary>
		/// <value></value>
		public string RequestType
		{
			get { return HttpMethod; }
		}

		/// <summary>
		/// Gets the HTTP method (GET, POST, etc).
		/// </summary>
		/// <value>The HTTP method.</value>
		public string HttpMethod
		{
			get { return request.HttpMethod; }
		}

		/// <summary>
		/// Gets the URI.
		/// </summary>
		/// <value>The URI.</value>
		public Uri Uri
		{
			get { return request.Url; }
		}

		/// <summary>
		/// Gets additional path information for 
		/// a resource with a URL extension.
		/// </summary>
		/// <value>The path info.</value>
		public String PathInfo
		{
			get { return request.PathInfo; }
		}

		/// <summary>
		/// Gets the raw URL.
		/// </summary>
		/// <value>The raw URL.</value>
		public String RawUrl
		{
			get { return request.RawUrl; }
		}

		/// <summary>
		/// Gets the file path.
		/// </summary>
		/// <value>The file path.</value>
		public String FilePath
		{
			get { return request.FilePath; }
		}

		/// <summary>
		/// Gets the Http headers.
		/// </summary>
		/// <value>The Http headers.</value>
		public NameValueCollection Headers
		{
			get { return request.Headers; }
		}

		/// <summary>
		/// Gets the params which accumulates headers, post, querystring and cookies.
		/// </summary>
		/// <value>The params.</value>
		public NameValueCollection Params
		{
			get { return request.Params; }
		}

		/// <summary>
		/// Gets the query string.
		/// </summary>
		/// <value>The query string.</value>
		public NameValueCollection QueryString
		{
			get { return request.QueryString; }
		}

		/// <summary>
		/// Gets the form.
		/// </summary>
		/// <value>The form.</value>
		public NameValueCollection Form
		{
			get { return request.Form; }
		}

		/// <summary>
		/// Indexer to access <see cref="Params"/> entries.
		/// </summary>
		/// <value></value>
		public string this[string name]
		{
			get { return request[name]; }
		}

//		/// <summary>
//		/// Reads the request data as a byte array.
//		/// </summary>
//		/// <param name="count">How many bytes.</param>
//		/// <returns></returns>
//		public byte[] BinaryRead(int count)
//		{
//			return request.BinaryRead(count);
//		}


		/// <summary>
		/// Gets the <see cref="HttpPostedFile"/> per key.
		/// </summary>
		/// <value></value>
		public IDictionary Files
		{
			get
			{
				if (files == null)
				{
					files = new FileDictionaryAdapter(request.Files);
				}
				return files;
			}
		}

		/// <summary>
		/// Gets the user languages.
		/// </summary>
		/// <value>The user languages.</value>
		public String[] UserLanguages
		{
			get { return request.UserLanguages; }
		}

		/// <summary>
		/// Gets the IP host address of the remote client.
		/// </summary>
		/// <value>The IP address of the remote client.</value>
		public string UserHostAddress
		{
			get { return request.UserHostAddress; }
		}

		/// <summary>
		/// Reads the cookie.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <returns></returns>
		public String ReadCookie(String name)
		{
			HttpCookie cookie = request.Cookies[name];
			if (cookie == null)
			{
				return null;
			}
			return cookie.Value;
		}

		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="Controller.Params"/>
		/// </summary>
		public virtual CompositeNode ParamsNode
		{
			get
			{
				if (paramsCompositeNode == null)
				{
					paramsCompositeNode = treeBuilder.BuildSourceNode(Params);
					treeBuilder.PopulateTree(paramsCompositeNode, request.Files);
				}

				return paramsCompositeNode;
			}
		}

		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="IRequest.Form"/>
		/// </summary>
		public virtual CompositeNode FormNode
		{
			get
			{
				if (formCompositeNode == null)
				{
					formCompositeNode = treeBuilder.BuildSourceNode(Form);
					treeBuilder.PopulateTree(formCompositeNode, request.Files);
				}

				return formCompositeNode;
			}
		}

		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="IRequest.QueryString"/>
		/// </summary>
		public virtual CompositeNode QueryStringNode
		{
			get
			{
				if (queryStringCompositeNode == null)
				{
					queryStringCompositeNode = treeBuilder.BuildSourceNode(QueryString);
					treeBuilder.PopulateTree(queryStringCompositeNode, request.Files);
				}

				return queryStringCompositeNode;
			}
		}

		/// <summary>
		/// This method is for internal use only
		/// </summary>
		/// <param name="from"></param>
		/// <returns></returns>
		public CompositeNode ObtainParamsNode(ParamStore from)
		{
			switch (from)
			{
				case ParamStore.Form:
					return FormNode;
				case ParamStore.QueryString:
					return QueryStringNode;
				default:
					return ParamsNode;
			}
		}

		/// <summary>
		/// Validates the input.
		/// </summary>
		public void ValidateInput()
		{
			request.ValidateInput();
		}
	}
}