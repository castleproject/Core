// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core
{
	using System;
	using System.Collections;
    using System.Collections.Generic;

#if SILVERLIGHT
	public class GraphNode : IVertex
#else
	[Serializable]
	public class GraphNode : MarshalByRefObject, IVertex
#endif
	{
        private List<GraphNode> incoming;
        private List<GraphNode> outgoing;

		public GraphNode()
		{
		}

//		/// <summary>
//		/// Kind of copy constructor
//		/// </summary>
//		/// <param name="copy"></param>
//		public GraphNode(GraphNode copy)
//		{
//			incoming = new ArrayList(Incoming);
//			outgoing = new ArrayList(Outgoing);
//		}

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

		private List<GraphNode> Incoming
		{
			get
			{
                if (incoming == null) incoming = new List<GraphNode>();
				return incoming;
			}
		}

        private List<GraphNode> Outgoing
		{
			get
			{
                if (outgoing == null) outgoing = new List<GraphNode>();
				return outgoing;
			}
		}

		/// <summary>
		/// The nodes that dependes on this node
		/// </summary>
		public GraphNode[] Dependers
		{
			get
			{
				if (incoming == null) return new GraphNode[0];
				return incoming.ToArray();
			}
		}

		/// <summary>
		/// The nodes that this node depends
		/// </summary>
		public GraphNode[] Dependents
		{
			get
			{
				if (outgoing == null) return new GraphNode[0];
				return outgoing.ToArray();
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
