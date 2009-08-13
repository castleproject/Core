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

	/// <summary>
	/// Represents the result of an asynchronous operation.
	/// </summary>
	public class Result : AbstractAsyncResult
	{
		private object guard = new object();
		private AsyncCallback asyncCallback;

		[ThreadStatic] internal static Result Last = null;

		/// <summary>
		/// Initializes the <see cref="Result"/>.
		/// </summary>
		internal Result() 
			: base(CallCallback, null)
		{
		}

		/// <summary>
		/// Gets the output values.
		/// </summary>
		public object[] OutValues { get; private set; }

		/// <summary>
		/// Gets the unbound output values.
		/// </summary>
		public object[] UnboundOutValues { get; private set; }

		/// <summary>
		/// Waits for the result to complete.
		/// </summary>
		public void End()
		{
			Result.End(this);
		}

		/// <summary>
		/// Waits for the result to complete with output.
		/// </summary>
		/// <typeparam name="TOut1">The output type.</typeparam>
		/// <param name="out1">The output value.</param>
		public void End<TOut1>(out TOut1 out1)
		{
			End();
			out1 = (TOut1)ExtractOutOfType(typeof(TOut1), 0);
			CreateUnboundOutValues(1);
		}

		/// <summary>
		/// Waits for the result to complete with outputs.
		/// </summary>
		/// <typeparam name="TOut1">The output type.</typeparam>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The output value.</param>
		public static void End<TOut1>(IAsyncResult result, out TOut1 out1)
		{
			EnsureResult(result).End(out out1);
		}

		/// <summary>
		///  Waits for the result to complete with outputs.
		/// </summary>
		/// <typeparam name="TOut1">The first output type.</typeparam>
		/// <typeparam name="TOut2">The first output type.</typeparam>
		/// <param name="out1">The first output type.</param>
		/// <param name="out2">The second output value.</param>
		public void End<TOut1, TOut2>(out TOut1 out1, out TOut2 out2)
		{
			End();
			out1 = (TOut1)ExtractOutOfType(typeof(TOut1), 0);
			out2 = (TOut2)ExtractOutOfType(typeof(TOut2), 1);
			CreateUnboundOutValues(2);
		}

		/// <summary>
		/// Waits for the result to complete with outputs.
		/// </summary>
		/// <typeparam name="TOut1">The first output type.</typeparam>
		/// <typeparam name="TOut2">The first output type.</typeparam>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The first output type.</param>
		/// <param name="out2">The second output value.</param>
		public static void End<TOut1, TOut2>(IAsyncResult result, out TOut1 out1, out TOut2 out2)
		{
			EnsureResult(result).End(out out1, out out2);
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
		public void End<TOut1, TOut2, TOut3>(out TOut1 out1, out TOut2 out2, out TOut3 out3)
		{
			End();
			out1 = (TOut1)ExtractOutOfType(typeof(TOut1), 0);
			out2 = (TOut2)ExtractOutOfType(typeof(TOut2), 1);
			out3 = (TOut3)ExtractOutOfType(typeof(TOut3), 2);
			CreateUnboundOutValues(3);
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
		public static void End<TOut1, TOut2, TOut3>(IAsyncResult result, out TOut1 out1, out TOut2 out2, out TOut3 out3)
		{
			EnsureResult(result).End(out out1, out out2, out out3);
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
		public void End<TOut1, TOut2, TOut3, TOut4>(out TOut1 out1, out TOut2 out2, out TOut3 out3, out TOut4 out4)
		{
			End();
			out1 = (TOut1)ExtractOutOfType(typeof(TOut1), 0);
			out2 = (TOut2)ExtractOutOfType(typeof(TOut2), 1);
			out3 = (TOut3)ExtractOutOfType(typeof(TOut3), 2);
			out4 = (TOut4)ExtractOutOfType(typeof(TOut4), 3);
			CreateUnboundOutValues(4);
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
		public void End<TOut1, TOut2, TOut3, TOut4>(IAsyncResult result, out TOut1 out1, out TOut2 out2, 
													out TOut3 out3, out TOut4 out4)
		{
			EnsureResult(result).End(out out1, out out2, out out3, out out4);
		}

		/// <summary>
		/// Gets the output argument at index <paramref name="index"/>.
		/// </summary>
		/// <typeparam name="TOut">The output type..</typeparam>
		/// <param name="index">The output index.</param>
		/// <returns>The output value.</returns>
		public TOut GetOutArg<TOut>(int index)
		{
			return (TOut)ExtractOutOfType(typeof(TOut), index);
		}

		/// <summary>
		/// Gets the unbound output argument at index <paramref name="index"/>.
		/// </summary>
		/// <typeparam name="TOut">The output type.</typeparam>
		/// <param name="index">The output index.</param>
		/// <returns>The output value.</returns>
		public TOut GetUnboundOutArg<TOut>(int index)
		{
			return (TOut)ExtractUnboundOutOfType(typeof(TOut), index);
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
		public static Result Of(Action action, AsyncCallback callback, object state)
		{
			var result = Result.Of(action);
			result.SetCallbackInfo(callback, state);
			return result;
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
			var result = Result.Of(action);
			result.SetCallbackInfo((IAsyncResult cb) => callback(result), state);
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
		public static Result<T> Of<T>(T ignored, AsyncCallback callback, object state)
		{
			var result = Result.Of(ignored);
			result.SetCallbackInfo(callback, state);
			return result;
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
			var result = Result.Of(ignored);
			result.SetCallbackInfo((IAsyncResult cb) => callback(result), state);
			return result;
		}

		/// <summary>
		/// Completes the asynchronous request.
		/// </summary>
		/// <param name="synchronously">true if synchronously.</param>
		/// <param name="result">The result.</param>
		/// <param name="outs">The additional outputs.</param>
		internal void SetValues(bool synchronously, object result, object[] outs)
		{
			lock (guard)
			{
				OutValues = outs;
				Complete(synchronously, result);
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
			}
		}

		/// <summary>
		/// Set the asynchronous callback information.
		/// </summary>
		/// <param name="callback">The callback.</param>
		/// <param name="state">The async state.</param>
		internal void SetCallbackInfo(AsyncCallback callback, object state)
		{
			if (callback != null) lock (guard)
			{
				AsyncState = state;
				asyncCallback = callback;

				if (IsCompleted)
				{
					asyncCallback(this);
				}
			}
		}

		private static void CallCallback(IAsyncResult asyncResult)
		{
			var result = (Result)asyncResult;
			if (result.asyncCallback != null)
			{
				result.asyncCallback(asyncResult);
			}
		}

		private static Result ResetLastResult()
		{
			var last = Result.Last;

			if (last == null)
			{
				throw new InvalidOperationException(
					"The result is only available for synchronized methods.");
			}

			Result.Last = null;
			return last;
		}

		/// <summary>
		/// Extracts the output at index <paramref name="index"/>
		/// </summary>
		/// <param name="type">The expected type.</param>
		/// <param name="index">The output index.</param>
		/// <returns>The extracted output.</returns>
		protected internal object ExtractOutOfType(Type type, int index)
		{
			return ExtractOutOfType(type, index, OutValues);
		}

		/// <summary>
		///  Extracts the unbound output at index <paramref name="index"/>
		/// </summary>
		/// <param name="type">The expected type.</param>
		/// <param name="index">The output index.</param>
		/// <returns>The extracted outout.</returns>
		protected internal object ExtractUnboundOutOfType(Type type, int index)
		{
			return ExtractOutOfType(type, index, UnboundOutValues);
		}

		private object ExtractOutOfType(Type type, int index, object[] outs)
		{
			if (outs == null)
			{
				throw new InvalidOperationException("Out arguments are not available.  Did you forget to call End?");
			}

			if (index >= outs.Length)
			{
				throw new IndexOutOfRangeException(string.Format(
					"There is no out argument at index {0}.  Please check the method signature.", index));
			}

			object outArg = outs[index];

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

		/// <summary>
		/// Creates the unbound output values starting at <paramref name="fromIndex"/>.
		/// </summary>
		/// <param name="fromIndex">The starting index.</param>
		protected internal void CreateUnboundOutValues(int fromIndex)
		{
			UnboundOutValues = new object[Math.Max(0, OutValues.Length - fromIndex)];
			if (UnboundOutValues.Length > 0)
			{
				Array.Copy(OutValues, fromIndex, UnboundOutValues, 0, UnboundOutValues.Length);
			}
		}

		internal static Result EnsureResult(IAsyncResult asyncResult)
		{
			var result = asyncResult as Result;
			if (result == null)
			{
				throw new InvalidOperationException("Unrecoginized IAsyncResult, was this from a call to Result.Of()?");
			}
			return result;
		}
	}
}
