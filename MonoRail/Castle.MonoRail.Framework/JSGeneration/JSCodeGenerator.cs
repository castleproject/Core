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

namespace Castle.MonoRail.Framework.JSGeneration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;
	using Helpers;
	using Services;

	/// <summary>
	/// Pendent
	/// </summary>
	public class JSCodeGenerator : IJSCodeGenerator
	{
		private readonly Dictionary<string, object> extensions =
			new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

		private readonly StringBuilder lines = new StringBuilder();
		private IJSGenerator generator;
		private IServerUtility serverUtility;
		private IViewEngineManager viewEngineManager;
		private IEngineContext engineContext;
		private IController controller;
		private IControllerContext context;
		private IUrlBuilder urlBuilder;

		/// <summary>
		/// Initializes a new instance of the <see cref="JSCodeGenerator"/> class.
		/// </summary>
		public JSCodeGenerator()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JSCodeGenerator"/> class.
		/// </summary>
		/// <param name="serverUtility">The server utility.</param>
		/// <param name="viewEngineManager">The view engine manager.</param>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		/// <param name="urlBuilder">The URL builder.</param>
		public JSCodeGenerator(IServerUtility serverUtility, IViewEngineManager viewEngineManager,
		                       IEngineContext engineContext, IController controller, IControllerContext context,
		                       IUrlBuilder urlBuilder)
		{
			this.serverUtility = serverUtility;
			this.viewEngineManager = viewEngineManager;
			this.engineContext = engineContext;
			this.controller = controller;
			this.context = context;
			this.urlBuilder = urlBuilder;
		}

		/// <summary>
		/// Gets the main JS generator.
		/// </summary>
		/// <value>The JS generator.</value>
		public IJSGenerator JSGenerator
		{
			get { return generator; }
			set { generator = value; }
		}

		/// <summary>
		/// Gets or sets the URL helper.
		/// </summary>
		/// <value>The URL helper.</value>
		public IUrlBuilder UrlBuilder
		{
			get { return urlBuilder; }
			set { urlBuilder = value; }
		}

		/// <summary>
		/// Gets or sets the server utility.
		/// </summary>
		/// <value>The server utility.</value>
		public IServerUtility ServerUtility
		{
			get { return serverUtility; }
			set { serverUtility = value; }
		}

		/// <summary>
		/// Gets or sets the engine context.
		/// </summary>
		/// <value>The engine context.</value>
		public IEngineContext EngineContext
		{
			get { return engineContext; }
			set { engineContext = value; }
		}

		/// <summary>
		/// Gets the extensions.
		/// </summary>
		/// <value>The extensions.</value>
		public IDictionary<string, object> Extensions
		{
			get { return extensions; }
		}

		/// <summary>
		/// Calls the specified function with the optional arguments.
		/// </summary>
		/// <param name="function">The function name.</param>
		/// <param name="args">The arguments.</param>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.call('myJsFunctionAlreadyDeclared', '10', "'message'", $somethingfrompropertybag, $anothermessage.to_squote)
		/// </code>
		/// 	<para>
		/// Which outputs:
		/// </para>
		/// 	<code>
		/// myJsFunctionAlreadyDeclared(10, 'message', 1001, 'who let the dogs out?')
		/// </code>
		/// </example>
		public void Call(object function, params object[] args)
		{
			if (String.IsNullOrEmpty(function.ToString()))
			{
				throw new ArgumentException("function cannot be null or an empty string.", "function");
			}

			Record(function + "(" + BuildJSArguments(args) + ")");
		}

		/// <summary>
		/// Writes the content specified to the generator instance
		/// </summary>
		/// <param name="content">The content.</param>
		/// <remarks>
		/// This is for advanced scenarios and for the infrastructure. Usually not useful.
		/// </remarks>
		public void Write(string content)
		{
			lines.Append(content);
		}

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
		public object Render(object renderOptions)
		{
			if (renderOptions == null)
			{
				throw new ArgumentNullException("renderOptions",
				                                "renderOptions cannot be null. Must be a string or a dictionary");
			}
			if (renderOptions is IDictionary)
			{
				IDictionary options = (IDictionary) renderOptions;

				String partialName = (String) options["partial"];

				if (partialName == null)
				{
					throw new ArgumentNullException("renderOptions",
					                                "renderOptions, as a dictionary, must have a 'partial' " +
					                                "entry with the template name to render");
				}

				try
				{
					StringWriter writer = new StringWriter();

					viewEngineManager.ProcessPartial(partialName, writer, engineContext, controller, context);

					// Ideally we would call (less overhead and safer)
					// viewEngineManager.ProcessPartial(partialName, writer, engineContext, parameters);

					renderOptions = writer.ToString();
				}
				catch(Exception ex)
				{
					throw new MonoRailException("Could not process partial " + partialName, ex);
				}
			}

			return AbstractHelper.Quote(JsEscape(renderOptions.ToString()));
		}

		/// <summary>
		/// Writes the content specified to the generator instance
		/// </summary>
		/// <param name="content">The content.</param>
		/// <remarks>
		/// This is for advanced scenarios and for the infrastructure. Usually not useful.
		/// </remarks>
		public void AppendLine(string content)
		{
			Record(content);
		}

		/// <summary>
		/// Gets the js lines.
		/// </summary>
		/// <value>The js lines.</value>
		public StringBuilder Lines
		{
			get { return lines; }
		}

		/// <summary>
		/// Records the specified line on the generator.
		/// </summary>
		/// <param name="line">The line.</param>
		public void Record(string line)
		{
			Lines.AppendFormat("{0};\r\n", line);
		}

		/// <summary>
		/// Builds the JS arguments.
		/// </summary>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public string BuildJSArguments(object[] args)
		{
			if (args == null || args.Length == 0) return String.Empty;

			StringBuilder tempBuffer = new StringBuilder();

			bool comma = false;

			foreach(object arg in args)
			{
				if (comma) tempBuffer.Append(',');

				tempBuffer.Append(arg);

				if (!comma) comma = true;
			}

			return tempBuffer.ToString();
		}

		/// <summary>
		/// Replaces the tail by period.
		/// </summary>
		public void ReplaceTailByPeriod()
		{
			int len = Lines.Length;

			if (len > 3)
			{
				RemoveTail();
				Lines.Append('.');
			}
		}

		/// <summary>
		/// Removes the tail.
		/// </summary>
		public void RemoveTail()
		{
			int len = Lines.Length;

			if (len > 3)
			{
				if (Lines[len - 3] == ';')
				{
					Lines.Length = len - 3;
				}
			}
		}

		/// <summary>
		/// Generates the final js code.
		/// </summary>
		/// <returns></returns>
		public string GenerateFinalJsCode()
		{
			return "try \n" +
			       "{\n" + lines +
			       "}\n" +
			       "catch(e)\n" +
			       "{\n" +
			       "alert('JS error ' + e.toString());\n" +
			       "}";
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return GenerateFinalJsCode();
		}

		#region Internal and private methods

		/// <summary>
		/// Escapes the content with C style escapes
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns></returns>
		protected static string JsEscape(string content)
		{
			// This means: replace all variations of line breaks by a \n
			content = Regex.Replace(content, "(\r\n)|(\r)|(\n)", "\\n", RegexOptions.Multiline);
			// This is a lookbehind. It means: replace all " -- that are not preceded by \ -- by \"
			content = Regex.Replace(content, "(?<!\\\\)\"", "\\\"", RegexOptions.Multiline);
			return content;
		}

		/// <summary>
		/// Escapes the content with C style escapes (favoring single quotes)
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns></returns>
		protected static string JsEscapeWithSQuotes(string content)
		{
			// This replaces all ' references by \'
			return Regex.Replace(JsEscape(content), "(\')", "\\'", RegexOptions.Multiline);
		}

		#endregion
	}
}