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
	using System.Collections.Generic;
	using System.ServiceModel.Description;
	using Castle.MicroKernel;

	internal class ServiceEndpointBehaviors : IWcfBehaviorVisitor
	{
		private readonly ServiceEndpoint endpoint;
		private readonly IKernel kernel;

		public ServiceEndpointBehaviors(ServiceEndpoint endpoint, IKernel kernel)
		{
			this.endpoint = endpoint;
			this.kernel = kernel;
		}

		public ServiceEndpointBehaviors Install(ICollection<IWcfBehavior> behaviors)
		{
			foreach (IWcfBehavior behavior in behaviors)
			{
				behavior.Accept(this);
			}
			return this;
		}

		public ServiceEndpointBehaviors Install(params IWcfBehavior[] behaviors)
		{
			return Install((ICollection<IWcfBehavior>)behaviors);
		}

		#region IWcfBehaviorVisitor Members

		void IWcfBehaviorVisitor.VisitServiceBehavior(IWcfServiceBehavior behavior)
		{
		}

		void IWcfBehaviorVisitor.VisitEndpointBehavior(IWcfEndpointBehavior behavior)
		{
			behavior.Install(endpoint, kernel);
		}

		#endregion
	}
}
