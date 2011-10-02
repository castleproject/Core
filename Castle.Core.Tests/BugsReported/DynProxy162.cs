// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests.BugsReported
{
	using System;

	using NUnit.Framework;

	[TestFixture]
	public class DynProxy163 : BasePEVerifyTestCase
	{
		[Test]
		public void ProxyMothedThrowExceptionWithExceptionCatchInterceptor()
		{
			var proxy = generator.CreateClassProxy<ClassHasMethodThrowException>(new ExceptionCatchInterceptor());

			var param1 = 1;
			var param2 = "1";

			var retVal = proxy.MethodWithRefParam(ref param1, out param2);

			Assert.AreEqual(42, retVal);
			Assert.AreEqual(23, param1);
			Assert.AreEqual("23", param2);
		}

		[Test]
		public void ProxyMothedThrowExceptionWithStandardInterceptor()
		{
			var proxy = generator.CreateClassProxy<ClassHasMethodThrowException>(new StandardInterceptor());

			var param1 = 1;
			var param2 = "1";
			var exMsg = "";
			var retVal = 1;

			try
			{
				retVal = proxy.MethodWithRefParam(ref param1, out param2);
			}
			catch (Exception ex)
			{
				exMsg = ex.Message;
			}

			Assert.AreEqual("intentional exception", exMsg);
			Assert.AreEqual(1, retVal);
			Assert.AreEqual(23, param1);
			Assert.AreEqual("23", param2);
		}
	}

	public class ClassHasMethodThrowException
	{
		public virtual int MethodWithRefParam(ref int refParam, out string outParam)
		{
			refParam = 23;
			outParam = "23";

			if (refParam == 23)
			{
				throw new Exception("intentional exception");
			}

			return 42;
		}
	}

	public class ExceptionCatchInterceptor : StandardInterceptor
	{
		protected override void PerformProceed(IInvocation invocation)
		{
			try
			{
				base.PerformProceed(invocation);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				invocation.ReturnValue = 42;
			}
		}
	}
}