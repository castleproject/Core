// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System;
	using System.Reflection;

	/// <summary>
	///   Encapsulates an invocation of a proxied method.
	/// </summary>
	public interface IInvocation
	{
		/// <summary>
		///   Gets the arguments that the <see cref = "Method" /> has been invoked with.
		/// </summary>
		/// <value>The arguments the method was invoked with.</value>
		object[] Arguments { get; }

		/// <summary>
		///   Gets the generic arguments of the method.
		/// </summary>
		/// <value>The generic arguments, or null if not a generic method.</value>
		Type[] GenericArguments { get; }

		/// <summary>
		///   Gets the object on which the invocation is performed. This is different from proxy object
		///   because most of the time this will be the proxy target object.
		/// </summary>
		/// <seealso cref = "IChangeProxyTarget" />
		/// <value>The invocation target.</value>
		object InvocationTarget { get; }

		/// <summary>
		///   Gets the <see cref = "MethodInfo" /> representing the method being invoked on the proxy.
		/// </summary>
		/// <value>The <see cref = "MethodInfo" /> representing the method being invoked.</value>
		MethodInfo Method { get; }

		/// <summary>
		///   For interface proxies, this will point to the <see cref = "MethodInfo" /> on the target class.
		/// </summary>
		/// <value>The method invocation target.</value>
		MethodInfo MethodInvocationTarget { get; }

		/// <summary>
		///   Gets the proxy object on which the intercepted method is invoked.
		/// </summary>
		/// <value>Proxy object on which the intercepted method is invoked.</value>
		object Proxy { get; }

		/// <summary>
		///   Gets or sets the return value of the method.
		/// </summary>
		/// <value>The return value of the method.</value>
		object ReturnValue { get; set; }

		/// <summary>
		///   Gets the type of the target object for the intercepted method.
		/// </summary>
		/// <value>The type of the target object.</value>
		Type TargetType { get; }

		/// <summary>
		///   Gets the value of the argument at the specified <paramref name = "index" />.
		/// </summary>
		/// <param name = "index">The index.</param>
		/// <returns>The value of the argument at the specified <paramref name = "index" />.</returns>
		object GetArgumentValue(int index);

		/// <summary>
		///   Returns the concrete instantiation of the <see cref = "Method" /> on the proxy, with any generic
		///   parameters bound to real types.
		/// </summary>
		/// <returns>
		///   The concrete instantiation of the <see cref = "Method" /> on the proxy, or the <see cref = "Method" /> if
		///   not a generic method.
		/// </returns>
		/// <remarks>
		///   Can be slower than calling <see cref = "Method" />.
		/// </remarks>
		MethodInfo GetConcreteMethod();

		/// <summary>
		///   Returns the concrete instantiation of <see cref = "MethodInvocationTarget" />, with any
		///   generic parameters bound to real types.
		///   For interface proxies, this will point to the <see cref = "MethodInfo" /> on the target class.
		/// </summary>
		/// <returns>The concrete instantiation of <see cref = "MethodInvocationTarget" />, or
		///   <see cref = "MethodInvocationTarget" /> if not a generic method.</returns>
		/// <remarks>
		///   In debug builds this can be slower than calling <see cref = "MethodInvocationTarget" />.
		/// </remarks>
		MethodInfo GetConcreteMethodInvocationTarget();

		/// <summary>
		///   Proceeds the call to the next interceptor in line, and ultimately to the target method.
		/// </summary>
		/// <remarks>
		///   Since interface proxies without a target don't have the target implementation to proceed to,
		///   it is important, that the last interceptor does not call this method, otherwise a
		///   <see cref = "NotImplementedException" /> will be thrown.
		/// </remarks>
		void Proceed();

		/// <summary>
		///   Overrides the value of an argument at the given <paramref name = "index" /> with the
		///   new <paramref name = "value" /> provided.
		/// </summary>
		/// <remarks>
		///   This method accepts an <see cref = "object" />, however the value provided must be compatible
		///   with the type of the argument defined on the method, otherwise an exception will be thrown.
		/// </remarks>
		/// <param name = "index">The index of the argument to override.</param>
		/// <param name = "value">The new value for the argument.</param>
		void SetArgumentValue(int index, object value);
	}
}