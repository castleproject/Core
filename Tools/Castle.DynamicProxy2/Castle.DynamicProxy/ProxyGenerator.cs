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

namespace Castle.DynamicProxy
{
	using System;
	using Castle.Core.Interceptor;

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

		#region CreateInterfaceProxyWithTarget

		public T CreateInterfaceProxyWithTarget<T>(object target, params IInterceptor[] interceptors)
		{
			return (T) CreateInterfaceProxyWithTarget(typeof(T), target, ProxyGenerationOptions.Default, interceptors);
		}

		public T CreateInterfaceProxyWithTarget<T>(object target, ProxyGenerationOptions options,
		                                           params IInterceptor[] interceptors)
		{
			return (T) CreateInterfaceProxyWithTarget(typeof(T), target, options, interceptors);
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
			return CreateInterfaceProxyWithTarget(theInterface, null, target, ProxyGenerationOptions.Default, interceptors);	
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

			if (theInterface.IsGenericTypeDefinition)
			{
				throw new ArgumentException("You can't specify a generic interface definition", "theInterface");
			}

			Type targetType = target.GetType();
			Type generatedType;

			if (theInterface.IsGenericType)
			{
				generatedType =
					CreateInterfaceProxyTypeWithTarget(theInterface.GetGenericTypeDefinition(), interfaces,
					                                   targetType.GetGenericTypeDefinition(), options);
			}
			else
			{
				generatedType = CreateInterfaceProxyTypeWithTarget(theInterface, interfaces, targetType, options);
			}

			if (theInterface.IsGenericType)
			{
				Type[] args = theInterface.GetGenericArguments();
				generatedType = generatedType.MakeGenericType(args);
			}

			return Activator.CreateInstance(generatedType, new object[] {interceptors, target});
		}

		#endregion

		#region CreateInterfaceProxyWithoutTarget

		public T CreateInterfaceProxyWithoutTarget<T>(IInterceptor interceptor)
		{
			return (T) CreateInterfaceProxyWithoutTarget(typeof(T), interceptor);
		}

		public T CreateInterfaceProxyWithoutTarget<T>(params IInterceptor[] interceptors)
		{
			return (T) CreateInterfaceProxyWithoutTarget(typeof(T), interceptors);
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

			if (theInterface.IsGenericTypeDefinition)
			{
				throw new ArgumentException("You can't specify a generic interface definition", "theInterface");
			}

			Type generatedType;

			if (theInterface.IsGenericType)
			{
				generatedType = CreateInterfaceProxyTypeWithoutTarget(theInterface.GetGenericTypeDefinition(), interfaces, options);
			}
			else
			{
				generatedType = CreateInterfaceProxyTypeWithoutTarget(theInterface, interfaces, options);
			}

			if (theInterface.IsGenericType)
			{
				Type[] args = theInterface.GetGenericArguments();
				generatedType = generatedType.MakeGenericType(args);
			}

			return Activator.CreateInstance(generatedType, new object[] {interceptors, new object(),});
		}

		public Type CreateInterfaceProxyTypeWithoutTarget(Type theInterface, Type[] interfaces, ProxyGenerationOptions options)
		{
			return ProxyBuilder.CreateInterfaceProxyTypeWithoutTarget(theInterface, interfaces, options);
		}

		#endregion

		#region CreateClassProxy

		public T CreateClassProxy<T>(params IInterceptor[] interceptors)
		{
			return (T) CreateClassProxy(typeof(T), ProxyGenerationOptions.Default, interceptors);
		}

		public object CreateClassProxy(Type targetType, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(targetType, ProxyGenerationOptions.Default, interceptors);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="interfaces"></param>
		/// <param name="interceptors"></param>
		/// <returns></returns>
		public object CreateClassProxy(Type targetType, Type[] interfaces, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(targetType, interfaces, ProxyGenerationOptions.Default, interceptors);
		}
		
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
		/// 
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="interfaces"></param>
		/// <param name="options"></param>
		/// <param name="interceptors"></param>
		/// <param name="constructorArgs"></param>
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

// #if DOTNET2
			if (targetType.IsGenericTypeDefinition)
			{
				throw new ArgumentException("You can't specify a generic type definition", "baseClass");
			}

			Type proxyType;

			if (targetType.IsGenericType)
			{
				proxyType = CreateClassProxyType(targetType.GetGenericTypeDefinition(), interfaces, options);
			}
			else
// #endif
			{
				proxyType = CreateClassProxyType(targetType, interfaces, options);
			}

			// #if DOTNET2
			if (targetType.IsGenericType)
			{
				proxyType = proxyType.MakeGenericType(targetType.GetGenericArguments());
			}
			// #endif

			object[] args;

			if (constructorArgs != null && constructorArgs.Length != 0)
			{
				args = new object[constructorArgs.Length + 1];
				args[0] = interceptors;

				Array.Copy(constructorArgs, 0, args, 1, constructorArgs.Length);
			}
			else
			{
				args = new object[] { interceptors };
			}

			return Activator.CreateInstance(proxyType, args);
		}

		#endregion

		protected Type CreateClassProxyType(Type baseClass, Type[] interfaces, ProxyGenerationOptions options)
		{
			return ProxyBuilder.CreateClassProxy(baseClass, interfaces, options);
		}

		protected Type CreateInterfaceProxyTypeWithTarget(Type theInterface, Type[] interfaces, Type targetType,
		                                                  ProxyGenerationOptions options)
		{
			return ProxyBuilder.CreateInterfaceProxyTypeWithTarget(theInterface, interfaces, targetType, options);
		}
	}
}