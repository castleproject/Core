namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.Collections;
	using Context;

	public class ASTObjectArray : SimpleNode
	{
		public ASTObjectArray(int id) : base(id)
		{
		}

		public ASTObjectArray(Parser p, int id) : base(p, id)
		{
		}


		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		public override Object Value(IInternalContextAdapter context)
		{
			int size = ChildrenCount;

			ArrayList objectArray = new ArrayList(size);

			for(int i = 0; i < size; i++)
				objectArray.Add(GetChild(i).Value(context));

			return objectArray;
		}
	}
}