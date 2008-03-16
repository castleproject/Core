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
	using Castle.Facilities.WcfIntegration.Internal;

	public class WcfFacility : AbstractFacility
	{
		private readonly WcfClientModel[] clientModels;

		public WcfFacility()
		{
		}

		public WcfFacility(params WcfClientModel[] clientModels)
		{
			foreach (WcfClientModel clientModel in clientModels)
			{
				ValidateClientModel(clientModel, null);
			}
			this.clientModels = clientModels;
		}

		protected override void Init()
		{
			Kernel.ComponentRegistered += Kernel_ComponentRegistered;
			Kernel.ComponentUnregistered += Kernel_ComponentUnregistered;
			Kernel.ComponentModelCreated += Kernel_ComponentModelCreated;

			if (clientModels != null && clientModels.Length > 0)
			{
				Kernel.Resolver.AddSubResolver(new WcfClientResolver(clientModels));
			}
		}

		private void Kernel_ComponentModelCreated(ComponentModel componentModel)
		{
			WcfClientModel clientModel = ResolveClientModel(componentModel);

			if (clientModel != null)
			{
				componentModel.ExtendedProperties[WcfConstants.ClientModelKey] = clientModel;
				componentModel.CustomComponentActivator = typeof(WcfClientActivator);
				componentModel.LifestyleType = LifestyleType.Transient;
			}
		}

		private void Kernel_ComponentRegistered(string key, IHandler handler)
		{
			ComponentModel componentModel = handler.ComponentModel;
			WcfServiceModel serviceModel = ResolveServiceModel(componentModel);

			if (serviceModel != null)
			{
				WindsorServiceHost serviceHost = CreateAndOpenServiceHost(serviceModel, componentModel);
				componentModel.ExtendedProperties[WcfConstants.ServiceHostKey] = serviceHost;
			}
		}

		private void Kernel_ComponentUnregistered(string key, IHandler handler)
		{
			ComponentModel componentModel = handler.ComponentModel;
			ServiceHost serviceHost =
				componentModel.ExtendedProperties[WcfConstants.ServiceHostKey] as ServiceHost;

			if (serviceHost != null)
			{
				serviceHost.Close();
			}
		}

		private WcfClientModel ResolveClientModel(ComponentModel componentModel)
		{
			WcfClientModel clientModel = null;

			if (componentModel.Service.IsInterface)
			{
				if (WcfUtils.FindDependency<WcfClientModel>(
					componentModel.CustomDependencies, out clientModel))
				{
					ValidateClientModel(clientModel, componentModel);
				}
			}

			return clientModel;
		}

		private WcfServiceModel ResolveServiceModel(ComponentModel componentModel)
		{
			WcfServiceModel serviceModel = null;

			if (componentModel.Implementation.IsClass && 
				!componentModel.Implementation.IsAbstract)
			{
				if (WcfUtils.FindDependency<WcfServiceModel>(
					componentModel.CustomDependencies, out serviceModel))
				{
					ValidateServiceModel(serviceModel, componentModel);
				}
			} 
			
			return serviceModel;
		}

		private WindsorServiceHost CreateAndOpenServiceHost(WcfServiceModel serviceModel,
															ComponentModel componentModel)
		{
			WindsorServiceHost serviceHost = new WindsorServiceHost(
				Kernel, componentModel.Implementation, serviceModel.GetBaseAddressesUris());

			ServiceHostBuilder serviceHostBuilder = new ServiceHostBuilder(); 
			foreach (IWcfEndpoint endpoint in serviceModel.Endpoints)
			{
				serviceHostBuilder.AddServiceEndpoint(serviceHost, endpoint);
			}

			serviceHost.Open();
			return serviceHost;
		}

		private void ValidateClientModel(WcfClientModel clientModel, ComponentModel componentModel)
		{
			Type contract;

			if (componentModel != null)
			{
				contract = componentModel.Service;
			}
			else if (clientModel.Contract != null)
			{
				contract = clientModel.Contract;
			}
			else
			{
				throw new FacilityException(
					"The client endpoint does not specify a contract.");
			}

			if ((componentModel != null) && (clientModel.Contract != null) &&
				(clientModel.Contract != componentModel.Service))
			{
				throw new FacilityException("The client endpoint contract " +
					clientModel.Contract.FullName + " does not match the expected contaxt" +
					componentModel.Service.FullName + ".");
			}

			if (clientModel.Endpoint == null)
			{
				throw new FacilityException(
					"The client model requires an endpoint.");
			}
		}

		private void ValidateServiceModel(WcfServiceModel serviceModel, ComponentModel componentModel)
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
				else if (!componentModel.Service.IsInterface)
				{
					throw new FacilityException(
						"No service endpoint contract can be implied from the componnt.");
				}
				else
				{
					endpoint.Contract = componentModel.Service;
				}
			}
		}
	}
}

