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
	using System.Threading.Tasks;

	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class AsyncInterceptorTestCase : BasePEVerifyTestCase
	{
		[Test]
		public async Task Should_Intercept_Asynchronous_Methods_With_An_Async_Operations_Prior_To_Calling_Proceed()
		{
			// Arrange
			IInterfaceWithAsynchronousMethod target = new ClassWithAsynchronousMethod();
			IInterceptor interceptor = new AsyncInterceptor();

			IInterfaceWithAsynchronousMethod proxy =
				generator.CreateInterfaceProxyWithTargetInterface(target, interceptor);

			// Act
			await proxy.Method().ConfigureAwait(false);
		}

		private class AsyncInterceptor : IInterceptor
		{
			public void Intercept(IInvocation invocation)
			{
				invocation.ReturnValue = InterceptAsyncMethod(invocation);
			}

			private static async Task InterceptAsyncMethod(IInvocation invocation)
			{
				var proceed = invocation.CaptureProceedInfo();

				await Task.Delay(10).ConfigureAwait(false);

				proceed.Invoke();

				// Return value is being set in two situations, but this doesn't matter
				// for the above test.
				Task returnValue = (Task)invocation.ReturnValue;

				await returnValue.ConfigureAwait(false);
			}
		}
	}
}
