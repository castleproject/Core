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
	using System.Collections.Specialized;
	using System.Web;

	/// <summary>
	/// 
	/// </summary>
	public class TreeBuilder
	{
		public CompositeNode BuildSourceNode(NameValueCollection nameValueCollection)
		{
			CompositeNode root = new CompositeNode("root");
			
			PopulateTree(root, nameValueCollection);
			
			return root;
		}
		
		public void PopulateTree(CompositeNode root, NameValueCollection nameValueCollection)
		{
			foreach(String key in nameValueCollection.Keys)
			{
				if (key == null) continue;

				String[] values = nameValueCollection.GetValues(key);

				if (values == null) continue;
				
				if (values.Length == 1)
				{
					ProcessNode(root, typeof(String), key, values[0]);
				}
				else
				{
					ProcessNode(root, typeof(String[]), key, values);
				}
			}
		}

		public void PopulateTree(CompositeNode root, HttpFileCollection fileCollection)
		{
			foreach(String key in fileCollection.Keys)
			{
				if (key == null) continue;

				HttpPostedFile value = fileCollection.Get(key);

				if (value == null) continue;
				
				ProcessNode(root, typeof(HttpPostedFile), key, value);
			}
		}
		
		private void ProcessNode(CompositeNode node, Type type, String name, object value)
		{
			if (name == null || name == String.Empty)
			{
				// Ignore
				return;
			}
			if (name[0] == '.' || name[0] == '[' || name[0] == ']')
			{
				AddLeafNode(node, type, name, value);
				return;
			}
			
			int dotIndex = name.IndexOf('.');
			int startBracketIndex = name.IndexOf('[');
				
			if (dotIndex != -1 && (startBracketIndex == -1 || dotIndex < startBracketIndex))
			{
				// Child node
					
				String childNodeName = name.Substring(0, dotIndex);
				
				CompositeNode newNode = GetOrCreateCompositeNode(node, childNodeName);
					
				ProcessNode(newNode, type, name.Substring(dotIndex + 1), value);
			}
			else if (startBracketIndex != -1)
			{
				// Indexed node
					
				int endBracket = name.IndexOf(']');
				
				if (endBracket == -1)
				{
					// TODO: Something is wrong
				}
				
				String enclosed = name.Substring(startBracketIndex + 1, endBracket - startBracketIndex - 1);
				
				if (enclosed == null || enclosed == String.Empty)
				{
					// TODO: Something is wrong
				}
				
				String indexedNodeName = name.Substring(0, startBracketIndex);
				
				CompositeNode newNode = GetOrCreateIndexedNode(node, indexedNodeName);
				
				if (endBracket + 1 == name.Length) // entries like emails[0] = value
				{
					AddLeafNode(newNode, type, value);
				}
				else
				{
					name = name.Substring(endBracket + 2); // entries like customer[0].name = value

					newNode = GetOrCreateCompositeNode(newNode, enclosed);
					
					ProcessNode(newNode, type, name, value);
				}
			}
			else
			{
				AddLeafNode(node, type, name, value);
			}
		}

		private void AddLeafNode(CompositeNode parent, Type type, object value)
		{
			AddLeafNode(parent, type, String.Empty, value);
		}
		
		private void AddLeafNode(CompositeNode parent, Type type, String nodeName, object value)
		{
			parent.AddChildNode(new LeafNode(type, nodeName, value));
		}

		private CompositeNode GetOrCreateCompositeNode(CompositeNode parent, string nodeName)
		{
			Node node = parent.GetChildNode(nodeName);
			
			if (node != null && node.NodeType != NodeType.Composite)
			{
				throw new BindingException("Attempt to create or obtain a composite node " + 
					"named {0}, but a node with the same exists with the type {1}", nodeName, node.NodeType);
			}
			
			if (node == null)
			{
				node = new CompositeNode(nodeName);
				parent.AddChildNode(node);
			}
			
			return (CompositeNode) node;
		}

		private IndexedNode GetOrCreateIndexedNode(CompositeNode parent, string nodeName)
		{
			Node node = parent.GetChildNode(nodeName);
			
			if (node != null && node.NodeType != NodeType.Indexed)
			{
				throw new BindingException("Attempt to create or obtain an indexed node " + 
					"named {0}, but a node with the same exists with the type {1}", nodeName, node.NodeType);
			}
			
			if (node == null)
			{
				node = new IndexedNode(nodeName);
				parent.AddChildNode(node);
			}
			
			return (IndexedNode) node;
		}
	}
}