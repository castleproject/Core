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
	using System.Reflection;

	/// <summary>
	/// Implementation for <see cref="IInterceptedComponent"/>
	/// which holds a component instance, its proxied version and 
	/// an interceptor chain.
	/// </summary>
	public class InterceptedComponent : IInterceptedComponent
	{
		private object m_instance;
		private object m_proxy;
		private IInterceptor m_interceptor = new TailInterceptor();

		/// <summary>
		/// Constructs an InterceptedComponent
		/// </summary>
		/// <param name="instance"></param>
		public InterceptedComponent(object instance)
		{
			AssertUtil.ArgumentNotNull(instance, "instance");
			m_instance = instance;
		}

		/// <summary>
		/// Sets the proxied instance.
		/// </summary>
		/// <param name="proxy"></param>
		public void SetProxiedInstance(object proxy)
		{
			AssertUtil.ArgumentNotNull(proxy, "proxy");
			m_proxy = proxy;
		}

		#region IInterceptedComponent Members

		/// <summary>
		/// Returns the component instance, non-proxied.
		/// </summary>
		public object Instance
		{
			get { return m_instance; }
		}

		/// <summary>
		/// Add the interceptor in the argument as the first interceptor
		/// and set its next to the interceptor that we have. In the first
		/// execution it will be the TailInterceptor.
		/// </summary>
		/// <param name="interceptor"></param>
		public virtual void Add(IInterceptor interceptor)
		{
			AssertUtil.ArgumentNotNull(interceptor, "interceptor");

			interceptor.Next = m_interceptor;
			m_interceptor = interceptor;
		}

		/// <summary>
		/// Returns the proxied instance, that should be available 
		/// to the outside world
		/// </summary>
		public virtual object ProxiedInstance
		{
			get { return m_proxy; }
		}

		/// <summary>
		/// Returns the first interceptor. A call to Process should 
		/// start the execution chain by itself.
		/// </summary>
		public virtual IInterceptor InterceptorChain
		{
			get { return m_interceptor; }
		}

		#endregion

		/// <summary>
		/// Represents the last node in the method execution chain.
		/// It - surprise, surprise - invokes the method on the object.
		/// </summary>
		public sealed class TailInterceptor : IInterceptor
		{
			#region IInterceptor Members

			/// <summary>
			/// Invokes the method on the object instance.
			/// </summary>
			/// <param name="instance">The actual component instance</param>
			/// <param name="method">Method being executed</param>
			/// <param name="arguments">Method arguments</param>
			/// <returns>Should return the method result</returns>
			public object Process(object instance, MethodInfo method, params object[] arguments)
			{
				return method.Invoke(instance, arguments);
			}

			/// <summary>
			/// There is no need to have the property implemented as
			/// there is no next interceptor after the TailInterceptor
			/// </summary>
			public IInterceptor Next
			{
				get { return null; }
				set { }
			}

			#endregion
		}
	}
}