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

namespace Castle.Facilities.WcfIntegration.Internal
{
	using System;
	using System.Reflection;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;

	internal class ChannelBuilder : IWcfEndpointVisitor
	{
		private Type contract;
		private CreateChannel channelCreator;

		public CreateChannel GetChannelCreator(IWcfEndpoint endpoint)
		{
			return GetChannelCreator(endpoint, null);
		}

		public CreateChannel GetChannelCreator(IWcfEndpoint endpoint, Type contract)
		{
			if (endpoint == null)
			{
				throw new ArgumentNullException("endpoint");
			}

			this.contract = contract ?? endpoint.Contract;
			endpoint.Accept(this);
			return channelCreator;
		}

		#region IWcfEndpointVisitor Members

		void IWcfEndpointVisitor.VisitServiceEndpointModel(ServiceEndpointModel model)
		{
			channelCreator = GetChannelCreator(model.ServiceEndpoint);
		}

		void IWcfEndpointVisitor.VisitConfigurationEndpointModel(ConfigurationEndpointModel model)
		{
			channelCreator = GetChannelCreator(model.EndpointName);
		}

		void IWcfEndpointVisitor.VisitBindingEndpointModel(BindingEndpointModel model)
		{
			channelCreator = GetChannelCreator(model.Binding, string.Empty);
		}

		void IWcfEndpointVisitor.VisitBindingAddressEndpointModel(BindingAddressEndpointModel model)
		{
			if (model.HasViaAddress)
			{
				EndpointAddress address = model.EndpointAddress ?? new EndpointAddress(model.Address);
				ContractDescription description = ContractDescription.GetContract(contract);
				ServiceEndpoint endpoint = new ServiceEndpoint(description, model.Binding, address);
				endpoint.Behaviors.Add(new ClientViaBehavior(model.ViaAddress));
				channelCreator = GetChannelCreator(endpoint);
			}
			else
			{
				object address = (object)model.EndpointAddress ?? model.Address;
				channelCreator = GetChannelCreator(model.Binding, address);
			}
		}

		#endregion

		private CreateChannel GetChannelCreator(params object[] channelFactoryArgs)
		{
			Type type = typeof(ChannelFactory<>).MakeGenericType(new Type[] { contract });

			IChannelFactory channelFactory = (IChannelFactory)
				Activator.CreateInstance(type, channelFactoryArgs);

			MethodInfo methodInfo = type.GetMethod("CreateChannel", new Type[0]);
			return (CreateChannel)Delegate.CreateDelegate(
				typeof(CreateChannel), channelFactory, methodInfo);
		}
	}
}
