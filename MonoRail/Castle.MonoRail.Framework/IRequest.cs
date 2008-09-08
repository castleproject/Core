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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using System.Web;
	using Castle.Components.Binder;

	/// <summary>
	/// Defines where the parameters should be obtained from
	/// </summary>
	public enum ParamStore
	{
		/// <summary>
		/// Query string
		/// </summary>
		QueryString,
		/// <summary>
		/// Only from the Form
		/// </summary>
		Form,
		/// <summary>
		/// From QueryString, Form and Environment variables.
		/// </summary>
		Params
	}

	/// <summary>
	/// Represents the request data
	/// </summary>
	public interface IRequest
	{
		/// <summary>
		/// Gets the accept header.
		/// </summary>
		/// <value>The accept header.</value>
		string AcceptHeader { get; }

		/// <summary>
		/// Gets a string array of client-supported MIME accept types.
		/// </summary>
		/// <value>A string array of client-supported MIME accept types.</value>
		string[] AcceptTypes { get; }

		/// <summary>
		/// Gets the request type (GET, POST, etc)
		/// </summary>
		[Obsolete("Use the property HttpMethod instead")]
		String RequestType { get; }

		/// <summary>
		/// Gets additional path information for 
		/// a resource with a URL extension.
		/// </summary>
		/// <value>The path info.</value>
		String PathInfo { get; }

		/// <summary>
		/// Gets the raw URL.
		/// </summary>
		/// <value>The raw URL.</value>
		[Obsolete("Use the property Url instead")]
		String RawUrl { get; }

		/// <summary>
		/// Gets the URI.
		/// </summary>
		/// <value>The URI.</value>
		Uri Uri { get; }

		/// <summary>
		/// Gets the request URL.
		/// </summary>
		String Url { get; }

		/// <summary>
		/// Gets the referring URL.
		/// </summary>
		String UrlReferrer { get; }

		/// <summary>
		/// Gets the <see cref="HttpPostedFile"/> per key.
		/// </summary>
		IDictionary Files { get; }

		/// <summary>
		/// Gets the params which accumulates headers, post, querystring and cookies.
		/// </summary>
		/// <value>The params.</value>
		NameValueCollection Params { get; }

		/// <summary>
		/// Gets the query string.
		/// </summary>
		/// <value>The query string.</value>
		NameValueCollection QueryString { get; }

		/// <summary>
		/// Gets the form.
		/// </summary>
		/// <value>The form.</value>
		NameValueCollection Form { get; }

		/// <summary>
		/// Gets the Http headers.
		/// </summary>
		/// <value>The Http headers.</value>
		NameValueCollection Headers { get; }

		/// <summary>
		/// Indexer to access <see cref="Params"/> entries.
		/// </summary>
		/// <value></value>
		string this[string name] { get; }
		
		/// <summary>
		/// Gets a value indicating whether this requeest is from a local address.
		/// </summary>
		/// <value><c>true</c> if this instance is local; otherwise, <c>false</c>.</value>
		bool IsLocal { get; }

		/// <summary>
		/// Gets the HTTP method.
		/// </summary>
		/// <value>The HTTP method.</value>
		String HttpMethod { get; }

		/// <summary>
		/// Gets the file path.
		/// </summary>
		/// <value>The file path.</value>
		String FilePath { get; }

//		/// <summary>
//		/// Reads the request data as a byte array.
//		/// </summary>
//		/// <param name="count">How many bytes.</param>
//		/// <returns></returns>
//		byte[] BinaryRead(int count);

		/// <summary>
		/// Reads the cookie.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <returns></returns>
		String ReadCookie(String name);

		/// <summary>
		/// Gets the user agent string of the client browser.
		/// </summary>
		/// <value>The agent string of the client browser.</value>
		String UserAgent { get; }

		/// <summary>
		/// Gets the user languages.
		/// </summary>
		/// <value>The user languages.</value>
		String[] UserLanguages { get; }

		/// <summary>
		/// Gets the IP host address of the remote client. 
		/// </summary>
		/// <value>The IP address of the remote client.</value>
		string UserHostAddress { get; }

		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="Controller.Params"/>
		/// </summary>
		CompositeNode ParamsNode { get; }

		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="IRequest.Form"/>
		/// </summary>
		CompositeNode FormNode { get; }

		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="IRequest.QueryString"/>
		/// </summary>
		/// <value>The query string node.</value>
		CompositeNode QueryStringNode { get; }

		/// <summary>
		/// Gets the contents of the incoming HTTP entity body.
		/// </summary>
		/// <value></value>
		Stream InputStream { get; }

		/// <summary>
		/// Gets or sets the MIME content type of the incoming request.
		/// </summary>
		/// <value></value>
		string ContentType { get; set; }

		/// <summary>
		/// Obtains the params node.
		/// </summary>
		/// <param name="from">From.</param>
		/// <returns></returns>
		CompositeNode ObtainParamsNode(ParamStore from);

		/// <summary>
		/// Gets the size of the input stream (if you plan to read it directly as a byte[]).
		/// </summary>
		/// <value>The size of the input stream.</value>
		int InputStreamSize { get; }

		/// <summary>
		/// Validates the input.
		/// </summary>
		void ValidateInput();
	}
}
