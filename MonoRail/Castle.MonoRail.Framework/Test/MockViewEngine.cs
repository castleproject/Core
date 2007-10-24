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

namespace Castle.MonoRail.Framework.Test
{
	using System;
	using System.IO;

	/// <summary>
	/// Represents a mock implementation of <see cref="IViewEngine"/> for unit test purposes.
	/// </summary>
	public class MockViewEngine : IViewEngine
	{
		private string viewFileExtension;
		private string jsGeneratorFileExtension;
		private bool supportsJSGeneration;
		private bool xHtmlRendering;

		/// <summary>
		/// Initializes a new instance of the <see cref="MockViewEngine"/> class.
		/// </summary>
		/// <param name="viewFileExtension">The view file extension.</param>
		/// <param name="jsGeneratorFileExtension">The js generator file extension.</param>
		/// <param name="supportsJSGeneration">if set to <c>true</c> [supports JS generation].</param>
		/// <param name="xHtmlRendering">if set to <c>true</c> [x HTML rendering].</param>
		public MockViewEngine(string viewFileExtension, string jsGeneratorFileExtension, bool supportsJSGeneration, bool xHtmlRendering)
		{
			this.viewFileExtension = viewFileExtension;
			this.jsGeneratorFileExtension = jsGeneratorFileExtension;
			this.supportsJSGeneration = supportsJSGeneration;
			this.xHtmlRendering = xHtmlRendering;
		}

		/// <summary>
		/// Gets a value indicating whether the view engine
		/// support the generation of JS.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if JS generation is supported; otherwise, <c>false</c>.
		/// </value>
		public virtual bool SupportsJSGeneration
		{
			get { return supportsJSGeneration; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the view engine should set the
		/// content type to xhtml.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the content type should be set to xhtml; otherwise, <c>false</c>.
		/// </value>
		public bool XHtmlRendering
		{
			get { return xHtmlRendering; }
			set { xHtmlRendering = value; }
		}

		/// <summary>
		/// Gets the view template file extension.
		/// </summary>
		/// <value>The view file extension.</value>
		public virtual string ViewFileExtension
		{
			get { return viewFileExtension; }
		}

		/// <summary>
		/// Gets the JS generator view template file extension.
		/// </summary>
		/// <value>The JS generator file extension.</value>
		public virtual string JSGeneratorFileExtension
		{
			get { return jsGeneratorFileExtension; }
		}

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="templateName"></param>
		/// <returns><c>true</c> if it exists</returns>
		public virtual bool HasTemplate(string templateName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template,
		/// and using the context to output the result.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="templateName"></param>
		public virtual void Process(IRailsEngineContext context, IController controller, string templateName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template
		/// and writes the results to the <see cref="TextWriter"/>.
		/// No layout is applied!
		/// </summary>
		/// <param name="output"></param>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="templateName"></param>
		public virtual void Process(TextWriter output, IRailsEngineContext context, IController controller, string templateName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Implementors should return a generator instance if
		/// the view engine supports JS generation.
		/// </summary>
		/// <param name="context">The request context.</param>
		/// <returns>A JS generator instance</returns>
		public virtual object CreateJSGenerator(IRailsEngineContext context)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Processes the js generation view template - using the templateName
		/// to obtain the correct template, and using the context to output the result.
		/// </summary>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="templateName">Name of the template.</param>
		public virtual void GenerateJS(IRailsEngineContext context, IController controller, string templateName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Processes the js generation view template - using the templateName
		/// to obtain the correct template, and using the specified <see cref="TextWriter"/>
		/// to output the result.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="templateName">Name of the template.</param>
		public virtual void GenerateJS(TextWriter output, IRailsEngineContext context, IController controller, string templateName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Should process the specified partial. The partial name must contains
		/// the path relative to the views folder.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="partialName">The partial name.</param>
		public virtual void ProcessPartial(TextWriter output, IRailsEngineContext context, IController controller, string partialName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Wraps the specified content in the layout using
		/// the context to output the result.
		/// </summary>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="contents">Content to output</param>
		public virtual void ProcessContents(IRailsEngineContext context, IController controller, string contents)
		{
			throw new NotImplementedException();
		}
	}
}
