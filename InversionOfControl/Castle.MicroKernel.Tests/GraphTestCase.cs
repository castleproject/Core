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

namespace Castle.MicroKernel.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.Core;
	using Castle.Core.Internal;

	[TestFixture]
	public class GraphTestCase
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
		public void TopologicalSortOnComponents()
		{
			kernel.AddComponent( "a", typeof(A) );
			kernel.AddComponent( "b", typeof(B) );
			kernel.AddComponent( "c", typeof(C) );

			GraphNode[] nodes = kernel.GraphNodes;

			Assert.IsNotNull( nodes );
			Assert.AreEqual( 3, nodes.Length );

			IVertex[] vertices = TopologicalSortAlgo.Sort( nodes );

			Assert.AreEqual( "c", (vertices[0] as ComponentModel).Name );
			Assert.AreEqual( "b", (vertices[1] as ComponentModel).Name );
			Assert.AreEqual( "a", (vertices[2] as ComponentModel).Name );
		}

		[Test]
		public void RemoveComponent()
		{
			kernel.AddComponent( "a", typeof(A) );
			kernel.AddComponent( "b", typeof(B) );
			kernel.AddComponent( "c", typeof(C) );

			Assert.IsFalse( kernel.RemoveComponent("a") );
			Assert.IsFalse( kernel.RemoveComponent("b") );

			Assert.IsTrue( kernel.RemoveComponent("c") );
			Assert.IsTrue( kernel.RemoveComponent("b") );
			Assert.IsTrue( kernel.RemoveComponent("a") );
		}
	}
}
