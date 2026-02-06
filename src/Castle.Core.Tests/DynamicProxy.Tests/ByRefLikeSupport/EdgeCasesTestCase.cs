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
	///   Tests various edge cases related to byref-like parameters and return types.
	/// </summary>
	[TestFixture]
	public class EdgeCasesTestCase : BasePEVerifyTestCase
	{
		#region Tests for defensive copying for `in` parameters

#if NET9_0_OR_GREATER

		[Test]
		public void ByRefLike__passed_by_ref_in__written_to_ByRefLikeReference__does_not_propagate_back_to_caller()
		{
			InvokeProxyAndSetInvocationArgument(
				invoke: (IPassByRefLikeByRefIn proxy) =>
				{
					ByRefLike arg = new("from caller");
					proxy.PassByRefIn(in arg);
					Assert.AreEqual("from caller", arg.Value);
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ByRefLikeReference<ByRefLike>>(invocationArg);
					ByRefLike arg = new("from interceptor");
					((ByRefLikeReference<ByRefLike>)invocationArg!).Value = arg;
				});
		}

#endif

		[Test]
		public void ReadOnlySpan__passed_by_ref_in__written_to_ByRefLikeReference__does_not_propagate_back_to_caller()
		{
			InvokeProxyAndSetInvocationArgument(
				invoke: (IPassReadOnlySpanByRefIn proxy) =>
				{
					ReadOnlySpan<char> arg = "from caller".AsSpan();
					proxy.PassByRefIn(in arg);
					Assert.AreEqual("from caller", new string(arg));
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ReadOnlySpanReference<char>>(invocationArg);
					ReadOnlySpan<char> arg = "from interceptor".AsSpan();
					((ReadOnlySpanReference<char>)invocationArg!).Value = arg;
				});
		}

		[Test]
		public void Span__passed_by_ref_in__written_to_ByRefLikeReference__does_not_propagate_back_to_caller()
		{
			InvokeProxyAndSetInvocationArgument(
				invoke: (IPassSpanByRefIn proxy) =>
				{
					Span<char> arg = "from caller".ToCharArray().AsSpan();
					proxy.PassByRefIn(in arg);
					Assert.AreEqual("from caller", new string(arg));
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<SpanReference<char>>(invocationArg);
					Span<char> arg = "from interceptor".ToCharArray().AsSpan();
					((SpanReference<char>)invocationArg!).Value = arg;
				});
		}

		#endregion

		#region Tests for (non-) compatibility with `invocation.CaptureProceedInfo()` / `proceedInfo.Invoke()`

		[Test]
		public void ByRefLike__cannot_run_interception_pipeline_a_second_time()
		{
			var interceptor = new AdHoc(Intercept);
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IPassByRefLikeByValue>(interceptor);
			proxy.PassByValue(default);

			void Intercept(IInvocation invocation)
			{
				Assert.Throws<InvalidOperationException>(() => invocation.CaptureProceedInfo());
			}

			Assert.True(interceptor.Executed);
		}

		[Test]
		public void ReadOnlySpan__cannot_run_interception_pipeline_a_second_time()
		{
			var interceptor = new AdHoc(Intercept);
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IPassReadOnlySpanByValue>(interceptor);
			proxy.PassByValue(default);

			void Intercept(IInvocation invocation)
			{
				Assert.Throws<InvalidOperationException>(() => invocation.CaptureProceedInfo());
			}

			Assert.True(interceptor.Executed);
		}

		[Test]
		public void Span__cannot_run_interception_pipeline_a_second_time()
		{
			var interceptor = new AdHoc(Intercept);
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IPassSpanByValue>(interceptor);
			proxy.PassByValue(default);

			void Intercept(IInvocation invocation)
			{
				Assert.Throws<InvalidOperationException>(() => invocation.CaptureProceedInfo());
			}

			Assert.True(interceptor.Executed);
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
	}
}

#endif
