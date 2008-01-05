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

namespace Castle.MonoRail.Framework.JSGeneration
{
	/// <summary>
	/// Depicts the operations supported by the element Js generator.
	/// </summary>
	/// 
	/// <remarks>
	/// In practice you can access this generator by using the element accessor. 
	/// For example (using nvelocity syntax):
	/// 
	/// <code>
	///   $page.el('element id') -> IJSElementGenerator instance
	/// </code>
	/// 
	/// </remarks>
    public interface IJSElementGenerator 
	{
        /// <summary>
        /// Gets the parent generator.
        /// </summary>
        /// <value>The parent generator.</value>
        IJSGenerator ParentGenerator { get; }

		/// <summary>
		/// Replaces the content of the element.
		/// </summary>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.el('elementid').ReplaceHtml("%{partial='shared/newmessage.vm'}")
		/// </code>
		/// </example>
		/// 
		/// <param name="renderOptions">Defines what to render</param>
		void ReplaceHtml(object renderOptions);

		/// <summary>
		/// Replaces the entire element's content -- and not only its innerHTML --
		/// by the content evaluated.
		/// </summary>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.el('messagediv').Replace("%{partial='shared/newmessage.vm'}")
		/// </code>
		/// </example>
		/// 
		/// <param name="renderOptions">Defines what to render</param>
		void Replace(object renderOptions);
    }
}
