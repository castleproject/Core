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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using System.Text;
	using Framework;
	using Services;

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
		/// Shows a JS alert
		/// </summary>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.Alert('You won a Mercedez')
		/// </code>
		/// </example>
		/// 
		/// <param name="message">The message to display.</param>
		void Alert(object message);

		/// <summary>
		/// Redirects to an url using the <c>location.href</c>.
		/// This is required as most ajax libs don't care for the redirect status
		/// in the http reply.
		/// </summary>
		/// 
		/// <example>
		/// The following redirects to a static page
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
		/// </example>
		/// 
		/// <param name="url">The URL.</param>
		void RedirectTo(object url);

		/// <summary>
		/// Re-apply Behaviour css' rules.
		/// </summary>
		/// <remarks>
		/// Only makes sense if you are using the Behaviour javascript library.
		/// </remarks>
		void ReApply();

		/// <summary>
		/// Generates a call to a scriptaculous' visual effect. 
		/// </summary>
		/// 
		/// <seealso cref="ScriptaculousHelper"/>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.VisualEffect('ToggleSlide', 'myelement')
		/// </code>
		/// 
		/// <para>
		/// This is especially useful to show which elements 
		/// where updated in an ajax call.
		/// </para>
		/// 
		/// <code>
		///	  $page.ReplaceHtml('mydiv', "Hey, I've changed")
		///   $page.VisualEffect('Highlight', 'mydiv')
		/// </code>
		/// 
		/// </example>
		/// 
		/// <param name="name">The effect name.</param>
		/// <param name="element">The target element.</param>
		/// <param name="options">The optional options.</param>
		void VisualEffect(String name, String element, IDictionary options);

		/// <summary>
		/// Generates a call to a scriptaculous' drop out visual effect. 
		/// </summary>
		/// 
		/// <seealso cref="ScriptaculousHelper"/>
		/// 
		/// <param name="element">The target element.</param>
		/// <param name="options">The optional options.</param>
		void VisualEffectDropOut(String element, IDictionary options);

		/// <summary>
		/// Assigns a javascript variable with the expression.
		/// </summary>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.Assign('myvariable', '10')
		/// </code>
		/// 
		/// <para>
		/// Which outputs:
		/// </para>
		/// 
		/// <code>
		///   myvariable = 10;
		/// </code>
		/// 
		/// <para>
		/// With strings you can escape strings:
		/// </para>
		/// 
		/// <code>
		///   $page.Assign('myvariable', '\'Hello world\'')
		/// </code>
		/// 
		/// <para>
		/// Which outputs:
		/// </para>
		/// 
		/// <code>
		///   myvariable = 'Hello world';
		/// </code>
		/// 
		/// </example>
		/// 
		/// <param name="variable">The target variable</param>
		/// <param name="expression">The right side expression</param>
		void Assign(String variable, String expression);

		/// <summary>
		/// Declares the specified variable as null.
		/// </summary>
		/// 
		/// <seealso cref="Assign"/>
		/// 
		/// <param name="variable">The variable name.</param>
		void Declare(String variable);

		/// <summary>
		/// Calls the specified function with the optional arguments.
		/// </summary>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.call('myJsFunctionAlreadyDeclared', '10', "'message'", $somethingfrompropertybag, $anothermessage.to_squote)
		/// </code>
		/// 
		/// <para>
		/// Which outputs:
		/// </para>
		/// 
		/// <code>
		///   myJsFunctionAlreadyDeclared(10, 'message', 1001, 'who let the dogs out?')
		/// </code>
		/// 
		/// </example>
		/// 
		/// <param name="function">The function name.</param>
		/// <param name="args">The arguments.</param>
		void Call(object function, params object[] args);

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
		/// Writes the content specified to the generator instance
		/// </summary>
		/// 
		/// <remarks>
		/// This is for advanced scenarios and for the infrastructure. Usually not useful.
		/// </remarks>
		/// 
		/// <param name="content">The content.</param>
		void Write(String content);

		/// <summary>
		/// Writes the content specified to the generator instance
		/// </summary>
		/// 
		/// <remarks>
		/// This is for advanced scenarios and for the infrastructure. Usually not useful.
		/// </remarks>
		/// <param name="content">The content.</param>
		void AppendLine(String content);

		/// <summary>
		/// Dump the operations recorded so far as javascript code. 
		/// </summary>
		/// 
		/// <returns></returns>
		string ToString();

		/// <summary>
		/// Gets the js lines.
		/// </summary>
		/// <value>The js lines.</value>
		StringBuilder Lines { get; }

		/// <summary>
		/// Creates a generator for a collection.
		/// </summary>
		/// <param name="root">The root expression.</param>
		/// <returns></returns>
		IJSCollectionGenerator CreateCollectionGenerator(string root);

		/// <summary>
		/// Creates a generator for an element.
		/// </summary>
		/// <param name="root">The root expression.</param>
		/// <returns></returns>
		IJSElementGenerator CreateElementGenerator(string root);
	}
}