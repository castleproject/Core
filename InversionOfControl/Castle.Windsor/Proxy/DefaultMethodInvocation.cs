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
	public class DefaultMethodInvocation : SameClassInvocation, IMethodInvocation
	{
		private static readonly LocalDataStoreSlot _slot = Thread.AllocateDataSlot();

		private IMethodInterceptor[] _interceptorChain;

		private object key = new object();

		/// <summary>
		/// Constructs a DefaultMethodInvocation. This is invoked 
		/// by the DynamicProxy generated code.
		/// </summary>
		/// <param name="callable"></param>
		/// <param name="proxy"></param>
		/// <param name="method"></param>
		public DefaultMethodInvocation(ICallable callable, object proxy, MethodInfo method) : base(callable, proxy, method)
		{
		}

		public override object Proceed(params object[] args)
		{
			// In a multithreaded application, the Proceed on the same 
			// invocation instance can be called at the same time

			// TODO: Keep the index on the threadslot or something similar
			// - we can't lock this execution, though

			int index = CurrentIndex;

			if (index < (_interceptorChain.Length))
			{
				CurrentIndex = index + 1;
				return _interceptorChain[index].Intercept(this, args);
			}
			else
			{
				// If the user changed the target, we use reflection
				// otherwise the delegate will be used.
				if (InvocationTarget == _original_target)
				{
					return _callable.Call(args);
				}
				else
				{
					return Method.Invoke(InvocationTarget, args);
				}
			}
		}

		private int CurrentIndex
		{
			get { return (int) Thread.GetData(_slot); }
			set { Thread.SetData(_slot, value); }
		}

		internal void Reset()
		{
			CurrentIndex = 0;
		}

		internal bool IsInitialized
		{
			get { return _interceptorChain != null; }
		}

		internal IMethodInterceptor[] InterceptorChain
		{
			set { _interceptorChain = value; }
		}
		}
}