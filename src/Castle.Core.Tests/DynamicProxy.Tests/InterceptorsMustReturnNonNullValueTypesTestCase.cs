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

	using NUnit.Framework;

	// The purpose of this test fixture is to ensure that DynamicProxy throws a useful exception
	// if an interceptor fails to perform its job to ensure that a non-void method that returns
	// a value type must return either a value or throw an exception.
	[TestFixture]
	public class InterceptorsMustReturnNonNullValueTypesTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void Proxy_method_without_interceptors_returns_struct_or_throws()
		{
			var proxy = generator.CreateClassProxy<Class>();

			Assert.AreEqual(1, proxy.IntReturn());
			Exception ex = Assert.Throws<Exception>(() => proxy.IntThrow());
			Assert.AreEqual("Throw from Class", ex.Message);

			Assert.AreEqual(null, proxy.NullableIntReturn());
			ex = Assert.Throws<Exception>(() => proxy.NullableIntThrow());
			Assert.AreEqual("Throw from Class", ex.Message);
		}

		[Test]
		public void Proxy_method_that_swallows_exception_and_returns_null_struct()
		{
			var interceptor = new SwallowExceptionInterceptor();
			var proxy = generator.CreateClassProxy<Class>(interceptor);

			Assert.AreEqual(1, proxy.IntReturn());
			Exception ex = Assert.Throws<InvalidOperationException>(() => proxy.IntThrow());
			Assert.AreEqual("Interceptors failed to set a return value, or swallowed the exception thrown by the target", ex.Message);

			Assert.AreEqual(null, proxy.NullableIntReturn());
			Assert.AreEqual(null, proxy.NullableIntThrow());
		}

		[Test]
		public void Proxy_method_that_returns_clears_return_value_or_throws()
		{
			var interceptor = new ReturnNullValueInterceptor(); // Clears the ReturnValue
			var proxy = generator.CreateClassProxy<Class>(interceptor);

			Exception ex = Assert.Throws<InvalidOperationException>(() => proxy.IntReturn());
			Assert.AreEqual("Interceptors failed to set a return value, or swallowed the exception thrown by the target", ex.Message);
			Assert.Throws<Exception>(() => proxy.IntThrow(), "Throw from Class");

			Assert.AreEqual(null, proxy.NullableIntReturn());
			ex = Assert.Throws<Exception>(() => proxy.NullableIntThrow());
			Assert.AreEqual("Throw from Class", ex.Message);
		}

		public class Class
		{
			public virtual int IntReturn()
			{
				return 1;
			}

			public virtual int IntThrow()
			{
				throw new Exception("Throw from Class"); // Throw an exception without returning any result
			}

			public virtual int? NullableIntReturn()
			{
				return null;
			}

			public virtual int? NullableIntThrow()
			{
				throw new Exception("Throw from Class"); // Throw an exception without returning any result
			}
		}

		public class SwallowExceptionInterceptor : IInterceptor
		{
			public void Intercept(IInvocation invocation)
			{
				try
				{
					invocation.Proceed();
				}
				catch (Exception)
				{
					// Swallow the exception, leaving invocation.ReturnValue=null
				}
			}
		}

		public class ReturnNullValueInterceptor : IInterceptor
		{
			public void Intercept(IInvocation invocation)
			{
				invocation.Proceed(); // If this throws, ReturnValue will remain null
				invocation.ReturnValue = null;
			}
		}
	}
}