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
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Interfaces;
	using NUnit.Framework;

	[TestFixture]
	public class InterfaceProxyWithTargetInterfaceTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void When_target_does_not_implement_additional_interface_method_should_throw()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
			                                                              new[] {typeof(ITwo)},
			                                                              new One(),
			                                                              ProxyGenerationOptions.Default,
			                                                              new StandardInterceptor());
			Assert.Throws(typeof(NotImplementedException), () => (proxy as ITwo).TwoMethod());
		}

		[Test]
		public void When_target_does_implement_additional_interface_method_should_forward()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
			                                                              new[] {typeof(ITwo)},
			                                                              new OneTwo(),
			                                                              ProxyGenerationOptions.Default);
			var result = (proxy as ITwo).TwoMethod();
			Assert.AreEqual(2, result);
		}

		[Test]
		public void TargetInterface_methods_should_be_forwarded_to_target()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
			                                                              new[] {typeof(ITwo)},
			                                                              new OneTwo(),
			                                                              ProxyGenerationOptions.Default);
			var result = (proxy as IOne).OneMethod();
			Assert.AreEqual(3, result);
		}

		[Test]
		public void Mixin_methods_should_be_forwarded_to_target_if_implements_mixin_interface()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new Two());
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
			                                                              new OneTwo(),
			                                                              options);
			var result = (proxy as ITwo).TwoMethod();
			Assert.AreEqual(2, result);
		}

		[Test]
		public void Invocation_should_be_IChangeInvocationTarget_for_Mixin_methods()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new Two());
			var interceptor = new ChangeTargetInterceptor(new OneTwo());
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
			                                                              new One(),
			                                                              options,
			                                                              interceptor);

			var result = (proxy as ITwo).TwoMethod();

			Assert.AreEqual(2, result);
		}

		[Test]
		public void Null_target_is_valid()
		{
			var interceptor = new ChangeTargetInterceptor(new OneTwo());
			generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
			                                                  default(object),
			                                                  new ProxyGenerationOptions(),
			                                                  interceptor);
		}

		[Test]
		public void Null_target_can_be_replaced()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new Two());
			var interceptor = new ChangeTargetInterceptor(new OneTwo());
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
			                                                              default(object),
			                                                              options,
			                                                              interceptor);

			Assert.AreEqual(3, ((IOne) proxy).OneMethod());
		}

		[Test]
		public void Should_detect_and_report_setting_null_as_target_for_Mixin_methods()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new Two());
			var interceptor = new ChangeTargetInterceptor(null);
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
			                                                              new One(),
			                                                              options,
			                                                              interceptor);

			Assert.Throws(typeof(NotImplementedException), () =>
			                                               (proxy as ITwo).TwoMethod());
		}

		[Test]
		public void ChangeProxyTarget_should_change_proxy_target_permanently()
		{
			var interceptor = new ChangeProxyTargetInterceptor(new OneTwo());
			var proxy = generator.CreateInterfaceProxyWithTargetInterface<IOne>(new One(), interceptor);

			proxy.OneMethod();

			var type = GetTargetType(proxy);
			Assert.AreEqual(typeof(OneTwo), type);
		}

		[Test]
		public void ChangeProxyTarget_should_not_affect_invocation_target()
		{
			var first = new ChangeProxyTargetInterceptor(new OneTwo());
			var second = new KeepDataInterceptor();
			var proxy = generator.CreateInterfaceProxyWithTargetInterface<IOne>(new One(),
			                                                                    first,
			                                                                    second);

			proxy.OneMethod();

			Assert.AreEqual(typeof(One), second.Invocation.InvocationTarget.GetType());
		}

		private Type GetTargetType(object proxy)
		{
			return (proxy as IProxyTargetAccessor).DynProxyGetTarget().GetType();
		}
	}
}