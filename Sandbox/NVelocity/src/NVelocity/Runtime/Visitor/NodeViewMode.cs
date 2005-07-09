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

				if (t.specialToken != null && !t.specialToken.image.StartsWith("##"))
					special = t.specialToken.image;

				tokens = " -> " + special + t.image;
			}

			Console.Out.WriteLine(indentString() + node + tokens);
			++indent;
			data = node.childrenAccept(this, data);
			--indent;
			return data;
		}

		/// <summary>Display a SimpleNode
		/// </summary>
		public override Object visit(SimpleNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTprocess node
		/// </summary>
		public override Object visit(ASTprocess node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTExpression node
		/// </summary>
		public override Object visit(ASTExpression node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTAssignment node ( = )
		/// </summary>
		public override Object visit(ASTAssignment node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTOrNode ( || )
		/// </summary>
		public override Object visit(ASTOrNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTAndNode ( && )
		/// </summary>
		public override Object visit(ASTAndNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTEQNode ( == )
		/// </summary>
		public override Object visit(ASTEQNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTNENode ( != )
		/// </summary>
		public override Object visit(ASTNENode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTLTNode ( < )
		/// </summary>
		public override Object visit(ASTLTNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTGTNode ( > )
		/// </summary>
		public override Object visit(ASTGTNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTLENode ( <= )
		/// </summary>
		public override Object visit(ASTLENode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTGENode ( >= )
		/// </summary>
		public override Object visit(ASTGENode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTAddNode ( + )
		/// </summary>
		public override Object visit(ASTAddNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTSubtractNode ( - )
		/// </summary>
		public override Object visit(ASTSubtractNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTMulNode ( * )
		/// </summary>
		public override Object visit(ASTMulNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTDivNode ( / )
		/// </summary>
		public override Object visit(ASTDivNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTModNode ( % )
		/// </summary>
		public override Object visit(ASTModNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTNotNode ( ! )
		/// </summary>
		public override Object visit(ASTNotNode node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTNumberLiteral node
		/// </summary>
		public override Object visit(ASTNumberLiteral node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTStringLiteral node
		/// </summary>
		public override Object visit(ASTStringLiteral node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTIdentifier node
		/// </summary>
		public override Object visit(ASTIdentifier node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTMethod node
		/// </summary>
		public override Object visit(ASTMethod node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTReference node
		/// </summary>
		public override Object visit(ASTReference node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTTrue node
		/// </summary>
		public override Object visit(ASTTrue node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTFalse node
		/// </summary>
		public override Object visit(ASTFalse node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTBlock node
		/// </summary>
		public override Object visit(ASTBlock node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTText node
		/// </summary>
		public override Object visit(ASTText node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTIfStatement node
		/// </summary>
		public override Object visit(ASTIfStatement node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTElseStatement node
		/// </summary>
		public override Object visit(ASTElseStatement node, Object data)
		{
			return showNode(node, data);
		}

		/// <summary>Display an ASTElseIfStatement node
		/// </summary>
		public override Object visit(ASTElseIfStatement node, Object data)
		{
			return showNode(node, data);
		}

		public override Object visit(ASTObjectArray node, Object data)
		{
			return showNode(node, data);
		}

		public override Object visit(ASTDirective node, Object data)
		{
			return showNode(node, data);
		}

		public override Object visit(ASTWord node, Object data)
		{
			return showNode(node, data);
		}

		public override Object visit(ASTSetDirective node, Object data)
		{
			return showNode(node, data);
		}
	}
}