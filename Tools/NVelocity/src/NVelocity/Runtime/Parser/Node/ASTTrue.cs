namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using NVelocity.Context;

	public class ASTTrue : SimpleNode
	{
		private const bool val = true;

		public ASTTrue(int id) : base(id)
		{
		}

		public ASTTrue(Parser p, int id) : base(p, id)
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
			return true;
		}

		public override Object Value(IInternalContextAdapter context)
		{
			return val;
		}
	}
}
