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
	using System;
	using System.ServiceModel;
	using System.ServiceModel.Description;
	using Castle.Core;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;

	internal abstract class WcfExplicitExtension : AbstractWcfExtension, 
		IWcfServiceExtension, IWcfChannelExtension, IWcfEndpointExtension
	{
		#region IWcfServiceExtension

		public void Install(ServiceHost serviceHost, IKernel kernel, IWcfBurden burden)
		{
			object extension = GetExtensionInstance(kernel, burden);

			if (extension is IServiceBehavior)
			{
				serviceHost.Description.Behaviors.Add((IServiceBehavior)extension);
			}
			else if (extension is IServiceHostAware)
			{
				WcfUtils.BindServiceHostAware(serviceHost, (IServiceHostAware)extension, true);
			}
			else if (extension is IExtension<ServiceHostBase>)
			{
				serviceHost.Extensions.Add((IExtension<ServiceHostBase>)extension);
			}
			else
			{
				WcfUtils.AttachExtension(serviceHost.Description.Behaviors, extension);
			}
		}

		#endregion

		#region IWcfChannelExtension Members

		public void Install(ChannelFactory channelFactory, IKernel kernel, IWcfBurden burden)
		{
			object extension = GetExtensionInstance(kernel, burden);

			if (extension is IChannelFactoryAware)
			{
				WcfUtils.BindChannelFactoryAware(channelFactory, (IChannelFactoryAware)extension, true);
			}
		}

		#endregion

		#region IWcfEndpointExtension 

		public void Install(ServiceEndpoint endpoint, bool withContract, IKernel kernel, IWcfBurden burden)
		{
			object extension = GetExtensionInstance(kernel, burden);

			if (extension is IEndpointBehavior)
			{
				endpoint.Behaviors.Add((IEndpointBehavior)extension);
			}
			else if (extension is IOperationBehavior)
			{
				if (withContract)
				{
					foreach (var operation in endpoint.Contract.Operations)
					{
						operation.Behaviors.Add((IOperationBehavior)extension);
					}
				}
			}
			else if (extension is IContractBehavior)
			{
				if (withContract)
				{
					endpoint.Contract.Behaviors.Add((IContractBehavior)extension);
				}
			}
			else if (!WcfUtils.AttachExtension(endpoint.Behaviors, extension))
			{
				if (withContract && !WcfUtils.AttachExtension(endpoint.Contract.Behaviors, extension))
				{
					Type owner = null;

					if (WcfUtils.IsExtension(extension, ref owner))
					{
						if (typeof(IOperationBehavior).IsAssignableFrom(owner))
						{
							foreach (var operation in endpoint.Contract.Operations)
							{
								WcfUtils.AttachExtension(operation.Behaviors, extension, owner);
							}
						}
					}
				}
			}
		}

		#endregion

		protected abstract object GetExtensionInstance(IKernel kernel, IWcfBurden burden);

		internal static IWcfExtension CreateFrom(object extension)
		{
			if (extension is Type)
			{
				return new WcfServiceTypeExtension((Type)extension);
			}
			else if (extension is string)
			{
				return new WcfServiceKeyExtension((string)extension);
			}
			else if (extension is IWcfExtension)
			{
				return (IWcfExtension)extension;
			}
			else
			{
				return new WcfInstanceExtension(extension);
			}
		}

		override public void Accept(IWcfExtensionVisitor visitor)
		{
			visitor.VisitServiceExtension(this);
			visitor.VisitChannelExtension(this);
			visitor.VisitEndpointExtension(this);
		}
	}

	#region Class: WcfServiceKeyExtension

	internal class WcfServiceKeyExtension : WcfExplicitExtension
	{
		private readonly string key;

		internal WcfServiceKeyExtension(string key)
		{
			this.key = key;
		}

		protected override object GetExtensionInstance(IKernel kernel, IWcfBurden burden)
		{
			object extension = kernel[key];
			burden.Add(extension);
			return extension;
		}

		public override void AddDependencies(IKernel kernel, ComponentModel model)
		{
			WcfUtils.AddExtensionDependency(key, null, model);
		}
	}

	#endregion

	#region Class: WcfServiceTypeExtension

	internal class WcfServiceTypeExtension : WcfExplicitExtension
	{
		private readonly Type service;

		internal WcfServiceTypeExtension(Type service)
		{
			this.service = service;
		}

		protected override object GetExtensionInstance(IKernel kernel, IWcfBurden burden)
		{
			object extension = kernel.Resolve(service);
			burden.Add(extension);
			return extension;
		}

		public override void AddDependencies(IKernel kernel, ComponentModel model)
		{
			WcfUtils.AddExtensionDependency(null, service, model);
		}
	}

	#endregion

	#region Class: WcfInstanceExtension

	internal class WcfInstanceExtension : WcfExplicitExtension
	{
		private readonly object instance;

		internal WcfInstanceExtension(object instance)
		{
			this.instance = instance;
		}

		protected override object GetExtensionInstance(IKernel kernel, IWcfBurden burden)
		{
			burden.Add(instance);
			return instance;
		}
	}

	#endregion
}
