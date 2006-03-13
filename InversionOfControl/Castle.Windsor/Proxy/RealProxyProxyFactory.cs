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

	using Castle.MicroKernel;
	using Castle.Model;


	public class RealProxyProxyFactory : AbstractProxyFactory
	{
		public RealProxyProxyFactory()
		{
		}

		/// <summary>
		/// Implementors must create a proxy based on 
		/// the information exposed by ComponentModel
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		/// <param name="constructorArguments"></param>
		/// <returns></returns>
		public override object Create(IKernel kernel, ComponentModel model, params object[] constructorArguments)
		{
			object target = Activator.CreateInstance(model.Implementation, constructorArguments);

			if (!(target is MarshalByRefObject))
			{
				throw new ApplicationException("RealProxyProxyFactory can only proxy types that extend MarshalByRefObject");
			}

			ComponentRealProxy proxy = new ComponentRealProxy( (MarshalByRefObject) target, 
				model.Implementation, ObtainInterceptors(kernel, model));

			return proxy.GetTransparentProxy();
		}
	}
}
