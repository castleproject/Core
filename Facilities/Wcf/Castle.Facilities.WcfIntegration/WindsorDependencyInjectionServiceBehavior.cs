using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Castle.Windsor;

namespace Castle.Facilities.WcfIntegration
{
	public class WindsorDependencyInjectionServiceBehavior : IServiceBehavior
	{
		private readonly IWindsorContainer container;

		public WindsorDependencyInjectionServiceBehavior(IWindsorContainer container)
		{
			this.container = container;
		}

		///<summary>
		///Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
		///</summary>
		///<param name="serviceHostBase">The service host that is currently being constructed.</param>
		///<param name="serviceDescription">The service description.</param>
		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

		///<summary>
		///Provides the ability to pass custom data to binding elements to support the contract implementation.
		///</summary>
		///<param name="serviceHostBase">The host of the service.</param>
		///<param name="bindingParameters">Custom objects to which binding elements have access.</param>
		///<param name="serviceDescription">The service description of the service.</param>
		///<param name="endpoints">The service endpoints.</param>
		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
		                                 Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			Dictionary<string, Type> contractNameToContractType = new Dictionary<string, Type>();
			foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints)
			{
				contractNameToContractType[endpoint.Contract.Name] = endpoint.Contract.ContractType;
			}
			foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
			{
				ChannelDispatcher cd = cdb as ChannelDispatcher;
				if (cd != null)
				{
					foreach (EndpointDispatcher ed in cd.Endpoints)
					{
						if (contractNameToContractType.ContainsKey(ed.ContractName))
						{
							ed.DispatchRuntime.InstanceProvider =
								new WindsorInstanceProvider(container,
									contractNameToContractType[ed.ContractName]
									);
						}
					}
				}
			}
		}
	}
}