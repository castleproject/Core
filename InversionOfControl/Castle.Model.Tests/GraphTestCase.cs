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

namespace Castle.Model.Tests
{
	using System;

	using NUnit.Framework;

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
		public void TopologicalSort()
		{
			GraphNode alone = new GraphNode();
			GraphNode first = new GraphNode();
			GraphNode second = new GraphNode();
			GraphNode third = new GraphNode();
			
			first.AddDependent(second);
			second.AddDependent(third);

			GraphNode[] nodes = 
				GraphNode.TopologicalSort( new GraphNode[] { alone, first, second, third } );

			Assert.AreSame( alone, nodes[0] );
			Assert.AreSame( first, nodes[1] );
			Assert.AreSame( second, nodes[2] );
			Assert.AreSame( third, nodes[3] );
		}
	}
}
