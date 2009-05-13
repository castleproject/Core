// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Interceptor
{
	using System;
	using System.Reflection;

	/// <summary>
	/// New interface that is going to be used by DynamicProxy 2
	/// </summary>
	public interface IInvocation
	{
		object Proxy { get; }

		object InvocationTarget { get; }

		Type TargetType { get; }

		object[] Arguments { get; }

		void SetArgumentValue(int index, object value);

		object GetArgumentValue(int index);

		/// <summary>
		/// The generic arguments of the method, or null if not a generic method.
		/// </summary>
		Type[] GenericArguments { get; }

		/// <summary>
		/// 
		/// </summary>
		MethodInfo Method { get; }

		/// <summary>
		/// Returns the concrete instantiation of <see cref="Method"/>, with any generic parameters bound to real types.
		/// </summary>
		/// <returns>The concrete instantiation of <see cref="Method"/>, or <see cref="Method"/> if not a generic method.</returns>
		/// <remarks>Can be slower than calling <see cref="Method"/>.</remarks>
		MethodInfo GetConcreteMethod();

		/// <summary>
		/// For interface proxies, this will point to the
		/// <see cref="MethodInfo"/> on the target class
		/// </summary>
		MethodInfo MethodInvocationTarget { get; }

		/// <summary>
		/// Returns the concrete instantiation of <see cref="MethodInvocationTarget"/>, with any generic parameters bound to real types.
		/// </summary>
		/// <returns>The concrete instantiation of <see cref="MethodInvocationTarget"/>, or <see cref="MethodInvocationTarget"/> if not a generic method.</returns>
		/// <remarks>Can be slower than calling <see cref="MethodInvocationTarget"/>.</remarks>
		MethodInfo GetConcreteMethodInvocationTarget();

		object ReturnValue { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		void Proceed();
	}
}