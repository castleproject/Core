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

namespace Castle.DynamicProxy.Test
{
	using System;
	using System.Collections;

	using NUnit.Framework;

	using Castle.DynamicProxy.Test.Classes;

	/// <summary>
	/// Summary description for AccessLevelTestCase.
	/// </summary>
	[TestFixture]
	public class AccessLevelTestCase
	{
		[Test]
		public void ProtectedConstructor()
		{
			NonPublicConstructorClass myClass = NonPublicConstructorClass.Create();

			ProxyGenerator generator = new ProxyGenerator();

			NonPublicConstructorClass proxy = (NonPublicConstructorClass) 
				generator.CreateClassProxy( typeof(NonPublicConstructorClass), new StandardInvocationHandler(myClass) );

			proxy.DoSomething();
		}

		[Test]
		public void ProtectedMethods()
		{
			NonPublicMethodsClass myClass = new NonPublicMethodsClass();

			ProxyGenerator generator = new ProxyGenerator();

			LogInvocationHandler logger = new LogInvocationHandler(myClass);
			NonPublicMethodsClass proxy = (NonPublicMethodsClass) 
				generator.CreateClassProxy( typeof(NonPublicMethodsClass), logger );

			proxy.DoSomething();

			Assert.AreEqual( 2, logger.Invocations.Length );
			Assert.AreEqual( "DoSomething", logger.Invocations[0] );
			Assert.AreEqual( "DoOtherThing", logger.Invocations[1] );
		}

	}

	public class LogInvocationHandler : StandardInvocationHandler
	{
		protected ArrayList invocations = new ArrayList();

		public LogInvocationHandler( object instanceDelegate ) : base(instanceDelegate)
		{
		}

		protected override void PreInvoke(object proxy, System.Reflection.MethodInfo method, params object[] arguments)
		{
			invocations.Add(method.Name);
		}

		public String[] Invocations
		{
			get { return (String[]) invocations.ToArray( typeof(String) ); }
		}
	}
}
