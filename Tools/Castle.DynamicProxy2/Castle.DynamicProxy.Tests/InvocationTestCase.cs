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
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.InterClasses;
	using NUnit.Framework;

	[TestFixture]
	public class InvocationTestCase : BasePEVerifyTestCase
	{
		private ProxyGenerator generator;

		[SetUp]
		public void Init()
		{
			generator = new ProxyGenerator();
		}

		[Test]
		public void InvocationForConcreteClassProxy()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			
			object proxy = generator.CreateClassProxy(typeof(ServiceClass), interceptor);

			ServiceClass instance = (ServiceClass) proxy;

			instance.Sum(20, 25);
			
			Assert.IsNotNull(interceptor.Invocation);
			
			Assert.IsNotNull(interceptor.Invocation.Arguments);
			Assert.AreEqual(2, interceptor.Invocation.Arguments.Length);
			Assert.AreEqual(20, interceptor.Invocation.Arguments[0]);
			Assert.AreEqual(25, interceptor.Invocation.Arguments[1]);
			Assert.AreEqual(20, interceptor.Invocation.GetArgumentValue(0));
			Assert.AreEqual(25, interceptor.Invocation.GetArgumentValue(1));
			Assert.AreEqual(45, interceptor.Invocation.ReturnValue);
			
			Assert.IsNotNull(interceptor.Invocation.Proxy);
			Assert.IsInstanceOfType(typeof(ServiceClass), interceptor.Invocation.Proxy);
			
			Assert.IsNotNull(interceptor.Invocation.InvocationTarget);
			Assert.IsInstanceOfType(typeof(ServiceClass), interceptor.Invocation.InvocationTarget);
			Assert.IsNotNull(interceptor.Invocation.TargetType);
			Assert.AreSame(typeof(ServiceClass), interceptor.Invocation.TargetType);
			
			Assert.IsNotNull(interceptor.Invocation.Method);
			Assert.IsNotNull(interceptor.Invocation.MethodInvocationTarget);
			Assert.AreSame(interceptor.Invocation.Method, interceptor.Invocation.MethodInvocationTarget);
		}

		[Test]
		public void InvocationForInterfaceProxyWithTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();

			object proxy = generator.CreateInterfaceProxyWithTarget(
				typeof(IService), new ServiceImpl(), interceptor);

			IService instance = (IService) proxy;

			instance.Sum(20, 25);

			Assert.IsNotNull(interceptor.Invocation);

			Assert.IsNotNull(interceptor.Invocation.Arguments);
			Assert.AreEqual(2, interceptor.Invocation.Arguments.Length);
			Assert.AreEqual(20, interceptor.Invocation.Arguments[0]);
			Assert.AreEqual(25, interceptor.Invocation.Arguments[1]);
			Assert.AreEqual(20, interceptor.Invocation.GetArgumentValue(0));
			Assert.AreEqual(25, interceptor.Invocation.GetArgumentValue(1));
			Assert.AreEqual(45, interceptor.Invocation.ReturnValue);

			Assert.IsNotNull(interceptor.Invocation.Proxy);
			Assert.IsNotInstanceOfType(typeof(ServiceImpl), interceptor.Invocation.Proxy);

			Assert.IsNotNull(interceptor.Invocation.InvocationTarget);
			Assert.IsInstanceOfType(typeof(ServiceImpl), interceptor.Invocation.InvocationTarget);
			Assert.IsNotNull(interceptor.Invocation.TargetType);
			Assert.AreSame(typeof(ServiceImpl), interceptor.Invocation.TargetType);

			Assert.IsNotNull(interceptor.Invocation.Method);
			Assert.IsNotNull(interceptor.Invocation.MethodInvocationTarget);
			Assert.AreNotSame(interceptor.Invocation.Method, interceptor.Invocation.MethodInvocationTarget);
		}
	}
}