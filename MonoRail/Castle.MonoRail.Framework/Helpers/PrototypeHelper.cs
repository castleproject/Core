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
	using System.Collections.Specialized;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Pendent
	/// </summary>
	public class PrototypeHelper : AbstractHelper
	{
		/// <summary>
		/// Pendent
		/// </summary>
		public class JSGenerator : DynamicDispatchSupport, IJSGenerator
		{
			private enum Position
			{
				Top,
				Bottom,
				Before,
				After
			}

			private static readonly IDictionary DispMethods;

			private readonly IRailsEngineContext context;
			private readonly StringBuilder lines = new StringBuilder();
			private readonly UrlHelper urlHelper;

			#region Type Constructor

			/// <summary>
			/// Collects the public methods
			/// </summary>
			static JSGenerator()
			{
				DispMethods = new HybridDictionary(true);

				BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

				MethodInfo[] methods = typeof(IJSGenerator).GetMethods(flags);

				PopulateAvailableMethods(DispMethods, methods);
			}

			#endregion

			#region Constructor

			/// <summary>
			/// Initializes a new instance of the <see cref="JSGenerator"/> class.
			/// </summary>
			/// <param name="context">The request context</param>
			public JSGenerator(IRailsEngineContext context)
			{
				this.context = context;

				urlHelper = (UrlHelper) context.CurrentController.Helpers["UrlHelper"];
			}

			#endregion

			#region DynamicDispatchSupport overrides

			/// <summary>
			/// Gets the generator methods.
			/// </summary>
			/// <value>The generator methods.</value>
			protected override IDictionary GeneratorMethods
			{
				get { return DispMethods; }
			}

			#endregion

			#region Dispatchable operations

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
			public void InsertHtml(string position, string id, object renderOptions)
			{
				position = Enum.Parse(typeof(Position), position, true).ToString();

				Call("new Insertion." + position, Quote(id), Render(renderOptions));
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
			public void ReplaceHtml(String id, object renderOptions)
			{
				Call("Element.update", Quote(id), Render(renderOptions));
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
			public void Replace(String id, object renderOptions)
			{
				Call("Element.replace", Quote(id), Render(renderOptions));
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
			public void Show(params string[] ids)
			{
				Call("Element.show", Quote(ids));
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
			public void Hide(params string[] ids)
			{
				Call("Element.hide", Quote(ids));
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
			public void Toggle(params string[] ids)
			{
				Call("Element.toggle", Quote(ids));
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
			public void Remove(params string[] ids)
			{
				Record(this, "[" + BuildJSArguments(Quote(ids)) + "].each(Element.remove)");
			}

			/// <summary>
			/// Shows a JS alert
			/// </summary>
			/// <param name="message">The message to display.</param>
			/// <example>
			/// The following example uses nvelocity syntax:
			/// <code>
			/// $page.Alert('You won a Mercedez')
			/// </code>
			/// </example>
			public void Alert(object message)
			{
				Call("alert", Quote(message));
			}

			/// <summary>
			/// Redirects to an url using the <c>location.href</c>.
			/// This is required as most ajax libs don't care for the redirect status
			/// in the http reply.
			/// </summary>
			/// <param name="url">The URL.</param>
			/// <example>
			/// The following redirects to a static page
			/// <code>
			/// $page.RedirectTo('about.aspx')
			/// </code>
			/// 	<para>
			/// The following redirects using the <see cref="UrlHelper"/>
			/// 	</para>
			/// 	<code>
			/// $page.RedirectTo("%{controller='Home',action='index'}")
			/// </code>
			/// </example>
			public void RedirectTo(object url)
			{
				string target;

				if (url is IDictionary)
				{
					target = urlHelper.For(url as IDictionary);
				}
				else
				{
					target = url.ToString();
				}

				Assign("window.location.href", Quote(target));
			}

			/// <summary>
			/// Re-apply Behaviour css' rules.
			/// </summary>
			/// <remarks>
			/// Only makes sense if you are using the Behaviour javascript library.
			/// </remarks>
			public void ReApply()
			{
				Call("Behaviour.apply");
			}

			/// <summary>
			/// Generates a call to a scriptaculous' visual effect.
			/// </summary>
			/// <param name="name">The effect name.</param>
			/// <param name="element">The target element.</param>
			/// <param name="options">The optional options.</param>
			/// <seealso cref="ScriptaculousHelper"/>
			/// <example>
			/// The following example uses nvelocity syntax:
			/// <code>
			/// $page.VisualEffect('ToggleSlide', 'myelement')
			/// </code>
			/// 	<para>
			/// This is especially useful to show which elements
			/// where updated in an ajax call.
			/// </para>
			/// 	<code>
			/// $page.ReplaceHtml('mydiv', "Hey, I've changed")
			/// $page.VisualEffect('Highlight', 'mydiv')
			/// </code>
			/// </example>
			public void VisualEffect(String name, String element, IDictionary options)
			{
				Write(new ScriptaculousHelper().VisualEffect(name, element, options));
				Write(Environment.NewLine);
			}

			/// <summary>
			/// Generates a call to a scriptaculous' drop out visual effect.
			/// </summary>
			/// <param name="element">The target element.</param>
			/// <param name="options">The optional options.</param>
			/// <seealso cref="ScriptaculousHelper"/>
			public void VisualEffectDropOut(String element, IDictionary options)
			{
				Write(new ScriptaculousHelper().VisualEffectDropOut(element, options));
				Write(Environment.NewLine);
			}

			/// <summary>
			/// Assigns a javascript variable with the expression.
			/// </summary>
			/// <param name="variable">The target variable</param>
			/// <param name="expression">The right side expression</param>
			/// <example>
			/// The following example uses nvelocity syntax:
			/// <code>
			/// $page.Assign('myvariable', '10')
			/// </code>
			/// 	<para>
			/// Which outputs:
			/// </para>
			/// 	<code>
			/// myvariable = 10;
			/// </code>
			/// 	<para>
			/// With strings you can escape strings:
			/// </para>
			/// 	<code>
			/// $page.Assign('myvariable', '\'Hello world\'')
			/// </code>
			/// 	<para>
			/// Which outputs:
			/// </para>
			/// 	<code>
			/// myvariable = 'Hello world';
			/// </code>
			/// </example>
			public void Assign(String variable, String expression)
			{
				Record(this, variable + " = " + expression);
			}

			/// <summary>
			/// Declares the specified variable as null.
			/// </summary>
			/// <param name="variable">The variable name.</param>
			/// <seealso cref="Assign"/>
			public void Declare(string variable)
			{
				Record(this, string.Format("var {0} = null", variable));
				Record(this, Environment.NewLine);
			}

			/// <summary>
			/// Calls the specified function with the optional arguments.
			/// </summary>
			/// <param name="function">The function name.</param>
			/// <param name="args">The arguments.</param>
			/// <example>
			/// The following example uses nvelocity syntax:
			/// <code>
			/// $page.call('myJsFunctionAlreadyDeclared', '10', "'message'", $somethingfrompropertybag, $anothermessage.to_squote)
			/// </code>
			/// 	<para>
			/// Which outputs:
			/// </para>
			/// 	<code>
			/// myJsFunctionAlreadyDeclared(10, 'message', 1001, 'who let the dogs out?')
			/// </code>
			/// </example>
			public void Call(object function, params object[] args)
			{
				if (String.IsNullOrEmpty(function.ToString()))
					throw new ArgumentException("function cannot be null or an empty string.", "function");

				Record(this, function + "(" + BuildJSArguments(args) + ")");
			}

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
			public object Render(object renderOptions)
			{
				if (renderOptions == null)
				{
					throw new ArgumentNullException("renderOptions",
					                                "renderOptions cannot be null. Must be a string or a dictionary");
				}
				else if (renderOptions is IDictionary)
				{
					IDictionary options = (IDictionary) renderOptions;

					String partialName = (String) options["partial"];

					if (partialName == null)
					{
						throw new ArgumentNullException("renderOptions",
						                                "renderOptions, as a dictionary, must have a 'partial' " +
						                                "entry with the template name to render");
					}

					try
					{
						IViewEngineManager viewEngineManager = (IViewEngineManager)
						                                       context.GetService(typeof(IViewEngineManager));

						StringWriter writer = new StringWriter();

						viewEngineManager.ProcessPartial(writer, context, context.CurrentController, partialName);

						renderOptions = writer.ToString();
					}
					catch(Exception ex)
					{
						throw new MonoRailException("Could not process partial " + partialName, ex);
					}
				}

				return Quote(JsEscape(renderOptions.ToString()));
			}

			/// <summary>
			/// Writes the content specified to the generator instance
			/// </summary>
			/// <param name="content">The content.</param>
			/// <remarks>
			/// This is for advanced scenarios and for the infrastructure. Usually not useful.
			/// </remarks>
			public void Write(String content)
			{
				lines.Append(content);
			}

			/// <summary>
			/// Writes the content specified to the generator instance
			/// </summary>
			/// <param name="content">The content.</param>
			/// <remarks>
			/// This is for advanced scenarios and for the infrastructure. Usually not useful.
			/// </remarks>
			public void AppendLine(String content)
			{
				Record(this, content);
			}

			#endregion

			#region Result generation (ToString)

			/// <summary>
			/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
			/// </returns>
			public override string ToString()
			{
				return @"try " +
				       "\n{\n" + lines +
				       "}\n" +
				       "catch(e)\n" +
				       "{\n" +
				       "alert('JS error ' + e.toString());\n" +
				       "alert(\"Generated content: \\n" + JsEscapeWithSQuotes(lines.ToString()) + "\");\n}";
			}

			#endregion

			/// <summary>
			/// Gets the js lines.
			/// </summary>
			/// <value>The js lines.</value>
			public StringBuilder Lines
			{
				get { return lines; }
			}

			/// <summary>
			/// Creates a generator for a collection.
			/// </summary>
			/// <param name="root">The root expression.</param>
			/// <returns></returns>
			public IJSCollectionGenerator CreateCollectionGenerator(string root)
			{
				return new PrototypeHelper.JSCollectionGenerator(this, root);
			}

			/// <summary>
			/// Creates a generator for an element.
			/// </summary>
			/// <param name="root">The root expression.</param>
			/// <returns></returns>
			public IJSElementGenerator CreateElementGenerator(string root)
			{
				return new PrototypeHelper.JSElementGenerator(this, root);
			}

			#region Static members

			/// <summary>
			/// Records the specified line on the generator.
			/// </summary>
			/// <param name="gen">The gen.</param>
			/// <param name="line">The line.</param>
			public static void Record(IJSGenerator gen, string line)
			{
				gen.Lines.AppendFormat("{0};\r\n", line);
			}

			/// <summary>
			/// Builds the JS arguments.
			/// </summary>
			/// <param name="args">The args.</param>
			/// <returns></returns>
			public static string BuildJSArguments(object[] args)
			{
				if (args == null || args.Length == 0) return String.Empty;

				StringBuilder tempBuffer = new StringBuilder();

				bool comma = false;

				foreach(object arg in args)
				{
					if (comma) tempBuffer.Append(',');

					tempBuffer.Append(arg);

					if (!comma) comma = true;
				}

				return tempBuffer.ToString();
			}

			/// <summary>
			/// Replaces the tail by period.
			/// </summary>
			/// <param name="generator">The generator.</param>
			public static void ReplaceTailByPeriod(IJSGenerator generator)
			{
				int len = generator.Lines.Length;

				if (len > 3)
				{
					RemoveTail(generator);
					generator.Lines.Append('.');
				}
			}

			/// <summary>
			/// Removes the tail.
			/// </summary>
			/// <param name="generator">The generator.</param>
			public static void RemoveTail(IJSGenerator generator)
			{
				int len = generator.Lines.Length;

				if (len > 3)
				{
					if (generator.Lines[len - 3] == ';')
					{
						generator.Lines.Length = len - 3;
					}
				}
			}

			#endregion

			#region Internal and private methods

			private static string JsEscape(string content)
			{
				// This means: replace all variations of line breaks by a \n
				content = Regex.Replace(content, "(\r\n)|(\r)|(\n)", "\\n", RegexOptions.Multiline);
				// This is a lookbehind. It means: replace all " -- that are not preceded by \ -- by \"
				content = Regex.Replace(content, "(?<!\\\\)\"", "\\\"", RegexOptions.Multiline);
				return content;
			}

			private static string JsEscapeWithSQuotes(string content)
			{
				// This replaces all ' references by \'
				return Regex.Replace(JsEscape(content), "(\')", "\\'", RegexOptions.Multiline);
			}

			#endregion
		}

		/// <summary>
		/// Implementation of <see cref="IJSCollectionGenerator"/>
		/// </summary>
		public class JSCollectionGenerator : DynamicDispatchSupport, IJSCollectionGenerator
		{
			private static readonly IDictionary DispMethods;

			private readonly JSGenerator generator;

			#region Type Constructor

			/// <summary>
			/// Collects the public methods
			/// </summary>
			static JSCollectionGenerator()
			{
				DispMethods = new HybridDictionary(true);

				BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

				MethodInfo[] methods = typeof(IJSCollectionGenerator).GetMethods(flags);

				PopulateAvailableMethods(DispMethods, methods);
			}

			#endregion

			#region Constructor

			/// <summary>
			/// Initializes a new instance of the <see cref="JSCollectionGenerator"/> class.
			/// </summary>
			/// <param name="generator">The generator.</param>
			/// <param name="root">The root</param>
			public JSCollectionGenerator(JSGenerator generator, String root)
			{
				this.generator = generator;

				JSGenerator.Record(generator, "$$('" + root + "')");
			}

			#endregion

			/// <summary>
			/// Gets the parent generator.
			/// </summary>
			/// <value>The parent generator.</value>
			public IJSGenerator ParentGenerator
			{
				get { return generator; }
			}

			#region DynamicDispatchSupport overrides

			/// <summary>
			/// Gets the generator methods.
			/// </summary>
			/// <value>The generator methods.</value>
			protected override IDictionary GeneratorMethods
			{
				get { return DispMethods; }
			}

			#endregion
		}

		/// <summary>
		/// Implementation for the <see cref="IJSElementGenerator"/>
		/// </summary>
		public class JSElementGenerator : DynamicDispatchSupport, IJSElementGenerator
		{
			private static readonly IDictionary DispMethods;

			private readonly JSGenerator generator;

			#region Type Constructor

			/// <summary>
			/// Collects the public methods
			/// </summary>
			static JSElementGenerator()
			{
				DispMethods = new HybridDictionary(true);

				BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

				MethodInfo[] methods = typeof(IJSElementGenerator).GetMethods(flags);

				PopulateAvailableMethods(DispMethods, methods);
			}

			#endregion

			#region Constructor

			/// <summary>
			/// Initializes a new instance of the <see cref="JSElementGenerator"/> class.
			/// </summary>
			/// <param name="generator">The generator.</param>
			/// <param name="root">The root.</param>
			public JSElementGenerator(JSGenerator generator, String root)
			{
				this.generator = generator;

				JSGenerator.Record(generator, "$('" + root + "')");
			}

			#endregion

			/// <summary>
			/// Gets the parent generator.
			/// </summary>
			/// <value>The parent generator.</value>
			public IJSGenerator ParentGenerator
			{
				get { return generator; }
			}

			#region DynamicDispatchSupport overrides

			/// <summary>
			/// Gets the generator methods.
			/// </summary>
			/// <value>The generator methods.</value>
			protected override IDictionary GeneratorMethods
			{
				get { return DispMethods; }
			}

			#endregion

			#region Dispatchable operations

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
				generator.Call("update", generator.Render(renderOptions));
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
				generator.Call("replace", generator.Render(renderOptions));
			}

			#endregion
		}
	}

	/// <summary>
	/// DynamicDispatch support is an infrastructure 
	/// that mimics a dynamic language/environment. 
	/// It is not finished but the idea is to allow 
	/// plugins to add operations to the generators.
	/// </summary>
	public abstract class DynamicDispatchSupport
	{
		/// <summary>
		/// Populates the available methods.
		/// </summary>
		/// <param name="generatorMethods">The generator methods.</param>
		/// <param name="methods">The methods.</param>
		protected static void PopulateAvailableMethods(IDictionary generatorMethods, MethodInfo[] methods)
		{
			foreach(MethodInfo method in methods)
			{
				generatorMethods[method.Name] = method;
			}
		}

		/// <summary>
		/// Gets the generator methods.
		/// </summary>
		/// <value>The generator methods.</value>
		protected abstract IDictionary GeneratorMethods { get; }

		/// <summary>
		/// Determines whether [is generator method] [the specified method].
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>
		/// 	<c>true</c> if [is generator method] [the specified method]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsGeneratorMethod(string method)
		{
			return GeneratorMethods.Contains(method);
		}

		/// <summary>
		/// Dispatches the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public object Dispatch(string method, params object[] args)
		{
			MethodInfo methodInfo = (MethodInfo) GeneratorMethods[method];

			ParameterInfo[] parameters = methodInfo.GetParameters();

			int paramArrayIndex = -1;

			for(int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo paramInfo = parameters[i];

				if (paramInfo.IsDefined(typeof(ParamArrayAttribute), true))
				{
					paramArrayIndex = i;
				}
			}

			try
			{
				return methodInfo.Invoke(this, BuildMethodArgs(methodInfo, args, paramArrayIndex));
			}
			catch(MonoRailException)
			{
				throw;
			}
			catch(Exception ex)
			{
				throw new MonoRailException("Error invoking method on generator. " +
				                         "Method invoked [" + method + "] with " + args.Length + " argument(s)", ex);
			}
		}

		private object[] BuildMethodArgs(MethodInfo method, object[] methodArguments, int paramArrayIndex)
		{
			ParameterInfo[] methodArgs = method.GetParameters();

			if (paramArrayIndex != -1)
			{
				Type arrayParamType = methodArgs[paramArrayIndex].ParameterType;

				object[] newParams = new object[methodArgs.Length];

				Array.Copy(methodArguments, newParams, methodArgs.Length - 1);

				if (methodArguments.Length < (paramArrayIndex + 1))
				{
					newParams[paramArrayIndex] = Array.CreateInstance(
						arrayParamType.GetElementType(), 0);
				}
				else
				{
					Array args = Array.CreateInstance(arrayParamType.GetElementType(), (methodArguments.Length + 1) - newParams.Length);

					Array.Copy(methodArguments, methodArgs.Length - 1, args, 0, args.Length);

					newParams[paramArrayIndex] = args;
				}

				methodArguments = newParams;
			}
			else
			{
				int expectedParameterCount = methodArgs.Length;

				if (methodArguments.Length < expectedParameterCount)
				{
					// Complete with nulls, assuming that parameters are optional

					object[] newArgs = new object[expectedParameterCount];

					Array.Copy(methodArguments, newArgs, methodArguments.Length);

					methodArguments = newArgs;
				}
			}

			return methodArguments;
		}
	}
}
