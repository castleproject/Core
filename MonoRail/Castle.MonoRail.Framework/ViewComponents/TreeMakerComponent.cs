// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.ViewComponents
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Text;

	/// <summary>
	/// This view component deals with object and children as a tree, rendering nodes in a depth-first algorithm fashion, building
	/// a tree using html's divs. 
	/// You must specify the name of the property that retrieve the children of an object using the <see cref="CollectionPropertyName"/>
	/// </summary>
	/// 
	/// <example>
	/// The following example uses nvelocity view engine syntax.
	/// <code>
	/// <![CDATA[
	/// #blockcomponent(TreeMakerComponent "rootnode=$sitemapnode,CollectionPropertyName='ChildNodes'")
	/// 
	/// #text
	///   $currentnode.data.Name
	/// #end
	/// 
	/// #end
	/// ]]>
	/// </code>
	/// </example>
	/// 
	/// <remarks>
	/// By default this view component will output inline styles for the div elements, such as 
	/// <c>float: left; width: 19px; height: 20px; background: transparent url(/Content/images/i.gif)</c>.
	/// You can set the property <see cref="UseInlineStyle"/> to false so the view component will use 
	/// the following classes instead: tree-pipe, tree-blank, tree-branchend and tree-branchline.
	/// 
	/// <para>
	/// You can use the following nested sections:
	/// Supported sections: <br/>
	/// <c>node</c>: when supplied, no tree will be outputted. instead the section will get a Node structure that has information like
	/// level, data and so on, so you can build your own tree. <br/>
	/// <c>text</c>: used to render the node content (name, description?) <br/>
	/// </para>
	/// </remarks>
	[ViewComponentDetails("TreeMakerComponent", Sections = "node,text")]
	public class TreeMakerComponent : ViewComponent
	{
		private string collectionPropertyName;
		private object rootNode;
		private bool useInlineStyle = true;
		private PropertyInfo collProperty;

		/// <summary>
		/// Gets or sets the root node.
		/// </summary>
		/// <value>The root node.</value>
		[ViewComponentParam(Required = true)]
		public object RootNode
		{
			get { return rootNode; }
			set { rootNode = value; }
		}

		/// <summary>
		/// Gets or sets the name of the collection property.
		/// </summary>
		/// <value>The name of the collection property.</value>
		[ViewComponentParam(Required = true)]
		public string CollectionPropertyName
		{
			get { return collectionPropertyName; }
			set { collectionPropertyName = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [use inline style].
		/// </summary>
		/// <value><c>true</c> if [use inline style]; otherwise, <c>false</c>.</value>
		[ViewComponentParam]
		public bool UseInlineStyle
		{
			get { return useInlineStyle; }
			set { useInlineStyle = value; }
		}

		/// <summary>
		/// Called by the framework once the component instance
		/// is initialized
		/// </summary>
		public override void Initialize()
		{
			collProperty = rootNode.GetType().GetProperty(collectionPropertyName, BindingFlags.Public | BindingFlags.Instance);

			base.Initialize();
		}

		/// <summary>
		/// Called by the framework so the component can
		/// render its content
		/// </summary>
		public override void Render()
		{
			Node processedNode = BuildNodeHierarchy(rootNode, 0);

			StringBuilder sb = new StringBuilder();

			RecursiveRenderNode(processedNode, sb);

			RenderText(sb.ToString());
		}

		/// <summary>
		/// Recursively renders the specified node.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="sb">The string builder.</param>
		protected virtual void RecursiveRenderNode(Node node, StringBuilder sb)
		{
			if (HasSection("node"))
			{
				PropertyBag["currentnode"] = node;
				RenderSection("node", new StringWriter(sb));
			}
			else
			{
				WriteNodeDiv(node, sb);
			}

			foreach(Node child in node.Children)
			{
				RecursiveRenderNode(child, sb);
			}
		}

		/// <summary>
		/// Writes a div element for the specified node.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="sb">The string builder.</param>
		protected virtual void WriteNodeDiv(Node node, StringBuilder sb)
		{
			RecursiveWriteLine(node.Parent, sb);

			if (node.Level != 0)
			{
				if (node.HasNext)
				{
					RenderBranchLine(sb);
				}
				else
				{
					RenderEndOfBranchLine(sb);
				}
			}

			if (HasSection("text"))
			{
				PropertyBag["currentnode"] = node;
				RenderSection("text", new StringWriter(sb));
			}
			else
			{
				sb.Append(node.Data);
			}

			sb.AppendLine("<div style=\"clear: both\"></div>");
		}

		/// <summary>
		/// Recursively writes the lines for the tree nodes.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="sb">The string builder.</param>
		protected virtual void RecursiveWriteLine(Node node, StringBuilder sb)
		{
			if (node == null) return;
			if (node.Level == 0) return;

			RecursiveWriteLine(node.Parent, sb);

			if (node.HasNext)
			{
				RenderPipeLine(sb);
			}
			else
			{
				RenderLinePlaceholder(sb);
			}
		}

		/// <summary>
		/// Renders a pipe kind of line for the tree.
		/// </summary>
		/// <param name="sb">The string builder.</param>
		protected virtual void RenderPipeLine(StringBuilder sb)
		{
			if (useInlineStyle)
			{
				sb.AppendLine(
					"<div style=\"float: left; width: 19px; height: 20px; background: transparent url(/Content/images/i.gif)\"></div>");
			}
			else
			{
				sb.AppendLine("<div class=\"tree-pipe\"></div>");
			}
		}

		/// <summary>
		/// Renders a line placeholder.
		/// </summary>
		/// <param name="sb">The string builder.</param>
		protected virtual void RenderLinePlaceholder(StringBuilder sb)
		{
			if (useInlineStyle)
			{
				sb.AppendLine(
					"<div style=\"float: left; width: 19px; height: 20px; background: transparent url(/Content/images/noexpand.gif)\"></div>");
			}
			else
			{
				sb.AppendLine("<div class=\"tree-blank\"></div>");
			}
		}

		/// <summary>
		/// Renders the end of branch line.
		/// </summary>
		/// <param name="sb">The string builder.</param>
		protected virtual void RenderEndOfBranchLine(StringBuilder sb)
		{
			if (useInlineStyle)
			{
				sb.AppendLine(
					"<div style=\"float: left; width: 19px; height: 20px; background: transparent url(/Content/images/l.gif)\"></div>");
			}
			else
			{
				sb.AppendLine("<div class=\"tree-branchend\"></div>");
			}
		}

		/// <summary>
		/// Renders the branch line.
		/// </summary>
		/// <param name="sb">The string builder.</param>
		protected virtual void RenderBranchLine(StringBuilder sb)
		{
			if (useInlineStyle)
			{
				sb.AppendLine(
					"<div style=\"float: left; width: 19px; height: 20px; background: transparent url(/Content/images/t.gif)\"></div>");
			}
			else
			{
				sb.AppendLine("<div class=\"tree-branchline\"></div>");
			}
		}

		/// <summary>
		/// Builds the node hierarchy.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="level">The level.</param>
		/// <returns></returns>
		protected virtual Node BuildNodeHierarchy(object node, int level)
		{
			Node processedNode = new Node(node, level);

			IEnumerable children = (IEnumerable) collProperty.GetValue(node, null);

			foreach(object child in children)
			{
				Node childNode = BuildNodeHierarchy(child, level + 1);
				childNode.Parent = processedNode;

				processedNode.Children.Add(childNode);
			}

			processedNode.MarkLastChild();

			return processedNode;
		}

		/// <summary>
		/// Represents a tree node
		/// </summary>
		public class Node
		{
			private bool hasNext = true;
			private Node parent;
			private readonly object data;
			private readonly int level;
			private readonly List<Node> children = new List<Node>();

			/// <summary>
			/// Initializes a new instance of the <see cref="Node"/> class.
			/// </summary>
			/// <param name="data">The data.</param>
			/// <param name="level">The level.</param>
			public Node(object data, int level)
			{
				this.data = data;
				this.level = level;
			}

			/// <summary>
			/// Gets or sets the parent.
			/// </summary>
			/// <value>The parent.</value>
			public Node Parent
			{
				get { return parent; }
				set { parent = value; }
			}

			/// <summary>
			/// Gets a value indicating whether this instance has children.
			/// </summary>
			/// <value>
			/// 	<c>true</c> if this instance has children; otherwise, <c>false</c>.
			/// </value>
			public bool HasChildren
			{
				get { return children.Count != 0; }
			}

			/// <summary>
			/// Gets the level.
			/// </summary>
			/// <value>The level.</value>
			public int Level
			{
				get { return level; }
			}

			/// <summary>
			/// Gets a value indicating whether this instance has a next sibling.
			/// </summary>
			/// <value><c>true</c> if this instance has next; otherwise, <c>false</c>.</value>
			public bool HasNext
			{
				get { return hasNext; }
			}

			/// <summary>
			/// Gets the data.
			/// </summary>
			/// <value>The data.</value>
			public object Data
			{
				get { return data; }
			}

			/// <summary>
			/// Gets the children.
			/// </summary>
			/// <value>The children.</value>
			public List<Node> Children
			{
				get { return children; }
			}

			/// <summary>
			/// Marks the last child.
			/// </summary>
			public void MarkLastChild()
			{
				if (HasChildren)
				{
					children[children.Count - 1].hasNext = false;
				}
			}
		}
	}
}