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
	using System.Reflection;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;

	public abstract class AbstractChannelBuilder<M> : AbstractChannelBuilder, IClientChannelBuilder<M>
			where M : IWcfClientModel
	{
		private M clientModel;

		public AbstractChannelBuilder(IKernel kernel)
			: base(kernel)
		{
		}

		/// <summary>
		/// Get a delegate capable of creating channels.
		/// </summary>
		/// <param name="clientModel">The client model.</param>
		/// <returns>The <see cref="ChannelCreator"/></returns>
		public ChannelCreator GetChannelCreator(M clientModel)
		{
			this.clientModel = clientModel;
			return GetEndpointChannelCreator(clientModel.Endpoint);
		}

		/// <summary>
		/// Get a delegate capable of creating channels.
		/// </summary>
		/// <param name="clientModel">The client model.</param>
		/// <param name="contract">The contract override.</param>
		/// <returns>The <see cref="ChannelCreator"/></returns>
		public ChannelCreator GetChannelCreator(M clientModel, Type contract)
		{
			this.clientModel = clientModel;
			return GetEndpointChannelCreator(clientModel.Endpoint, contract);
		}

		#region AbstractChannelBuilder Members

		protected override ChannelCreator GetChannel(Type contract)
		{
			return GetChannel(clientModel, contract);
		}

		protected override ChannelCreator GetChannel(Type contract, ServiceEndpoint endpoint)
		{
			return GetChannel(clientModel, contract, endpoint);
		}

		protected override ChannelCreator GetChannel(Type contract, string configurationName)
		{
			return GetChannel(clientModel, contract, configurationName);
		}

		protected override ChannelCreator GetChannel(Type contract, Binding binding, string address)
		{
			return GetChannel(clientModel, contract, binding, address);
		}

		protected override ChannelCreator GetChannel(Type contract, Binding binding, EndpointAddress address)
		{
			return GetChannel(clientModel, contract, binding, address);
		}

		#endregion

		#region GetChannel Members

		protected virtual ChannelCreator GetChannel(M clientModel, Type contract)
		{
			return CreateChannelCreator(contract, clientModel, contract);
		}

		protected virtual ChannelCreator GetChannel(M clientModel, Type contract, ServiceEndpoint endpoint)
		{
			return CreateChannelCreator(contract, clientModel, endpoint);
		}

		protected virtual ChannelCreator GetChannel(M clientModel, Type contract, string configurationName)
		{
			return CreateChannelCreator(contract, clientModel, configurationName);
		}

		protected virtual ChannelCreator GetChannel(M clientModel, Type contract, Binding binding, string address)
		{
			return CreateChannelCreator(contract, clientModel, binding, address);
		}

		protected virtual ChannelCreator GetChannel(M clientModel, Type contract, Binding binding, 
			                                        EndpointAddress address)
		{
			return CreateChannelCreator(contract, clientModel, binding, address);
		}

		protected virtual ChannelCreator CreateChannelCreator(Type contract, M clientModel, 
			                                                  params object[] channelFactoryArgs)
		{
			Type type = typeof(ChannelFactory<>).MakeGenericType(new Type[] { contract });

			ChannelFactory channelFactory = (ChannelFactory)
				Activator.CreateInstance(type, channelFactoryArgs);
			channelFactory.Opening += delegate { OnOpening(channelFactory, clientModel); };
			channelFactory.Closed += delegate { OnClosed(channelFactory, clientModel); };

			MethodInfo methodInfo = type.GetMethod("CreateChannel", new Type[0]);
			return (ChannelCreator)Delegate.CreateDelegate(
				typeof(ChannelCreator), channelFactory, methodInfo);
		}

		protected virtual void OnOpening(ChannelFactory channelFactory, M clientModel)
		{
			ServiceEndpointExtensions extensions =
				new ServiceEndpointExtensions(channelFactory.Endpoint, Kernel)
					.Install(new WcfEndpointExtensions(WcfExtensionScope.Clients));

			if (clientModel != null)
			{
				extensions.Install(clientModel.Extensions);
				extensions.Install(clientModel.Endpoint.Extensions);
			}
		}

		protected virtual void OnClosed(ChannelFactory channelFactory, M clientModel)
		{
		}

		#endregion
	}
}
