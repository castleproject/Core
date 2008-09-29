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

namespace Castle.MonoRail.Framework.JSGeneration.jQuery
{
	using System;
	using Helpers;
	/// <summary>
	/// JQuery Generator implementation
	/// </summary>
	public class JQueryGenerator : AbstractJSGenerator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JQueryGenerator"/> class.
		/// </summary>
		/// <param name="codeGenerator">The code generator.</param>
		public JQueryGenerator(IJSCodeGenerator codeGenerator)
			: base(codeGenerator)
		{
		}

		/// <summary>
		/// Selects the relevant elements.
		/// </summary>
		/// <param name="ids">The ids.</param>
		private void SelectRelevantElements(params string[] ids)
		{
			if (ids.Length == 0)
				throw new InvalidOperationException("Must pass at least one id");

			CodeGenerator.Write("jQuery(");
			CodeGenerator.Write(AbstractHelper.Quote(string.Join(",", ids)));
			CodeGenerator.Write(")");
		}

		/// <summary>
		/// Write a new line
		/// </summary>
		private void WriteNewLine()
		{
			CodeGenerator.Write("\r\n");
		}
		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="position"></param>
		/// <param name="id"></param>
		/// <param name="renderOptions"></param>
		public override void InsertHtml(string position, string id, object renderOptions)
		{
			Position pos = (Position)Enum.Parse(typeof(Position), position, true);
			position = pos.ToString();
			string selector = id;
			object render = Render(renderOptions);
			if (pos == Position.appendTo || pos == Position.prependTo)
			{
				selector = render.ToString().Replace("\"", "");
				render = AbstractHelper.Quote(id);
			}
			SelectRelevantElements(selector);

			CodeGenerator.Write("." + position);
			CodeGenerator.Write("(" + render + ");");
			WriteNewLine();
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="id"></param>
		/// <param name="renderOptions"></param>
		public override void ReplaceHtml(string id, object renderOptions)
		{
			SelectRelevantElements(id);
			CodeGenerator.Write(".html(");
			CodeGenerator.Write(Render(renderOptions).ToString());
			CodeGenerator.Write(");");
			WriteNewLine();
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="id"></param>
		/// <param name="renderOptions"></param>
		public override void Replace(string id, object renderOptions)
		{
			SelectRelevantElements(id);
			CodeGenerator.Write(".replaceWith(");
			CodeGenerator.Write(Render(renderOptions).ToString());
			CodeGenerator.Write(");");
			WriteNewLine();
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="ids"></param>
		public override void Show(params string[] ids)
		{
			SelectRelevantElements(ids);
			CodeGenerator.Write(".show();");
			WriteNewLine();
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="ids"></param>
		public override void Hide(params string[] ids)
		{
			SelectRelevantElements(ids);
			CodeGenerator.Write(".hide();");
			WriteNewLine();
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="ids"></param>
		public override void Toggle(params string[] ids)
		{
			SelectRelevantElements(ids);
			CodeGenerator.Write(".toggle();");
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="ids"></param>
		public override void Remove(params string[] ids)
		{
			SelectRelevantElements(ids);
			CodeGenerator.Write(".remove();");
			WriteNewLine();
		}

		/// <summary>
		/// Creates a generator for an element.
		/// </summary>
		/// <param name="root">The root expression.</param>
		/// <returns></returns>
		public override IJSElementGenerator CreateElementGenerator(string root)
		{
			return new JQueryElementGenerator(this, root);
		}

		#region Nested type: Position

		/// <summary>
		/// 
		/// </summary>
		[Flags]
		private enum Position
		{
			/// <summary>
			/// Append content to the inside of every matched element.
			/// </summary>
			append = 0x1,
			/// <summary>
			/// Append all of the matched elements to another, specified, set of elements.
			/// </summary>
			appendTo = 0x2,
			/// <summary>
			/// 
			/// </summary>
			prepend = 0x4,
			/// <summary>
			/// Prepend content to the inside of every matched element.
			/// </summary>
			prependTo = 0x8,
			/// <summary>
			/// Insert content after each of the matched elements.
			/// </summary>
			after = 0x16,
			/// <summary>
			/// Insert content before each of the matched elements.
			/// </summary>
			before = 0x32,
			/// <summary>
			/// Insert all of the matched elements after another, specified, set of elements.
			/// </summary>
			insertAfter = 0x64,
			/// <summary>
			/// Insert all of the matched elements before another, specified, set of elements.
			/// </summary>
			insertBefore = 0x128,
			/// <summary>
			/// Wrap each matched element with the specified HTML content.
			/// </summary>
			wrap = 0x256,
			/// <summary>
			/// Wrap all the elements in the matched set into a single wrapper element.
			/// </summary>
			wrapInner = 0x512,
			/// <summary>
			/// Wrap all the elements in the matched set into a single wrapper element.
			/// </summary>
			wrapAll = 0x1024
		}

		#endregion
	}
}
