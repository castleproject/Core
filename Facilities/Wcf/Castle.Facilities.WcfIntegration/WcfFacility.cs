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
	using System.ServiceModel.Description;
	using Castle.Core;
	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// Facility to simplify the management of WCF clients and services. 
	/// </summary>
	public class WcfFacility : AbstractFacility
	{
		private WcfClientExtension clientExtension;
		private WcfServiceExtension serviceExtension;

		protected override void Init()
		{
			clientExtension = new WcfClientExtension(Kernel);
			serviceExtension = new WcfServiceExtension(Kernel);

			Kernel.ComponentModelCreated += Kernel_ComponentModelCreated;
		}

		public WcfClientExtension Clients
		{
			get { return clientExtension; }
		}

		public WcfServiceExtension Services
		{
			get { return serviceExtension; }
		}

		private void Kernel_ComponentModelCreated(ComponentModel model)
		{
			Type implementation = model.Implementation;

			if (typeof(IServiceBehavior).IsAssignableFrom(implementation) ||
				typeof(IEndpointBehavior).IsAssignableFrom(implementation) ||
				typeof(IOperationBehavior).IsAssignableFrom(implementation) ||
				typeof(IContractBehavior).IsAssignableFrom(implementation))
			{
				model.LifestyleType = LifestyleType.Transient;
				model.CustomComponentActivator = typeof(WcfBehaviorActivator);
			}
		}

		public override void Dispose()
		{
			base.Dispose();

			if (clientExtension != null) clientExtension.Dispose();
			if (serviceExtension != null) serviceExtension.Dispose();
		}
	}
}

