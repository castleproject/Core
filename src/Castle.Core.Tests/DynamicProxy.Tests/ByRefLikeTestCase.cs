// Copyright 2004-2025 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests
{
	using System;

	using Castle.DynamicProxy.Tests.Interceptors;

	using NUnit.Framework;

	/// <summary>
	///   Tests for by-ref-like (<see langword="ref"/> <see langword="struct"/>) method parameter and return types.
	/// </summary>
	[TestFixture]
	public class ByRefLikeTestCase : BasePEVerifyTestCase
	{
		[TestCase(typeof(IHaveMethodWithByRefLikeParameter))]
		[TestCase(typeof(IHaveMethodWithByRefLikeInParameter))]
		[TestCase(typeof(IHaveMethodWithByRefLikeRefParameter))]
		[TestCase(typeof(IHaveMethodWithByRefLikeOutParameter))]
		[TestCase(typeof(IHaveMethodWithByRefLikeReturnType))]
		public void Can_proxy_type(Type interfaceType)
		{
			_ = generator.CreateInterfaceProxyWithoutTarget(interfaceType);
		}

		#region Can methods with by-ref-like parameters be intercepted without crashing?

		[Test]
		public void Can_invoke_method_with_by_ref_like_parameter()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithByRefLikeParameter>(new DoNothingInterceptor());
			var arg = default(ByRefLike);
			proxy.Method(arg);
		}

		[Test]
		public void Can_invoke_method_with_by_ref_like_in_parameter()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithByRefLikeInParameter>(new DoNothingInterceptor());
			ByRefLike arg = default;
			proxy.Method(in arg);
		}

		[Test]
		public void Can_invoke_method_with_by_ref_like_ref_parameter()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithByRefLikeRefParameter>(new DoNothingInterceptor());
			ByRefLike arg = default;
			proxy.Method(ref arg);
		}

		[Test]
		public void Can_invoke_method_with_by_ref_like_out_parameter()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithByRefLikeOutParameter>(new DoNothingInterceptor());
			proxy.Method(out _);
		}

		[Test]
		public void Can_invoke_method_with_by_ref_like_return_type()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithByRefLikeReturnType>(new DoNothingInterceptor());
			_ = proxy.Method();
		}

		#endregion

		#region Can methods with by-ref-like parameters be proceeded to without crashing?

		[Test]
		public void Can_proceed_to_target_method_with_by_ref_like_parameter()
		{
			var target = new HasMethodWithByRefLikeParameter();
			var proxy = generator.CreateInterfaceProxyWithTarget<IHaveMethodWithByRefLikeParameter>(target, new StandardInterceptor());
			ByRefLike arg = default;
			proxy.Method(arg);
		}

		[Test]
		public void Can_proceed_to_target_method_with_by_ref_like_in_parameter()
		{
			var target = new HasMethodWithByRefLikeInParameter();
			var proxy = generator.CreateInterfaceProxyWithTarget<IHaveMethodWithByRefLikeInParameter>(target, new StandardInterceptor());
			ByRefLike arg = default;
			proxy.Method(in arg);
		}

		[Test]
		public void Can_proceed_to_target_method_with_by_ref_like_ref_parameter()
		{
			var target = new HasMethodWithByRefLikeRefParameter();
			var proxy = generator.CreateInterfaceProxyWithTarget<IHaveMethodWithByRefLikeRefParameter>(target, new StandardInterceptor());
			ByRefLike arg = default;
			proxy.Method(ref arg);
		}

		[Test]
		public void Can_proceed_to_target_method_with_by_ref_like_out_parameter()
		{
			var target = new HasMethodWithByRefLikeOutParameter();
			var proxy = generator.CreateInterfaceProxyWithTarget<IHaveMethodWithByRefLikeOutParameter>(target, new StandardInterceptor());
			proxy.Method(out _);
		}

		[Test]
		public void Can_proceed_to_target_method_with_by_ref_like_return_type()
		{
			var target = new HasMethodWithByRefLikeReturnType();
			var proxy = generator.CreateInterfaceProxyWithTarget<IHaveMethodWithByRefLikeReturnType>(target, new StandardInterceptor());
			_ = proxy.Method();
		}

		#endregion

		public ref struct ByRefLike
		{
		}

		public interface IHaveMethodWithByRefLikeParameter
		{
			void Method(ByRefLike arg);
		}

		public class HasMethodWithByRefLikeParameter : IHaveMethodWithByRefLikeParameter
		{
			public virtual void Method(ByRefLike arg)
			{
			}
		}

		public interface IHaveMethodWithByRefLikeInParameter
		{
			void Method(in ByRefLike arg);
		}

		public class HasMethodWithByRefLikeInParameter : IHaveMethodWithByRefLikeInParameter
		{
			public virtual void Method(in ByRefLike arg)
			{
			}
		}

		public interface IHaveMethodWithByRefLikeRefParameter
		{
			void Method(ref ByRefLike arg);
		}

		public class HasMethodWithByRefLikeRefParameter : IHaveMethodWithByRefLikeRefParameter
		{
			public virtual void Method(ref ByRefLike arg)
			{
			}
		}

		public interface IHaveMethodWithByRefLikeOutParameter
		{
			void Method(out ByRefLike arg);
		}

		public class HasMethodWithByRefLikeOutParameter : IHaveMethodWithByRefLikeOutParameter
		{
			public virtual void Method(out ByRefLike arg)
			{
				arg = default;
			}
		}

		public interface IHaveMethodWithByRefLikeReturnType
		{
			ByRefLike Method();
		}

		public class HasMethodWithByRefLikeReturnType : IHaveMethodWithByRefLikeReturnType
		{
			public virtual ByRefLike Method()
			{
				return default;
			}
		}

		#region What values do interceptors see for by-ref-like arguments?

		[Test]
		public void By_ref_like_arguments_are_replaced_with_null_in_invocation()
		{
			var interceptor = new ObservingInterceptor();
			var proxy = generator.CreateClassProxy<HasMethodWithSpanParameter>(interceptor);
			var arg = "original".AsSpan();
			proxy.Method(arg);
			Assert.IsNull(interceptor.ObservedArg);
		}

		[Test]
		public void By_ref_like_in_arguments_are_replaced_with_null_in_invocation()
		{
			var interceptor = new ObservingInterceptor();
			var proxy = generator.CreateClassProxy<HasMethodWithSpanInParameter>(interceptor);
			var arg = "original".AsSpan();
			proxy.Method(in arg);
			Assert.IsNull(interceptor.ObservedArg);
		}

		[Test]
		public void By_ref_like_ref_arguments_are_replaced_with_null_in_invocation()
		{
			var interceptor = new ObservingInterceptor();
			var proxy = generator.CreateClassProxy<HasMethodWithSpanRefParameter>(interceptor);
			var arg = "original".AsSpan();
			proxy.Method(ref arg);
			Assert.IsNull(interceptor.ObservedArg);
		}

		// Note the somewhat weird semantics of this test: DynamicProxy allows you to read the incoming values
		// of `out` arguments, which would be illegal in plain C# ("use of unassigned out parameter").
		// DynamicProxy does not distinguish between `ref` and `out` in this regard.
		[Test]
		public void By_ref_like_out_arguments_are_replaced_with_null_in_invocation()
		{
			var interceptor = new ObservingInterceptor();
			var proxy = generator.CreateClassProxy<HasMethodWithSpanOutParameter>(interceptor);
			var arg = "original".AsSpan();
			proxy.Method(out arg);
			Assert.IsNull(interceptor.ObservedArg);
		}

		#endregion

		#region What values do proceeded-to targets see for by-ref-like arguments?

		// This test merely describes the status quo, and not the behavior we'd ideally want.
		[Test]
		public void By_ref_like_arguments_arrive_reset_to_default_value_at_target()
		{
			var target = new HasMethodWithSpanParameter();
			var proxy = generator.CreateClassProxyWithTarget(target, new StandardInterceptor());
			var arg = "original".AsSpan();
			proxy.Method(arg);
			Assert.AreEqual("", target.RecordedArg);
		}

		// This test merely describes the status quo, and not the behavior we'd ideally want.
		[Test]
		public void By_ref_like_in_arguments_arrive_reset_to_default_value_at_target()
		{
			var target = new HasMethodWithSpanInParameter();
			var proxy = generator.CreateClassProxyWithTarget(target, new StandardInterceptor());
			var arg = "original".AsSpan();
			proxy.Method(in arg);
			Assert.AreEqual("", target.RecordedArg);
		}

		// This test merely describes the status quo, and not the behavior we'd ideally want.
		[Test]
		public void By_ref_like_ref_arguments_arrive_reset_to_default_value_at_target()
		{
			var target = new HasMethodWithSpanRefParameter();
			var proxy = generator.CreateClassProxyWithTarget(target, new StandardInterceptor());
			var arg = "original".AsSpan();
			proxy.Method(ref arg);
			Assert.AreEqual("", target.RecordedArg);
		}

		#endregion

		#region How are by-ref-like by-ref arguments changed by interception?

		[Test]
		public void By_ref_like_in_arguments_are_left_unchanged()
		{
			var proxy = generator.CreateClassProxy<HasMethodWithSpanInParameter>(new DoNothingInterceptor());
			var arg = "original".AsSpan();
			proxy.Method(in arg);
			Assert.AreEqual("original", arg.ToString());
		}

		[Test]
		public void By_ref_like_in_arguments_are_left_unchanged_if_interception_includes_proceed_to_target()
		{
			var target = new HasMethodWithSpanInParameter();
			var proxy = generator.CreateClassProxyWithTarget(target, new StandardInterceptor());
			var arg = "original".AsSpan();
			proxy.Method(in arg);
			Assert.AreEqual("original", arg.ToString());
		}

		[Test]
		public void By_ref_like_ref_arguments_are_left_unchanged()
		{
			var proxy = generator.CreateClassProxy<HasMethodWithSpanRefParameter>(new DoNothingInterceptor());
			var arg = "original".AsSpan();
			proxy.Method(ref arg);
			Assert.AreEqual("original", arg.ToString());
		}

		[Test]
		public void By_ref_like_ref_arguments_are_left_unchanged_if_interception_includes_proceed_to_target()
		{
			var target = new HasMethodWithSpanRefParameter();
			var proxy = generator.CreateClassProxyWithTarget(target, new DoNothingInterceptor());
			var arg = "original".AsSpan();
			proxy.Method(ref arg);
			Assert.AreEqual("original", arg.ToString());
		}

		// This test merely describes the status quo, and not the behavior we'd ideally want:
		// DynamicProxy records the initial values of all by-ref arguments in `IInvocation.Arguments` and,
		// unless changed during interception, writes out the final value for both `ref` and `out` parameters
		// from there... meaning all non-by-ref-like by-ref arguments are by default left unchanged.
		// This cannot work for by-ref-likes, since their initial value cannot be preserved in `Arguments`.
		// To honor the semantics of `out` parameters DynamicProxy *does* write a value (unlike with `ref`,
		// above, where it is free to choose not to).
		[Test]
		public void By_ref_like_out_arguments_are_reset_to_default_value()
		{
			var proxy = generator.CreateClassProxy<HasMethodWithSpanOutParameter>(new DoNothingInterceptor());
			var arg = "original".AsSpan();
			proxy.Method(out arg);
			Assert.AreEqual("", arg.ToString());
		}

		// Once we manage to change the implementation so that `out` arguments aren't reset,
		// and the above test is replaced with an `are_left_unchanged` version, then we should also add
		// an additional `are_left_unchanged_if_interception_includes_proceed_to_target` test variant.

		#endregion

		#region Can interception targets set by-ref-like by-ref arguments?

		// This test merely describes the status quo, and not the behavior we'd ideally want.
		[Test]
		public void By_ref_like_ref_arguments_cannot_be_set_by_target()
		{
			var target = new HasMethodWithSpanRefParameter();
			var proxy = generator.CreateClassProxyWithTarget(target, new StandardInterceptor());
			var arg = "original".AsSpan();
			proxy.Method(ref arg);
			Assert.AreEqual("original", arg.ToString());  // ideally, would be equal to "set"
		}

		// This test merely describes the status quo, and not the behavior we'd ideally want.
		[Test]
		public void By_ref_like_out_arguments_cannot_be_set_by_target()
		{
			var target = new HasMethodWithSpanOutParameter();
			var proxy = generator.CreateClassProxyWithTarget(target, new StandardInterceptor());
			var arg = "original".AsSpan();
			proxy.Method(out arg);
			Assert.AreEqual("", arg.ToString());  // ideally, would be equal to "set"
		}

		#endregion

		public class HasMethodWithSpanParameter
		{
			public string? RecordedArg;

			public virtual void Method(ReadOnlySpan<char> arg)
			{
				RecordedArg = arg.ToString();
			}
		}

		public class HasMethodWithSpanInParameter
		{
			public string? RecordedArg;

			public virtual void Method(in ReadOnlySpan<char> arg)
			{
				RecordedArg = arg.ToString();
			}
		}

		public class HasMethodWithSpanRefParameter
		{
			public string? RecordedArg;

			public virtual void Method(ref ReadOnlySpan<char> arg)
			{
				RecordedArg = arg.ToString();
				arg = "set".AsSpan();
			}
		}

		public class HasMethodWithSpanOutParameter
		{
			public virtual void Method(out ReadOnlySpan<char> arg)
			{
				arg = "set".AsSpan();
			}
		}

		public class ObservingInterceptor : IInterceptor
		{
			public object? ObservedArg;

			public void Intercept(IInvocation invocation)
			{
				ObservedArg = invocation.Arguments[0];
			}
		}
	}
}

#endif
