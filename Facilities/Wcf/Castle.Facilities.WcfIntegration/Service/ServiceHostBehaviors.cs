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
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.ServiceModel.Description;
	using Castle.MicroKernel;

	internal class ServiceHostBehaviors : IWcfBehaviorVisitor
	{
		private readonly ServiceHost serviceHost;
		private readonly IKernel kernel;

		public ServiceHostBehaviors(ServiceHost serviceHost, IKernel kernel)
		{
			this.serviceHost = serviceHost;
			this.kernel = kernel;
		}

		public ServiceHostBehaviors Install(ICollection<IWcfBehavior> behaviors)
		{
			foreach (IWcfBehavior behavior in behaviors)
			{
				behavior.Accept(this);
			}
			return this;
		}

		public ServiceHostBehaviors Install(params IWcfBehavior[] behaviors)
		{
			return Install((ICollection<IWcfBehavior>)behaviors);
		}

		#region IWcfBehaviorVisitor Members

		void IWcfBehaviorVisitor.VisitServiceBehavior(IWcfServiceBehavior behavior)
		{
			behavior.Install(serviceHost.Description, kernel);
		}

		void IWcfBehaviorVisitor.VisitEndpointBehavior(IWcfEndpointBehavior behavior)
		{
			foreach (ServiceEndpoint endpoint in serviceHost.Description.Endpoints)
			{
				behavior.Install(endpoint, kernel);
			}
		}

		#endregion
	}
}
