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

namespace Castle.Facilities.WcfIntegration
{
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Description;
	using Castle.Core;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;

	internal class WcfServiceExtensions : AbstractWcfExtension, IWcfServiceExtension
	{
		public void Install(ServiceHost serviceHost, IKernel kernel, IWcfBurden burden)
		{
			BindServiceHostAware(serviceHost, kernel, burden);
			AddServiceBehaviors(serviceHost, kernel, burden);
		}

		public override void AddDependencies(IKernel kernel, ComponentModel model)
		{
			WcfUtils.AddExtensionDependencies<IServiceBehavior>(kernel, WcfExtensionScope.Services, model);
			WcfUtils.AddExtensionDependencies<IServiceHostAware>(kernel, WcfExtensionScope.Services, model);
		}

		public override void Accept(IWcfExtensionVisitor visitor)
		{
			visitor.VisitServiceExtension(this);
		}

		private void AddServiceBehaviors(ServiceHost serviceHost, IKernel kernel, IWcfBurden burden)
		{
			WcfUtils.AddBehaviors(kernel, WcfExtensionScope.Services,
				serviceHost.Description.Behaviors, burden, delegate(IServiceBehavior behavior)
				{
					if (behavior.GetType() == typeof(ServiceBehaviorAttribute))
					{
						serviceHost.Description.Behaviors.Remove<ServiceBehaviorAttribute>();
					}
					else if (behavior.GetType() == typeof(ServiceDebugBehavior))
					{
						serviceHost.Description.Behaviors.Remove<ServiceDebugBehavior>();
					}
					else if (behavior.GetType() == typeof(AspNetCompatibilityRequirementsAttribute))
					{
						serviceHost.Description.Behaviors.Remove<AspNetCompatibilityRequirementsAttribute>();
					}
					return true;
				});
		}

		private void BindServiceHostAware(ServiceHost serviceHost, IKernel kernel, IWcfBurden burden)
		{
            WcfUtils.AddBehaviors<IServiceHostAware>(kernel, WcfExtensionScope.Services, null, burden,
				delegate(IServiceHostAware serviceHostAware)
				{
					WcfUtils.BindServiceHostAware(serviceHost, serviceHostAware, true);
					return true;
				});
		}
	}
}
