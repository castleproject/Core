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

namespace Castle.Facilities.WcfIntegration.Async
{
	using System;
	using System.Threading;
	using Castle.Core.Interceptor;

	public abstract class AsyncWcfCallBase<TProxy> : IWcfAsyncBindings
	{
		private readonly TProxy proxy;
		private readonly Action<TProxy> onBegin;
		private readonly WcfRemotingAsyncInterceptor wcfAsyncInterceptor;
		private AsyncWcfCallContext context;
		protected object[] outArgs;
		protected object[] unboundOutArgs;

		public AsyncWcfCallBase(TProxy proxy, Action<TProxy> onBegin)
		{
			if (proxy == null)
			{
				throw new ArgumentNullException("proxy");
			}

			if (onBegin == null)
			{
				throw new ArgumentNullException("onBegin");
			}

			if ((proxy is IProxyTargetAccessor) == false)
			{
				throw new ArgumentException(
					"The given object is not a proxy created using Castle Dynamic Proxy library and is not supported.",
					"proxy");
			}

			this.proxy = proxy;
			this.onBegin = onBegin;
			wcfAsyncInterceptor = GetAsyncInterceptor(proxy);
		}

		public object[] OutArgs
		{
			get { return outArgs; }
		}

		public object[] UnboundOutArgs
		{
			get { return unboundOutArgs; }
		}

		public TOut GetOutArg<TOut>(int index)
		{
			return (TOut)ExtractOutOfType(typeof(TOut), index);
		}

		public TOut GetUnboundOutArg<TOut>(int index)
		{
			return (TOut)ExtractUnboundOutOfType(typeof(TOut), index);
		}

		public bool UseSynchronizationContext { get; set; }

		#region IAsyncResult Members

		public object AsyncState
		{
			get { return context.AsyncResult.AsyncState; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return context.AsyncResult.AsyncWaitHandle; }
		}

		public bool CompletedSynchronously
		{
			get { return context.AsyncResult.CompletedSynchronously; }
		}

		public bool IsCompleted
		{
			get { return context.AsyncResult.IsCompleted; }
		}

		#endregion

		private WcfRemotingAsyncInterceptor GetAsyncInterceptor(TProxy proxy)
		{
			var interceptors = Array.FindAll((proxy as IProxyTargetAccessor).GetInterceptors(),
											 i => i is WcfRemotingAsyncInterceptor);

			if (interceptors.Length <= 0)
			{
				throw new ArgumentException("This proxy does not support async calls.", "proxy");
			}

			if (interceptors.Length != 1)
			{
				throw new ArgumentException("This proxy has more than one WcfRemotingAsyncInterceptor.", "proxy");
			}

			return interceptors[0] as WcfRemotingAsyncInterceptor;
		}

		internal IAsyncResult Begin(AsyncCallback callback, object state)
		{
			context = wcfAsyncInterceptor.PrepareCall(WrapCallback(callback), state, proxy, GetDefaultReturnValue());
			onBegin(proxy);
			return context.AsyncResult;
		}

		private AsyncCallback WrapCallback(AsyncCallback callback)
		{
			var cb = callback;
			callback = ar => 
			{
				if (context != null)
					context.AsyncResult = ar;
				cb(ar);
			};

			var useSynchronizationContext = UseSynchronizationContext;
			var currentSynchronizationContext = SynchronizationContext.Current;
			if (callback == null || useSynchronizationContext == false || currentSynchronizationContext == null)
			{
				return callback;
			}
			return ar => currentSynchronizationContext.Post(o => callback(ar), null);
		}

		protected abstract object GetDefaultReturnValue();

		protected void End(Action<WcfRemotingAsyncInterceptor, AsyncWcfCallContext> action)
		{
			if (context == null)
			{
				throw new InvalidOperationException(
					"Something is wrong.  Either Begin threw an exception, or we have a bug here.  Please report it.");
			}

			if (context.AsyncResult == null)
			{
				throw new InvalidOperationException(
					"Something is wrong.  No AsyncResult is available.  This may be a bug so please report it.");
			}

			action(wcfAsyncInterceptor, context);
		}

		protected object ExtractOutOfType(Type type, int index)
		{
			return ExtractOutOfType(type, index, outArgs);
		}

		protected object ExtractUnboundOutOfType(Type type, int index)
		{
			return ExtractOutOfType(type, index, unboundOutArgs);
		}

		private object ExtractOutOfType(Type type, int index, object[] outArgs)
		{
			if (outArgs == null)
			{
				throw new InvalidOperationException("Out arguments are not available.  Did you forget to call End?");
			}

			if (index >= outArgs.Length)
			{
				throw new IndexOutOfRangeException(string.Format(
					"There is no out argument at index {0}.  Please check the method signature.", index));
			}

			object outArg = outArgs[index];

			if (outArg == null && type.IsValueType)
			{
				throw new InvalidOperationException(string.Format(
					"The out argument at index {0} is a value type and cannot be null.  Please check the method signature.", index));
			}

			if (!type.IsInstanceOfType(outArg))
			{
				throw new InvalidOperationException(string.Format(
					"There is a type mismatch for the out argument at index {0}.  Expected {1}, but found {2}.  Please check the method signature.",
						index, type.FullName, outArg.GetType().FullName));
			}

			return outArg;
		}

		protected void CreateUnusedOutArgs(int fromIndex)
		{
			unboundOutArgs = new object[Math.Max(0, outArgs.Length - fromIndex)];
			if (unboundOutArgs.Length > 0)
			{
				Array.Copy(outArgs, fromIndex, unboundOutArgs, 0, unboundOutArgs.Length);
			}
		}
	}
}