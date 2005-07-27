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

	using Castle.MicroKernel;
	
	using Castle.Model;
	using Castle.Model.Interceptor;


	public abstract class AbstractProxyFactory : IProxyFactory
	{
		public AbstractProxyFactory()
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
		public abstract object Create(IKernel kernel, ComponentModel model, params object[] constructorArguments);

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
	}
}
