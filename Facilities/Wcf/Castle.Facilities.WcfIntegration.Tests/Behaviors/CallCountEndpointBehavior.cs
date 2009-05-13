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

using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Castle.Facilities.WcfIntegration.Tests.Behaviors
{
	public class CallCountEndpointBehavior
		: IEndpointBehavior
	{
		public static int CallCount = 0;


		public CallCountEndpointBehavior()
		{
		}

		#region IEndpointBehavior Members

		public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{

		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(new CallCountMessageInspector());
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
		{

		}

		public void Validate(ServiceEndpoint endpoint)
		{

		}

		#endregion

		public class CallCountMessageInspector : IClientMessageInspector
		{
			#region IClientMessageInspector Members

			public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
			{
			}

			public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
			{
				CallCount++;
				return null;
			}

			#endregion
		}
	}
}
