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
	using DynamicDispatching;
	using Helpers;

	/// <summary>
	/// Pendent
	/// </summary>
	public abstract class AbstractJSGenerator : IJSGenerator
	{
		private readonly IJSCodeGenerator codeGenerator;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractJSGenerator"/> class.
		/// </summary>
		/// <param name="codeGenerator">The code generator.</param>
		protected AbstractJSGenerator(IJSCodeGenerator codeGenerator)
		{
			this.codeGenerator = codeGenerator;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		[DynamicOperation]
		public abstract void InsertHtml(string position, string id, object renderOptions);

		/// <summary>
		/// Pendent
		/// </summary>
		[DynamicOperation]
		public abstract void ReplaceHtml(string id, object renderOptions);

		/// <summary>
		/// Pendent
		/// </summary>
		[DynamicOperation]
		public abstract void Replace(string id, object renderOptions);

		/// <summary>
		/// Pendent
		/// </summary>
		[DynamicOperation]
		public abstract void Show(params string[] ids);

		/// <summary>
		/// Pendent
		/// </summary>
		[DynamicOperation]
		public abstract void Hide(params string[] ids);

		/// <summary>
		/// Pendent
		/// </summary>
		[DynamicOperation]
		public abstract void Toggle(params string[] ids);

		/// <summary>
		/// Pendent
		/// </summary>
		[DynamicOperation]
		public abstract void Remove(params string[] ids);

		/// <summary>
		/// Outputs the content using the renderOptions approach.
		/// <para>
		/// If the renderOptions is a string, the content is escaped and quoted.
		/// </para>
		/// 	<para>
		/// If the renderOptions is a dictionary, we extract the key <c>partial</c>
		/// and evaluate the template it points to. The content is escaped and quoted.
		/// </para>
		/// </summary>
		/// <param name="renderOptions">The render options.</param>
		/// <returns></returns>
		/// <example>
		/// The following example uses nvelocity syntax:
		/// <code>
		/// $page.Call('myJsFunction', $page.render("%{partial='shared/newmessage.vm'}") )
		/// </code>
		/// 	<para>
		/// Which outputs:
		/// </para>
		/// 	<code>
		/// myJsFunction('the content from the newmessage partial view template')
		/// </code>
		/// </example>
		[DynamicOperation]
		public virtual object Render(object renderOptions)
		{
			return CodeGenerator.Render(renderOptions);
		}

		/// <summary>
		/// Creates a generator for an element.
		/// </summary>
		/// <param name="root">The root expression.</param>
		/// <returns></returns>
		public abstract IJSElementGenerator CreateElementGenerator(string root);

		/// <summary>
		/// Gets the code generator instance.
		/// </summary>
		/// <value>The code generator.</value>
		public IJSCodeGenerator CodeGenerator
		{
			get { return codeGenerator; }
		}

		/// <summary>
		/// Quotes the specified content.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns></returns>
		protected string Quote(string content)
		{
			return AbstractHelper.Quote(content);
		}

		/// <summary>
		/// Quotes the specified content array.
		/// </summary>
		/// <param name="content">The content array.</param>
		/// <returns></returns>
		protected string[] Quote(object[] content)
		{
			return AbstractHelper.Quote(content);
		}
	}
}