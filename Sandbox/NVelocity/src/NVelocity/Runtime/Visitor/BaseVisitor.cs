using System;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Context;

namespace NVelocity.Runtime.Visitor {

    /// <summary> This is the base class for all visitors.
    /// For each AST node, this class will provide
    /// a bare-bones method for traversal.
    /// *
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version> $Id: BaseVisitor.cs,v 1.3 2003/10/27 13:54:11 corts Exp $
    ///
    /// </version>
    public abstract class BaseVisitor : ParserVisitor {
	public virtual System.IO.StreamWriter Writer {
	    set {
		this.writer = value;
	    }

	}
	public virtual InternalContextAdapter Context {
	    set {
		this.context = value;
	    }

	}
	/// <summary>Context used during traversal
	/// </summary>
	protected internal InternalContextAdapter context;

	/// <summary>Writer used as the output sink
	/// </summary>
	protected internal System.IO.StreamWriter writer;



	public virtual System.Object visit(SimpleNode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTprocess node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTExpression node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTAssignment node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTOrNode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTAndNode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTEQNode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTNENode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTLTNode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTGTNode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTLENode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTGENode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTAddNode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTSubtractNode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTMulNode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTDivNode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTModNode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTNotNode node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTNumberLiteral node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTStringLiteral node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTIdentifier node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTMethod node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTReference node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTTrue node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTFalse node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTBlock node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTText node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTIfStatement node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTElseStatement node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTElseIfStatement node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTComment node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTObjectArray node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTWord node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTSetDirective node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}

	public virtual System.Object visit(ASTDirective node, System.Object data) {
	    data = node.childrenAccept(this, data);
	    return data;
	}
    }
}
