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
	using System.Collections.Generic;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.InterClasses;
	using NUnit.Framework;

	[TestFixture]
	public class BasicInterfaceProxyWithoutTargetTestCase : BasePEVerifyTestCase
	{

	    [Test]
		public void BasicInterfaceProxyWithValidTarget_ThrowsIfInterceptorCallsProceed()
	    {
	    	IService service = (IService)
	    	                   generator.CreateInterfaceProxyWithoutTarget(
	    	                   	typeof (IService), new StandardInterceptor());
	    	var exception = (NotImplementedException) Assert.Throws(typeof (NotImplementedException), () =>

                service.Sum(1, 2));

	    	Assert.AreEqual(
				"This is a DynamicProxy2 error: The interceptor attempted to 'Proceed' for method 'Int32 Sum(Int32, Int32)' which has no target. " +
	    		"When calling method without target there is no implementation to 'proceed' to and it is the responsibility of the interceptor " +
	    		"to mimic the implementation (set return value, out arguments etc)",
	    		exception.Message);
	    }

		[Test]
		public void CanReplaceReturnValueOfInterfaceMethod()
		{
			IService service = (IService)
							   generator.CreateInterfaceProxyWithoutTarget(
                                typeof(IService), new SetReturnValueInterceptor(3));

			int result = service.Sum(2, 2);
			Assert.AreEqual(3, result);
		}

		[Test]
		[ExpectedException(typeof (ThrowingInterceptorException), ExpectedMessage = "Because I feel like it")]
		public void CanThrowExceptionFromInterceptorOfInterfaceMethod()
		{
			IService service = (IService)
							   generator.CreateInterfaceProxyWithoutTarget(
								typeof(IService), new ThrowingInterceptor());

			service.Sum(2, 2);
		}

		[Test]
		public void ProxyWithGenericTypeThatInheritFromGenericType()
		{
			// Only PEVerify is enough
			generator.CreateInterfaceProxyWithoutTarget<IList<int>>(new ThrowingInterceptor());
		}

		[Test]
		public void ProducesInvocationsThatCantChangeTarget()
		{
			IService service = (IService)
							   generator.CreateInterfaceProxyWithoutTarget(
								typeof(IService), new AssertCannotChangeTargetInterceptor(), new SetReturnValueInterceptor(3));

			int result = service.Sum(2, 2);
			Assert.AreEqual(3, result);
		}
        
		public class ThrowingInterceptorException : Exception
		{
			public ThrowingInterceptorException(string message)
				: base(message)
			{}
		}

		public class ThrowingInterceptor : IInterceptor
		{
			public void Intercept(IInvocation invocation)
			{
				throw new ThrowingInterceptorException("Because I feel like it");
			}
		}
	}
}
