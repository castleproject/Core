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

	using Interceptors;
	using InterClasses;
	using Mixins;
	using NUnit.Framework;

	/// <summary>
	/// See http://support.castleproject.org/projects/DYNPROXY/issues/view/DYNPROXY-ISSUE-96 for details
	/// </summary>
	[TestFixture]
	public class OrderOfInterfacePrecedenceTestCase:BasePEVerifyTestCase
	{
		[Test]
		public void Same_Interface_on_target_and_mixin_should_forward_to_target()
		{
			var target = new ServiceImpl();
			var mixin = new AlwaysThrowsServiceImpl();
			var proxy = generator.CreateInterfaceProxyWithTarget(typeof (IService), Type.EmptyTypes, target, MixIn(mixin)) as IService;
			Assert.DoesNotThrow(() => proxy.Sum(1, 2));
		}

		[Test]
		public void Same_Interface_on_proxy_withouth_target_and_mixin_should_forward_to_null_target()
		{
			var interceptor = new WithCallbackInterceptor(i =>
			                                              	{
			                                              		Assert.IsNull(i.InvocationTarget);
			                                              		i.ReturnValue = 0;
			                                              	});
			var mixin = new AlwaysThrowsServiceImpl();
			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof (IService), Type.EmptyTypes, MixIn(mixin), interceptor);
			Assert.DoesNotThrow(() => (proxy as IService).Sum(2, 2));
		}

		[Test]
		public void Same_Interface_on_proxy_with_target_interface_and_mixin_should_forward_to_target()
		{
			var target = new ServiceImpl();
			var mixin = new AlwaysThrowsServiceImpl();
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof (IService), target, MixIn(mixin));
			Assert.DoesNotThrow(() => (proxy as IService).Sum(2, 2));
		}

		[Test]
		public void Same_Interface_on_target_of_proxy_with_target_interface_and_mixin_should_forward_to_target()
		{
			var target = new ServiceImpl();
			var mixin = new ServiceImpl();
			IInterceptor interceptor = new WithCallbackInterceptor(i=>
			                                                       	{
			                                                       		Assert.AreSame(target,i.InvocationTarget);
			                                                       		i.ReturnValue = 0;
			                                                       	});
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IService), target, MixIn(mixin),interceptor);
			Assert.DoesNotThrow(() => (proxy as IService).Sum(2, 2));
		}

		[Test]
		public void Same_Interface_on_target_and_additionalInterface_should_forward_to_target()
		{
			var target = new ServiceImpl();
			var proxy = generator.CreateInterfaceProxyWithTarget(typeof (IService), new[] {typeof (IService)}, target) as IService;
			Assert.DoesNotThrow(() => proxy.Sum(1, 2));
		}

		[Test]
		public void Mixin_with_derived_target_interface_forwards_base_to_target_derived_to_mixin()
		{
			var mixin = new ClassImplementingIDerivedSimpleMixin();
			object proxy = generator.CreateInterfaceProxyWithTarget(typeof (ISimpleMixin), new SimpleMixin(), MixIn(mixin));
			Assert.AreEqual(1, (proxy as ISimpleMixin).DoSomething());
			Assert.AreEqual(2, (proxy as IDerivedSimpleMixin).DoSomethingDerived());
		}

		[Test]
		public void Mixin_and_target_implement_additionalInterface_forwards_to_target()
		{
			var mixin = new SimpleMixin();
			object proxy = generator.CreateInterfaceProxyWithTarget(typeof (ISimpleMixin),
			                                                        new[] {typeof (IDerivedSimpleMixin)},
			                                                        new ClassImplementingIDerivedSimpleMixin(),
			                                                        MixIn(mixin));
			Assert.AreEqual(3, (proxy as ISimpleMixin).DoSomething());
			Assert.AreEqual(2, (proxy as IDerivedSimpleMixin).DoSomethingDerived());
		}

		[Test]
		public void Mixin_same_as_additionalInterface_forwards_to_mixin()
		{
			var mixin = new ClassImplementingIDerivedSimpleMixin();
			object proxy = generator.CreateInterfaceProxyWithTarget(typeof (ISimpleMixin),
			                                                        new[] {typeof (IDerivedSimpleMixin)},
			                                                        new SimpleMixin(),
			                                                        MixIn(mixin));
			Assert.AreEqual(1, (proxy as ISimpleMixin).DoSomething());
			Assert.AreEqual(2, (proxy as IDerivedSimpleMixin).DoSomethingDerived());
		}

		[Test]
		public void Mixin_same_as_proxied_class_forwards_to_base()
		{
			var interceptor = new LogInvocationInterceptor();
			var mixin = new ClassImplementingISimpleMixin();
			object proxy = generator.CreateClassProxy(typeof(SimpleMixin), MixIn(mixin), interceptor);
			Assert.AreEqual(1, (proxy as ISimpleMixin).DoSomething());
			Assert.IsEmpty(interceptor.Invocations);
		}

		[Test]
		public void Mixin_same_as_proxied_class_and_additional_interface_forwards_to_base_interceptable()
		{
			var interceptor = new LogInvocationInterceptor();
			var mixin = new ClassImplementingISimpleMixin();
			object proxy = generator.CreateClassProxy(typeof(SimpleMixin), new[] { typeof(ISimpleMixin) }, MixIn(mixin),
													  interceptor);
			Assert.AreEqual(1, (proxy as ISimpleMixin).DoSomething());
			Assert.IsNotEmpty(interceptor.Invocations);
		}

		[Test]
		public void Mixin_with_derived_base_forwards_to_mixin()
		{
			var interceptor = new LogInvocationInterceptor();
			var mixin = new ClassImplementingIDerivedSimpleMixin();
			object proxy = generator.CreateClassProxy(typeof(SimpleMixin), new[] { typeof(IDerivedSimpleMixin) }, MixIn(mixin),
													  interceptor);
			Assert.AreEqual(2, (proxy as IDerivedSimpleMixin).DoSomethingDerived());
			Assert.IsNotEmpty(interceptor.Invocations);
		}


		private ProxyGenerationOptions MixIn(Object mixin)
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(mixin);
			return options;
		}
	}
}