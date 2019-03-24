// Copyright 2004-2019 Castle Project - http://www.castleproject.org/
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

	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class InvocationProceedInfoTestCase
	{
		private readonly ProxyGenerator generator = new ProxyGenerator();

		[Test]
		public void Proxy_without_target_and_last_interceptor_ProceedInfo_succeeds()
		{
			var interceptors = new IInterceptor[]
			{
				new SetReturnValueInterceptor(0),
				new WithCallbackInterceptor(invocation =>
				{
					var proceed = invocation.GetProceedInfo();
				}),
			};

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IOne>(interceptors);
			proxy.OneMethod();
		}

		[Test]
		public void Proxy_without_target_and_last_interceptor_ProceedInfo_Invoke_throws_NotImplementedException()
		{
			var interceptors = new IInterceptor[]
			{
				new SetReturnValueInterceptor(0),
				new WithCallbackInterceptor(invocation =>
				{
					var proceed = invocation.GetProceedInfo();
					Assert.Throws<NotImplementedException>(() => proceed.Invoke());
				}),
			};

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IOne>(interceptors);
			proxy.OneMethod();
		}

		[Test]
		public void Proxy_without_target_and_second_to_last_interceptor_ProceedInfo_Invoke_proceeds_to_interceptor()
		{
			var interceptors = new IInterceptor[]
			{
				new WithCallbackInterceptor(invocation =>
				{
					var proceed = invocation.GetProceedInfo();
					proceed.Invoke();
				}),
				new SetReturnValueInterceptor(1),
			};

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IOne>(interceptors);
			var returnValue = proxy.OneMethod();
			Assert.AreEqual(1, returnValue);
		}

		[Test]
		public void Proxy_with_target_and_last_interceptor_ProceedInfo_Invoke_proceeds_to_target()
		{
			var target = new One();

			var interceptor = new WithCallbackInterceptor(invocation =>
				{
					var proceed = invocation.GetProceedInfo();
					proceed.Invoke();
				});

			var proxy = generator.CreateInterfaceProxyWithTarget<IOne>(new One(), interceptor);
			var returnValue = proxy.OneMethod();
			Assert.AreEqual(1, returnValue);
		}
	}
}
