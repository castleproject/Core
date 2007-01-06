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
		public class JSGenerator : DynamicDispatchSupport
		{
			enum Position
			{
				Top,
				Bottom,
				Before,
				After
			}

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
			/// TODO: Implement and document this one
			/// <para>
			/// Top, After, Before, Bottom
			/// </para>
			/// </summary>
			/// <param name="position"></param>
			/// <param name="id"></param>
			/// <param name="renderOptions"></param>
			public void InsertHtml(string position, string id, object renderOptions)
			{
				position = Enum.Parse(typeof(Position), position, true).ToString();

				Call("new Insertion." + position, Quote(id), Render(renderOptions));
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="id"></param>
			/// <param name="renderOptions"></param>
			public void ReplaceHtml(String id, object renderOptions)
			{
				Call("Element.update", Quote(id), Render(renderOptions));
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="id"></param>
			/// <param name="renderOptions"></param>
			public void Replace(String id, object renderOptions)
			{
				Call("Element.replace", Quote(id), Render(renderOptions));
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="ids"></param>
			public void Show(params string[] ids)
			{
				Call("Element.show", Quote(ids));
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="ids"></param>
			public void Hide(params string[] ids)
			{
				Call("Element.hide", Quote(ids));
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="ids"></param>
			public void Toggle(params string[] ids)
			{
				Call("Element.toggle", Quote(ids));
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="ids"></param>
			public void Remove(params string[] ids)
			{
				Record(this, "[" + BuildJSArguments(Quote(ids)) + "].each(Element.remove)");
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="message"></param>
			public void Alert(object message)
			{
				Call("alert", Quote(message));
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="url"></param>
			public void RedirectTo(String url)
			{
				Assign("window.location.href", Quote(url));
			}

			/// <summary>
			/// Re-apply Behaviour css' rules.
			/// </summary>
			public void ReApply()
			{
				Call("Behaviour.apply");
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="name">The name.</param>
			/// <param name="element">The element.</param>
			/// <param name="options">The options.</param>
			public void VisualEffect(String name, String element, IDictionary options)
			{
				Write(new ScriptaculousHelper().VisualEffect(name, element, options));
				Write(Environment.NewLine);
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="element">The element.</param>
			/// <param name="options">The options.</param>
			public void VisualEffectDropOut(String element, IDictionary options)
			{
				Write(new ScriptaculousHelper().VisualEffectDropOut(element, options));
				Write(Environment.NewLine);
			}

			/// <summary>
			/// TODO: Implement and document this one
			/// </summary>
			/// <param name="variable"></param>
			/// <param name="expression"></param>
			public void Assign(String variable, String expression)
			{
				Record(this, variable + " = " + expression);
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
						throw new RailsException("Could not process partial " + partialName, ex);
					}
				}
				else
				{
					// TODO: add support for partial rendering
				}

				return Quote(JsEscape(renderOptions.ToString()));
			}

			/// <summary>
			/// Writes the content specified to the generator instance
			/// </summary>
			/// <param name="content">The content.</param>
			public void Write(String content)
			{
				lines.Append(content);
			}

			/// <summary>
			/// Writes the content specified to the generator instance
			/// </summary>
			/// <param name="content">The content.</param>
			public void AppendLine(String content)
			{
				Record(this, content);
			}

			#endregion

			#region Result generation (ToString)

			public override string ToString()
			{
				return @"try " +
					"\n{\n" + lines +
					"}\n" +
					"catch(e)\n" +
					"{\n" +
					"alert('JS error ' + e.toString());\n" +
					"alert('Generated content: \\n" + JsEscapeWithSQuotes(lines.ToString()) + "');\n}";
			}

			#endregion

			#region Static members

			public static void Record(JSGenerator gen, string line)
			{
				gen.lines.AppendFormat("{0};\r\n", line);
			}

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
				content = Regex.Replace(content, "(\r\n)|(\r)|(\n)", "\\n", RegexOptions.Multiline);
				content = Regex.Replace(content, "\\\"", "\\\"", RegexOptions.Multiline);
				return content;
			}

			private string JsEscapeWithSQuotes(string content)
			{
				return Regex.Replace(JsEscape(content), "(\')", "\\'", RegexOptions.Multiline);
			}

			#endregion
		}

		public class JSCollectionGenerator : DynamicDispatchSupport
		{
			private readonly static IDictionary DispMethods;

			private readonly JSGenerator generator;

			#region Type Constructor

			/// <summary>
			/// Collects the public methods
			/// </summary>
			static JSCollectionGenerator()
			{
				DispMethods = new HybridDictionary(true);

				BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

				MethodInfo[] methods = typeof(JSCollectionGenerator).GetMethods(flags);

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
			public JSGenerator ParentGenerator
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
			public JSGenerator ParentGenerator
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
			MethodInfo methodInfo = (MethodInfo) GeneratorMethods[method];

			ParameterInfo[] parameters = methodInfo.GetParameters();

			int paramArrayIndex = -1;

			for(int i=0; i < parameters.Length; i++)
			{
				ParameterInfo paramInfo = parameters[i];

				if (paramInfo.IsDefined(typeof(ParamArrayAttribute), true))
				{
					paramArrayIndex = i;
				}
			}

			try
			{
				methodInfo.Invoke(this, BuildMethodArgs(methodInfo, args, paramArrayIndex));
			}
			catch(Exception ex)
			{
				throw new RailsException("Error invoking method on generator. " + 
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
