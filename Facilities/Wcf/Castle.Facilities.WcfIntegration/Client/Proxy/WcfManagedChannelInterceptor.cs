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
	using Castle.Core;
	using Castle.Core.Interceptor;
	using Castle.Facilities.WcfIntegration.Internal;

	[Transient]
	internal class WcfManagedChannelInterceptor : IInterceptor, IDisposable
	{
		private IClientChannel channel;
		private ChannelCreator channelCreator;

		public void Intercept(IInvocation invocation)
		{
			if (invocation.Method.DeclaringType == typeof(IManagedChannel))
			{
				channel = (IClientChannel)invocation.Arguments[0];
				channelCreator = (ChannelCreator)invocation.Arguments[1];
			}
			else
			{
				EnsureAvailableChannel(invocation);
				invocation.Proceed();
			}
		}

		private void EnsureAvailableChannel(IInvocation invocation)
		{
			if (!WcfUtils.IsCommunicationObjectReady(channel))
			{
				lock (this)
				{
					if (!WcfUtils.IsCommunicationObjectReady(channel))
					{
						WcfUtils.ReleaseCommunicationObject(channel, TimeSpan.Zero);
						channel = (IClientChannel)channelCreator();
					}
				}
			}

			if (invocation.InvocationTarget != channel)
			{
				var changeTarget = (IChangeProxyTarget)invocation;
				changeTarget.ChangeInvocationTarget(channel);
			}
		}

		public void Dispose()
		{
			foreach (var cleanUp in channel.Extensions.FindAll<IWcfCleanUp>())
			{
				cleanUp.CleanUp();
			}			
		}
	}
}
