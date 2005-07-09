namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.Collections;
	using NVelocity.Context;

	/// <summary>
	/// AST Node for creating a map / dictionary.
	/// This class was originally generated from Parset.jjt.
	/// </summary>
	/// <version>$Id: ASTMap.cs,v 1.2 2004/12/27 05:50:11 corts Exp $</version>
	public class ASTMap : SimpleNode
	{
		public ASTMap(int id) : base(id)
		{
		}

		public ASTMap(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor. 
		/// </summary>
		public override Object jjtAccept(ParserVisitor visitor, Object data)
		{
			return visitor.visit(this, data);
		}

		/// <summary>
		/// Evaluate the node.
		/// </summary>
		public override Object Value(InternalContextAdapter context)
		{
			int size = jjtGetNumChildren();

			IDictionary objectMap = new Hashtable();

			for (int i = 0; i < size; i += 2)
			{
				SimpleNode keyNode = (SimpleNode) jjtGetChild(i);
				SimpleNode valueNode = (SimpleNode) jjtGetChild(i + 1);

				Object key = (keyNode == null ? null : keyNode.Value(context));
				Object value_ = (valueNode == null ? null : valueNode.Value(context));

				objectMap.Add(key, value_);
			}

			return objectMap;
		}

	}
}