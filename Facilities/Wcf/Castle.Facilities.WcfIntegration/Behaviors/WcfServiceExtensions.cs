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
	using Castle.Core;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;

	internal class WcfServiceExtensions : AbstractWcfExtension, IWcfServiceExtension
	{
		public void Install(ServiceHost serviceHost, IKernel kernel)
		{
			BindServiceHostAware(serviceHost, kernel);
			AddServiceBehaviors(serviceHost, kernel);
		}

		public override void AddDependencies(IKernel kernel, ComponentModel model)
		{
			WcfUtils.AddExtensionDependencies<IServiceBehavior>(kernel, WcfExtensionScope.Services, model);
		}

		public override void Accept(IWcfExtensionVisitor visitor)
		{
			visitor.VisitServiceExtension(this);
		}

		private void AddServiceBehaviors(ServiceHost serviceHost, IKernel kernel)
		{
			ICollection<IHandler> serviceBehaviors = WcfUtils.FindExtensions<IServiceBehavior>(
				kernel, WcfExtensionScope.Services);

			ServiceDescription description = serviceHost.Description;

			foreach (IHandler handler in serviceBehaviors)
			{
				if (handler.ComponentModel.Implementation == typeof(ServiceDebugBehavior))
				{
					description.Behaviors.Remove<ServiceDebugBehavior>();
				}

				description.Behaviors.Add((IServiceBehavior)handler.Resolve(CreationContext.Empty));
			}
		}

		private void BindServiceHostAware(ServiceHost serviceHost, IKernel kernel)
		{
			ICollection<IHandler> serviceHostAwares = WcfUtils.FindExtensions<IServiceHostAware>(
				kernel, WcfExtensionScope.Services);

			foreach (IHandler handler in serviceHostAwares)
			{
				IServiceHostAware serviceHostAware = (IServiceHostAware)handler.Resolve(CreationContext.Empty);
				WcfUtils.BindServiceHostAware(serviceHost, serviceHostAware, true);
			}
		}
	}
}
