// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	using Castle.DynamicProxy.Builder;

	/// <summary>
	/// Generates a Java style proxy. This overrides the .Net proxy requirements 
	/// that forces one to extend MarshalByRefObject or (for a different purpose)
	/// ContextBoundObject to have a Proxiable class.
	/// </summary>
	/// <remarks>
	/// The <see cref="ProxyGenerator"/> should be used to generate a class 
	/// implementing the specified interfaces. The dynamic implementation will 
	/// only calls the internal <see cref="IInterceptor"/> instance.
	/// </remarks>
	/// <remarks>
	/// Please note that this proxy implementation currently doesn't not supports ref and out arguments 
	/// in methods.
	/// Also note that only virtual methods can be proxied in a class.
	/// </remarks>
	/// <example>
	/// <code>
	/// MyInvocationHandler interceptor = ...
	/// ProxyGenerator generator = new ProxyGenerator();
	/// IInterfaceExposed proxy = 
	///		generator.CreateProxy( new Type[] { typeof(IInterfaceExposed) }, interceptor );
	/// </code>
	/// </example>
	public class ProxyGenerator
	{
		private IProxyBuilder _builder;

		public ProxyGenerator(IProxyBuilder builder)
		{
			_builder = builder;
		}

		public ProxyGenerator() : this( new DefaultProxyBuilder() )
		{
		}

		public IProxyBuilder ProxyBuilder
		{
			get { return _builder; }
			set { _builder = value; }
		}

		public virtual object CreateClassProxy(Type baseClass, IInterceptor interceptor, 
			params object[] argumentsForConstructor)
		{
			AssertUtil.IsClass(baseClass, "baseClass");
			AssertUtil.NotNull(interceptor, "interceptor");

			Type newType = ProxyBuilder.CreateClassProxy(baseClass);
			return CreateClassProxyInstance( newType, interceptor, argumentsForConstructor );
		}

		

		public virtual object CreateCustomClassProxy(Type baseClass, 
			IInterceptor interceptor, GeneratorContext context, 
			params object[] argumentsForConstructor)
		{
			AssertUtil.IsClass(baseClass, "baseClass");
			AssertUtil.NotNull(interceptor, "interceptor");
			AssertUtil.NotNull(context, "context");

			Type newType = ProxyBuilder.CreateCustomClassProxy(baseClass, context);
			return CreateCustomClassProxyInstance( newType, interceptor, context, 
				argumentsForConstructor );
		}

		/// <summary>
		/// Generates a proxy implementing all the specified interfaces and
		/// redirecting method invocations to the specifed interceptor.
		/// </summary>
		/// <param name="theInterface">Interface to be implemented</param>
		/// <param name="interceptor">instance of <see cref="IInterceptor"/></param>
		/// <returns>Proxy instance</returns>
		public virtual object CreateProxy(Type theInterface, IInterceptor interceptor, object target)
		{
			return CreateProxy(new Type[] {theInterface}, interceptor, target);
		}

		/// <summary>
		/// Generates a proxy implementing all the specified interfaces and
		/// redirecting method invocations to the specifed interceptor.
		/// </summary>
		/// <param name="interfaces">Array of interfaces to be implemented</param>
		/// <param name="interceptor">instance of <see cref="IInterceptor"/></param>
		/// <returns>Proxy instance</returns>
		public virtual object CreateProxy(Type[] interfaces, IInterceptor interceptor, object target)
		{
			AssertUtil.IsInterface(interfaces, "interfaces");
			AssertUtil.NotNull(interceptor, "interceptor");
			AssertUtil.NotNull(target, "target");

			Type newType = ProxyBuilder.CreateInterfaceProxy(interfaces);
			return CreateProxyInstance( newType, interceptor, target );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="theInterface"></param>
		/// <param name="interceptor"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public virtual object CreateCustomProxy(Type theInterface, 
			IInterceptor interceptor, object target, GeneratorContext context )
		{
			return CreateCustomProxy( new Type[] { theInterface }, interceptor, target, context );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="interfaces"></param>
		/// <param name="interceptor"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public virtual object CreateCustomProxy(Type[] interfaces, 
			IInterceptor interceptor, object target, GeneratorContext context )
		{
			AssertUtil.IsInterface(interfaces, "interfaces");
			AssertUtil.NotNull(interceptor, "interceptor");
			AssertUtil.NotNull(target, "target");
			AssertUtil.NotNull(context, "context");

			Type newType = ProxyBuilder.CreateCustomInterfaceProxy(interfaces, context);
			return CreateCustomProxyInstance( newType, interceptor, target, context );
		}

		protected virtual object CreateProxyInstance(Type type, IInterceptor interceptor, object target)
		{
			return Activator.CreateInstance(type, new object[] {interceptor, target});
		}

		protected virtual object CreateCustomProxyInstance(Type type, IInterceptor interceptor, 
			object target, GeneratorContext context)
		{
			if (context.HasMixins)
			{
				return Activator.CreateInstance( type, new object[] { interceptor, target, context.MixinsAsArray() } );
			}
			return CreateProxyInstance( type, interceptor, target );
		}

		protected virtual object CreateClassProxyInstance(Type type, IInterceptor interceptor, 
			params object[] argumentsForConstructor)
		{
			ArrayList args = new ArrayList();
			args.Add(interceptor);
			args.AddRange(argumentsForConstructor);

			return Activator.CreateInstance(type, args.ToArray());
		}

		protected virtual object CreateCustomClassProxyInstance(Type type, IInterceptor interceptor, 
			GeneratorContext context, params object[] argumentsForConstructor)
		{
			if (context.HasMixins)
			{
				ArrayList args = new ArrayList();
				args.Add(interceptor);
				args.Add(context.MixinsAsArray());
				args.AddRange(argumentsForConstructor);

				return Activator.CreateInstance( type, args.ToArray() );
			}
			return CreateClassProxyInstance( type, interceptor, argumentsForConstructor );
		}

		protected virtual object CreateCustomClassProxyInstance(Type type, IInterceptor interceptor, 
			GeneratorContext context, object target)
		{
			return CreateProxyInstance( type, interceptor, target );
		}
	}
}