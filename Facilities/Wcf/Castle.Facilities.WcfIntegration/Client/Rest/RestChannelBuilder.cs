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

namespace Castle.Facilities.WcfIntegration.Rest
{
	using System;
	using System.Reflection;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Web;
	using Castle.MicroKernel;

	public class RestChannelBuilder : AbstractChannelBuilder<RestClientModel>
	{
		public RestChannelBuilder(IKernel kernel, IChannelFactoryBuilder<RestClientModel> channelFactoryBuilder)
			: base(kernel, channelFactoryBuilder)
		{
		}

		protected override ChannelCreator GetChannel(RestClientModel clientModel, Type contract,
												     Binding binding, string address)
		{
			Uri remoteAddress = new Uri(address, UriKind.Absolute);

			if (binding == null)
			{
				return CreateChannelCreator(contract, clientModel, remoteAddress);
			}

			return CreateChannelCreator(contract, clientModel, binding, remoteAddress);
		}

		protected override ChannelCreator GetChannel(RestClientModel clientModel, Type contract, 
			                                         Binding binding, EndpointAddress address)
		{
			return GetChannel(clientModel, contract, binding, address.Uri.AbsoluteUri);
		}

		protected override ChannelCreator CreateChannelCreator(Type contract, RestClientModel clientModel,
			                                                   params object[] channelFactoryArgs)
		{
			Type type = typeof(WebChannelFactory<>).MakeGenericType(new Type[] { contract });
			var channelFactory = ChannelFactoryBuilder.CreateChannelFactory(type, clientModel, channelFactoryArgs);
			ConfigureChannelFactory(channelFactory);

			MethodInfo methodInfo = type.GetMethod("CreateChannel", new Type[0]);
			return (ChannelCreator)Delegate.CreateDelegate(typeof(ChannelCreator), channelFactory, methodInfo);
		}
	}
}
