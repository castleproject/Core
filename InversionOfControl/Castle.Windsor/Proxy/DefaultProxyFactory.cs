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

namespace Castle.Windsor.Proxy
{
	using System;
	using System.Reflection;
	using System.Runtime.Serialization;

	using Castle.Model;
	using Castle.Model.Interceptor;

	using Castle.MicroKernel;
	
	using Castle.DynamicProxy;

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
	public class DefaultProxyFactory : IProxyFactory, IDeserializationCallback
	{
		[NonSerialized]
		protected ProxyGenerator _generator;

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
		public object Create(IKernel kernel, ComponentModel model, params object[] constructorArguments)
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

				proxy = _generator.CreateCustomProxy( CollectInterfaces(model.Service, model.Implementation), 
					interceptorChain, target, context);
			}
			else
			{
				proxy = _generator.CreateCustomClassProxy(model.Implementation, 
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

		protected IMethodInterceptor[] ObtainInterceptors(IKernel kernel, ComponentModel model)
		{
			IMethodInterceptor[] interceptors = new IMethodInterceptor[model.Interceptors.Count];
			int index = 0;

			foreach(InterceptorReference interceptorRef in model.Interceptors)
			{
				IHandler handler = null;

				if (interceptorRef.ReferenceType == InterceptorReferenceType.Interface)
				{
					handler = kernel.GetHandler( interceptorRef.ServiceType );
				}
				else
				{
					handler = kernel.GetHandler( interceptorRef.ComponentKey );
				}

				if (handler == null)
				{
					// This shoul be virtually impossible to happen
					// Seriously!
					throw new ApplicationException("The interceptor could not be resolved");
				}

				try
				{
					IMethodInterceptor interceptor = (IMethodInterceptor) handler.Resolve();
					
					interceptors[index++] = interceptor;

					SetOnBehalfAware(interceptor as IOnBehalfAware, model);
				}
				catch(InvalidCastException)
				{
					String message = String.Format(
						"An interceptor registered for {0} doesnt implement " + 
						"the IMethodInterceptor interface", 
						model.Name);

					throw new ApplicationException(message);
				}
			}

			return interceptors;
		}

		protected void SetOnBehalfAware(IOnBehalfAware onBehalfAware, ComponentModel target)
		{
			if (onBehalfAware != null)
			{
				onBehalfAware.SetInterceptedComponentModel(target);
			}
		}

		#region IDeserializationCallback

		public void OnDeserialization(object sender)
		{
			Init();
		}

		#endregion

		private void Init()
		{
			_generator = new ProxyGenerator();
		}
	}
}
