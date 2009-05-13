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
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using System.ServiceModel.Dispatcher;

	public class UnitOfworkEndPointBehavior : IEndpointBehavior
	{
		#region IEndpointBehavior Members

		public void Validate(ServiceEndpoint endpoint)
		{
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			foreach (DispatchOperation operation in endpointDispatcher.DispatchRuntime.Operations)
			{
				operation.CallContextInitializers.Add(new UnitOfWorkCallContextInitializer());
			}
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
		}

		#endregion
	}

	public class UnitOfWorkCallContextInitializer : ICallContextInitializer
	{
		#region ICallContextInitializer Members

		public object BeforeInvoke(InstanceContext instanceContext, IClientChannel channel, Message message)
		{
			UnitOfWork.initialized = true;
			return null;
		}

		public void AfterInvoke(object correlationState)
		{
			UnitOfWork.initialized = false;
		}

		#endregion
	}

	public static class UnitOfWork
	{
		public static bool initialized = false;
	}
}