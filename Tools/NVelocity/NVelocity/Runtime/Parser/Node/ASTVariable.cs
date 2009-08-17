namespace NVelocity.Runtime.Parser.Node
{
	using System;

	public class ASTVariable : SimpleNode
	{
		public ASTVariable(int id) : base(id)
		{
		}

		public ASTVariable(Parser p, int id) : base(p, id)
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