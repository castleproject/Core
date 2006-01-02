// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using AspectSharp.Builder;
	using AspectSharp.Tests.Classes;
	using NUnit.Framework;

	[TestFixture]
	public class AspectEngineTestCase
	{
		public AspectEngineTestCase()
		{
		}

		[SetUp]
		public void Init()
		{
			LogInvocationInterceptor.Clear();
		}

		[Test]
		public void ClassWithConstructorArguments()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut method|property(*)" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();

			ComplexClass instance = null;

			instance = engine.WrapClass(typeof(ComplexClass), "Eric Cartman") as ComplexClass;
			Assert.AreEqual("Eric Cartman", instance.Name);
			Assert.IsFalse(instance.Started);
			InvokeAndAssert(instance);

			instance = engine.WrapClass(typeof(ComplexClass), "Kenny McKormick", true) as ComplexClass;
			Assert.AreEqual("Kenny McKormick", instance.Name);
			Assert.IsTrue(instance.Started);
			InvokeAndAssert(instance);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 20, messages.Length );
		}

		[Test]
		public void ClassWithConstructorArgumentsAndNoAspects()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" interceptors [" + 
				" \"key\" : DummyInterceptor " + 
				" ]" + 
				" mixins [" + 
				" \"key\" : DummyMixin " + 
				" ]" + 
				" " + 
				" aspect McBrother for DummyCustomer " + 
				"   include \"key\"" + 
				"   " + 
				"   pointcut method(*)" + 
				"     advice(\"key\")" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();

			ComplexClass instance = null;

			instance = engine.WrapClass(typeof(ComplexClass), "Eric Cartman") as ComplexClass;
			Assert.AreEqual("Eric Cartman", instance.Name);
			Assert.IsFalse(instance.Started);
		}

			 
		[Test]
		public void InterfaceWrap()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for [ assignableFrom(IPartiallyComplex) ] " + 
				"   " + 
				"   pointcut method|property(*)" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();

			IPartiallyComplex instance = null;

			instance = engine.WrapInterface(typeof(IPartiallyComplex), new ComplexClass()) as IPartiallyComplex;

			instance.DoNothing();
			instance.DoSomething();
			
			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 2, messages.Length );
		}

		private static void InvokeAndAssert(ComplexClass instance)
		{
			instance.DoNothing();
			instance.DoSomething();
			int arg = 1;
	
			instance.DoSomething(arg);
			instance.DoSomething(arg, "hiya");
	
			//TODO: Intercept by ref calls.
			//Assert.AreEqual(arg, instance.Pong(ref arg));
	
			instance.Name = "John Johnson";
			Assert.AreEqual( "John Johnson", instance.Name );
			instance.Started = true;
			Assert.IsTrue( instance.Started );
		}
	}
}
