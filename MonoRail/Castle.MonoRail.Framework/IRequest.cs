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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Web;

	/// <summary>
	/// Represents the request data
	/// </summary>
	public interface IRequest
	{
		/// <summary>
		/// Gets the Http headers.
		/// </summary>
		/// <value>The Http headers.</value>
		NameValueCollection Headers { get; }

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
		/// Gets a value indicating whether this requeest is from a local address.
		/// </summary>
		/// <value><c>true</c> if this instance is local; otherwise, <c>false</c>.</value>
		bool IsLocal { get; }

		/// <summary>
		/// Gets the raw URL.
		/// </summary>
		/// <value>The raw URL.</value>
		String RawUrl { get; }

		/// <summary>
		/// Gets the URI.
		/// </summary>
		/// <value>The URI.</value>
		Uri Uri { get; }

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

		/// <summary>
		/// Reads the request data as a byte array.
		/// </summary>
		/// <param name="count">How many bytes.</param>
		/// <returns></returns>
		byte[] BinaryRead(int count);

		/// <summary>
		/// Gets the param with the specified key.
		/// </summary>
		/// <value></value>
		String this [String key] { get; }

		/// <summary>
		/// Reads the cookie.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <returns></returns>
		String ReadCookie( String name );

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
		/// Validates the input.
		/// </summary>
		void ValidateInput();
	}
}
