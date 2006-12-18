// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

	/// <summary>
	/// Pendent
	/// </summary>
	public class PrototypeHelper : AbstractHelper
	{

		/// <summary>
		/// Pendent
		/// </summary>
		public class JSGenerator : DynamicDispatchSupport
		{
			private readonly static IDictionary DispMethods;

			private readonly IRailsEngineContext context;
			private readonly StringBuilder lines = new StringBuilder();

			#region Type Constructor

			/// <summary>
			/// Collects the public methods
			/// </summary>
			static JSGenerator()
			{
				DispMethods = new HybridDictionary(true);

				BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

				MethodInfo[] methods = typeof(JSGenerator).GetMethods(flags);

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
			}

			#endregion

			protected override IDictionary GeneratorMethods
			{
				get { return DispMethods; }
			}

			#region Dispatchable operations

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="position"></param>
			/// <param name="id"></param>
			/// <param name="renderOptions"></param>
			public void InsertHtml(string position, string id, object renderOptions)
			{
				Call(this, "new Insertion." + position, Quote(id), Render(renderOptions));
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="id"></param>
			/// <param name="renderOptions"></param>
			public void ReplaceHtml(String id, object renderOptions)
			{
				Call(this, "Element.update", Quote(id), Render(renderOptions));
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="id"></param>
			/// <param name="renderOptions"></param>
			public void Replace(String id, object renderOptions)
			{
				Call(this, "Element.replace", Quote(id), Render(renderOptions));
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="ids"></param>
			public void Show(params object[] ids)
			{
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="ids"></param>
			public void Hide(params object[] ids)
			{
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="ids"></param>
			public void Toggle(params object[] ids)
			{
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="ids"></param>
			public void Remove(params object[] ids)
			{
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="message"></param>
			public void Alert(String message)
			{
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="url"></param>
			public void RedirectTo(String url)
			{
				
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="variable"></param>
			/// <param name="expression"></param>
			public void Assign(String variable, String expression)
			{

			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="function"></param>
			/// <param name="args"></param>
			public void Call(object function, params object[] args)
			{
				Record(this, function + "(" + BuildJSArguments(args) + ")");
			}

			/// <summary>
			/// Renders the specified render options.
			/// <para>
			/// If the renderOptions is a string, the content is escaped and quoted.
			/// </para>
			/// <para>
			/// If the renderOptions is a dictionary, we extract the key <c>partial</c>
			/// and evaluate the template it points to. The content is escaped and quoted.
			/// </para>
			/// </summary>
			/// <param name="renderOptions">The render options.</param>
			/// <returns></returns>
			public object Render(object renderOptions)
			{
				if (renderOptions == null)
				{
					throw new ArgumentNullException("renderOptions",
						"renderOptions cannot be null. Must be a string or a dictionary");
				}
				else if (renderOptions is IDictionary)
				{
					IDictionary options = (IDictionary)renderOptions;

					String partialName = (String)options["partial"];

					if (partialName == null)
					{
						throw new ArgumentNullException("renderOptions",
							"renderOptions, as a dictionary, must have a 'partial' " +
							"entry with the template name to render");
					}

					IViewEngineManager viewEngineManager = (IViewEngineManager)
						context.GetService(typeof(IViewEngineManager));

					StringWriter writer = new StringWriter();

					viewEngineManager.ProcessPartial(writer, context, context.CurrentController, partialName);

					renderOptions = writer.ToString();
				}
				else
				{
					// TODO: add support for partial rendering
				}

				return Quote(JsEscape(renderOptions.ToString()));
			}

			#endregion

			#region Result generation (ToString)

			public override string ToString()
			{
				return @"try " +
					"\r\n{\r\n" + lines +
					"}\r\n" +
					"catch(e)\r\n" +
					"{\r\n" +
					"alert('Generated javascript threw an error: ' + e.toString() + '\\r\\n\\r\\n" +
					"Generated content: \\r\\n' + '" + JsEscape(lines.ToString()) + "');\r\n}";
			}

			#endregion

			#region Static members

			public static void Record(JSGenerator gen, string line)
			{
				gen.lines.AppendFormat("{0};\r\n", line);
			}

			/// <summary>
			/// Writes the content specified to the generator instance
			/// </summary>
			/// <param name="generator">The generator.</param>
			/// <param name="content">The content.</param>
			/// <returns></returns>
			public static void Write(JSGenerator generator, String content)
			{
				generator.lines.Append(content);
			}

			public static string BuildJSArguments(object[] args)
			{
				if (args == null || args.Length == 0) return String.Empty;

				StringBuilder tempBuffer = new StringBuilder();

				bool comma = false;

				foreach (String arg in args)
				{
					if (comma) tempBuffer.Append(',');

					tempBuffer.Append(arg);

					if (!comma) comma = true;
				}

				return tempBuffer.ToString();
			}

			public static void ReplaceTailByPeriod(JSGenerator generator)
			{
				int len = generator.lines.Length;

				if (len > 3)
				{
					RemoveTail(generator);
					generator.lines.Append('.');
				}
			}

			public static void RemoveTail(JSGenerator generator)
			{
				int len = generator.lines.Length;

				if (len > 3)
				{
					if (generator.lines[len - 3] == ';')
					{
						generator.lines.Length = len - 3;
					}
				}
			}

			#endregion

			#region Internal and private methods

			private string JsEscape(string content)
			{
				return context.Server.JavaScriptEscape(content);
			}

			#endregion
		}

		public class JSElementGenerator : DynamicDispatchSupport
		{
			private readonly static IDictionary DispMethods;

			private readonly JSGenerator generator;

			#region Type Constructor

			/// <summary>
			/// Collects the public methods
			/// </summary>
			static JSElementGenerator()
			{
				DispMethods = new HybridDictionary(true);

				BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

				MethodInfo[] methods = typeof(JSElementGenerator).GetMethods(flags);

				PopulateAvailableMethods(DispMethods, methods);
			}

			#endregion

			#region Constructor

			/// <summary>
			/// Initializes a new instance of the <see cref="JSElementGenerator"/> class.
			/// </summary>
			/// <param name="generator">The generator.</param>
			public JSElementGenerator(JSGenerator generator)
			{
				this.generator = generator;
			}

			#endregion

			/// <summary>
			/// Gets the parent generator.
			/// </summary>
			/// <value>The parent generator.</value>
			public JSGenerator ParentGenerator
			{
				get { return generator; }
			}

			protected override IDictionary GeneratorMethods
			{
				get { return DispMethods; }
			}

			#region Dispatchable operations

			/// <summary>
			/// TODO: Document this
			/// </summary>
			/// <param name="renderOptions">The render options.</param>
			public void ReplaceHtml(object renderOptions)
			{
				generator.Call("update", generator.Render(renderOptions));
			}

			/// <summary>
			/// TODO: Document this
			/// </summary>
			/// <param name="renderOptions">The render options.</param>
			public void Replace(object renderOptions)
			{
				generator.Call("replace", generator.Render(renderOptions));
			}

			#endregion
		}
	}

	public abstract class DynamicDispatchSupport
	{
		protected static void PopulateAvailableMethods(IDictionary generatorMethods, MethodInfo[] methods)
		{
			foreach(MethodInfo method in methods)
			{
				generatorMethods[method.Name] = method;
			}
		}

		protected abstract IDictionary GeneratorMethods { get; }

		public bool IsGeneratorMethod(string method)
		{
			return GeneratorMethods.Contains(method);
		}

		public void Dispatch(string method, params object[] args)
		{
			MethodInfo methInfo = (MethodInfo) GeneratorMethods[method];

			int expectedParameterCount = methInfo.GetParameters().Length;

			if (args.Length < expectedParameterCount)
			{
				// Complete with nulls, assuming that parameters are optional

				object[] newArgs = new object[expectedParameterCount];

				Array.Copy(args, newArgs, args.Length);

				args = newArgs;
			}

			methInfo.Invoke(this, args);
		}
	}

	
}
