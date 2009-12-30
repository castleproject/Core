// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using Interfaces;
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
		public void BaseTypeForInterfaceProxyHonored()
		{
			var options = new ProxyGenerationOptions();
			options.BaseTypeForInterfaceProxy = typeof (SimpleClass);
			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof (IService), Type.EmptyTypes, options);
			Assert.NotNull(proxy as SimpleClass);
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

		[Test]
		public void Should_properly_implement_two_interfaces_with_methods_with_identical_signatures()
		{
			object proxy = generator.CreateInterfaceProxyWithoutTarget(typeof (IIdenticalOne), new[] {typeof (IIdenticalTwo)},
			                                                            new DoNothingInterceptor());
			(proxy as IIdenticalOne).Foo();
			(proxy as IIdenticalTwo).Foo();
		}

		[Test]
		public void Should_properly_proxy_class_that_implements_interface_virtually_non_interceptable()
		{
			var proxy = generator.CreateClassProxy(typeof(IdenticalOneVirtual));
			(proxy as IIdenticalOne).Foo();
		}

		[Test]
		public void Should_properly_proxy_class_that_implements_interface_virtually_interceptable()
		{
			var proxy = generator.CreateClassProxy(typeof (IdenticalOneVirtual), new Type[] {typeof (IIdenticalOne)},
			                                       ProxyGenerationOptions.Default);
			(proxy as IIdenticalOne).Foo();
		}

		[Test]
		public void Should_implement_explicitly_duplicate_interface_members()
		{
			Type type =
				generator.CreateInterfaceProxyWithoutTarget(typeof(IIdenticalOne), new[] { typeof(IIdenticalTwo) }).GetType();
			MethodInfo method = type.GetMethod("Foo", BindingFlags.Instance | BindingFlags.Public);
			Assert.IsNotNull(method);
			Assert.AreSame(method, type.GetInterfaceMap(typeof(IIdenticalOne)).TargetMethods[0]);
			MethodInfo method2 = type.GetMethod("IIdenticalTwo.Foo", BindingFlags.Instance | BindingFlags.Public);
			Assert.IsNotNull(method2);
		}

		private ParameterInfo[] GetMyTestMethodParams(Type type)
		{
			MethodInfo methodInfo = type.GetMethod("MyTestMethod", BindingFlags.Instance | BindingFlags.Public);
			return methodInfo.GetParameters();
		}
	}
	public class IdenticalOneVirtual:IIdenticalOne
	{
		public virtual string Foo()
		{
			return "Foo";
		}
	}
}