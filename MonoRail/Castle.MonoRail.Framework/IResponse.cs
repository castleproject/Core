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
	using System.Web;

	public interface IResponse
	{
		int StatusCode { get; set; }

		String ContentType { get; set; }

		/// <summary>
		/// Gets the caching policy (expiration time, privacy, 
		/// vary clauses) of a Web page.
		/// </summary>
		HttpCachePolicy CachePolicy  { get; }

		/// <summary>
		/// Sets the Cache-Control HTTP header to Public or Private.
		/// </summary>
		String CacheControlHeader { get; set; } 

		/// <summary>
		/// Gets or sets the HTTP character set of the output stream.
		/// </summary>
		String Charset { get; set; } 

		void AppendHeader(String name, String value);

		System.IO.TextWriter Output { get; }
		
		System.IO.Stream OutputStream { get; }

		void Write(String s);

		void Write(object obj);

		void Write(char ch);

		void Write(char[] buffer, int index, int count);

		void WriteFile(String fileName);

		void Redirect(String controller, String action);

		void Redirect(String area, String controller, String action);

		void Redirect(String url);

		void Redirect(String url, bool endProcess);

		void CreateCookie(String name, String value);
		
		void CreateCookie(String name, String value, DateTime expiration);
	}
}
