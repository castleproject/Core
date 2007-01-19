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

namespace Castle.MonoRail.Views.Brail
{
	using System;
	using Boo.Lang;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// 
	/// </summary>
	public class BrailJSGenerator : JSGeneratorBase, IQuackFu
	{
		public BrailJSGenerator(PrototypeHelper.JSGenerator generator)
			: base(generator)
		{
		}

		/// <summary>
		/// Defines the behavior when a property is read
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <param name="parameters">Parameters for indexers</param>
		/// <returns>value back to the template</returns>
		public object QuackGet(string propName, object[] parameters)
		{
			return QuackInvoke(propName, parameters);
		}

		/// <summary>
		/// Defines the behavior when a property is written
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <param name="parameters">Parameters for indexers</param>
		/// <param name="value">The value to assign.</param>
		public object QuackSet(string propName, object[] parameters, object value)
		{
			throw new NotSupportedException("You can't set properties on the generator");
		}

		/// <summary>
		/// Invokes the specified method.
		/// </summary>
		/// <param name="method">The method name.</param>
		/// <param name="args">The method arguments.</param>
		/// <returns>value back to the template</returns>
		public object QuackInvoke(string method, params object[] args)
		{
			if (method == "get_Item")
				method = "el";
			return InternalInvoke(method, args);
		}

		/// <summary>
		/// Delegates to the generator
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
			return generator.ToString();
		}

		protected override object CreateNullGenerator()
		{
			return null;
		}

		protected override object CreateJSCollectionGenerator(IJSCollectionGenerator collectionGenerator)
		{
			return new BrailJSCollectionGenerator(collectionGenerator);
		}

		protected override object CreateJSElementGenerator(IJSElementGenerator elementGenerator)
		{
			return new BrailJSElementGenerator(elementGenerator);
		}
	}
}