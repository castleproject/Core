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

		public object CreateClassProxy(Type baseClass, IInterceptor interceptor)
		{
			return CreateClassProxy(baseClass, new IInterceptor[] {interceptor} );
		}

		public object CreateClassProxy(Type baseClass, IInterceptor[] interceptors)
		{
			Type type = CreateClassProxy(baseClass, interceptors, ProxyGenerationOptions.Default);

			return Activator.CreateInstance(type, new object[] { interceptors });
		}

		protected Type CreateClassProxy(Type baseClass, IInterceptor[] interceptors, ProxyGenerationOptions options)
		{
			return ProxyBuilder.CreateClassProxy(baseClass, options);
		}
	}
}
