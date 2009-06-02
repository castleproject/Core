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
using System.Collections.Generic;

	/// <summary>
	/// Represents the result of an asynchronous operation.
	/// </summary>
	public class Result : AbstractAsyncResult
	{
		private object value;
		private ResultDelegate resultCallback;
		private object guard = new object();

		[ThreadStatic]
		internal static Result Last = null;

		/// <summary>
		/// Initializes the <see cref="Result"/>.
		/// </summary>
		internal Result() : base(null, null)
		{
			value = guard;
		}

		/// <summary>
		/// Initializes the <see cref="Result"/> with value.
		/// </summary>
		/// <param name="value">The result value.</param>
		internal Result(object value) : this()
		{
			SetValue(true, value);
		}

		/// <summary>
		/// Gets the result value, blocking if not available yet.
		/// </summary>
		public object Value
		{
			get 
			{
				Interlocked.CompareExchange(ref value, End(this) , guard);
				return value;
			}
		}

		/// <summary>
		/// Completes the asynchronous request.
		/// </summary>
		/// <param name="synchronously">true if synchronously.</param>
		/// <param name="result">The result.</param>
		internal void SetValue(bool synchronously, object result)
		{
			lock (guard)
			{
				Complete(synchronously, result);

				if (resultCallback != null)
				{
					resultCallback(this);
				}
			}
		}

		/// <summary>
		/// Completes the asynchronous request with exception.
		/// </summary>
		/// <param name="synchronously">true if synchronously.</param>
		/// <param name="exception">The exception.</param>
		internal void SetException(bool synchronously, Exception exception)
		{
			lock (guard)
			{
				Complete(synchronously, exception);

				if (resultCallback != null)
				{
					resultCallback(this);
				}
			}
		}

		/// <summary>
		/// Set the asynchronous callback information.
		/// </summary>
		/// <param name="callback">The callback.</param>
		/// <param name="state">The async state.</param>
		internal void SetCallbackInfo(ResultDelegate callback, object state)
		{
			if (callback != null) lock (guard)
			{
				AsyncState = state;
				resultCallback = callback;

				if (IsCompleted)
				{
					callback(this);
				}
			}
		}

		private IEnumerable<object> GetValue()
		{
			object value = End(this);
			while (true) yield return value;
		}

		/// <summary>
		/// Gets the result of the last called made.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <returns>The result handle.</returns>
		public static Result Of(Action action)
		{
			action();
			return ResetLastResult();
		}

		/// <summary>
		/// Gets the result of the last called made.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <param name="callback">The callback.</param>
		/// <param name="state">The async state.</param>
		/// <returns>The result handle.</returns>
		public static Result Of(Action action, ResultDelegate callback, object state)
		{
			Result result = Result.Of(action);
			result.SetCallbackInfo(callback, state);
			return result;
		}

		/// <summary>
		/// Gets the result of the last called made.
		/// </summary>
		/// <typeparam name="T">The result type.</typeparam>
		/// <param name="ignored"></param>
		/// <returns>The result handle.</returns>
		public static Result<T> Of<T>(T ignored)
		{
			return new Result<T>(ResetLastResult());
		}

		/// <summary>
		/// Gets the result of the last called made.
		/// </summary>
		/// <typeparam name="T">The result type.</typeparam>
		/// <param name="ignored"></param>
		/// <param name="callback">The callback.</param>
		/// <param name="state">The async state.</param>
		/// <returns>The result handle.</returns>
		public static Result<T> Of<T>(T ignored, ResultDelegate<T> callback, object state)
		{
			Result<T> result = Result.Of(ignored);
			result.SetCallbackInfo(callback, state);
			return result;
		}

		private static Result ResetLastResult()
		{
			Result last = Result.Last;

			if (last == null)
			{
				throw new InvalidOperationException(
					"The result is only available for synchronized methods.");
			}

			Result.Last = null;
			return last;
		}
	}

	/// <summary>
	/// Delegate called when results are available.
	/// </summary>
	/// <param name="result">The result.</param>
	public delegate void ResultDelegate(Result result);

	/// <summary>
	/// Delegate called when typed results are available.
	/// </summary>
	/// <typeparam name="T">The result type.</typeparam>
	/// <param name="result">The result.</param>
	public delegate void ResultDelegate<T>(Result<T> result);

	#region Typed Result

	/// <summary>
	/// Represents the typed result of an asynchronous operation.
	/// </summary>
	/// <typeparam name="T">The result type.</typeparam>
	public class Result<T> : IAsyncResult
	{
		private readonly Result result;

		internal Result(Result result)
		{
			this.result = result;
		}

		/// <summary>
		/// Gets the result value, blocking if not available yet.
		/// </summary>
		public T Value
		{
			get { return (T) result.Value; }
		}

		/// <summary>
		/// Set the asynchronous callback information.
		/// </summary>
		/// <param name="callback">The callback.</param>
		/// <param name="state">The async state.</param>
		internal void SetCallbackInfo(ResultDelegate<T> callback, object state)
		{
			if (callback != null)
			{
				result.SetCallbackInfo(delegate { callback(this); }, state);
			}
		}

		/// <summary>
		/// Provide implicit conversion to the actual value.
		/// </summary>
		/// <param name="result">The result holder.</param>
		/// <returns>The actual result value.</returns>
		public static implicit operator T(Result<T> result)
		{
			return result != null ? result.Value : default(T);
		}

		#region IAsyncResult Members

		/// <summary>
		/// Gets the asynchournous state.
		/// </summary>
		public object AsyncState
		{
			get { return result.AsyncState; }
		}

		/// <summary>
		/// Gets the asynchronous <see cref="WaitHandle"/>.
		/// </summary>
		public WaitHandle AsyncWaitHandle
		{
			get { return result.AsyncWaitHandle; }
		}

		/// <summary>
		/// Determines if the result completed synchronously.
		/// </summary>
		public bool CompletedSynchronously
		{
			get { return result.CompletedSynchronously; }
		}

		/// <summary>
		/// Determines if the result is available.
		/// </summary>
		public bool IsCompleted
		{
			get { return result.IsCompleted; }
		}

		#endregion
	}

	#endregion
}
