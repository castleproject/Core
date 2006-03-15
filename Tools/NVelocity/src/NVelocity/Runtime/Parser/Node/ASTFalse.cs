namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using NVelocity.Context;

	public class ASTFalse : SimpleNode
	{
		private const bool val = false;

		public ASTFalse(int id) : base(id)
		{
		}

		public ASTFalse(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Evaluate(IInternalContextAdapter context)
		{
			return false;
		}

		public override Object Value(IInternalContextAdapter context)
		{
			return val;
		}
	}
}
