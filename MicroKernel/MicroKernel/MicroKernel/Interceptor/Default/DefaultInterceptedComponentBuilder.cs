// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Interceptor.Default
{
	using System;
	using System.Reflection;

	using Apache.Avalon.DynamicProxy;

	/// <summary>
	/// Summary description for DefaultInterceptorManager.
	/// </summary>
	public class DefaultInterceptedComponentBuilder : IInterceptedComponentBuilder
	{
		#region IInterceptorManager Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="service"></param>
		/// <returns></returns>
		public IInterceptedComponent CreateInterceptedComponent(object instance, Type service)
		{
			AssertUtil.ArgumentNotNull(instance, "instance");
			AssertUtil.ArgumentNotNull(service, "service");
			AssertUtil.ArgumentMustBeInterface(service, "service");

			InterceptedComponent intercepted = new InterceptedComponent(instance);

			object proxy = ProxyGenerator.CreateProxy(service, new InterceptorInvocationHandler(intercepted) );
			intercepted.SetProxiedInstance(proxy);

			return intercepted;
		}

		#endregion

		/// <summary>
		/// Implementation of <see cref="DynamicProxy.IInvocationHandler"/>
		/// which delegates the execution to the interceptor chain.
		/// </summary>
		public sealed class InterceptorInvocationHandler : IInvocationHandler
		{
			private InterceptedComponent m_interceptedComponent;

			/// <summary>
			/// Constructs an InterceptorInvocationHandler
			/// </summary>
			/// <param name="interceptedComponent"></param>
			public InterceptorInvocationHandler(InterceptedComponent interceptedComponent)
			{
				AssertUtil.ArgumentNotNull( interceptedComponent, "interceptedComponent" );
				m_interceptedComponent = interceptedComponent;
			}

			#region IInvocationHandler Members

			/// <summary>
			/// Pending.
			/// </summary>
			/// <param name="proxy"></param>
			/// <param name="method"></param>
			/// <param name="arguments"></param>
			/// <returns></returns>
			public object Invoke(object proxy, MethodInfo method, params object[] arguments)
			{
				object instance = m_interceptedComponent.Instance;
				return m_interceptedComponent.InterceptorChain.Process( instance, method, arguments );
			}

			#endregion
		}
	}
}