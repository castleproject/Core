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
	using Castle.Core.Interceptor;

	[CLSCompliant(true)]
	public class ProxyGenerator
	{
		private readonly IProxyBuilder proxyBuilder;

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyGenerator"/> class.
		/// </summary>
		/// <param name="builder">The builder.</param>
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
		/// Gets the proxy builder instance.
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

		public T CreateInterfaceProxyWithTarget<T>(object target, params IInterceptor[] interceptors)
		{
			return (T) CreateInterfaceProxyWithTarget(typeof (T), target, ProxyGenerationOptions.Default, interceptors);
		}

		public T CreateInterfaceProxyWithTarget<T>(object target, ProxyGenerationOptions options,
		                                           params IInterceptor[] interceptors)
		{
			return (T) CreateInterfaceProxyWithTarget(typeof (T), target, options, interceptors);
		}

		public object CreateInterfaceProxyWithTarget(Type theInterface, object target, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(theInterface, target, ProxyGenerationOptions.Default, interceptors);
		}

		public object CreateInterfaceProxyWithTarget(Type theInterface, object target, ProxyGenerationOptions options,
		                                             params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(theInterface, null, target, options, interceptors);
		}

		public object CreateInterfaceProxyWithTarget(Type theInterface, Type[] interfaces,
		                                             object target, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(theInterface, interfaces, target, ProxyGenerationOptions.Default, interceptors);
		}

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

			ArrayList arguments = GetConstructorArguments(target, interceptors, options);
			return Activator.CreateInstance(generatedType, arguments.ToArray());
		}

		private ArrayList GetConstructorArguments(object target, IInterceptor[] interceptors, ProxyGenerationOptions options)
		{
			// create constructor arguments (initialized with mixin implementations, interceptors and target type constructor arguments)
			ArrayList arguments = new ArrayList(options.MixinData.GetMixinInterfaceImplementationsAsArray());
			arguments.Add(interceptors);
			arguments.Add(target);
			return arguments;
		}

		#endregion

		#region CreateInterfaceProxyWithTargetInterface

		public object CreateInterfaceProxyWithTargetInterface(Type theInterface, object target,
		                                                      params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTargetInterface(theInterface, target, ProxyGenerationOptions.Default, interceptors);
		}

		public object CreateInterfaceProxyWithTargetInterface(Type theInterface, object target,
															  ProxyGenerationOptions options,
			                                                  params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTargetInterface(theInterface, null, target, options, interceptors);
		}

		public object CreateInterfaceProxyWithTargetInterface(Type theInterface, Type[] interfaces, object target,
		                                                      ProxyGenerationOptions options,
		                                                      params IInterceptor[] interceptors)
		{
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
			ArrayList arguments = GetConstructorArguments(target, interceptors, options);
			return Activator.CreateInstance(generatedType, arguments.ToArray());
		}

		#endregion

		#region CreateInterfaceProxyWithoutTarget

		public T CreateInterfaceProxyWithoutTarget<T>(IInterceptor interceptor)
		{
			return (T) CreateInterfaceProxyWithoutTarget(typeof (T), interceptor);
		}

		public T CreateInterfaceProxyWithoutTarget<T>(params IInterceptor[] interceptors)
		{
			return (T) CreateInterfaceProxyWithoutTarget(typeof (T), interceptors);
		}

		public object CreateInterfaceProxyWithoutTarget(Type theInterface, IInterceptor interceptor)
		{
			return CreateInterfaceProxyWithoutTarget(theInterface, new Type[0],
			                                         ProxyGenerationOptions.Default,
			                                         interceptor);
		}

		public object CreateInterfaceProxyWithoutTarget(Type theInterface, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithoutTarget(theInterface, new Type[0], ProxyGenerationOptions.Default, interceptors);
		}

		public object CreateInterfaceProxyWithoutTarget(Type theInterface, Type[] interfaces,
		                                                params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithoutTarget(theInterface, interfaces, ProxyGenerationOptions.Default, interceptors);
		}

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
			ArrayList arguments = GetConstructorArguments(new object(), interceptors, options);
			return Activator.CreateInstance(generatedType, arguments.ToArray());
		}

		#endregion

		#region CreateClassProxy

		public T CreateClassProxy<T>(params IInterceptor[] interceptors)
		{
			return (T) CreateClassProxy(typeof (T), ProxyGenerationOptions.Default, interceptors);
		}

		public object CreateClassProxy(Type targetType, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(targetType, ProxyGenerationOptions.Default, interceptors);
		}

		/// <summary>
		/// Creates the class proxy.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="interfaces">The interfaces.</param>
		/// <param name="interceptors">The interceptors.</param>
		/// <returns></returns>
		public object CreateClassProxy(Type targetType, Type[] interfaces, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(targetType, interfaces, ProxyGenerationOptions.Default, interceptors);
		}

		/// <summary>
		/// Creates the class proxy.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="interceptors">The interceptors.</param>
		/// <param name="constructorArgs">The constructor args.</param>
		/// <returns></returns>
		public object CreateClassProxy(Type targetType, IInterceptor[] interceptors,
		                               params object[] constructorArgs)
		{
			return CreateClassProxy(targetType, null, ProxyGenerationOptions.Default,
			                        constructorArgs, interceptors);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="options"></param>
		/// <param name="interceptors"></param>
		/// <returns></returns>
		public object CreateClassProxy(Type targetType, ProxyGenerationOptions options, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(targetType, null, options, interceptors);
		}

		public object CreateClassProxy(Type targetType, Type[] interfaces,
		                               ProxyGenerationOptions options, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(targetType, interfaces, options, null, interceptors);
		}

		/// <summary>
		/// Creates the class proxy.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="interfaces">The interfaces.</param>
		/// <param name="options">The options.</param>
		/// <param name="constructorArgs">The constructor args.</param>
		/// <param name="interceptors">The interceptors.</param>
		/// <returns></returns>
		public object CreateClassProxy(Type targetType, Type[] interfaces, ProxyGenerationOptions options,
		                               object[] constructorArgs, params IInterceptor[] interceptors)
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
			ArrayList arguments = new ArrayList(options.MixinData.GetMixinInterfaceImplementationsAsArray());
			arguments.Add(interceptors);
			if (constructorArgs != null && constructorArgs.Length != 0)
			{
				arguments.AddRange(constructorArgs);
			}
			// create instance
			return Activator.CreateInstance(proxyType, arguments.ToArray());
		}

		#endregion

		protected Type CreateClassProxyType(Type baseClass, Type[] interfaces, ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateClassProxy(baseClass, interfaces, options);
		}

		protected Type CreateInterfaceProxyTypeWithTarget(Type theInterface, Type[] interfaces, Type targetType,
		                                                  ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateInterfaceProxyTypeWithTarget(theInterface, interfaces, targetType, options);
		}

		protected Type CreateInterfaceProxyTypeWithTargetInterface(Type theInterface, Type[] interfaces, Type targetType,
		                                                           ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(theInterface, interfaces, options);
		}

		protected Type CreateInterfaceProxyTypeWithoutTarget(Type theInterface, Type[] interfaces,
		                                                     ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateInterfaceProxyTypeWithoutTarget(theInterface, interfaces, options);
		}
	}
}
