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

namespace Castle.Facilities.WcfIntegration
{
	using System;
	using Castle.Facilities.WcfIntegration.Async;

	public static class WcfAsync
	{
		private static readonly AsyncCallback Nothing = delegate { };

		/// <summary>
		/// Begins an asynchronous call of <paramref name="method"/> on given <paramref name="proxy"/>.
		/// </summary>
		/// <typeparam name="TProxy">The type of the proxy.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="method">The delegate encapsulating the invocation of the method.</param>
		/// <returns>The async call handle.</returns>
		public static IWcfAsyncCall<TResult> BeginWcfCall<TProxy, TResult>(
			this TProxy proxy, Func<TProxy, TResult> method)
		{
			return BeginWcfCall(proxy, method, Nothing, null);
		}

		/// <summary>
		/// Begins an asynchronous call of <paramref name="method"/> on given <paramref name="proxy"/>.
		/// </summary>
		/// <typeparam name="TProxy">The type of the proxy.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="method">The delegate encapsulating the invocation of the method.</param>
		/// <param name="callback">The asynchronous callback.</param>
		/// <param name="state">The asynchronous state.</param>
		/// <returns>The async call handle.</returns>
		public static IWcfAsyncCall<TResult> BeginWcfCall<TProxy, TResult>(
			this TProxy proxy, Func<TProxy, TResult> method,
			Action<IWcfAsyncCall<TResult>> callback, object state)
		{
			var call = new AsyncWcfCall<TProxy, TResult>(proxy, method);
			call.Begin(ar => callback(call), state);
			return call;
		}

		/// <summary>
		/// Begins an asynchronous call of <paramref name="method"/> on given <paramref name="proxy"/>.
		/// </summary>
		/// <typeparam name="TProxy">The type of the proxy.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="method">The delegate encapsulating the invocation of the method.</param>
		/// <param name="callback">The asynchronous callback.</param>
		/// <param name="state">The asynchronous state.</param>
		/// <returns>The async call handle.</returns>
		public static IWcfAsyncCall<TResult> BeginWcfCall<TProxy, TResult>(
			this TProxy proxy, Func<TProxy, TResult> method,
			AsyncCallback callback, object state)
		{
			var call = new AsyncWcfCall<TProxy, TResult>(proxy, method);
			call.Begin(ar => callback(call), state);
			return call;
		}

		/// <summary>
		/// Ends the asynchronous call and returns the result.
		/// </summary>
		/// <typeparam name="TResult">The result type.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="result">The asynchronous result.</param>
		/// <returns>The result of the call.</returns>
		public static TResult EndWcfCall<TResult>(this object proxy, IAsyncResult result)
		{
			return EnsureAsyncCall<TResult>(result).End();
		}

		/// <summary>
		/// Ends the asynchronous call and returns the results.
		/// </summary>
		/// <typeparam name="TResult">The main result type.</typeparam>
		/// <typeparam name="TOut1">The additional result type.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The additional result.</param>
		/// <returns>The main result.</returns>
		public static TResult EndWcfCall<TResult, TOut1>(this object proxy, IAsyncResult result, out TOut1 out1)
		{
			return EnsureAsyncCall<TResult>(result).End(out out1);
		}

		/// <summary>
		/// Ends the asynchronous call and returns the results.
		/// </summary>
		/// <typeparam name="TResult">The main result type.</typeparam>
		/// <typeparam name="TOut1">The additional result type.</typeparam>
		/// <typeparam name="TOut2">The additional result type.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The first additional result.</param>
		/// <param name="out2">The second additional result.</param>
		/// <returns>The main result.</returns>
		public static TResult EndWcfCall<TResult, TOut1, TOut2>(
			this object proxy, IAsyncResult result, out TOut1 out1, TOut2 out2)
		{
			return EnsureAsyncCall<TResult>(result).End(out out1, out out2);
		}

		/// <summary>
		/// Ends the asynchronous call and returns the results.
		/// </summary>
		/// <typeparam name="TResult">The main result type.</typeparam>
		/// <typeparam name="TOut1">The additional result type.</typeparam>
		/// <typeparam name="TOut2">The additional result type.</typeparam>
		/// <typeparam name="TOut3">The additional result type.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The first additional result.</param>
		/// <param name="out2">The second additional result.</param>
		/// <param name="out3">The thrid additional result.</param>
		/// <returns>The main result.</returns>
		public static TResult EndWcfCall<TResult, TOut1, TOut2, TOut3>(
			this object proxy, IAsyncResult result, out TOut1 out1,TOut2 out2, TOut3 out3)
		{
			return EnsureAsyncCall<TResult>(result).End(out out1, out out2, out out3);
		}

		/// <summary>
		/// Ends the asynchronous call and returns the results.
		/// </summary>
		/// <typeparam name="TResult">The main result type.</typeparam>
		/// <typeparam name="TOut1">he additional result type.</typeparam>
		/// <typeparam name="TOut2">The additional result type.</typeparam>
		/// <typeparam name="TOut3">The additional result type.</typeparam>
		/// <typeparam name="TOut4">The additional result type.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The first additional result.</param>
		/// <param name="out2">The second additional result.</param>
		/// <param name="out3">The third additional result.</param>
		/// <param name="out4">The fourth additional result.</param>
		/// <returns>The main result.</returns>
		public static TResult EndWcfCall<TResult, TOut1, TOut2, TOut3, TOut4>(
			this object proxy, IAsyncResult result, out TOut1 out1, TOut2 out2, TOut3 out3, TOut4 out4)
		{
			return EnsureAsyncCall<TResult>(result).End(out out1, out out2, out out3, out out4);
		}

		/// <summary>
		/// Begins an asynchronous call of <paramref name="method"/> on given <paramref name="proxy"/>.
		/// </summary>
		/// <typeparam name="TProxy">The type of the proxy.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="method">The delegate encapsulating the invocation of the method.</param>
		/// <returns>The async call handle.</returns>
		public static IWcfAsyncCall BeginWcfCall<TProxy>(this TProxy proxy, Action<TProxy> method)
		{
			return BeginWcfCall(proxy, method, Nothing, null);
		}

		/// <summary>
		/// Begins an asynchronous call of <paramref name="method"/> on given <paramref name="proxy"/>.
		/// </summary>
		/// <typeparam name="TProxy">The type of the proxy.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="method">The delegate encapsulating the invocation of the method.</param>
		/// <param name="callback">The asynchronous callback.</param>
		/// <param name="state">The asynchronous state.</param>
		/// <returns>The async call handle.</returns>
		public static IWcfAsyncCall BeginWcfCall<TProxy>(this TProxy proxy, Action<TProxy> method,
														 Action<IWcfAsyncCall> callback, object state)
		{
			var call = new AsyncWcfCall<TProxy>(proxy, method);
			call.Begin(ar => callback(call), state);
			return call;
		}

		/// <summary>
		/// Begins an asynchronous call of <paramref name="method"/> on given <paramref name="proxy"/>.
		/// </summary>
		/// <typeparam name="TProxy">The type of the proxy.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="method">The delegate encapsulating the invocation of the method.</param>
		/// <param name="callback">The asynchronous callback.</param>
		/// <param name="state">The asynchronous state.</param>
		/// <returns>The async call handle.</returns>
		public static IWcfAsyncCall BeginWcfCall<TProxy>(this TProxy proxy, Action<TProxy> method,
														 AsyncCallback callback, object state)
		{
			var call = new AsyncWcfCall<TProxy>(proxy, method);
			call.Begin(ar => callback(call), state);
			return call;
		}

		/// <summary>
		/// Ends the asynchronous call.
		/// </summary>
		/// <param name="proxy">The proxy.</param>
		/// <param name="result">The asynchronous result.</param>
		public static void EndWcfCall(this object proxy, IAsyncResult result)
		{
			EnsureAsyncCall(result).End();
		}

		/// <summary>
		/// Ends the asynchronous call and returns the results.
		/// </summary>
		/// <typeparam name="TOut1">The additional result type.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The additional result.</param>
		public static void EndWcfCall<TOut1>(this object proxy, IAsyncResult result, out TOut1 out1)
		{
			EnsureAsyncCall(result).End(out out1);
		}

		/// <summary>
		/// Ends the asynchronous call and returns the results.
		/// </summary>
		/// <typeparam name="TOut1">The additional result type.</typeparam>
		/// <typeparam name="TOut2">The additional result type.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The additional result.</param>
		/// <param name="out2">The additional result.</param>
		public static void EndWcfCall<TOut1, TOut2>(this object proxy, IAsyncResult result, out TOut1 out1, TOut2 out2)
		{
			EnsureAsyncCall(result).End(out out1, out out2);
		}

		/// <summary>
		/// Ends the asynchronous call and returns the results.
		/// </summary>
		/// <typeparam name="TOut1">The additional result type.</typeparam>
		/// <typeparam name="TOut2">The additional result type.</typeparam>
		/// <typeparam name="TOut3">The additional result type.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The additional result.</param>
		/// <param name="out2">The additional result.</param>
		/// <param name="out3">The additional result.</param>
		public static void EndWcfCall<TOut1, TOut2, TOut3>(
			this object proxy, IAsyncResult result, out TOut1 out1, TOut2 out2, TOut3 out3)
		{
			EnsureAsyncCall(result).End(out out1, out out2, out out3);
		}

		/// <summary>
		/// Ends the asynchronous call and returns the results.
		/// </summary>
		/// <typeparam name="TOut1">The additional result type.</typeparam>
		/// <typeparam name="TOut2">The additional result type.</typeparam>
		/// <typeparam name="TOut3">The additional result type.</typeparam>
		/// <typeparam name="TOut4">The additional result type.</typeparam>
		/// <param name="proxy">The proxy.</param>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="out1">The additional result.</param>
		/// <param name="out2">The additional result.</param>
		/// <param name="out3">The additional result.</param>
		/// <param name="out4">The additional result.</param>
		public static void EndWcfCall<TOut1, TOut2, TOut3, TOut4>(
			this object proxy, IAsyncResult result, out TOut1 out1, TOut2 out2, TOut3 out3, TOut4 out4)
		{
			EnsureAsyncCall(result).End(out out1, out out2, out out3, out out4);
		}

		private static IWcfAsyncCall<TResult> EnsureAsyncCall<TResult>(IAsyncResult result)
		{
			var call = result as IWcfAsyncCall<TResult>;
			if (call == null)
			{
				throw new ArgumentException("The supplied result is not the correct type.  " +
					" Did you obtain it from a call to BeginWcfCall<TResult>?");
			}
			return call;
		}

		private static IWcfAsyncCall EnsureAsyncCall(IAsyncResult result)
		{
			var call = result as IWcfAsyncCall;
			if (call == null)
			{
				throw new ArgumentException("The supplied result is not the correct type.  " +
					" Did you obtain it from a call to BeginWcfCall?");
			}
			return call;
		}
	}
}