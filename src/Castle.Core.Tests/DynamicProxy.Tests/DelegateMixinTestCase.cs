// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
	using System;
	using System.Reflection;

	using NUnit.Framework;

	[TestFixture]
	public class DelegateMixinTestCase  : BasePEVerifyTestCase
	{
		[Test]
		public void ProxyGenerationOptions_AddDelegateTypeMixin_when_given_null_throws_ArgumentNullException()
		{
			var options = new ProxyGenerationOptions();
			Assert.Throws<ArgumentNullException>(() => options.AddDelegateTypeMixin(null));
		}

		[Test]
		public void ProxyGenerationOptions_AddDelegateTypeMixin_when_given_non_delegate_type_throws_ArgumentException()
		{
			var options = new ProxyGenerationOptions();
			Assert.Throws<ArgumentException>(() => options.AddDelegateTypeMixin(typeof(Exception)));
		}

		[Test]
		public void ProxyGenerationOptions_AddDelegateTypeMixin_when_given_delegate_type_succeeds()
		{
			var options = new ProxyGenerationOptions();
			options.AddDelegateTypeMixin(typeof(Action));
		}

		[Test]
		public void ProxyGenerationOptions_AddDelegateMixin_when_given_null_throws_ArgumentNullException()
		{
			var options = new ProxyGenerationOptions();
			Assert.Throws<ArgumentNullException>(() => options.AddDelegateMixin(null));
		}

		[Test]
		public void ProxyGenerationOptions_AddDelegateMixin_when_given_delegate_succeeds()
		{
			var options = new ProxyGenerationOptions();
			options.AddDelegateMixin(new Action(() => { }));
		}


		[Test]
		public void ProxyGenerator_CreateClassProxy_can_create_delegate_proxy_without_target()
		{
			var options = new ProxyGenerationOptions();
			options.AddDelegateTypeMixin(typeof(Action));
			var _ = new Interceptor();
			var proxy = generator.CreateClassProxy(typeof(object), options, _);
		}

		[Test]
		public void ProxyGenerator_CreateInterfaceProxyWithoutTarget_can_create_delegate_proxy_without_target()
		{
			var options = new ProxyGenerationOptions();
			options.AddDelegateTypeMixin(typeof(Action));
			var _ = new Interceptor();
			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IComparable), options, _);
		}

		[Test]
		public void ProxyGenerator_CreateInterfaceProxyWithTarget_can_create_delegate_proxy_without_target()
		{
			var target = new Target();

			var options = new ProxyGenerationOptions();
			options.AddDelegateTypeMixin(typeof(Action));
			var _ = new Interceptor();
			var proxy = generator.CreateInterfaceProxyWithTarget(typeof(IComparable), target, options, _);
		}

		[Test]
		public void ProxyGenerator_CreateClassProxy_can_create_callable_delegate_proxy_without_target()
		{
			var options = new ProxyGenerationOptions();
			options.AddDelegateTypeMixin(typeof(Action));

			var interceptor = new Interceptor();

			var proxy = generator.CreateClassProxy(typeof(object), options, interceptor);
			var action = ProxyUtil.CreateDelegateToMixin<Action>(proxy);
			Assert.NotNull(action);

			action.Invoke();
			Assert.AreSame(typeof(Action).GetTypeInfo().GetMethod("Invoke"), interceptor.LastInvocation.Method);
		}

		[Test]
		public void ProxyGenerator_CreateInterfaceProxyWithoutTarget_can_create_callable_delegate_proxy_without_target()
		{
			var options = new ProxyGenerationOptions();
			options.AddDelegateTypeMixin(typeof(Action));

			var interceptor = new Interceptor();

			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IComparable), options, interceptor);
			var action = ProxyUtil.CreateDelegateToMixin<Action>(proxy);
			Assert.NotNull(action);

			action.Invoke();
			Assert.AreSame(typeof(Action).GetTypeInfo().GetMethod("Invoke"), interceptor.LastInvocation.Method);
		}

		[Test]
		public void ProxyGenerator_CreateInterfaceProxyWithTarget_can_create_callable_delegate_proxy_without_target()
		{
			var target = new Target();

			var options = new ProxyGenerationOptions();
			options.AddDelegateTypeMixin(typeof(Action));

			var interceptor = new Interceptor();

			var proxy = generator.CreateInterfaceProxyWithTarget(typeof(IComparable), target, options, interceptor);
			var action = ProxyUtil.CreateDelegateToMixin<Action>(proxy);
			Assert.NotNull(action);

			action.Invoke();
			Assert.AreSame(typeof(Action).GetTypeInfo().GetMethod("Invoke"), interceptor.LastInvocation.Method);
		}

		[Test]
		public void ProxyGenerator_CreateClassProxy_cannot_proceed_to_delegate_type_mixin()
		{
			var options = new ProxyGenerationOptions();
			options.AddDelegateTypeMixin(typeof(Action));

			var interceptor = new Interceptor(shouldProceed: true);

			var proxy = generator.CreateClassProxy(typeof(object), options, interceptor);
			var action = ProxyUtil.CreateDelegateToMixin<Action>(proxy);
			Assert.NotNull(action);

			Assert.Throws<NotImplementedException>(() => action.Invoke());
		}

		[Test]
		public void ProxyGenerator_CreateInterfaceProxyWithoutTarget_cannot_proceed_to_delegate_type_mixin()
		{
			var options = new ProxyGenerationOptions();
			options.AddDelegateTypeMixin(typeof(Action));

			var interceptor = new Interceptor(shouldProceed: true);

			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IComparable), options, interceptor);
			var action = ProxyUtil.CreateDelegateToMixin<Action>(proxy);
			Assert.NotNull(action);

			Assert.Throws<NotImplementedException>(() => action.Invoke());
		}

		[Test]
		public void ProxyGenerator_CreateInterfaceProxyWithTarget_cannot_proceed_to_delegate_type_mixin()
		{
			var target = new Target();

			var options = new ProxyGenerationOptions();
			options.AddDelegateTypeMixin(typeof(Action));

			var interceptor = new Interceptor(shouldProceed: true);

			var proxy = generator.CreateInterfaceProxyWithTarget(typeof(IComparable), target, options, interceptor);
			var action = ProxyUtil.CreateDelegateToMixin<Action>(proxy);
			Assert.NotNull(action);

			Assert.Throws<NotImplementedException>(() => action.Invoke());
		}

		[Test]
		public void ProxyGenerator_CreateClassProxy_can_proceed_to_delegate_mixin()
		{
			var target = new Target();

			var options = new ProxyGenerationOptions();
			options.AddDelegateMixin(new Action(target.Method));

			var interceptor = new Interceptor(shouldProceed: true);

			var proxy = generator.CreateClassProxy(typeof(object), options, interceptor);
			var action = ProxyUtil.CreateDelegateToMixin<Action>(proxy);
			Assert.NotNull(action);

			action.Invoke();
			Assert.True(target.MethodInvoked);
		}

		[Test]
		public void ProxyGenerator_CreateInterfaceProxyWithoutTarget_can_proceed_to_delegate_mixin()
		{
			var target = new Target();

			var options = new ProxyGenerationOptions();
			options.AddDelegateMixin(new Action(target.Method));

			var interceptor = new Interceptor(shouldProceed: true);

			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IComparable), options, interceptor);
			var action = ProxyUtil.CreateDelegateToMixin<Action>(proxy);
			Assert.NotNull(action);

			action.Invoke();
			Assert.True(target.MethodInvoked);
		}

		[Test]
		public void ProxyGenerator_CreateInterfaceProxyWithTarget_can_proceed_to_delegate_mixin()
		{
			var target = new Target();

			var options = new ProxyGenerationOptions();
			options.AddDelegateMixin(new Action(target.Method));

			var interceptor = new Interceptor(shouldProceed: true);

			var proxy = generator.CreateInterfaceProxyWithTarget(typeof(IComparable), target, options, interceptor);
			var action = ProxyUtil.CreateDelegateToMixin<Action>(proxy);
			Assert.NotNull(action);

			action.Invoke();
			Assert.True(target.MethodInvoked);
		}

		[Test]
		public void Can_mixin_several_different_delegate_types_simultaneously()
		{
			var options = new ProxyGenerationOptions();
			options.AddDelegateTypeMixin(typeof(Action));
			options.AddDelegateTypeMixin(typeof(Action<int>));

			var interceptor = new Interceptor();

			var proxy = generator.CreateClassProxy(typeof(object), options, interceptor);

			var action = ProxyUtil.CreateDelegateToMixin<Action>(proxy);
			Assert.NotNull(action);
			action.Invoke();

			var intAction = ProxyUtil.CreateDelegateToMixin<Action<int>>(proxy);
			Assert.NotNull(action);
			intAction.Invoke(42);
		}

		[Test]
		public void Cannot_mixin_several_delegate_types_with_same_signature()
		{
			var options = new ProxyGenerationOptions();
			options.AddDelegateTypeMixin(typeof(Func<Exception, bool>));
			options.AddDelegateTypeMixin(typeof(Predicate<Exception>));
			Assert.Throws<InvalidMixinConfigurationException>(() => options.Initialize());

		}

		[Serializable]
		public sealed class Target : IComparable
		{
			public bool CompareToInvoked { get; set; }

			public bool MethodInvoked { get; set; }

			public int CompareTo(object obj)
			{
				CompareToInvoked = true;
				return 123;
			}

			public void Method()
			{
				MethodInvoked = true;
			}
		}

		private sealed class Interceptor : IInterceptor
		{
			private readonly bool shouldProceed;

			public Interceptor(bool shouldProceed = false)
			{
				this.shouldProceed = shouldProceed;
			}

			public IInvocation LastInvocation { get; set; }

			public void Intercept(IInvocation invocation)
			{
				LastInvocation = invocation;
				if (shouldProceed)
				{
					invocation.Proceed();
				}
			}
		}
	}
}
