namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.IO;
	using NVelocity.Context;

	public class ASTBlock : SimpleNode
	{
		public ASTBlock(int id) : base(id)
		{
		}

		public ASTBlock(Parser p, int id) : base(p, id)
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
			int i, k = ChildrenCount;

			for (i = 0; i < k; i++)
				GetChild(i).Render(context, writer);

			return true;
		}
	}
}
