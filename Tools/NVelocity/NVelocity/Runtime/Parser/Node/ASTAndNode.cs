namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using Context;

	/// <summary>
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: ASTAndNode.cs,v 1.3 2003/10/27 13:54:10 corts Exp $ </version>
	public class ASTAndNode : SimpleNode
	{
		public ASTAndNode(int id) : base(id)
		{
		}

		public ASTAndNode(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
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
		/// logical and :
		/// null &amp;&amp; right = false
		/// left &amp;&amp; null = false
		/// null &amp;&amp; null = false
		/// </summary>
		public override bool Evaluate(IInternalContextAdapter context)
		{
			INode left = GetChild(0);
			INode right = GetChild(1);

			// if either is null, lets log and bail
			if (left == null || right == null)
			{
				runtimeServices.Error(
					string.Format("{0} side of '&&' operation is null. Operation not possible. {1} [line {2}, column {3}]",
					              (left == null ? "Left" : "Right"), context.CurrentTemplateName, Line, Column));
				return false;
			}

			// short circuit the test.  Don't eval the RHS if the LHS is false
			if (left.Evaluate(context))
				if (right.Evaluate(context))
					return true;

			return false;
		}
	}
}