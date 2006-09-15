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

namespace Castle.Components.Binder
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	public enum NodeType
	{
		Unspecified,
		Composite,
		Indexed,
		Leaf
	}

	public abstract class Node
	{
		private readonly String name;
		private readonly NodeType nodeType;

		protected Node(String name, NodeType nodeType)
		{
			this.name = name;
			this.nodeType = nodeType;
		}

		public String Name 
		{ 
			get { return name;}
		}
		
		public NodeType NodeType
		{
			get { return nodeType; }
		}
	}
	
	public class CompositeNode : Node
	{
		private readonly HybridDictionary name2Node = new HybridDictionary(true);
		private readonly ArrayList nodeList = new ArrayList();

		public CompositeNode(String name) : base(name, NodeType.Composite)
		{
		}

		protected CompositeNode(string name, NodeType nodeType) : base(name, nodeType)
		{
		}

		public void AddChildNode(Node node)
		{
			if (node == null) throw new ArgumentNullException("node");
			
			name2Node[node.Name] = node;
			nodeList.Add(node);
		}
		
		public Node GetChildNode(String name)
		{
			int index = name.IndexOf(".");
			
			if (index != -1)
			{
				string firstNodeName = name.Substring(0, index);
				string remainingName = name.Substring(index + 1);

				Node innerNode = (Node)name2Node[firstNodeName];

				if (innerNode != null && innerNode.NodeType == NodeType.Composite)
				{
					return (innerNode as CompositeNode).GetChildNode(remainingName);
				}

				throw new BindingException("node is not Composite but still need to recurse to {0}", remainingName);
			}

			return (Node) name2Node[name];
		}
		
		public Node[] ChildNodes
		{
			get { return (Node[]) nodeList.ToArray(typeof(Node)); }
		}
		
		public int ChildrenCount
		{
			get { return nodeList.Count; }
		}
	}
	
	public class IndexedNode : CompositeNode
	{
		public IndexedNode(string name) : base(name, NodeType.Indexed)
		{
		}
	}
	
	public class LeafNode : Node
	{
		private readonly Type type;
		private readonly object value;

		public LeafNode(Type type, string name, object value) : base(name, NodeType.Leaf)
		{
			if (type == null) throw new ArgumentNullException("type", "A type must be specified");
			if (value == null) throw new ArgumentNullException("value", "A value must be specified");
			
			this.type = type;
			this.value = value;
		}

		public Type ValueType
		{
			get { return type; }
		}

		public bool IsArray
		{
			get { return type.IsArray; }
		}
		
		public object Value
		{
			get { return value; }
		}
	}
}