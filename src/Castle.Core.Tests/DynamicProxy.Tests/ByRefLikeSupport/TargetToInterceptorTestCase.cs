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
	///   Tests that byref-like argument values can be successfully marshalled from the target method
	///   back to the interception pipeline (more specifically, from `IInvocation.Arguments`).
	/// </summary>
	[TestFixture]
	public class TargetToInterceptorTestCase : BasePEVerifyTestCase
	{
		#region Tests for `ByRefLike` parameters and return type (this type represents all byref-like types that aren't spans)

#if NET9_0_OR_GREATER

		[Test]
		public void ByRefLike__passed_by_ref_ref__set_at_target__can_be_read_from_ReadOnlySpanReference()
		{
			InvokeProxyAndProceedToTargetAndInspectInvocationArgument(
				invoke: (IPassByRefLikeByRefRef proxy) =>
				{
					ByRefLike arg = default;
					proxy.PassByRefRef(ref arg);
				},
				target: () => new PassByRefLikeTarget(set: (out ByRefLike arg) =>
				{
					arg = new ByRefLike("from target");
				}),
				inspect: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ByRefLikeReference<ByRefLike>>(invocationArg);
					ByRefLike arg = ((ByRefLikeReference<ByRefLike>)invocationArg!).Value;
					Assert.AreEqual("from target", arg.Value);
				});
		}

		[Test]
		public void ByRefLike__passed_by_ref_out__set_at_target__can_be_read_from_ReadOnlySpanReference()
		{
			InvokeProxyAndProceedToTargetAndInspectInvocationArgument(
				invoke: (IPassByRefLikeByRefOut proxy) =>
				{
					proxy.PassByRefOut(out ByRefLike arg);
				},
				target: () => new PassByRefLikeTarget(set: (out ByRefLike arg) =>
				{
					arg = new ByRefLike("from target");
				}),
				inspect: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ByRefLikeReference<ByRefLike>>(invocationArg);
					ByRefLike arg = ((ByRefLikeReference<ByRefLike>)invocationArg!).Value;
					Assert.AreEqual("from target", arg.Value);
				});
		}

		[Test]
		public void ByRefLike__return_value__returned_at_target__can_be_read_from_ReadOnlySpanReference()
		{
			InvokeProxyAndProceedToTargetAndInspectInvocationReturnValue(
				invoke: (IReturnByRefLikeByValue proxy) =>
				{
					ByRefLike returnValue = proxy.ReturnByValue();
				},
				target: () => new PassByRefLikeTarget(set: (out ByRefLike returnValue) =>
				{
					returnValue = new ByRefLike("from target");
				}),
				inspect: (object? invocationReturnValue) =>
				{
					Assert.IsInstanceOf<ByRefLikeReference<ByRefLike>>(invocationReturnValue);
					ByRefLike returnValue = ((ByRefLikeReference<ByRefLike>)invocationReturnValue!).Value;
					Assert.AreEqual("from target", returnValue.Value);
				});
		}

#endif

		#endregion

		#region Tests for `ReadOnlySpan<T>` parameters and return type

		[Test]
		public void ReadOnlySpan__passed_by_ref_ref__set_at_target__can_be_read_from_ReadOnlySpanReference()
		{
			InvokeProxyAndProceedToTargetAndInspectInvocationArgument(
				invoke: (IPassReadOnlySpanByRefRef proxy) =>
				{
					ReadOnlySpan<char> arg = default;
					proxy.PassByRefRef(ref arg);
				},
				target: () => new PassReadOnlySpanTarget(set: (out ReadOnlySpan<char> arg) =>
				{
					arg = "from target".AsSpan();
				}),
				inspect: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ReadOnlySpanReference<char>>(invocationArg);
					ReadOnlySpan<char> arg = ((ReadOnlySpanReference<char>)invocationArg!).Value;
					Assert.AreEqual("from target", new string(arg));
				});
		}

		[Test]
		public void ReadOnlySpan__passed_by_ref_out__set_at_target__can_be_read_from_ReadOnlySpanReference()
		{
			InvokeProxyAndProceedToTargetAndInspectInvocationArgument(
				invoke: (IPassReadOnlySpanByRefOut proxy) =>
				{
					proxy.PassByRefOut(out ReadOnlySpan<char> arg);
				},
				target: () => new PassReadOnlySpanTarget(set: (out ReadOnlySpan<char> arg) =>
				{
					arg = "from target".AsSpan();
				}),
				inspect: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ReadOnlySpanReference<char>>(invocationArg);
					ReadOnlySpan<char> arg = ((ReadOnlySpanReference<char>)invocationArg!).Value;
					Assert.AreEqual("from target", new string(arg));
				});
		}

		[Test]
		public void ReadOnlySpan__return_value__returned_at_target__can_be_read_from_ReadOnlySpanReference()
		{
			InvokeProxyAndProceedToTargetAndInspectInvocationReturnValue(
				invoke: (IReturnReadOnlySpanByValue proxy) =>
				{
					ReadOnlySpan<char> returnValue = proxy.ReturnByValue();
				},
				target: () => new PassReadOnlySpanTarget(set: (out ReadOnlySpan<char> returnValue) =>
				{
					returnValue = "from target".AsSpan();
				}),
				inspect: (object? invocationReturnValue) =>
				{
					Assert.IsInstanceOf<ReadOnlySpanReference<char>>(invocationReturnValue);
					ReadOnlySpan<char> returnValue = ((ReadOnlySpanReference<char>)invocationReturnValue!).Value;
					Assert.AreEqual("from target", new string(returnValue));
				});
		}

		#endregion

		#region Tests for `Span<T>` parameters and return type

		[Test]
		public void Span__passed_by_ref_ref__set_at_target__can_be_read_from_ReadOnlySpanReference()
		{
			InvokeProxyAndProceedToTargetAndInspectInvocationArgument(
				invoke: (IPassSpanByRefRef proxy) =>
				{
					Span<char> arg = default;
					proxy.PassByRefRef(ref arg);
				},
				target: () => new PassSpanTarget(set: (out Span<char> arg) =>
				{
					arg = "from target".ToCharArray().AsSpan();
				}),
				inspect: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<SpanReference<char>>(invocationArg);
					Span<char> arg = ((SpanReference<char>)invocationArg!).Value;
					Assert.AreEqual("from target", new string(arg));
				});
		}

		[Test]
		public void Span__passed_by_ref_out__set_at_target__can_be_read_from_ReadOnlySpanReference()
		{
			InvokeProxyAndProceedToTargetAndInspectInvocationArgument(
				invoke: (IPassSpanByRefOut proxy) =>
				{
					proxy.PassByRefOut(out Span<char> arg);
				},
				target: () => new PassSpanTarget(set: (out Span<char> arg) =>
				{
					arg = "from target".ToCharArray().AsSpan();
				}),
				inspect: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<SpanReference<char>>(invocationArg);
					Span<char> arg = ((SpanReference<char>)invocationArg!).Value;
					Assert.AreEqual("from target", new string(arg));
				});
		}

		[Test]
		public void Span__return_value__returned_at_target__can_be_read_from_ReadOnlySpanReference()
		{
			InvokeProxyAndProceedToTargetAndInspectInvocationReturnValue(
				invoke: (IReturnSpanByValue proxy) =>
				{
					Span<char> returnValue = proxy.ReturnByValue();
				},
				target: () => new PassSpanTarget(set: (out Span<char> result) =>
				{
					result = "from target".ToCharArray().AsSpan();
				}),
				inspect: (object? invocationReturnValue) =>
				{
					Assert.IsInstanceOf<SpanReference<char>>(invocationReturnValue);
					Span<char> returnValue = ((SpanReference<char>)invocationReturnValue!).Value;
					Assert.AreEqual("from target", new string(returnValue));
				});
		}

		#endregion

		private void InvokeProxyAndProceedToTargetAndInspectInvocationArgument<TInterface, TTarget>(Action<TInterface> invoke,
		                                                                                            Func<TTarget> target,
		                                                                                            Action<object?> inspect)
			where TInterface : class
			where TTarget : ITarget, TInterface
		{
			var interceptor = new AdHoc(invocation =>
			{
				invocation.Proceed();
				inspect(invocation.Arguments[0]);
			});
			var settingTarget = target();
			var proxy = generator.CreateInterfaceProxyWithTarget<TInterface>(settingTarget, interceptor);
			invoke(proxy);
			Assert.True(interceptor.Executed);
			Assert.True(settingTarget.Executed);
		}

		private void InvokeProxyAndProceedToTargetAndInspectInvocationReturnValue<TInterface, TTarget>(Action<TInterface> invoke,
		                                                                                               Func<TTarget> target,
		                                                                                               Action<object?> inspect)
			where TInterface : class
			where TTarget : ITarget, TInterface
		{
			var interceptor = new AdHoc(invocation =>
			{
				invocation.Proceed();
				inspect(invocation.ReturnValue);
			});
			var settingTarget = target();
			var proxy = generator.CreateInterfaceProxyWithTarget<TInterface>(settingTarget, interceptor);
			invoke(proxy);
			Assert.True(interceptor.Executed);
			Assert.True(settingTarget.Executed);
		}
	}
}

#endif
