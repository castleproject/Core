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
		/// Gets the output values.
		/// </summary>
		public object[] OutValues
		{
			get { return result.OutValues; }
		}

		/// <summary>
		/// Gets the unbound output values.
		/// </summary>
		public object[] UnboundOutValues
		{
			get { return result.UnboundOutValues; }
		}

		/// <summary>
		///  Waits for the result to complete.
		/// </summary>
		/// <returns>The result value.</returns>
		public T End()
		{
			return (T)AbstractAsyncResult.End(result);
		}

		/// <summary>
		/// Waits for the result to complete.
		/// </summary>
		/// <param name="result">The asynchronous result.</param>
		/// <returns>The result value.</returns>
		public static T End(IAsyncResult result)
		{
			return new Result<T>(Result.EnsureResult(result)).End();
		}

		/// <summary>
		/// Waits for the result to complete with output.
		/// </summary>
		/// <typeparam name="TOut1">The output type.</typeparam>
		/// <param name="out1">The output value.</param>
		/// <returns>The result value.</returns>
		public T End<TOut1>(out TOut1 out1)
		{
			T returnValue = End();
			out1 = (TOut1)result.ExtractOutOfType(typeof(TOut1), 0);
			result.CreateUnboundOutValues(1);
			return returnValue;
		}

		/// <summary>
		/// Waits for the result to complete with outputs.
		/// </summary>
		/// <typeparam name="TOut1">The output type.</typeparam>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The output value.</param>
		/// <returns>The result value.</returns>
		public static T End<TOut1>(IAsyncResult result, out TOut1 out1)
		{
			return new Result<T>(Result.EnsureResult(result)).End(out out1);
		}

		/// <summary>
		///  Waits for the result to complete with outputs.
		/// </summary>
		/// <typeparam name="TOut1">The first output type.</typeparam>
		/// <typeparam name="TOut2">The first output type.</typeparam>
		/// <param name="out1">The first output type.</param>
		/// <param name="out2">The second output value.</param>
		/// <returns>The result value.</returns>
		public T End<TOut1, TOut2>(out TOut1 out1, out TOut2 out2)
		{
			T returnValue = End();
			out1 = (TOut1)result.ExtractOutOfType(typeof(TOut1), 0);
			out2 = (TOut2)result.ExtractOutOfType(typeof(TOut2), 1);
			result.CreateUnboundOutValues(2);
			return returnValue;
		}

		/// <summary>
		/// Waits for the result to complete with outputs.
		/// </summary>
		/// <typeparam name="TOut1">The first output type.</typeparam>
		/// <typeparam name="TOut2">The first output type.</typeparam>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The first output type.</param>
		/// <param name="out2">The second output value.</param>
		/// <returns>The result value.</returns>
		public static T End<TOut1, TOut2>(IAsyncResult result, out TOut1 out1, out TOut2 out2)
		{
			return new Result<T>(Result.EnsureResult(result)).End(out out1, out out2);
		}

		/// <summary>
		/// Waits for the result to complete with outputs.
		/// </summary>
		/// <typeparam name="TOut1">The first output type.</typeparam>
		/// <typeparam name="TOut2">The second output type.</typeparam>
		/// <typeparam name="TOut3">The third output type.</typeparam>
		/// <param name="out1">The first output type.</param>
		/// <param name="out2">The second output type.</param>
		/// <param name="out3">The third output type.</param>
		/// <returns>The result value.</returns>
		public T End<TOut1, TOut2, TOut3>(out TOut1 out1, out TOut2 out2, out TOut3 out3)
		{
			T returnValue = End();
			out1 = (TOut1)result.ExtractOutOfType(typeof(TOut1), 0);
			out2 = (TOut2)result.ExtractOutOfType(typeof(TOut2), 1);
			out3 = (TOut3)result.ExtractOutOfType(typeof(TOut3), 2);
			result.CreateUnboundOutValues(3);
			return returnValue;
		}

		/// <summary>
		/// Waits for the result to complete with outputs.
		/// </summary>
		/// <typeparam name="TOut1">The first output type.</typeparam>
		/// <typeparam name="TOut2">The second output type.</typeparam>
		/// <typeparam name="TOut3">The third output type.</typeparam>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The first output type.</param>
		/// <param name="out2">The second output type.</param>
		/// <param name="out3">The third output type.</param>
		/// <returns>The result value.</returns>
		public static T End<TOut1, TOut2, TOut3>(IAsyncResult result, out TOut1 out1, out TOut2 out2, out TOut3 out3)
		{
			return new Result<T>(Result.EnsureResult(result)).End(out out1, out out2, out out3);
		}

		/// <summary>
		/// Waits for the result to complete with outputs.
		/// </summary>
		/// <typeparam name="TOut1">The first output type.</typeparam>
		/// <typeparam name="TOut2">The second output type.</typeparam>
		/// <typeparam name="TOut3">The third output type.</typeparam>
		/// <typeparam name="TOut4">The fourth output type.</typeparam>
		/// <param name="out1">The first output type.</param>
		/// <param name="out2">The second output type.</param>
		/// <param name="out3">The third output type.</param>
		/// <param name="out4">The fourth output value</param>
		/// <returns>The result value.</returns>
		public T End<TOut1, TOut2, TOut3, TOut4>(out TOut1 out1, out TOut2 out2, out TOut3 out3, out TOut4 out4)
		{
			T returnValue = End();
			out1 = (TOut1)result.ExtractOutOfType(typeof(TOut1), 0);
			out2 = (TOut2)result.ExtractOutOfType(typeof(TOut2), 1);
			out3 = (TOut3)result.ExtractOutOfType(typeof(TOut3), 2);
			out4 = (TOut4)result.ExtractOutOfType(typeof(TOut4), 3);
			result.CreateUnboundOutValues(4);
			return returnValue;
		}

		/// <summary>
		/// Waits for the result to complete with outputs.
		/// </summary>
		/// <typeparam name="TOut1">The first output type.</typeparam>
		/// <typeparam name="TOut2">The second output type.</typeparam>
		/// <typeparam name="TOut3">The third output type.</typeparam>
		/// <typeparam name="TOut4">The fourth output type.</typeparam>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The first output type.</param>
		/// <param name="out2">The second output type.</param>
		/// <param name="out3">The third output type.</param>
		/// <param name="out4">The fourth output value</param>
		/// <returns>The result value.</returns>
		public T End<TOut1, TOut2, TOut3, TOut4>(IAsyncResult result, out TOut1 out1, out TOut2 out2, 
												 out TOut3 out3, out TOut4 out4)
		{
			return new Result<T>(Result.EnsureResult(result)).End(out out1, out out2, out out3, out out4);
		}

		/// <summary>
		/// Gets the output argument at index <paramref name="index"/>.
		/// </summary>
		/// <typeparam name="TOut">The output type..</typeparam>
		/// <param name="index">The output index.</param>
		/// <returns>The output value.</returns>
		public TOut GetOutArg<TOut>(int index)
		{
			return result.GetOutArg<TOut>(index);
		}

		/// <summary>
		/// Gets the unbound output argument at index <paramref name="index"/>.
		/// </summary>
		/// <typeparam name="TOut">The output type.</typeparam>
		/// <param name="index">The output index.</param>
		/// <returns>The output value.</returns>
		public TOut GetUnboundOutArg<TOut>(int index)
		{
			return result.GetUnboundOutArg<TOut>(index);
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

		internal void SetCallbackInfo(AsyncCallback callback, object state)
		{
			result.SetCallbackInfo(callback, state);
		}
	}
}
