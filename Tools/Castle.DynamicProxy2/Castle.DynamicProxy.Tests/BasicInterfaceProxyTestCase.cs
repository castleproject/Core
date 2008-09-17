// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Tests.BugsReported;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.InterClasses;
	using NUnit.Framework;

	[TestFixture]
	public class BasicInterfaceProxyTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void BasicInterfaceProxyWithValidTarget()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			IService service = (IService)
			                   generator.CreateInterfaceProxyWithTarget(
			                   	typeof (IService), new ServiceImpl(), logger);

			Assert.AreEqual(3, service.Sum(1, 2));

			Assert.AreEqual("Sum ", logger.LogContents);
		}

		[Test]
		public void Caching()
		{
			IService service = (IService)
			                   generator.CreateInterfaceProxyWithTarget(
			                   	typeof (IService), new ServiceImpl(), new StandardInterceptor());
			service = (IService)
			          generator.CreateInterfaceProxyWithTarget(
			          	typeof (IService), new ServiceImpl(), new StandardInterceptor());
			service = (IService)
			          generator.CreateInterfaceProxyWithTarget(
			          	typeof (IService), new ServiceImpl(), new StandardInterceptor());
			service = (IService)
			          generator.CreateInterfaceProxyWithTarget(
			          	typeof (IService), new ServiceImpl(), new StandardInterceptor());
		}

		[Test]
		public void BasicInterfaceProxyWithValidTarget2()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			IService2 service = (IService2)
			                    generator.CreateInterfaceProxyWithTarget(
			                    	typeof (IService2), new Service2(), logger);

			service.DoOperation2();

			Assert.AreEqual("DoOperation2 ", logger.LogContents);
		}

		[Test]
		public void InterfaceInheritance()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			IService service = (IExtendedService)
			                   generator.CreateInterfaceProxyWithTarget(
			                   	typeof (IExtendedService), new ServiceImpl(), logger);

			Assert.AreEqual(3, service.Sum(1, 2));

			Assert.AreEqual("Sum ", logger.LogContents);
		}

		[Test]
		public void Indexer()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			InterfaceWithIndexer service = (InterfaceWithIndexer)
			                               generator.CreateInterfaceProxyWithTarget(
			                               	typeof (InterfaceWithIndexer), new ClassWithIndexer(), logger);

			Assert.AreEqual(1, service[1]);

			Assert.AreEqual("get_Item ", logger.LogContents);
		}

		[Test]
		public void ProxyTypeWithMultiDimentionalArrayAsParameter()
		{
			generator.CreateInterfaceProxyWithTarget<IClassWithMultiDimentionalArray>(
				new ClassWithMultiDimentionalArray(),
				new LogInvocationInterceptor());
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void CantCreateInterfaceTargetedProxyWithoutInterface()
		{
			IService2 service = (IService2)
			                    generator.CreateInterfaceProxyWithTargetInterface(
			                    	typeof (Service2), new Service2());
		}

		[Test]
		public void InterfaceTargetTypeProducesInvocationsThatCanChangeTarget()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();
			AssertCanChangeTargetInterceptor invocationChecker = new AssertCanChangeTargetInterceptor();

			IService2 service = (IService2)
			                    generator.CreateInterfaceProxyWithTargetInterface(
			                    	typeof (IService2), new Service2(), invocationChecker, logger);

			service.DoOperation2();

			Assert.AreEqual("DoOperation2 ", logger.LogContents);
		}

		[Test]
		public void ChangingInvocationTargetSucceeds()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			IService service = (IService)
			                   generator.CreateInterfaceProxyWithTargetInterface(
			                   	typeof (IService), new AlwaysThrowsServiceImpl(), new ChangeTargetInterceptor(new ServiceImpl()),
			                   	logger);

			Assert.AreEqual(20, service.Sum(10, 10));
		}


		/// <summary>
		/// See http://support.castleproject.org/browse/DYNPROXY-43
		/// </summary>
		[Test]
		public void MethodParamNamesAreReplicated()
		{
			// Try with interface proxy (with target)
			IMyInterface i = generator.CreateInterfaceProxyWithTarget(typeof (IMyInterface), new MyClass(),
			                                                          new StandardInterceptor()) as IMyInterface;

			ParameterInfo[] methodParams = GetMyTestMethodParams(i.GetType());
			Assert.AreEqual("myParam", methodParams[0].Name);
		}

		private ParameterInfo[] GetMyTestMethodParams(Type type)
		{
			MethodInfo methodInfo = type.GetMethod("MyTestMethod");
			return methodInfo.GetParameters();
		}
	}
}