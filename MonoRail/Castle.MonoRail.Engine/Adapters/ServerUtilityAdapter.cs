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

namespace Castle.MonoRail.Engine.Adapters
{
	using System;
	using System.Web;

	using Castle.MonoRail.Framework;
	
	public class ServerUtilityAdapter : IServerUtility
	{
		private readonly HttpServerUtility server;

		public ServerUtilityAdapter(HttpServerUtility server)
		{
			this.server = server;
		}

		/// <summary>
		/// HTML encodes a string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text string to HTML encode.</param>
		/// <returns>The HTML encoded text.</returns>
		public String HtmlEncode(String content)
		{
			return server.HtmlEncode(content);
		}

		/// <summary>
		/// Escapes JavaScript with Url encoding and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text to URL encode and escape JavaScript within.</param>
		/// <returns>The URL encoded and JavaScript escaped text.</returns>
		public String JavaScriptEscape(String content)
		{
			return server.UrlEncode(content).Replace("'", @"\'"); 
		}

		/// <summary>
		/// URL encodes a string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text to URL encode.</param>
		/// <returns>The URL encoded text.</returns>
		public String UrlEncode(String content)
		{
			return server.UrlEncode(content);
		}
		
		/// <summary>
		/// URL encodes the path portion of a URL string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text to URL encode.</param>
		/// <returns>The URL encoded text.</returns>
		public String UrlPathEncode(String content)
		{
			return server.UrlPathEncode(content);
		}
	}
}
