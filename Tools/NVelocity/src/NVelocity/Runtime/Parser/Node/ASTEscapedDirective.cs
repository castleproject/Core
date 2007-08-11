namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.IO;
	using Context;

	/// <summary>
	/// This class is responsible for handling EscapedDirectives
	/// in VTL.
	///
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: ASTEscapedDirective.cs,v 1.3 2003/10/27 13:54:10 corts Exp $ </version>
	public class ASTEscapedDirective : SimpleNode
	{
		public ASTEscapedDirective(int id) : base(id)
		{
		}

		public ASTEscapedDirective(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			writer.Write(FirstToken.Image);
			return true;
		}
	}
}