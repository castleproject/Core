namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using Context;

	public class ASTNumberLiteral : SimpleNode
	{
		private Int32 valueField;

		public ASTNumberLiteral(int id) : base(id)
		{
		}

		public ASTNumberLiteral(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>Accept the visitor. *
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		/// <summary>  Initialization method - doesn't do much but do the object
		/// creation.  We only need to do it once.
		/// </summary>
		public override Object Init(IInternalContextAdapter context, Object data)
		{
			/*
	    *  init the tree correctly
	    */

			base.Init(context, data);

			valueField = Int32.Parse(FirstToken.Image);

			return data;
		}

		public override Object Value(IInternalContextAdapter context)
		{
			return valueField;
		}
	}
}