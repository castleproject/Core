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

namespace Castle.MonoRail.Framework.Test
{
	using System;
	using System.IO;
	using System.Web;

	/// <summary>
	/// Represents a mock implementation of <see cref="IServerUtility"/> for unit test purposes.
	/// </summary>
	public class StubServerUtility : IServerUtility
	{
		/// <summary>
		/// Returns the physical path for the
		/// specified virtual path.
		/// </summary>
		/// <param name="virtualPath">The virtual path.</param>
		/// <returns>The mapped path</returns>
		public virtual string MapPath(string virtualPath)
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, virtualPath);
		}

		/// <summary>
		/// HTML encodes a string and returns the encoded string.
		/// </summary>
		/// <param name="content">The text string to HTML encode.</param>
		/// <returns>The HTML encoded text.</returns>
		public virtual string HtmlEncode(string content)
		{
			return HttpUtility.HtmlEncode(content);
		}

		/// <summary>
		/// URL encodes a string and returns the encoded string.
		/// </summary>
		/// <param name="content">The text to URL encode.</param>
		/// <returns>The URL encoded text.</returns>
		public virtual string UrlEncode(string content)
		{
			return HttpUtility.UrlEncode(content);
		}

		/// <summary>
		/// URL decodes a string and returns the decoded string.
		/// </summary>
		/// <param name="content">The text to URL decode.</param>
		/// <returns>The URL decoded text.</returns>
		public virtual string UrlDecode(string content)
		{
			return HttpUtility.UrlDecode(content);
		}

		/// <summary>
		/// URL encodes the path portion of a URL string and returns the encoded string.
		/// </summary>
		/// <param name="content">The text to URL encode.</param>
		/// <returns>The URL encoded text.</returns>
		public virtual string UrlPathEncode(string content)
		{
			return HttpUtility.UrlPathEncode(content);
		}

		/// <summary>
		/// Escapes JavaScript with Url encoding and returns the encoded string.
		/// </summary>
		/// <param name="content">The text to URL encode and escape JavaScript within.</param>
		/// <returns>
		/// The URL encoded and JavaScript escaped text.
		/// </returns>
		/// <remarks>
		/// Converts quotes, single quotes and CR/LFs to their representation as an escape character.
		/// </remarks>
		public virtual string JavaScriptEscape(string content)
		{
			return content.
				Replace("\"", "\\\"").
				Replace("\r", "").
				Replace("\n", "\\n").
				Replace("'", "\\'");
		}
	}
}