namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using NVelocity.Context;

	/// <summary>
	/// This class is responsible for handling the Else VTL control statement.
	///
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: ASTElseStatement.cs,v 1.3 2003/10/27 13:54:10 corts Exp $ </version>
	public class ASTElseStatement : SimpleNode
	{
		public ASTElseStatement(int id) : base(id)
		{
		}

		public ASTElseStatement(Parser p, int id) : base(p, id)
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
		/// An ASTElseStatement always evaluates to
		/// true. Basically behaves like an #if(true).
		/// </summary>
		public override bool Evaluate(IInternalContextAdapter context)
		{
			return true;
		}
	}
}
