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
	using System.ServiceModel.Channels;
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.Proxy;
	using Castle.Facilities.WcfIntegration.Internal;
	using System.ServiceModel;

	internal class WcfClientExtension : IDisposable
	{
		private readonly IKernel kernel;
		private IClientChannelBuilder channelBuilder;
 
		public WcfClientExtension(IKernel kernel)
		{
			this.kernel = kernel;

			SetUpClientChannelBuilder();

			kernel.AddComponent<WcfManagedChannelInterceptor>();
			kernel.ComponentModelCreated += Kernel_ComponentModelCreated;
		}

		private void Kernel_ComponentModelCreated(ComponentModel model)
		{
			WcfClientModel clientModel = ResolveClientModel(model);

			if (clientModel != null)
			{
				model.CustomComponentActivator = typeof(WcfClientActivator);
				model.ExtendedProperties[WcfConstants.ClientModelKey] = clientModel;
				model.LifecycleSteps.Add(LifecycleStepType.Decommission, WcfChannelCleanupConcern.Instance);
				InstallWcfManagedChannelInterceptor(model);
			}
		}

		private void SetUpClientChannelBuilder()
		{
			if (!kernel.HasComponent(typeof(IClientChannelBuilder)))
			{
				kernel.AddComponent<DefaultChannelBuilder>(typeof(IClientChannelBuilder));
			}
			channelBuilder = kernel.Resolve<IClientChannelBuilder>();
		}

		private void InstallWcfManagedChannelInterceptor(ComponentModel model)
		{
			model.Dependencies.Add(new DependencyModel(DependencyType.Service, null,
													   typeof(WcfManagedChannelInterceptor), false));
			model.Interceptors.Add(new InterceptorReference(typeof(WcfManagedChannelInterceptor)));
			ProxyOptions options = ProxyUtil.ObtainProxyOptions(model, true);
			options.AllowChangeTarget = true;
		}

		private WcfClientModel ResolveClientModel(ComponentModel model)
		{
			WcfClientModel clientModel = null;

			if (model.Service.IsInterface)
			{
				if (WcfUtils.FindDependency<WcfClientModel>(
					model.CustomDependencies, out clientModel))
				{
					ValidateClientModel(clientModel, model);
				}
				else if (model.Configuration != null)
				{
					string endpointConfiguration =
						 model.Configuration.Attributes[WcfConstants.EndpointConfiguration];

					if (!string.IsNullOrEmpty(endpointConfiguration))
					{
						clientModel = new WcfClientModel(
							WcfEndpoint.FromConfiguration(endpointConfiguration));
					}
				}
			}

			return clientModel;
		}

		private void ValidateClientModel(WcfClientModel clientModel, ComponentModel model)
		{
			Type contract;

			if (model != null)
			{
				contract = model.Service;
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

			if ((model != null) && (clientModel.Contract != null) &&
				(clientModel.Contract != model.Service))
			{
				throw new FacilityException("The client endpoint contract " +
					clientModel.Contract.FullName + " does not match the expected contaxt" +
					model.Service.FullName + ".");
			}

			if (clientModel.Endpoint == null)
			{
				throw new FacilityException("The client model requires an endpoint.");
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
