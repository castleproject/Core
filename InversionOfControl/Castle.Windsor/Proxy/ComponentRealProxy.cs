// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.Runtime.Remoting.Proxies;
	using System.Runtime.Remoting.Messaging;

	using Castle.Core.Interceptor;


	public class ComponentRealProxy : RealProxy
	{
		private readonly IMethodInterceptor[] interceptors;
		private readonly MarshalByRefObject target;

		public ComponentRealProxy(MarshalByRefObject target, Type classToProxy, IMethodInterceptor[] interceptors) : base(classToProxy)
		{
			this.target = target;
			this.interceptors = interceptors;
		}

		public override IMessage Invoke(IMessage msg)
		{
			IMessage returnMessage = null;

			if (msg is IMethodCallMessage)
			{
				IMethodCallMessage call = msg as IMethodCallMessage;

				MessageProxyInvocation inv = new MessageProxyInvocation( target, call, interceptors );

				try
				{
					object retval = inv.Proceed( call.Args );

					returnMessage = new ReturnMessage( retval, null, 0, call.LogicalCallContext, call );
				}
				catch(Exception ex)
				{
					returnMessage = new ReturnMessage(ex, call);
				}
			}
			else
			{
				throw new NotSupportedException("Message type not supported");
			}

			return returnMessage;
		}
	}
}