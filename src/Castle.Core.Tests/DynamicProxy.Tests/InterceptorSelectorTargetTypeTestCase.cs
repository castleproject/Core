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
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Tests.Interceptors;

	using NUnit.Framework;

	[TestFixture]
	public class InterceptorSelectorTargetTypeTestCase : BasePEVerifyTestCase
	{
		// NOTE: This fixture does not just contain tests targeting IInterceptorSelector.SelectInterceptors,
		// but also tests targeting IInvocation.TargetType. These tests complement the former set of tests
		// to ensure consistency between the two.

		[Test]
		public void When_using_CreateClassProxy_SelectInterceptors_receives_type_equal_to_proxied_type()
		{
			var selector = new InterceptorSelector();

			var proxy = generator.CreateClassProxy<Foo>(new ProxyGenerationOptions { Selector = selector }, new DoNothingInterceptor());
			proxy.Method();

			Assert.AreEqual(typeof(Foo), selector.ReceivedType);
		}

		[Test]
		public void When_using_CreateClassProxy_Invocation_TargetType_is_equal_to_proxied_type()
		{
			var interceptor = new Interceptor();

			var proxy = generator.CreateClassProxy<Foo>(interceptor);
			proxy.Method();

			Assert.AreEqual(typeof(Foo), interceptor.ReceivedTargetType);
		}

		[Test]
		public void When_using_CreateClassProxyWithTarget_SelectInterceptors_receives_type_equal_to_type_of_target()
		{
			var selector = new InterceptorSelector();

			var proxy = generator.CreateClassProxyWithTarget<Foo>(new FooTarget(), new ProxyGenerationOptions { Selector = selector }, new DoNothingInterceptor());
			proxy.Method();

			Assert.AreEqual(typeof(FooTarget), selector.ReceivedType);
		}

		[Test]
		public void When_using_CreateClassProxyWithTarget_Invocation_TargetType_is_equal_to_type_of_target()
		{
			var interceptor = new Interceptor();

			var proxy = generator.CreateClassProxyWithTarget<Foo>(new FooTarget(), interceptor);
			proxy.Method();

			Assert.AreEqual(typeof(FooTarget), interceptor.ReceivedTargetType);
		}

		[Test]
		public void When_using_CreateInterfaceProxyWithoutTarget_SelectInterceptors_receives_type_equal_to_null()
		{
			var selector = new InterceptorSelector();

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IFoo>(new ProxyGenerationOptions { Selector = selector }, new DoNothingInterceptor());
			proxy.Method();

			Assert.AreEqual(null, selector.ReceivedType);
		}

		[Test]
		public void When_using_CreateInterfaceProxyWithoutTarget_Invocation_TargetType_is_equal_to_null()
		{
			var interceptor = new Interceptor();

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IFoo>(interceptor);
			proxy.Method();

			Assert.AreEqual(null, interceptor.ReceivedTargetType);
		}

		[Test]
		public void When_using_CreateInterfaceProxyWithTarget_SelectInterceptors_receives_type_equal_to_type_of_target()
		{
			var selector = new InterceptorSelector();

			var proxy = generator.CreateInterfaceProxyWithTarget<IFoo>(new FooTarget(), new ProxyGenerationOptions { Selector = selector }, new DoNothingInterceptor());
			proxy.Method();

			Assert.AreEqual(typeof(FooTarget), selector.ReceivedType);
		}

		[Test]
		public void When_using_CreateInterfaceProxyWithTarget_Invocation_TargetType_is_equal_to_type_of_target()
		{
			var interceptor = new Interceptor();

			var proxy = generator.CreateInterfaceProxyWithTarget<IFoo>(new FooTarget(), interceptor);
			proxy.Method();

			Assert.AreEqual(typeof(FooTarget), interceptor.ReceivedTargetType);
		}

		[Test]
		public void When_using_CreateInterfaceProxyWithTargetInterface_SelectInterceptors_receives_type_equal_to_type_of_target()
		{
			var selector = new InterceptorSelector();

			var proxy = generator.CreateInterfaceProxyWithTargetInterface<IFoo>(new FooTarget(), new ProxyGenerationOptions { Selector = selector }, new DoNothingInterceptor());
			proxy.Method();

			Assert.AreEqual(typeof(FooTarget), selector.ReceivedType);
		}

		[Test]
		public void When_using_CreateInterfaceProxyWithTargetInterface_Invocation_TargetType_is_equal_to_type_of_target()
		{
			var interceptor = new Interceptor();

			var proxy = generator.CreateInterfaceProxyWithTargetInterface<IFoo>(new FooTarget(), interceptor);
			proxy.Method();

			Assert.AreEqual(typeof(FooTarget), interceptor.ReceivedTargetType);
		}

		public interface IFoo
		{
			void Method();
		}

		public abstract class Foo : IFoo
		{
			public abstract void Method();
		}

		public sealed class FooTarget : Foo
		{
			public override void Method()
			{
			}
		}

#if FEATURE_SERIALIZATION
		[Serializable]
#endif
		public sealed class InterceptorSelector : IInterceptorSelector
		{
			public Type ReceivedType { get; private set; }

			public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
			{
				this.ReceivedType = type;
				return interceptors;
			}
		}

		public sealed class Interceptor : IInterceptor
		{
			public Type ReceivedTargetType { get; private set; }

			public void Intercept(IInvocation invocation)
			{
				ReceivedTargetType = invocation.TargetType;
			}
		}
	}
}
