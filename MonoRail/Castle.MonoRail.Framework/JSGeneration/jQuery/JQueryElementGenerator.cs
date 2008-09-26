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
	/// <summary>
	/// JQuery element generator
	/// </summary>
	public class JQueryElementGenerator : IJSElementGenerator
	{
		private readonly JQueryGenerator parent;
		private readonly string root;

		/// <summary>
		/// Initializes a new instance of the <see cref="JQueryElementGenerator"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="root">The root.</param>
		public JQueryElementGenerator(JQueryGenerator parent, string root)
		{
			this.parent = parent;
			this.root = root;
		}

		#region IJSElementGenerator Members

		/// <summary>
		/// Replaces the content of the element.
		/// </summary>
		/// <param name="renderOptions">Defines what to render</param>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.el('elementid').ReplaceHtml("%{partial='shared/newmessage.vm'}")
		/// </code>
		/// </example>
		public void ReplaceHtml(object renderOptions)
		{
			parent.ReplaceHtml(root, renderOptions);
		}

		/// <summary>
		/// Replaces the entire element's content -- and not only its innerHTML --
		/// by the content evaluated.
		/// </summary>
		/// <param name="renderOptions">Defines what to render</param>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.el('messagediv').Replace("%{partial='shared/newmessage.vm'}")
		/// </code>
		/// </example>
		public void Replace(object renderOptions)
		{
			parent.Replace(root, renderOptions);
		}

		/// <summary>
		/// Gets the parent generator.
		/// </summary>
		/// <value>The parent generator.</value>
		public IJSGenerator ParentGenerator
		{
			get { return parent; }
		}

		#endregion
	}
}