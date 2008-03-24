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
	using System.ServiceModel.Description;
	using Castle.Core;

	public abstract class AbstractServiceHostBuilder : IServiceHostBuilder, IWcfEndpointVisitor
	{
		private ServiceHost serviceHost;
		private ServiceEndpoint serviceEndpoint;

		#region IServiceHostBuilder Members

		/// <summary>
		/// Builds a new <see cref="ServiceHost"/> for the <see cref="ComponentModel"/>.
		/// </summary>
		/// <param name="model">The component model.</param>
		/// <param name="serviceModel">The service model.</param>
		/// <returns>The correpsonding service host.</returns>
		public ServiceHost Build(ComponentModel model, WcfServiceModel serviceModel)
		{
			ServiceHost serviceHost = CreateServiceHost(model, serviceModel);

			foreach (IWcfEndpoint endpoint in serviceModel.Endpoints)
			{
				AddServiceEndpoint(serviceHost, endpoint);
			}

			return serviceHost;
		}

		#endregion

		protected abstract ServiceHost CreateServiceHost(ComponentModel model,
												         WcfServiceModel serviceModel);

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost, 
			                                                 IWcfEndpoint endpoint)
		{
			this.serviceHost = serviceHost;
			endpoint.Accept(this);
			return serviceEndpoint;
		}

		#region IWcfEndpointVisitor Members

		void IWcfEndpointVisitor.VisitServiceEndpointModel(ServiceEndpointModel model)
		{
			serviceHost.Description.Endpoints.Add(model.ServiceEndpoint);
			serviceEndpoint = model.ServiceEndpoint;
		}

		void IWcfEndpointVisitor.VisitConfigurationEndpointModel(ConfigurationEndpointModel model)
		{
			throw new InvalidOperationException("The ServiceEndpoint for a ServiceHost " +
				"cannot be created from an endpoint name.");
		}

		void IWcfEndpointVisitor.VisitBindingEndpointModel(BindingEndpointModel model)
		{
			serviceEndpoint = serviceHost.AddServiceEndpoint(
				model.Contract, model.Binding, string.Empty);
		}

		void IWcfEndpointVisitor.VisitBindingAddressEndpointModel(BindingAddressEndpointModel model)
		{
			if (model.HasViaAddress)
			{
				serviceEndpoint = serviceHost.AddServiceEndpoint(
					model.Contract, model.Binding, model.Address, model.ViaAddress);
			}
			else
			{
				serviceEndpoint = serviceHost.AddServiceEndpoint(
					model.Contract, model.Binding, model.Address);
			}
		}

		#endregion
	}
}
