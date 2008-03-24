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
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;

	public class WcfClientActivator : DefaultComponentActivator
	{
		private readonly ChannelCreator createChannel;

		public WcfClientActivator(ComponentModel model, IKernel kernel,
			ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{
			createChannel = CreateChannelCreator(kernel, model);
		}

		protected override object InternalCreate(CreationContext context)
		{
			object instance = Instantiate(context);

			ApplyCommissionConcerns(instance);

			return instance;
		}

		protected override object Instantiate(CreationContext context)
		{
			object instance = createChannel();

			try
			{
				instance = Kernel.ProxyFactory.Create(Kernel, instance, Model);
			}
			catch (Exception ex)
			{
				throw new ComponentActivatorException("WcfClientActivator: could not proxy service " +
					Model.Service.FullName, ex);
			}

			return instance;
		}

		private ChannelCreator CreateChannelCreator(IKernel kernel, ComponentModel model)
		{
			WcfClientModel clientModel = (WcfClientModel)
				model.ExtendedProperties[WcfConstants.ClientModelKey];

			IClientChannelBuilder channelBuilder = kernel.Resolve<IClientChannelBuilder>();
			ChannelCreator creator = channelBuilder.GetChannelCreator(clientModel.Endpoint, model.Service);
			model.ExtendedProperties[WcfConstants.ChannelCreatorKey] = creator;
			return creator;
		}
	}
}
