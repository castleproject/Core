namespace NVelocity.Runtime.Parser.Node
{
	using System;

	public class ASTComment : SimpleNode
	{
		public ASTComment(int id) : base(id)
		{
		}

		public ASTComment(Parser p, int id) : base(p, id)
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