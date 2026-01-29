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
	///   Tests that byref-like argument values can be successfully marshalled from the interception pipeline
	///   (more specifically, from `IInvocation.Arguments`) to the target method.
	/// </summary>
	[TestFixture]
	public class InterceptorToTargetTestCase : BasePEVerifyTestCase
	{
		#region Tests for `ByRefLike` parameters (this type represents all byref-like types that aren't spans)

#if NET9_0_OR_GREATER

		[Test]
		public void ByRefLike__passed_by_value__written_to_ByRefLikeReference__can_be_read_from_parameter()
		{
			InvokeProxyAndSetInvocationArgumentAndInspectAtTarget(
				invoke: (IPassByRefLikeByValue proxy) =>
				{
					ByRefLike arg = default;
					proxy.PassByValue(arg);
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ByRefLikeReference<ByRefLike>>(invocationArg);
					ByRefLike arg = new("from interceptor");
					((ByRefLikeReference<ByRefLike>)invocationArg!).Value = arg;
				},
				target: () => new PassByRefLikeTarget(inspect: (ByRefLike arg) =>
				{
					Assert.AreEqual("from interceptor", arg.Value);
				}));
		}

		[Test]
		public void ByRefLike__passed_by_ref_in__written_to_ByRefLikeReference__can_be_read_from_parameter()
		{
			InvokeProxyAndSetInvocationArgumentAndInspectAtTarget(
				invoke: (IPassByRefLikeByRefIn proxy) =>
				{
					ByRefLike arg = default;
					proxy.PassByRefIn(in arg);
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ByRefLikeReference<ByRefLike>>(invocationArg);
					ByRefLike arg = new("from interceptor");
					((ByRefLikeReference<ByRefLike>)invocationArg!).Value = arg;
				},
				target: () => new PassByRefLikeTarget(inspect: (ByRefLike arg) =>
				{
					Assert.AreEqual("from interceptor", arg.Value);
				}));
		}

		[Test]
		public void ByRefLike__passed_by_ref_ref__written_to_ByRefLikeReference__can_be_read_from_parameter()
		{
			InvokeProxyAndSetInvocationArgumentAndInspectAtTarget(
				invoke: (IPassByRefLikeByRefRef proxy) =>
				{
					ByRefLike arg = default;
					proxy.PassByRefRef(ref arg);
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ByRefLikeReference<ByRefLike>>(invocationArg);
					ByRefLike arg = new("from interceptor");
					((ByRefLikeReference<ByRefLike>)invocationArg!).Value = arg;
				},
				target: () => new PassByRefLikeTarget(inspect: (ByRefLike arg) =>
				{
					Assert.AreEqual("from interceptor", arg.Value);
				}));
		}

#endif

		#endregion

		#region Tests for `ReadOnlySpan<T>` parameters

		[Test]
		public void ReadOnlySpan__passed_by_value__written_to_ByRefLikeReference__can_be_read_from_parameter()
		{
			InvokeProxyAndSetInvocationArgumentAndInspectAtTarget(
				invoke: (IPassReadOnlySpanByValue proxy) =>
				{
					ReadOnlySpan<char> arg = default;
					proxy.PassByValue(arg);
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ReadOnlySpanReference<char>>(invocationArg);
					ReadOnlySpan<char> arg = "from interceptor".AsSpan();
					((ReadOnlySpanReference<char>)invocationArg!).Value = arg;
				},
				target: () => new PassReadOnlySpanTarget(inspect: (ReadOnlySpan<char> arg) =>
				{
					Assert.AreEqual("from interceptor", new string(arg));
				}));
		}

		[Test]
		public void ReadOnlySpan__passed_by_ref_in__written_to_ByRefLikeReference__can_be_read_from_parameter()
		{
			InvokeProxyAndSetInvocationArgumentAndInspectAtTarget(
				invoke: (IPassReadOnlySpanByRefIn proxy) =>
				{
					ReadOnlySpan<char> arg = default;
					proxy.PassByRefIn(in arg);
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ReadOnlySpanReference<char>>(invocationArg);
					ReadOnlySpan<char> arg = "from interceptor".AsSpan();
					((ReadOnlySpanReference<char>)invocationArg!).Value = arg;
				},
				target: () => new PassReadOnlySpanTarget(inspect: (ReadOnlySpan<char> arg) =>
				{
					Assert.AreEqual("from interceptor", new string(arg));
				}));
		}

		[Test]
		public void ReadOnlySpan__passed_by_ref_ref__written_to_ByRefLikeReference__can_be_read_from_parameter()
		{
			InvokeProxyAndSetInvocationArgumentAndInspectAtTarget(
				invoke: (IPassReadOnlySpanByRefRef proxy) =>
				{
					ReadOnlySpan<char> arg = default;
					proxy.PassByRefRef(ref arg);
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<ReadOnlySpanReference<char>>(invocationArg);
					ReadOnlySpan<char> arg = "from interceptor".AsSpan();
					((ReadOnlySpanReference<char>)invocationArg!).Value = arg;
				},
				target: () => new PassReadOnlySpanTarget(inspect: (ReadOnlySpan<char> arg) =>
				{
					Assert.AreEqual("from interceptor", new string(arg));
				}));
		}

		#endregion

		#region Tests for `Span<T>` parameters

		[Test]
		public void Span__passed_by_value__written_to_ByRefLikeReference__can_be_read_from_parameter()
		{
			InvokeProxyAndSetInvocationArgumentAndInspectAtTarget(
				invoke: (IPassSpanByValue proxy) =>
				{
					Span<char> arg = default;
					proxy.PassByValue(arg);
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<SpanReference<char>>(invocationArg);
					Span<char> arg = "from interceptor".ToCharArray().AsSpan();
					((SpanReference<char>)invocationArg!).Value = arg;
				},
				target: () => new PassSpanTarget(inspect: (Span<char> arg) =>
				{
					Assert.AreEqual("from interceptor", new string(arg));
				}));
		}

		[Test]
		public void Span__passed_by_ref_in__written_to_ByRefLikeReference__can_be_read_from_parameter()
		{
			InvokeProxyAndSetInvocationArgumentAndInspectAtTarget(
				invoke: (IPassSpanByRefIn proxy) =>
				{
					Span<char> arg = default;
					proxy.PassByRefIn(in arg);
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<SpanReference<char>>(invocationArg);
					Span<char> arg = "from interceptor".ToCharArray().AsSpan();
					((SpanReference<char>)invocationArg!).Value = arg;
				},
				target: () => new PassSpanTarget(inspect: (Span<char> arg) =>
				{
					Assert.AreEqual("from interceptor", new string(arg));
				}));
		}

		[Test]
		public void Span__passed_by_ref_ref__written_to_ByRefLikeReference__can_be_read_from_parameter()
		{
			InvokeProxyAndSetInvocationArgumentAndInspectAtTarget(
				invoke: (IPassSpanByRefRef proxy) =>
				{
					Span<char> arg = default;
					proxy.PassByRefRef(ref arg);
				},
				set: (object? invocationArg) =>
				{
					Assert.IsInstanceOf<SpanReference<char>>(invocationArg);
					Span<char> arg = "from interceptor".ToCharArray().AsSpan();
					((SpanReference<char>)invocationArg!).Value = arg;
				},
				target: () => new PassSpanTarget(inspect: (Span<char> arg) =>
				{
					Assert.AreEqual("from interceptor", new string(arg));
				}));
		}

		#endregion

		private void InvokeProxyAndSetInvocationArgumentAndInspectAtTarget<TInterface, TTarget>(Action<TInterface> invoke,
		                                                                                        Action<object?> set,
		                                                                                        Func<TTarget> target)
			where TInterface : class
			where TTarget : ITarget, TInterface
		{
			var interceptor = new AdHoc(invocation =>
			{
				set(invocation.Arguments[0]);
				invocation.Proceed();
			});
			var inspectingTarget = target();
			var proxy = generator.CreateInterfaceProxyWithTarget<TInterface>(inspectingTarget, interceptor);
			invoke(proxy);
			Assert.True(interceptor.Executed);
			Assert.True(inspectingTarget.Executed);
		}
	}
}

#endif
