namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.IO;
	using Context;

	/// <summary> This class is responsible for handling Escapes
	/// in VTL.
	///
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// *
	/// </summary>
	public class ASTEscape : SimpleNode
	{
		private String text = string.Empty;

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
			writer.Write(text);
			return true;
		}
	}
}