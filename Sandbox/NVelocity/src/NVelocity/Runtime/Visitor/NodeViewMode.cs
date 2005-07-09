using System;
using NVelocity;
using NVelocity.Runtime.Parser;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Context;

namespace NVelocity.Runtime.Visitor {

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
    public class NodeViewMode : BaseVisitor {
	private int indent = 0;
	private bool showTokens = true;

	/// <summary>Indent child nodes to help visually identify
	/// the structure of the AST.
	/// </summary>
	private System.String indentString() {
	    System.Text.StringBuilder sb = new System.Text.StringBuilder();
	    for (int i = 0; i < indent; ++i) {
		sb.Append("  ");
	    }
	    return sb.ToString();
	}

	/// <summary> Display the type of nodes and optionally the
	/// first token.
	/// </summary>
	private System.Object showNode(INode node, System.Object data) {
	    System.String tokens = "";
	    System.String special = "";
	    Token t;

	    if (showTokens) {
		t = node.FirstToken;

		if (t.specialToken != null && !t.specialToken.image.StartsWith("##"))
		    special = t.specialToken.image;

		tokens = " -> " + special + t.image;
	    }

	    System.Console.Out.WriteLine(indentString() + node + tokens);
	    ++indent;
	    data = node.childrenAccept(this, data);
	    --indent;
	    return data;
	}

	/// <summary>Display a SimpleNode
	/// </summary>
	public override System.Object visit(SimpleNode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTprocess node
	/// </summary>
	public override System.Object visit(ASTprocess node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTExpression node
	/// </summary>
	public override System.Object visit(ASTExpression node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTAssignment node ( = )
	/// </summary>
	public override System.Object visit(ASTAssignment node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTOrNode ( || )
	/// </summary>
	public override System.Object visit(ASTOrNode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTAndNode ( && )
	/// </summary>
	public override System.Object visit(ASTAndNode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTEQNode ( == )
	/// </summary>
	public override System.Object visit(ASTEQNode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTNENode ( != )
	/// </summary>
	public override System.Object visit(ASTNENode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTLTNode ( < )
	/// </summary>
	public override System.Object visit(ASTLTNode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTGTNode ( > )
	/// </summary>
	public override System.Object visit(ASTGTNode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTLENode ( <= )
	/// </summary>
	public override System.Object visit(ASTLENode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTGENode ( >= )
	/// </summary>
	public override System.Object visit(ASTGENode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTAddNode ( + )
	/// </summary>
	public override System.Object visit(ASTAddNode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTSubtractNode ( - )
	/// </summary>
	public override System.Object visit(ASTSubtractNode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTMulNode ( * )
	/// </summary>
	public override System.Object visit(ASTMulNode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTDivNode ( / )
	/// </summary>
	public override System.Object visit(ASTDivNode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTModNode ( % )
	/// </summary>
	public override System.Object visit(ASTModNode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTNotNode ( ! )
	/// </summary>
	public override System.Object visit(ASTNotNode node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTNumberLiteral node
	/// </summary>
	public override System.Object visit(ASTNumberLiteral node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTStringLiteral node
	/// </summary>
	public override System.Object visit(ASTStringLiteral node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTIdentifier node
	/// </summary>
	public override System.Object visit(ASTIdentifier node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTMethod node
	/// </summary>
	public override System.Object visit(ASTMethod node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTReference node
	/// </summary>
	public override System.Object visit(ASTReference node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTTrue node
	/// </summary>
	public override System.Object visit(ASTTrue node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTFalse node
	/// </summary>
	public override System.Object visit(ASTFalse node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTBlock node
	/// </summary>
	public override System.Object visit(ASTBlock node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTText node
	/// </summary>
	public override System.Object visit(ASTText node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTIfStatement node
	/// </summary>
	public override System.Object visit(ASTIfStatement node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTElseStatement node
	/// </summary>
	public override System.Object visit(ASTElseStatement node, System.Object data) {
	    return showNode(node, data);
	}

	/// <summary>Display an ASTElseIfStatement node
	/// </summary>
	public override System.Object visit(ASTElseIfStatement node, System.Object data) {
	    return showNode(node, data);
	}

	public override System.Object visit(ASTObjectArray node, System.Object data) {
	    return showNode(node, data);
	}

	public override System.Object visit(ASTDirective node, System.Object data) {
	    return showNode(node, data);
	}

	public override System.Object visit(ASTWord node, System.Object data) {
	    return showNode(node, data);
	}

	public override System.Object visit(ASTSetDirective node, System.Object data) {
	    return showNode(node, data);
	}
    }
}
