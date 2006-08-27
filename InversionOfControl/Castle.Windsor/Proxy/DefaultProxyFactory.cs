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

using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using IInterceptor = Castle.DynamicProxy.IInterceptor;
using IInvocation = Castle.DynamicProxy.IInvocation;

namespace Castle.Windsor.Proxy
{
	using System;
	using System.Reflection;
	using System.Runtime.Serialization;
	
	using Castle.Core;
	using Castle.MicroKernel;

	/// <summary>
	/// This implementation of <see cref="IProxyFactory"/> relies 
	/// on DynamicProxy to expose proxy capabilies.
	/// </summary>
	/// <remarks>
	/// Note that only virtual methods can be intercepted in a 
	/// concrete class. However, if the component 
	/// was registered with a service interface, we proxy
	/// the interface and the methods don't need to be virtual,
	/// </remarks>
	[Serializable]
	public class DefaultProxyFactory : AbstractProxyFactory, IDeserializationCallback
	{
		[NonSerialized]
		protected ProxyGenerator generator;

		/// <summary>
		/// Constructs a DefaultProxyFactory
		/// </summary>
		public DefaultProxyFactory()
		{
			Init();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		/// <param name="constructorArguments"></param>
		/// <returns></returns>
		public override object Create(IKernel kernel, ComponentModel model, params object[] constructorArguments)
		{
			IMethodInterceptor[] interceptors = ObtainInterceptors(kernel, model);
			IInterceptor interceptorChain = new InterceptorChain(interceptors);

			// This is a hack to avoid unnecessary object creation 
			// and unecessary delegations.
			// We supply our contracts (Interceptor and Invocation)
			// and the implementation for Invocation dispatchers
			// DynamicProxy should be able to use them as long as the 
			// signatures match
			GeneratorContext context = new GeneratorContext();
			context.Interceptor = typeof(IMethodInterceptor);
			context.Invocation = typeof(IMethodInvocation);
			context.SameClassInvocation = typeof(DefaultMethodInvocation);
			context.InterfaceInvocation = typeof(DefaultMethodInvocation);

			CustomizeContext(context, kernel, model, constructorArguments);

			object proxy = null;

			if (model.Service.IsInterface)
			{
				Object target = Activator.CreateInstance( model.Implementation, constructorArguments );

				proxy = generator.CreateCustomProxy( CollectInterfaces(model.Service, model.Implementation), 
					interceptorChain, target, context);
			}
			else
			{
				proxy = generator.CreateCustomClassProxy(model.Implementation, 
					interceptorChain, context, constructorArguments);
			}

			CustomizeProxy(proxy, context, kernel, model);

			return proxy;
		}

		protected virtual void CustomizeProxy(object proxy, GeneratorContext context, 
			IKernel kernel, ComponentModel model)
		{
		}

		protected virtual void CustomizeContext(GeneratorContext context, IKernel kernel, 
			ComponentModel model, object[] arguments)
		{
		}

		protected Type[] CollectInterfaces(Type service, Type implementation)
		{
			Type[] interfaces = implementation.FindInterfaces(new TypeFilter(EmptyTypeFilter), null);

			if (interfaces.Length == 0)
			{
				return new Type[] { service };
			}

			return interfaces;
		}

		private bool EmptyTypeFilter(Type type, object criteria)
		{
			return true;
		}

		#region IDeserializationCallback

		public void OnDeserialization(object sender)
		{
			Init();
		}

		#endregion

		private void Init()
		{
			generator = new ProxyGenerator();
		}
	}
}
