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
	using System.ServiceModel.Channels;
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

		protected abstract ChannelCreator GetChannelCreator(Type contract, ServiceEndpoint endpoint);
		protected abstract ChannelCreator GetChannelCreator(Type contract, string configurationName);
		protected abstract ChannelCreator GetChannelCreator(Type contract, Binding binding, string address);
		protected abstract ChannelCreator GetChannelCreator(Type contract, Binding binding, EndpointAddress address);

		#region IWcfEndpointVisitor Members

		void IWcfEndpointVisitor.VisitServiceEndpoint(ServiceEndpointModel model)
		{
			channelCreator = GetChannelCreator(contract, model.ServiceEndpoint);
		}

		void IWcfEndpointVisitor.VisitConfigurationEndpoint(ConfigurationEndpointModel model)
		{
			channelCreator = GetChannelCreator(contract, model.EndpointName);
		}

		void IWcfEndpointVisitor.VisitBindingEndpoint(BindingEndpointModel model)
		{
			channelCreator = GetChannelCreator(contract, model.Binding, string.Empty);
		}

		void IWcfEndpointVisitor.VisitBindingAddressEndpoint(BindingAddressEndpointModel model)
		{
			if (model.HasViaAddress)
			{
				EndpointAddress address = model.EndpointAddress ?? new EndpointAddress(model.Address);
				ContractDescription description = ContractDescription.GetContract(contract);
				ServiceEndpoint endpoint = new ServiceEndpoint(description, model.Binding, address);
				endpoint.Behaviors.Add(new ClientViaBehavior(model.ViaAddress));
				channelCreator = GetChannelCreator(contract, endpoint);
			}
			else
			{
				if (model.EndpointAddress != null)
				{
					channelCreator = GetChannelCreator(contract, model.Binding, model.EndpointAddress);
				}
				else
				{
					channelCreator = GetChannelCreator(contract, model.Binding, model.Address);
				}
			}
		}

		#endregion
	}
}
