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
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using Castle.Core;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;

	public abstract class AbstractServiceHostBuilder : IWcfEndpointVisitor
	{
		private readonly IKernel kernel;
		private ServiceHost serviceHost;
		private ServiceEndpoint serviceEndpoint;

		protected AbstractServiceHostBuilder(IKernel kernel)
		{
			this.kernel = kernel;
		}

		protected IKernel Kernel
		{
			get { return kernel; }
		}

		protected ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost, IWcfEndpoint endpoint)
		{
			this.serviceHost = serviceHost;
			endpoint.Accept(this);
			new ServiceEndpointBehaviors(serviceEndpoint, kernel)
				.Install(endpoint.Behaviors);
			return serviceEndpoint;
		}

		protected Uri[] GetEffectiveBaseAddresses(IWcfServiceModel serviceModel, Uri[] defaultBaseAddresses)
		{
			List<Uri> baseAddresses = new List<Uri>(serviceModel.BaseAddresses);
			foreach (Uri defaultBaseAddress in defaultBaseAddresses)
			{
				if (!baseAddresses.Exists(delegate(Uri uri)
					{
						return uri.Scheme == defaultBaseAddress.Scheme;
					}))
				{
					baseAddresses.Add(defaultBaseAddress);
				}
			}
			return baseAddresses.ToArray();
		}

		protected virtual void OnOpening(ServiceHost serviceHost, IWcfServiceModel serviceModel,
										 ComponentModel model)
		{
			if (serviceHost is IWcfServiceHost)
			{
				((IWcfServiceHost)serviceHost).OpeningComplete += delegate
				{
					ApplyBehaviors(serviceHost, serviceModel, model);
				};
			}
			else
			{
				ApplyBehaviors(serviceHost, serviceModel, model);
			}
		}

		protected virtual void OnClosed(ServiceHost serviceHost, IWcfServiceModel serviceModel,
										ComponentModel model)
		{
			ReleaseBehaviors(serviceHost, serviceModel, model);
		}

		protected virtual void ApplyBehaviors(ServiceHost serviceHost, IWcfServiceModel serviceModel,
			                                  ComponentModel model)
		{
			serviceHost.Description.Behaviors.Add(
				new WindsorDependencyInjectionServiceBehavior(kernel, model)
				);

			ServiceHostBehaviors behaviors = 
				new ServiceHostBehaviors(serviceHost, kernel)
					.Install(new WcfServiceBehaviors())
					.Install(new WcfEndpointBehaviors(WcfBehaviorScope.Services)
					);

			if (serviceModel != null)
			{
				behaviors.Install(serviceModel.Behaviors);
			}
		}

		protected virtual void ReleaseBehaviors(ServiceHost serviceHost, IWcfServiceModel serviceModel,
											    ComponentModel model)
		{
			WcfUtils.ReleaseBehaviors(kernel, serviceHost);
		}

		#region IWcfEndpointVisitor Members

		void IWcfEndpointVisitor.VisitContractEndpoint(ContractEndpointModel model)
		{
			serviceEndpoint = AddServiceEndpoint(serviceHost, model);
		}

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost,
															 ContractEndpointModel model)
		{
			Binding binding = GetDefaultBinding(serviceHost, string.Empty);
			return serviceHost.AddServiceEndpoint(model.Contract, binding, string.Empty);
		}

		void IWcfEndpointVisitor.VisitServiceEndpoint(ServiceEndpointModel model)
		{
			serviceEndpoint = AddServiceEndpoint(serviceHost, model);
		}

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost,
															 ServiceEndpointModel model)
		{
			serviceHost.Description.Endpoints.Add(model.ServiceEndpoint);
			return model.ServiceEndpoint;
		}

		void IWcfEndpointVisitor.VisitConfigurationEndpoint(ConfigurationEndpointModel model)
		{
			serviceEndpoint = AddServiceEndpoint(serviceHost, model);
		}

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost,
															 ConfigurationEndpointModel model)
		{
			throw new InvalidOperationException("The ServiceEndpoint for a ServiceHost " +
				"cannot be created from an endpoint name.");
		}

		void IWcfEndpointVisitor.VisitBindingEndpoint(BindingEndpointModel model)
		{
			serviceEndpoint = AddServiceEndpoint(serviceHost, model);
		}

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost,
															 BindingEndpointModel model)
		{
			Binding binding = model.Binding ?? GetDefaultBinding(serviceHost, string.Empty);
			return serviceHost.AddServiceEndpoint(model.Contract, binding, string.Empty);
		}

		void IWcfEndpointVisitor.VisitBindingAddressEndpoint(BindingAddressEndpointModel model)
		{
			serviceEndpoint = AddServiceEndpoint(serviceHost, model);
		}

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost,
															 BindingAddressEndpointModel model)
		{
			Binding binding = model.Binding ?? GetDefaultBinding(serviceHost, model.Address);

			if (model.HasViaAddress)
			{
				return serviceHost.AddServiceEndpoint(
					model.Contract, binding, model.Address, model.ViaAddress);
			}
			else
			{
				return serviceHost.AddServiceEndpoint(model.Contract, binding, model.Address);
			}
		}

		protected virtual Binding GetDefaultBinding(ServiceHost serviceHost, string address)
		{
			return null;
		}

		#endregion
	}
}
