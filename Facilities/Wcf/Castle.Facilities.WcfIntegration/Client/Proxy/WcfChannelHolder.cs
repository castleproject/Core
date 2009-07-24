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
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Runtime.Remoting;
	using System.Runtime.Remoting.Proxies;
	using System.ServiceModel;
	using Castle.Facilities.WcfIntegration.Internal;

	public class WcfChannelHolder : IWcfChannelHolder
	{
		private readonly ChannelCreator channelCreator;

		public WcfChannelHolder(ChannelCreator channelCreator, IWcfBurden burden)
		{
			this.channelCreator = channelCreator;
			ChannelBurden = burden;
			ObtainChannelFactory();
			CreateChannel();
		}

		public IClientChannel Channel { get; private set; }

		public RealProxy RealProxy { get; private set; }

		public ChannelFactory ChannelFactory { get; private set; }

		public IWcfBurden ChannelBurden { get; private set; }

		public bool IsChannelUsable
		{
			get { return WcfUtils.IsCommunicationObjectReady(Channel); }
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void RefreshChannel()
		{
			if (Channel == null || !IsChannelUsable)
			{
				if (Channel != null)
				{
					WcfUtils.ReleaseCommunicationObject(Channel, TimeSpan.Zero);
				}

				CreateChannel();
			}
		}

		public void Dispose()
		{
			foreach (var cleanUp in Channel.Extensions.FindAll<IWcfCleanUp>())
			{
				cleanUp.CleanUp();
			}

			if (Channel != null)
			{
				WcfUtils.ReleaseCommunicationObject(Channel, TimeSpan.Zero);
			}
		}

		private void CreateChannel()
		{
			Channel = (IClientChannel)channelCreator();
			RealProxy = RemotingServices.GetRealProxy(Channel);
		}

		private void ObtainChannelFactory()
		{
			var channelFactoryHolder = ChannelBurden.Dependencies
				.OfType<ChannelFactoryHolder>().FirstOrDefault();

			if (channelFactoryHolder != null)
			{
				ChannelFactory = channelFactoryHolder.ChannelFactory;
			}
		}
	}
}