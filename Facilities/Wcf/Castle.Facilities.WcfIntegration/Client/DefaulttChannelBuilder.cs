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
	using Castle.MicroKernel;

	public class DefaultChannelBuilder : AbstractChannelBuilder
	{
		private readonly IKernel kernel;

		public DefaultChannelBuilder(IKernel kernel)
		{
			this.kernel = kernel;
		}

		protected override ChannelCreator GetChannelCreator(Type contract, ServiceEndpoint endpoint)
		{
			return CreateChannelCreator(contract, endpoint);
		}

		protected override ChannelCreator GetChannelCreator(Type contract, string configurationName)
		{
			return CreateChannelCreator(contract, configurationName);
		}

		protected override ChannelCreator GetChannelCreator(Type contract, Binding binding, string address)
		{
			return CreateChannelCreator(contract, binding, address);
		}

		protected override ChannelCreator GetChannelCreator(Type contract, Binding binding, 
			                                                EndpointAddress address)
		{
			return CreateChannelCreator(contract, binding, address);
		}

		private ChannelCreator CreateChannelCreator(Type contract, params object[] channelFactoryArgs)
		{
			Type type = typeof(ChannelFactory<>).MakeGenericType(new Type[] { contract });

			ChannelFactory channelFactory = (ChannelFactory)
				Activator.CreateInstance(type, channelFactoryArgs);
			channelFactory.Opening += ChannelFactory_Opening;

			MethodInfo methodInfo = type.GetMethod("CreateChannel", new Type[0]);
			return (ChannelCreator)Delegate.CreateDelegate(
				typeof(ChannelCreator), channelFactory, methodInfo);
		}

		private void ChannelFactory_Opening(object sender, EventArgs e)
		{
			ChannelFactory channelFactory = (ChannelFactory)sender;
			ServiceEndpoint endpoint = channelFactory.Endpoint;

			IHandler[] endPointBehaviors = kernel.GetHandlers(typeof(IEndpointBehavior));
			foreach (IHandler handler in endPointBehaviors)
			{
				endpoint.Behaviors.Add((IEndpointBehavior)handler.Resolve(CreationContext.Empty));
			}

			IHandler[] operationBehaviors = kernel.GetHandlers(typeof(IOperationBehavior));
			foreach (OperationDescription operation in endpoint.Contract.Operations)
			{
				foreach (IHandler operationHandler in operationBehaviors)
				{
					operation.Behaviors.Add((IOperationBehavior)
						operationHandler.Resolve(CreationContext.Empty));
				}
			}
		}
	}
}
