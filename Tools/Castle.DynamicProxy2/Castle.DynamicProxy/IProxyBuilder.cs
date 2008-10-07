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

namespace Castle.DynamicProxy
{
	using System;

	/// <summary>
	/// Abstracts the implementation of proxy constructions
	/// </summary>
	public interface IProxyBuilder
	{
		/// <summary>
		/// Gets the module scope used by this builder for generating code.
		/// </summary>
		/// <value>The module scope used by this builder.</value>
		ModuleScope ModuleScope { get; }

		/// <summary>
		/// Implementors should return a proxy for the specified type.
		/// </summary>
		/// <param name="theClass">The proxy base class.</param>
		/// <param name="options">The proxy generation options.</param>
		/// <returns>The generated proxy type.</returns>
		Type CreateClassProxy(Type theClass, ProxyGenerationOptions options);

		/// <summary>
		/// Implementors should return a proxy for the specified
		/// type and interfaces. The interfaces must be only "mark" interfaces
		/// </summary>
		/// <param name="theClass"></param>
		/// <param name="interfaces"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Type CreateClassProxy(Type theClass, Type[] interfaces, ProxyGenerationOptions options);

		/// <summary>
		/// Implementors should return a proxy for the specified
		/// interface that 'proceeds' executions to the 
		/// specified target.
		/// </summary>
		/// <param name="theInterface"></param>
		/// <param name="interfaces"></param>
		/// <param name="targetType"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Type CreateInterfaceProxyTypeWithTarget(Type theInterface, Type[] interfaces, Type targetType,
		                                        ProxyGenerationOptions options);

		/// <summary>
		/// Implementors should return a proxy for the specified
		/// interface that delegate all executions to the 
		/// specified interceptor(s).
		/// </summary>
		/// <param name="theInterface"></param>
		/// <param name="interfaces"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Type CreateInterfaceProxyTypeWithoutTarget(Type theInterface, Type[] interfaces, ProxyGenerationOptions options);

		/// <summary>
		/// Implementors should return a proxy for the specified
		/// interface(s) that delegate all executions to the
		/// specified interceptor(s) and uses an instance of the interface
		/// as their targets, rather than a class. All IInvocation's
		/// should then implement IChangeProxyTarget.
		/// </summary>
		/// <param name="theInterface"></param>
		/// <param name="interfaces"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Type CreateInterfaceProxyTypeWithTargetInterface(Type theInterface, Type[] interfaces, ProxyGenerationOptions options);
	}
}
