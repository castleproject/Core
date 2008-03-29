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
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using System.ServiceModel.Channels;

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

		protected Uri[] GetBaseAddressArray(IWcfServiceModel serviceModel)
		{
			Uri[] baseAddresses = new Uri[serviceModel.BaseAddresses.Count];
			serviceModel.BaseAddresses.CopyTo(baseAddresses, 0);
			return baseAddresses;
		}

		protected ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost, IWcfEndpoint endpoint)
		{
			this.serviceHost = serviceHost;
			endpoint.Accept(this);
			return serviceEndpoint;
		}

		protected virtual void OnOpening(ServiceHost serviceHost, ComponentModel model)
		{
			serviceHost.Description.Behaviors.Add(new WindsorDependencyInjectionServiceBehavior(Kernel, model));

			IHandler[] serviceBehaviorHandlers = Kernel.GetAssignableHandlers(typeof(IServiceBehavior));
			foreach (IHandler handler in serviceBehaviorHandlers)
			{
				if (handler.ComponentModel.Implementation == typeof(ServiceDebugBehavior))
				{
					serviceHost.Description.Behaviors.Remove<ServiceDebugBehavior>();
				}
				serviceHost.Description.Behaviors.Add((IServiceBehavior)handler.Resolve(CreationContext.Empty));
			}

			IHandler[] endPointBehaviors = kernel.GetAssignableHandlers(typeof(IEndpointBehavior));
			IHandler[] operationBehaviors = kernel.GetAssignableHandlers(typeof(IOperationBehavior));

			foreach (ServiceEndpoint endpoint in serviceHost.Description.Endpoints)
			{
				foreach (IHandler handler in endPointBehaviors)
				{
					endpoint.Behaviors.Add((IEndpointBehavior)handler.Resolve(CreationContext.Empty));
				}

				foreach (OperationDescription operation in endpoint.Contract.Operations)
				{
					foreach (IHandler operationHandler in operationBehaviors)
					{
						operation.Behaviors.Add((IOperationBehavior)operationHandler.Resolve(CreationContext.Empty));
					}
				}
			}
		}

		#region IWcfEndpointVisitor Members

		void IWcfEndpointVisitor.VisitServiceEndpointModel(ServiceEndpointModel model)
		{
			serviceEndpoint = AddServiceEndpoint(serviceHost, model);
		}

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost, 
			                                                 ServiceEndpointModel model)
		{
			serviceHost.Description.Endpoints.Add(model.ServiceEndpoint);
			return model.ServiceEndpoint;
		}

		void IWcfEndpointVisitor.VisitConfigurationEndpointModel(ConfigurationEndpointModel model)
		{
			serviceEndpoint = AddServiceEndpoint(serviceHost, model);
		}

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost,
			                                                 ConfigurationEndpointModel model)
		{
			throw new InvalidOperationException("The ServiceEndpoint for a ServiceHost " +
				"cannot be created from an endpoint name.");
		}

		void IWcfEndpointVisitor.VisitBindingEndpointModel(BindingEndpointModel model)
		{
			serviceEndpoint = AddServiceEndpoint(serviceHost, model);
		}

		protected virtual ServiceEndpoint AddServiceEndpoint(ServiceHost serviceHost,
			                                                 BindingEndpointModel model)
		{
			Binding binding = model.Binding ?? GetDefaultBinding(serviceHost, string.Empty);
			return serviceHost.AddServiceEndpoint(model.Contract, binding, string.Empty);
		}

		void IWcfEndpointVisitor.VisitBindingAddressEndpointModel(BindingAddressEndpointModel model)
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

	public abstract class AbstractServiceHostBuilder<M> : AbstractServiceHostBuilder, IServiceHostBuilder<M>
			where M : IWcfServiceModel
	{
		private M serviceModel;

		protected AbstractServiceHostBuilder(IKernel kernel)
			: base(kernel)
		{
		}

		protected M ServiceModel
		{
			get { return serviceModel; }
		}

		#region IServiceHostBuilder Members

		/// <summary>
		/// Builds a new <see cref="ServiceHost"/> for the <see cref="ComponentModel"/>.
		/// </summary>
		/// <param name="model">The component model.</param>
		/// <param name="serviceModel">The service model.</param>
		/// <returns>The correpsonding service host.</returns>
		public ServiceHost Build(ComponentModel model, M serviceModel)
		{
			this.serviceModel = serviceModel;
			ValidateServiceModelInternal(model, serviceModel);
			ServiceHost serviceHost = CreateServiceHost(model, serviceModel);
			ConfigureServiceHost(serviceHost, serviceModel);
			OpenServiceHost(serviceHost, serviceModel, model);
			return serviceHost;
		}

		/// <summary>
		///  Builds a service host.
		/// </summary>
		/// <param name="serviceType">The service type.</param>
		/// <param name="serviceModel">The service model.</param>
		/// <returns>The service host.</returns>
		public ServiceHost Build(Type serviceType, M serviceModel)
		{
			this.serviceModel = serviceModel;
			ValidateServiceModelInternal(null, serviceModel);
			ServiceHost serviceHost = CreateServiceHost(serviceType, serviceModel);
			ConfigureServiceHost(serviceHost, serviceModel);
			OpenServiceHost(serviceHost, serviceModel, null);
			return serviceHost;
		}

		#endregion

		protected virtual void ConfigureServiceHost(ServiceHost serviceHost, M serviceModel)
		{
			foreach (IWcfEndpoint endpoint in serviceModel.Endpoints)
			{
				AddServiceEndpoint(serviceHost, endpoint);
			}
		}

		private void OpenServiceHost(ServiceHost serviceHost, M serviceModel, ComponentModel model)
		{
			serviceHost.Opening += delegate { OnOpening(serviceHost, model); };
			
			if (!serviceModel.IsHosted)
			{
				serviceHost.Open();
			}
		}

		private void ValidateServiceModelInternal(ComponentModel model, M serviceModel)
		{
			ValidateServiceModel(model, serviceModel);

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
						"No service endpoint contract can be implied from the component.");
				}
				else
				{
					endpoint.Contract = model.Service;
				}
			}
		}

		protected virtual void ValidateServiceModel(ComponentModel model, M serviceModel)
		{
		}

		protected abstract ServiceHost CreateServiceHost(ComponentModel model, M serviceModel);

		protected abstract ServiceHost CreateServiceHost(Type serviceType, M serviceModel);
	}
}
