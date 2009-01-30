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
	using System.ServiceModel;
	using Castle.Core;
	using Castle.Core.Interceptor;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;

	[Transient]
	internal class WcfManagedChannelInterceptor : IInterceptor
	{
		private readonly IKernel kernel;

		/// <summary>
		/// Constructs a new <see cref="WcfManagedChannelInterceptor"/>.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		public WcfManagedChannelInterceptor(IKernel kernel)
		{
			this.kernel = kernel;
		}

		#region IInterceptor Members

		public void Intercept(IInvocation invocation)
		{
			EnsureOpenChannel(invocation);

			invocation.Proceed();
		}

		#endregion

		private void EnsureOpenChannel(IInvocation invocation)
		{
			object target = invocation.InvocationTarget;
			var accessor = target as IProxyTargetAccessor;

			if (accessor != null)
			{
				target = accessor.DynProxyGetTarget();
				var changeTarget = (IChangeProxyTarget)invocation;
				changeTarget.ChangeInvocationTarget(target);
			}

			IContextChannel channel = target as IContextChannel;

			if (channel != null && !WcfUtils.IsCommunicationObjectReady(channel))
			{
				WcfUtils.ReleaseCommunicationObject(channel);
				var changeTarget = (IChangeProxyTarget)invocation;
				var channelCreator = channel.Extensions.Find<ChannelCreatorExtension>();
				changeTarget.ChangeInvocationTarget(channelCreator.Create());
			}
		}
	}
}
