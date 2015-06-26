// Copyright 2004-2013 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Tests.GenInterfaces;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Interfaces;

	using Xunit;

	public class GenericInterfaceProxyTestCase : BasePEVerifyTestCase
	{
		private LogInvocationInterceptor logger;

		public GenericInterfaceProxyTestCase()
		{
			logger = new LogInvocationInterceptor();
		}

		[Fact]
		public void Type_has_constraint_T_on_interface_method_has_constraint_T1_on_T()
		{
			generator.CreateInterfaceProxyWithoutTarget
				<IGenericInterfaceWithGenericMethodWithCascadingConstraintOnInterface<IEmpty>>();
		}

		[Fact]
		public void Type_has_constraint_T_on_class_method_has_constraint_T1_on_T()
		{
			generator.CreateInterfaceProxyWithoutTarget<IGenericInterfaceWithGenericMethodWithCascadingConstraintOnClass<Empty>>();
		}

		[Fact]
		public void Type_has_constraint_T_on_reference_type_method_has_constraint_T1_on_T()
		{
			generator.CreateInterfaceProxyWithoutTarget
				<IGenericInterfaceWithGenericMethodWithCascadingConstraintOnAnyReferenceType<Empty>>();
		}

		[Fact]
		public void Type_has_constraint_T_on_type_with_default_ctor_method_has_constraint_T1_on_T()
		{
			generator.CreateInterfaceProxyWithoutTarget
				<IGenericInterfaceWithGenericMethodWithCascadingConstraintOnAnyTypeWithDefaultConstructor<Empty>>();
		}

		[Fact]
		public void ProxyWithGenericArgument()
		{
			var proxy =
				generator.CreateInterfaceProxyWithTarget<GenInterface<int>>(
					new GenInterfaceImpl<int>(), logger);

			Assert.NotNull(proxy);

			Assert.Equal(1, proxy.DoSomething(1));

			Assert.Equal("DoSomething ", logger.LogContents);
		}

		[Fact]
		public void Proxy_with_method_with_nested_generic_parameter()
		{
			var interceptors = new SetReturnValueInterceptor(null);
			var proxy =
				generator.CreateInterfaceProxyWithoutTarget<GenInterfaceWithMethodWithNestedGenericParameter>(interceptors);
			proxy.Foo<int>(null);
		}

		[Fact]
		public void Proxy_with_method_with_nested_generic_parameter_by_ref()
		{
			var interceptors = new SetReturnValueInterceptor(null);
			var proxy =
				generator.CreateInterfaceProxyWithoutTarget<GenInterfaceWithMethodWithNestedGenericParameterByRef>(interceptors);
			IEnumerable<IComparable<int>> param = null;
			proxy.Foo(ref param);
		}

		[Fact]
		public void Proxy_with_method_with_nested_generic_return()
		{
			var interceptors = new SetReturnValueInterceptor(null);
			var proxy =
				generator.CreateInterfaceProxyWithoutTarget<GenInterfaceWithMethodWithNestedGenericReturn>(interceptors);
			proxy.Foo<int>();
		}

#if !MONO

		[Fact]
		public void ProxyWithGenericArgumentAndGenericMethod()
		{
			var proxy =
				generator.CreateInterfaceProxyWithTarget<GenInterfaceWithGenMethods<int>>(
					new GenInterfaceWithGenMethodsImpl<int>(), logger);

			Assert.NotNull(proxy);

			proxy.DoSomething(10L, 1);

			Assert.Equal("DoSomething ", logger.LogContents);
		}

		[Fact]
		public void ProxyWithGenericArgumentAndGenericMethodAndGenericReturn()
		{
			var proxy =
				generator.CreateInterfaceProxyWithTarget<GenInterfaceWithGenMethodsAndGenReturn<int>>(
					new GenInterfaceWithGenMethodsAndGenReturnImpl<int>(), logger);

			Assert.NotNull(proxy);

			Assert.Equal(10L, proxy.DoSomething(10L, 1));

			Assert.Equal("DoSomething ", logger.LogContents);
		}

		[Fact]
		public void ProxyWithGenInterfaceWithGenericArrays()
		{
			var proxy =
				generator.CreateInterfaceProxyWithTarget<IGenInterfaceWithGenArray<int>>(
					new GenInterfaceWithGenArray<int>(), logger);

			Assert.NotNull(proxy);

			var items = new[] { 1, 2, 3 };
			proxy.CopyTo(items);
			items = proxy.CreateItems();
			Assert.NotNull(items);
			Assert.Equal(3, items.Length);

			Assert.Equal("CopyTo CreateItems ", logger.LogContents);
		}

		[Fact]
		public void ProxyWithGenInterfaceWithBase()
		{
			var proxy =
				generator.CreateInterfaceProxyWithTarget<IGenInterfaceHierarchySpecialization<int>>(
					new GenInterfaceHierarchy<int>(), logger);

			Assert.NotNull(proxy);

			proxy.Add();
			proxy.Add(1);
			Assert.NotNull(proxy.FetchAll());

			Assert.Equal("Add Add FetchAll ", logger.LogContents);
		}

		[Fact]
		public void ProxyWithGenExplicitImplementation()
		{
			var target = generator.CreateInterfaceProxyWithTarget<InterfaceWithExplicitImpl<int>>(
				new GenExplicitImplementation<int>(), logger);
			var enumerator = target.GetEnum1();
			Assert.NotNull(enumerator);
		}

		[Fact]
		public void TwoGenericsInterfaceWithoutTarget()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(GenInterface<object>),
				new[] { typeof(InterfaceWithExplicitImpl<int>) },
				new LogInvocationInterceptor());
		}

		[Fact]
		public void NonGenInterfaceWithParentGenClassImplementingGenInterface()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(IUserRepository),
				new[] { typeof(InterfaceWithExplicitImpl<int>) },
				new LogInvocationInterceptor());
		}

		[Fact]
		public void WithoutTarget()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(InterfaceWithExplicitImpl<int>), new LogInvocationInterceptor());
		}

		[Fact]
		public void MethodInfoClosedInGenIfcGenMethodRefTypeNoTarget()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateInterfaceProxyWithoutTarget<GenInterfaceWithGenMethods<List<object>>>(interceptor);

			proxy.DoSomething(1, null);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void), typeof(int),
				typeof(List<object>));
			Assert.Equal(null, interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new List<object>(), new List<object>());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void),
				typeof(List<object>), typeof(List<object>));
			Assert.Equal(null, interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Fact]
		public void MethodInfoClosedInGenIfcGenMethodValueTypeNoTarget()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy =
				generator.CreateInterfaceProxyWithoutTarget<GenInterfaceWithGenMethods<int>>(interceptor);

			proxy.DoSomething(1, 1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void), typeof(int),
				typeof(int));

			proxy.DoSomething(new List<object>(), 1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void),
				typeof(List<object>), typeof(int));
		}

		[Fact]
		public void MethodInfoClosedInGenIfcNongenMethodRefTypeNoTarget()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy =
				generator.CreateInterfaceProxyWithoutTarget<IGenInterfaceHierarchyBase<List<object>>>(interceptor);

			proxy.Get();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(List<object>));

			proxy.Add(null);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void),
				typeof(List<object>));
		}

		[Fact]
		public void MethodInfoClosedInGenIfcNongenMethodValueTypeNoTarget()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy =
				generator.CreateInterfaceProxyWithoutTarget<IGenInterfaceHierarchyBase<int>>(interceptor);

			proxy.Get();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int));

			proxy.Add(0);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void), typeof(int));
		}

		[Fact]
		public void MethodInfoClosedInNongenIfcGenMethodNoTarget()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateInterfaceProxyWithoutTarget<OnlyGenMethodsInterface>(interceptor);

			proxy.DoSomething(1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int), typeof(int));

			proxy.DoSomething(new List<object>());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(List<object>),
				typeof(List<object>));
		}

		[Fact]
		public void MethodInfoClosedInGenIfcGenMethodRefTypeWithTarget()
		{
			var interceptor = new KeepDataInterceptor();
			GenInterfaceWithGenMethods<List<object>> target = new GenInterfaceWithGenMethodsImpl<List<object>>();
			var proxy =
				generator.CreateInterfaceProxyWithTarget(target, interceptor);

			proxy.DoSomething(1, null);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void), typeof(int),
				typeof(List<object>));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(void),
				typeof(int), typeof(List<object>));
			Assert.NotEqual(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new List<object>(), new List<object>());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void),
				typeof(List<object>), typeof(List<object>));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(void),
				typeof(List<object>), typeof(List<object>));
			Assert.NotEqual(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Fact]
		public void MethodInfoClosedInGenIfcGenMethodValueTypeWithTarget()
		{
			var interceptor = new KeepDataInterceptor();
			GenInterfaceWithGenMethods<int> target = new GenInterfaceWithGenMethodsImpl<int>();
			var proxy =
				generator.CreateInterfaceProxyWithTarget(target, interceptor);

			proxy.DoSomething(1, 1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void), typeof(int),
				typeof(int));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(void),
				typeof(int), typeof(int));
			Assert.NotEqual(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new List<object>(), 1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void),
				typeof(List<object>), typeof(int));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(void),
				typeof(List<object>), typeof(int));
			Assert.NotEqual(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Fact]
		public void MethodInfoClosedInGenIfcNongenMethodRefTypeWithTarget()
		{
			var interceptor = new KeepDataInterceptor();
			IGenInterfaceHierarchyBase<List<object>> target = new GenInterfaceHierarchy<List<object>>();
			var proxy =
				generator.CreateInterfaceProxyWithTarget(target, interceptor);

			proxy.Add(null);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void),
				typeof(List<object>));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(void),
				typeof(List<object>));
			Assert.NotEqual(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.Get();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(List<object>));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(),
				typeof(List<object>));
			Assert.NotEqual(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Fact]
		public void MethodInfoClosedInGenIfcNongenMethodValueTypeWithTarget()
		{
			var interceptor = new KeepDataInterceptor();
			IGenInterfaceHierarchyBase<int> target = new GenInterfaceHierarchy<int>();
			var proxy =
				generator.CreateInterfaceProxyWithTarget(target, interceptor);

			proxy.Add(0);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void), typeof(int));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(void),
				typeof(int));
			Assert.NotEqual(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.Get();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(int));
			Assert.NotEqual(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Fact]
		public void MethodInfoClosedInNongenIfcGenMethodWithTarget()
		{
			var interceptor = new KeepDataInterceptor();
			OnlyGenMethodsInterface target = new OnlyGenMethodsInterfaceImpl();
			var proxy =
				generator.CreateInterfaceProxyWithTarget(target, interceptor);

			proxy.DoSomething(1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int), typeof(int));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(int),
				typeof(int));
			Assert.NotEqual(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new List<object>());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(List<object>),
				typeof(List<object>));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(),
				typeof(List<object>), typeof(List<object>));
			Assert.NotEqual(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Fact]
		public void ThrowsWhenProxyingGenericTypeDefNoTarget()
		{
			Assert.Throws<GeneratorException>(() =>
			{
				var interceptor = new KeepDataInterceptor();
				var o = generator.CreateInterfaceProxyWithoutTarget(typeof(IGenInterfaceHierarchyBase<>), interceptor);
			});
		}

		[Fact]
		//(Description = "There is a strange CLR bug resulting from our loading the tokens of methods in generic types. " + "This test ensures we do not trigger it.")]
		//[Ignore("Currently, we trigger the bug, and work around it - see MethodFinder")]
		public void TypeGetMethodsIsStable()
		{
			ProxyWithGenInterfaceWithBase();
			Assert.Equal(4, typeof(IGenInterfaceHierarchyBase<int>).GetMethods().Length);
		}

		[Fact]
		//(Description = "There is a strange CLR bug resulting from our loading the tokens of methods in generic types. " + "This test ensures we correctly work around it.")]
		public void MethodFinderIsStable()
		{
			ProxyWithGenInterfaceWithBase();
			Assert.Equal(4,
				MethodFinder.GetAllInstanceMethods(typeof(IGenInterfaceHierarchyBase<int>),
					BindingFlags.Public | BindingFlags.Instance).Length);
		}

		//(Description = "There is a strange CLR bug resulting from our loading the tokens of methods in generic types. " + "This test ensures we do not trigger it across AppDomains. If we do, MethodFinder must provide a cross-AppDomain workaround.")]
#if SILVERLIGHT || NETCORE
		[Fact(Skip = "Cannot create separate AppDomain in Silverlight.")]
		public void TypeGetMethodsIsStableInDifferentAppDomains()
		{
		}
#else
		[Fact]
		public void TypeGetMethodsIsStableInDifferentAppDomains()
		{
			ProxyWithGenInterfaceWithBase();
			var newDomain =
				AppDomain.CreateDomain("NewDomain", AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);
			try
			{
				newDomain.DoCallBack(delegate { Assert.Equal(4, typeof(IGenInterfaceHierarchyBase<int>).GetMethods().Length); });
			}
			finally
			{
				AppDomain.Unload(newDomain);
			}
		}
#endif
#endif
	}
}