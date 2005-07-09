namespace NVelocity.Runtime.Visitor
{
	using System;
	using System.IO;
	using NVelocity.Context;
	using NVelocity.Runtime.Parser.Node;

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
	public abstract class BaseVisitor : ParserVisitor
	{
		public StreamWriter Writer
		{
			set { this.writer = value; }

		}

		public InternalContextAdapter Context
		{
			set { this.context = value; }

		}

		/// <summary>Context used during traversal
		/// </summary>
		protected internal InternalContextAdapter context;

		/// <summary>Writer used as the output sink
		/// </summary>
		protected internal StreamWriter writer;


		public virtual Object visit(SimpleNode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTprocess node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTExpression node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTAssignment node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTOrNode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTAndNode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTEQNode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTNENode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTLTNode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTGTNode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTLENode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTGENode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTAddNode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTSubtractNode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTMulNode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTDivNode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTModNode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTNotNode node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTNumberLiteral node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTStringLiteral node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTIdentifier node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTMethod node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTReference node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTTrue node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTFalse node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTBlock node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTText node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTIfStatement node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTElseStatement node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTElseIfStatement node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTComment node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTObjectArray node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTWord node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTSetDirective node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}

		public virtual Object visit(ASTDirective node, Object data)
		{
			data = node.childrenAccept(this, data);
			return data;
		}
	}
}