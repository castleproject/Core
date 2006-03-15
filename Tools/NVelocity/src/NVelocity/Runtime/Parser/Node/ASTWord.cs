namespace NVelocity.Runtime.Parser.Node
{
	using System;

	public class ASTWord : SimpleNode
	{
		public ASTWord(int id) : base(id)
		{
		}

		public ASTWord(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
