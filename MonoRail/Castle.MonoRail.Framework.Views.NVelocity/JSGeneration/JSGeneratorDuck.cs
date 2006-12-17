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

using NVelocity;

namespace Castle.MonoRail.Framework.Views.NVelocity.JSGeneration
{
	using System;
	using Castle.MonoRail.Framework.Helpers;

	public class JSGeneratorDuck : IDuck
	{
		private readonly PrototypeHelper.JSGenerator generator;

		/// <summary>
		/// Initializes a new instance of the <see cref="JSGeneratorDuck"/> class.
		/// </summary>
		/// <param name="generator">The generator.</param>
		public JSGeneratorDuck(PrototypeHelper.JSGenerator generator)
		{
			this.generator = generator;
		}

		#region IDuck

		/// <summary>
		/// Defines the behavior when a property is read
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <returns>value back to the template</returns>
		public object GetInvoke(string propName)
		{
			// return new JSElementGeneratorDuck(generator, propName);
			return null;
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
			if (method == "el")
			{
				if (args == null || args.Length != 1)
				{
					throw new ArgumentException("el() method must be invoked with the element name as an argument");
				}
				if (args[0] == null)
				{
					throw new ArgumentNullException("el() method null invoked with a null argument");
				}

				return new JSElementGeneratorDuck(this, generator, args[0].ToString());
			}

			if (PrototypeHelper.JSGenerator.IsGeneratorMethod(method))
			{
				PrototypeHelper.JSGenerator.Dispatch(generator, method, args);
			}

			return null;
		}

		#endregion

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
	}

	/// <summary>
	/// 
	/// </summary>
	public class JSElementGeneratorDuck : IDuck
	{
		private readonly IDuck parent;
		private readonly PrototypeHelper.JSGenerator generator;

		public JSElementGeneratorDuck(IDuck parent, PrototypeHelper.JSGenerator generator, string root)
		{
			this.parent = parent;

			this.generator = generator;

			PrototypeHelper.JSGenerator.Write(generator, "$('" + root + "')");
		}

		#region IDuck

		/// <summary>
		/// Defines the behavior when a property is read
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <returns>value back to the template</returns>
		public object GetInvoke(string propName)
		{
			PrototypeHelper.JSGenerator.ReplaceTailByPeriod(generator);
			PrototypeHelper.JSGenerator.Write(generator, propName);

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
			if (method == "set")
			{
				PrototypeHelper.JSGenerator.Write(generator, " = " + args[0]);

				return null;
			}
			else
			{
				PrototypeHelper.JSGenerator.ReplaceTailByPeriod(generator);

				if (PrototypeHelper.JSGenerator.IsGeneratorMethod(method))
				{
					PrototypeHelper.JSGenerator.Dispatch(generator, method, args);
				}
				else
				{
					PrototypeHelper.JSGenerator.Call(generator, method, args);
				}

				return this;
			}
		}

		#endregion
	}
}
