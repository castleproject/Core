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
	using System.Runtime.CompilerServices;

	using Castle.Core.Logging;
	using Castle.DynamicProxy.Generators;

	/// <summary>
	///   Abstracts the implementation of proxy type construction.
	/// </summary>
	public interface IProxyBuilder
	{
		/// <summary>
		///   Gets or sets the <see cref = "ILogger" /> that this <see cref = "ProxyGenerator" /> logs to.
		/// </summary>
		ILogger Logger { get; set; }

		/// <summary>
		///   Gets the <see cref = "ModuleScope" /> associated with this builder.
		/// </summary>
		/// <value>The module scope associated with this builder.</value>
		ModuleScope ModuleScope { get; }

		/// <summary>
		///   Creates a proxy type for given <paramref name = "classToProxy" />, implementing <paramref
		///    name = "additionalInterfacesToProxy" />, using <paramref name = "options" /> provided.
		/// </summary>
		/// <param name = "classToProxy">The class type to proxy.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types to proxy.</param>
		/// <param name = "options">The proxy generation options.</param>
		/// <returns>The generated proxy type.</returns>
		/// <remarks>
		///   Implementers should return a proxy type for the specified class and interfaces.
		///   Additional interfaces should be only 'mark' interfaces, that is, they should work like interface proxy without target. (See <see
		///    cref = "CreateInterfaceProxyTypeWithoutTarget" /> method.)
		/// </remarks>
		/// <exception cref = "GeneratorException">Thrown when <paramref name = "classToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "GeneratorException">Thrown when <paramref name = "classToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is not public.
		///   Note that to avoid this exception, you can mark offending type internal, and define <see
		///    cref = "InternalsVisibleToAttribute" /> 
		///   pointing to Castle Dynamic Proxy assembly, in assembly containing that type, if this is appropriate.</exception>
		/// <seealso cref = "ClassProxyGenerator" />
		Type CreateClassProxyType(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options);

		Type CreateClassProxyTypeWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy,
		                                    ProxyGenerationOptions options);

		/// <summary>
		///   Creates a proxy type that proxies calls to <paramref name = "interfaceToProxy" /> members on <paramref
		///    name = "targetType" />, implementing <paramref name = "additionalInterfacesToProxy" />, using <paramref
		///    name = "options" /> provided.
		/// </summary>
		/// <param name = "interfaceToProxy">The interface type to proxy.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types to proxy.</param>
		/// <param name = "targetType">Type implementing <paramref name = "interfaceToProxy" /> on which calls to the interface members should be intercepted.</param>
		/// <param name = "options">The proxy generation options.</param>
		/// <returns>The generated proxy type.</returns>
		/// <remarks>
		///   Implementers should return a proxy type for the specified interface that 'proceeds' executions to the specified target.
		///   Additional interfaces should be only 'mark' interfaces, that is, they should work like interface proxy without target. (See <see
		///    cref = "CreateInterfaceProxyTypeWithoutTarget" /> method.)
		/// </remarks>
		/// <exception cref = "GeneratorException">Thrown when <paramref name = "interfaceToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "GeneratorException">Thrown when <paramref name = "interfaceToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is not public.
		///   Note that to avoid this exception, you can mark offending type internal, and define <see
		///    cref = "InternalsVisibleToAttribute" /> 
		///   pointing to Castle Dynamic Proxy assembly, in assembly containing that type, if this is appropriate.</exception>
		/// <seealso cref = "InterfaceProxyWithTargetGenerator" />
		Type CreateInterfaceProxyTypeWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy, Type targetType,
		                                        ProxyGenerationOptions options);

		/// <summary>
		///   Creates a proxy type for given <paramref name = "interfaceToProxy" /> and <parmaref
		///    name = "additionalInterfacesToProxy" /> that delegates all calls to the provided interceptors and allows interceptors to switch the actual target of invocation.
		/// </summary>
		/// <param name = "interfaceToProxy">The interface type to proxy.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types to proxy.</param>
		/// <param name = "options">The proxy generation options.</param>
		/// <returns>The generated proxy type.</returns>
		/// <remarks>
		///   Implementers should return a proxy type for the specified interface(s) that delegate all executions to the specified interceptors
		///   and uses an instance of the interface as their targets (i.e. <see cref = "IInvocation.InvocationTarget" />), rather than a class. All <see
		///    cref = "IInvocation" /> classes should then implement <see cref = "IChangeProxyTarget" /> interface,
		///   to allow interceptors to switch invocation target with instance of another type implementing called interface.
		/// </remarks>
		/// <exception cref = "GeneratorException">Thrown when <paramref name = "interfaceToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "GeneratorException">Thrown when <paramref name = "interfaceToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is not public.
		///   Note that to avoid this exception, you can mark offending type internal, and define <see
		///    cref = "InternalsVisibleToAttribute" /> 
		///   pointing to Castle Dynamic Proxy assembly, in assembly containing that type, if this is appropriate.</exception>
		/// <seealso cref = "InterfaceProxyWithTargetInterfaceGenerator" />
		Type CreateInterfaceProxyTypeWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                                 ProxyGenerationOptions options);

		/// <summary>
		///   Creates a proxy type for given <paramref name = "interfaceToProxy" /> that delegates all calls to the provided interceptors.
		/// </summary>
		/// <param name = "interfaceToProxy">The interface type to proxy.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types to proxy.</param>
		/// <param name = "options">The proxy generation options.</param>
		/// <returns>The generated proxy type.</returns>
		/// <remarks>
		///   Implementers should return a proxy type for the specified interface and additional interfaces that delegate all executions to the specified interceptors.
		/// </remarks>
		/// <exception cref = "GeneratorException">Thrown when <paramref name = "interfaceToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "GeneratorException">Thrown when <paramref name = "interfaceToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is not public.
		///   Note that to avoid this exception, you can mark offending type internal, and define <see
		///    cref = "InternalsVisibleToAttribute" /> 
		///   pointing to Castle Dynamic Proxy assembly, in assembly containing that type, if this is appropriate.</exception>
		/// <seealso cref = "InterfaceProxyWithoutTargetGenerator" />
		Type CreateInterfaceProxyTypeWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                           ProxyGenerationOptions options);
	}
}