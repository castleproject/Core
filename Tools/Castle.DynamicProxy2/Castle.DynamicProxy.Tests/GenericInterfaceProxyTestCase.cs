// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using Castle.DynamicProxy.Tests.GenInterfaces;
	using Castle.DynamicProxy.Tests.Interceptors;
	using NUnit.Framework;

	[TestFixture]
	public class GenericInterfaceProxyTestCase : BasePEVerifyTestCase
	{
		private ProxyGenerator generator;
		private LogInvocationInterceptor logger;

		[SetUp]
		public void Init()
		{
			generator = new ProxyGenerator();
			logger = new LogInvocationInterceptor();
		}

		[Test]
		public void ProxyWithGenericArgument()
		{
			GenInterface<int> proxy = 
				generator.CreateInterfaceProxyWithTarget<GenInterface<int>>(
					new GenInterfaceImpl<int>(), logger);

			Assert.IsNotNull(proxy);

			Assert.AreEqual(1, proxy.DoSomething(1));

			Assert.AreEqual("DoSomething ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenericArgumentAndGenericMethod()
		{
			GenInterfaceWithGenMethods<int> proxy =
				generator.CreateInterfaceProxyWithTarget<GenInterfaceWithGenMethods<int>>(
					new GenInterfaceWithGenMethodsImpl<int>(), logger);

			Assert.IsNotNull(proxy);

			proxy.DoSomething<long>(10L, 1);

			Assert.AreEqual("DoSomething ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenericArgumentAndGenericMethodAndGenericReturn()
		{
			GenInterfaceWithGenMethodsAndGenReturn<int> proxy =
				generator.CreateInterfaceProxyWithTarget<GenInterfaceWithGenMethodsAndGenReturn<int>>(
					new GenInterfaceWithGenMethodsAndGenReturnImpl<int>(), logger);

			Assert.IsNotNull(proxy);

			Assert.AreEqual(10L, proxy.DoSomething<long>(10L, 1));

			Assert.AreEqual("DoSomething ", logger.LogContents);
		}
		
		[Test]
		public void ProxyWithGenInterfaceWithGenericTypes()
		{
			GenInterfaceWithGenericTypes proxy =
				generator.CreateInterfaceProxyWithTarget<GenInterfaceWithGenericTypes>(
					new GenInterfaceWithGenericTypesImpl(), logger);

			Assert.IsNotNull(proxy);

			Assert.IsNotNull(proxy.Find(""));
			Assert.IsNotNull(proxy.Find<String>(""));
			
			proxy.Populate<String>(new List<String>());

			Assert.AreEqual("Find Find Populate ", logger.LogContents);

		}

		[Test]
		public void ProxyWithGenInterfaceWithGenericArrays()
		{
			IGenInterfaceWithGenArray<int> proxy =
				generator.CreateInterfaceProxyWithTarget<IGenInterfaceWithGenArray<int>>(
					new GenInterfaceWithGenArray<int>(), logger);

			Assert.IsNotNull(proxy);

			int[] items = new int[] { 1,2,3 };
			proxy.CopyTo(items);
			items = proxy.CreateItems();
			Assert.IsNotNull(items);
			Assert.AreEqual(3, items.Length);

			Assert.AreEqual("CopyTo CreateItems ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenInterfaceWithBase()
		{
			IGenInterfaceHierarchySpecialization<int> proxy =
				generator.CreateInterfaceProxyWithTarget<IGenInterfaceHierarchySpecialization<int>>(
					new GenInterfaceHierarchy<int>(), logger);

			Assert.IsNotNull(proxy);

			proxy.Add();
			proxy.Add(1);
			Assert.IsNotNull(proxy.FetchAll());

			Assert.AreEqual("Add Add FetchAll ", logger.LogContents);
		}

		[Test]
		[ExpectedException(typeof(GeneratorException), "DynamicProxy cannot create an interface (with target) proxy for 'InterfaceWithExplicitImpl`1' as the target 'GenExplicitImplementation`1' has an explicit implementation of one of the methods exposed by the interface. The runtime prevents use from invoking the private method on the target. Method Castle.DynamicProxy.Tests.GenInterfaces.InterfaceWithExplicitImpl<T>.GetEnum1")]
		public void ProxyWithGenExplicitImplementation()
		{
			generator.CreateInterfaceProxyWithTarget<InterfaceWithExplicitImpl<int>>(
				new GenExplicitImplementation<int>(), logger);
		}
	}
}