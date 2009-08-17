namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using Context;

	/// <summary>
	/// Handles integer addition of nodes
	/// 
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: ASTAddNode.cs,v 1.3 2003/10/27 13:54:10 corts Exp $ </version>
	public class ASTAddNode : SimpleNode
	{
		public ASTAddNode(int id) : base(id)
		{
		}

		public ASTAddNode(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		/// <summary>
		/// Computes the sum of the two nodes.
		/// Currently only integer operations are supported.
		/// </summary>
		/// <returns>Integer object with value, or null</returns>
		public override Object Value(IInternalContextAdapter context)
		{
			// get the two addends
			Object left = GetChild(0).Value(context);
			Object right = GetChild(1).Value(context);

			// if either is null, lets log and bail
			if (left == null || right == null)
			{
				runtimeServices.Error(
					string.Format(
						"{0} side ({1}) of addition operation has null value. Operation not possible. {2} [line {3}, column {4}]",
						(left == null ? "Left" : "Right"), GetChild((left == null ? 0 : 1)).Literal, context.CurrentTemplateName, Line,
						Column));
				return null;
			}

			Type maxType = MathUtil.ToMaxType(left.GetType(), right.GetType());

			if (maxType == null)
			{
				return null;
			}

			return MathUtil.Add(maxType, left, right);

			// if not an Integer, not much we can do either
//			if (!(left is Int32) || !(right is Int32))
//			{
//				runtimeServices.Error((!(left is Int32) ? "Left" : "Right") + " side of addition operation is not a valid type. " + "Currently only integers (1,2,3...) and Integer type is supported. " + context.CurrentTemplateName + " [line " + Line + ", column " + Column + "]");
//
//				return null;
//			}
//			return ((Int32) left) + ((Int32) right);
		}
	}
}