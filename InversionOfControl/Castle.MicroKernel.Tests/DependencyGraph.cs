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

namespace Castle.MicroKernel.Tests
{
	using System;

	using NUnit.Framework;

	public class A
	{		
	}

	public class B
	{
		public B(A a) {}
	}

	public class C
	{
		public C(B b) {}
	}

	public class CycleA
	{
		public CycleA(CycleB b) {}
	}

	public class CycleB
	{
		public CycleB(CycleA a) {}
	}

	[TestFixture]
	public class DependencyGraph
	{
		private IKernel kernel;

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();
		}

		[TearDown]
		public void Dispose()
		{
			kernel.Dispose();
		}

		[Test]
		public void ValidSituation()
		{
			kernel.AddComponent( "a", typeof(A) );
			kernel.AddComponent( "b", typeof(B) );
			kernel.AddComponent( "c", typeof(C) );

			Assert.IsNotNull( kernel["a"] );
			Assert.IsNotNull( kernel["b"] );
			Assert.IsNotNull( kernel["c"] );
		}

		[Test]
		public void GraphInvalid()
		{
			kernel.AddComponent( "b", typeof(B) );
			kernel.AddComponent( "c", typeof(C) );

			IHandler handlerB = kernel.GetHandler( typeof(B) );
			IHandler handlerC = kernel.GetHandler( typeof(C) );

			Assert.AreEqual( HandlerState.WaitingDependency, handlerB.CurrentState );
			Assert.AreEqual( HandlerState.WaitingDependency, handlerC.CurrentState );
		}

		[Test]
		public void GraphInvalidAndLateValidation()
		{
			kernel.AddComponent( "b", typeof(B) );
			kernel.AddComponent( "c", typeof(C) );

			IHandler handlerB = kernel.GetHandler( typeof(B) );
			IHandler handlerC = kernel.GetHandler( typeof(C) );

			Assert.AreEqual( HandlerState.WaitingDependency, handlerB.CurrentState );
			Assert.AreEqual( HandlerState.WaitingDependency, handlerC.CurrentState );

			kernel.AddComponent( "a", typeof(A) );

			Assert.AreEqual( HandlerState.Valid, handlerB.CurrentState );
			Assert.AreEqual( HandlerState.Valid, handlerC.CurrentState );
		}

		/// This test case must pass !
//		[Test]
//		public void CycleComponents()
//		{
//			kernel.AddComponent( "a", typeof(CycleA) );
//			kernel.AddComponent( "b", typeof(CycleB) );
//
//			Assert.IsNotNull( kernel["a"] );
//			Assert.IsNotNull( kernel["b"] );
//		}
	}
}
