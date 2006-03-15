namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.IO;
	using NVelocity.Context;

	/// <summary> This class is responsible for handling the ElseIf VTL control statement.
	///
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// *
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
	/// </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: ASTElseIfStatement.cs,v 1.3 2003/10/27 13:54:10 corts Exp $
	/// </version>
	public class ASTElseIfStatement : SimpleNode
	{
		public ASTElseIfStatement(int id) : base(id)
		{
		}

		public ASTElseIfStatement(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		/// <summary>
		/// An ASTElseStatement is true if the expression
		/// it contains evaluates to true. Expressions know
		/// how to evaluate themselves, so we do that
		/// here and return the value back to ASTIfStatement
		/// where this node was originally asked to evaluate
		/// itself.
		/// </summary>
		public override bool Evaluate(IInternalContextAdapter context)
		{
			return GetChild(0).Evaluate(context);
		}

		/// <summary>
		/// renders the block
		/// </summary>
		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			return GetChild(1).Render(context, writer);
		}
	}
}
