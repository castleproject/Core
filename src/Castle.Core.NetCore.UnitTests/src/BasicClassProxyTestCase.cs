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

namespace CastleTests
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy;
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Tests;
	using Castle.DynamicProxy.Tests.BugsReported;
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Generators.Emitters;

	using System.Collections.Generic;

	using ClassWithIndexer = Castle.DynamicProxy.Tests.Classes.ClassWithIndexer;

	using Xunit;

	public class FactCannotBeSignedAttribute : FactAttribute
	{
		public FactCannotBeSignedAttribute()
		{
			if (TestAssemblySigned())
			{
				Skip = "To get this running, the Tests project must not be signed.";
			}
		}

		private bool TestAssemblySigned()
		{
			return StrongNameUtil.IsAssemblySigned(GetType().GetTypeInfo().Assembly);
		}
	}

	public class BasicClassProxyTestCase : BasePEVerifyTestCase
	{
		[Fact]
		public void ProxyForClass()
		{
			object proxy = generator.CreateClassProxy(typeof(ServiceClass), new ResultModifierInterceptor());

			Assert.NotNull(proxy);
			Assert.True(typeof(ServiceClass).IsAssignableFrom(proxy.GetType()));

			ServiceClass instance = (ServiceClass)proxy;

			// return value is changed by the interceptor
			Assert.Equal(44, instance.Sum(20, 25));

			// return value is changed by the interceptor
			Assert.Equal(true, instance.Valid);

			Assert.Equal(45, instance.Sum((byte)20, (byte)25)); // byte
			Assert.Equal(45, instance.Sum(20L, 25L)); // long
			Assert.Equal(45, instance.Sum((short)20, (short)25)); // short
			Assert.Equal(45, instance.Sum(20f, 25f)); // float
			Assert.Equal(45, instance.Sum(20.0, 25.0)); // double
			Assert.Equal(45, instance.Sum((ushort)20, (ushort)25)); // ushort
			Assert.Equal((uint)45, instance.Sum((uint)20, (uint)25)); // uint
			Assert.Equal((ulong)45, instance.Sum((ulong)20, (ulong)25)); // ulong
		}

		[Fact]
		public void Caching()
		{
#pragma warning disable 219
			object proxy = generator.CreateClassProxy(
				typeof(ServiceClass), new StandardInterceptor());
			proxy = generator.CreateClassProxy(
				typeof(ServiceClass), new StandardInterceptor());
			proxy = generator.CreateClassProxy(
				typeof(ServiceClass), new StandardInterceptor());
			proxy = generator.CreateClassProxy(
				typeof(ServiceClass), new StandardInterceptor());
#pragma warning restore 219
		}

#if !MONO
		[Fact]
		public void ProxyForNonPublicClass()
		{
			// have to use a type that is not from this assembly, because it is marked as internals visible to 
			// DynamicProxy2

			var type = Type.GetType("System.AppDomainInitializerInfo, mscorlib");
			var exception = Assert.Throws<GeneratorException>(() => generator.CreateClassProxy(type, new StandardInterceptor()));
			Assert.Equal(
				"Can not create proxy for type System.AppDomainInitializerInfo because it is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7\")] attribute, because assembly mscorlib is strong-named.",
				exception.Message);
		}
#endif

		[Fact]
		public void ProxyForClassWithIndexer()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			object proxy = generator.CreateClassProxy(typeof(ClassWithIndexer), logger);

			Assert.NotNull(proxy);
			Assert.IsAssignableFrom(typeof(ClassWithIndexer), proxy);

			ClassWithIndexer type = (ClassWithIndexer)proxy;

			type["name"] = 10;
			Assert.Equal(10, type["name"]);

			Assert.Equal("set_Item get_Item ", logger.LogContents);
		}

		[Fact]
		public void Can_proxy_class_with_ctor_having_params_array()
		{
			generator.CreateClassProxy(typeof(HasCtorWithParamsStrings), new object[] { new string[0] });
		}

#if !MONO && !SILVERLIGHT && !NETCORE
		[Fact]
		public void ClassWithDifferentAccessLevelOnProperties()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			object proxy = generator.CreateClassProxy(typeof(DiffAccessLevelOnProperties), logger);

			Assert.NotNull(proxy);
			Assert.IsAssignableFrom(typeof(DiffAccessLevelOnProperties), proxy);

			DiffAccessLevelOnProperties type = (DiffAccessLevelOnProperties)proxy;

			type.SetProperties();

			Assert.Equal("10 11 12 13 name", type.ToString());
		}

#endif

		[Fact]
		public void GetPropertyByReflectionTest()
		{
			object proxy = generator.CreateClassProxy(
				typeof(ServiceClass), new StandardInterceptor());

			try
			{
				Assert.False((bool)proxy.GetType().GetProperty("Valid").GetValue(proxy, null),
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
					typeof(string),
					typeof(
						PropertyAttributes),
					typeof(
						CallingConventions),
					typeof(Type),
					typeof(Type[]),
					typeof(Type[]),
					typeof(Type[]),
					typeof(Type[][]),
					typeof(Type[][])
				});

				bool net20SP1IsInstalled = newDefinePropertyMethodInfo != null;

				if (net20SP1IsInstalled)
				{
					throw;
				}
			}
		}

		[Fact]
		public void ClassWithInheritance()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			object proxy = generator.CreateClassProxy(typeof(ExtendedServiceClass), logger);

			Assert.NotNull(proxy);

			ExtendedServiceClass extended = (ExtendedServiceClass)proxy;

			extended.Sum2(1, 2);
			extended.Sum(1, 2);

			Assert.Equal("Sum2 Sum ", logger.LogContents);
		}

		[Fact]
		public void ProxyForNestedClass()
		{
			object proxy = generator.CreateClassProxy(typeof(ServiceClass.InernalClass), new Type[] { typeof(IDisposable) });
			Assert.NotNull(proxy);
			Assert.True(proxy is ServiceClass.InernalClass);
		}

		[Fact]
		public void ProxyForClassWithInterfaces()
		{
			object proxy = generator.CreateClassProxy(typeof(ServiceClass), new[] { typeof(IDisposable) },
				new ResultModifierInterceptor());

			Assert.NotNull(proxy);
			Assert.True(typeof(ServiceClass).IsAssignableFrom(proxy.GetType()));
			Assert.True(typeof(IDisposable).IsAssignableFrom(proxy.GetType()));

			ServiceClass inter = (ServiceClass)proxy;

			Assert.Equal(44, inter.Sum(20, 25));
			Assert.Equal(true, inter.Valid);

			try
			{
				IDisposable disp = (IDisposable)proxy;
				disp.Dispose();

				Assert.True(false, "Expected exception as Dispose has no implementation");
			}
			catch (NotImplementedException ex)
			{
				Assert.Equal(
					"This is a DynamicProxy2 error: The interceptor attempted to 'Proceed' for method 'Void Dispose()' which has no target. " +
					"When calling method without target there is no implementation to 'proceed' to and it is the responsibility of the interceptor " +
					"to mimic the implementation (set return value, out arguments etc)",
					ex.Message);
			}
		}

		[Fact]
		public void ProxyForCharReturnType()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();
			object proxy = generator.CreateClassProxy(typeof(ClassWithCharRetType), logger);
			Assert.NotNull(proxy);
			ClassWithCharRetType classProxy = (ClassWithCharRetType)proxy;
			Assert.Equal('c', classProxy.DoSomething());
		}

		[Fact]
		public void ProxyForClassWithConstructors()
		{
			object proxy = generator.CreateClassProxy(
				typeof(ClassWithConstructors), new object[] { "name" }, new StandardInterceptor());

			Assert.NotNull(proxy);
			ClassWithConstructors classProxy = (ClassWithConstructors)proxy;
			Assert.Equal("name", classProxy.Name);

			proxy = generator.CreateClassProxy(typeof(ClassWithConstructors), new object[] { "name", 10 },
				new StandardInterceptor());

			Assert.NotNull(proxy);
			classProxy = (ClassWithConstructors)proxy;
			Assert.Equal("name", classProxy.Name);
			Assert.Equal(10, classProxy.X);
		}

		/// <summary>
		/// See http://support.castleproject.org/browse/DYNPROXY-43
		/// </summary>
		[Fact]
		public void MethodParamNamesAreReplicated()
		{
			MyClass mc = generator.CreateClassProxy<MyClass>(new StandardInterceptor());
			ParameterInfo[] methodParams = GetMyTestMethodParams(mc.GetType());
			Assert.Equal("myParam", methodParams[0].Name);
		}

		[Fact]
		public void ProducesInvocationsThatCantChangeTarget()
		{
			AssertCannotChangeTargetInterceptor invocationChecker = new AssertCannotChangeTargetInterceptor();
			object proxy = generator.CreateClassProxy(typeof(ClassWithCharRetType), invocationChecker);
			Assert.NotNull(proxy);
			ClassWithCharRetType classProxy = (ClassWithCharRetType)proxy;
			char x = classProxy.DoSomething();
			Assert.Equal('c', x);
		}

		[Fact]
		public void ProxyTypeWithMultiDimentionalArrayAsParameters()
		{
			LogInvocationInterceptor log = new LogInvocationInterceptor();

			ClassWithMultiDimentionalArray proxy =
				generator.CreateClassProxy<ClassWithMultiDimentionalArray>(log);

			int[,] x = new int[1, 2];

			proxy.Do(new int[] { 1 });
			proxy.Do2(x);
			proxy.Do3(new string[] { "1", "2" });

			Assert.Equal("Do Do2 Do3 ", log.LogContents);
		}

		private ParameterInfo[] GetMyTestMethodParams(Type type)
		{
			MethodInfo methodInfo = type.GetMethod("MyTestMethod");
			return methodInfo.GetParameters();
		}

#if !SILVERLIGHT && !NETCORE
		[Fact]
		public void ProxyForBaseTypeFromSignedAssembly()
		{
			const bool shouldBeSigned = true;
			Type t = typeof(List<object>);
			Assert.True(StrongNameUtil.IsAssemblySigned(t.Assembly));
			object proxy = generator.CreateClassProxy(t, new StandardInterceptor());
			Assert.Equal(shouldBeSigned, StrongNameUtil.IsAssemblySigned(proxy.GetType().Assembly));
		}

		[Fact]
		public void ProxyForBaseTypeAndInterfaceFromSignedAssembly()
		{
			const bool shouldBeSigned = true;
			Type t1 = typeof(List<object>);
			Type t2 = typeof(IServiceProvider);
			Assert.True(StrongNameUtil.IsAssemblySigned(t1.Assembly));
			Assert.True(StrongNameUtil.IsAssemblySigned(t2.Assembly));
			object proxy = generator.CreateClassProxy(t1, new Type[] { t2 }, new StandardInterceptor());
			Assert.Equal(shouldBeSigned, StrongNameUtil.IsAssemblySigned(proxy.GetType().Assembly));
		}
#endif

#if SILVERLIGHT // Silverlight test runner treats Assert.Ignore as failed test :/
		[Ignore]
#endif

		[FactCannotBeSigned]
		public void ProxyForBaseTypeFromUnsignedAssembly()
		{
			Type t = typeof(MyClass);
			Assert.False(StrongNameUtil.IsAssemblySigned(t.GetTypeInfo().Assembly));
			object proxy = generator.CreateClassProxy(t, new StandardInterceptor());
			Assert.False(StrongNameUtil.IsAssemblySigned(proxy.GetType().GetTypeInfo().Assembly));
		}

#if SILVERLIGHT // Silverlight test runner treats Assert.Ignore as failed test :/
		[Ignore]
#endif

		[FactCannotBeSigned]
		public void ProxyForBaseTypeAndInterfaceFromUnsignedAssembly()
		{
			Type t1 = typeof(MyClass);
			Type t2 = typeof(IService);
			Assert.False(StrongNameUtil.IsAssemblySigned(t1.GetTypeInfo().Assembly));
			Assert.False(StrongNameUtil.IsAssemblySigned(t2.GetTypeInfo().Assembly));
			object proxy = generator.CreateClassProxy(t1, new Type[] { t2 }, new StandardInterceptor());
			Assert.False(StrongNameUtil.IsAssemblySigned(proxy.GetType().GetTypeInfo().Assembly));
		}

#if SILVERLIGHT // Silverlight test runner treats Assert.Ignore as failed test :/
		[Ignore]
#endif

		[FactCannotBeSigned]
		public void ProxyForBaseTypeAndInterfaceFromSignedAndUnsignedAssemblies1()
		{
			Type t1 = typeof(MyClass);
			Type t2 = typeof(IServiceProvider);
			Assert.False(StrongNameUtil.IsAssemblySigned(t1.GetTypeInfo().Assembly));
			Assert.True(StrongNameUtil.IsAssemblySigned(t2.GetTypeInfo().Assembly));
			object proxy = generator.CreateClassProxy(t1, new Type[] { t2 }, new StandardInterceptor());
			Assert.False(StrongNameUtil.IsAssemblySigned(proxy.GetType().GetTypeInfo().Assembly));
		}

#if SILVERLIGHT // Silverlight test runner treats Assert.Ignore as failed test :/
		[Ignore]
#endif

		[FactCannotBeSigned]
		public void ProxyForBaseTypeAndInterfaceFromSignedAndUnsignedAssemblies2()
		{
			Type t1 = typeof(List<int>);
			Type t2 = typeof(IService);
			Assert.True(StrongNameUtil.IsAssemblySigned(t1.GetTypeInfo().Assembly));
			Assert.False(StrongNameUtil.IsAssemblySigned(t2.GetTypeInfo().Assembly));
			object proxy = generator.CreateClassProxy(t1, new Type[] { t2 }, new StandardInterceptor());
			Assert.False(StrongNameUtil.IsAssemblySigned(proxy.GetType().GetTypeInfo().Assembly));
		}

		[Fact]
		public void VirtualCallFromCtor()
		{
			StandardInterceptor interceptor = new StandardInterceptor();
			ClassCallingVirtualMethodFromCtor proxy = generator.CreateClassProxy<ClassCallingVirtualMethodFromCtor>(interceptor);
			Assert.Equal(7, proxy.Result);
		}

		[Fact]
		public void ClassProxyShouldHaveDefaultConstructor()
		{
			object proxy = generator.CreateClassProxy<ClassWithDefaultConstructor>();
			Assert.NotNull(Activator.CreateInstance(proxy.GetType()));
		}

		[Fact]
		public void ClassProxyShouldCallBaseClassDefaultConstructor()
		{
			object proxy = generator.CreateClassProxy<ClassWithDefaultConstructor>();
			object proxy2 = Activator.CreateInstance(proxy.GetType());
			Assert.Equal("Something", ((ClassWithDefaultConstructor)proxy2).SomeString);
		}

#if !SILVERLIGHT && !NETCORE
		[Fact]
		public void ClassProxyShouldHaveDefaultConstructorWhenBaseClassHasInternal()
		{
			object proxy = generator.CreateClassProxy<ClassWithInternalDefaultConstructor>();
			Assert.NotNull(Activator.CreateInstance(proxy.GetType()));
		}

		[Fact]
		public void ClassProxyShouldCallInternalDefaultConstructor()
		{
			object proxy = generator.CreateClassProxy<ClassWithInternalDefaultConstructor>();
			object proxy2 = Activator.CreateInstance(proxy.GetType());
			Assert.Equal("Something", ((ClassWithInternalDefaultConstructor)proxy2).SomeString);
		}
#endif

		[Fact]
		public void ClassProxyShouldHaveDefaultConstructorWhenBaseClassHasProtected()
		{
			object proxy = generator.CreateClassProxy<ClassWithProtectedDefaultConstructor>();
			Assert.NotNull(Activator.CreateInstance(proxy.GetType()));
		}

		[Fact]
		public void ClassProxyShouldCallProtectedDefaultConstructor()
		{
			object proxy = generator.CreateClassProxy<ClassWithProtectedDefaultConstructor>();
			object proxy2 = Activator.CreateInstance(proxy.GetType());
			Assert.Equal("Something", ((ClassWithProtectedDefaultConstructor)proxy2).SomeString);
		}

		// TODO: GetInterfaceMap is not available
#if !NETCORE
		[Fact]
		public void ClassImplementingInterfaceVitrually()
		{
			var @class = typeof(ClassWithVirtualInterface);
			var @interface = typeof(ISimpleInterface);
			var baseMethod = @class.GetMethod("Do");
			var interceptor = new SetReturnValueInterceptor(123);
			var proxy = generator.CreateClassProxy(@class, new[] { @interface }, interceptor);
			var mapping = proxy.GetType().GetInterfaceMap(@interface);

			Assert.Equal(mapping.TargetMethods[0].GetBaseDefinition(), baseMethod);

			Assert.Equal(123, (proxy as ClassWithVirtualInterface).Do());
			Assert.Equal(123, (proxy as ISimpleInterface).Do());
		}
#endif

		[Fact]
		public void ClassImplementingInterfacePropertyVirtuallyWithInterface()
		{
			generator.CreateClassProxy(typeof(VirtuallyImplementsInterfaceWithProperty), new[] { typeof(IHasProperty) });
		}

		[Fact]
		public void ClassImplementingInterfacePropertyVirtuallyWithoutInterface()
		{
			generator.CreateClassProxy(typeof(VirtuallyImplementsInterfaceWithProperty));
		}

		[Fact]
		public void ClassImplementingInterfaceEventVirtuallyWithInterface()
		{
			var proxy = generator.CreateClassProxy(typeof(VirtuallyImplementsInterfaceWithEvent), new[] { typeof(IHasEvent) });
			(proxy as VirtuallyImplementsInterfaceWithEvent).MyEvent += null;
			(proxy as IHasEvent).MyEvent += null;
		}

		[Fact]
		public void ClassImplementingInterfaceEventVirtuallyWithoutInterface()
		{
			var proxy = generator.CreateClassProxy(typeof(VirtuallyImplementsInterfaceWithEvent));
			(proxy as VirtuallyImplementsInterfaceWithEvent).MyEvent += null;
			(proxy as IHasEvent).MyEvent += null;
		}

		[Fact]
		public void Finalizer_is_not_proxied()
		{
			var proxy = generator.CreateClassProxy<HasFinalizer>();

			var finalize = proxy.GetType().GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic);

			Assert.NotNull(finalize);
			Assert.Equal(typeof(HasFinalizer), finalize.DeclaringType); //, "Finalizer shouldn't have been proxied");
		}

		[Fact]
		public void Finalize_method_is_proxied_even_though_its_not_the_best_idea_ever()
		{
			var proxy = generator.CreateClassProxy<HasFinalizeMethod>();

			var finalize = proxy.GetType().GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic);

			Assert.NotNull(finalize);
			Assert.NotEqual(typeof(HasFinalizeMethod), finalize.DeclaringType);
				//, "Finalize method is not a finalizer and should have been proxied");
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