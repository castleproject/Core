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
	using System.Collections;
	using System.Reflection;
	using Castle.Core.Interceptor;
	using System.Collections.Generic;

	/// <summary>
	/// Provides proxy objects for classes and interfaces.
	/// </summary>
	[CLSCompliant(true)]
	public class ProxyGenerator
	{
		private readonly IProxyBuilder proxyBuilder;

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyGenerator"/> class.
		/// </summary>
		/// <param name="builder">Proxy types builder.</param>
		public ProxyGenerator(IProxyBuilder builder)
		{
			proxyBuilder = builder;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyGenerator"/> class.
		/// </summary>
		public ProxyGenerator() : this(new DefaultProxyBuilder())
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the proxy builder instance used to generate proxy types.
		/// </summary>
		/// <value>The proxy builder.</value>
		public IProxyBuilder ProxyBuilder
		{
			get { return proxyBuilder; }
		}

		#endregion

		private void CheckNotGenericTypeDefinition(Type type, string argumentName)
		{
			if (type != null && type.IsGenericTypeDefinition)
			{
				throw new ArgumentException("You can't specify a generic type definition.", argumentName);
			}
		}

		private void CheckNotGenericTypeDefinitions(IEnumerable types, string argumentName)
		{
			if (types != null)
			{
				foreach (Type t in types)
				{
					CheckNotGenericTypeDefinition(t, argumentName);
				}
			}
		}

		#region CreateInterfaceProxyWithTarget

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <typeparamref name="TInterface"/> on <paramref name="target"/> object with given <paramref name="interceptors"/>.
		/// </summary>
		/// <typeparam name="TInterface">Type of the interface implemented by <paramref name="target"/> which will be proxied.</typeparam>
		/// <param name="target">The target object, calls to which will be intercepted.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>Object proxying calls to members of <typeparamref name="TInterface"/> on <paramref name="target"/> object.</returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="target"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <typeparamref name="TInterface"/>is not an interface type.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on actual type of <paramref name="target"/> object.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of actual type of <paramref name="target"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, params IInterceptor[] interceptors) where TInterface : class
		{
			// NOTE: we don't need to document exception case where interface type is null, since it can never be for a generic method.
			// If we leave target as being of type TInterface we also have covered exception where target does not implement TInterface.

			// NOTE: Can any other Activator.CreateInstance exception be thrown in this context?

			return (TInterface)CreateInterfaceProxyWithTarget(typeof(TInterface), target, ProxyGenerationOptions.Default, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <typeparamref name="TInterface"/> on <paramref name="target"/> object with given <paramref name="interceptors"/>.
		/// </summary>
		/// <typeparam name="TInterface">Type of the interface implemented by <paramref name="target"/> which will be proxied.</typeparam>
		/// <param name="target">The target object, calls to which will be intercepted.</param>
		/// <param name="options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <typeparamref name="TInterface"/> on <paramref name="target"/> object.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="target"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <typeparamref name="TInterface"/>is not an interface type.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on actual type of <paramref name="target"/> object.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of actual type of <paramref name="target"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public TInterface CreateInterfaceProxyWithTarget<TInterface>(object target, ProxyGenerationOptions options,
												   params IInterceptor[] interceptors)
		{
			return (TInterface) CreateInterfaceProxyWithTarget(typeof (TInterface), target, options, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <paramref name="theInterface"/> on <paramref name="target"/> object with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="theInterface">Type of the interface implemented by <paramref name="target"/> which will be proxied.</param>
		/// <param name="target">The target object, calls to which will be intercepted.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <paramref name="theInterface"/> type on <paramref name="target"/> object.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="theInterface"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="target"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is not an interface type.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="target"/> does not implement <paramref name="theInterface"/> interface.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on actual type of <paramref name="target"/> object.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of actual type of <paramref name="target"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateInterfaceProxyWithTarget(Type theInterface, object target, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(theInterface, target, ProxyGenerationOptions.Default, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <paramref name="theInterface"/> on <paramref name="target"/> object with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="theInterface">Type of the interface implemented by <paramref name="target"/> which will be proxied.</param>
		/// <param name="target">The target object, calls to which will be intercepted.</param>
		/// <param name="options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <paramref name="theInterface"/> type on <paramref name="target"/> object.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="theInterface"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="target"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is not an interface type.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="target"/> does not implement <paramref name="theInterface"/> interface.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on actual type of <paramref name="target"/> object.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of actual type of <paramref name="target"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateInterfaceProxyWithTarget(Type theInterface, object target, ProxyGenerationOptions options,
													 params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(theInterface, null, target, options, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <paramref name="theInterface"/> on <paramref name="target"/> object with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="theInterface">Type of the interface implemented by <paramref name="target"/> which will be proxied.</param>
		/// <param name="target">The target object, calls to which will be intercepted.</param>
		/// <param name="interfaces">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <paramref name="theInterface"/> and <paramref name="interfaces"/> types  on <paramref name="target"/> object.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="theInterface"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="target"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> or any of <paramref name="interfaces"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is not an interface type.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="target"/> does not implement <paramref name="theInterface"/> interface.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on actual type of <paramref name="target"/> object.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of actual type of <paramref name="target"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateInterfaceProxyWithTarget(Type theInterface, Type[] interfaces,
													 object target, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(theInterface, interfaces, target, ProxyGenerationOptions.Default, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <paramref name="theInterface"/> on <paramref name="target"/> object with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="theInterface">Type of the interface implemented by <paramref name="target"/> which will be proxied.</param>
		/// <param name="target">The target object, calls to which will be intercepted.</param>
		/// <param name="options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name="interfaces">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <paramref name="theInterface"/> and <paramref name="interfaces"/> types on <paramref name="target"/> object.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="theInterface"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="target"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> or any of <paramref name="interfaces"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is not an interface type.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="target"/> does not implement <paramref name="theInterface"/> interface.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on actual type of <paramref name="target"/> object.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of actual type of <paramref name="target"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateInterfaceProxyWithTarget(Type theInterface, Type[] interfaces, object target,
													 ProxyGenerationOptions options, params IInterceptor[] interceptors)
		{
			if (theInterface == null)
			{
				throw new ArgumentNullException("theInterface");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (interceptors == null)
			{
				throw new ArgumentNullException("interceptors");
			}

			if (!theInterface.IsInterface)
			{
				throw new ArgumentException("Specified type is not an interface", "theInterface");
			}

			if (!theInterface.IsAssignableFrom(target.GetType()))
			{
				throw new ArgumentException("Target does not implement interface " + theInterface.FullName, "target");
			}

			CheckNotGenericTypeDefinition(theInterface, "theInterface");
			CheckNotGenericTypeDefinitions(interfaces, "interfaces");

			Type targetType = target.GetType();
			Type generatedType = CreateInterfaceProxyTypeWithTarget(theInterface, interfaces, targetType, options);

			List<object> arguments = GetConstructorArguments(target, interceptors, options);
			return Activator.CreateInstance(generatedType, arguments.ToArray());
		}

		private List<object> GetConstructorArguments(object target, IInterceptor[] interceptors, ProxyGenerationOptions options)
		{
			// create constructor arguments (initialized with mixin implementations, interceptors and target type constructor arguments)
			List<object> arguments = new List<object>(options.MixinData.GetMixinInterfaceImplementationsAsArray());
			arguments.Add(interceptors);
			arguments.Add(target);
			return arguments;
		}

		#endregion

		#region CreateInterfaceProxyWithTargetInterface

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <paramref name="theInterface"/> on <paramref name="target"/> object with given <paramref name="interceptors"/>.
		/// Interceptors can use <see cref="IChangeProxyTarget"/> interface to provide other target for method invocation than default <paramref name="target"/>.
		/// </summary>
        /// <param name="theInterface">Type of the interface implemented by <paramref name="target"/> which will be proxied.</param>
		/// <param name="target">The target object, calls to which will be intercepted.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <paramref name="theInterface"/> type on <paramref name="target"/> object or alternative implementation swapped at runtime by an interceptor.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="theInterface"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="target"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is not an interface type.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="target"/> does not implement <paramref name="theInterface"/> interface.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on actual type of <paramref name="target"/> object.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of actual type of <paramref name="target"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateInterfaceProxyWithTargetInterface(Type theInterface, object target,
															  params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTargetInterface(theInterface, target, ProxyGenerationOptions.Default, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <paramref name="theInterface"/> on <paramref name="target"/> object with given <paramref name="interceptors"/>.
		/// Interceptors can use <see cref="IChangeProxyTarget"/> interface to provide other target for method invocation than default <paramref name="target"/>.
		/// </summary>
        /// <param name="theInterface">Type of the interface implemented by <paramref name="target"/> which will be proxied.</param>
		/// <param name="target">The target object, calls to which will be intercepted.</param>
		/// <param name="options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <paramref name="theInterface"/> type on <paramref name="target"/> object or alternative implementation swapped at runtime by an interceptor.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="theInterface"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="target"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is not an interface type.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="target"/> does not implement <paramref name="theInterface"/> interface.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on actual type of <paramref name="target"/> object.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of actual type of <paramref name="target"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateInterfaceProxyWithTargetInterface(Type theInterface, object target,
															  ProxyGenerationOptions options,
															  params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTargetInterface(theInterface, null, target, options, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <paramref name="theInterface"/> on <paramref name="target"/> object with given <paramref name="interceptors"/>.
		/// Interceptors can use <see cref="IChangeProxyTarget"/> interface to provide other target for method invocation than default <paramref name="target"/>.
		/// </summary>
        /// <param name="theInterface">Type of the interface implemented by <paramref name="target"/> which will be proxied.</param>
		/// <param name="target">The target object, calls to which will be intercepted.</param>
		/// <param name="options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name="interfaces">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <paramref name="theInterface"/> and <paramref name="interfaces"/> types on <paramref name="target"/> object or alternative implementation swapped at runtime by an interceptor.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="theInterface"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="target"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> or any of <paramref name="interfaces"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is not an interface type.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="target"/> does not implement <paramref name="theInterface"/> interface.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on actual type of <paramref name="target"/> object.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of actual type of <paramref name="target"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateInterfaceProxyWithTargetInterface(Type theInterface, Type[] interfaces, object target,
															  ProxyGenerationOptions options,
															  params IInterceptor[] interceptors)
		{
			//TODO: add <example> to xml comments to show how to use IChangeProxyTarget

			if (!theInterface.IsInstanceOfType(target))
			{
				throw new ArgumentException("targetType");
			}
			if (theInterface == null)
			{
				throw new ArgumentNullException("theInterface");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (interceptors == null)
			{
				throw new ArgumentNullException("interceptors");
			}

			if (!theInterface.IsInterface)
			{
				throw new ArgumentException("Specified type is not an interface", "theInterface");
			}

			if (!theInterface.IsAssignableFrom(target.GetType()))
			{
				throw new ArgumentException("Target does not implement interface " + theInterface.FullName, "target");
			}

			CheckNotGenericTypeDefinition(theInterface, "theInterface");
			CheckNotGenericTypeDefinitions(interfaces, "interfaces");

			Type generatedType = CreateInterfaceProxyTypeWithTargetInterface(theInterface, interfaces, theInterface, options);
			List<object> arguments = GetConstructorArguments(target, interceptors, options);
			return Activator.CreateInstance(generatedType, arguments.ToArray());
		}

		#endregion

		#region CreateInterfaceProxyWithoutTarget
		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <typeparamref name="TInterface"/> on target object generated at runtime with given <paramref name="interceptor"/>.
		/// </summary>
        /// <typeparam name="TInterface">Type of the interface which will be proxied.</typeparam>
		/// <param name="interceptor">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <typeparamref name="TInterface"/> types on generated target object.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptor"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <typeparamref name="TInterface"/> is not an interface type.</exception>
		/// <remarks>
		/// Since this method uses an empty-shell implementation of interfaces to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see cref="IInterceptor"/> implementations.
		/// They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see cref="IInvocation.Proceed"/>, since there's no actual implementation to proceed with.
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(IInterceptor interceptor)
		{
			return (TInterface) CreateInterfaceProxyWithoutTarget(typeof (TInterface), interceptor);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <typeparamref name="TInterface"/> on target object generated at runtime with given <paramref name="interceptors"/>.
		/// </summary>
		/// <typeparam name="TInterface">Type of the interface which will be proxied.</typeparam>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <typeparamref name="TInterface"/> types on generated target object.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <typeparamref name="TInterface"/> is not an interface type.</exception>
		/// <remarks>
		/// Since this method uses an empty-shell implementation of interfaces to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see cref="IInterceptor"/> implementations.
		/// They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see cref="IInvocation.Proceed"/>, since there's no actual implementation to proceed with.
		/// As a result of that also at least one <see cref="IInterceptor"/> implementation must be provided.
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(params IInterceptor[] interceptors)
		{
			return (TInterface) CreateInterfaceProxyWithoutTarget(typeof (TInterface), interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <paramref name="theInterface"/> on target object generated at runtime with given <paramref name="interceptor"/>.
		/// </summary>
		/// <param name="theInterface">Type of the interface which will be proxied.</param>
		/// <param name="interceptor">The interceptor called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <paramref name="theInterface"/> type on generated target object.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="theInterface"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptor"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is not an interface type.</exception>
		/// <remarks>
		/// Since this method uses an empty-shell implementation of interfaces to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see cref="IInterceptor"/> implementations.
		/// They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see cref="IInvocation.Proceed"/>, since there's no actual implementation to proceed with.
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateInterfaceProxyWithoutTarget(Type theInterface, IInterceptor interceptor)
		{
			return CreateInterfaceProxyWithoutTarget(theInterface, new Type[0],
													 ProxyGenerationOptions.Default,
													 interceptor);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <paramref name="theInterface"/> on target object generated at runtime with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="theInterface">Type of the interface which will be proxied.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <paramref name="theInterface"/> type on generated target object.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="theInterface"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is not an interface type.</exception>
		/// <remarks>
		/// Since this method uses an empty-shell implementation of interfaces to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see cref="IInterceptor"/> implementations.
		/// They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see cref="IInvocation.Proceed"/>, since there's no actual implementation to proceed with.
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateInterfaceProxyWithoutTarget(Type theInterface, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithoutTarget(theInterface, new Type[0], ProxyGenerationOptions.Default, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <paramref name="theInterface"/> on target object generated at runtime with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="theInterface">Type of the interface which will be proxied.</param>
		/// <param name="interfaces">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <paramref name="theInterface"/> and <paramref name="interfaces"/> types on generated target object.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="theInterface"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> or any of <paramref name="interfaces"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is not an interface type.</exception>
		/// <remarks>
		/// Since this method uses an empty-shell implementation of interfaces to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see cref="IInterceptor"/> implementations.
		/// They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see cref="IInvocation.Proceed"/>, since there's no actual implementation to proceed with.
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateInterfaceProxyWithoutTarget(Type theInterface, Type[] interfaces,
														params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithoutTarget(theInterface, interfaces, ProxyGenerationOptions.Default, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to members of interface <paramref name="theInterface"/> on target object generated at runtime with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="theInterface">Type of the interface which will be proxied.</param>
		/// <param name="options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name="interfaces">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// Object proxying calls to members of <paramref name="theInterface"/> and <paramref name="interfaces"/> types on generated target object.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="theInterface"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="interceptors"/> array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> or any of <paramref name="interfaces"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="theInterface"/> is not an interface type.</exception>
		/// <remarks>
		/// Since this method uses an empty-shell implementation of interfaces to proxy generated at runtime, the actual implementation of proxied methods must be provided by given <see cref="IInterceptor"/> implementations.
		/// They are responsible for setting return value (and out parameters) on proxied methods. It is also illegal for an interceptor to call <see cref="IInvocation.Proceed"/>, since there's no actual implementation to proceed with.
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateInterfaceProxyWithoutTarget(Type theInterface, Type[] interfaces, ProxyGenerationOptions options,
														params IInterceptor[] interceptors)
		{
			if (theInterface == null)
			{
				throw new ArgumentNullException("theInterface");
			}
			if (interceptors == null)
			{
				throw new ArgumentNullException("interceptors");
			}

			if (!theInterface.IsInterface)
			{
				throw new ArgumentException("Specified type is not an interface", "theInterface");
			}

			CheckNotGenericTypeDefinition(theInterface, "theInterface");
			CheckNotGenericTypeDefinitions(interfaces, "interfaces");

			Type generatedType = CreateInterfaceProxyTypeWithoutTarget(theInterface, interfaces, options);
			List<object> arguments = GetConstructorArguments(new object(), interceptors, options);
			return Activator.CreateInstance(generatedType, arguments.ToArray());
		}

		#endregion

		#region CreateClassProxy
		/// <summary>
		/// Creates proxy object intercepting calls to virtual members of type <typeparamref name="TClass"/> on newly created instance of that type with given <paramref name="interceptors"/>.
		/// </summary>
		/// <typeparam name="TClass">Type of class which will be proxied.</typeparam>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
        /// New object of type <typeparamref name="TClass"/> proxying calls to virtual members of <typeparamref name="TClass"/> type.
		/// </returns>
        /// <exception cref="ArgumentException">Thrown when given <typeparamref name="TClass"/> is not a class type.</exception>
        /// <exception cref="MissingMethodException">Thrown when no default constructor exists on type <typeparamref name="TClass"/>.</exception>
        /// <exception cref="TargetInvocationException">Thrown when default constructor of type <typeparamref name="TClass"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class
		{
			return (TClass) CreateClassProxy(typeof(TClass), ProxyGenerationOptions.Default, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to virtual members of type <paramref name="targetType"/> on newly created instance of that type with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="targetType">Type of class which will be proxied.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// New object of type <paramref name="targetType"/> proxying calls to virtual members of <paramref name="targetType"/> type.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="targetType"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="targetType"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="targetType"/> is not a class type.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on type <paramref name="targetType"/>.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of type <paramref name="targetType"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateClassProxy(Type targetType, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(targetType, ProxyGenerationOptions.Default, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to virtual members of type <paramref name="targetType"/> on newly created instance of that type with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="targetType">Type of class which will be proxied.</param>
		/// <param name="interfaces">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// New object of type <paramref name="targetType"/> proxying calls to virtual members of <paramref name="targetType"/> and <paramref name="interfaces"/> types.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="targetType"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="targetType"/> or any of <paramref name="interfaces"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="targetType"/> is not a class type.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on type <paramref name="targetType"/>.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of type <paramref name="targetType"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateClassProxy(Type targetType, Type[] interfaces, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(targetType, interfaces, ProxyGenerationOptions.Default, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to virtual members of type <paramref name="targetType"/> on newly created instance of that type with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="targetType">Type of class which will be proxied.</param>
		/// <param name="constructorArgs">Arguments of constructor of type <paramref name="targetType"/> which should be used to create a new instance of that type.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// New object of type <paramref name="targetType"/> proxying calls to virtual members of <paramref name="targetType"/> type.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="targetType"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="targetType"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="targetType"/> is not a class type.</exception>
		/// <exception cref="MissingMethodException">Thrown when no constructor exists on type <paramref name="targetType"/> with parameters matching <paramref name="constructorArgs"/>.</exception>
		/// <exception cref="TargetInvocationException">Thrown when constructor of type <paramref name="targetType"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateClassProxy(Type targetType, IInterceptor[] interceptors,
									   params object[] constructorArgs)
		{
			return CreateClassProxy(targetType, null, ProxyGenerationOptions.Default,
									constructorArgs, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to virtual members of type <paramref name="targetType"/> on newly created instance of that type with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="targetType">Type of class which will be proxied.</param>
		/// <param name="options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// New object of type <paramref name="targetType"/> proxying calls to virtual members of <paramref name="targetType"/> type.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="targetType"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="options"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="targetType"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="targetType"/> is not a class type.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on type <paramref name="targetType"/>.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of type <paramref name="targetType"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateClassProxy(Type targetType, ProxyGenerationOptions options, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(targetType, null, options, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to virtual members of type <paramref name="targetType"/> on newly created instance of that type with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="targetType">Type of class which will be proxied.</param>
		/// <param name="interfaces">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name="options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// New object of type <paramref name="targetType"/> proxying calls to virtual members of <paramref name="targetType"/> and <paramref name="interfaces"/> types.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="targetType"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="options"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="targetType"/> or any of <paramref name="interfaces"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="targetType"/> is not a class type.</exception>
		/// <exception cref="MissingMethodException">Thrown when no default constructor exists on type <paramref name="targetType"/>.</exception>
		/// <exception cref="TargetInvocationException">Thrown when default constructor of type <paramref name="targetType"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateClassProxy(Type targetType, Type[] interfaces,
									   ProxyGenerationOptions options, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(targetType, interfaces, options, null, interceptors);
		}

		/// <summary>
		/// Creates proxy object intercepting calls to virtual members of type <paramref name="targetType"/> on newly created instance of that type with given <paramref name="interceptors"/>.
		/// </summary>
        /// <param name="targetType">Type of class which will be proxied.</param>
		/// <param name="interfaces">Additional interface types. Calls to their members will be proxied as well.</param>
		/// <param name="options">The proxy generation options used to influence generated proxy type and object.</param>
		/// <param name="constructorArgs">Arguments of constructor of type <paramref name="targetType"/> which should be used to create a new instance of that type.</param>
		/// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
		/// <returns>
		/// New object of type <paramref name="targetType"/> proxying calls to virtual members of <paramref name="targetType"/> and <paramref name="interfaces"/> types.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="targetType"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentNullException">Thrown when given <paramref name="options"/> object is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="targetType"/> or any of <paramref name="interfaces"/> is a generic type definition.</exception>
		/// <exception cref="ArgumentException">Thrown when given <paramref name="targetType"/> is not a class type.</exception>
		/// <exception cref="MissingMethodException">Thrown when no constructor exists on type <paramref name="targetType"/> with parameters matching <paramref name="constructorArgs"/>.</exception>
		/// <exception cref="TargetInvocationException">Thrown when constructor of type <paramref name="targetType"/> throws an exception.</exception>
		/// <remarks>
		/// This method uses <see cref="IProxyBuilder"/> implementation to generate a proxy type.
		/// As such caller should expect any type of exception that given <see cref="IProxyBuilder"/> implementation may throw.
		/// </remarks>
		public object CreateClassProxy(Type targetType, Type[] interfaces, ProxyGenerationOptions options,object[] constructorArgs, params IInterceptor[] interceptors)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			if (!targetType.IsClass)
			{
				throw new ArgumentException("'targetType' must be a class", "targetType");
			}

			CheckNotGenericTypeDefinition(targetType, "targetType");
			CheckNotGenericTypeDefinitions(interfaces, "interfaces");

			Type proxyType = CreateClassProxyType(targetType, interfaces, options);

			// create constructor arguments (initialized with mixin implementations, interceptors and target type constructor arguments)
			List<object> arguments = new List<object>(options.MixinData.GetMixinInterfaceImplementationsAsArray());
			arguments.Add(interceptors);
			if (constructorArgs != null && constructorArgs.Length != 0)
			{
				arguments.AddRange(constructorArgs);
			}
			// create instance
			return Activator.CreateInstance(proxyType, arguments.ToArray());
		}

		#endregion

		/// <summary>
		/// Creates the proxy type for class proxy with given <paramref name="baseClass"/> class, implementing given <paramref name="interfaces"/> and using provided <paramref name="options"/>.
		/// </summary>
		/// <param name="baseClass">The base class for proxy type.</param>
		/// <param name="interfaces">The interfaces that proxy type should implement.</param>
		/// <param name="options">The options for proxy generation process.</param>
		/// <returns><see cref="Type"/> of proxy.</returns>
		protected Type CreateClassProxyType(Type baseClass, Type[] interfaces, ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateClassProxy(baseClass, interfaces, options);
		}

		/// <summary>
        /// Creates the proxy type for interface proxy with target for given <paramref name="theInterface"/> interface, implementing given <paramref name="interfaces"/> on given <paramref name="targetType"/> and using provided <paramref name="options"/>.
		/// </summary>
		/// <param name="theInterface">The interface proxy type should implement.</param>
		/// <param name="interfaces">The additional interfaces proxy type should implement.</param>
		/// <param name="targetType">Actual type that the proxy type will encompass.</param>
		/// <param name="options">The options for proxy generation process.</param>
		/// <returns><see cref="Type"/> of proxy.</returns>
		protected Type CreateInterfaceProxyTypeWithTarget(Type theInterface, Type[] interfaces, Type targetType,
														  ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateInterfaceProxyTypeWithTarget(theInterface, interfaces, targetType, options);
		}

		/// <summary>
        /// Creates the proxy type for interface proxy with target interface for given <paramref name="theInterface"/> interface, implementing given <paramref name="interfaces"/> on given <paramref name="targetType"/> and using provided <paramref name="options"/>.
		/// </summary>
		/// <param name="theInterface">The interface proxy type should implement.</param>
		/// <param name="interfaces">The additional interfaces proxy type should implement.</param>
		/// <param name="targetType">Actual type that the proxy type will encompass.</param>
		/// <param name="options">The options for proxy generation process.</param>
		/// <returns><see cref="Type"/> of proxy.</returns>
		protected Type CreateInterfaceProxyTypeWithTargetInterface(Type theInterface, Type[] interfaces, Type targetType,
																   ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(theInterface, interfaces, options);
		}

		/// <summary>
		/// Creates the proxy type for interface proxy without target for given <paramref name="theInterface"/> interface, implementing given <paramref name="interfaces"/> and using provided <paramref name="options"/>.
		/// </summary>
		/// <param name="theInterface">The interface proxy type should implement.</param>
		/// <param name="interfaces">The additional interfaces proxy type should implement.</param>
		/// <param name="options">The options for proxy generation process.</param>
		/// <returns><see cref="Type"/> of proxy.</returns>
		protected Type CreateInterfaceProxyTypeWithoutTarget(Type theInterface, Type[] interfaces,
															 ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateInterfaceProxyTypeWithoutTarget(theInterface, interfaces, options);
		}
	}
}
