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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections.Specialized;

	public interface IServerUtility
	{
		/// <summary>
		/// HTML encodes a string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text string to HTML encode.</param>
		/// <returns>The HTML encoded text.</returns>
		String HtmlEncode(String content);

		/// <summary>
		/// URL encodes a string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text to URL encode.</param>
		/// <returns>The URL encoded text.</returns>
		String UrlEncode(String content);

		/// <summary>
		/// URL encodes the path portion of a URL string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text to URL encode.</param>
		/// <returns>The URL encoded text.</returns>
		String UrlPathEncode(String content);

		/// <summary>
		/// Escapes JavaScript with Url encoding and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text to URL encode and escape JavaScript within.</param>
		/// <returns>The URL encoded and JavaScript escaped text.</returns>
		String JavaScriptEscape(String content);

		/// <summary>
		/// Build an encoded QueryString
		/// </summary>
		/// <param name="queryString">A NameValueCollection with the values for the QueryParams</param>
		/// <returns>The QueryString encoded.</returns>
		String BuildWebParams(NameValueCollection queryParams);
	}
}
