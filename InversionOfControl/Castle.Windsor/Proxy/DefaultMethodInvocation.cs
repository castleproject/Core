// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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
	using System.Collections;
	using System.Reflection;
	using System.Threading;

	using Castle.DynamicProxy;
	using Castle.Model.Interceptor;

	/// <summary>
	/// Summary description for DefaultMethodInvocation.
	/// </summary>
	public sealed class DefaultMethodInvocation : IMethodInvocation
	{
		private static readonly LocalDataStoreSlot _slot = Thread.AllocateDataSlot();
//		private static ReaderWriterLock _locker = new ReaderWriterLock();

		private ICallable _callable;
		private MethodInfo _method;
		private object _proxy;
		private object _target;
		private object _original_target;
		private IMethodInterceptor[] _interceptorChain;

		private object key = new object();

		public DefaultMethodInvocation(ICallable callable, object proxy, MethodInfo method)
		{
			_callable = callable;
			_proxy = proxy; 
			_method = method;
			_target = _original_target = callable.Target;
		}

		#region IMethodInvocation Members

		public object Proxy
		{
			get { return _proxy; }
		}

		public object InvocationTarget
		{
			get { return _target; }
			set { _target = value; }
		}

		public MethodInfo Method
		{
			get { return _method; }
		}

		public object Proceed(params object[] args)
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

		#endregion

		private int CurrentIndex
		{
			get
			{
				return (int) Thread.GetData(_slot);
			}
			set
			{
				Thread.SetData(_slot, value);
			}
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