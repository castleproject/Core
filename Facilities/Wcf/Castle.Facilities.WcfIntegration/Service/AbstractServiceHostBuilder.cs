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

	public abstract class AbstractServiceHostBuilder
	{
		private readonly IKernel kernel;

		protected AbstractServiceHostBuilder(IKernel kernel)
		{
			this.kernel = kernel;
		}

		protected IKernel Kernel
		{
			get { return kernel; }
		}

		public WcfServiceExtension Services { get; set; }

		protected void ConfigureServiceHost(ServiceHost serviceHost, IWcfServiceModel serviceModel,
											ComponentModel model)
		{
			serviceHost.Description.Behaviors.Add(
				new WindsorDependencyInjectionServiceBehavior(kernel, model)
				);

			var burden = new WcfBurden(kernel);
			var contracts = new HashSet<ContractDescription>();
			Dictionary<IWcfEndpoint, ServiceEndpoint> endpoints = null;

			if (serviceModel != null && serviceModel.Endpoints.Count > 0)
			{
				endpoints = new Dictionary<IWcfEndpoint, ServiceEndpoint>();
				var builder = new ServiceEndpointBuilder(this, serviceHost);

				foreach (var endpoint in serviceModel.Endpoints)
				{
					endpoints.Add(endpoint, builder.AddServiceEndpoint(endpoint));
				}
			}

			var extensions = new ServiceHostExtensions(serviceHost, kernel)
				.Install(burden, new WcfServiceExtensions());

			if (serviceModel != null)
			{
				extensions.Install(serviceModel.Extensions, burden);
			}

			extensions.Install(burden, new WcfEndpointExtensions(WcfExtensionScope.Services));

			if (endpoints != null)
			{
				foreach (var endpoint in endpoints)
				{
					var addContract = contracts.Add(endpoint.Value.Contract);
					new ServiceEndpointExtensions(endpoint.Value, addContract, kernel)
						.Install(endpoint.Key.Extensions, burden);
				}
			}

			if (serviceHost is IWcfServiceHost)
			{
				var wcfServiceHost = (IWcfServiceHost)serviceHost;
				wcfServiceHost.EndpointCreated += delegate(object source, EndpointCreatedArgs e)
				{
					var addContract = contracts.Add(e.Endpoint.Contract);
					var endpointExtensions = new ServiceEndpointExtensions(e.Endpoint, addContract, kernel)
						.Install(burden, new WcfEndpointExtensions(WcfExtensionScope.Services));

					if (serviceModel != null)
					{
						endpointExtensions.Install(serviceModel.Extensions, burden);
					}
				};
			}

			serviceHost.Extensions.Add(new WcfBurdenExtension<ServiceHostBase>(burden));
		}

		protected Uri[] GetEffectiveBaseAddresses(IWcfServiceModel serviceModel, Uri[] defaultBaseAddresses)
		{
			var baseAddresses = new List<Uri>(serviceModel.BaseAddresses);
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

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost,
															 ContractEndpointModel model)
		{
			var binding = GetEffectiveBinding(null, serviceHost, string.Empty);
			return serviceHost.AddServiceEndpoint(model.Contract, binding, string.Empty);
		}

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost,
															 ServiceEndpointModel model)
		{
			serviceHost.Description.Endpoints.Add(model.ServiceEndpoint);
			return model.ServiceEndpoint;
		}

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost,
															 ConfigurationEndpointModel model)
		{
			throw new InvalidOperationException("The ServiceEndpoint for a ServiceHost " +
				"cannot be created from an endpoint name.");
		}

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost,
															 BindingEndpointModel model)
		{
			var binding = GetEffectiveBinding(model.Binding, serviceHost, string.Empty);
			return serviceHost.AddServiceEndpoint(model.Contract, binding, string.Empty);
		}

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost,
															 BindingAddressEndpointModel model)
		{
			var binding = GetEffectiveBinding(model.Binding, serviceHost, model.Address);

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

		private Binding GetEffectiveBinding(Binding binding, ServiceHost serviceHost, string address)
		{
			if (binding == null && Services != null)
			{
				binding = Services.DefaultBinding;
			}
			return binding ?? GetDefaultBinding(serviceHost, address);
		}

		#region Nested Class: ServiceEndpointBuilder

		class ServiceEndpointBuilder : IWcfEndpointVisitor
		{
			private readonly AbstractServiceHostBuilder builder;
			private readonly ServiceHost serviceHost;
			private ServiceEndpoint serviceEndpoint;

			public ServiceEndpointBuilder(AbstractServiceHostBuilder builder, ServiceHost serviceHost)
			{
				this.builder = builder;
				this.serviceHost = serviceHost;
			}

			public ServiceEndpoint AddServiceEndpoint(IWcfEndpoint endpoint)
			{
				endpoint.Accept(this);
				return serviceEndpoint;				
			}

			void IWcfEndpointVisitor.VisitContractEndpoint(ContractEndpointModel model)
			{
				serviceEndpoint = builder.AddServiceEndpoint(serviceHost, model);
			}

			void IWcfEndpointVisitor.VisitServiceEndpoint(ServiceEndpointModel model)
			{
				serviceEndpoint = builder.AddServiceEndpoint(serviceHost, model);
			}

			void IWcfEndpointVisitor.VisitConfigurationEndpoint(ConfigurationEndpointModel model)
			{
				serviceEndpoint = builder.AddServiceEndpoint(serviceHost, model);
			}

			void IWcfEndpointVisitor.VisitBindingAddressEndpoint(BindingAddressEndpointModel model)
			{
				serviceEndpoint = builder.AddServiceEndpoint(serviceHost, model);
			}

			void IWcfEndpointVisitor.VisitBindingEndpoint(BindingEndpointModel model)
			{
				serviceEndpoint = builder.AddServiceEndpoint(serviceHost, model);
			}
		}

		#endregion
	}
}
