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

namespace Castle.Windsor.Proxy
{
	using System;

	using Castle.Core;
	using Castle.MicroKernel;
	
	/// <summary>
	/// Create proxies that best suits with the <see cref="ComponentModel.Implementation"/>.
	/// </summary>
	public class ProxySmartFactory : AbstractProxyFactory
	{
		private DefaultProxyFactory _defaultProxyFactory;
		private RealProxyProxyFactory _realProxyFactory;

		public ProxySmartFactory()
		{
			_defaultProxyFactory = new DefaultProxyFactory();
			_realProxyFactory = new RealProxyProxyFactory();
		}

		/// <summary>
		/// Creates a proxied instance of the component.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="model">The <see cref="ComponentModel"/>.</param>
		/// <param name="constructorArguments">The arguments that will be passed to the constructor.</param>
		/// <returns>The proxied instance of the component.</returns>
		/// <remarks>
		/// If the <see cref="ComponentModel.Implementation"/> inherits from <see cref="MarshalByRefObject"/>
		/// <see cref="RealProxyProxyFactory"/> will be used, otherwise the <see cref="DefaultProxyFactory"/> do the job.
		/// </remarks>
		public override object Create(IKernel kernel, ComponentModel model, params object[] constructorArguments)
		{
			if (typeof(MarshalByRefObject).IsAssignableFrom(model.Implementation))
			{
				return  _realProxyFactory.Create(kernel,  model, constructorArguments);
			}
			else
			{
				return _defaultProxyFactory.Create(kernel,  model, constructorArguments);
			}
		}
	}
}
