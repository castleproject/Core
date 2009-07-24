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
	using Castle.Facilities.WcfIntegration.Async.TypeSystem;
	using Castle.Facilities.WcfIntegration.Proxy;

	public class AsyncWcfCallContext
	{
		private readonly AsyncCallback callback;
		private readonly object state;
		private readonly AsyncType asyncType;
		private readonly IWcfChannelHolder channelHolder;
		private readonly object result;
		private BeginMethod beginMethod;
		private EndMethod endMethod;
		private object[] syncArguments;
		private IAsyncResult asyncResult;
		private bool initCalled;

		public IAsyncResult AsyncResult
		{
			get { return asyncResult; }
		}

		public AsyncWcfCallContext(AsyncCallback callback, object state,  AsyncType asyncType,
								   IWcfChannelHolder proxyHolder, object result)
		{
			this.callback = callback;
			this.state = state;
			this.asyncType = asyncType;
			this.channelHolder = proxyHolder;
			this.result = result;
		}

		public IWcfChannelHolder ChannelHolder
		{
			get { return channelHolder; }
		}

		public EndMethod EndMethod
		{
			get { return endMethod; }
		}

		public void Init(MethodInfo method, object[] arguments)
		{
			if (initCalled)
			{
				throw new InvalidOperationException("Init can only be called once.");
			}

			initCalled = true;
			beginMethod = asyncType.GetBeginMethod(method);
			endMethod = asyncType.GetEndMethod(method);
			syncArguments = arguments;
		}

		public MethodCallMessage CreateBeginMessage()
		{
			if (initCalled == false)
			{
				throw new InvalidOperationException("Context not initialized.  Call Init first.");
			}

			var arguments = BuildAsyncBeginArgumentsList();
			return new MethodCallMessage(beginMethod, arguments);
		}

		private object[] BuildAsyncBeginArgumentsList()
		{
			var arguments = new object[syncArguments.Length + 2];
			syncArguments.CopyTo(arguments, 0);
			arguments[arguments.Length - 2] = callback;
			arguments[arguments.Length - 1] = state;
			return arguments;
		}

		public MethodCallMessage CreateEndMessage()
		{
			if (initCalled == false)
			{
				throw new InvalidOperationException("Context not initialized. Call Init first.");
			}

			var arguments = BuildAsyncEndArgumentsList();
			return new MethodCallMessage(endMethod, arguments);
		}

		private object[] BuildAsyncEndArgumentsList()
		{
			object[] arguments = new object[endMethod.GetParameters().Length];
			arguments[arguments.Length - 1] = asyncResult;
			return arguments;
		}

		public object PostProcess(IMethodReturnMessage message)
		{
			if (initCalled == false)
			{
				throw new InvalidOperationException("Context not initialized. Call Init first.");
			}

			if (message != null)
			{
				if (message.Exception != null)
				{
					throw message.Exception;
				}

				var returnValue = message.ReturnValue as IAsyncResult;

				if (returnValue == null)
				{
					throw new InvalidOperationException("Return value of the message is not of type IAsyncResult. " +
						"This indicate it's not a result of async operation. This may also be a bug, so if you think it is, please report it.");
				}

				asyncResult = returnValue;
			}

			return result;
		}
	}
}