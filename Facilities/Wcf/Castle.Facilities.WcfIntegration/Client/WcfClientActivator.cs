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
	using Castle.Facilities.WcfIntegration.Internal;

	internal delegate object CreateChannel();

	public class WcfClientActivator : DefaultComponentActivator
	{
		private readonly CreateChannel createChannel;

		public WcfClientActivator(ComponentModel model, IKernel kernel,
			ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{
			WcfClientModel clientModel = (WcfClientModel)
				model.ExtendedProperties[WcfConstants.ClientModelKey];

			createChannel = new ChannelBuilder().GetChannelCreator(
				clientModel.Endpoint, model.Service);
		}

		protected override object Instantiate(CreationContext context)
		{
			WcfClientModel clientModelOverride = GetClientModelOverride(context);

			if (clientModelOverride != null)
			{
				CreateChannel createChannelOverride = new ChannelBuilder()
					.GetChannelCreator(clientModelOverride.Endpoint, Model.Service);
				return createChannelOverride();
			}
			
			return createChannel();
		}

		private WcfClientModel GetClientModelOverride(CreationContext context)
		{
			WcfClientModel clientModel = null;
			Predicate<WcfClientModel> contractMatch = delegate(WcfClientModel candidate)
			{
				return candidate.Contract == null || 
					(Model.Service == candidate.Contract);
			};

			if (context != null && context.HasAdditionalParameters)
			{
				if (WcfUtils.FindDependency<WcfClientModel>(
						context.AdditionalParameters, contractMatch,
						out clientModel))
				{
					return clientModel;
				}
			}

			return clientModel;
		}
	}
}
