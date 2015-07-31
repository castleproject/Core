// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Interfaces;
	using Castle.InterClasses;

	using Xunit;

	public class InterfaceProxyWithTargetInterfaceTestCase : BasePEVerifyTestCase
	{
		[Fact]
		public void When_target_does_not_implement_additional_interface_method_should_throw()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
				new[] { typeof(ITwo) },
				new One(),
				ProxyGenerationOptions.Default,
				new StandardInterceptor());
			Assert.Throws(typeof(NotImplementedException), () => (proxy as ITwo).TwoMethod());
		}

		[Fact]
		public void When_target_does_implement_additional_interface_method_should_forward()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
				new[] { typeof(ITwo) },
				new OneTwo(),
				ProxyGenerationOptions.Default);
			var result = (proxy as ITwo).TwoMethod();
			Assert.Equal(2, result);
		}

		[Fact]
		public void TargetInterface_methods_should_be_forwarded_to_target()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
				new[] { typeof(ITwo) },
				new OneTwo(),
				ProxyGenerationOptions.Default);
			var result = (proxy as IOne).OneMethod();
			Assert.Equal(3, result);
		}

		[Fact]
		public void Mixin_methods_should_be_forwarded_to_target_if_implements_mixin_interface()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new Two());
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
				new OneTwo(),
				options);
			var result = (proxy as ITwo).TwoMethod();
			Assert.Equal(2, result);
		}

		[Fact]
		public void Invocation_should_be_IChangeInvocationTarget_for_AdditionalInterfaces_methods()
		{
			var interceptor = new ChangeTargetInterceptor(new Two());
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
				new[] { typeof(ITwo) },
				new OneTwo(),
				interceptor);

			var result = (proxy as ITwo).TwoMethod();

			Assert.Equal(20, result);
		}

		[Fact]
		public void Invocation_should_be_IChangeInvocationTarget_for_target_methods()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new Two());
			var interceptor = new ChangeTargetInterceptor(new OneTwo());
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
				new One(),
				options,
				interceptor);
			var result = (proxy as IOne).OneMethod();

			Assert.Equal(3, result);
		}

		[Fact]
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

			Assert.Equal(2, result);
		}

		[Fact]
		public void Null_target_is_valid()
		{
			var interceptor = new ChangeTargetInterceptor(new OneTwo());
			generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
				default(object),
				new ProxyGenerationOptions(),
				interceptor);
		}

		[Fact]
		public void Null_target_can_be_replaced()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new Two());
			var interceptor = new ChangeTargetInterceptor(new OneTwo());
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),
				default(object),
				options,
				interceptor);

			Assert.Equal(3, ((IOne)proxy).OneMethod());
		}

		[Fact]
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

		[Fact]
		public void ChangeProxyTarget_should_change_proxy_target_permanently()
		{
			var interceptor = new ChangeProxyTargetInterceptor(new OneTwo());
			var proxy = generator.CreateInterfaceProxyWithTargetInterface<IOne>(new One(), interceptor);

			proxy.OneMethod();

			var type = GetTargetType(proxy);
			Assert.Equal(typeof(OneTwo), type);
		}

		[Fact]
		public void ChangeProxyTarget_should_not_affect_invocation_target()
		{
			var first = new ChangeProxyTargetInterceptor(new OneTwo());
			var second = new KeepDataInterceptor();
			var proxy = generator.CreateInterfaceProxyWithTargetInterface<IOne>(new One(),
				first,
				second);

			proxy.OneMethod();

			Assert.Equal(typeof(One), second.Invocation.InvocationTarget.GetType());
		}

		[Fact]
		public void Cannot_proxy_inaccessible_interface()
		{
			var exception =
				Assert.Throws<GeneratorException>(
					() => generator.CreateInterfaceProxyWithTargetInterface<PrivateInterface>(new PrivateClass(), new IInterceptor[0]));
			//TODO
			//Assert.That(exception.Message, Is.StringStarting("Can not create proxy for type Castle.DynamicProxy.Tests.InterfaceProxyWithTargetInterfaceTestCase+PrivateInterface because it is not accessible. Make it public, or internal"));
		}

		[Fact]
		public void Cannot_proxy_generic_interface_with_inaccessible_type_argument()
		{
			var exception =
				Assert.Throws<GeneratorException>(
					() =>
						generator.CreateInterfaceProxyWithTargetInterface<IList<PrivateInterface>>(new List<PrivateInterface>(),
							new IInterceptor[0]));
			//TODO
			//Assert.That(exception.Message, Is.StringStarting("Can not create proxy for type System.Collections.Generic.IList`1[[Castle.DynamicProxy.Tests.InterfaceProxyWithTargetInterfaceTestCase+PrivateInterface, Castle.Core.Tests, Version=0.0.0.0, Culture=neutral, PublicKeyToken=407dd0808d44fbdc]] because type Castle.DynamicProxy.Tests.InterfaceProxyWithTargetInterfaceTestCase+PrivateInterface is not accessible. Make it public, or internal"));
		}

		[Fact]
		public void Cannot_proxy_generic_interface_with_type_argument_that_has_inaccessible_type_argument()
		{
			var expected =
				string.Format("Can not create proxy for type {0} because type {1} is not accessible. Make it public, or internal",
					typeof(IList<IList<PrivateInterface>>).FullName, typeof(PrivateInterface).FullName);

			var exception =
				Assert.Throws<GeneratorException>(
					() =>
						generator.CreateInterfaceProxyWithTargetInterface<IList<IList<PrivateInterface>>>(
							new List<IList<PrivateInterface>>(), new IInterceptor[0]));

			Assert.StartsWith(expected, exception.Message);
		}

		[Fact]
		public void Can_proxy_generic_interface()
		{
			generator.CreateInterfaceProxyWithTargetInterface<IList<object>>(new List<object>(), new IInterceptor[0]);
		}

		private Type GetTargetType(object proxy)
		{
			return (proxy as IProxyTargetAccessor).DynProxyGetTarget().GetType();
		}

		private interface PrivateInterface
		{
		}

		private class PrivateClass : PrivateInterface
		{
		}
	}
}