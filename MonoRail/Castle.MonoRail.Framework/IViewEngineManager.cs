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
	/// Sits between the controller and the view engines (multiples)
	/// to decide which view engine should render a specific content
	/// </summary>
	public interface IViewEngineManager
	{
		/// <summary>
		/// Creates the JS code generator info. Temporarily on IViewEngineManager
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns></returns>
		JSCodeGeneratorInfo CreateJSCodeGeneratorInfo(IEngineContext engineContext, IController controller,
		                                              IControllerContext controllerContext);

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <returns><c>true</c> if it exists</returns>
		bool HasTemplate(String templateName);

		///<summary>
		/// Processes the view - using the templateName 
		/// to obtain the correct template
		/// and writes the results to the System.TextWriter. 
		/// </summary>
		void Process(string templateName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext);

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template
		/// and writes the results to the System.TextWriter.
		/// </summary>
		void Process(string templateName, string layoutName, TextWriter output, IDictionary<string,object> parameters);

		/// <summary>
		/// Processes a partial view using the partialName
		/// to obtain the correct template and writes the
		/// results to the System.TextWriter.
		/// </summary>
		/// <param name="partialName">The partial name.</param>
		/// <param name="output">The output.</param>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		void ProcessPartial(String partialName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext);

//		Holding it a little more as it would be a breaking change (meaning all partials view would break)
// 
//		/// <summary>
//		/// Processes a partial view using the partialName
//		/// to obtain the correct template and writes the
//		/// results to the System.TextWriter.
//		/// </summary>
//		/// <param name="partialName">The partial name.</param>
//		/// <param name="output">The output.</param>
//		/// <param name="context">The context.</param>
//		/// <param name="parameters">The parameters.</param>
//		void ProcessPartial(String partialName, TextWriter output, IEngineContext context, IDictionary<string, object> parameters);

		/// <summary>
		/// Wraps the specified content in the layout using 
		/// the context to output the result.
		/// </summary>
		void RenderStaticWithinLayout(String contents, IEngineContext context, IController controller, IControllerContext controllerContext);
	}
}
