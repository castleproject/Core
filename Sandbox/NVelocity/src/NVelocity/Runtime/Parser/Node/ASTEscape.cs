namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.IO;
	using NVelocity.Context;

	/// <summary> This class is responsible for handling Escapes
	/// in VTL.
	///
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: ASTEscape.cs,v 1.3 2003/10/27 13:54:10 corts Exp $
	///
	/// </version>
	public class ASTEscape : SimpleNode
	{
		private String text = "";

		public ASTEscape(int id) : base(id)
		{
		}

		public ASTEscape(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>Accept the visitor. *
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		public override Object Init(IInternalContextAdapter context, Object data)
		{
			text = FirstToken.Image;
			return data;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			char[] c = text.ToCharArray();
			writer.Write(c, 0, c.Length);
			return true;
		}
	}
}
