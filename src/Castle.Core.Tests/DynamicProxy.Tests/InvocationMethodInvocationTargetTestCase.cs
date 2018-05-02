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
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.GenClasses;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class InvocationMethodInvocationTargetTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void ClassProxy_MethodInvocationTarget_should_be_base_Method()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy<ServiceClass>(interceptor);
			proxy.Sum(2, 2);
			var methodOnClass = typeof(ServiceClass).GetMethod("Sum", new[] { typeof(int), typeof(int) });
			Assert.AreSame(methodOnClass, interceptor.Invocation.MethodInvocationTarget);
		}

		[Test]
		public void ClassProxy_MethodInvocationTarget_should_be_base_Method_for_interface_methods_implemented_non_virtually()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy(typeof(One), new[] { typeof(IOne) }, interceptor) as IOne;
			proxy.OneMethod();
			var methodOnInterface = typeof(One).GetMethod("OneMethod", Type.EmptyTypes);
			Assert.AreSame(methodOnInterface, interceptor.Invocation.MethodInvocationTarget);
		}
		
		[Test]
		public void ClassProxy_MethodInvocationTarget_should_be_base_Method_for_interface_methods_implemented_virtually()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy(typeof(ClassWithVirtualInterface), new[] { typeof(ISimpleInterface) }, interceptor) as ISimpleInterface;
			proxy.Do();
			var methodOnClass = typeof(ClassWithVirtualInterface).GetMethod("Do", Type.EmptyTypes);
			Assert.AreSame(methodOnClass, interceptor.Invocation.MethodInvocationTarget);
		}

		[Test]
		public void InterfaceProxyWithTarget_MethodInvocationTarget_should_be_methodOnTargetType()
		{
			var interceptor = new KeepDataInterceptor();
			var target = new ServiceImpl();
			var proxy = generator.CreateInterfaceProxyWithTarget<IService>(target, interceptor);
			proxy.Sum(2, 2);
			MethodInfo methodOnTarget = target.GetType().GetMethod("Sum", new[] { typeof(int), typeof(int) });
			Assert.AreSame(methodOnTarget, interceptor.Invocation.MethodInvocationTarget);
		}

		[Test]
		public void InterfaceProxyWithTarget_MethodInvocationTarget_should_be_null()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IService>(interceptor);
			proxy.Sum(2, 2);
			Assert.IsNull(interceptor.Invocation.MethodInvocationTarget);
		}

		[Test]
		public void InterfaceProxyWithTargetInterface_MethodInvocationTarget_should_be_methodOnTargetType()
		{
			var interceptor = new KeepDataInterceptor();
			var target = new ServiceImpl();
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IService), target, interceptor) as IService;
			proxy.Sum(2, 2);
			MethodInfo methodOnTarget = target.GetType().GetMethod("Sum", new[] { typeof(int), typeof(int) });
			Assert.AreSame(methodOnTarget, interceptor.Invocation.MethodInvocationTarget);
		}

		[Test]
		public void InterfaceProxyWithTargetInterface_MethodInvocationTarget_should_be_updated_when_target_changes()
		{
			MethodInfo invocationTarget1 = null;
			MethodInfo invocationTarget2 = null;
			var target1 = new AlwaysThrowsServiceImpl();
			var target2 = new ServiceImpl();
			MethodInfo methodOnTarget1 = target1.GetType().GetMethod("Sum", new[] { typeof(int), typeof(int) });
			MethodInfo methodOnTarget2 = target2.GetType().GetMethod("Sum", new[] { typeof(int), typeof(int) });
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(
			            	typeof(IService),
			            	target1,
			            	new WithCallbackInterceptor(i =>
			            	{
			            		invocationTarget1 = i.MethodInvocationTarget;
			            		i.Proceed();
			            	}),
			            	new ChangeTargetInterceptor(target2),
			            	new WithCallbackInterceptor(i =>
			            	{
			            		invocationTarget2 = i.MethodInvocationTarget;
			            		i.Proceed();
			            	})) as IService;

			proxy.Sum(2, 2);

			Assert.AreNotEqual(invocationTarget1, invocationTarget2);
			Assert.AreSame(methodOnTarget1, invocationTarget1);
			Assert.AreSame(methodOnTarget2, invocationTarget2);
		}

		[Test]
		public void ClassProxyForGeneric_MethodInvocationTarget_should_be_proxyMethod()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = (IChangeTracking)generator.CreateClassProxy<GenClassWithExplicitImpl<int>>(interceptor);
			Assert.IsTrue(proxy.IsChanged);
			Assert.IsNotNull(interceptor.Invocation.MethodInvocationTarget);
		}
	}
}