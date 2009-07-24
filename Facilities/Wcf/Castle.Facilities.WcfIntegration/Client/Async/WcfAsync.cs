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
			return BeginWcfCall(proxy, method, delegate { }, null);
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
		/// <param name="proxy">The proxy.</param>
		/// <param name="method">The delegate encapsulating the invocation of the method.</param>
		/// <returns>The async call handle.</returns>
		public static IWcfAsyncCall BeginWcfCall<TProxy>(this TProxy proxy, Action<TProxy> method)
		{
			return BeginWcfCall(proxy, method, delegate { }, null);
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
	}
}