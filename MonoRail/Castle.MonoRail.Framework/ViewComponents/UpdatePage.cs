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

namespace Castle.MonoRail.Framework.ViewComponents
{
	using System.IO;
	using JSGeneration;

	/// <summary>
	/// Renders a javascript content that changes the page 
	/// elements using a special dsl-like language.
	/// </summary>
	/// 
	/// <seealso cref="IJSGenerator"/>
	/// 
	/// <example>
	/// The following illustrates its use.
	/// <code>
	/// #blockcomponent(UpdatePage)
	///   $page.ReplaceHtml('myotherdiv', 'new content')
	///   $page.Highlight('mydivid')
	/// #end
	/// </code>
	/// </example>
	/// <remarks>
	/// The current implementation is dependent on 
	/// prototype.js and scriptaculous.js
	/// </remarks>
	public class UpdatePage : ViewComponent
	{
		/// <summary>
		/// Called by the framework so the component can
		/// render its content
		/// </summary>
		public override void Render()
		{
			Context.Writer.WriteLine(GenerateJS());
		}

		/// <summary>
		/// Evaluates the component's body providing a <c>page</c>
		/// instance which is a <see cref="IJSGenerator"/>
		/// </summary>
		/// <returns></returns>
		protected string GenerateJS()
		{
			IViewEngineManager viewEngManager = EngineContext.Services.ViewEngineManager;
			IViewEngine viewEngine = Context.ViewEngine;
			IController currentController = EngineContext.CurrentController;
			IControllerContext currentControllerCtx = EngineContext.CurrentControllerContext;

			JSCodeGeneratorInfo jsCodeGen =
				viewEngManager.CreateJSCodeGeneratorInfo(EngineContext, currentController, currentControllerCtx);

			object generator = viewEngine.CreateJSGenerator(jsCodeGen, EngineContext, currentController, currentControllerCtx);

			PropertyBag["page"] = generator;

			Context.RenderBody(new StringWriter()); // Just for evaluation of generator

			PropertyBag.Remove("page");

			return generator.ToString();
		}
	}
}