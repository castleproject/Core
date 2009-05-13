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

namespace Castle.Facilities.WcfIntegration.Tests.Behaviors
{
	using System.Collections.ObjectModel;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using System.ServiceModel.Dispatcher;

	public class CallCountServiceBehavior : IServiceBehavior
	{
		public static int CallCount = 0;

		#region IServiceBehavior Members

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

		public void AddBindingParameters(
			ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
			{
				foreach (EndpointDispatcher endPointDispatcher in channelDispatcher.Endpoints)
				{
					endPointDispatcher.DispatchRuntime.MessageInspectors.Add(new CallCountMessageInspector());
				}
			}
		}

		#endregion

		#region Nested type: CallCountMessageInspector

		public class CallCountMessageInspector : IDispatchMessageInspector
		{
			#region IDispatchMessageInspector Members

			public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
			{
				CallCount += 1;
				return null;
			}

			public void BeforeSendReply(ref Message reply, object correlationState)
			{
			}

			#endregion
		}

		#endregion
	}
}