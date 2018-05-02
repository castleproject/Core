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
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Explicit;
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
	}
}