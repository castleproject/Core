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
			ProxyGenerator generator = new ProxyGenerator();

			NonPublicConstructorClass proxy =  
				generator.CreateClassProxy( 
					typeof(NonPublicConstructorClass), new StandardInterceptor() ) 
						as NonPublicConstructorClass;

			Assert.IsNotNull(proxy);

			proxy.DoSomething();
		}

		[Test]
		public void ProtectedMethods()
		{
			ProxyGenerator generator = new ProxyGenerator();

			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			NonPublicMethodsClass proxy = (NonPublicMethodsClass) 
				generator.CreateClassProxy( typeof(NonPublicMethodsClass), logger );

			proxy.DoSomething();

			Assert.AreEqual( 2, logger.Invocations.Length );
			Assert.AreEqual( "DoSomething", logger.Invocations[0] );
			Assert.AreEqual( "DoOtherThing", logger.Invocations[1] );
		}
	}

	public class LogInvocationInterceptor : StandardInterceptor
	{
		protected ArrayList invocations = new ArrayList();

		protected override void PreProceed(IInvocation invocation, params object[] arguments)
		{
			invocations.Add(invocation.Method.Name);
		}

		public String[] Invocations
		{
			get { return (String[]) invocations.ToArray( typeof(String) ); }
		}
	}
}
