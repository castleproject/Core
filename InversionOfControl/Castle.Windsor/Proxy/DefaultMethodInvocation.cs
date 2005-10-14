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
	using System.Runtime.Remoting.Messaging;
	using System.Threading;

	using Castle.DynamicProxy;
	using Castle.DynamicProxy.Invocation;
	
	using Castle.Model.Interceptor;

	/// <summary>
	/// This implementation of <see cref="IMethodInvocation"/>
	/// holds an array of interceptors. When the Proceed method is invoked,
	/// it just increment the Current index and invoke the next interceptor. 
	/// </summary>
	/// <remarks>
	/// Although we have multithread test cases to ensure the correct 
	/// behavior, we might have threading synchronization issues.
	/// </remarks>
	public class DefaultMethodInvocation : AbstractInvocation, IMethodInvocation
	{
		// private static readonly LocalDataStoreSlot slot = Thread.AllocateDataSlot();

		private IMethodInterceptor[] interceptorChain;

		/// <summary>
		/// Constructs a DefaultMethodInvocation. This is invoked 
		/// by the DynamicProxy generated code.
		/// </summary>
		/// <param name="callable"></param>
		/// <param name="proxy"></param>
		/// <param name="method"></param>
		public DefaultMethodInvocation(ICallable callable, object proxy, MethodInfo method, object fieldTarget) : base(callable, proxy, method, fieldTarget)
		{
		}

		public override object Proceed(params object[] args)
		{
			// In a multithreaded application, the Proceed on the same 
			// invocation instance can be called at the same time

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
					return callable.Call(args);
				}
				else
				{
					return Method.Invoke(changed_target, args);
				}
			}
		}

		private int CurrentIndex
		{
			get 
			{ 
				object index = CallContext.GetData("interceptor_index");
				if (index == null) return 0;
				return (int) index;
			}
			set
			{
				CallContext.SetData("interceptor_index", value);
			}
		}

		internal void Reset()
		{
			CurrentIndex = 0;
		}

		internal bool IsInitialized
		{
			get { return interceptorChain != null; }
		}

		internal IMethodInterceptor[] InterceptorChain
		{
			set { interceptorChain = value; }
		}
	}
}