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
	using System.Collections.Generic;

	/// <summary>
	/// JQuery JS generator
	/// </summary>
	public class JQueryGenerator : AbstractJSGenerator
	{
		private static readonly IDictionary<string, string> positions =
			new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// Initializes a new instance of the <see cref="JQueryGenerator"/> class.
		/// </summary>
		/// <param name="codeGenerator">The code generator.</param>
		public JQueryGenerator(IJSCodeGenerator codeGenerator)
			: base(codeGenerator)
		{
			positions.Add("before", "before");
			positions.Add("after", "after");
			positions.Add("top", "prepend");
			positions.Add("bottom", "append");
			positions.Add("prepend", "prepend");
			positions.Add("append", "append");
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

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="ids"></param>
		public override void Hide(string[] ids)
		{
			SelectRelevantElements(ids);
			CodeGenerator.Write(".hide();");
		}

		/// <summary>
		/// Selects the relevant elements.
		/// </summary>
		/// <param name="ids">The ids.</param>
		private void SelectRelevantElements(params string[] ids)
		{
			if (ids.Length == 0)
			{
				throw new InvalidOperationException("Must pass at least one id");
			}

			CodeGenerator.Write("jQuery(");
			foreach(string id in ids)
			{
				CodeGenerator.Write("#" + id + ",");
			}
			CodeGenerator.RemoveTail();
			CodeGenerator.Write(")");
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="position"></param>
		/// <param name="id"></param>
		/// <param name="renderOptions"></param>
		public override void InsertHtml(string position, string id, object renderOptions)
		{
			string realPosition;
			if (positions.TryGetValue(position, out realPosition) == false)
			{
				throw new InvalidOperationException("Position " + position + " is invalid");
			}
			SelectRelevantElements(id);
			CodeGenerator.Write(".");
			CodeGenerator.Write(realPosition);
			CodeGenerator.Write(");");
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="ids"></param>
		public override void Remove(string[] ids)
		{
			SelectRelevantElements(ids);
			CodeGenerator.Write(".remove();");
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
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="id"></param>
		/// <param name="renderOptions"></param>
		public override void ReplaceHtml(string id, object renderOptions)
		{
			SelectRelevantElements(id);
			CodeGenerator.Write(".empty();");
			SelectRelevantElements(id);
			CodeGenerator.Write(".append(");
			CodeGenerator.Write(Render(renderOptions).ToString());
			CodeGenerator.Write(");");
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="ids"></param>
		public override void Show(string[] ids)
		{
			SelectRelevantElements(ids);
			CodeGenerator.Write(".show();");
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="ids"></param>
		public override void Toggle(string[] ids)
		{
			SelectRelevantElements(ids);
			CodeGenerator.Write(".toggle();");
		}
	}
}