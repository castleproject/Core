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
	using System.Reflection;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;

	public abstract class AbstractChannelBuilder<M> : AbstractChannelBuilder, IClientChannelBuilder<M>
			where M : IWcfClientModel
	{
		[ThreadStatic] private static M ClientModel;
		[ThreadStatic] private static IWcfBurden Burden;

		protected AbstractChannelBuilder(IKernel kernel, IChannelFactoryBuilder<M> channelFactoryBuilder)
			: base(kernel)
		{
			ChannelFactoryBuilder = channelFactoryBuilder;
		}

		protected IChannelFactoryBuilder<M> ChannelFactoryBuilder { get; private set; }

		/// <summary>
		/// Get a delegate capable of creating channels.
		/// </summary>
		/// <param name="clientModel">The client model.</param>
		/// <param name="burden">Receives the client burden.</param>
		/// <returns>The <see cref="ChannelCreator"/></returns>
		public ChannelCreator GetChannelCreator(M clientModel, out IWcfBurden burden)
		{
			ClientModel = clientModel;
			burden = Burden = new WcfBurden(Kernel);
			return GetEndpointChannelCreator(clientModel.Endpoint);
		}

		/// <summary>
		/// Get a delegate capable of creating channels.
		/// </summary>
		/// <param name="clientModel">The client model.</param>
		/// <param name="contract">The contract override.</param>
		/// <param name="burden">Receives the client burden.</param>
		/// <returns>The <see cref="ChannelCreator"/></returns>
		public ChannelCreator GetChannelCreator(M clientModel, Type contract, out IWcfBurden burden)
		{
			ClientModel = clientModel;
			burden = Burden = new WcfBurden(Kernel);
			return GetEndpointChannelCreator(clientModel.Endpoint, contract);
		}

		protected void ConfigureChannelFactory(ChannelFactory channelFactory)
		{
			ConfigureChannelFactory(channelFactory, ClientModel, Burden);
		}

		#region AbstractChannelBuilder Members

		protected override ChannelCreator GetChannel(Type contract)
		{
			return GetChannel(ClientModel, contract);
		}

		protected override ChannelCreator GetChannel(Type contract, ServiceEndpoint endpoint)
		{
			return GetChannel(ClientModel, contract, endpoint);
		}

		protected override ChannelCreator GetChannel(Type contract, string configurationName)
		{
			return GetChannel(ClientModel, contract, configurationName);
		}

		protected override ChannelCreator GetChannel(Type contract, Binding binding, string address)
		{
			return GetChannel(ClientModel, contract, binding, address);
		}

		protected override ChannelCreator GetChannel(Type contract, Binding binding, EndpointAddress address)
		{
			return GetChannel(ClientModel, contract, binding, address);
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

		#endregion

		protected virtual ChannelCreator CreateChannelCreator(Type contract, M clientModel, 
			                                                  params object[] channelFactoryArgs)
		{
			Type type = typeof(ChannelFactory<>).MakeGenericType(new Type[] { contract });
			var channelFactory = ChannelFactoryBuilder.CreateChannelFactory(type, clientModel, channelFactoryArgs);
			ConfigureChannelFactory(channelFactory);

			MethodInfo methodInfo = type.GetMethod("CreateChannel", new Type[0]);
			return (ChannelCreator)Delegate.CreateDelegate(typeof(ChannelCreator), channelFactory, methodInfo);
		}
	}
}
