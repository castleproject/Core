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

namespace AspectSharp.Tests.InterceptorTests
{
	using System;

	using NUnit.Framework;

	using AspectSharp.Builder;
	using AspectSharp.Tests.Classes;

	/// <summary>
	/// Summary description for InterceptorTestCase.
	/// </summary>
	[TestFixture]
	public class InterceptorTestCase
	{
		[SetUp]
		public void Reset()
		{
			LogInvocationInterceptor.Clear();
		}

		[Test]
		public void InterceptAll()
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

			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 8, messages.Length );
			// TODO: Assert messages in correct order
		}

		[Test]
		public void InterceptAllMethods()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut method(*)" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 4, messages.Length );
			// TODO: Assert messages in correct order
		}

		[Test]
		public void InterceptAllProperties()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut property(*)" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 4, messages.Length );
			// TODO: Assert messages in correct order
		}

		[Test]
		public void InterceptAllPropertyRead()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut propertyread(*)" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 2, messages.Length );
			// TODO: Assert messages in correct order
		}

		[Test]
		public void InterceptAllPropertyReadReturningString()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut propertyread(String)" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 1, messages.Length );
			// TODO: Assert messages in correct order
		}

		[Test]
		public void InterceptAllPropertyWrite()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut propertyread(*)" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 2, messages.Length );
			// TODO: Assert messages in correct order
		}

		[Test]
		public void InterceptAllMethodsPropertyWrite()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut method|propertyread(*)" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 6, messages.Length );
			// TODO: Assert messages in correct order
		}

		[Test]
		public void InterceptAllDoSomethingMethods()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut method(* DoSomething)" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 3, messages.Length );
		}

		[Test]
		public void InterceptAllDoSomethingMethodsWithAnIntArgument()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut method(* DoSomething(int))" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 1, messages.Length );
		}

		[Test]
		public void InterceptAllDoSomethingMethodsWithAnIntArgumentAndAnyOther()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut method(* DoSomething(int, *))" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 2, messages.Length );
		}

		[Test]
		public void InterceptAllDoSomethingMethodsWithAnIntAndAnIntArguments()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut method(* DoSomething(int, int))" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 0, messages.Length );
		}

		[Test]
		public void InterceptAllDoSomethingMethodReturningVoid()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut method(void DoSomething(*))" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 0, messages.Length );
		}

		[Test]
		public void InterceptAllDoSomethingMethodReturningInt()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut method(int DoSomething(*))" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 3, messages.Length );
		}

		[Test]
		public void InterceptAllDoSMethods()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut method(* DoS.*(*))" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 3, messages.Length );
		}

		[Test]
		public void InterceptPropertyName()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut property(* Na.*)" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 2, messages.Length );
			// TODO: Assert messages in correct order
		}

		[Test]
		public void PointcutsCombined()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect MyAspect for ComplexClass " + 
				"   " + 
				"   pointcut property(*)" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   pointcut method(*)" + 
				"     advice(AspectSharp.Tests.Classes.LogInvocationInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();
			
			WrapAndInvokeEverything(engine);

			String[] messages = LogInvocationInterceptor.Messages;
			Assert.AreEqual( 8, messages.Length );
			// TODO: Assert messages in correct order
		}

		private static void WrapAndInvokeEverything(AspectEngine engine)
		{
			ComplexClass instance = engine.WrapClass(typeof(ComplexClass)) as ComplexClass;
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
