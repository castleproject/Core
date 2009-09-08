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

namespace Castle.Facilities.WcfIntegration.Async
{
	using System;
	using System.Reflection;
	using System.Runtime.Remoting.Messaging;
	using Castle.Core.Interceptor;
	using Castle.Facilities.WcfIntegration.Async.TypeSystem;
	using Castle.Facilities.WcfIntegration.Proxy;

	public class WcfRemotingAsyncInterceptor : WcfRemotingInterceptor
	{
		private readonly AsyncType asyncType;

		[ThreadStatic]
		private static AsyncWcfCallContext callContext;

		public WcfRemotingAsyncInterceptor(AsyncType asyncType, WcfClientExtension clients)
			: base(clients)
		{
			this.asyncType = asyncType;
		}

		internal AsyncWcfCallContext PrepareCall(AsyncCallback callback, object state, object proxy, object result)
		{
			var channelHolder = proxy as IWcfChannelHolder;

			if (channelHolder == null)
			{
				throw new ArgumentException(
					"The given proxy is not supported.  Did you create it using the WcfFacility? " +
					"If the answer is yes, this is probably a bug so please report it.");
			}

			var context = new AsyncWcfCallContext(callback, state, asyncType, channelHolder, result);
			callContext = context;
			return context;
		}

		protected override bool Handles(MethodInfo method)
		{
			return method.DeclaringType.IsAssignableFrom(asyncType.SyncType);
		}

		protected override void PerformInvocation(IInvocation invocation, IWcfChannelHolder channelHolder)
		{
			if (callContext == null)
			{
				base.PerformInvocation(invocation, channelHolder);
				return;
			}

			var context = callContext;
			callContext = null;

			CallBeginMethod(invocation, context);
		}

		private void CallBeginMethod(IInvocation invocation, AsyncWcfCallContext context)
		{
			context.Init(invocation.Method, invocation.Arguments);

			Action beginAction = () =>
			{
				var message = context.CreateBeginMessage();
				var returnMessage = context.ChannelHolder.RealProxy.Invoke(message) as IMethodReturnMessage;
				invocation.ReturnValue = context.PostProcess(returnMessage);
			};

			ApplyActionPolicy(context.ChannelHolder, invocation, beginAction);
		}

		public TResult EndCall<TResult>(AsyncWcfCallContext context, out object[] outs)
		{
			var returnMessage = CallEndMethod(context, out outs);
			return (TResult) returnMessage.ReturnValue;
		}

		private IMethodReturnMessage CallEndMethod(AsyncWcfCallContext context, out object[] outs)
		{
			outs = new object[0];
			var message = context.CreateEndMessage();
			var returnMessage = context.ChannelHolder.RealProxy.Invoke(message) as IMethodReturnMessage;

			if (returnMessage.Exception != null)
			{
				throw returnMessage.Exception;
			}

			outs = returnMessage.OutArgs;
			return returnMessage;
		}

		public void EndCall(AsyncWcfCallContext context, out object[] outs)
		{
			CallEndMethod(context, out outs);
		}
	}
}