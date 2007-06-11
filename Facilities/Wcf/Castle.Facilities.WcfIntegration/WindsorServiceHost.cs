// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.ServiceModel;
	using System.ServiceModel.Description;
	using MicroKernel;
	using Windsor;

	public class WindsorServiceHost : ServiceHost
	{
		private readonly IWindsorContainer container;

		public WindsorServiceHost(IWindsorContainer container, Type serviceType, params Uri[] baseAddresses)
			: base(serviceType, baseAddresses)
		{
			this.container = container;
		}

		protected override void OnOpening()
		{
			Description.Behaviors.Add(new WindsorDependencyInjectionServiceBehavior(container));
			IHandler[] serviceBehaviorHandlers = container.Kernel.GetHandlers(typeof (IServiceBehavior));
			foreach (IHandler handler in serviceBehaviorHandlers)
			{
				Description.Behaviors.Add((IServiceBehavior) handler.Resolve(CreationContext.Empty));
			}
			IHandler[] endPointBehaviors = container.Kernel.GetHandlers(typeof (IEndpointBehavior));
			foreach (IHandler handler in endPointBehaviors)
			{
				foreach (ServiceEndpoint endpoint in Description.Endpoints)
				{
					endpoint.Behaviors.Add((IEndpointBehavior) handler.Resolve(CreationContext.Empty));
				}
			}
			base.OnOpening();
		}
	}
}