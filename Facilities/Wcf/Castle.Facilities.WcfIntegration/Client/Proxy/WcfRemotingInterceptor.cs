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
// limitations under the License

namespace Castle.Facilities.WcfIntegration.Proxy
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Remoting.Messaging;
	using Castle.Core.Interceptor;

	public class WcfRemotingInterceptor : IWcfInterceptor
	{
		private readonly WcfClientExtension clients;

		public WcfRemotingInterceptor(WcfClientExtension clients)
		{
			this.clients = clients;
		}

		public WcfClientExtension Clients
		{
			get { return clients; }
		}

		public void Intercept(IInvocation invocation)
		{
			var channelHolder = invocation.Proxy as IWcfChannelHolder;

			if (channelHolder == null)
			{
				throw new ArgumentException("The given Proxy is not valid WCF dynamic proxy.");
			}

			ApplyRefreshPolicy(channelHolder, invocation);

			PerformInvocation(invocation, channelHolder);
		}

		protected virtual void PerformInvocation(IInvocation invocation, IWcfChannelHolder channelHolder)
		{
			Action sendAction = () =>
			{
				var proxy = channelHolder.RealProxy;
				var message = new MethodCallMessage(invocation.Method, invocation.Arguments);
				var returnMessage = (IMethodReturnMessage)proxy.Invoke(message);

				if (returnMessage.Exception != null)
				{
					throw returnMessage.Exception;
				}

				invocation.ReturnValue = returnMessage.ReturnValue;
			};

			ApplyActionPolicy(channelHolder, invocation, sendAction);
		}

		bool IWcfInterceptor.Handles(MethodInfo method)
		{
			return Handles(method);
		}

		protected virtual bool Handles(MethodInfo method)
		{
			return true;
		}

		protected void ApplyRefreshPolicy(IWcfChannelHolder channelHolder, IInvocation invocation)
		{
			if (!channelHolder.IsChannelUsable)
			{
				bool hasCustomRefreshPolicy = false;
				var channelBurden = channelHolder.ChannelBurden;

				foreach (var refreshPolicy in channelBurden.Dependencies
					.OfType<IRefreshChannelPolicy>().OrderBy(p => p.ExecutionOrder))
				{
					refreshPolicy.WantsToUseUnusableChannel(channelHolder, invocation.Method);
					hasCustomRefreshPolicy = true;
				}

				if (!hasCustomRefreshPolicy)
				{
					channelHolder.RefreshChannel();
				}
			}
		}

		protected void ApplyActionPolicy(IWcfChannelHolder channelHolder, IInvocation invocation, Action action)
		{
			bool actionPolicyApplied = false;
			var channelBurden = channelHolder.ChannelBurden;

			foreach (var actionPolicy in channelBurden.Dependencies
				.OfType<IChannelActionPolicy>().OrderBy(p => p.ExecutionOrder))
			{
				if (actionPolicy.Perform(channelHolder, invocation.Method, action))
				{
					actionPolicyApplied = true;
					break;
				}
			}

			if (!actionPolicyApplied)
			{
				if (clients.DefaultChannelPolicy != null)
				{
					clients.DefaultChannelPolicy.Perform(channelHolder, invocation.Method, action);
				}
				else
				{
					action();
				}
			}
		}
	}
}