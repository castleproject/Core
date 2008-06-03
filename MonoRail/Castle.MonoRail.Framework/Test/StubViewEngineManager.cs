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
	using System.Collections.Generic;
	using System.IO;
	using Castle.MonoRail.Framework.JSGeneration.Prototype;
	using JSGeneration;

	/// <summary>
	/// Pendent
	/// </summary>
	public class StubViewEngineManager : IViewEngineManager
	{
		private readonly List<string> templates = new List<string>();
		private string templateRendered;
		private string partialRendered;
		private string contentWithinLayoutRendered;

		/// <summary>
		/// Gets the name of the template rendered by the controller.
		/// </summary>
		/// <value>The template rendered.</value>
		public string TemplateRendered
		{
			get { return templateRendered; }
		}

		/// <summary>
		/// Gets the name of the partial template rendered.
		/// </summary>
		/// <value>The partial rendered.</value>
		public string PartialRendered
		{
			get { return partialRendered; }
		}

		/// <summary>
		/// Gets the static content rendered from the controller.
		/// </summary>
		/// <value>The content within layout rendered.</value>
		public string ContentWithinLayoutRendered
		{
			get { return contentWithinLayoutRendered; }
		}

		/// <summary>
		/// Registers the template.
		/// </summary>
		/// <param name="template">The template.</param>
		public void RegisterTemplate(string template)
		{
			templates.Add(template);
		}

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="templateName"></param>
		/// <returns><c>true</c> if it exists</returns>
		public bool HasTemplate(string templateName)
		{
			return templates.Exists(
				delegate(string item) { return item.Equals(templateName, StringComparison.InvariantCultureIgnoreCase); });
		}

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template
		/// and writes the results to the System.TextWriter.
		/// <para>
		/// Please note that no layout is applied
		/// </para>
		/// </summary>
		public void Process(string templateName, TextWriter output, IEngineContext context, IController controller,
		                    IControllerContext controllerContext)
		{
			templateRendered = templateName;
		}

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template
		/// and writes the results to the System.TextWriter.
		/// </summary>
		public void Process(string templateName, string layoutName, TextWriter output, IDictionary<string, object> parameters)
		{
			templateRendered = templateName;
		}

		/// <summary>
		/// Processes a partial view using the partialName
		/// to obtain the correct template and writes the
		/// results to the System.TextWriter.
		/// </summary>
		public void ProcessPartial(string partialName, TextWriter output, IEngineContext context, IController controller,
		                           IControllerContext controllerContext)
		{
			partialRendered = partialName;
		}

		/// <summary>
		/// Wraps the specified content in the layout using
		/// the context to output the result.
		/// </summary>
		public void RenderStaticWithinLayout(string contents, IEngineContext context, IController controller,
		                                     IControllerContext controllerContext)
		{
			contentWithinLayoutRendered = contents;
		}

		/// <summary>
		/// Creates the JS code generator info. Temporarily on IViewEngineManager
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns></returns>
		public JSCodeGeneratorInfo CreateJSCodeGeneratorInfo(IEngineContext engineContext, IController controller,
		                                                     IControllerContext controllerContext)
		{
			JSCodeGenerator codeGen = new JSCodeGenerator();

			return new JSCodeGeneratorInfo(codeGen, new PrototypeGenerator(codeGen), new object[0], new object[0]);
		}
	}
}