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

namespace Castle.Windsor.Proxy
{
	using System;

	using Castle.Model;
	using Castle.MicroKernel;
	using Castle.DynamicProxy;
	using Castle.Model.Interceptor;

	/// <summary>
	/// This implementation of <see cref="IProxyFactory"/> relies 
	/// on DynamicProxy to deliver proxies capabilies.
	/// </summary>
	/// <remarks>
	/// Note that only virtual methods can be intercepted in a 
	/// concrete class. 
	/// </remarks>
	public class DefaultProxyFactory : IProxyFactory
	{
		private ProxyGenerator _generator;

		public DefaultProxyFactory()
		{
			_generator = new ProxyGenerator();
		}

		public object Create(ComponentModel mode, params object[] constructorArguments)
		{
			IMethodInterceptor[] interceptors = mode.Interceptors.ToArray();
			IInterceptor interceptorChain = new InterceptorChain(interceptors);

			// This is a hack to avoid unnecessary object creations
			// We supply our contracts (Interceptor and Invocation)
			// and the implementation for Invocation dispatchers
			// DynamicProxy should be able to use them as long as the 
			// signatures match
			GeneratorContext context = new GeneratorContext();
			context.Interceptor = typeof(IMethodInterceptor);
			context.Invocation = typeof(IMethodInvocation);
			context.SameClassInvocation = typeof(DefaultMethodInvocation);

			return _generator.CreateCustomClassProxy(mode.Implementation, 
				interceptorChain, context, constructorArguments);
		}
	}
}
