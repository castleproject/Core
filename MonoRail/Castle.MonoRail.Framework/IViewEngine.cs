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
	using System.Collections.Generic;
	using System.IO;
	using JSGeneration;

	#region JSCodeGeneratorInfo

	/// <summary>
	/// Pendent
	/// </summary>
	public class JSCodeGeneratorInfo
	{
		private IJSCodeGenerator codeGenerator; 
		private IJSGenerator libraryGenerator;
		private object[] extensions;
		private object[] elementExtensions;

		/// <summary>
		/// Initializes a new instance of the <see cref="JSCodeGeneratorInfo"/> class.
		/// </summary>
		/// <param name="codeGenerator">The code generator.</param>
		/// <param name="libraryGenerator">The library generator.</param>
		/// <param name="extensions">The extensions.</param>
		/// <param name="elementExtensions">The element extensions.</param>
		public JSCodeGeneratorInfo(IJSCodeGenerator codeGenerator, IJSGenerator libraryGenerator, object[] extensions, object[] elementExtensions)
		{
			this.codeGenerator = codeGenerator;
			this.libraryGenerator = libraryGenerator;
			this.extensions = extensions;
			this.elementExtensions = elementExtensions;
		}

		/// <summary>
		/// Gets or sets the code generator.
		/// </summary>
		/// <value>The code generator.</value>
		public IJSCodeGenerator CodeGenerator
		{
			get { return codeGenerator; }
			set { codeGenerator = value; }
		}

		/// <summary>
		/// Gets or sets the library generator.
		/// </summary>
		/// <value>The library generator.</value>
		public IJSGenerator LibraryGenerator
		{
			get { return libraryGenerator; }
			set { libraryGenerator = value; }
		}

		/// <summary>
		/// Gets or sets the extensions.
		/// </summary>
		/// <value>The extensions.</value>
		public object[] Extensions
		{
			get { return extensions; }
			set { extensions = value; }
		}

		/// <summary>
		/// Gets or sets the element extensions.
		/// </summary>
		/// <value>The element extensions.</value>
		public object[] ElementExtensions
		{
			get { return elementExtensions; }
			set { elementExtensions = value; }
		}
	}

	#endregion

	/// <summary>
	/// Depicts the contract used by the engine
	/// to process views, in an independent manner.
	/// </summary>
	public interface IViewEngine
	{
		/// <summary>
		/// Gets a value indicating whether the view engine
		/// support the generation of JS.
		/// </summary>
		/// <value>
		/// <c>true</c> if JS generation is supported; otherwise, <c>false</c>.
		/// </value>
		bool SupportsJSGeneration { get; }

		/// <summary>
		/// Evaluates whether the specified template can be used to generate js.
		/// </summary>
		/// <returns><c>true</c> if it exists</returns>
		bool IsTemplateForJSGeneration(String templateName);

		/// <summary>
		/// Gets the JS generator view template file extension.
		/// </summary>
		/// <value>The JS generator file extension.</value>
		string JSGeneratorFileExtension { get; }

		/// <summary>
		/// Implementors should return a generator instance if
		/// the view engine supports JS generation.
		/// </summary>
		/// <param name="generatorInfo">The generator info.</param>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns>A JS generator instance</returns>
		object CreateJSGenerator(JSCodeGeneratorInfo generatorInfo, IEngineContext context, IController controller, IControllerContext controllerContext);

		/// <summary>
		/// Processes the js generation view template - using the templateName
		/// to obtain the correct template, and using the specified <see cref="TextWriter"/>
		/// to output the result.
		/// </summary>
		/// <param name="templateName">Name of the template.</param>
		/// <param name="output">The output.</param>
		/// <param name="generatorInfo">The generator info.</param>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		void GenerateJS(string templateName, TextWriter output, JSCodeGeneratorInfo generatorInfo, 
		                IEngineContext context, IController controller, IControllerContext controllerContext);

		/// <summary>
		/// Gets or sets a value indicating whether the view engine should set the
		/// content type to xhtml.
		/// </summary>
		/// <value><c>true</c> if the content type should be set to xhtml; otherwise, <c>false</c>.</value>
		bool XHtmlRendering { get; set; }

		/// <summary>
		/// Gets the view template file extension.
		/// </summary>
		/// <value>The view file extension.</value>
		string ViewFileExtension { get; }

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <returns><c>true</c> if it exists</returns>
		bool HasTemplate(string templateName);

		///<summary>
		/// Processes the view - using the templateName 
		/// to obtain the correct template
		/// and writes the results to the <see cref="TextWriter"/>. 
		/// No layout is applied!
		/// </summary>
		void Process(string templateName, TextWriter output, IEngineContext context, IController controller,
		             IControllerContext controllerContext);

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template
		/// and writes the results to the <see cref="TextWriter"/>.
		/// No layout is applied!
		/// </summary>
		void Process(string templateName, string layoutName, TextWriter output, IDictionary<string,object> parameters);

		/// <summary>
		/// Wraps the specified content in the layout using
		/// the context to output the result.
		/// </summary>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="contents">Static content to output within the layout</param>
		void RenderStaticWithinLayout(string contents, IEngineContext context, IController controller,
		                              IControllerContext controllerContext);

		/// <summary>
		/// Should process the specified partial. The partial name must contains
		/// the path relative to the views folder.
		/// </summary>
		/// <param name="partialName">The partial name.</param>
		/// <param name="output">The output.</param>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		void ProcessPartial(string partialName, TextWriter output, IEngineContext context, IController controller,
		                    IControllerContext controllerContext);

//		Holding it a little more as it would be a breaking change (meaning all partials view would break)
// 
//		/// <summary>
//		/// Should process the specified partial. The partial name must contains
//		/// the path relative to the views folder.
//		/// </summary>
//		/// <param name="partialName">The partial name.</param>
//		/// <param name="output">The output.</param>
//		/// <param name="context">The request context.</param>
//		/// <param name="parameters">The parameters.</param>
//		void ProcessPartial(String partialName, TextWriter output, IEngineContext context, IDictionary<string, object> parameters);
	}
}