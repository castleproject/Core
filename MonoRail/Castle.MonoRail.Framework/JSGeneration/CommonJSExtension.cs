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
	using Helpers;

	/// <summary>
	/// Pendent
	/// </summary>
	public class CommonJSExtension
	{
		private readonly IJSCodeGenerator jsCodeGenerator;

		/// <summary>
		/// Initializes a new instance of the <see cref="CommonJSExtension"/> class.
		/// </summary>
		/// <param name="jsCodeGenerator">The js code generator.</param>
		public CommonJSExtension(IJSCodeGenerator jsCodeGenerator)
		{
			this.jsCodeGenerator = jsCodeGenerator;
		}

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
		[DynamicOperation]
		public void Assign(String variable, String expression)
		{
			jsCodeGenerator.Record(variable + " = " + expression);
		}

		/// <summary>
		/// Declares the specified variable as null.
		/// </summary>
		/// 
		/// <seealso cref="Assign"/>
		/// 
		/// <param name="variable">The variable name.</param>
		[DynamicOperation]
		public void Declare(String variable)
		{
			jsCodeGenerator.Record(string.Format("var {0} = null", variable));
		}

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
		[DynamicOperation]
		public void Call(object function, params object[] args)
		{
			jsCodeGenerator.Call(function, args);
		}

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
		[DynamicOperation]
		public void Alert(object message)
		{
			Call("alert", AbstractHelper.Quote(message));
		}

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
		[DynamicOperation]
		public void RedirectTo(object url)
		{
			string target;

			if (url is IDictionary)
			{
				target = jsCodeGenerator.UrlBuilder.BuildUrl(
					jsCodeGenerator.EngineContext.UrlInfo, url as IDictionary);
			}
			else
			{
				target = url.ToString();
			}

			Assign("window.location.href", AbstractHelper.Quote(target));
		}
	}
}
