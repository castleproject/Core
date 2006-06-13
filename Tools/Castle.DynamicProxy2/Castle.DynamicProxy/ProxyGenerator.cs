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

	public class ProxyGenerator
	{
		private IProxyBuilder proxyBuilder;

		public ProxyGenerator() : this(new DefaultProxyBuilder())
		{
		}

		public ProxyGenerator(IProxyBuilder builder)
		{
			proxyBuilder = builder;
		}

		public IProxyBuilder ProxyBuilder
		{
			get { return proxyBuilder; }
			set { proxyBuilder = value; }
		}

		#region CreateInterfaceProxyWithTarget overloads

		public object CreateInterfaceProxyWithTarget(Type theInterface, IInterceptor interceptor, 
		                                             object target)
		{
			return CreateInterfaceProxyWithTarget(theInterface, new IInterceptor[] {interceptor}, 
			                                      target);
		}

		public object CreateInterfaceProxyWithTarget(Type theInterface, IInterceptor[] interceptors, 
		                                             object target)
		{
			return CreateInterfaceProxyWithTarget(theInterface, interceptors, target, 
			                                      ProxyGenerationOptions.Default);
		}

		public object CreateInterfaceProxyWithTarget(Type theInterface, IInterceptor[] interceptors, 
		                                             object target, ProxyGenerationOptions options)
		{
			// TODO: Make sure the target is not null and implement the interface
			
			Type type = CreateInterfaceProxyTypeWithTarget(theInterface, target.GetType(), options);

			if (theInterface.IsGenericType)
			{
				Type[] args = theInterface.GetGenericArguments();
				type = type.MakeGenericType(args);
			}

			return Activator.CreateInstance(type, new object[] { target, interceptors });
		}

		#endregion

		#region CreateClassProxy overloads

		public object CreateClassProxy(Type baseClass, IInterceptor interceptor)
		{
			return CreateClassProxy(baseClass, new IInterceptor[] {interceptor} );
		}

		public object CreateClassProxy(Type baseClass, IInterceptor[] interceptors)
		{
#if DOTNET2
			if (baseClass.IsGenericTypeDefinition)
			{
				throw new ArgumentException("You can't specify a generic type definition", "baseClass");
			}
#endif

			Type type = CreateClassProxyType(baseClass, ProxyGenerationOptions.Default);

#if DOTNET2
			if (baseClass.IsGenericType)
			{
				type = type.MakeGenericType(baseClass.GetGenericArguments());
			}
#endif
			
			return Activator.CreateInstance(type, new object[] { interceptors });
		}

		#endregion

		protected Type CreateClassProxyType(Type baseClass, ProxyGenerationOptions options)
		{
			return ProxyBuilder.CreateClassProxy(baseClass, options);
		}

		protected Type CreateInterfaceProxyTypeWithTarget(Type theInterface, Type targetType, 
		                                                  ProxyGenerationOptions options)
		{
			return ProxyBuilder.CreateInterfaceProxyTypeWithTarget(theInterface, targetType, options);
		}
	}
}
