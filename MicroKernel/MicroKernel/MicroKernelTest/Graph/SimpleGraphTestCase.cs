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

namespace Castle.MicroKernel.Test.Graph
{
	using System;

	using NUnit.Framework;

	using Castle.MicroKernel.Graph;

	/// <summary>
	/// Summary description for SimpleGraphTestCase.
	/// </summary>
	[TestFixture]
	public class SimpleGraphTestCase : Assertion
	{
		[Test]
		public void AddVertex()
		{
			SimpleGraph graph = new SimpleGraph();
			
			Vertex A = graph.CreateVertex( "A" );

			AssertNotNull( A );
			AssertEquals( "A", A.Content );
			AssertEquals( 1, graph.Vertices.Length );
		}

		[Test]
		public void AddEdge()
		{
			SimpleGraph graph = new SimpleGraph();
			
			Vertex A = graph.CreateVertex( "A" );
			Vertex B = graph.CreateVertex( "B" );

			Edge edge = graph.CreateEdge( A, B );
			
			AssertNotNull( edge );
			
			AssertEquals( A, edge.Source );
			AssertEquals( B, edge.Target );
			AssertEquals( 1, A.Successors.Length );
			AssertEquals( 0, B.Successors.Length );
		}
	}
}
