// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Runtime.InteropServices.ComTypes;
	using Castle.Core;
	using Castle.Core.Interceptor;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Proxy;


	public abstract class AbstractProxyFactory : IProxyFactory
	{
		private readonly IList<IModelInterceptorsSelector> selectors = new List<IModelInterceptorsSelector>();

		public abstract object Create(IKernel kernel, object instance, ComponentModel model, CreationContext context,
		                              params object[] constructorArguments);

		public abstract bool RequiresTargetInstance(IKernel kernel, ComponentModel model);

		public void AddInterceptorSelector(IModelInterceptorsSelector selector)
		{
			selectors.Add(selector);
		}

		public bool ShouldCreateProxy(ComponentModel model)
		{
			foreach(IModelInterceptorsSelector selector in selectors)
			{
				if (selector.HasInterceptors(model))
					return true;
			}
			return model.Interceptors.HasInterceptors;
		}

		/// <summary>
		/// Obtains the interceptors associated with the component.
		/// </summary>
		/// <param name="kernel">The kernel instance</param>
		/// <param name="model">The component model</param>
		/// <param name="context">The creation context</param>
		/// <returns>interceptors array</returns>
		protected IInterceptor[] ObtainInterceptors(IKernel kernel, ComponentModel model, CreationContext context)
		{
			List<IInterceptor> interceptors = new List<IInterceptor>();

			foreach(InterceptorReference interceptorRef in GetInterceptorsFor(model))
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
					IInterceptor interceptor = (IInterceptor) handler.Resolve(context);

					interceptors.Add(interceptor);

					SetOnBehalfAware(interceptor as IOnBehalfAware, model);
				}
				catch(InvalidCastException)
				{
					String message = String.Format(
						"An interceptor registered for {0} doesnt implement " +
						"the IInterceptor interface",
						model.Name);

					throw new ApplicationException(message);
				}
			}

			return interceptors.ToArray();
		}

		protected IEnumerable<InterceptorReference> GetInterceptorsFor(ComponentModel model)
		{
			foreach(IModelInterceptorsSelector selector in selectors)
			{
				InterceptorReference[] interceptors = selector.SelectInterceptors(model);
				if (interceptors != null)
					return interceptors;
			}
			return model.Interceptors;
		}

		protected static void SetOnBehalfAware(IOnBehalfAware onBehalfAware, ComponentModel target)
		{
			if (onBehalfAware != null)
			{
				onBehalfAware.SetInterceptedComponentModel(target);
			}
		}
	}
}