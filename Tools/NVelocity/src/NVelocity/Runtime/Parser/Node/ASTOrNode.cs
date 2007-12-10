namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using Context;

	/// <summary>
	/// Please look at the Parser.jjt file which is what controls 
	/// the generation of this class.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: ASTOrNode.cs,v 1.3 2003/10/27 13:54:10 corts Exp $ </version>
	public class ASTOrNode : SimpleNode
	{
		public ASTOrNode(int id) : base(id)
		{
		}

		public ASTOrNode(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>Accept the visitor. *
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		/// <summary>  Returns the value of the expression.
		/// Since the value of the expression is simply the boolean
		/// result of evaluate(), lets return that.
		/// </summary>
		public override Object Value(IInternalContextAdapter context)
		{
			return Evaluate(context);
		}

		/// <summary>
		/// the logical or :
		/// the rule :
		/// left || null -> left
		/// null || right -> right
		/// null || null -> false
		/// left || right ->  left || right
		/// </summary>
		public override bool Evaluate(IInternalContextAdapter context)
		{
			INode left = GetChild(0);
			INode right = GetChild(1);

			// if the left is not null and true, then true
			if (left != null && left.Evaluate(context))
			{
				return true;
			}

			// same for right
			if (right != null && right.Evaluate(context))
			{
				return true;
			}

			return false;
		}
	}
}