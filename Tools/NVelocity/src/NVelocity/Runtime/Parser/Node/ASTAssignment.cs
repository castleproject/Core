namespace NVelocity.Runtime.Parser.Node
{
	using System;

	public class ASTAssignment : SimpleNode
	{
		public ASTAssignment(int id) : base(id)
		{
		}

		public ASTAssignment(Parser p, int id) : base(p, id)
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
