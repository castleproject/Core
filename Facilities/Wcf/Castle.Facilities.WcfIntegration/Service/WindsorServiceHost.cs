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
	using System.ServiceModel;
	using System.ServiceModel.Description;
	using Castle.Core;
	using Castle.MicroKernel;

	public class WindsorServiceHost : ServiceHost
	{
		private readonly IKernel kernel;
		private readonly ComponentModel model;

		public WindsorServiceHost(IKernel kernel, Type serviceType, params Uri[] baseAddresses)
			: base(serviceType, baseAddresses)
		{
			this.kernel = kernel;
		}

		public WindsorServiceHost(IKernel kernel, ComponentModel model, params Uri[] baseAddresses)
			: this(kernel, model.Implementation, baseAddresses)
		{
			this.model = model;
		}

		protected override void OnOpening()
		{
			Description.Behaviors.Add(new WindsorDependencyInjectionServiceBehavior(kernel, model));

			IHandler[] serviceBehaviorHandlers = kernel.GetAssignableHandlers(typeof (IServiceBehavior));
			foreach (IHandler handler in serviceBehaviorHandlers)
			{
				if (handler.ComponentModel.Implementation == typeof(ServiceDebugBehavior))
				{
					Description.Behaviors.Remove<ServiceDebugBehavior>();
				}
				Description.Behaviors.Add((IServiceBehavior) handler.Resolve(CreationContext.Empty));
			}

			IHandler[] endPointBehaviors = kernel.GetAssignableHandlers(typeof(IEndpointBehavior));
			IHandler[] operationBehaviors = kernel.GetAssignableHandlers(typeof(IOperationBehavior));

			foreach (ServiceEndpoint endpoint in Description.Endpoints)
			{
				foreach (IHandler handler in endPointBehaviors)
				{
					endpoint.Behaviors.Add((IEndpointBehavior) handler.Resolve(CreationContext.Empty));
				}

				foreach (OperationDescription operation in endpoint.Contract.Operations)
				{
					foreach (IHandler operationHandler in operationBehaviors)
					{
						operation.Behaviors.Add((IOperationBehavior)operationHandler.Resolve(CreationContext.Empty));
					}
				}
			}

			base.OnOpening();
		}
	}
}