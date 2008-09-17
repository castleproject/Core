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
	using System.Collections;
	using Castle.DynamicProxy.Tests.GenClasses;
	using Castle.DynamicProxy.Tests.Interceptors;
	using NUnit.Framework;

	[TestFixture]
	public class GenericClassProxyTestCase : BasePEVerifyTestCase
	{
		private LogInvocationInterceptor logger;

		public override void Init()
		{
			base.Init();
			logger = new LogInvocationInterceptor();
		}

		[Test]
		public void ProxyWithGenericArgument()
		{
			ClassWithGenArgs<int> proxy = generator.CreateClassProxy<ClassWithGenArgs<int>>(logger);

			Assert.IsNotNull(proxy);

			proxy.DoSomething();

			Assert.IsTrue(proxy.Invoked);

			proxy.AProperty = true;
			Assert.IsTrue(proxy.AProperty);

			Assert.AreEqual("DoSomething set_AProperty get_AProperty ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenericArguments()
		{
			ClassWithGenArgs<int, string> proxy = generator.CreateClassProxy<ClassWithGenArgs<int, string>>(logger);

			Assert.IsNotNull(proxy);

			proxy.DoSomething();

			Assert.IsTrue(proxy.Invoked);

			proxy.AProperty = true;
			Assert.IsTrue(proxy.AProperty);

			Assert.AreEqual("DoSomething set_AProperty get_AProperty ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenericArgumentsWithBaseGenericClass()
		{
			SubClassWithGenArgs<int, string, int> proxy =
				generator.CreateClassProxy<SubClassWithGenArgs<int, string, int>>(logger);

			Assert.IsNotNull(proxy);

			proxy.DoSomething();

			Assert.IsTrue(proxy.Invoked);

			proxy.AProperty = true;
			Assert.IsTrue(proxy.AProperty);

			Assert.AreEqual("DoSomething set_AProperty get_AProperty ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenericArgumentsAndArgumentConstraints()
		{
			GenClassWithConstraints<int> proxy = generator.CreateClassProxy<GenClassWithConstraints<int>>(logger);

			Assert.IsNotNull(proxy);

			proxy.DoSomething();

			Assert.IsTrue(proxy.Invoked);

			Assert.AreEqual("DoSomething ", logger.LogContents);
		}

		[Test]
		public void GenericProxyWithIndexer()
		{
			object proxy = generator.CreateClassProxy<ClassWithIndexer<string, int>>(logger);

			Assert.IsNotNull(proxy);

			ClassWithIndexer<string, int> type = (ClassWithIndexer<string, int>) proxy;

			type["name"] = 10;
			Assert.AreEqual(10, type["name"]);

			Assert.AreEqual("set_Item get_Item ", logger.LogContents);
		}

#if !MONO

		[Test]
		public void ProxyWithGenericArgumentsAndMethodGenericArguments()
		{
			GenClassWithGenMethods<ArrayList> proxy =
				generator.CreateClassProxy<GenClassWithGenMethods<ArrayList>>(logger);

			Assert.IsNotNull(proxy);

			proxy.DoSomething("z param");

			Assert.IsTrue(proxy.Invoked);
			Assert.AreEqual("z param", proxy.SavedParam);
			Assert.AreEqual("DoSomething ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenericArgumentsAndMethodGenericArgumentsWithConstraints()
		{
			GenClassWithGenMethodsConstrained<ArrayList> proxy =
				generator.CreateClassProxy<GenClassWithGenMethodsConstrained<ArrayList>>(logger);

			Assert.IsNotNull(proxy);

			proxy.DoSomething("z param");

			Assert.IsTrue(proxy.Invoked);
			Assert.AreEqual("z param", proxy.SavedParam);
			Assert.AreEqual("DoSomething ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenericArgumentsAndMethodGenericArgumentsWithOneNotDefinedOnType()
		{
			GenClassWithGenMethods<ArrayList> proxy =
				generator.CreateClassProxy<GenClassWithGenMethods<ArrayList>>(logger);

			Assert.IsNotNull(proxy);

			int value1 = 10;

			proxy.DoSomethingElse<string>(delegate(int param1) { return param1.ToString(); }, value1);

			Assert.IsTrue(proxy.Invoked);
			Assert.AreEqual("10", proxy.SavedParam);
			Assert.AreEqual("DoSomethingElse ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenericArgumentsAndMethodGenericReturn()
		{
			GenClassWithGenReturn<ArrayList, Hashtable> proxy =
				generator.CreateClassProxy<GenClassWithGenReturn<ArrayList, Hashtable>>(logger);

			Assert.IsNotNull(proxy);

			object ret1 = proxy.DoSomethingT();
			object ret2 = proxy.DoSomethingZ();

			Assert.IsInstanceOfType(typeof (ArrayList), ret1);
			Assert.IsInstanceOfType(typeof (Hashtable), ret2);
			Assert.AreEqual("DoSomethingT DoSomethingZ ", logger.LogContents);
		}

		[Test]
		public void GenericMethodArgumentsAndTypeGenericArgumentsWithSameName()
		{
			GenClassNameClash<ArrayList, Hashtable> proxy =
				generator.CreateClassProxy<GenClassNameClash<ArrayList, Hashtable>>(logger);

			Assert.IsNotNull(proxy);

			proxy.DoSomethingT<int>(1);
			proxy.DoSomethingZ<long>(1L);
			proxy.DoSomethingTX<int, string>(1, "a");
			proxy.DoSomethingZX<long, string>(1L, "b");

			Assert.AreEqual("DoSomethingT DoSomethingZ DoSomethingTX DoSomethingZX ", logger.LogContents);
		}

		[Test]
		public void ClassWithGenMethodOnly()
		{
			OnlyGenMethodsClass proxy =
				generator.CreateClassProxy<OnlyGenMethodsClass>(logger);

			Assert.IsNotNull(proxy);

			proxy.DoSomething(new ArrayList());

			Assert.IsTrue(proxy.Invoked);
			Assert.AreEqual("DoSomething ", logger.LogContents);
		}

		[Test]
		public void MethodInfoClosedInGenTypeGenMethodRefType()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			GenClassWithGenMethods<ArrayList> proxy = generator.CreateClassProxy<GenClassWithGenMethods<ArrayList>>(interceptor);

			proxy.DoSomething(1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof (ArrayList),
			                                           typeof (int));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new Hashtable());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof (ArrayList),
			                                           typeof (Hashtable));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		public void MethodInfoClosedInGenTypeGenMethodValueType()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			GenClassWithGenMethods<int> proxy = generator.CreateClassProxy<GenClassWithGenMethods<int>>(interceptor);

			proxy.DoSomething(1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof (int), typeof (int));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new Hashtable());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof (int),
			                                           typeof (Hashtable));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		public void MethodInfoClosedInGenTypeNongenMethodRefTypeRefType()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			GenClassWithGenReturn<ArrayList, ArrayList> proxy =
				generator.CreateClassProxy<GenClassWithGenReturn<ArrayList, ArrayList>>(interceptor);

			proxy.DoSomethingT();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof (ArrayList));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomethingZ();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof (ArrayList));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		public void MethodInfoClosedInGenTypeNongenMethodValueTypeValueType()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			GenClassWithGenReturn<int, int> proxy = generator.CreateClassProxy<GenClassWithGenReturn<int, int>>(interceptor);

			proxy.DoSomethingT();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof (int));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomethingZ();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof (int));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		public void MethodInfoClosedInGenTypeNongenMethodValueTypeRefType()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			GenClassWithGenReturn<int, ArrayList> proxy =
				generator.CreateClassProxy<GenClassWithGenReturn<int, ArrayList>>(interceptor);

			proxy.DoSomethingT();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof (int));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomethingZ();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof (ArrayList));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		public void MethodInfoClosedInNongenTypeGenMethod()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			OnlyGenMethodsClass proxy = generator.CreateClassProxy<OnlyGenMethodsClass>(interceptor);

			proxy.DoSomething(1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof (int), typeof (int));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new Hashtable());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof (Hashtable),
			                                           typeof (Hashtable));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void ThrowsWhenProxyingGenericTypeDefNoTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			object o = generator.CreateClassProxy(typeof (GenClassWithGenReturn<,>), interceptor);
		}

#endif
	}
}