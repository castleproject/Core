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

namespace Castle.MicroKernel.SubSystems.Naming
{
	using System;
	using System.Collections;


	[Serializable]
	public class BinaryTreeComponentName
	{
		private TreeNode _root;
		private int _count;

		public BinaryTreeComponentName()
		{
		}

		public int Count
		{
			get { return _count; }
		}

		public IHandler[] Handlers
		{
			get
			{
				ArrayList list = new ArrayList();
				Visit(_root, list);
				return (IHandler[]) list.ToArray( typeof(IHandler) );
			}
		}

		public bool Contains(ComponentName name)
		{
			return FindNode(name) != null;
		}

		public void Remove(ComponentName name)
		{
			// Not implemented yet
		}

		public void Add(ComponentName name, IHandler handler)
		{
			if (_root == null)
			{
				_root = new TreeNode(name, handler);
				_count++;
				return;
			}

			TreeNode current = _root;

			while(true)
			{
				int cmp = String.Compare(current.CompName.Service, name.Service);

				if ( cmp < 0 )
				{
					if (current.Left != null) 
					{	
						current = current.Left;
					}
					else
					{
						current.Left = new TreeNode(name, handler);
						_count++;
						break;
					}
				}
				else if ( cmp > 0 )
				{
					if (current.Right != null) 
					{	
						current = current.Right;
					}
					else
					{
						current.Right = new TreeNode(name, handler);
						_count++;
						break;
					}
				}
				else
				{
					current.AddSibling(new TreeNode(name, handler));
					_count++;
					break;
				}
			}
		}

		public IHandler GetHandler(ComponentName name)
		{
			TreeNode node = FindNode(name);
			
			return node != null ? node.Handler : null;
		}

		public IHandler[] GetHandlers(ComponentName name)
		{
			TreeNode node = FindNode(name);
			
			if (node != null)
			{
				ArrayList list = new ArrayList();
				
				list.Add(node.Handler);

				while(node.NextSibling != null)
				{
					node = node.NextSibling.FindBestMatch(name);
					
					list.Add(node.Handler);
				}

				return (IHandler[]) list.ToArray( typeof(IHandler) );
			}

			return null;
		}

		internal void Visit(TreeNode node, ArrayList list)
		{
			list.Add(node.Handler);
			
			if (node.Left != null)
			{
				Visit(node.Left, list);
			}
			if (node.Right != null)
			{
				Visit(node.Right, list);
			}

			while(node.NextSibling != null)
			{
				list.Add( node.NextSibling.Handler );
				node = node.NextSibling;
			}
		}

		internal TreeNode FindNode(ComponentName name)
		{
			TreeNode current = _root;

			while(true)
			{
				int cmp = String.Compare(current.CompName.Service, name.Service);

				if ( cmp < 0 )
				{
					if (current.Left != null) 
					{	
						current = current.Left;
					}
					else
					{
						return null;
					}
				}
				else if ( cmp > 0 )
				{
					if (current.Right != null) 
					{	
						current = current.Right;
					}
					else
					{
						return null;
					}
				}
				else
				{
					return current.FindBestMatch(name);
				}
			}
		}
	}


	[Serializable]
	internal class TreeNode
	{
		private ComponentName _CompName;
		private IHandler _Handler;

		/// <summary>Node's left</summary>
		private TreeNode _Left;

		/// <summary>Node's right</summary>
		private TreeNode _Right;
		
		/// <summary>DA Linked List</summary>
		private TreeNode _NextSibling;

		public TreeNode(ComponentName compName, IHandler handler)
		{
			if (compName == null) throw new ArgumentNullException("compName");
			if (handler == null) throw new ArgumentNullException("handler");

			_CompName = compName;
			_Handler = handler;
		}

		public ComponentName CompName
		{
			get { return _CompName; }
		}

		public IHandler Handler
		{
			get { return _Handler; }
		}

		public TreeNode Left
		{
			get { return _Left; }
			set { _Left = value; }
		}

		public TreeNode Right
		{
			get { return _Right; }
			set { _Right = value; }
		}

		public TreeNode NextSibling
		{
			get { return _NextSibling; }
			set { _NextSibling = value; }
		}

		public void AddSibling(TreeNode node)
		{
			if (NextSibling == null)
			{
				NextSibling = node; return;
			}

			TreeNode current = NextSibling;
			
			while(current.NextSibling != null)
			{
				current = current.NextSibling;
			}

			current.NextSibling = node;
		}

		public TreeNode FindBestMatch(ComponentName name)
		{
			TreeNode current = this;

			while(current != null)
			{
				if ("*".Equals(name.LiteralProperties))
				{
					break;
				}

				bool selected = true;

				foreach(DictionaryEntry entry in name.Properties)
				{
					String value = current.CompName.Properties[ entry.Key ] as String;

					if (value == null || !value.Equals(entry.Value))
					{
						selected = false;
						break;
					}
				}

				if (selected) break;

				current = current.NextSibling;
			}

			return current;
		}
	}
}
