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
	/// <summary>
	/// Operations related to an element
	/// </summary>
	public abstract class JSElementGeneratorDispatcherBase : DynamicDispatcher
	{
		private readonly IJSCodeGenerator codeGen;

		/// <summary>
		/// Initializes a new instance of the <see cref="JSElementGeneratorDispatcherBase"/> class.
		/// </summary>
		/// <param name="codeGen">The code gen.</param>
		/// <param name="elementGenerator">The element generator.</param>
		/// <param name="extensions">The extensions.</param>
		protected JSElementGeneratorDispatcherBase(IJSCodeGenerator codeGen, IJSElementGenerator elementGenerator, params object[] extensions)
			: base(elementGenerator, extensions)
		{
			this.codeGen = codeGen;
		}

		/// <summary>
		/// Generates a get statement
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		protected void InternalGet(string propName)
		{
			codeGen.ReplaceTailByPeriod();
			codeGen.Record(propName);
		}

		/// <summary>
		/// Dispatches the invocation (late bound)
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		protected object InternalInvoke(string method, object[] args)
		{
			if (method == "set")
			{
				codeGen.RemoveTail();
				codeGen.Record(" = " + args[0]);
				return null;
			}
			else
			{
				codeGen.ReplaceTailByPeriod();

				if (HasMethod(method))
				{
					Dispatch(method, args);
				}
				else
				{
					codeGen.Call(method, args);
				}

				return this;
			}
		}
	}
}