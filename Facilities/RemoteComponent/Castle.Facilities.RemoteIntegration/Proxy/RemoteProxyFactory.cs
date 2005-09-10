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

namespace Castle.Facilities.RemoteIntegration
{
	using System;
	using Castle.Model;
	using Castle.MicroKernel;
	using Castle.Windsor.Proxy;

	public class RemoteProxyFactory : RealProxyProxyFactory
	{
		private IProxyFactory _defaultProxyFactory;

		public RemoteProxyFactory(IProxyFactory defaultProxyFactory)
		{
			_defaultProxyFactory = defaultProxyFactory;
		}

		public override object Create(IKernel kernel, ComponentModel model, params object[] constructorArguments)
		{
			//TODO:When I used this proxy, the remote method will called many times

			if (typeof(MarshalByRefObject).IsAssignableFrom(model.Implementation))
			{
				object target = Activator.CreateInstance( model.Implementation, constructorArguments );

				if (!(target is MarshalByRefObject))
				{
					throw new ApplicationException("RemoteProxyFactory can only proxy types that extend MarshalByRefObject");
				}

				ComponentRealProxy proxy = new ComponentRealProxy( (MarshalByRefObject) target, 
					model.Implementation, ObtainInterceptors(kernel, model));

				return proxy.GetTransparentProxy();
			}
			else
			{
				return _defaultProxyFactory.Create(kernel,  model, constructorArguments);
			}
		}
	}
}
