// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Test.Interceptor
{
	using NUnit.Framework;

	using Castle.MicroKernel.Interceptor;

	/// <summary>
	/// Summary description for AbstractInterceptorTestCase.
	/// </summary>
	[TestFixture]
	public class AbstractInterceptorTestCase : Assertion
	{
		[Test]
		public void CheckAbstractImplementation()
		{
			TestInterceptor testInterceptor = new TestInterceptor();
			EndInterceptor endInterceptor = new EndInterceptor();

			testInterceptor.Next = endInterceptor;

			AssertEquals( 1, testInterceptor.Process( this, null ) );
		}

		public class TestInterceptor : AbstractInterceptor
		{
			public override object Process(object instance, System.Reflection.MethodInfo method, params object[] arguments)
			{
				return base.Process (instance, method, arguments);
			}
		}

		public class EndInterceptor : AbstractInterceptor
		{
			public override object Process(object instance, System.Reflection.MethodInfo method, params object[] arguments)
			{
				return 1;
			}
		}
	}
}
