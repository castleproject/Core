// Copyright 2004 The Apache Software Foundation
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

namespace Apache.Avalon.DynamicProxy
{
	using System;
	using System.Reflection.Emit;

	using Apache.Avalon.DynamicProxy.Builder;

	public delegate void EnhanceTypeDelegate( TypeBuilder mainType, FieldBuilder handlerFieldBuilder, ConstructorBuilder constructorBuilder );

	public delegate Type[] ScreenInterfacesDelegate( Type[] interfaces );

	/// <summary>
	/// Generates a Java style proxy. This overrides the .Net proxy requirements 
	/// that forces one to extend MarshalByRefObject or (for a different purpose)
	/// ContextBoundObject to have a Proxiable class.
	/// </summary>
	/// <remarks>
	/// The <see cref="ProxyGenerator"/> should be used to generate a class 
	/// implementing the specified interfaces. The dynamic implementation will 
	/// only calls the internal <see cref="IInvocationHandler"/> instance.
	/// </remarks>
	/// <remarks>
	/// Please note that this proxy implementation currently doesn't not supports ref and out arguments 
	/// in methods.
	/// Also note that only virtual methods can be proxied in a class.
	/// </remarks>
	/// <example>
	/// <code>
	/// MyInvocationHandler handler = ...
	/// ProxyGenerator generator = new ProxyGenerator();
	/// IInterfaceExposed proxy = 
	///		generator.CreateProxy( new Type[] { typeof(IInterfaceExposed) }, handler );
	/// </code>
	/// </example>
	public class ProxyGenerator
	{
		private IProxyBuilder m_builder;

		public ProxyGenerator(IProxyBuilder builder)
		{
			m_builder = builder;
		}

		public ProxyGenerator() : this( new DefaultProxyBuilder() )
		{
		}

		public IProxyBuilder ProxyBuilder
		{
			get { return m_builder; }
			set { m_builder = value; }
		}

		public virtual object CreateClassProxy(Type baseClass, IInvocationHandler handler)
		{
			AssertCreateClassProxyArguments(baseClass, handler);

			Type newType = ProxyBuilder.CreateClassProxy(baseClass);
			return CreateProxyInstance( newType, handler );
		}

		public virtual object CreateCustomClassProxy(Type baseClass, 
			IInvocationHandler handler, GeneratorContext context)
		{
			AssertCreateClassProxyArguments(baseClass, handler, context);

			Type newType = ProxyBuilder.CreateCustomClassProxy(baseClass, context);
			return CreateProxyInstance( newType, handler, context );
		}

		/// <summary>
		/// Generates a proxy implementing all the specified interfaces and
		/// redirecting method invocations to the specifed handler.
		/// </summary>
		/// <param name="theInterface">Interface to be implemented</param>
		/// <param name="handler">instance of <see cref="IInvocationHandler"/></param>
		/// <returns>Proxy instance</returns>
		public virtual object CreateProxy(Type theInterface, IInvocationHandler handler)
		{
			return CreateProxy(new Type[] {theInterface}, handler);
		}

		/// <summary>
		/// Generates a proxy implementing all the specified interfaces and
		/// redirecting method invocations to the specifed handler.
		/// </summary>
		/// <param name="interfaces">Array of interfaces to be implemented</param>
		/// <param name="handler">instance of <see cref="IInvocationHandler"/></param>
		/// <returns>Proxy instance</returns>
		public virtual object CreateProxy(Type[] interfaces, IInvocationHandler handler)
		{
			AssertCreateProxyArguments(interfaces, handler);

			Type newType = ProxyBuilder.CreateInterfaceProxy(interfaces);
			return CreateProxyInstance( newType, handler );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="theInterface"></param>
		/// <param name="handler"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public virtual object CreateCustomProxy(Type theInterface, 
			IInvocationHandler handler, 
			GeneratorContext context )
		{
			return CreateCustomProxy( new Type[] { theInterface }, handler, context );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="interfaces"></param>
		/// <param name="handler"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public virtual object CreateCustomProxy(Type[] interfaces, 
			IInvocationHandler handler, GeneratorContext context )
		{
			AssertCreateProxyArguments( interfaces, handler, context );
			Type newType = ProxyBuilder.CreateCustomInterfaceProxy(interfaces, context);
			return CreateProxyInstance( newType, handler, context );
		}

		protected virtual object CreateProxyInstance(Type type, IInvocationHandler handler)
		{
			return Activator.CreateInstance(type, new object[] {handler});
		}

		protected virtual object CreateProxyInstance(Type type, IInvocationHandler handler, GeneratorContext context)
		{
			return CreateProxyInstance( type, handler );
		}

		protected static void AssertCreateProxyArguments(Type[] interfaces, IInvocationHandler handler)
		{
			if (interfaces == null)
			{
				throw new ArgumentNullException("interfaces");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (interfaces.Length == 0)
			{
				throw new ArgumentException("Can't handle an empty interface array");
			}
		}

		protected static void AssertCreateProxyArguments(Type[] interfaces, IInvocationHandler handler, GeneratorContext context)
		{
			AssertCreateProxyArguments(interfaces, handler);

			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
		}

		protected static void AssertCreateClassProxyArguments(Type baseClass, IInvocationHandler handler)
		{
			if (baseClass == null)
			{
				throw new ArgumentNullException("theClass");
			}
			if (baseClass.IsInterface)
			{
				throw new ArgumentException("'baseClass' must be a class, not an interface");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
		}

		protected static void AssertCreateClassProxyArguments(Type baseClass, IInvocationHandler handler, GeneratorContext context)
		{
			AssertCreateClassProxyArguments(baseClass, handler);
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
		}
	}
}