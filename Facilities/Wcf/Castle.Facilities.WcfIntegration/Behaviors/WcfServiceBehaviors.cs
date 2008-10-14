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

	internal class WcfServiceBehaviors : AbstractWcfBehaviors, IWcfServiceBehavior
	{
		public void Install(ServiceDescription description, IKernel kernel)
		{
			ICollection<IHandler> serviceBehaviors = WcfUtils.FindBehaviors<IServiceBehavior>(
				kernel, WcfBehaviorScope.Services);

			foreach (IHandler handler in serviceBehaviors)
			{
				if (handler.ComponentModel.Implementation == typeof(ServiceDebugBehavior))
				{
					description.Behaviors.Remove<ServiceDebugBehavior>();
				}

				description.Behaviors.Add((IServiceBehavior)handler.Resolve(CreationContext.Empty));
			}
		}

		public override void AddDependencies(IKernel kernel, ComponentModel model)
		{
			WcfUtils.AddBehaviorDependencies<IServiceBehavior>(kernel, WcfBehaviorScope.Services, model);
		}

		public override void Accept(IWcfBehaviorVisitor visitor)
		{
			visitor.VisitServiceBehavior(this);
		}
	}
}
