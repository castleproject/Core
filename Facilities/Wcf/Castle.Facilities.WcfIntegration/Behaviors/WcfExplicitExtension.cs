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
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;

	internal abstract class WcfExplicitExtension : AbstractWcfExtension, IWcfServiceExtension, IWcfEndpointExtension
	{
		#region IWcfServiceExtension

		public void Install(ServiceHost serviceHost, IKernel kernel)
		{
			object extension = GetExtensionInstance(kernel);

			if (extension is IServiceBehavior)
			{
				serviceHost.Description.Behaviors.Add((IServiceBehavior)extension);
			}
			else if (extension is IServiceHostAware)
			{
				WcfUtils.BindServiceHostAware(serviceHost, (IServiceHostAware)extension, true);
			}
		}

		#endregion

		#region IWcfEndpointExtension 

		public void Install(ServiceEndpoint endpoint, IKernel kernel)
		{
			object extension = GetExtensionInstance(kernel);

			if (extension is IEndpointBehavior)
			{
				endpoint.Behaviors.Add((IEndpointBehavior)extension);
			}
			else if (extension is IOperationBehavior)
			{
				foreach (OperationDescription operation in endpoint.Contract.Operations)
				{
					operation.Behaviors.Add((IOperationBehavior)extension);
				}
			}
			else if (extension is IContractBehavior)
			{
				endpoint.Contract.Behaviors.Add((IContractBehavior)extension);
			}
		}

		#endregion

		protected abstract object GetExtensionInstance(IKernel kernel);

		internal static IWcfExtension CreateFrom(object extension)
		{
			if (extension is Type)
			{
				return new WcfServiceTypeBehavior((Type)extension);
			}
			else if (extension is string)
			{
				return new WcfServiceKeyBehavior((string)extension);
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
			visitor.VisitEndpointExtension(this);
		}
	}

	#region Class: WcfServiceKeyBehavior

	internal class WcfServiceKeyBehavior : WcfExplicitExtension
	{
		private readonly string key;

		internal WcfServiceKeyBehavior(string key)
		{
			this.key = key;
		}

		protected override object GetExtensionInstance(IKernel kernel)
		{
			return kernel[key];
		}

		public override void AddDependencies(IKernel kernel, ComponentModel model)
		{
			WcfUtils.AddBehaviorDependency(key, null, model);
		}
	}

	#endregion

	#region Class: WcfServiceTypeBehavior

	internal class WcfServiceTypeBehavior : WcfExplicitExtension
	{
		private readonly Type service;

		internal WcfServiceTypeBehavior(Type service)
		{
			this.service = service;
		}

		protected override object GetExtensionInstance(IKernel kernel)
		{
			return kernel.Resolve(service); 
		}

		public override void AddDependencies(IKernel kernel, ComponentModel model)
		{
			WcfUtils.AddBehaviorDependency(null, service, model);
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

		protected override object GetExtensionInstance(IKernel kernel)
		{
			return instance;
		}
	}

	#endregion
}
