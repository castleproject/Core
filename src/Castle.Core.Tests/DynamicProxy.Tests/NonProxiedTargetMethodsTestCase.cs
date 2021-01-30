// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Explicit;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class NonProxiedTargetMethodsTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void Target_method_WithTarget()
		{
			var proxy = generator.CreateInterfaceProxyWithTarget<ISimpleInterface>(new ClassWithInterface(),
			                                                                       new ProxyGenerationOptions(
			                                                                       	new ProxyNothingHook()));
			var result = -1;
			Assert.DoesNotThrow(() => result = proxy.Do());
			Assert.AreEqual(5, result);
		}

		[Test]
		public void Target_method_WithTargetInterface()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface<ISimpleInterface>(new ClassWithInterface(),
			                                                                                new ProxyGenerationOptions(
			                                                                                	new ProxyNothingHook()));
			var result = -1;
			Assert.DoesNotThrow(() => result = proxy.Do());
			Assert.AreEqual(5, result);
		}

		[Test]
		public void Target_method_explicit_WithTarget()
		{
			var proxy = generator.CreateInterfaceProxyWithTarget<ISimpleInterface>(new SimpleInterfaceExplicit(),
			                                                                       new ProxyGenerationOptions(
			                                                                       	new ProxyNothingHook()));
			var result = -1;
			Assert.DoesNotThrow(() => result = proxy.Do());
			Assert.AreEqual(5, result);
		}

		[Test]
		public void Target_method_explicit_WithTargetInterface()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface<ISimpleInterface>(new SimpleInterfaceExplicit(),
			                                                                                new ProxyGenerationOptions(
			                                                                                	new ProxyNothingHook()));
			var result = -1;
			Assert.DoesNotThrow(() => result = proxy.Do());
			Assert.AreEqual(5, result);
		}

		[Test]
		public void Target_method_generic_WithTarget()
		{
			var proxy = generator.CreateInterfaceProxyWithTarget<IGenericInterface>(new GenericClass(),
			                                                                        new ProxyGenerationOptions(
			                                                                        	new ProxyNothingHook()));
			var result = -1;
			Assert.DoesNotThrow(() => result = proxy.GenericMethod<int>());
			Assert.AreEqual(0, result);
		}

		[Test]
		public void Target_method_generic_WithTargetInterface()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface<IGenericInterface>(new GenericClass(),
			                                                                                 new ProxyGenerationOptions(
			                                                                                 	new ProxyNothingHook()));
			var result = -1;
			Assert.DoesNotThrow(() => result = proxy.GenericMethod<int>());
			Assert.AreEqual(0, result);
		}

		[Test]
		public void Target_method_out_ref_parameters_WithTarget()
		{
			var proxy = generator.CreateInterfaceProxyWithTarget<IWithRefOut>(new WithRefOut(),
			                                                                  new ProxyGenerationOptions(
			                                                                  	new ProxyNothingHook()));
			var result = -1;
			Assert.DoesNotThrow(() => proxy.Do(out result));
			Assert.AreEqual(5, result);

			result = -1;
			Assert.DoesNotThrow(() => proxy.Did(ref result));
			Assert.AreEqual(5, result);
		}

		[Test]
		public void Target_method_out_ref_parameters_WithTargetInterface()
		{
			var proxy = generator.CreateInterfaceProxyWithTargetInterface<IWithRefOut>(new WithRefOut(),
			                                                                  new ProxyGenerationOptions(
			                                                                  	new ProxyNothingHook()));
			var result = -1;
			Assert.DoesNotThrow(() => proxy.Do(out result));
			Assert.AreEqual(5, result);

			result = -1;
			Assert.DoesNotThrow(() => proxy.Did(ref result));
			Assert.AreEqual(5, result);
		}

		[Test]
		public void Unproxied_methods_should_pass_through_to_target()
		{
			var target = new HasVirtualStringAutoProperty();

			var options = new ProxyGenerationOptions(
				hook: new ProxySomeMethodsHook(
					shouldInterceptMethod: (_, method) => method.Name == "set_" + nameof(HasVirtualStringAutoProperty.Property)));

			var convertToLowerThenProceed = new WithCallbackInterceptor(invocation =>
			{
				string value = (string)invocation.GetArgumentValue(0);
				string lowerCase = value?.ToLowerInvariant();
				invocation.SetArgumentValue(0, lowerCase);
				invocation.Proceed();
			});

			var proxy = generator.CreateClassProxyWithTarget(target, options, convertToLowerThenProceed);

			proxy.Property = "HELLO WORLD";

			Assert.AreEqual("hello world", target.Property);
			Assert.AreEqual("hello world", proxy.Property);
		}

		[Test]
		public void Unproxied_public_method_should_not_invoke_interceptor()
		{
			var target = new VirtualClassWithMethod();
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			var proxy = generator.CreateClassProxyWithTarget(target, options, new ThrowingInterceptor());
			proxy.Method();  // the hook says "don't proxy anything", so this should not call the throwing interceptor
		}

		[Test]
		public void Unproxied_non_public_method_should_not_invoke_interceptor()
		{
			var target = new ClassWithProtectedMethod();
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			var proxy = generator.CreateClassProxyWithTarget(target, options, new ThrowingInterceptor());
			proxy.PublicMethod();
		}
	}
}