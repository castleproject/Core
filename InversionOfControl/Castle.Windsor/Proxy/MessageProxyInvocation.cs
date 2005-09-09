// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Proxy
{
	using System;
	using System.Reflection;
	using System.Runtime.Remoting;
	using System.Runtime.Remoting.Messaging;
	using System.Threading;

	using Castle.Model.Interceptor;


	public class MessageProxyInvocation : IMethodInvocation
	{
		private readonly MarshalByRefObject proxy;
		private readonly IMethodCallMessage methodCall;
		
		private IMethodInterceptor[] interceptorChain;
		private MethodInfo methodInfo;

		private int index = 0;
		private object changed_target;

		public MessageProxyInvocation(MarshalByRefObject target, IMethodCallMessage methodCall, IMethodInterceptor[] interceptorChain)
		{
			this.proxy = target;
			this.methodCall = methodCall;
			this.interceptorChain = interceptorChain;
		}

		/// <summary>
		/// The proxy instance.
		/// </summary>
		public object Proxy
		{
			get { return proxy; }
		}

		/// <summary>
		/// The target of this invocation, which usually is the
		/// instance of the class being proxy. 
		/// You might change the target, but be aware of the 
		/// implications.
		/// </summary>
		public object InvocationTarget
		{
			get { return changed_target != null ? changed_target : proxy; }
			set { changed_target = value; }
		}

		/// <summary>
		/// The Method being invoked.
		/// </summary>
		public MethodInfo Method
		{
			get
			{
				if (methodInfo == null)
				{
					methodInfo = (MethodInfo) methodCall.MethodBase;
				}
				return methodInfo;
			}
		}

		/// <summary>
		/// The Method being invoked on the target.
		/// </summary>
		public MethodInfo MethodInvocationTarget
		{
			get { return Method; }
		}

		/// <summary>
		/// The Proceed go on with the actual invocation.
		/// </summary>
		/// <param name="args">The arguments of the method</param>
		/// <returns></returns>
		public object Proceed(params object[] args)
		{
			int index = CurrentIndex;

			if (index < (interceptorChain.Length))
			{
				CurrentIndex = index + 1;
				return interceptorChain[index].Intercept(this, args);
			}
			else
			{
				// If the user changed the target, we use reflection
				// otherwise the delegate will be used.

				if (changed_target == null)
				{
					IMethodReturnMessage retMessage = RemotingServices.ExecuteMessage(proxy, methodCall);

					if (retMessage.Exception != null)
					{
						throw retMessage.Exception;
					}

					return retMessage.ReturnValue;
				}
				else
				{
					return Method.Invoke(changed_target, args);
				}
			}
		}

		private int CurrentIndex
		{
			get { return index; }
			set { index = value; }
		}
	}
}