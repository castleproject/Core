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

namespace Castle.Facilities.Synchronize
{
	using System;
	using System.Threading;

	/// <summary>
	/// Abstract result for asynchronous operations.
	/// </summary>
	public abstract class AbstractAsyncResult : IAsyncResult
	{
		private int cleanUp;
		private int completed;
		private int invokedCallback;
		private readonly AsyncCallback callback;
		private bool completedSynchronously;
		private bool endCalled;
		private Timeout timeout;
		private Exception exception;
		private object result;
		private object waitEvent;

		/// <summary>
		/// Initializes a new <see cref="AbstractAsyncResult"/>
		/// </summary>
		/// <param name="callback">The async callback.</param>
		/// <param name="state">The async state</param>
		protected AbstractAsyncResult(AsyncCallback callback, object state)
		{
			AsyncState = state;
			this.callback = callback;
			invokedCallback = (callback != null) ? 0 : 1;
		}

		#region IAsyncResult Members

		/// <summary>
		/// Gets the asynchournous state.
		/// </summary>
		public object AsyncState { get; protected set; }

		/// <summary>
		/// Determines if the result is available.
		/// </summary>
		public bool IsCompleted
		{
			get { return (completed != 0); }
		}

		/// <summary>
		/// Determines if the result completed synchronously.
		/// </summary>
		public bool CompletedSynchronously
		{
			get { return completedSynchronously; }
			protected internal set
            {
            	 completedSynchronously = value;
            }
		}

		/// <summary>
		/// Gets the asynchronous <see cref="WaitHandle"/>.
		/// </summary>
		public WaitHandle AsyncWaitHandle
		{
			get
			{
				int isCompleted = completed;

				if (waitEvent == null)
				{
					Interlocked.CompareExchange(ref waitEvent,
						new ManualResetEvent(isCompleted != 0), (object)null);
				}

				var ev = (ManualResetEvent)waitEvent;

				if ((isCompleted == 0) && (completed != 0))
				{
					ev.Set();
				}

				return ev;
			}
		}

		/// <summary>
		/// Establishes an async timeout for the interval.
		/// </summary>
		/// <param name="interval">The timeout interval without units.</param>
		/// <returns>The timeout specification.</returns>
		public Timeout TimeoutAfter(int interval)
		{
			lock (this)
			{
				if (timeout != null)
				{
					throw new InvalidOperationException(
						"A timeout has already been established");
				}
				timeout = new Timeout(this, interval);
			}
			return timeout;
		}

		/// <summary>
		/// Ends the asynchronous request.
		/// </summary>
		/// <param name="asyncResult">The asynchronous result.</param>
		/// <returns>The result.</returns>
		public static object End(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}

			var result = asyncResult as AbstractAsyncResult;

			if (result == null)
			{
				throw new ArgumentException("Unrecognized IAsyncResult", "result");
			}

			if (result.endCalled)
			{
				throw new InvalidOperationException("IAsyncResult has already ended");
			}

			result.endCalled = true;

			if (result.completed == 0)
			{
				result.AsyncWaitHandle.WaitOne();
			}

			if (result.exception != null)
			{
				throw result.exception;
			}

			return result.result;
		}

		#endregion

		/// <summary>
		/// Completes the asynchronous request.
		/// </summary>
		/// <param name="synchronously">true if synchronously.</param>
		protected void Complete(bool synchronously)
		{
			completedSynchronously = synchronously;

			Interlocked.Increment(ref completed);

			if (waitEvent != null)
			{
				((ManualResetEvent)waitEvent).Set();
			}

			if ((callback != null) && (Interlocked.Increment(ref invokedCallback) == 1))
			{
				callback(this);
			}

			InternalCleanup();
		}

		/// <summary>
		/// Completes the asynchronous request.
		/// </summary>
		/// <param name="synchronously">true if synchronously.</param>
		/// <param name="result">The result.</param>
		protected void Complete(bool synchronously, object result)
		{
			this.result = result;
			Complete(synchronously);
		}

		/// <summary>
		/// Completes the asynchronous request with exception.
		/// </summary>
		/// <param name="synchronously">true if synchronously.</param>
		/// <param name="exception">The exception.</param>
		protected void Complete(bool synchronously, Exception exception)
		{
			this.exception = exception;
			Complete(synchronously);
		}

		/// <summary>
		/// Performs any behavior when a timeout occurs.
		/// </summary>
		protected virtual void OnTimeout()
		{
			Complete(false, new TimeoutException());
		}

		/// <summary>
		/// Performs any cleanup.
		/// </summary>
		protected virtual void Cleanup()
		{
		}

		private void InternalCleanup()
		{
			if (Interlocked.Increment(ref cleanUp) == 1)
			{
				Cleanup();
			}
		}

		/// <summary>
		/// Finalizer to ensure cleanup.
		/// </summary>
		~AbstractAsyncResult()
		{
			InternalCleanup();
		}

		#region Nested Class: Timeout

		/// <summary>
		/// Represents the timeout description.
		/// </summary>
		public class Timeout
		{
			private readonly int interval;
			private readonly AbstractAsyncResult result;

			/// <summary>
			/// Constructs a new <see cref="Timeout"/>
			/// </summary>
			/// <param name="result">The async result.</param>
			/// <param name="interval">The timeout interval.</param>
			public Timeout(AbstractAsyncResult result, int interval)
			{
				this.result = result;
				this.interval = interval;
			}

			/// <summary>
			/// Registers the timeout in milliseconds.
			/// </summary>
			/// <returns></returns>
			public AbstractAsyncResult MilliSeconds()
			{
				RegisterTimeout(interval);
				return result;
			}

			/// <summary>
			/// Registers the timeout in seconds. 
			/// </summary>
			/// <returns></returns>
			public AbstractAsyncResult Seconds()
			{
				RegisterTimeout(interval * 1000);
				return result;
			}

			/// <summary>
			/// Registers the timeout in minutes.
			/// </summary>
			/// <returns></returns>
			public AbstractAsyncResult Minutes()
			{
				RegisterTimeout(interval * 60 * 1000);
				return result;
			}

			private void RegisterTimeout(int milliInterval)
			{
				ThreadPool.RegisterWaitForSingleObject(
					result.AsyncWaitHandle, OnCompleteOrTimeout, null,
					milliInterval, true);
			}

			private void OnCompleteOrTimeout(object state, bool timedOut)
			{
				if (timedOut)
				{
					result.OnTimeout();
				}
			}
		}

		#endregion
	}
}
