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
	/// Summary description for TopologicalSortTestCase.
	/// </summary>
	[TestFixture]
	public class TopologicalSortTestCase : Assertion
	{
		protected void AssertEquals( Vertex[] expected, Vertex[] result )
		{
			AssertEquals( expected.Length, result.Length );
			for(int i=0; i < expected.Length ; i++)
			{
				AssertEquals( expected[i], result[i] );
			}
		}

		[Test]
		public void SimpleUse()
		{
			SimpleGraph graph = new SimpleGraph();

			Vertex A = graph.CreateVertex( "A" );
			Vertex B = graph.CreateVertex( "B" );
			Vertex C = graph.CreateVertex( "C" );
			Vertex D = graph.CreateVertex( "D" );
			Vertex E = graph.CreateVertex( "E" );

			graph.CreateEdge( A, B );
			graph.CreateEdge( A, C );
			graph.CreateEdge( C, D );

			Vertex[] vertices = TopologicalSort.Perform( graph );
			Vertex[] expected = new Vertex[] { A, E, B, C, D };

			AssertNotNull( vertices );
			AssertEquals( expected, vertices );
		}

		[Test]
		public void BiggerSituation()
		{
			SimpleGraph graph = new SimpleGraph();

			Vertex A = graph.CreateVertex( "A" );
			Vertex B = graph.CreateVertex( "B" );
			Vertex C = graph.CreateVertex( "C" );
			Vertex D = graph.CreateVertex( "D" );
			Vertex E = graph.CreateVertex( "E" );
			Vertex F = graph.CreateVertex( "F" );
			Vertex G = graph.CreateVertex( "G" );

			graph.CreateEdge( B, C );
			graph.CreateEdge( A, C );
			graph.CreateEdge( C, E );
			graph.CreateEdge( B, E );
			graph.CreateEdge( A, E );
			graph.CreateEdge( E, F );
			graph.CreateEdge( F, G );

			Vertex[] vertices = TopologicalSort.Perform( graph );
			Vertex[] expected = new Vertex[] { A, B, D, C, E, F, G };

			AssertNotNull( vertices );
			AssertEquals( expected, vertices );
		}
	}
}
