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

//		#region CreateInterfaceProxyWithTarget overloads
//
//		public object CreateInterfaceProxyWithTarget(Type theInterface, IInterceptor interceptor, 
//		                                             object target)
//		{
//			return CreateInterfaceProxyWithTarget(theInterface, new IInterceptor[] {interceptor}, 
//			                                      target);
//		}
//
//		public object CreateInterfaceProxyWithTarget(Type theInterface, IInterceptor[] interceptors, 
//		                                             object target)
//		{
//			return CreateInterfaceProxyWithTarget(theInterface, interceptors, target, 
//			                                      ProxyGenerationOptions.Default);
//		}
//
//		public object CreateInterfaceProxyWithTarget(Type theInterface, IInterceptor[] interceptors, 
//		                                             object target, ProxyGenerationOptions options)
//		{
//			// TODO: Make sure the target is not null and implement the interface
//			
//			Type type = CreateInterfaceProxyTypeWithTarget(theInterface, target.GetType(), options);
//
//			if (theInterface.IsGenericType)
//			{
//				Type[] args = theInterface.GetGenericArguments();
//				type = type.MakeGenericType(args);
//			}
//
//			return Activator.CreateInstance(type, new object[] { target, interceptors });
//		}
//
//		#endregion

		#region CreateClassProxy overloads

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
			
#if DOTNET2
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
#endif
			{
				proxyType = CreateClassProxyType(targetType, interfaces, options);
			}

#if DOTNET2
			if (targetType.IsGenericType)
			{
				proxyType = proxyType.MakeGenericType(targetType.GetGenericArguments());
			}
#endif

			return Activator.CreateInstance(proxyType, new object[] { interceptors });
		}

		#endregion

		protected Type CreateClassProxyType(Type baseClass, Type[] interfaces, ProxyGenerationOptions options)
		{
			return ProxyBuilder.CreateClassProxy(baseClass, interfaces, options);
		}

//		protected Type CreateInterfaceProxyTypeWithTarget(Type theInterface, Type targetType, 
//		                                                  ProxyGenerationOptions options)
//		{
//			return ProxyBuilder.CreateInterfaceProxyTypeWithTarget(theInterface, targetType, options);
//		}
	}
}
