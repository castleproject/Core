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
 
		public WcfClientExtension(IKernel kernel)
		{
			this.kernel = kernel;

			SetUpDefaultClientChannelBuilder();

			kernel.AddComponent<WcfManagedChannelInterceptor>();
			kernel.ComponentModelCreated += Kernel_ComponentModelCreated;
		}

		private void Kernel_ComponentModelCreated(ComponentModel model)
		{
			IWcfClientModel clientModel = ResolveClientModel(model);

			if (clientModel != null)
			{
				model.CustomComponentActivator = typeof(WcfClientActivator);
				model.ExtendedProperties[WcfConstants.ClientModelKey] = clientModel;
				model.LifecycleSteps.Add(LifecycleStepType.Decommission, WcfChannelCleanupConcern.Instance);
				InstallManagedChannelInterceptor(model);
			}
		}

		private void SetUpDefaultClientChannelBuilder()
		{
			if (!kernel.HasComponent(typeof(IClientChannelBuilder<WcfClientModel>)))
			{
				kernel.AddComponent<DefaultChannelBuilder>(typeof(IClientChannelBuilder<WcfClientModel>));
			}
		}

		private void InstallManagedChannelInterceptor(ComponentModel model)
		{
			model.Dependencies.Add(new DependencyModel(DependencyType.Service, null,
													   typeof(WcfManagedChannelInterceptor), false));
			model.Interceptors.Add(new InterceptorReference(typeof(WcfManagedChannelInterceptor)));
			ProxyOptions options = ProxyUtil.ObtainProxyOptions(model, true);
			options.AllowChangeTarget = true;
		}

		private IWcfClientModel ResolveClientModel(ComponentModel model)
		{
			IWcfClientModel clientModel = null;

			if (model.Service.IsInterface)
			{
				if (!WcfUtils.FindDependency<IWcfClientModel>(
						model.CustomDependencies, out clientModel) &&
                    model.Configuration != null)
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

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
