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
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	/// The DynamicDispatcher has the ability of late bound and dispatch method invocation
	/// to several targets. The intention is mimic what you can achieve with dynamic languages
	/// and mixins.
	/// </summary>
	public class DynamicDispatcher
	{
		private readonly Dictionary<string, MethodTarget> method2Target;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicDispatcher"/> class.
		/// </summary>
		/// <param name="mainTarget">The main target.</param>
		/// <param name="extensions">The extensions.</param>
		public DynamicDispatcher(object mainTarget, params object[] extensions)
		{
			method2Target = new Dictionary<string, MethodTarget>(StringComparer.InvariantCultureIgnoreCase);

			// We can create a cache of operations per type here as inspecting types over and over is very time consuming

			CollectOperations(mainTarget);

			foreach(object extension in extensions)
			{
				CollectOperations(extension);
			}
		}

		/// <summary>
		/// Determines whether the specified instance has the method.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <returns>
		/// 	<c>true</c> if the method was registered; otherwise, <c>false</c>.
		/// </returns>
		public bool HasMethod(string methodName)
		{
			return method2Target.ContainsKey(methodName);
		}

		/// <summary>
		/// Dispatches the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public object Dispatch(string method, params object[] args)
		{
			MethodTarget target;
			if (!method2Target.TryGetValue(method, out target))
			{
				throw new InvalidOperationException("Method " + method + " not found for dynamic dispatching");
			}

			MethodInfo methodInfo = target.Method;

			ParameterInfo[] parameters = methodInfo.GetParameters();

			int paramArrayIndex = -1;

			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo paramInfo = parameters[i];

				if (paramInfo.IsDefined(typeof(ParamArrayAttribute), true))
				{
					paramArrayIndex = i;
				}
			}

			try
			{
				return methodInfo.Invoke(target.Target, BuildMethodArgs(methodInfo, args, paramArrayIndex));
			}
			catch(MonoRailException)
			{
				throw;
			}
			catch(Exception ex)
			{
				throw new Exception("Error invoking method on generator. " +
				                    "Method invoked [" + method + "] with " + args.Length + " argument(s)", ex);
			}
		}

		private void CollectOperations(object target)
		{
			foreach (MethodInfo method in target.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
			{
				if (!method.IsDefined(typeof(DynamicOperationAttribute), true))
				{
					continue;
				}

				method2Target[method.Name] = new MethodTarget(target, method);
			}
		}

		private object[] BuildMethodArgs(MethodInfo method, object[] methodArguments, int paramArrayIndex)
		{
			if (methodArguments == null) return new object[0];

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

		private class MethodTarget
		{
			private readonly object target;
			private readonly MethodInfo method;

			public MethodTarget(object target, MethodInfo method)
			{
				this.target = target;
				this.method = method;
			}

			public object Target
			{
				get { return target; }
			}

			public MethodInfo Method
			{
				get { return method; }
			}
		}
	}
}