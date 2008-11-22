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
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;

	public abstract class AbstractChannelBuilder : IWcfEndpointVisitor
	{
		private Type contract;
		private readonly IKernel kernel;
		private ChannelCreator channelCreator;

		public AbstractChannelBuilder(IKernel kernel)
		{
			this.kernel = kernel;
		}

		protected IKernel Kernel
		{
			get { return kernel; }
		}

		protected void ConfigureChannelFactory(ChannelFactory channelFactory, IWcfClientModel clientModel)
		{
			BindChannelFactoryAware(channelFactory, kernel);

			ServiceEndpointExtensions extensions =
				new ServiceEndpointExtensions(channelFactory.Endpoint, Kernel)
			.Install(new WcfEndpointExtensions(WcfExtensionScope.Clients));

			if (clientModel != null)
			{
				extensions.Install(clientModel.Extensions);
				extensions.Install(clientModel.Endpoint.Extensions);
			}
		}

		protected ChannelCreator GetEndpointChannelCreator(IWcfEndpoint endpoint)
		{
			return GetEndpointChannelCreator(endpoint, null);
		}

		protected ChannelCreator GetEndpointChannelCreator(IWcfEndpoint endpoint, Type contract)
		{
			this.contract = contract ?? endpoint.Contract;
			endpoint.Accept(this);
			return channelCreator;
		}

		private void BindChannelFactoryAware(ChannelFactory channelFactory, IKernel kernel)
		{
			ICollection<IHandler> channelFactoryAwares = WcfUtils.FindExtensions<IChannelFactoryAware>(
				kernel, WcfExtensionScope.Clients);

			foreach (IHandler handler in channelFactoryAwares)
			{
				IChannelFactoryAware channelFactoryAware = (IChannelFactoryAware)handler.Resolve(CreationContext.Empty);
				WcfUtils.BindChannelFactoryAware(channelFactory, channelFactoryAware, true);
			}
		}

		protected abstract ChannelCreator GetChannel(Type contract);
		protected abstract ChannelCreator GetChannel(Type contract, ServiceEndpoint endpoint);
		protected abstract ChannelCreator GetChannel(Type contract, string configurationName);
		protected abstract ChannelCreator GetChannel(Type contract, Binding binding, string address);
		protected abstract ChannelCreator GetChannel(Type contract, Binding binding, EndpointAddress address);

		#region IWcfEndpointVisitor Members

		void IWcfEndpointVisitor.VisitContractEndpoint(ContractEndpointModel model)
		{
			channelCreator = GetChannel(contract);
		}
        
		void IWcfEndpointVisitor.VisitServiceEndpoint(ServiceEndpointModel model)
		{
			channelCreator = GetChannel(contract, model.ServiceEndpoint);
		}

		void IWcfEndpointVisitor.VisitConfigurationEndpoint(ConfigurationEndpointModel model)
		{
			channelCreator = GetChannel(contract, model.EndpointName);
		}

		void IWcfEndpointVisitor.VisitBindingEndpoint(BindingEndpointModel model)
		{
			channelCreator = GetChannel(contract, model.Binding, string.Empty);
		}

		void IWcfEndpointVisitor.VisitBindingAddressEndpoint(BindingAddressEndpointModel model)
		{
			if (model.HasViaAddress)
			{
				EndpointAddress address = model.EndpointAddress ?? new EndpointAddress(model.Address);
				ContractDescription description = ContractDescription.GetContract(contract);
				ServiceEndpoint endpoint = new ServiceEndpoint(description, model.Binding, address);
				endpoint.Behaviors.Add(new ClientViaBehavior(model.ViaAddress));
				channelCreator = GetChannel(contract, endpoint);
			}
			else
			{
				if (model.EndpointAddress != null)
				{
					channelCreator = GetChannel(contract, model.Binding, model.EndpointAddress);
				}
				else
				{
					channelCreator = GetChannel(contract, model.Binding, model.Address);
				}
			}
		}

		#endregion
	}
}
