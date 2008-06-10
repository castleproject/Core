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
	using System.Collections.Generic;
	using System.IO;
	using Castle.MonoRail.Framework.JSGeneration.Prototype;

	/// <summary>
	/// Pendent
	/// </summary>
	public class ViewEngineStub : ViewEngineBase
	{
		private string viewFileExtension;
		private bool supportsJSGeneration;
		private string jsGeneratorFileExtension;
		private readonly List<string> jsTemplateRendered = new List<string>();
		private readonly List<string> templateRendered = new List<string>();
		private readonly List<string> partialRendered = new List<string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewEngineStub"/> class.
		/// </summary>
		/// <param name="viewFileExtension">The view file extension.</param>
		/// <param name="supportsJSGeneration">if set to <c>true</c> [supports JS generation].</param>
		/// <param name="jsGeneratorFileExtension">The js generator file extension.</param>
		public ViewEngineStub(string viewFileExtension, string jsGeneratorFileExtension, bool supportsJSGeneration)
		{
			this.viewFileExtension = viewFileExtension;
			this.supportsJSGeneration = supportsJSGeneration;
			this.jsGeneratorFileExtension = jsGeneratorFileExtension;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewEngineStub"/> class.
		/// </summary>
		public ViewEngineStub() : this(".mm", ".jsm", true)
		{
		}

		/// <summary>
		/// Gets a list of partial templates that were "rendered" since the stub was created.
		/// </summary>
		/// <value>The partial rendered.</value>
		public List<string> PartialRendered
		{
			get { return partialRendered; }
		}

		/// <summary>
		/// Gets a list of templates that were "rendered" since the stub was created.
		/// </summary>
		/// <value>The template rendered.</value>
		public List<string> TemplateRendered
		{
			get { return templateRendered; }
		}

		/// <summary>
		/// Gets a list of js templates that were "rendered" since the stub was created.
		/// </summary>
		/// <value>The js template rendered.</value>
		public List<string> JsTemplateRendered
		{
			get { return jsTemplateRendered; }
		}

		/// <summary>
		/// Gets the view file extension.
		/// </summary>
		/// <value>The view file extension.</value>
		public override string ViewFileExtension
		{
			get { return viewFileExtension; }
		}

		/// <summary>
		/// Gets a value indicating whether the view engine
		/// support the generation of JS.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if JS generation is supported; otherwise, <c>false</c>.
		/// </value>
		public override bool SupportsJSGeneration
		{
			get { return supportsJSGeneration; }
		}

		/// <summary>
		/// Gets the JS generator file extension.
		/// </summary>
		/// <value>The JS generator file extension.</value>
		public override string JSGeneratorFileExtension
		{
			get { return jsGeneratorFileExtension; }
		}

		/// <summary>
		/// Returns a prototype generator.
		/// </summary>
		/// <param name="generatorInfo"></param>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="controllerContext"></param>
		/// <returns></returns>
		public override object CreateJSGenerator(JSCodeGeneratorInfo generatorInfo, IEngineContext context,
		                                         IController controller, IControllerContext controllerContext)
		{
			return new PrototypeGenerator(generatorInfo.CodeGenerator);
		}

		/// <summary>
		/// Records on <see cref="JsTemplateRendered"/> the js template
		/// specified.
		/// </summary>
		/// <param name="templateName">Name of the template.</param>
		/// <param name="output">The output.</param>
		/// <param name="generatorInfo">The generator info.</param>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		public override void GenerateJS(string templateName, TextWriter output, JSCodeGeneratorInfo generatorInfo,
		                                IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			jsTemplateRendered.Add(templateName);
		}

		/// <summary>
		/// Records on <see cref="TemplateRendered"/> the template
		/// specified.
		/// </summary>
		/// <param name="templateName"></param>
		/// <param name="output"></param>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="controllerContext"></param>
		public override void Process(string templateName, TextWriter output, IEngineContext context, IController controller,
		                             IControllerContext controllerContext)
		{
			templateRendered.Add(templateName);
		}

		/// <summary>
		/// Records on <see cref="TemplateRendered"/> the template
		/// specified.
		/// </summary>
		/// <param name="templateName"></param>
		/// <param name="layoutName"></param>
		/// <param name="output"></param>
		/// <param name="parameters"></param>
		public override void Process(string templateName, string layoutName, TextWriter output,
		                             IDictionary<string, object> parameters)
		{
			templateRendered.Add(templateName);
		}

		/// <summary>
		/// Records on <see cref="PartialRendered"/> the partial template
		/// specified.
		/// </summary>
		/// <param name="partialName">The partial name.</param>
		/// <param name="output">The output.</param>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		public override void ProcessPartial(string partialName, TextWriter output, IEngineContext context,
		                                    IController controller, IControllerContext controllerContext)
		{
			partialRendered.Add(partialName);
		}

		/// <summary>
		/// Records on <see cref="PartialRendered"/> the partial template
		/// specified.
		/// </summary>
		/// <param name="contents"></param>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="controllerContext"></param>
		public override void RenderStaticWithinLayout(string contents, IEngineContext context, IController controller,
		                                              IControllerContext controllerContext)
		{
			partialRendered.Add(contents);
		}

        /// <summary>
        /// Evaluates whether the specified template exists.
        /// </summary>
        /// <returns><c>true</c> if it exists</returns>
        public override bool HasTemplate(string templateName)
        {
            return true;
        }
	}
}
