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

namespace Castle.MonoRail.Views.Brail
{
	using Boo.Lang;
	using Castle.MonoRail.Framework.JSGeneration;
	using Castle.MonoRail.Framework.JSGeneration.DynamicDispatching;

	public class BrailJSElementGenerator : JSElementGeneratorDispatcherBase, IQuackFu
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BrailJSElementGenerator"/> class.
		/// </summary>
		/// <param name="codeGen">The code gen.</param>
		/// <param name="elementGenerator">The element generator.</param>
		/// <param name="extensions">The extensions.</param>
		public BrailJSElementGenerator(IJSCodeGenerator codeGen, IJSElementGenerator elementGenerator, params object[] extensions) :
			base(codeGen, elementGenerator, extensions)
		{
		}

		public object QuackGet(string name, object[] parameters)
		{
			InternalGet(name);
			return this;
		}

		public object QuackSet(string name, object[] parameters, object value)
		{
			InternalGet(name); //get the current element and then set it
			return InternalInvoke("set", new object[] { value });
		}

		public object QuackInvoke(string name, params object[] args)
		{
			return InternalInvoke(name, args);
		}
	}
}