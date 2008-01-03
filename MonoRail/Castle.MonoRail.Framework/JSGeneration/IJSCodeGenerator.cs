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

namespace Castle.MonoRail.Framework.JSGeneration
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Services;

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IJSCodeGenerator
	{
		/// <summary>
		/// Gets the main JS generator.
		/// </summary>
		/// <value>The JS generator.</value>
		IJSGenerator JSGenerator { get; set; }

		/// <summary>
		/// Gets or sets the URL helper.
		/// </summary>
		/// <value>The URL helper.</value>
		IUrlBuilder UrlBuilder { get; set; }

		/// <summary>
		/// Gets or sets the server utility.
		/// </summary>
		/// <value>The server utility.</value>
		IServerUtility ServerUtility { get; set; }

		/// <summary>
		/// Gets or sets the engine context.
		/// </summary>
		/// <value>The engine context.</value>
		IEngineContext EngineContext { get; set; }

		/// <summary>
		/// Gets the extensions.
		/// </summary>
		/// <value>The extensions.</value>
		IDictionary<string, object> Extensions { get; }

		/// <summary>
		/// Calls the specified function with the optional arguments.
		/// </summary>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.call('myJsFunctionAlreadyDeclared', '10', "'message'", $somethingfrompropertybag, $anothermessage.to_squote)
		/// </code>
		/// 
		/// <para>
		/// Which outputs:
		/// </para>
		/// 
		/// <code>
		///   myJsFunctionAlreadyDeclared(10, 'message', 1001, 'who let the dogs out?')
		/// </code>
		/// 
		/// </example>
		/// 
		/// <param name="function">The function name.</param>
		/// <param name="args">The arguments.</param>
		void Call(object function, params object[] args);

		/// <summary>
		/// Writes the content specified to the generator instance
		/// </summary>
		/// 
		/// <remarks>
		/// This is for advanced scenarios and for the infrastructure. Usually not useful.
		/// </remarks>
		/// 
		/// <param name="content">The content.</param>
		void Write(String content);

		/// <summary>
		/// Outputs the content using the renderOptions approach.
		/// <para>
		/// If the renderOptions is a string, the content is escaped and quoted.
		/// </para>
		/// 	<para>
		/// If the renderOptions is a dictionary, we extract the key <c>partial</c>
		/// and evaluate the template it points to. The content is escaped and quoted.
		/// </para>
		/// </summary>
		/// <param name="renderOptions">The render options.</param>
		/// <returns></returns>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.Call('myJsFunction', $page.render("%{partial='shared/newmessage.vm'}") )
		/// </code>
		/// 	<para>
		/// Which outputs:
		/// </para>
		/// 	<code>
		/// myJsFunction('the content from the newmessage partial view template')
		/// </code>
		/// </example>
		object Render(object renderOptions);

		/// <summary>
		/// Writes the content specified to the generator instance
		/// </summary>
		/// 
		/// <remarks>
		/// This is for advanced scenarios and for the infrastructure. Usually not useful.
		/// </remarks>
		/// <param name="content">The content.</param>
		void AppendLine(String content);

		/// <summary>
		/// Records the specified line.
		/// </summary>
		/// <param name="line">The line.</param>
		void Record(string line);

		/// <summary>
		/// Builds the JS arguments.
		/// </summary>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		string BuildJSArguments(object[] args);

		/// <summary>
		/// Replaces the tail from the current line (usually a ';') by period.
		/// </summary>
		void ReplaceTailByPeriod();

		/// <summary>
		/// Removes the tail from the current line (usually a ';')
		/// </summary>
		void RemoveTail();

		/// <summary>
		/// Gets the js lines.
		/// </summary>
		/// <value>The js lines.</value>
		StringBuilder Lines { get; }

		/// <summary>
		/// Dump the operations recorded so far as javascript code. 
		/// </summary>
		/// 
		/// <returns></returns>
		string ToString();
	}
}
