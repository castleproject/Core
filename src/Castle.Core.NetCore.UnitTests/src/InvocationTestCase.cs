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
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.InterClasses;

	using System.Reflection;

	using Xunit;

	public class InvocationTestCase : BasePEVerifyTestCase
	{
		[Fact]
		public void InvocationForConcreteClassProxy()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();

			object proxy = generator.CreateClassProxy(typeof(ServiceClass), interceptor);

			ServiceClass instance = (ServiceClass)proxy;

			instance.Sum(20, 25);

			Assert.NotNull(interceptor.Invocation);

			Assert.NotNull(interceptor.Invocation.Arguments);
			Assert.Equal(2, interceptor.Invocation.Arguments.Length);
			Assert.Equal(20, interceptor.Invocation.Arguments[0]);
			Assert.Equal(25, interceptor.Invocation.Arguments[1]);
			Assert.Equal(20, interceptor.Invocation.GetArgumentValue(0));
			Assert.Equal(25, interceptor.Invocation.GetArgumentValue(1));
			Assert.Equal(45, interceptor.Invocation.ReturnValue);

			Assert.NotNull(interceptor.Invocation.Proxy);
			Assert.IsAssignableFrom(typeof(ServiceClass), interceptor.Invocation.Proxy);

			Assert.NotNull(interceptor.Invocation.InvocationTarget);
			Assert.IsAssignableFrom(typeof(ServiceClass), interceptor.Invocation.InvocationTarget);
			Assert.NotNull(interceptor.Invocation.TargetType);
			Assert.Same(typeof(ServiceClass), interceptor.Invocation.TargetType);

			Assert.NotNull(interceptor.Invocation.Method);
			Assert.NotNull(interceptor.Invocation.MethodInvocationTarget);
			Assert.Same(interceptor.Invocation.Method, interceptor.Invocation.MethodInvocationTarget.GetBaseDefinition());
		}

		[Fact]
		public void InvocationForInterfaceProxyWithTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();

			object proxy = generator.CreateInterfaceProxyWithTarget(
				typeof(IService), new ServiceImpl(), interceptor);

			IService instance = (IService)proxy;

			instance.Sum(20, 25);

			Assert.NotNull(interceptor.Invocation);

			Assert.NotNull(interceptor.Invocation.Arguments);
			Assert.Equal(2, interceptor.Invocation.Arguments.Length);
			Assert.Equal(20, interceptor.Invocation.Arguments[0]);
			Assert.Equal(25, interceptor.Invocation.Arguments[1]);
			Assert.Equal(20, interceptor.Invocation.GetArgumentValue(0));
			Assert.Equal(25, interceptor.Invocation.GetArgumentValue(1));
			Assert.Equal(45, interceptor.Invocation.ReturnValue);

			Assert.NotNull(interceptor.Invocation.Proxy);
			Assert.IsNotType(typeof(ServiceImpl), interceptor.Invocation.Proxy);

			Assert.NotNull(interceptor.Invocation.InvocationTarget);
			Assert.IsType(typeof(ServiceImpl), interceptor.Invocation.InvocationTarget);
			Assert.NotNull(interceptor.Invocation.TargetType);
			Assert.Same(typeof(ServiceImpl), interceptor.Invocation.TargetType);

			Assert.NotNull(interceptor.Invocation.Method);
			Assert.NotNull(interceptor.Invocation.MethodInvocationTarget);
			Assert.NotSame(interceptor.Invocation.Method, interceptor.Invocation.MethodInvocationTarget);
		}
	}
}