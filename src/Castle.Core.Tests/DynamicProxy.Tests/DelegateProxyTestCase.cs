// Copyright 2004-2018 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests
{
	using Castle.DynamicProxy.Tests.Interceptors;

	using NUnit.Framework;
	using System;

	[TestFixture]
	public class DelegateProxyTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void CreateDelegateProxy_throws_when_given_null_for_parameter_delegateToProxy()
		{
			var _ = new DoNothingInterceptor();

			TestDelegate call = () => generator.CreateDelegateProxy(null, _);

			var ex = Assert.Throws<ArgumentNullException>(call);
			Assert.AreEqual("delegateToProxy", ex.ParamName);
		}

		// Note: We will not repeat the above test for `CreateDelegateProxyWithTarget`,
		// since both methods mostly delegate to the same private implementation method.
		// We will only target `CreateDelegateProxyWithTarget` in tests where its logic
		// differs from `CreateDelegateProxy`, and where the target matters.

		[Test]
		public void CreateDelegateProxy_throws_when_given_null_for_parameter_interceptors()
		{
			var _ = typeof(Action);

			TestDelegate call = () => generator.CreateDelegateProxy(_, null);

			var ex = Assert.Throws<ArgumentNullException>(call);
			Assert.AreEqual("interceptors", ex.ParamName);
		}

		[Test]
		public void CreateDelegateProxy_throws_when_given_any_null_in_parameter_interceptors()
		{
			var _ = typeof(Action);
			var __ = new DoNothingInterceptor();

			TestDelegate call = () => generator.CreateDelegateProxy(_, __, null, __);

			var ex = Assert.Throws<ArgumentException>(call);
			Assert.AreEqual("interceptors", ex.ParamName);
		}

		[Test]
		public void CreateDelegateProxy_succeeds_when_arguments_non_null()
		{
			var proxy = generator.CreateDelegateProxy(typeof(Action), new DoNothingInterceptor());

			Assert.NotNull(proxy);
		}

		[TestCase(typeof(void))]
		[TestCase(typeof(int))]
		[TestCase(typeof(string))]
		[TestCase(typeof(AttributeTargets))]
		[TestCase(typeof(EventArgs))]
		[TestCase(typeof(Exception))]
		public void CreateDelegateProxy_throws_when_given_non_delegate_type(Type nonDelegateType)
		{
			TestDelegate call = () => generator.CreateDelegateProxy(nonDelegateType);

			var ex = Assert.Throws<ArgumentException>(call);
			Assert.AreEqual("delegateToProxy", ex.ParamName);
		}

		[TestCase(typeof(Action<>))]
		[TestCase(typeof(Func<,>))]
		public void CreateDelegateProxy_throws_when_given_open_generic_delegate_type(Type openGenericDelegateType)
		{
			TestDelegate call = () => generator.CreateDelegateProxy(openGenericDelegateType);

			var ex = Assert.Throws<ArgumentException>(call);
			Assert.AreEqual("delegateToProxy", ex.ParamName);
		}

		[Test]
		public void CreateDelegateProxyWithTarget_throws_when_given_null_for_target()
		{
			var _ = typeof(Action);

			TestDelegate call = () => generator.CreateDelegateProxyWithTarget(_, (Action)null);

			var ex = Assert.Throws<ArgumentNullException>(call);
			Assert.AreEqual("target", ex.ParamName);
		}

		[Test]
		public void CreateDelegateProxyWithTarget_throws_when_target_type_does_not_match_delegate_type_to_proxy()
		{
			var delegateType = typeof(Action);
			Func<bool> targetWithDifferentType = () => true;

			TestDelegate call = () => generator.CreateDelegateProxyWithTarget(delegateType, targetWithDifferentType);

			var ex = Assert.Throws<ArgumentException>(call);
			Assert.AreEqual("target", ex.ParamName);
		}

		[Test]
		public void CreateDelegateProxy_creates_delegate_that_calls_interceptor()
		{
			var interceptorInvoked = false;
			var interceptor = new WithCallbackInterceptor(_ =>
			{
				interceptorInvoked = true;
			});
			var proxy = generator.CreateDelegateProxy<Action>(interceptor);

			proxy();

			Assert.True(interceptorInvoked);
		}

		[Test]
		public void CreateDelegateProxyWithTarget_creates_delegate_that_calls_target()
		{
			var targetInvoked = false;
			Action target = () => targetInvoked = true;
			var proxy = generator.CreateDelegateProxyWithTarget<Action>(target);

			proxy();

			Assert.True(targetInvoked);
		}

		[Test]
		public void CreateDelegateProxyWithTarget_creates_delegate_that_calls_interceptor_but_not_target_if_not_Proceed()
		{
			var interceptorInvoked = false;
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				interceptorInvoked = true;
			});
			var targetInvoked = false;
			Action target = () => targetInvoked = true;
			var proxy = generator.CreateDelegateProxyWithTarget<Action>(target, interceptor);

			proxy();

			Assert.True(interceptorInvoked);
			Assert.False(targetInvoked);
		}

		[Test]
		public void CreateDelegateProxyWithTarget_creates_delegate_that_calls_interceptor_and_target_if_Proceed()
		{
			var interceptorInvoked = false;
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				interceptorInvoked = true;
				invocation.Proceed();
			});
			var targetInvoked = false;
			Action target = () => targetInvoked = true;
			var proxy = generator.CreateDelegateProxyWithTarget<Action>(target, interceptor);

			proxy();

			Assert.True(interceptorInvoked);
			Assert.True(targetInvoked);
		}

		[Test]
		public void CreateDelegateProxy_creates_delegate_that_calls_several_interceptors_in_order_given()
		{
			int value = 1;
			var firstInterceptor = new WithCallbackInterceptor(invocation =>
			{
				value += 2;
				invocation.Proceed();
			});
			var secondInterceptor = new WithCallbackInterceptor(_ =>
			{
				value *= 3;
			});
			var proxy = generator.CreateDelegateProxy<Action>(firstInterceptor, secondInterceptor);

			proxy();

			Assert.AreEqual((1 + 2) * 3, value);
			Assert.AreNotEqual((1 * 3) + 2, value);
		}

		[Test]
		public void CreateDelegateProxy_passes_argument_to_interceptor()
		{
			const int expectedArg = 42;
			object actualArg = null;
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				actualArg = invocation.Arguments[0];
			});
			var proxy = generator.CreateDelegateProxy<Action<int>>(interceptor);

			proxy(expectedArg);

			Assert.AreEqual(expectedArg, actualArg);
		}

		[Test]
		public void CreateDelegateProxyWithTarget_passes_argument_to_target()
		{
			const int expectedArg = 42;
			object actualArg = null;
			Action<int> target = a => actualArg = a;
			var proxy = generator.CreateDelegateProxyWithTarget<Action<int>>(target);

			proxy(expectedArg);

			Assert.AreEqual(expectedArg, actualArg);
		}

		[Test]
		public void CreateDelegateProxy_creates_delegate_that_can_return_value()
		{
			const int expectedReturnValue = 42;
			object actualReturnValue;
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				invocation.ReturnValue = expectedReturnValue;
			});
			var proxy = generator.CreateDelegateProxy<Func<int>>(interceptor);

			actualReturnValue = proxy();

			Assert.AreEqual(expectedReturnValue, actualReturnValue);
		}

		[Test]
		public void CreateDelegateProxyWithTarget_creates_delegate_that_can_return_value()
		{
			const int expectedReturnValue = 42;
			object actualReturnValue;
			Func<int> target = () => expectedReturnValue;
			var proxy = generator.CreateDelegateProxyWithTarget<Func<int>>(target);

			actualReturnValue = proxy();

			Assert.AreEqual(expectedReturnValue, actualReturnValue);
		}

		[Test]
		[ExcludeOnFramework(Framework.NetCore | Framework.NetFramework, "Fails with a MissingMethodException due to a bug in System.Reflection.Emit. See https://github.com/dotnet/corefx/issues/29254.")]
		public void CreateDelegateProxy_creates_delegate_that_can_get_in_parameter_generic_delegate_type()
		{
			int expectedArgAtStartOfInvocation = 13;
			object actualArgAtStartOfInvocation = null;
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				actualArgAtStartOfInvocation = invocation.Arguments[0];
			});
			var proxy = generator.CreateDelegateProxy<ActionWithInParameter<int>>(interceptor);

			proxy(in expectedArgAtStartOfInvocation);

			Assert.AreEqual(expectedArgAtStartOfInvocation, actualArgAtStartOfInvocation);
		}

		[Test]
		public void CreateDelegateProxy_creates_delegate_that_can_get_in_parameter_nongeneric_delegate_type()
		{
			int expectedArgAtStartOfInvocation = 13;
			object actualArgAtStartOfInvocation = null;
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				actualArgAtStartOfInvocation = invocation.Arguments[0];
			});
			var proxy = generator.CreateDelegateProxy<ActionWithInIntParameter>(interceptor);

			proxy(in expectedArgAtStartOfInvocation);

			Assert.AreEqual(expectedArgAtStartOfInvocation, actualArgAtStartOfInvocation);
		}

		[Test]
		public void CreateDelegateProxyWithTarget_creates_delegate_that_can_get_in_parameter_nongeneric_delegate_type()
		{
			int expectedArgAtStartOfInvocation = 13;
			object actualArgAtStartOfInvocation = null;
			ActionWithInIntParameter target = (in int a) => actualArgAtStartOfInvocation = a;
			var proxy = generator.CreateDelegateProxyWithTarget<ActionWithInIntParameter>(target);

			proxy(in expectedArgAtStartOfInvocation);

			Assert.AreEqual(expectedArgAtStartOfInvocation, actualArgAtStartOfInvocation);
		}

		[Test]
		[ExcludeOnFramework(Framework.NetCore | Framework.NetFramework, "Fails with a MissingMethodException due to a bug in System.Reflection.Emit. See https://github.com/dotnet/corefx/issues/29254.")]
		public void CreateDelegateProxy_creates_delegate_that_cannot_set_in_parameter_generic_delegate_type()
		{
			const int argBeforeInvocation = 0;
			const int expectedArgAfterInvocation = argBeforeInvocation;
			int arg = argBeforeInvocation;
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				invocation.SetArgumentValue(0, argBeforeInvocation + 42);
			});
			var proxy = generator.CreateDelegateProxy<ActionWithInParameter<int>>(interceptor);

			proxy(in arg);

			Assert.AreEqual(expectedArgAfterInvocation, arg);
		}

		[Test]
		public void CreateDelegateProxy_creates_delegate_that_cannot_set_in_parameter_nongeneric_delegate_type()
		{
			const int argBeforeInvocation = 0;
			const int expectedArgAfterInvocation = argBeforeInvocation;
			int arg = argBeforeInvocation;
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				invocation.SetArgumentValue(0, argBeforeInvocation + 42);
			});
			var proxy = generator.CreateDelegateProxy<ActionWithInIntParameter>(interceptor);

			proxy(in arg);

			Assert.AreEqual(expectedArgAfterInvocation, arg);
		}

		[Test]
		public void CreateDelegateProxy_creates_delegate_that_can_set_out_parameter()
		{
			const int expectedArgAfterInvocation = 42;
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				invocation.SetArgumentValue(0, expectedArgAfterInvocation);
			});
			var proxy = generator.CreateDelegateProxy<ActionWithOutParameter<int>>(interceptor);

			proxy(out int actualArgAfterInvocation);

			Assert.AreEqual(expectedArgAfterInvocation, actualArgAfterInvocation);
		}

		[Test]
		public void CreateDelegateProxyWithTarget_creates_delegate_that_can_set_out_parameter()
		{
			const int expectedArgAfterInvocation = 42;
			ActionWithOutParameter<int> target = (out int a) => a = expectedArgAfterInvocation;
			var proxy = generator.CreateDelegateProxyWithTarget<ActionWithOutParameter<int>>(target);

			proxy(out int actualArgAfterInvocation);

			Assert.AreEqual(expectedArgAfterInvocation, actualArgAfterInvocation);
		}

		[Test]
		public void CreateDelegateProxy_creates_delegate_that_can_get_ref_parameter()
		{
			int expectedArgAtStartOfInvocation = 13;
			object actualArgAtStartOfInvocation = null;
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				actualArgAtStartOfInvocation = invocation.Arguments[0];
			});
			var proxy = generator.CreateDelegateProxy<ActionWithRefParameter<int>>(interceptor);

			proxy(ref expectedArgAtStartOfInvocation);

			Assert.AreEqual(expectedArgAtStartOfInvocation, actualArgAtStartOfInvocation);
		}

		[Test]
		public void CreateDelegateProxyWithTarget_creates_delegate_that_can_get_ref_parameter()
		{
			int expectedArgAtStartOfInvocation = 13;
			object actualArgAtStartOfInvocation = null;
			ActionWithRefParameter<int> target = (ref int a) => actualArgAtStartOfInvocation = a;
			var proxy = generator.CreateDelegateProxyWithTarget<ActionWithRefParameter<int>>(target);

			proxy(ref expectedArgAtStartOfInvocation);

			Assert.AreEqual(expectedArgAtStartOfInvocation, actualArgAtStartOfInvocation);
		}

		[Test]
		public void CreateDelegateProxy_creates_delegate_that_can_set_ref_parameter()
		{
			const int expectedArgAfterInvocation = 42;
			int actualArgAfterInvocation = 0;
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				invocation.SetArgumentValue(0, expectedArgAfterInvocation);
			});
			var proxy = generator.CreateDelegateProxy<ActionWithRefParameter<int>>(interceptor);

			proxy(ref actualArgAfterInvocation);

			Assert.AreEqual(expectedArgAfterInvocation, actualArgAfterInvocation);
		}

		[Test]
		public void CreateDelegateProxyWithTarget_creates_delegate_that_can_set_ref_parameter()
		{
			const int expectedArgAfterInvocation = 42;
			int actualArgAfterInvocation = 0;
			ActionWithRefParameter<int> target = (ref int a) => a = expectedArgAfterInvocation;
			var proxy = generator.CreateDelegateProxyWithTarget<ActionWithRefParameter<int>>(target);

			proxy(ref actualArgAfterInvocation);

			Assert.AreEqual(expectedArgAfterInvocation, actualArgAfterInvocation);
		}

		public delegate void ActionWithInParameter<T>(in T arg);
		public delegate void ActionWithInIntParameter(in int arg);
		public delegate void ActionWithOutParameter<T>(out T arg);
		public delegate void ActionWithRefParameter<T>(ref T arg);
	}
}
