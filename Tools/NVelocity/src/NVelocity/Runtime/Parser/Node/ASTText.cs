namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.IO;
	using NVelocity.Context;

	public class ASTText : SimpleNode
	{
		private char[] ctext;

		public ASTText(int id) : base(id)
		{
		}

		public ASTText(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		public override Object Init(IInternalContextAdapter context, Object data)
		{
			Token t = FirstToken;

			String text = NodeUtils.tokenLiteral(t);

			ctext = text.ToCharArray();

			return data;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			writer.Write(ctext, 0, ctext.Length);
			return true;
		}
	}
}
