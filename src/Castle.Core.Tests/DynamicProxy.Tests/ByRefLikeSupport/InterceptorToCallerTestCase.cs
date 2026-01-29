// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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

#if FEATURE_BYREFLIKE

#nullable enable

namespace Castle.DynamicProxy.Tests.ByRefLikeSupport
{
	using System;

	using NUnit.Framework;

	/// <summary>
	///   Tests that byref-like argument and return values can be successfully marshalled
	///   from the interception pipeline (more specifically, from `IInvocation.Arguments`)
	///   back to user code.
	/// </summary>
	[TestFixture]
	public class InterceptorToCallerTestCase : BasePEVerifyTestCase
	{
		#region Tests for `ByRefLike` parameters and return type (this type represents all byref-like types that aren't spans)

#if NET9_0_OR_GREATER

		[Test]
		public void ByRefLike__passed_by_ref_ref___can_be_written_to_ByRefLikeReference()
		{
			InvokeProxyAndSetInvocationArgument(
				invoke: (IPassByRefLikeByRefRef proxy) =>
				{
					ByRefLike arg = default;
					proxy.PassByRefRef(ref arg);
					Assert.AreEqual("from interceptor", arg.Value);
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ByRefLikeReference<ByRefLike>>(invocationArg);
					ByRefLike arg = new("from interceptor");
					((ByRefLikeReference<ByRefLike>)invocationArg!).Value = arg;
				});
		}

		[Test]
		public void ByRefLike__passed_by_ref_out___can_be_written_to_ByRefLikeReference()
		{
			InvokeProxyAndSetInvocationArgument(
				invoke: (IPassByRefLikeByRefOut proxy) =>
				{
					proxy.PassByRefOut(out ByRefLike arg);
					Assert.AreEqual("from interceptor", arg.Value);
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ByRefLikeReference<ByRefLike>>(invocationArg);
					ByRefLike arg = new("from interceptor");
					((ByRefLikeReference<ByRefLike>)invocationArg!).Value = arg;
				});
		}

		[Test]
		public void ByRefLike__return_value__can_be_written_to_ByRefLikeReference()
		{
			InvokeProxyAndSetInvocationReturnValue(
				invoke: (IReturnByRefLikeByValue proxy) =>
				{
					ByRefLike returnValue = proxy.ReturnByValue();
					Assert.AreEqual("from interceptor", returnValue.Value);
				},
				set: (object? invocationReturnValue) =>
				{
					Assert.IsInstanceOf<ByRefLikeReference<ByRefLike>>(invocationReturnValue);
					ByRefLike returnValue = new("from interceptor");
					((ByRefLikeReference<ByRefLike>)invocationReturnValue!).Value = returnValue;
				});
		}

#endif

		#endregion

		#region Tests for `ReadOnlySpan<T>` parameters and return type

		[Test]
		public void ReadOnlySpan__passed_by_ref_ref___can_be_written_to_ReadOnlySpanReference()
		{
			InvokeProxyAndSetInvocationArgument(
				invoke: (IPassReadOnlySpanByRefRef proxy) =>
				{
					ReadOnlySpan<char> arg = "from caller".AsSpan();
					proxy.PassByRefRef(ref arg);
					Assert.AreEqual("from interceptor", new string(arg));
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ReadOnlySpanReference<char>>(invocationArg);
					ReadOnlySpan<char> arg = "from interceptor".AsSpan();
					((ReadOnlySpanReference<char>)invocationArg!).Value = arg;
				});
		}

		[Test]
		public void ReadOnlySpan__passed_by_ref_out___can_be_written_to_ReadOnlySpanReference()
		{
			InvokeProxyAndSetInvocationArgument(
				invoke: (IPassReadOnlySpanByRefOut proxy) =>
				{
					proxy.PassByRefOut(out ReadOnlySpan<char> arg);
					Assert.AreEqual("from interceptor", new string(arg));
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ReadOnlySpanReference<char>>(invocationArg);
					ReadOnlySpan<char> arg = "from interceptor".AsSpan();
					((ReadOnlySpanReference<char>)invocationArg!).Value = arg;
				});
		}

		[Test]
		public void ReadOnlySpan__return_value__can_be_written_to_ReadOnlySpanReference()
		{
			InvokeProxyAndSetInvocationReturnValue(
				invoke: (IReturnReadOnlySpanByValue proxy) =>
				{
					ReadOnlySpan<char> returnValue = proxy.ReturnByValue();
					Assert.AreEqual("from interceptor", new string(returnValue));
				},
				set: (object? invocationReturnValue) =>
				{
					Assert.IsInstanceOf<ReadOnlySpanReference<char>>(invocationReturnValue);
					ReadOnlySpan<char> returnValue = "from interceptor".AsSpan();
					((ReadOnlySpanReference<char>)invocationReturnValue!).Value = returnValue;
				});
		}

		#endregion

		#region Tests for `Span<T>` parameters and return type

		[Test]
		public void Span__passed_by_ref_ref___can_be_written_to_SpanReference()
		{
			InvokeProxyAndSetInvocationArgument(
				invoke: (IPassSpanByRefRef proxy) =>
				{
					Span<char> arg = "from caller".ToCharArray().AsSpan();
					proxy.PassByRefRef(ref arg);
					Assert.AreEqual("from interceptor", new string(arg));
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<SpanReference<char>>(invocationArg);
					Span<char> arg = "from interceptor".ToCharArray().AsSpan();
					((SpanReference<char>)invocationArg!).Value = arg;
				});
		}

		[Test]
		public void Span__passed_by_ref_out___can_be_written_to_SpanReference()
		{
			InvokeProxyAndSetInvocationArgument(
				invoke: (IPassSpanByRefOut proxy) =>
				{
					proxy.PassByRefOut(out Span<char> arg);
					Assert.AreEqual("from interceptor", new string(arg));
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<SpanReference<char>>(invocationArg);
					Span<char> arg = "from interceptor".ToCharArray().AsSpan();
					((SpanReference<char>)invocationArg!).Value = arg;
				});
		}

		[Test]
		public void Span__return_value__can_be_written_to_SpanReference()
		{
			InvokeProxyAndSetInvocationReturnValue(
				invoke: (IReturnSpanByValue proxy) =>
				{
					Span<char> returnValue = proxy.ReturnByValue();
					Assert.AreEqual("from interceptor", new string(returnValue));
				},
				set: (object? invocationReturnValue) =>
				{
					Assert.IsInstanceOf<SpanReference<char>>(invocationReturnValue);
					Span<char> returnValue = "from interceptor".ToCharArray().AsSpan();
					((SpanReference<char>)invocationReturnValue!).Value = returnValue;
				});
		}

		#endregion

		private void InvokeProxyAndSetInvocationArgument<TInterface>(Action<TInterface> invoke, Action<object?> set)
			where TInterface : class
		{
			var interceptor = new AdHoc(invocation => set(invocation.Arguments[0]));
			var proxy = generator.CreateInterfaceProxyWithoutTarget<TInterface>(interceptor);
			invoke(proxy);
			Assert.True(interceptor.Executed);
		}

		private void InvokeProxyAndSetInvocationReturnValue<TInterface>(Action<TInterface> invoke, Action<object?> set)
			where TInterface : class
		{
			var interceptor = new AdHoc(invocation => set(invocation.ReturnValue));
			var proxy = generator.CreateInterfaceProxyWithoutTarget<TInterface>(interceptor);
			invoke(proxy);
			Assert.True(interceptor.Executed);
		}
	}
}

#endif
