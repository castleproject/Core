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
	using System.Reflection;
	using System.Text;

	public class PrototypeHelper : AbstractHelper
	{
		public class JSGenerator
		{
			private static IDictionary GeneratorMethods;
			private readonly IServerUtility serverUtility;
			private StringBuilder lines = new StringBuilder();

			#region Type Constructor

			/// <summary>
			/// Collects the public methods
			/// </summary>
			static JSGenerator()
			{
				GeneratorMethods = new HybridDictionary(true);

				MethodInfo[] methods =
					typeof(JSGenerator).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				foreach(MethodInfo method in methods)
				{
					GeneratorMethods[method.Name] = method;
				}
			}

			#endregion

			#region Constructor

			/// <summary>
			/// Initializes a new instance of the <see cref="JSGenerator"/> class.
			/// </summary>
			/// <param name="serverUtility">The server utility instance.</param>
			public JSGenerator(IServerUtility serverUtility)
			{
				this.serverUtility = serverUtility;
			}

			#endregion

			public void ReplaceHtml(String id, object renderOptions)
			{
				Call(this, "Element.update", Quote(id), Render(renderOptions));
			}

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

			#region Static members

			public static void Call(JSGenerator gen, object function, params object[] args)
			{
				Record(gen, function + "(" + BuildJSArguments(args) + ")");
			}

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

				foreach(String arg in args)
				{
					if (comma) tempBuffer.Append(',');

					tempBuffer.Append(arg);

					if (!comma) comma = true;
				}

				return tempBuffer.ToString();
			}

			public static bool IsGeneratorMethod(string method)
			{
				return GeneratorMethods.Contains(method);
			}

			public static void Dispatch(JSGenerator generator, string method, params object[] args)
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

				methInfo.Invoke(generator, args);
			}

			public static void ReplaceTailByPeriod(JSGenerator generator)
			{
				int len = generator.lines.Length;

				if (len > 3)
				{
					if (generator.lines[len-3] == ';')
					{
						generator.lines.Length = len - 3;
					}
					generator.lines.Append('.');
				}
			}

			#endregion

			#region Internal and private methods

			internal object Render(object renderOptions)
			{
				if (renderOptions == null)
				{
					throw new ArgumentNullException("renderOptions",
						"renderOptions cannot be null. Must be a string or a dictionary");
				}
				else
				{
					// TODO: add support for partial rendering
				}

				return Quote(JsEscape(renderOptions.ToString()));
			}

			private string JsEscape(string content)
			{
				return serverUtility.JavaScriptEscape(content);
			}

			#endregion
		}
	}
}
