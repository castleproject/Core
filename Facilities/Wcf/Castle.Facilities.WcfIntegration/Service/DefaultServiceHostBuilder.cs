// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration
{
	using System;
	using System.ServiceModel;
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// The default implementation of <see cref="IServiceHostBuilder{M}"/>.
	/// </summary>
	public class DefaultServiceHostBuilder : AbstractServiceHostBuilder<WcfServiceModel>
	{
		private readonly IKernel kernel;

		/// <summary>
		/// Constructs a new <see cref="DefaultServiceHostBuilder"/>.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		public DefaultServiceHostBuilder(IKernel kernel)
		{
			this.kernel = kernel;
		}

		#region AbstractServiceHostBuilder Members

		protected override ServiceHost CreateServiceHost(ComponentModel model, WcfServiceModel serviceModel)
		{
			ServiceHost serviceHost = new WindsorServiceHost(kernel, model, GetBaseAddressArray(serviceModel));
			ConfigureServiceHost(serviceHost, serviceModel);
			return serviceHost;
		}

		protected override ServiceHost CreateServiceHost(Type serviceType, WcfServiceModel serviceModel)
		{
			ServiceHost serviceHost = new WindsorServiceHost(kernel, serviceType, GetBaseAddressArray(serviceModel));
			ConfigureServiceHost(serviceHost, serviceModel);
			return serviceHost;
		}

		protected override void ValidateServiceModel(ComponentModel model, WcfServiceModel serviceModel)
		{
			foreach (IWcfEndpoint endpoint in serviceModel.Endpoints)
			{
				Type contract = endpoint.Contract;

				if (contract != null)
				{
					if (!contract.IsInterface)
					{
						throw new FacilityException("The service endpoint contract " +
							contract.FullName + " does not represent an interface.");
					}
				}
				else if (model == null || !model.Service.IsInterface)
				{
					throw new FacilityException(
						"No service endpoint contract can be implied from the componnt.");
				}
				else
				{
					endpoint.Contract = model.Service;
				}
			}
		}

		#endregion

		protected virtual void ConfigureServiceHost(ServiceHost serviceHost, WcfServiceModel serviceModel)
		{
			foreach (IWcfEndpoint endpoint in serviceModel.Endpoints)
			{
				AddServiceEndpoint(serviceHost, endpoint);
			}

			if (!serviceModel.IsHosted)
			{
				serviceHost.Open();
			}
		}

		private Uri[] GetBaseAddressArray(WcfServiceModel serviceModel)
		{
			Uri[] baseAddresses = new Uri[serviceModel.BaseAddresses.Count];
			serviceModel.BaseAddresses.CopyTo(baseAddresses, 0);
			return baseAddresses;
		}
	}
}
