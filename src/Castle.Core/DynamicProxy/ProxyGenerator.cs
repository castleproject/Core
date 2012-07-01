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
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
#if !SILVERLIGHT
	using System.Runtime.InteropServices;
	using System.Runtime.Remoting;
	using System.Security;
	using System.Security.Permissions;
	using Castle.Core.Internal;
#endif
	using System.Text;

	using Castle.Core.Logging;

	/// <summary>
	///   Provides proxy objects for classes and interfaces.
	/// </summary>
	[CLSCompliant(true)]
	public class ProxyGenerator
	{
		private ILogger logger = NullLogger.Instance;
		private readonly IProxyBuilder proxyBuilder;

		/// <summary>
		///   Initializes a new instance of the <see cref = "ProxyGenerator" /> class.
		/// </summary>
		/// <param name = "builder">Proxy types builder.</param>
		public ProxyGenerator(IProxyBuilder builder)
		{
			proxyBuilder = builder;

#if !SILVERLIGHT
			if (HasSecurityPermission())
			{
				Logger = new TraceLogger("Castle.DynamicProxy", LoggerLevel.Warn);
			}
#endif
		}

#if !SILVERLIGHT
		private bool HasSecurityPermission()
		{
			const SecurityPermissionFlag flag = SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy;
			return new SecurityPermission(flag).IsGranted();
		}
#endif

		/// <summary>
		///   Initializes a new instance of the <see cref = "ProxyGenerator" /> class.
		/// </summary>
		public ProxyGenerator() : this(new DefaultProxyBuilder())
		{
		}

		/// <summary>
		///   Initializes a new instance of the <see cref = "ProxyGenerator" /> class.
		/// </summary>
		/// <param name="disableSignedModule">If <c>true</c> forces all types to be generated into an unsigned module.</param>
		public ProxyGenerator(bool disableSignedModule) : this(new DefaultProxyBuilder(new ModuleScope(false, disableSignedModule)))
		{
		}

		/// <summary>
		///   Gets or sets the <see cref = "ILogger" /> that this <see cref = "ProxyGenerator" /> log to.
		/// </summary>
		public ILogger Logger
		{
			get { return logger; }
			set
			{
				logger = value;
				proxyBuilder.Logger = value;
			}
		}

		/// <summary>
		///   Gets the proxy builder instance used to generate proxy types.
		/// </summary>
		/// <value>The proxy builder.</value>
		public IProxyBuilder ProxyBuilder
		{
			get { return proxyBuilder; }
		}

#if MONO
#pragma warning disable 1584, 1580, 1574 // Mono chokes on cref with generic arguments
#endif

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
		public TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, params IInterceptor[] interceptors)
			where TInterface : class
		{
			// NOTE: we don't need to document exception case where interface type is null, since it can never be for a generic method.
			// If we leave target as being of type TInterface we also have covered exception where target does not implement TInterface.

			// NOTE: Can any other Activator.CreateInstance exception be thrown in this context?

			return
				(TInterface)
				CreateInterfaceProxyWithTarget(typeof(TInterface), target, ProxyGenerationOptions.Default, interceptors);
		}

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
		public TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, ProxyGenerationOptions options,
		                                                             params IInterceptor[] interceptors)
			where TInterface : class
		{
			return (TInterface)CreateInterfaceProxyWithTarget(typeof(TInterface), target, options, interceptors);
		}

#if MONO
#pragma warning restore 1584, 1580, 1574
#endif

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
		public object CreateInterfaceProxyWithTarget(Type interfaceToProxy, object target, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(interfaceToProxy, target, ProxyGenerationOptions.Default, interceptors);
		}

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
		public object CreateInterfaceProxyWithTarget(Type interfaceToProxy, object target, ProxyGenerationOptions options,
		                                             params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(interfaceToProxy, null, target, options, interceptors);
		}

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
		public object CreateInterfaceProxyWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy, object target,
		                                             params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(interfaceToProxy, additionalInterfacesToProxy, target,
			                                      ProxyGenerationOptions.Default, interceptors);
		}

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
		public virtual object CreateInterfaceProxyWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                                     object target,
		                                                     ProxyGenerationOptions options,
		                                                     params IInterceptor[] interceptors)
		{
			if (interfaceToProxy == null)
			{
				throw new ArgumentNullException("interfaceToProxy");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (interceptors == null)
			{
				throw new ArgumentNullException("interceptors");
			}

			if (!interfaceToProxy.IsInterface)
			{
				throw new ArgumentException("Specified type is not an interface", "interfaceToProxy");
			}

			var targetType = target.GetType();
			if (!interfaceToProxy.IsAssignableFrom(targetType))
			{
				throw new ArgumentException("Target does not implement interface " + interfaceToProxy.FullName, "target");
			}

			CheckNotGenericTypeDefinition(interfaceToProxy, "interfaceToProxy");
			CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, "additionalInterfacesToProxy");

			var generatedType = CreateInterfaceProxyTypeWithTarget(interfaceToProxy, additionalInterfacesToProxy, targetType,
			                                                       options);

			var arguments = GetConstructorArguments(target, interceptors, options);
			return Activator.CreateInstance(generatedType, arguments.ToArray());
		}

		protected List<object> GetConstructorArguments(object target, IInterceptor[] interceptors,
		                                               ProxyGenerationOptions options)
		{
			// create constructor arguments (initialized with mixin implementations, interceptors and target type constructor arguments)
			var arguments = new List<object>(options.MixinData.Mixins) { interceptors, target };
			if (options.Selector != null)
			{
				arguments.Add(options.Selector);
			}
			return arguments;
		}

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
		public object CreateInterfaceProxyWithTargetInterface(Type interfaceToProxy, object target,
		                                                      params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTargetInterface(interfaceToProxy, target, ProxyGenerationOptions.Default, interceptors);
		}

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
		public TInterface CreateInterfaceProxyWithTargetInterface<TInterface>(TInterface target,
		                                                                      params IInterceptor[] interceptors)
			where TInterface : class
		{
			return (TInterface)CreateInterfaceProxyWithTargetInterface(typeof(TInterface),
			                                                           target,
			                                                           ProxyGenerationOptions.Default,
			                                                           interceptors);
		}

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
		public TInterface CreateInterfaceProxyWithTargetInterface<TInterface>(TInterface target,
		                                                                      ProxyGenerationOptions options,
		                                                                      params IInterceptor[] interceptors)
			where TInterface : class
		{
			return (TInterface)CreateInterfaceProxyWithTargetInterface(typeof(TInterface),
			                                                           target,
			                                                           options,
			                                                           interceptors);
		}

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
		public object CreateInterfaceProxyWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                                      object target, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTargetInterface(interfaceToProxy, additionalInterfacesToProxy, target,
			                                               ProxyGenerationOptions.Default, interceptors);
		}

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
		public object CreateInterfaceProxyWithTargetInterface(Type interfaceToProxy, object target,
		                                                      ProxyGenerationOptions options,
		                                                      params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTargetInterface(interfaceToProxy, null, target, options, interceptors);
		}

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
#if DOTNET40
		[SecuritySafeCritical]
#endif
		public virtual object CreateInterfaceProxyWithTargetInterface(Type interfaceToProxy,
		                                                              Type[] additionalInterfacesToProxy,
		                                                              object target, ProxyGenerationOptions options,
		                                                              params IInterceptor[] interceptors)
		{
			//TODO: add <example> to xml comments to show how to use IChangeProxyTarget

			if (target != null && interfaceToProxy.IsInstanceOfType(target) == false)
			{
				throw new ArgumentException("targetType");
			}
			if (interfaceToProxy == null)
			{
				throw new ArgumentNullException("interfaceToProxy");
			}
			if (interceptors == null)
			{
				throw new ArgumentNullException("interceptors");
			}

			if (!interfaceToProxy.IsInterface)
			{
				throw new ArgumentException("Specified type is not an interface", "interfaceToProxy");
			}

			var isRemotingProxy = false;
			if (target != null && interfaceToProxy.IsInstanceOfType(target) == false)
			{
#if !SILVERLIGHT
				//check if we have remoting proxy at hand...
				if (RemotingServices.IsTransparentProxy(target))
				{
					var info = (RemotingServices.GetRealProxy(target) as IRemotingTypeInfo);
					if (info != null)
					{
						if (!info.CanCastTo(interfaceToProxy, target))
						{
							throw new ArgumentException("Target does not implement interface " + interfaceToProxy.FullName, "target");
						}
						isRemotingProxy = true;
					}
				}
				else if (Marshal.IsComObject(target))
				{
					var interfaceId = interfaceToProxy.GUID;
					if (interfaceId != Guid.Empty)
					{
						var iUnknown = Marshal.GetIUnknownForObject(target);
						var interfacePointer = IntPtr.Zero;
						var result = Marshal.QueryInterface(iUnknown, ref interfaceId, out interfacePointer);
						if (result == 0 && interfacePointer == IntPtr.Zero)
						{
							throw new ArgumentException("Target COM object does not implement interface " + interfaceToProxy.FullName,
							                            "target");
						}
					}
				}
				else
				{
#endif
					throw new ArgumentException("Target does not implement interface " + interfaceToProxy.FullName, "target");

#if !SILVERLIGHT
				}
#endif
			}

			CheckNotGenericTypeDefinition(interfaceToProxy, "interfaceToProxy");
			CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, "additionalInterfacesToProxy");

			var generatedType = CreateInterfaceProxyTypeWithTargetInterface(interfaceToProxy, additionalInterfacesToProxy,
			                                                                options);
			var arguments = GetConstructorArguments(target, interceptors, options);
			if (isRemotingProxy)
			{
				var constructors = generatedType.GetConstructors();

				// one .ctor to rule them all
				Debug.Assert(constructors.Length == 1, "constructors.Length == 1");
				return constructors[0].Invoke(arguments.ToArray());
			}
			return Activator.CreateInstance(generatedType, arguments.ToArray());
		}

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
		public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(IInterceptor interceptor) where TInterface : class
		{
			return (TInterface)CreateInterfaceProxyWithoutTarget(typeof(TInterface), interceptor);
		}

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
		public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(params IInterceptor[] interceptors)
			where TInterface : class
		{
			return (TInterface)CreateInterfaceProxyWithoutTarget(typeof(TInterface), interceptors);
		}

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
		public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(ProxyGenerationOptions options,
		                                                                params IInterceptor[] interceptors)
			where TInterface : class
		{
			return (TInterface)CreateInterfaceProxyWithoutTarget(typeof(TInterface), Type.EmptyTypes, options, interceptors);
		}

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
		public object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, IInterceptor interceptor)
		{
			return CreateInterfaceProxyWithoutTarget(interfaceToProxy, Type.EmptyTypes, ProxyGenerationOptions.Default,
			                                         interceptor);
		}

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
		public object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithoutTarget(interfaceToProxy, Type.EmptyTypes, ProxyGenerationOptions.Default,
			                                         interceptors);
		}

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
		public object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                                params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithoutTarget(interfaceToProxy, additionalInterfacesToProxy,
			                                         ProxyGenerationOptions.Default, interceptors);
		}

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
		public object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, ProxyGenerationOptions options,
		                                                params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithoutTarget(interfaceToProxy, Type.EmptyTypes, options, interceptors);
		}

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
		public virtual object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                                        ProxyGenerationOptions options,
		                                                        params IInterceptor[] interceptors)
		{
			if (interfaceToProxy == null)
			{
				throw new ArgumentNullException("interfaceToProxy");
			}
			if (interceptors == null)
			{
				throw new ArgumentNullException("interceptors");
			}

			if (!interfaceToProxy.IsInterface)
			{
				throw new ArgumentException("Specified type is not an interface", "interfaceToProxy");
			}

			CheckNotGenericTypeDefinition(interfaceToProxy, "interfaceToProxy");
			CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, "additionalInterfacesToProxy");

			var generatedType = CreateInterfaceProxyTypeWithoutTarget(interfaceToProxy, additionalInterfacesToProxy, options);
			var arguments = GetConstructorArguments(null, interceptors, options);
			return Activator.CreateInstance(generatedType, arguments.ToArray());
		}

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
		public TClass CreateClassProxyWithTarget<TClass>(TClass target, params IInterceptor[] interceptors)
			where TClass : class
		{
			return (TClass)CreateClassProxyWithTarget(typeof(TClass),
			                                          Type.EmptyTypes,
			                                          target,
			                                          ProxyGenerationOptions.Default,
			                                          new object[0],
			                                          interceptors);
		}

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
		public TClass CreateClassProxyWithTarget<TClass>(TClass target, ProxyGenerationOptions options,
		                                                 params IInterceptor[] interceptors) where TClass : class
		{
			return (TClass)CreateClassProxyWithTarget(typeof(TClass),
			                                          Type.EmptyTypes,
			                                          target,
			                                          options,
			                                          new object[0],
			                                          interceptors);
		}

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
		public object CreateClassProxyWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy, object target,
		                                         params IInterceptor[] interceptors)
		{
			return CreateClassProxyWithTarget(classToProxy,
			                                  additionalInterfacesToProxy,
			                                  target,
			                                  ProxyGenerationOptions.Default,
			                                  new object[0],
			                                  interceptors);
		}

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
		public object CreateClassProxyWithTarget(Type classToProxy, object target, ProxyGenerationOptions options,
		                                         object[] constructorArguments, params IInterceptor[] interceptors)
		{
			return CreateClassProxyWithTarget(classToProxy,
			                                  Type.EmptyTypes,
			                                  target,
			                                  options,
			                                  constructorArguments,
			                                  interceptors);
		}

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
		public object CreateClassProxyWithTarget(Type classToProxy, object target, object[] constructorArguments,
		                                         params IInterceptor[] interceptors)
		{
			return CreateClassProxyWithTarget(classToProxy,
			                                  Type.EmptyTypes,
			                                  target,
			                                  ProxyGenerationOptions.Default,
			                                  constructorArguments,
			                                  interceptors);
		}

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
		public object CreateClassProxyWithTarget(Type classToProxy, object target, params IInterceptor[] interceptors)
		{
			return CreateClassProxyWithTarget(classToProxy,
			                                  Type.EmptyTypes,
			                                  target,
			                                  ProxyGenerationOptions.Default,
			                                  new object[0],
			                                  interceptors);
		}

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
		public object CreateClassProxyWithTarget(Type classToProxy, object target, ProxyGenerationOptions options,
		                                         params IInterceptor[] interceptors)
		{
			return CreateClassProxyWithTarget(classToProxy,
			                                  Type.EmptyTypes,
			                                  target,
			                                  options,
			                                  new object[0],
			                                  interceptors);
		}

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
		public object CreateClassProxyWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy, object target,
		                                         ProxyGenerationOptions options, params IInterceptor[] interceptors)
		{
			return CreateClassProxyWithTarget(classToProxy,
			                                  additionalInterfacesToProxy,
			                                  target,
			                                  options,
			                                  new object[0],
			                                  interceptors);
		}

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
		public virtual object CreateClassProxyWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy, object target,
		                                                 ProxyGenerationOptions options, object[] constructorArguments,
		                                                 params IInterceptor[] interceptors)
		{
			if (classToProxy == null)
			{
				throw new ArgumentNullException("classToProxy");
			}
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			if (!classToProxy.IsClass)
			{
				throw new ArgumentException("'classToProxy' must be a class", "classToProxy");
			}

			CheckNotGenericTypeDefinition(classToProxy, "classToProxy");
			CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, "additionalInterfacesToProxy");

			var proxyType = CreateClassProxyTypeWithTarget(classToProxy, additionalInterfacesToProxy, options);

			// create constructor arguments (initialized with mixin implementations, interceptors and target type constructor arguments)
			var arguments = BuildArgumentListForClassProxyWithTarget(target, options, interceptors);
			if (constructorArguments != null && constructorArguments.Length != 0)
			{
				arguments.AddRange(constructorArguments);
			}
			return CreateClassProxyInstance(proxyType, arguments, classToProxy, constructorArguments);
		}

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
		public TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class
		{
			return (TClass)CreateClassProxy(typeof(TClass), ProxyGenerationOptions.Default, interceptors);
		}

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
		public TClass CreateClassProxy<TClass>(ProxyGenerationOptions options, params IInterceptor[] interceptors)
			where TClass : class
		{
			return (TClass)CreateClassProxy(typeof(TClass), options, interceptors);
		}

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
		public object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy,
		                               params IInterceptor[] interceptors)
		{
			return CreateClassProxy(classToProxy, additionalInterfacesToProxy, ProxyGenerationOptions.Default, interceptors);
		}

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
		public object CreateClassProxy(Type classToProxy, ProxyGenerationOptions options, object[] constructorArguments,
		                               params IInterceptor[] interceptors)
		{
			return CreateClassProxy(classToProxy, null, options, constructorArguments, interceptors);
		}

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
		public object CreateClassProxy(Type classToProxy, object[] constructorArguments, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(classToProxy, null, ProxyGenerationOptions.Default, constructorArguments, interceptors);
		}

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
		public object CreateClassProxy(Type classToProxy, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(classToProxy, null, ProxyGenerationOptions.Default,
			                        null, interceptors);
		}

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
		public object CreateClassProxy(Type classToProxy, ProxyGenerationOptions options, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(classToProxy, null, options, interceptors);
		}

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
		public object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options,
		                               params IInterceptor[] interceptors)
		{
			return CreateClassProxy(classToProxy, additionalInterfacesToProxy, options, null, interceptors);
		}

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
		public virtual object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy,
		                                       ProxyGenerationOptions options,
		                                       object[] constructorArguments, params IInterceptor[] interceptors)
		{
			if (classToProxy == null)
			{
				throw new ArgumentNullException("classToProxy");
			}
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			if (!classToProxy.IsClass)
			{
				throw new ArgumentException("'classToProxy' must be a class", "classToProxy");
			}

			CheckNotGenericTypeDefinition(classToProxy, "classToProxy");
			CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, "additionalInterfacesToProxy");

			var proxyType = CreateClassProxyType(classToProxy, additionalInterfacesToProxy, options);

			// create constructor arguments (initialized with mixin implementations, interceptors and target type constructor arguments)
			var arguments = BuildArgumentListForClassProxy(options, interceptors);
			if (constructorArguments != null && constructorArguments.Length != 0)
			{
				arguments.AddRange(constructorArguments);
			}
			return CreateClassProxyInstance(proxyType, arguments, classToProxy, constructorArguments);
		}

		protected object CreateClassProxyInstance(Type proxyType, List<object> proxyArguments, Type classToProxy,
		                                          object[] constructorArguments)
		{
			try
			{
				return Activator.CreateInstance(proxyType, proxyArguments.ToArray());
			}
			catch (MissingMethodException)
			{
				var message = new StringBuilder();
				message.AppendFormat("Can not instantiate proxy of class: {0}.", classToProxy.FullName);
				message.AppendLine();
				if (constructorArguments == null || constructorArguments.Length == 0)
				{
					message.Append("Could not find a parameterless constructor.");
				}
				else
				{
					message.AppendLine("Could not find a constructor that would match given arguments:");
					foreach (var argument in constructorArguments)
					{
						var argumentText = argument == null ? "<null>" : argument.GetType().ToString();
						message.AppendLine(argumentText);
					}
				}

				throw new InvalidProxyConstructorArgumentsException(message.ToString(),proxyType,classToProxy);
			}
		}

		protected void CheckNotGenericTypeDefinition(Type type, string argumentName)
		{
			if (type != null && type.IsGenericTypeDefinition)
			{
				throw new ArgumentException("You can't specify a generic type definition.", argumentName);
			}
		}

		protected void CheckNotGenericTypeDefinitions(IEnumerable<Type> types, string argumentName)
		{
			if (types == null)
			{
				return;
			}
			foreach (var t in types)
			{
				CheckNotGenericTypeDefinition(t, argumentName);
			}
		}

		protected List<object> BuildArgumentListForClassProxyWithTarget(object target, ProxyGenerationOptions options,
		                                                                IInterceptor[] interceptors)
		{
			var arguments = new List<object>();
			arguments.Add(target);
			arguments.AddRange(options.MixinData.Mixins);
			arguments.Add(interceptors);
			if (options.Selector != null)
			{
				arguments.Add(options.Selector);
			}
			return arguments;
		}

		protected List<object> BuildArgumentListForClassProxy(ProxyGenerationOptions options, IInterceptor[] interceptors)
		{
			var arguments = new List<object>(options.MixinData.Mixins) { interceptors };
			if (options.Selector != null)
			{
				arguments.Add(options.Selector);
			}
			return arguments;
		}

		/// <summary>
		///   Creates the proxy type for class proxy with given <paramref name = "classToProxy" /> class, implementing given <paramref
		///    name = "additionalInterfacesToProxy" /> and using provided <paramref name = "options" />.
		/// </summary>
		/// <param name = "classToProxy">The base class for proxy type.</param>
		/// <param name = "additionalInterfacesToProxy">The interfaces that proxy type should implement.</param>
		/// <param name = "options">The options for proxy generation process.</param>
		/// <returns><see cref = "Type" /> of proxy.</returns>
		protected Type CreateClassProxyType(Type classToProxy, Type[] additionalInterfacesToProxy,
		                                    ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateClassProxyType(classToProxy, additionalInterfacesToProxy, options);
		}

		/// <summary>
		///   Creates the proxy type for interface proxy with target for given <paramref name = "interfaceToProxy" /> interface, implementing given <paramref
		///    name = "additionalInterfacesToProxy" /> on given <paramref name = "targetType" /> and using provided <paramref
		///    name = "options" />.
		/// </summary>
		/// <param name = "interfaceToProxy">The interface proxy type should implement.</param>
		/// <param name = "additionalInterfacesToProxy">The additional interfaces proxy type should implement.</param>
		/// <param name = "targetType">Actual type that the proxy type will encompass.</param>
		/// <param name = "options">The options for proxy generation process.</param>
		/// <returns><see cref = "Type" /> of proxy.</returns>
		protected Type CreateInterfaceProxyTypeWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                                  Type targetType,
		                                                  ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateInterfaceProxyTypeWithTarget(interfaceToProxy, additionalInterfacesToProxy, targetType,
			                                                       options);
		}

		/// <summary>
		///   Creates the proxy type for interface proxy with target interface for given <paramref name = "interfaceToProxy" /> interface, implementing given <paramref
		///    name = "additionalInterfacesToProxy" /> on given <paramref name = "interfaceToProxy" /> and using provided <paramref
		///    name = "options" />.
		/// </summary>
		/// <param name = "interfaceToProxy">The interface proxy type should implement.</param>
		/// <param name = "additionalInterfacesToProxy">The additional interfaces proxy type should implement.</param>
		/// <param name = "options">The options for proxy generation process.</param>
		/// <returns><see cref = "Type" /> of proxy.</returns>
		protected Type CreateInterfaceProxyTypeWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                                           ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(interfaceToProxy, additionalInterfacesToProxy,
			                                                                options);
		}

		/// <summary>
		///   Creates the proxy type for interface proxy without target for given <paramref name = "interfaceToProxy" /> interface, implementing given <paramref
		///    name = "additionalInterfacesToProxy" /> and using provided <paramref name = "options" />.
		/// </summary>
		/// <param name = "interfaceToProxy">The interface proxy type should implement.</param>
		/// <param name = "additionalInterfacesToProxy">The additional interfaces proxy type should implement.</param>
		/// <param name = "options">The options for proxy generation process.</param>
		/// <returns><see cref = "Type" /> of proxy.</returns>
		protected Type CreateInterfaceProxyTypeWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                                     ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateInterfaceProxyTypeWithoutTarget(interfaceToProxy, additionalInterfacesToProxy, options);
		}

		protected Type CreateClassProxyTypeWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy,
		                                              ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateClassProxyTypeWithTarget(classToProxy, additionalInterfacesToProxy, options);
		}
	}
}