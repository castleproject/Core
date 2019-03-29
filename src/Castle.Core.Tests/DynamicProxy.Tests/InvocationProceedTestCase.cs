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
	using System.Collections.Generic;
	using System.Linq;

	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class InvocationProceedTestCase
	{
		private readonly ProxyGenerator generator = new ProxyGenerator();

		[Test]
		public void Proceed_when_called_in_proxy_target_method_throws_InvalidOperationException()
		{
			IInvocation cachedInvocation = null;
			InvalidOperationException ex = null;

			var proxy = generator.CreateInterfaceProxyWithTarget<ISimple>(
				interceptors: new[]
				{
					new WithCallbackInterceptor(invocation =>
					{
						cachedInvocation = invocation;
						invocation.Proceed();
					})
				},
				target: new WithCallbackSimple(method: () =>
				{
					ex = Assert.Throws<InvalidOperationException>(() => cachedInvocation.Proceed());
				}));

			proxy.Method();
			Assert.NotNull(ex); // ensure that interception actually made it to the target
		}

		[Test]
		public void Proxy_without_target_and_last_interceptor_ProceedInfo_succeeds()
		{
			var interceptor = new WithCallbackInterceptor(invocation =>
				{
					invocation.ReturnValue = 0;  // not relevant to this test, but prevents DP
					                             // from complaining about missing return value.
					var proceed = invocation.CaptureProceedInfo();
				});

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IOne>(interceptor);
			proxy.OneMethod();
		}

		[Test]
		public void Proxy_without_target_and_last_interceptor_ProceedInfo_Invoke_throws_NotImplementedException()
		{
			var interceptor = new WithCallbackInterceptor(invocation =>
				{
					invocation.ReturnValue = 0;  // not relevant for this test, but prevents DP
					                             // from complaining about missing return value.
					var proceed = invocation.CaptureProceedInfo();
					Assert.Throws<NotImplementedException>(() => proceed.Invoke());
				});

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IOne>(interceptor);
			proxy.OneMethod();
		}

		[Test]
		public void Proxy_without_target_and_second_to_last_interceptor_ProceedInfo_Invoke_proceeds_to_interceptor()
		{
			var interceptors = new IInterceptor[]
			{
				new WithCallbackInterceptor(invocation =>
				{
					var proceed = invocation.CaptureProceedInfo();
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
					var proceed = invocation.CaptureProceedInfo();
					proceed.Invoke();
				});

			var proxy = generator.CreateInterfaceProxyWithTarget<IOne>(new One(), interceptor);
			var returnValue = proxy.OneMethod();
			Assert.AreEqual(1, returnValue);
		}

		[Test]
		public void CaptureProceedInfo_returns_a_new_object_every_time()
		{
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				invocation.ReturnValue = 0;  // not relevant to this test, but prevents DP
				                             // from complaining about missing return value.
				var proceed1 = invocation.CaptureProceedInfo();
				var proceed2 = invocation.CaptureProceedInfo();
				Assert.AreNotSame(proceed2, proceed1);
			});

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IOne>(interceptor);
			_ = proxy.OneMethod();
		}

		[Test]
		public void ProceedInfo_Invoke_proceeds_to_same_interceptor_on_repeated_calls()
		{
			var secondInterceptorInvokeCount = 0;
			var thirdInterceptorInvokeCount = 0;

			var interceptors = new IInterceptor[]
			{
				new WithCallbackInterceptor(invocation =>
				{
					invocation.ReturnValue = 0;  // not relevant to this test, but prevents DP
					                             // from complaining about missing return value.

					var proceed = invocation.CaptureProceedInfo();
					proceed.Invoke();
					proceed.Invoke();
				}),
				new WithCallbackInterceptor(invocation =>
				{
					secondInterceptorInvokeCount++;
				}),
				new WithCallbackInterceptor(invocation =>
				{
					thirdInterceptorInvokeCount++;
				}),
			};

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IOne>(interceptors);
			_ = proxy.OneMethod();
			Assert.AreEqual(2, secondInterceptorInvokeCount);
			Assert.AreEqual(0, thirdInterceptorInvokeCount);
		}

		[Test]
		public void ProceedInfo_Invoke_of_different_instances_captured_at_same_time_proceed_to_same_interceptor()
		{
			// The second-to-last test above established that `CaptureProceedInfo`
			// returns a different object every time. The following test established
			// that `proceed.Invoke` proceeds to the same place every time. Let's now
			// combine these and see whether two different `proceedN.Invoke` *still*
			// reach the same place. (Conceptually, this is sort of a value equality
			// test for `ProceedInfo` objects.)

			var secondInterceptorInvokeCount = 0;
			var thirdInterceptorInvokeCount = 0;

			var interceptors = new IInterceptor[]
			{
				new WithCallbackInterceptor(invocation =>
				{
					invocation.ReturnValue = 0;  // not relevant to this test, but prevents DP
					                             // from complaining about missing return value.

					var proceed1 = invocation.CaptureProceedInfo();
					var proceed2 = invocation.CaptureProceedInfo();
					Assume.That(proceed1 != proceed2);

					proceed1.Invoke();
					proceed2.Invoke();
				}),
				new WithCallbackInterceptor(invocation =>
				{
					secondInterceptorInvokeCount++;
				}),
				new WithCallbackInterceptor(invocation =>
				{
					thirdInterceptorInvokeCount++;
				}),
			};

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IOne>(interceptors);
			_ = proxy.OneMethod();
			Assert.AreEqual(2, secondInterceptorInvokeCount);
			Assert.AreEqual(0, thirdInterceptorInvokeCount);
		}

		[Test]
		public void Proxy_with_several_interceptors_ProceedInfo_Invoke_proceeds_in_correct_order()
		{
			// At various points during interception, we will record a number in this list.
			// These numbers represent the expected order in which the statements should be hit.
			// See the documented examples below.
			var breadcrumbs = new List<int>();

			var interceptors = new IInterceptor[]
			{
				new WithCallbackInterceptor(invocation =>
				{
					breadcrumbs.Add(1); // (This statement is expected to be the first one
					                    //  recorded, because it has the smallest number.)
					var proceed = invocation.CaptureProceedInfo();
					proceed.Invoke();
					breadcrumbs.Add(5); // (This statement is expected to be the last one
					                    //  recorded, because it has the largest number.)
				}),
				new WithCallbackInterceptor(invocation =>
				{
					breadcrumbs.Add(2);
					var proceed = invocation.CaptureProceedInfo();
					proceed.Invoke();
					breadcrumbs.Add(4);
				}),
				new WithCallbackInterceptor(invocation =>
				{
					breadcrumbs.Add(3);
					invocation.ReturnValue = 42;
				}),
			};

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IOne>(interceptors);
			var returnValue = proxy.OneMethod();
			Assert.AreEqual(42, returnValue);
			CollectionAssert.AreEqual(Enumerable.Range(1, 5), breadcrumbs);
		}
	}
}
