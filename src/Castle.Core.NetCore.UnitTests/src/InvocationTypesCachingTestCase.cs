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
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Interfaces;
	using Castle.InterClasses;

	using Xunit;

	public class InvocationTypesCachingTestCase : BasePEVerifyTestCase
	{
		[Fact]
		public void Should_share_invocations_for_interface_methods()
		{
			var interceptor1 = new KeepDataInterceptor();
			var interceptor2 = new KeepDataInterceptor();
			var first = generator.CreateInterfaceProxyWithTarget<IOne>(new One(), interceptor1);
			var second = generator.CreateInterfaceProxyWithTarget<IOne>(new OneTwo(), interceptor2);

			Assert.NotEqual(first.GetType(), second.GetType()); //, "proxy types are different");

			first.OneMethod();
			second.OneMethod();

			Assert.Equal(interceptor1.Invocation.GetType(), interceptor2.Invocation.GetType());
		}

		[Fact]
		public void Should_not_share_invocations_for_interface_methods_when_one_is_IChangeProxyTarget()
		{
			var interceptor1 = new KeepDataInterceptor();
			var interceptor2 = new KeepDataInterceptor();
			var first = generator.CreateInterfaceProxyWithTarget<IOne>(new One(), interceptor1);
			var second = generator.CreateInterfaceProxyWithTargetInterface<IOne>(new OneTwo(), interceptor2);

			Assert.NotEqual(first.GetType(), second.GetType()); //, "proxy types are different");

			first.OneMethod();
			second.OneMethod();

			Assert.IsNotType<IChangeProxyTarget>(interceptor1.Invocation);
			Assert.NotNull(interceptor2.Invocation as IChangeProxyTarget);
			Assert.NotEqual(interceptor1.Invocation.GetType(), interceptor2.Invocation.GetType());
		}
	}
}