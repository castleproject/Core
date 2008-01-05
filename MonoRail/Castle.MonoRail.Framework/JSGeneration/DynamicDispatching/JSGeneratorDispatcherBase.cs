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

namespace Castle.MonoRail.Framework.JSGeneration.DynamicDispatching
{
	using System;

	/// <summary>
	/// Abstract class that contains the shared logic of JS Generation, separated from
	/// the various view engines implementations
	/// </summary>
	public abstract class JSGeneratorDispatcherBase : DynamicDispatcher
	{
		private readonly IJSCodeGenerator codeGen;
		private readonly IJSGenerator generator;
		private readonly object[] elementExtensions;

		/// <summary>
		/// Initializes a new instance of the <see cref="JSGeneratorDispatcherBase"/> class.
		/// </summary>
		/// <param name="codeGen">The code gen.</param>
		/// <param name="generator">The generator.</param>
		/// <param name="extensions">The extensions.</param>
		/// <param name="elementExtensions">The element extensions.</param>
		protected JSGeneratorDispatcherBase(IJSCodeGenerator codeGen, IJSGenerator generator, object[] extensions, object[] elementExtensions)
			: base(generator, extensions)
		{
			this.codeGen = codeGen;
			this.generator = generator;
			this.elementExtensions = elementExtensions;
		}

		/// <summary>
		/// Gets the javascript code generator.
		/// </summary>
		/// <value>The code gen.</value>
		public IJSCodeGenerator CodeGen
		{
			get { return codeGen; }
		}

		/// <summary>
		/// Executes an operation (totally late bound)
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		protected object InternalInvoke(string method, params object[] args)
		{
			if (method == "el")
			{
				AssertValidElementAccessor(args);

				string root = args[0].ToString();

				return CreateJSElementGeneratorProxy(codeGen, generator.CreateElementGenerator(root), elementExtensions);
			}

			if (HasMethod(method))
			{
				return Dispatch(method, args);
			}

			return CreateNullGenerator();
		}

		/// <summary>
		/// Creates a null generator.
		/// </summary>
		/// <returns></returns>
		protected abstract object CreateNullGenerator();

		/// <summary>
		/// Creates a JS element generator.
		/// </summary>
		/// <param name="codeGen">The code gen.</param>
		/// <param name="elementGenerator">The element generator.</param>
		/// <param name="elementExtensions">The element extensions.</param>
		/// <returns></returns>
		protected abstract object CreateJSElementGeneratorProxy(IJSCodeGenerator codeGen, IJSElementGenerator elementGenerator, object[] elementExtensions);

		private static void AssertValidElementAccessor(object[] args)
		{
			if (args == null || args.Length != 1)
			{
				throw new ArgumentException("el() method must be invoked with the element name as an argument");
			}
			if (args[0] == null)
			{
				throw new ArgumentException("el() method invoked with a null argument");
			}
		}
	}
}