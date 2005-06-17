// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace AspectSharp.Tests
{
	using System;

	using NUnit.Framework;

	using AspectSharp.Tests.Classes;
	using AspectSharp.Builder;

	/// <summary>
	/// Summary description for OptimizationTestCase.
	/// </summary>
	[TestFixture]
	public class OptimizationTestCase
	{
		[Test]
		public void InterceptAll()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut method|property(*)" + 
				"     advice(AspectSharp.Tests.InterceptorTests.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			WrapAndInvokeEverything(engine);
		}

		[Test]
		public void InterceptWithComplexSignatures()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut method(* Do.*(*))" + 
				"     advice(AspectSharp.Tests.InterceptorTests.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				"   pointcut propertyread(*)" + 
				"     advice(AspectSharp.Tests.InterceptorTests.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				"   pointcut propertywrite(*)" + 
				"     advice(AspectSharp.Tests.InterceptorTests.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			WrapAndInvokeEverything(engine);
		}

		private static void WrapAndInvokeEverything(AspectEngine engine)
		{
			long begin = DateTime.Now.Ticks;

			ComplexClass instance = engine.WrapClass(typeof(ComplexClass)) as ComplexClass;

			for(int i=0; i < 10000; i++)
			{
				instance.DoNothing();
				instance.DoSomething();
				instance.DoSomething(1);
				instance.DoSomething(1, "hiya");
				instance.Name = "John Johnson";
				Assert.AreEqual( "John Johnson", instance.Name );
				instance.Started = true;
				Assert.IsTrue( instance.Started );
			}

			long end = DateTime.Now.Ticks;
			long result = (end - begin) / 1000;
			System.Console.WriteLine( "Execution took " + (result).ToString() + " ms " );
		}
	}
}
