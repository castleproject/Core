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

namespace Castle.MonoRail.Framework.Internal
{
	using Castle.MonoRail.Framework.Helpers;

	/// <summary>
	/// Operations related to an element
	/// </summary>
	public abstract class JSElementGeneratorBase
	{
		/// <summary>
		/// Element generator instance
		/// </summary>
		protected readonly IJSElementGenerator generator;
		/// <summary>
		/// Parent Generator instance
		/// </summary>
		protected readonly IJSGenerator parentGenerator;

		/// <summary>
		/// Initializes a new instance of the <see cref="JSElementGeneratorBase"/> class.
		/// </summary>
		/// <param name="generator">The generator.</param>
		public JSElementGeneratorBase(IJSElementGenerator generator)
		{
			this.generator = generator;
			parentGenerator = generator.ParentGenerator;
		}

		/// <summary>
		/// Generates a get statement
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		protected void InternalGet(string propName)
		{
			PrototypeHelper.JSGenerator.ReplaceTailByPeriod(parentGenerator);
			PrototypeHelper.JSGenerator.Record(parentGenerator, propName);
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
				PrototypeHelper.JSGenerator.RemoveTail(parentGenerator);

				PrototypeHelper.JSGenerator.Record(parentGenerator, " = " + args[0]);

				return null;
			}
			else
			{
				PrototypeHelper.JSGenerator.ReplaceTailByPeriod(parentGenerator);
				//TODO: This code is duplicated JSCollectionGeneratorBase line 65
				DynamicDispatchSupport dispInterface = generator as DynamicDispatchSupport;
				if (dispInterface == null)
				{
					throw new MonoRail.Framework.RailsException("JS Generators must inherit DynamicDispatchSupport");
				}
				if (dispInterface.IsGeneratorMethod(method))
				{
					dispInterface.Dispatch(method, args);
				}
				else
				{
					parentGenerator.Call(method, args);
				}

				return this;
			}
		}
	}
}