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

namespace Castle.Model.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.Model.Internal;

	[TestFixture]
	public class GraphTestCase
	{
		[Test]
		public void SimpleUsage()
		{
			GraphNode parent = new GraphNode();
			GraphNode child = new GraphNode();
			
			parent.AddDependent(child);

			Assert.AreSame( parent, child.Dependers[0] );
			Assert.AreSame( child, parent.Dependents[0] );
		}

		[Test]
		public void Removal()
		{
			GraphNode parent = new GraphNode();
			GraphNode child = new GraphNode();
			
			parent.AddDependent(child);
			
			child.RemoveDepender(parent);

			Assert.IsTrue( parent.Dependents.Length == 0 );
			Assert.IsTrue( parent.Dependers.Length == 0 );
			Assert.IsTrue( child.Dependers.Length == 0 );
			Assert.IsTrue( child.Dependents.Length == 0 );
		}

		[Test]
		public void TopologicalSortOneElement()
		{
			GraphNode alone = new TestGraphNode("alone");

			IVertex[] nodes = TopologicalSortAlgo.Sort( new GraphNode[] { alone } );

			Assert.AreSame( alone, nodes[0] );
		}

		[Test]
		public void TopologicalSortSimple()
		{
			GraphNode alone = new TestGraphNode("alone");
			GraphNode first = new TestGraphNode("first");
			GraphNode second = new TestGraphNode("second");
			GraphNode third = new TestGraphNode("third");
			
			first.AddDependent(second);
			second.AddDependent(third);

			IVertex[] nodes = 
				TopologicalSortAlgo.Sort( new GraphNode[] { alone, second, first, third } );

			Assert.AreSame( first, nodes[0] );
			Assert.AreSame( second, nodes[1] );
			Assert.AreSame( third, nodes[2] );
			Assert.AreSame( alone, nodes[3] );
		}

		[Test]
		public void ComplexDag()
		{
			GraphNode shirt = new TestGraphNode("shirt");
			GraphNode tie = new TestGraphNode("tie");
			GraphNode jacket = new TestGraphNode("jacket");
			GraphNode belt = new TestGraphNode("belt");
			GraphNode watch = new TestGraphNode("watch");
			GraphNode undershorts = new TestGraphNode("undershorts");
			GraphNode pants = new TestGraphNode("pants");
			GraphNode shoes = new TestGraphNode("shoes");
			GraphNode socks = new TestGraphNode("socks");
			
			shirt.AddDependent(belt);
			shirt.AddDependent(tie);

			tie.AddDependent(jacket);

			pants.AddDependent(belt);
			pants.AddDependent(shoes);

			undershorts.AddDependent(pants);
			undershorts.AddDependent(shoes);

			socks.AddDependent(shoes);
			belt.AddDependent(jacket);

			IVertex[] nodes = 
				TopologicalSortAlgo.Sort( 
					new GraphNode[] 
					{ shirt, tie, jacket, belt, watch, undershorts, pants, shoes, socks} );

			Assert.AreSame( socks, nodes[0] );
			Assert.AreSame( undershorts, nodes[1] );
			Assert.AreSame( pants, nodes[2] );
			Assert.AreSame( shoes, nodes[3] );
			Assert.AreSame( watch, nodes[4] );
			Assert.AreSame( shirt, nodes[5] );
			Assert.AreSame( tie, nodes[6] );
			Assert.AreSame( belt, nodes[7] );
			Assert.AreSame( jacket, nodes[8] );
		}
	}

	public class TestGraphNode : GraphNode
	{
		private String _name;

		public TestGraphNode(String name)
		{
			_name = name;
		}

		public String Name
		{
			get { return _name; }
		}
	}
}
