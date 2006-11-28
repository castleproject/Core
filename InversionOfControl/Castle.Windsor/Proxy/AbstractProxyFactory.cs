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
	using Castle.Core.Interceptor;
	using Castle.MicroKernel;

	public abstract class AbstractProxyFactory : IProxyFactory
	{
		public abstract object Create(IKernel kernel, object instance, 
		                              ComponentModel model, params object[] constructorArguments);

		public abstract bool RequiresTargetInstance(IKernel kernel, ComponentModel model);

		/// <summary>
		/// Obtains the interceptors associated with the component.
		/// </summary>
		/// <param name="kernel">The kernel instance</param>
		/// <param name="model">The component model</param>
		/// <returns>interceptors array</returns>
		protected IInterceptor[] ObtainInterceptors(IKernel kernel, ComponentModel model)
		{
			IInterceptor[] interceptors = new IInterceptor[model.Interceptors.Count];
			int index = 0;

			foreach(InterceptorReference interceptorRef in model.Interceptors)
			{
				IHandler handler;

				if (interceptorRef.ReferenceType == InterceptorReferenceType.Interface)
				{
					handler = kernel.GetHandler(interceptorRef.ServiceType);
				}
				else
				{
					handler = kernel.GetHandler(interceptorRef.ComponentKey);
				}

				if (handler == null)
				{
					// This shoul be virtually impossible to happen
					// Seriously!
					throw new ApplicationException("The interceptor could not be resolved");
				}

				try
				{
					IInterceptor interceptor = (IInterceptor) handler.Resolve(CreationContext.Empty);
					
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
