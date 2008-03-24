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
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel.Facilities;

	internal class WcfServiceExtension : IDisposable
	{
		private readonly IKernel kernel;
		private IServiceHostBuilder serviceHostBuilder;

		public WcfServiceExtension(IKernel kernel)
		{
			this.kernel = kernel;

			SetUpServiceHostBuilder();
			WindsorServiceHostFactory.RegisterContainer(kernel);

			kernel.ComponentRegistered += Kernel_ComponentRegistered;
			kernel.ComponentUnregistered += Kernel_ComponentUnregistered;
		}

		private void Kernel_ComponentRegistered(string key, IHandler handler)
		{
			ComponentModel model = handler.ComponentModel;
			WcfServiceModel serviceModel = ResolveServiceModel(model);

			if (serviceModel != null)
			{
				ServiceHost serviceHost = CreateAndOpenServiceHost(serviceModel, model);
				model.ExtendedProperties[WcfConstants.ServiceHostKey] = serviceHost;
			}
		}

		private void Kernel_ComponentUnregistered(string key, IHandler handler)
		{
			ComponentModel model = handler.ComponentModel;
			ServiceHost serviceHost =
				model.ExtendedProperties[WcfConstants.ServiceHostKey] as ServiceHost;

			if (serviceHost != null)
			{
				serviceHost.Close();
			}
		}

		private void SetUpServiceHostBuilder()
		{
			if (!kernel.HasComponent(typeof(IServiceHostBuilder)))
			{
				kernel.AddComponent<WindsorServiceHostBuilder>(typeof(IServiceHostBuilder));
			}
			serviceHostBuilder = kernel.Resolve<IServiceHostBuilder>();
		}

		private WcfServiceModel ResolveServiceModel(ComponentModel model)
		{
			WcfServiceModel serviceModel = null;

			if (model.Implementation.IsClass && !model.Implementation.IsAbstract)
			{
				if (WcfUtils.FindDependency<WcfServiceModel>(
					model.CustomDependencies, out serviceModel))
				{
					ValidateServiceModel(serviceModel, model);
				}
				else if (model.Configuration != null &&
					"true" == model.Configuration.Attributes[WcfConstants.ServiceHostEnabled])
				{
					serviceModel = new WcfServiceModel();
				}
			}

			return serviceModel;
		}

		private ServiceHost CreateAndOpenServiceHost(WcfServiceModel serviceModel,
												     ComponentModel model)
		{
			ServiceHost serviceHost = serviceHostBuilder.Build(model, serviceModel);
			serviceHost.Open();
			return serviceHost;
		}

		private void ValidateServiceModel(WcfServiceModel serviceModel, ComponentModel model)
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
				else if (!model.Service.IsInterface)
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

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
