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

using NVelocity;

namespace Castle.MonoRail.Framework.Views.NVelocity.JSGeneration
{
	using System;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// 
	/// </summary>
	public class JSElementGeneratorDuck : JSElementGeneratorBase, IDuck
	{
		public JSElementGeneratorDuck(IJSElementGenerator generator) : base(generator)
		{
		}

		#region IDuck

		/// <summary>
		/// Defines the behavior when a property is read
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <returns>value back to the template</returns>
		public object GetInvoke(string propName)
		{
			InternalGet(propName);

			return this;
		}

		/// <summary>
		/// Defines the behavior when a property is written
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <param name="value">The value to assign.</param>
		public void SetInvoke(string propName, object value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Invokes the specified method.
		/// </summary>
		/// <param name="method">The method name.</param>
		/// <param name="args">The method arguments.</param>
		/// <returns>value back to the template</returns>
		public object Invoke(string method, params object[] args)
		{
			return InternalInvoke(method, args);
		}

		#endregion
	}
}