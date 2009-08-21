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
	/// <summary>
	/// Service for compressing Javascript and CSS files
	/// </summary>
	public interface IScriptBuilder
	{
		/// <summary>
		/// Gets a value indicating whether this <see cref="IScriptBuilder"/> will concatenate
		/// script files into a single file. By default, this is set to <c>true</c>.
		/// </summary>
		/// <value><c>true</c> if concatenation should occur; otherwise, <c>false</c>.</value>
		bool Concatenate { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="IScriptBuilder"/> will minify
		/// script files.  By default, this is set to <c>false</c>.
		/// </summary>
		/// <value><c>true</c> if compression should occur; otherwise, <c>false</c>.</value>
		bool Minify { get; }

		/// <summary>
		/// Gets a value indicating whether JavaScript should be optimized if supported by the 
		/// script builder. By default, this is set to <c>false</c>.
		/// </summary>
		/// <value><c>true</c> if JavaScript should be optimized; otherwise, <c>false</c>.</value>
		bool OptimizeJavaScript { get; }

		/// <summary>
		/// Gets a value indicating whether JavaScript should be obfuscated if supported by the 
		/// script builder. By default, this is set to <c>false</c>.
		/// </summary>
		/// <value><c>true</c> if JavaScript should be obfuscated; otherwise, <c>false</c>.</value>
		bool ObfuscateJavaScript { get; }

		/// <summary>
		/// Gets the width of the CSS columns to use when minifying CSS if supported by the script builder.
		/// If the value is null, then no column width will be set. By default, this is set to null.
		/// </summary>
		/// <value>The width of the CSS column.</value>
		int? CssColumnWidth { get; }

		/// <summary>
		/// the position where a line feed is appened when the next semicolon is reached -1 (never add a line break).
		/// </summary>
		int LineBreakPosition { get; }

		/// <summary>
		/// Compresses Javascript.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <returns></returns>
		string CompressJavascript(string script);

		/// <summary>
		/// Compresses CSS.
		/// </summary>
		/// <param name="css">The CSS.</param>
		/// <returns></returns>
		string CompressCSS(string css);
	}
}