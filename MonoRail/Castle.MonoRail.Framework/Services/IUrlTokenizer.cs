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

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IUrlTokenizer
	{
		/// <summary>
		/// Tokenizes the URL.
		/// </summary>
		/// <param name="filePath">The raw URL without query string or path info.</param>
		/// <param name="pathInfo">The path info.</param>
		/// <param name="uri">The URI.</param>
		/// <param name="isLocal">if set to <c>true</c> [is local].</param>
		/// <param name="appVirtualDir">Virtual directory</param>
		/// <returns></returns>
		UrlInfo TokenizeUrl(string filePath, string pathInfo, Uri uri, bool isLocal, string appVirtualDir);

		/// <summary>
		/// Adds a default url rule. 
		/// </summary>
		/// <param name="url">The simple url (like index.castle).</param>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		void AddDefaultRule(string url, string area, string controller, string action);
	}
}
