// Copyright 2004-2018 Castle Project - http://www.castleproject.org/
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

	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.GenClasses;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class InvocationMethodTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void ClassProxy_Method_should_be_base_Method()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy<ServiceClass>(interceptor);
			proxy.Sum(2, 2);
			var methodOnClass = typeof(ServiceClass).GetMethod("Sum", new[] { typeof(int), typeof(int) });
			Assert.AreSame(methodOnClass, interceptor.Invocation.Method);
		}

		[Test]
		public void ClassProxy_Method_should_be_interface_method_for_interface_methods_implemented_non_virtually()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy(typeof(One), new[] { typeof(IOne) }, interceptor) as IOne;
			proxy.OneMethod();
			var methodOnInterface = typeof(IOne).GetMethod("OneMethod", Type.EmptyTypes);
			Assert.AreSame(methodOnInterface, interceptor.Invocation.Method);
		}

		[Test]
		public void ClassProxy_Method_should_be_base_Method_for_interface_methods_implemented_virtually()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy(typeof(ClassWithVirtualInterface), new[] { typeof(ISimpleInterface) }, interceptor) as ISimpleInterface;
			proxy.Do();
			var methodOnClass = typeof(ClassWithVirtualInterface).GetMethod("Do", Type.EmptyTypes);
			Assert.AreSame(methodOnClass, interceptor.Invocation.Method);
		}

		[Test]
		public void InterfaceProxyWithTarget_Method_should_be_interface_method()
		{
			var interceptor = new KeepDataInterceptor();
			var target = new ServiceImpl();
			var proxy = generator.CreateInterfaceProxyWithTarget<IService>(target, interceptor);
			proxy.Sum(2, 2);
			MethodInfo methodOnInterface = typeof(IService).GetMethod("Sum", new[] { typeof(int), typeof(int) });
			Assert.AreSame(methodOnInterface, interceptor.Invocation.Method);
		}

		[Test]
		public void InterfaceProxyWithoutTarget_Method_should_be_interface_method()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IService>(interceptor);
			proxy.Sum(2, 2);
			MethodInfo methodOnInterface = typeof(IService).GetMethod("Sum", new[] { typeof(int), typeof(int) });
			Assert.AreSame(methodOnInterface, interceptor.Invocation.Method);
		}

		[Test]
		public void InterfaceProxyWithTargetInterface_Method_should_be_interface_method()
		{
			var interceptor = new KeepDataInterceptor();
			var target = new ServiceImpl();
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IService), target, interceptor) as IService;
			proxy.Sum(2, 2);
			MethodInfo methodOnInterface = typeof(IService).GetMethod("Sum", new[] { typeof(int), typeof(int) });
			Assert.AreSame(methodOnInterface, interceptor.Invocation.Method);
		}

		[Test]
		public void DelegateProxy_Method_should_be_Invoke_method_on_delegate_type()
		{
			var interceptor = new KeepDataInterceptor();

			var proxy = generator.CreateDelegateProxy<Action>(interceptor);
			proxy();

			var methodOnDelegateType = typeof(Action).GetMethod("Invoke");
			Assert.AreSame(methodOnDelegateType, interceptor.Invocation.Method);
		}

		[Test]
		public void DelegateProxyWithTarget_Method_should_be_Invoke_method_on_delegate_type()
		{
			var interceptor = new KeepDataInterceptor();
			Action target = delegate { };

			var proxy = generator.CreateDelegateProxyWithTarget<Action>(target, interceptor);
			proxy();

			var methodOnDelegateType = typeof(Action).GetMethod("Invoke");
			Assert.AreSame(methodOnDelegateType, interceptor.Invocation.Method);
		}
	}
}