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

namespace Castle.Model
{
	using System;
	using System.Collections;

	public class GraphNode : IVertex
	{
		private ArrayList _incoming;
		private ArrayList _outgoing;

		public GraphNode()
		{
		}
		
		#region IVertex Members

		public IVertex[] Adjacencies
		{
			get { return Dependents; }
		}

		#endregion

		public void AddDependent(GraphNode node)
		{
			Outgoing.Add(node);
			node.Incoming.Add(this);
		}

		private ArrayList Incoming
		{
			get
			{
				if (_incoming == null) _incoming = new ArrayList();
				return _incoming;
			}
		}

		private ArrayList Outgoing
		{
			get
			{
				if (_outgoing == null) _outgoing = new ArrayList();
				return _outgoing;
			}
		}

		/// <summary>
		/// The nodes that dependes on this node
		/// </summary>
		public GraphNode[] Dependers
		{
			get
			{
				if (_incoming == null) return new GraphNode[0];
				return (GraphNode[]) _incoming.ToArray( typeof(GraphNode) );
			}
		}

		/// <summary>
		/// The nodes that this node depends
		/// </summary>
		public GraphNode[] Dependents
		{
			get
			{
				if (_outgoing == null) return new GraphNode[0];
				return (GraphNode[]) _outgoing.ToArray( typeof(GraphNode) );
			}
		}

		public void RemoveDepender(GraphNode depender)
		{
			Incoming.Remove(depender);
			depender.RemoveDependent(this);
		}

		private void RemoveDependent(GraphNode graphNode)
		{
			Outgoing.Remove(graphNode);
		}

		
	}	
}
