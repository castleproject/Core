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
	using System.Reflection.Emit;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Tests.BugsReported;
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.InterClasses;
	using ClassWithIndexer = Castle.DynamicProxy.Tests.Classes.ClassWithIndexer;
	using Castle.DynamicProxy.Generators.Emitters;
	using NUnit.Framework;
	using System.Collections.Generic;

	[TestFixture]
	public class BasicClassProxyTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void ProxyForClass()
		{
			object proxy = generator.CreateClassProxy(typeof(ServiceClass), new ResultModifierInterceptor());

			Assert.IsNotNull(proxy);
			Assert.IsTrue(typeof(ServiceClass).IsAssignableFrom(proxy.GetType()));

			ServiceClass instance = (ServiceClass)proxy;

			// return value is changed by the interceptor
			Assert.AreEqual(44, instance.Sum(20, 25));

			// return value is changed by the interceptor
			Assert.AreEqual(true, instance.Valid);

			Assert.AreEqual(45, instance.Sum((byte)20, (byte)25)); // byte
			Assert.AreEqual(45, instance.Sum(20L, 25L)); // long
			Assert.AreEqual(45, instance.Sum((short)20, (short)25)); // short
			Assert.AreEqual(45, instance.Sum(20f, 25f)); // float
			Assert.AreEqual(45, instance.Sum(20.0, 25.0)); // double
			Assert.AreEqual(45, instance.Sum((ushort)20, (ushort)25)); // ushort
			Assert.AreEqual(45, instance.Sum((uint)20, (uint)25)); // uint
			Assert.AreEqual(45, instance.Sum((ulong)20, (ulong)25)); // ulong
		}

		[Test]
		public void Caching()
		{
			object proxy = generator.CreateClassProxy(
				typeof(ServiceClass), new StandardInterceptor());
			proxy = generator.CreateClassProxy(
				typeof(ServiceClass), new StandardInterceptor());
			proxy = generator.CreateClassProxy(
				typeof(ServiceClass), new StandardInterceptor());
			proxy = generator.CreateClassProxy(
				typeof(ServiceClass), new StandardInterceptor());
		}

#if !MONO

		[Test, ExpectedException(typeof(GeneratorException), ExpectedMessage = "Type is not public, so a proxy " +
																				"cannot be generated. Type: System.AppDomainInitializerInfo"
			)]
		public void ProxyForNonPublicClass()
		{
			// have to use a type that is not from this assembly, because it is marked as internals visible to 
			// DynamicProxy2

			object proxy = generator.CreateClassProxy(
				Type.GetType("System.AppDomainInitializerInfo, mscorlib"), new StandardInterceptor());
		}

#endif

		[Test]
		public void ProxyForClassWithIndexer()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			object proxy = generator.CreateClassProxy(typeof(ClassWithIndexer), logger);

			Assert.IsNotNull(proxy);
			Assert.IsInstanceOf(typeof(ClassWithIndexer), proxy);

			ClassWithIndexer type = (ClassWithIndexer)proxy;

			type["name"] = 10;
			Assert.AreEqual(10, type["name"]);

			Assert.AreEqual("set_Item get_Item ", logger.LogContents);
		}

#if !MONO

		[Test]
		public void ClassWithDifferentAccessLevelOnProperties()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			object proxy = generator.CreateClassProxy(typeof(DiffAccessLevelOnProperties), logger);

			Assert.IsNotNull(proxy);
			Assert.IsInstanceOf(typeof(DiffAccessLevelOnProperties), proxy);

			DiffAccessLevelOnProperties type = (DiffAccessLevelOnProperties)proxy;

			type.SetProperties();

			Assert.AreEqual("10 11 12 13 name", type.ToString());
		}

#endif

		[Test]
		public void GetPropertyByReflectionTest()
		{
			object proxy = generator.CreateClassProxy(
				typeof(ServiceClass), new StandardInterceptor());

			try
			{
				Assert.IsFalse((bool)proxy.GetType().GetProperty("Valid").GetValue(proxy, null),
							   "check reflected property is true");
			}
			catch (AmbiguousMatchException)
			{
				// this exception is acceptible if the current runtime doesn't
				// have .NET 2.0 SP1 installed
				// we'd try to grab a method info that in in .NET 2.0 SP1, and if it's
				// not present then we'd ignore that exception
				MethodInfo newDefinePropertyMethodInfo = typeof(TypeBuilder).GetMethod("DefineProperty", new Type[]
				                                                                                          	{
				                                                                                          		typeof (string),
				                                                                                          		typeof (
				                                                                                          			PropertyAttributes),
				                                                                                          		typeof (
				                                                                                          			CallingConventions),
				                                                                                          		typeof (Type),
				                                                                                          		typeof (Type[]),
				                                                                                          		typeof (Type[]),
				                                                                                          		typeof (Type[]),
				                                                                                          		typeof (Type[][]),
				                                                                                          		typeof (Type[][])
				                                                                                          	});

				bool net20SP1IsInstalled = newDefinePropertyMethodInfo != null;

				if (net20SP1IsInstalled)
					throw;
			}
		}

		[Test]
		public void ClassWithInheritance()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			object proxy = generator.CreateClassProxy(typeof(ExtendedServiceClass), logger);

			Assert.IsNotNull(proxy);

			ExtendedServiceClass extended = (ExtendedServiceClass)proxy;

			extended.Sum2(1, 2);
			extended.Sum(1, 2);

			Assert.AreEqual("Sum2 Sum ", logger.LogContents);
		}

		[Test]
		public void ProxyForNestedClass()
		{
			object proxy = generator.CreateClassProxy(typeof(ServiceClass.InernalClass), new Type[] { typeof(IDisposable) });
			Assert.IsNotNull(proxy);
			Assert.IsTrue(proxy is ServiceClass.InernalClass);
		}

		[Test]
		public void ProxyForClassWithInterfaces()
		{
			object proxy = generator.CreateClassProxy(typeof(ServiceClass), new Type[] { typeof(IDisposable) },
													  new ResultModifierInterceptor());

			Assert.IsNotNull(proxy);
			Assert.IsTrue(typeof(ServiceClass).IsAssignableFrom(proxy.GetType()));
			Assert.IsTrue(typeof(IDisposable).IsAssignableFrom(proxy.GetType()));

			ServiceClass inter = (ServiceClass)proxy;

			Assert.AreEqual(44, inter.Sum(20, 25));
			Assert.AreEqual(true, inter.Valid);

			try
			{
				IDisposable disp = (IDisposable)proxy;
				disp.Dispose();

				Assert.Fail("Expected exception as Dispose has no implementation");
			}
			catch (NotImplementedException ex)
			{
				Assert.AreEqual("This is a DynamicProxy2 error: the interceptor attempted " +
								"to 'Proceed' for a method without a target, for example, an interface method or an abstract method",
								ex.Message);
			}
		}

		[Test]
		public void ProxyForCharReturnType()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();
			object proxy = generator.CreateClassProxy(typeof(ClassWithCharRetType), logger);
			Assert.IsNotNull(proxy);
			ClassWithCharRetType classProxy = (ClassWithCharRetType)proxy;
			Assert.AreEqual('c', classProxy.DoSomething());
		}

		[Test]
		public void ProxyForClassWithConstructors()
		{
			object proxy = generator.CreateClassProxy(
				typeof(ClassWithConstructors), new object[] {"name"}, new StandardInterceptor());

			Assert.IsNotNull(proxy);
			ClassWithConstructors classProxy = (ClassWithConstructors)proxy;
			Assert.AreEqual("name", classProxy.Name);

			proxy = generator.CreateClassProxy(typeof(ClassWithConstructors), new object[] {"name", 10},
			                                   new StandardInterceptor());

			Assert.IsNotNull(proxy);
			classProxy = (ClassWithConstructors)proxy;
			Assert.AreEqual("name", classProxy.Name);
			Assert.AreEqual(10, classProxy.X);
		}

		/// <summary>
		/// See http://support.castleproject.org/browse/DYNPROXY-43
		/// </summary>
		[Test]
		public void MethodParamNamesAreReplicated()
		{
			MyClass mc = generator.CreateClassProxy<MyClass>(new StandardInterceptor());
			ParameterInfo[] methodParams = GetMyTestMethodParams(mc.GetType());
			Assert.AreEqual("myParam", methodParams[0].Name);
		}

		[Test]
		public void ProducesInvocationsThatCantChangeTarget()
		{
			AssertCannotChangeTargetInterceptor invocationChecker = new AssertCannotChangeTargetInterceptor();
			object proxy = generator.CreateClassProxy(typeof(ClassWithCharRetType), invocationChecker);
			Assert.IsNotNull(proxy);
			ClassWithCharRetType classProxy = (ClassWithCharRetType)proxy;
			char x = classProxy.DoSomething();
			Assert.AreEqual('c', x);
		}

		[Test]
		[Ignore("Multi dimensional arrays seems to not work at all")]
		public void ProxyTypeWithMultiDimentionalArrayAsParameters()
		{
			LogInvocationInterceptor log = new LogInvocationInterceptor();

			ClassWithMultiDimentionalArray proxy =
				generator.CreateClassProxy<ClassWithMultiDimentionalArray>(log);

			int[,] x = new int[1,2];

			proxy.Do(new int[] {1});
			proxy.Do2(x);
			proxy.Do3(new string[] {"1", "2"});

			Assert.AreEqual("Do Do2 Do3 ", log.LogContents);
		}

		private ParameterInfo[] GetMyTestMethodParams(Type type)
		{
			MethodInfo methodInfo = type.GetMethod("MyTestMethod");
			return methodInfo.GetParameters();
		}

		[Test]
		public void ProxyForBaseTypeFromSignedAssembly()
		{
#if SILVERLIGHT
#warning Silverlight does not allow us to sign generated assemblies
			
			const bool shouldBeSigned = false;
#else
			const bool shouldBeSigned = true;
#endif
			Type t = typeof(List<object>);
			Assert.IsTrue(StrongNameUtil.IsAssemblySigned(t.Assembly));
			object proxy = generator.CreateClassProxy(t, new StandardInterceptor());
			Assert.AreEqual(shouldBeSigned, StrongNameUtil.IsAssemblySigned(proxy.GetType().Assembly));
		}

		[Test]
		public void ProxyForBaseTypeAndInterfaceFromSignedAssembly()
		{
#if SILVERLIGHT
#warning Silverlight does not allow us to sign generated assemblies
			
			const bool shouldBeSigned = false;
#else
			const bool shouldBeSigned = true;
#endif
			Type t1 = typeof(List<object>);
			Type t2 = typeof(IServiceProvider);
			Assert.IsTrue(StrongNameUtil.IsAssemblySigned(t1.Assembly));
			Assert.IsTrue(StrongNameUtil.IsAssemblySigned(t2.Assembly));
			object proxy = generator.CreateClassProxy(t1, new Type[] { t2 }, new StandardInterceptor());
			Assert.AreEqual(shouldBeSigned, StrongNameUtil.IsAssemblySigned(proxy.GetType().Assembly));
		}

		[Test]
		[Ignore("To get this running, the Tests project must not be signed.")]
		public void ProxyForBaseTypeFromUnsignedAssembly()
		{
			Type t = typeof (MyClass);
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(t.Assembly));
			object proxy = generator.CreateClassProxy(t, new StandardInterceptor());
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(proxy.GetType().Assembly));
		}

		[Test]
		[Ignore("To get this running, the Tests project must not be signed.")]
		public void ProxyForBaseTypeAndInterfaceFromUnsignedAssembly()
		{
			Type t1 = typeof (MyClass);
			Type t2 = typeof (IService);
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(t1.Assembly));
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(t2.Assembly));
			object proxy = generator.CreateClassProxy(t1, new Type[] {t2}, new StandardInterceptor());
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(proxy.GetType().Assembly));
		}

		[Test]
		[Ignore("To get this running, the Tests project must not be signed.")]
		public void ProxyForBaseTypeAndInterfaceFromSignedAndUnsignedAssemblies1()
		{
			Type t1 = typeof (MyClass);
			Type t2 = typeof (IServiceProvider);
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(t1.Assembly));
			Assert.IsTrue(StrongNameUtil.IsAssemblySigned(t2.Assembly));
			object proxy = generator.CreateClassProxy(t1, new Type[] {t2}, new StandardInterceptor());
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(proxy.GetType().Assembly));
		}

		[Test]
		[Ignore("To get this running, the Tests project must not be signed.")]
		public void ProxyForBaseTypeAndInterfaceFromSignedAndUnsignedAssemblies2()
		{
			Type t1 = typeof (List<int>);
			Type t2 = typeof (IService);
			Assert.IsTrue(StrongNameUtil.IsAssemblySigned(t1.Assembly));
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(t2.Assembly));
			object proxy = generator.CreateClassProxy(t1, new Type[] {t2}, new StandardInterceptor());
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(proxy.GetType().Assembly));
		}

		[Test]
		public void VirtualCallFromCtor()
		{
			StandardInterceptor interceptor = new StandardInterceptor();
			ClassCallingVirtualMethodFromCtor proxy = generator.CreateClassProxy<ClassCallingVirtualMethodFromCtor>(interceptor);
			Assert.AreEqual(7, proxy.Result);
		}

		[Test]
		public void ClassProxyShouldHaveDefaultConstructor()
		{
			object proxy = generator.CreateClassProxy<ClassWithDefaultConstructor>();
			Assert.IsNotNull(Activator.CreateInstance(proxy.GetType()));
		}

		[Test]
		public void ClassProxyShouldCallBaseClassDefaultConstructor()
		{
			object proxy = generator.CreateClassProxy<ClassWithDefaultConstructor>();
			object proxy2 = Activator.CreateInstance(proxy.GetType());
			Assert.AreEqual("Something", ((ClassWithDefaultConstructor)proxy2).SomeString);
		}

#if SILVERLIGHT
#warning Silverlight does not allow us to access internal constructors
#else
		[Test]
		public void ClassProxyShouldHaveDefaultConstructorWhenBaseClassHasInternal()
		{
			object proxy = generator.CreateClassProxy<ClassWithInternalDefaultConstructor>();
			Assert.IsNotNull(Activator.CreateInstance(proxy.GetType()));
		}

		[Test]
		public void ClassProxyShouldCallInternalDefaultConstructor()
		{
			object proxy = generator.CreateClassProxy<ClassWithInternalDefaultConstructor>();
			object proxy2 = Activator.CreateInstance(proxy.GetType());
			Assert.AreEqual("Something", ((ClassWithInternalDefaultConstructor)proxy2).SomeString);
		}
#endif
		[Test]
		public void ClassProxyShouldHaveDefaultConstructorWhenBaseClassHasProtected()
		{
			object proxy = generator.CreateClassProxy<ClassWithProtectedDefaultConstructor>();
			Assert.IsNotNull(Activator.CreateInstance(proxy.GetType()));
		}

		[Test]
		public void ClassProxyShouldCallProtectedDefaultConstructor()
		{
			object proxy = generator.CreateClassProxy<ClassWithProtectedDefaultConstructor>();
			object proxy2 = Activator.CreateInstance(proxy.GetType());
			Assert.AreEqual("Something", ((ClassWithProtectedDefaultConstructor)proxy2).SomeString);
		}

		[Test]
		public void ClassImplementingInterfaceVitrually()
		{
			var @class = typeof (ClassWithVirtualInterface);
			var @interface = typeof(ISimpleInterface);
			var baseMethod = @class.GetMethod("Do");
			var interceptor = new SetReturnValueInterceptor(123);
			var proxy = generator.CreateClassProxy(@class, new[] {@interface}, interceptor);
			var mapping = proxy.GetType().GetInterfaceMap(@interface);

			Assert.AreEqual(mapping.TargetMethods[0].GetBaseDefinition(), baseMethod);

			Assert.AreEqual(123, (proxy as ClassWithVirtualInterface).Do());
			Assert.AreEqual(123, (proxy as ISimpleInterface).Do());
		}


		[Test]
		public void ClassImplementingInterfacePropertyVirtuallyWithInterface()
		{
			generator.CreateClassProxy(typeof (VirtuallyImplementsInterfaceWithProperty), new[] {typeof (IHasProperty)});
		}

		[Test]
		public void ClassImplementingInterfacePropertyVirtuallyWithoutInterface()
		{
			generator.CreateClassProxy(typeof(VirtuallyImplementsInterfaceWithProperty));
		}

		[Test]
		public void ClassImplementingInterfaceEventVirtuallyWithInterface()
		{
			var proxy = generator.CreateClassProxy(typeof (VirtuallyImplementsInterfaceWithEvent), new[] {typeof (IHasEvent)});
			(proxy as VirtuallyImplementsInterfaceWithEvent).MyEvent += null;
			(proxy as IHasEvent).MyEvent += null;
		}

		[Test]
		public void ClassImplementingInterfaceEventVirtuallyWithoutInterface()
		{
			var proxy = generator.CreateClassProxy(typeof(VirtuallyImplementsInterfaceWithEvent));
			(proxy as VirtuallyImplementsInterfaceWithEvent).MyEvent += null;
			(proxy as IHasEvent).MyEvent += null;
		}
		public class ResultModifierInterceptor : StandardInterceptor
		{
			protected override void PostProceed(IInvocation invocation)
			{
				object returnValue = invocation.ReturnValue;

				if (returnValue != null && returnValue.GetType() == typeof(int))
				{
					int value = (int)returnValue;

					invocation.ReturnValue = --value;
				}
				if (returnValue != null && returnValue.GetType() == typeof(bool))
				{
					invocation.ReturnValue = true;
				}
			}
		}
	}
}
