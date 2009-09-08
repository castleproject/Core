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
	using Castle.Core;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;

	public class WcfChannelExtensions : AbstractWcfExtension, IWcfChannelExtension
	{
		public void Install(ChannelFactory channelFactory, IKernel kernel, IWcfBurden burden)
		{
			BindChannelFactoryAware(channelFactory, kernel, burden);
		}

		public override void AddDependencies(IKernel kernel, ComponentModel model)
		{
			WcfUtils.AddExtensionDependencies<IWcfChannelPolicy>(kernel, WcfExtensionScope.Undefined, model);
			WcfUtils.AddExtensionDependencies<IChannelFactoryAware>(kernel, WcfExtensionScope.Clients, model);
		}

		public override void Accept(IWcfExtensionVisitor visitor)
		{
			visitor.VisitChannelExtension(this);
		}

		private void BindChannelFactoryAware(ChannelFactory channelFactory, IKernel kernel, IWcfBurden burden)
		{
			WcfUtils.AddBehaviors<IWcfChannelPolicy>(kernel, WcfExtensionScope.Undefined, null, burden, null);
			WcfUtils.AddBehaviors<IChannelFactoryAware>(kernel, WcfExtensionScope.Clients, null, burden,
				delegate(IChannelFactoryAware channelFactoryAware)
				{
					WcfUtils.BindChannelFactoryAware(channelFactory, channelFactoryAware, true);
					return true;
				});
		}
	}
}
