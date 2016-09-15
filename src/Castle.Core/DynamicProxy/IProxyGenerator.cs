// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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

	using Castle.Core.Logging;

	/// <summary>
	///   Provides proxy objects for classes and interfaces.
	/// </summary>
	[CLSCompliant(true)]
	public interface IProxyGenerator
	{
		/// <summary>
		///   Gets or sets the <see cref = "ILogger" /> that this <see cref = "ProxyGenerator" /> log to.
		/// </summary>
		ILogger Logger { get; set; }

		/// <summary>
		///   Gets the proxy builder instance used to generate proxy types.
		/// </summary>
		/// <value>The proxy builder.</value>
		IProxyBuilder ProxyBuilder { get; }

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <typeparamref name = "TInterface" /> on <paramref
		///    name = "target" /> object with given <paramref name = "interceptors" />.
		/// </summary>
		/// <typeparam name = "TInterface">Type of the interface implemented by <paramref name = "target" /> which will be proxied.</typeparam>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>Object proxying calls to members of <typeparamref name = "TInterface" /> on <paramref name = "target" /> object.</returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "target" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <typeparamref name = "TInterface" />is not an interface type.</exception>
		/// <exception cref = "MissingMethodException">Thrown when no default constructor exists on actual type of <paramref
		///    name = "target" /> object.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of actual type of <paramref
		///    name = "target" /> throws an exception.</exception>
		/// <remarks>
		///   This method generates new proxy type for each type of <paramref name = "target" />, which affects performance. If you don't want to proxy types differently depending on the type of the target
		///   use <see cref = "CreateInterfaceProxyWithTargetInterface{TInterface}(TInterface,IInterceptor[])" /> method.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, params IInterceptor[] interceptors)
			where TInterface : class;

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <typeparamref name = "TInterface" /> on <paramref
		///    name = "target" /> object with given <paramref name = "interceptors" />.
		/// </summary>
		/// <typeparam name = "TInterface">Type of the interface implemented by <paramref name = "target" /> which will be proxied.</typeparam>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <typeparamref name = "TInterface" /> on <paramref name = "target" /> object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "target" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <typeparamref name = "TInterface" />is not an interface type.</exception>
		/// <exception cref = "MissingMethodException">Thrown when no default constructor exists on actual type of <paramref
		///    name = "target" /> object.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of actual type of <paramref
		///    name = "target" /> throws an exception.</exception>
		/// <remarks>
		///   This method generates new proxy type for each type of <paramref name = "target" />, which affects performance. If you don't want to proxy types differently depending on the type of the target
		///   use <see
		///    cref = "CreateInterfaceProxyWithTargetInterface{TInterface}(TInterface,Castle.DynamicProxy.ProxyGenerationOptions,IInterceptor[])" /> method.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, ProxyGenerationOptions options,
		                                                      params IInterceptor[] interceptors)
			where TInterface : class;

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on <paramref
		///    name = "target" /> object with given <paramref name = "interceptors" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface implemented by <paramref name = "target" /> which will be proxied.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> type on <paramref name = "target" /> object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "target" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "target" /> does not implement <paramref
		///    name = "interfaceToProxy" /> interface.</exception>
		/// <exception cref = "MissingMethodException">Thrown when no default constructor exists on actual type of <paramref
		///    name = "target" /> object.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of actual type of <paramref
		///    name = "target" /> throws an exception.</exception>
		/// <remarks>
		///   This method generates new proxy type for each type of <paramref name = "target" />, which affects performance. If you don't want to proxy types differently depending on the type of the target
		///   use <see cref = "CreateInterfaceProxyWithTargetInterface(Type,object,IInterceptor[])" /> method.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithTarget(Type interfaceToProxy, object target, params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on <paramref
		///    name = "target" /> object with given <paramref name = "interceptors" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface implemented by <paramref name = "target" /> which will be proxied.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> type on <paramref name = "target" /> object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "target" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "target" /> does not implement <paramref
		///    name = "interfaceToProxy" /> interface.</exception>
		/// <exception cref = "MissingMethodException">Thrown when no default constructor exists on actual type of <paramref
		///    name = "target" /> object.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of actual type of <paramref
		///    name = "target" /> throws an exception.</exception>
		/// <remarks>
		///   This method generates new proxy type for each type of <paramref name = "target" />, which affects performance. If you don't want to proxy types differently depending on the type of the target
		///   use <see cref = "CreateInterfaceProxyWithTargetInterface(Type,object,ProxyGenerationOptions,IInterceptor[])" /> method.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithTarget(Type interfaceToProxy, object target, ProxyGenerationOptions options,
		                                      params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on <paramref
		///    name = "target" /> object with given <paramref name = "interceptors" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface implemented by <paramref name = "target" /> which will be proxied.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> and <paramref
		///    name = "additionalInterfacesToProxy" /> types  on <paramref name = "target" /> object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "target" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "target" /> does not implement <paramref
		///    name = "interfaceToProxy" /> interface.</exception>
		/// <exception cref = "MissingMethodException">Thrown when no default constructor exists on actual type of <paramref
		///    name = "target" /> object.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of actual type of <paramref
		///    name = "target" /> throws an exception.</exception>
		/// <remarks>
		///   This method generates new proxy type for each type of <paramref name = "target" />, which affects performance. If you don't want to proxy types differently depending on the type of the target
		///   use <see cref = "CreateInterfaceProxyWithTargetInterface(Type,Type[],object,IInterceptor[])" /> method.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy, object target,
		                                      params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on <paramref
		///    name = "target" /> object with given <paramref name = "interceptors" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface implemented by <paramref name = "target" /> which will be proxied.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> and <paramref
		///    name = "additionalInterfacesToProxy" /> types on <paramref name = "target" /> object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "target" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "target" /> does not implement <paramref
		///    name = "interfaceToProxy" /> interface.</exception>
		/// <exception cref = "MissingMethodException">Thrown when no default constructor exists on actual type of <paramref
		///    name = "target" /> object.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of actual type of <paramref
		///    name = "target" /> throws an exception.</exception>
		/// <remarks>
		///   This method generates new proxy type for each type of <paramref name = "target" />, which affects performance. If you don't want to proxy types differently depending on the type of the target
		///   use <see cref = "CreateInterfaceProxyWithTargetInterface(Type,Type[],object,ProxyGenerationOptions,IInterceptor[])" /> method.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                      object target,
		                                      ProxyGenerationOptions options,
		                                      params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on <paramref
		///    name = "target" /> object with given <paramref name = "interceptors" />.
		///   Interceptors can use <see cref = "IChangeProxyTarget" /> interface to provide other target for method invocation than default <paramref
		///    name = "target" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface implemented by <paramref name = "target" /> which will be proxied.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> type on <paramref name = "target" /> object or alternative implementation swapped at runtime by an interceptor.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "target" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "target" /> does not implement <paramref
		///    name = "interfaceToProxy" /> interface.</exception>
		/// <exception cref = "MissingMethodException">Thrown when no default constructor exists on actual type of <paramref
		///    name = "target" /> object.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of actual type of <paramref
		///    name = "target" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithTargetInterface(Type interfaceToProxy, object target,
		                                               params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <typeparamref name = "TInterface" /> on <paramref
		///    name = "target" /> object with given <paramref name = "interceptors" />.
		///   Interceptors can use <see cref = "IChangeProxyTarget" /> interface to provide other target for method invocation than default <paramref
		///    name = "target" />.
		/// </summary>
		/// <typeparam name = "TInterface">Type of the interface implemented by <paramref name = "target" /> which will be proxied.</typeparam>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <typeparamref name = "TInterface" /> type on <paramref name = "target" /> object or alternative implementation swapped at runtime by an interceptor.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "target" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <typeparamref name = "TInterface" /> is not an interface type.</exception>
		/// <exception cref = "MissingMethodException">Thrown when no default constructor exists on actual type of <paramref
		///    name = "target" /> object.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of actual type of <paramref
		///    name = "target" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		TInterface CreateInterfaceProxyWithTargetInterface<TInterface>(TInterface target,
		                                                               params IInterceptor[] interceptors)
			where TInterface : class;

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <typeparamref name = "TInterface" /> on <paramref
		///    name = "target" /> object with given <paramref name = "interceptors" />.
		///   Interceptors can use <see cref = "IChangeProxyTarget" /> interface to provide other target for method invocation than default <paramref
		///    name = "target" />.
		/// </summary>
		/// <typeparam name = "TInterface">Type of the interface implemented by <paramref name = "target" /> which will be proxied.</typeparam>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <typeparamref name = "TInterface" /> type on <paramref name = "target" /> object or alternative implementation swapped at runtime by an interceptor.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "target" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <typeparamref name = "TInterface" /> is not an interface type.</exception>
		/// <exception cref = "MissingMethodException">Thrown when no default constructor exists on actual type of <paramref
		///    name = "target" /> object.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of actual type of <paramref
		///    name = "target" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		TInterface CreateInterfaceProxyWithTargetInterface<TInterface>(TInterface target,
		                                                               ProxyGenerationOptions options,
		                                                               params IInterceptor[] interceptors)
			where TInterface : class;

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on <paramref
		///    name = "target" /> object with given <paramref name = "interceptors" />.
		///   Interceptors can use <see cref = "IChangeProxyTarget" /> interface to provide other target for method invocation than default <paramref
		///    name = "target" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface implemented by <paramref name = "target" /> which will be proxied.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> and <paramref
		///    name = "additionalInterfacesToProxy" /> types on <paramref name = "target" /> object or alternative implementation swapped at runtime by an interceptor.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "target" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "target" /> does not implement <paramref
		///    name = "interfaceToProxy" /> interface.</exception>
		/// <exception cref = "MissingMethodException">Thrown when no default constructor exists on actual type of <paramref
		///    name = "target" /> object.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of actual type of <paramref
		///    name = "target" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                               object target, params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on <paramref
		///    name = "target" /> object with given <paramref name = "interceptors" />.
		///   Interceptors can use <see cref = "IChangeProxyTarget" /> interface to provide other target for method invocation than default <paramref
		///    name = "target" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface implemented by <paramref name = "target" /> which will be proxied.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> type on <paramref name = "target" /> object or alternative implementation swapped at runtime by an interceptor.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "target" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "target" /> does not implement <paramref
		///    name = "interfaceToProxy" /> interface.</exception>
		/// <exception cref = "MissingMethodException">Thrown when no default constructor exists on actual type of <paramref
		///    name = "target" /> object.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of actual type of <paramref
		///    name = "target" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithTargetInterface(Type interfaceToProxy, object target,
		                                               ProxyGenerationOptions options,
		                                               params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on <paramref name = "target" /> object with given <paramref name = "interceptors" />.
		///   Interceptors can use <see cref = "IChangeProxyTarget" /> interface to provide other target for method invocation than default <paramref name = "target" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface implemented by <paramref name = "target" /> which will be proxied.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> and <paramref name = "additionalInterfacesToProxy" /> types on <paramref name = "target" /> object or alternative implementation swapped at runtime by an interceptor.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "target" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> or any of <paramref name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "target" /> does not implement <paramref name = "interfaceToProxy" /> interface.</exception>
		/// <exception cref = "MissingMethodException">Thrown when no default constructor exists on actual type of <paramref name = "target" /> object.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of actual type of <paramref name = "target" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithTargetInterface(Type interfaceToProxy,
		                                               Type[] additionalInterfacesToProxy,
		                                               object target, ProxyGenerationOptions options,
		                                               params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <typeparamref name = "TInterface" /> on target object generated at runtime with given <paramref
		///    name = "interceptor" />.
		/// </summary>
		/// <typeparam name = "TInterface">Type of the interface which will be proxied.</typeparam>
		/// <param name = "interceptor">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <typeparamref name = "TInterface" /> types on generated target object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptor" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <typeparamref name = "TInterface" /> is not an interface type.</exception>
		/// <remarks>
		///   Since this method uses an empty-shell implementation of interfaces to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see
		///    cref = "IInterceptor" /> implementations.
		///   They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see
		///    cref = "IInvocation.Proceed" />, since there's no actual implementation to proceed with.
		///   As a result of that also at least one <see cref = "IInterceptor" /> implementation must be provided.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		TInterface CreateInterfaceProxyWithoutTarget<TInterface>(IInterceptor interceptor)
			where TInterface : class;

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <typeparamref name = "TInterface" /> on target object generated at runtime with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <typeparam name = "TInterface">Type of the interface which will be proxied.</typeparam>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <typeparamref name = "TInterface" /> types on generated target object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <typeparamref name = "TInterface" /> is not an interface type.</exception>
		/// <remarks>
		///   Since this method uses an empty-shell implementation of interfaces to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see
		///    cref = "IInterceptor" /> implementations.
		///   They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see
		///    cref = "IInvocation.Proceed" />, since there's no actual implementation to proceed with.
		///   As a result of that also at least one <see cref = "IInterceptor" /> implementation must be provided.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		TInterface CreateInterfaceProxyWithoutTarget<TInterface>(params IInterceptor[] interceptors)
			where TInterface : class;

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <typeparamref name = "TInterface" /> on target object generated at runtime with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <typeparam name = "TInterface">Type of the interface which will be proxied.</typeparam>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <typeparamref name = "TInterface" /> types on generated target object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <typeparamref name = "TInterface" /> is not an interface type.</exception>
		/// <remarks>
		///   Since this method uses an empty-shell implementation of interfaces to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see
		///    cref = "IInterceptor" /> implementations.
		///   They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see
		///    cref = "IInvocation.Proceed" />, since there's no actual implementation to proceed with.
		///   As a result of that also at least one <see cref = "IInterceptor" /> implementation must be provided.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		TInterface CreateInterfaceProxyWithoutTarget<TInterface>(ProxyGenerationOptions options,
		                                                         params IInterceptor[] interceptors)
			where TInterface : class;

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on target object generated at runtime with given <paramref
		///    name = "interceptor" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface which will be proxied.</param>
		/// <param name = "interceptor">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> type on generated target object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptor" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <remarks>
		///   Since this method uses an empty-shell implementation of interfaces to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see
		///    cref = "IInterceptor" /> implementations.
		///   They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see
		///    cref = "IInvocation.Proceed" />, since there's no actual implementation to proceed with.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, IInterceptor interceptor);

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on target object generated at runtime with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface which will be proxied.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> type on generated target object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <remarks>
		///   Since this method uses an empty-shell implementation of interfaces to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see
		///    cref = "IInterceptor" /> implementations.
		///   They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see
		///    cref = "IInvocation.Proceed" />, since there's no actual implementation to proceed with.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on target object generated at runtime with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface which will be proxied.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> and <paramref
		///    name = "additionalInterfacesToProxy" /> types on generated target object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <remarks>
		///   Since this method uses an empty-shell implementation of interfaces to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see
		///    cref = "IInterceptor" /> implementations.
		///   They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see
		///    cref = "IInvocation.Proceed" />, since there's no actual implementation to proceed with.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                         params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on target object generated at runtime with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface which will be proxied.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> on generated target object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" />  is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <remarks>
		///   They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see
		///    cref = "IInvocation.Proceed" />, since there's no actual implementation to proceed with.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, ProxyGenerationOptions options,
		                                         params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to members of interface <paramref name = "interfaceToProxy" /> on target object generated at runtime with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "interfaceToProxy">Type of the interface which will be proxied.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   Object proxying calls to members of <paramref name = "interfaceToProxy" /> and <paramref
		///    name = "additionalInterfacesToProxy" /> types on generated target object.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interfaceToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "interceptors" /> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "interfaceToProxy" /> is not an interface type.</exception>
		/// <remarks>
		///   Since this method uses an empty-shell implementation of <paramref name = "additionalInterfacesToProxy" /> to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see
		///    cref = "IInterceptor" /> implementations.
		///   They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see
		///    cref = "IInvocation.Proceed" />, since there's no actual implementation to proceed with.
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                         ProxyGenerationOptions options,
		                                         params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <typeparamref name = "TClass" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <typeparam name = "TClass">Type of class which will be proxied.</typeparam>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <typeparamref name = "TClass" /> proxying calls to virtual members of <typeparamref
		///    name = "TClass" /> type.
		/// </returns>
		/// <exception cref = "ArgumentException">Thrown when given <typeparamref name = "TClass" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no default constructor exists on type <typeparamref name = "TClass" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of type <typeparamref name = "TClass" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		TClass CreateClassProxyWithTarget<TClass>(TClass target, params IInterceptor[] interceptors)
			where TClass : class;

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <typeparamref name = "TClass" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <typeparam name = "TClass">Type of class which will be proxied.</typeparam>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <typeparamref name = "TClass" /> proxying calls to virtual members of <typeparamref
		///    name = "TClass" /> type.
		/// </returns>
		/// <exception cref = "ArgumentException">Thrown when given <typeparamref name = "TClass" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no default constructor exists on type <typeparamref name = "TClass" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of type <typeparamref name = "TClass" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		TClass CreateClassProxyWithTarget<TClass>(TClass target, ProxyGenerationOptions options,
		                                          params IInterceptor[] interceptors) where TClass : class;

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> and <paramref name = "additionalInterfacesToProxy" /> types.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no default constructor exists on type <paramref name = "classToProxy" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxyWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy, object target,
		                                  params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "constructorArguments">Arguments of constructor of type <paramref name = "classToProxy" /> which should be used to create a new instance of that type.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> type.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no constructor exists on type <paramref name = "classToProxy" /> with parameters matching <paramref
		///    name = "constructorArguments" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxyWithTarget(Type classToProxy, object target, ProxyGenerationOptions options,
		                                  object[] constructorArguments, params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "constructorArguments">Arguments of constructor of type <paramref name = "classToProxy" /> which should be used to create a new instance of that type.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> type.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no constructor exists on type <paramref name = "classToProxy" /> with parameters matching <paramref
		///    name = "constructorArguments" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxyWithTarget(Type classToProxy, object target, object[] constructorArguments,
		                                  params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> type.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no parameterless constructor exists on type <paramref
		///    name = "classToProxy" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxyWithTarget(Type classToProxy, object target, params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> type.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "options" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no default constructor exists on type <paramref name = "classToProxy" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxyWithTarget(Type classToProxy, object target, ProxyGenerationOptions options,
		                                  params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> and <paramref name = "additionalInterfacesToProxy" /> types.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "options" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no default constructor exists on type <paramref name = "classToProxy" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxyWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy, object target,
		                                  ProxyGenerationOptions options, params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name = "target">The target object, calls to which will be intercepted.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "constructorArguments">Arguments of constructor of type <paramref name = "classToProxy" /> which should be used to create a new instance of that type.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> and <paramref name = "additionalInterfacesToProxy" /> types.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "options" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no constructor exists on type <paramref name = "classToProxy" /> with parameters matching <paramref
		///    name = "constructorArguments" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxyWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy, object target,
		                                  ProxyGenerationOptions options, object[] constructorArguments,
		                                  params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <typeparamref name = "TClass" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <typeparam name = "TClass">Type of class which will be proxied.</typeparam>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <typeparamref name = "TClass" /> proxying calls to virtual members of <typeparamref
		///    name = "TClass" /> type.
		/// </returns>
		/// <exception cref = "ArgumentException">Thrown when given <typeparamref name = "TClass" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no default constructor exists on type <typeparamref name = "TClass" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of type <typeparamref name = "TClass" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class;

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <typeparamref name = "TClass" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <typeparam name = "TClass">Type of class which will be proxied.</typeparam>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <typeparamref name = "TClass" /> proxying calls to virtual members of <typeparamref
		///    name = "TClass" /> type.
		/// </returns>
		/// <exception cref = "ArgumentException">Thrown when given <typeparamref name = "TClass" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no default constructor exists on type <typeparamref name = "TClass" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of type <typeparamref name = "TClass" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		TClass CreateClassProxy<TClass>(ProxyGenerationOptions options, params IInterceptor[] interceptors)
			where TClass : class;

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> and <paramref name = "additionalInterfacesToProxy" /> types.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no default constructor exists on type <paramref name = "classToProxy" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy,
		                        params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "constructorArguments">Arguments of constructor of type <paramref name = "classToProxy" /> which should be used to create a new instance of that type.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> type.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no constructor exists on type <paramref name = "classToProxy" /> with parameters matching <paramref
		///    name = "constructorArguments" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxy(Type classToProxy, ProxyGenerationOptions options, object[] constructorArguments,
		                        params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "constructorArguments">Arguments of constructor of type <paramref name = "classToProxy" /> which should be used to create a new instance of that type.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> type.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no constructor exists on type <paramref name = "classToProxy" /> with parameters matching <paramref
		///    name = "constructorArguments" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxy(Type classToProxy, object[] constructorArguments, params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> type.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no parameterless constructor exists on type <paramref
		///    name = "classToProxy" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxy(Type classToProxy, params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> type.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "options" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no default constructor exists on type <paramref name = "classToProxy" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxy(Type classToProxy, ProxyGenerationOptions options, params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> and <paramref name = "additionalInterfacesToProxy" /> types.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "options" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no default constructor exists on type <paramref name = "classToProxy" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when default constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options,
		                        params IInterceptor[] interceptors);

		/// <summary>
		///   Creates proxy object intercepting calls to virtual members of type <paramref name = "classToProxy" /> on newly created instance of that type with given <paramref
		///    name = "interceptors" />.
		/// </summary>
		/// <param name = "classToProxy">Type of class which will be proxied.</param>
		/// <param name = "additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name = "options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name = "constructorArguments">Arguments of constructor of type <paramref name = "classToProxy" /> which should be used to create a new instance of that type.</param>
		/// <param name = "interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		///   New object of type <paramref name = "classToProxy" /> proxying calls to virtual members of <paramref
		///    name = "classToProxy" /> and <paramref name = "additionalInterfacesToProxy" /> types.
		/// </returns>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentNullException">Thrown when given <paramref name = "options" /> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> or any of <paramref
		///    name = "additionalInterfacesToProxy" /> is a generic type definition.</exception>
		/// <exception cref = "ArgumentException">Thrown when given <paramref name = "classToProxy" /> is not a class type.</exception>
		/// <exception cref = "ArgumentException">Thrown when no constructor exists on type <paramref name = "classToProxy" /> with parameters matching <paramref
		///    name = "constructorArguments" />.</exception>
		/// <exception cref = "TargetInvocationException">Thrown when constructor of type <paramref name = "classToProxy" /> throws an exception.</exception>
		/// <remarks>
		///   This method uses <see cref = "IProxyBuilder" /> implementation to generate a proxy type.
		///   As such caller should expect any type of exception that given <see cref = "IProxyBuilder" /> implementation may throw.
		/// </remarks>
		object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy,
		                        ProxyGenerationOptions options,
		                        object[] constructorArguments, params IInterceptor[] interceptors);
	}
}