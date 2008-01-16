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

namespace Castle.MonoRail.Framework.JSGeneration.Prototype
{
	using System;
	using Castle.MonoRail.Framework.Helpers;

	/// <summary>
	/// Pendent
	/// </summary>
	public class PrototypeGenerator : AbstractJSGenerator
	{
		private enum Position
		{
			Top,
			Bottom,
			Before,
			After
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeGenerator"/> class.
		/// </summary>
		/// <param name="codeGenerator">The code generator.</param>
		public PrototypeGenerator(IJSCodeGenerator codeGenerator) : base(codeGenerator)
		{
		}

		/// <summary>
		/// Inserts a content snippet relative to the element specified by the <paramref name="id"/>
		/// 	<para>
		/// The supported positions are
		/// Top, After, Before, Bottom
		/// </para>
		/// </summary>
		/// <param name="position">The position to insert the content relative to the element id</param>
		/// <param name="id">The target element id</param>
		/// <param name="renderOptions">Defines what to render</param>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.InsertHtml('Bottom', 'messagestable', "%{partial='shared/newmessage.vm'}")
		/// </code>
		/// </example>
		public override void InsertHtml(string position, string id, object renderOptions)
		{
			position = Enum.Parse(typeof(Position), position, true).ToString();

			CodeGenerator.Call("new Insertion." + position, AbstractHelper.Quote(id), Render(renderOptions));
		}

		/// <summary>
		/// Replaces the content of the specified target element.
		/// </summary>
		/// <param name="id">The target element id</param>
		/// <param name="renderOptions">Defines what to render</param>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.ReplaceHtml('messagediv', "%{partial='shared/newmessage.vm'}")
		/// </code>
		/// </example>
		public override void ReplaceHtml(String id, object renderOptions)
		{
			CodeGenerator.Call("Element.update", AbstractHelper.Quote(id), Render(renderOptions));
		}

		/// <summary>
		/// Replaces the entire target element -- and not only its innerHTML --
		/// by the content evaluated.
		/// </summary>
		/// <param name="id">The target element id</param>
		/// <param name="renderOptions">Defines what to render</param>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.Replace('messagediv', "%{partial='shared/newmessage.vm'}")
		/// </code>
		/// </example>
		public override void Replace(String id, object renderOptions)
		{
			CodeGenerator.Call("Element.replace", AbstractHelper.Quote(id), Render(renderOptions));
		}

		/// <summary>
		/// Shows the specified elements.
		/// </summary>
		/// <param name="ids">The elements ids.</param>
		/// <remarks>
		/// The elements must exist.
		/// </remarks>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.Show('div1', 'div2')
		/// </code>
		/// </example>
		public override void Show(params string[] ids)
		{
			CodeGenerator.Call("Element.show", AbstractHelper.Quote(ids));
		}

		/// <summary>
		/// Hides the specified elements.
		/// </summary>
		/// <param name="ids">The elements ids.</param>
		/// <remarks>
		/// The elements must exist.
		/// </remarks>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.Hide('div1', 'div2')
		/// </code>
		/// </example>
		public override void Hide(params string[] ids)
		{
			CodeGenerator.Call("Element.hide", AbstractHelper.Quote(ids));
		}

		/// <summary>
		/// Toggles the display status of the specified elements.
		/// </summary>
		/// <param name="ids">The elements ids.</param>
		/// <remarks>
		/// The elements must exist.
		/// </remarks>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.Toggle('div1', 'div2')
		/// </code>
		/// </example>
		public override void Toggle(params string[] ids)
		{
			CodeGenerator.Call("Element.toggle", AbstractHelper.Quote(ids));
		}

		/// <summary>
		/// Remove the specified elements from the DOM.
		/// </summary>
		/// <param name="ids">The elements ids.</param>
		/// <remarks>
		/// The elements must exist.
		/// </remarks>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.Remove('div1', 'div2')
		/// </code>
		/// </example>
		public override void Remove(params string[] ids)
		{
			CodeGenerator.Record("[" + CodeGenerator.BuildJSArguments(Quote(ids)) + "].each(Element.remove)");
		}

		/// <summary>
		/// Creates a generator for an element.
		/// </summary>
		/// <param name="root">The root expression.</param>
		/// <returns></returns>
		public override IJSElementGenerator CreateElementGenerator(string root)
		{
			return new PrototypeElementGenerator(this, root);
		}
	}
}
