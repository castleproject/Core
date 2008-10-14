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
	using System.ServiceModel.Description;
	using Castle.Core;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;

	internal class WcfEndpointBehaviors : AbstractWcfBehaviors, IWcfEndpointBehavior
	{
		private readonly WcfBehaviorScope scope;

		public WcfEndpointBehaviors(WcfBehaviorScope scope)
		{
			this.scope = scope;
		}

		public void Install(ServiceEndpoint endpoint, IKernel kernel)
		{
			ICollection<IHandler> endpointBehaviors = WcfUtils.FindBehaviors<IEndpointBehavior>(kernel, scope);
			ICollection<IHandler> operationBehaviors = WcfUtils.FindBehaviors<IOperationBehavior>(kernel, scope);

			foreach (IHandler handler in endpointBehaviors)
			{
				endpoint.Behaviors.Add((IEndpointBehavior)handler.Resolve(CreationContext.Empty));
			}

			foreach (OperationDescription operation in endpoint.Contract.Operations)
			{
				foreach (IHandler operationHandler in operationBehaviors)
				{
					operation.Behaviors.Add((IOperationBehavior)operationHandler.Resolve(CreationContext.Empty));
				}
			}
		}

		public override void AddDependencies(IKernel kernel, ComponentModel model)
		{
			WcfUtils.AddBehaviorDependencies<IEndpointBehavior>(kernel, scope, model);
			WcfUtils.AddBehaviorDependencies<IOperationBehavior>(kernel, scope, model);
		}

		override public void Accept(IWcfBehaviorVisitor visitor)
		{
			visitor.VisitEndpointBehavior(this);
		}
	}
}
