namespace NVelocity.Runtime.Visitor
{
	using System;
	using System.Text;
	using NVelocity.Runtime.Parser;
	using NVelocity.Runtime.Parser.Node;

	/// <summary> This class is simply a visitor implementation
	/// that traverses the AST, produced by the Velocity
	/// parsing process, and creates a visual structure
	/// of the AST. This is primarily used for
	/// debugging, but it useful for documentation
	/// as well.
	/// *
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
	/// </author>
	/// <version> $Id: NodeViewMode.cs,v 1.3 2003/10/27 13:54:11 corts Exp $
	///
	/// </version>
	public class NodeViewMode : BaseVisitor
	{
		private int indent = 0;
		private bool showTokens = true;

		/// <summary>Indent child nodes to help visually identify
		/// the structure of the AST.
		/// </summary>
		private String indentString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < indent; ++i)
			{
				sb.Append("  ");
			}
			return sb.ToString();
		}

		/// <summary> Display the type of nodes and optionally the
		/// first token.
		/// </summary>
		private Object showNode(INode node, Object data)
		{
			String tokens = "";
			String special = "";
			Token t;

			if (showTokens)
			{
				t = node.FirstToken;

				if (t.SpecialToken != null && !t.SpecialToken.Image.StartsWith("##"))
					special = t.SpecialToken.Image;

				tokens = " -> " + special + t.Image;
			}

			Console.Out.WriteLine(indentString() + node + tokens);
			++indent;
			data = node.ChildrenAccept(this, data);
			--indent;
			return data;
		}

		/// <summary>Display a SimpleNode
		/// </summary>
		public override Object Visit(SimpleNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTprocess node
		/// </summary>
		public override Object Visit(ASTprocess node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTExpression node
		/// </summary>
		public override Object Visit(ASTExpression node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTAssignment node ( = )
		/// </summary>
		public override Object Visit(ASTAssignment node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTOrNode ( || )
		/// </summary>
		public override Object Visit(ASTOrNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTAndNode ( && )
		/// </summary>
		public override Object Visit(ASTAndNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTEQNode ( == )
		/// </summary>
		public override Object Visit(ASTEQNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTNENode ( != )
		/// </summary>
		public override Object Visit(ASTNENode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTLTNode ( < )
		/// </summary>
		public override Object Visit(ASTLTNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTGTNode ( > )
		/// </summary>
		public override Object Visit(ASTGTNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTLENode ( <= )
		/// </summary>
		public override Object Visit(ASTLENode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTGENode ( >= )
		/// </summary>
		public override Object Visit(ASTGENode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTAddNode ( + )
		/// </summary>
		public override Object Visit(ASTAddNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTSubtractNode ( - )
		/// </summary>
		public override Object Visit(ASTSubtractNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTMulNode ( * )
		/// </summary>
		public override Object Visit(ASTMulNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTDivNode ( / )
		/// </summary>
		public override Object Visit(ASTDivNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTModNode ( % )
		/// </summary>
		public override Object Visit(ASTModNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTNotNode ( ! )
		/// </summary>
		public override Object Visit(ASTNotNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTNumberLiteral node
		/// </summary>
		public override Object Visit(ASTNumberLiteral node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTStringLiteral node
		/// </summary>
		public override Object Visit(ASTStringLiteral node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTIdentifier node
		/// </summary>
		public override Object Visit(ASTIdentifier node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTMethod node
		/// </summary>
		public override Object Visit(ASTMethod node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTReference node
		/// </summary>
		public override Object Visit(ASTReference node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTTrue node
		/// </summary>
		public override Object Visit(ASTTrue node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTFalse node
		/// </summary>
		public override Object Visit(ASTFalse node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTBlock node
		/// </summary>
		public override Object Visit(ASTBlock node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTText node
		/// </summary>
		public override Object Visit(ASTText node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTIfStatement node
		/// </summary>
		public override Object Visit(ASTIfStatement node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTElseStatement node
		/// </summary>
		public override Object Visit(ASTElseStatement node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTElseIfStatement node
		/// </summary>
		public override Object Visit(ASTElseIfStatement node, Object data)
		{
			return showNode(node, data);
		}

		public override Object Visit(ASTObjectArray node, Object data)
		{
			return showNode(node, data);
		}

		public override Object Visit(ASTDirective node, Object data)
		{
			return showNode(node, data);
		}

		public override Object Visit(ASTWord node, Object data)
		{
			return showNode(node, data);
		}

		public override Object Visit(ASTSetDirective node, Object data)
		{
			return showNode(node, data);
		}
	}
}