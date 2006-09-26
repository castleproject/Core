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

	/// <summary>
	/// 
	/// </summary>
	public class ProxyGenerator
	{
		private readonly IProxyBuilder proxyBuilder;

		public ProxyGenerator(IProxyBuilder builder)
		{
			proxyBuilder = builder;
		}

		public ProxyGenerator() : this(new DefaultProxyBuilder())
		{
		}

		public IProxyBuilder ProxyBuilder
		{
			get { return proxyBuilder; }
		}

		public T CreateInterfaceProxyWithTarget<T>(object target, params IInterceptor[] interceptors)
		{
			return (T) CreateInterfaceProxyWithTarget(typeof(T), target, ProxyGenerationOptions.Default, interceptors);
		}

		public T CreateInterfaceProxyWithTarget<T>(object target, ProxyGenerationOptions options, params IInterceptor[] interceptors)
		{
			return (T) CreateInterfaceProxyWithTarget(typeof(T), target, options, interceptors);
		}

		public object CreateInterfaceProxyWithTarget(Type theInterface, object target, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(theInterface, target, ProxyGenerationOptions.Default, interceptors);
		}

		public object CreateInterfaceProxyWithTarget(Type theInterface, object target, ProxyGenerationOptions options, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(theInterface, null, target, options, interceptors);
		}

		public object CreateInterfaceProxyWithTarget(Type theInterface, Type[] interfaces, object target, ProxyGenerationOptions options, params IInterceptor[] interceptors)
		{
			if (theInterface == null) throw new ArgumentNullException("theInterface");
			if (target == null) throw new ArgumentNullException("target");
			if (interceptors == null) throw new ArgumentNullException("interceptors");

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
				generatedType = CreateInterfaceProxyTypeWithTarget(theInterface.GetGenericTypeDefinition(), interfaces, targetType.GetGenericTypeDefinition(), options);
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

			return Activator.CreateInstance(generatedType, new object[] { interceptors, target });
		}

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
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="interfaces"></param>
		/// <param name="options"></param>
		/// <param name="interceptors"></param>
		/// <returns></returns>
		public object CreateClassProxy(Type targetType, Type[] interfaces, ProxyGenerationOptions options, params IInterceptor[] interceptors)
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

			return Activator.CreateInstance(proxyType, new object[] { interceptors });
		}

		protected Type CreateClassProxyType(Type baseClass, Type[] interfaces, ProxyGenerationOptions options)
		{
			return ProxyBuilder.CreateClassProxy(baseClass, interfaces, options);
		}

		protected Type CreateInterfaceProxyTypeWithTarget(Type theInterface, Type[] interfaces, Type targetType, ProxyGenerationOptions options)
		{
			return ProxyBuilder.CreateInterfaceProxyTypeWithTarget(theInterface, interfaces, targetType, options);
		}
	}
}
