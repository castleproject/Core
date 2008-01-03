// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System;
	using System.Collections;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Services;

	/// <summary>
	/// Depicts the contract for javascript generators.
	/// </summary>
	/// 
	/// <remarks>
	/// <para>
	/// Urls can be specified as string or a dictionary. If the latter, the <see cref="UrlHelper"/>
	/// is used. See <see cref="DefaultUrlBuilder.BuildUrl(UrlInfo,IDictionary)"/>
	/// </para>
	/// 
	/// <para>
	/// The <c>renderOptions</c> is also a common parameter. If you pass a string, 
	/// the string will be rendered. If it is a dictionary, it instructs the infrastructure to
	/// render a partial content. The dictionary must contain an entry named <c>partial</c>
	/// with the absolute path to the view to render.
	/// </para>
	/// 
	/// </remarks>
	/// 
	/// <example>
	/// The following is an example of using it with a nvelocity 
	/// syntax and renders static content:
	/// 
	/// <code>
	///   $page.InsertHtml('Bottom', 'messagestable', "Message sent")
	/// </code>
	/// 
	/// <para>
	/// The following uses a partial view:
	/// </para>
	/// 
	/// <code>
	///   $page.InsertHtml('Bottom', 'messagestable', "%{partial='shared/newmessage.vm'}")
	/// </code>
	/// 
	/// <para>
	/// The following redirects to a static page
	/// </para>
	/// 
	/// <code>
	///   $page.RedirectTo('about.aspx')
	/// </code>
	/// 
	/// <para>
	/// The following redirects using the <see cref="UrlHelper"/>
	/// </para>
	/// 
	/// <code>
	///   $page.RedirectTo("%{controller='Home',action='index'}")
	/// </code>
	/// 
	/// </example>
	public interface IJSGenerator
	{
		/// <summary>
		/// Inserts a content snippet relative to the element specified by the <paramref name="id"/>
		/// 
		/// <para>
		/// The supported positions are
		/// Top, After, Before, Bottom
		/// </para>
		/// </summary>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.InsertHtml('Bottom', 'messagestable', "%{partial='shared/newmessage.vm'}")
		/// </code>
		/// </example>
		/// 
		/// <param name="position">The position to insert the content relative to the element id</param>
		/// <param name="id">The target element id</param>
		/// <param name="renderOptions">Defines what to render</param>
		void InsertHtml(string position, string id, object renderOptions);

		/// <summary>
		/// Replaces the content of the specified target element.
		/// </summary>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.ReplaceHtml('messagediv', "%{partial='shared/newmessage.vm'}")
		/// </code>
		/// </example>
		/// 
		/// <param name="id">The target element id</param>
		/// <param name="renderOptions">Defines what to render</param>
		void ReplaceHtml(String id, object renderOptions);

		/// <summary>
		/// Replaces the entire target element -- and not only its innerHTML --
		///  by the content evaluated.
		/// </summary>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.Replace('messagediv', "%{partial='shared/newmessage.vm'}")
		/// </code>
		/// </example>
		/// 
		/// <param name="id">The target element id</param>
		/// <param name="renderOptions">Defines what to render</param>
		void Replace(String id, object renderOptions);

		/// <summary>
		/// Shows the specified elements.
		/// </summary>
		/// 
		/// <remarks>
		/// The elements must exist.
		/// </remarks>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.Show('div1', 'div2')
		/// </code>
		/// </example>
		/// 
		/// <param name="ids">The elements ids.</param>
		void Show(params string[] ids);

		/// <summary>
		/// Hides the specified elements.
		/// </summary>
		/// 
		/// <remarks>
		/// The elements must exist.
		/// </remarks>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.Hide('div1', 'div2')
		/// </code>
		/// </example>
		/// 
		/// <param name="ids">The elements ids.</param>		
		void Hide(params string[] ids);

		/// <summary>
		/// Toggles the display status of the specified elements.
		/// </summary>
		/// 
		/// <remarks>
		/// The elements must exist.
		/// </remarks>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.Toggle('div1', 'div2')
		/// </code>
		/// </example>
		/// 
		/// <param name="ids">The elements ids.</param>
		void Toggle(params string[] ids);

		/// <summary>
		/// Remove the specified elements from the DOM.
		/// </summary>
		/// 
		/// <remarks>
		/// The elements must exist.
		/// </remarks>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.Remove('div1', 'div2')
		/// </code>
		/// </example>
		/// 
		/// <param name="ids">The elements ids.</param>
		void Remove(params string[] ids);

		/// <summary>
		/// Outputs the content using the renderOptions approach.
		/// 
		/// <para>
		/// If the renderOptions is a string, the content is escaped and quoted.
		/// </para>
		/// 
		/// <para>
		/// If the renderOptions is a dictionary, we extract the key <c>partial</c>
		/// and evaluate the template it points to. The content is escaped and quoted.
		/// </para>
		/// 
		/// </summary>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.Call('myJsFunction', $page.render("%{partial='shared/newmessage.vm'}") )
		/// </code>
		/// 
		/// <para>
		/// Which outputs:
		/// </para>
		/// 
		/// <code>
		///   myJsFunction('the content from the newmessage partial view template')
		/// </code>
		/// 
		/// </example>
		/// 
		/// <param name="renderOptions">The render options.</param>
		/// <returns></returns>
		object Render(object renderOptions);

		/// <summary>
		/// Creates a generator for an element.
		/// </summary>
		/// <param name="root">The root expression.</param>
		/// <returns></returns>
		IJSElementGenerator CreateElementGenerator(string root);

//		/// <summary>
//		/// Creates a generator for a collection.
//		/// </summary>
//		/// <param name="root">The root expression.</param>
//		/// <returns></returns>
//		IJSCollectionGenerator CreateCollectionGenerator(string root);

	}
}