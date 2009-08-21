// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Services
{
	using System;

	/// <summary>
	/// Standard implementation of <see cref="IScriptBuilder"/>.
	/// </summary>
	public class DefaultScriptBuilder : IScriptBuilder
	{
		/// <summary>
		/// Gets a value indicating whether this <see cref="IScriptBuilder"/> will concatenate
		/// script files into a single file.
		/// </summary>
		/// <value><c>true</c> if concatenation should occur; otherwise, <c>false</c>.</value>
		public virtual bool Concatenate
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="IScriptBuilder"/> will minify
		/// script files.
		/// </summary>
		/// <value><c>true</c> if compression should occur; otherwise, <c>false</c>.</value>
		public virtual bool Minify
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether JavaScript should be optimized if supported by the 
		/// script builder.
		/// </summary>
		/// <value><c>true</c> if JavaScript should be optimized; otherwise, <c>false</c>.</value>
		public virtual bool OptimizeJavaScript
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether JavaScript should be obfuscated.
		/// </summary>
		/// <value><c>true</c> if JavaScript should be obfuscated; otherwise, <c>false</c>.</value>
		public virtual bool ObfuscateJavaScript
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the width of the CSS columns to use when minifying CSS.  If the value is null, then
		/// no column width will be set.
		/// </summary>
		/// <value>The width of the CSS column.</value>
		public virtual int? CssColumnWidth
		{
			get { return null; }
		}

		/// <summary>
		/// the position where a line feed is appened when the next semicolon is reached -1 (never add a line break).
		/// </summary>
		public int LineBreakPosition
		{
			get { return -1; }
		}

		/// <summary>
		/// Compresses Javascript.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <returns></returns>
		public virtual string CompressJavascript(string script)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Compresses CSS.
		/// </summary>
		/// <param name="css">The CSS.</param>
		/// <returns></returns>
		public virtual string CompressCSS(string css)
		{
			throw new NotSupportedException();
		}
	}
}
