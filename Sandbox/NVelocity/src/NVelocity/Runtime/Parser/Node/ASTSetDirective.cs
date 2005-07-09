namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.IO;
	using NVelocity.App.Events;
	using NVelocity.Context;

	/// <summary> Node for the #set directive
	/// *
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
	/// </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: ASTSetDirective.cs,v 1.4 2003/10/27 13:54:10 corts Exp $
	///
	/// </version>
	public class ASTSetDirective : SimpleNode
	{
		private ASTReference LeftHandSide
		{
			get { return (ASTReference) jjtGetChild(0).jjtGetChild(0).jjtGetChild(0); }

		}

		private INode RightHandSide
		{
			get { return jjtGetChild(0).jjtGetChild(0).jjtGetChild(1).jjtGetChild(0); }

		}

		private String leftReference = "";
		private INode right;
		private ASTReference left;
		internal bool blather = false;

		public ASTSetDirective(int id) : base(id)
		{
		}

		public ASTSetDirective(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>Accept the visitor. *
		/// </summary>
		public override Object jjtAccept(ParserVisitor visitor, Object data)
		{
			return visitor.visit(this, data);
		}

		/// <summary>  simple init.  We can get the RHS and LHS as the the tree structure is static
		/// </summary>
		public override Object init(InternalContextAdapter context, Object data)
		{
			/*
	    *  init the tree correctly
	    */

			base.init(context, data);

			right = RightHandSide;
			left = LeftHandSide;

			blather = rsvc.getBoolean(RuntimeConstants_Fields.RUNTIME_LOG_REFERENCE_LOG_INVALID, true);

			/*
	    *  grab this now.  No need to redo each time
	    */
			leftReference = left.FirstToken.image.Substring(1);

			return data;
		}

		/// <summary>   puts the value of the RHS into the context under the key of the LHS
		/// </summary>
		public override bool render(InternalContextAdapter context, TextWriter writer)
		{
			/*
	    *  get the RHS node, and it's value
	    */

			Object value_ = right.Value(context);

			/*
	    * it's an error if we don't have a value of some sort
	    */

			if (value_ == null)
			{
				/*
				*  first, are we supposed to say anything anyway?
				*/
				if (blather)
				{
					EventCartridge ec = context.EventCartridge;

					bool doit = true;

					/*
		    *  if we have an EventCartridge...
		    */
					if (ec != null)
					{
						doit = ec.shouldLogOnNullSet(left.literal(), right.literal());
					}

					if (doit)
					{
						rsvc.error("RHS of #set statement is null. Context will not be modified. " + context.CurrentTemplateName + " [line " + Line + ", column " + Column + "]");
					}
				}

				return false;
			}

			/*
	    *  if the LHS is simple, just punch the value into the context
	    *  otherwise, use the setValue() method do to it.
	    *  Maybe we should always use setValue()
	    */

			if (left.jjtGetNumChildren() == 0)
			{
				context.Put(leftReference, value_);
			}
			else
			{
				left.setValue(context, value_);
			}

			return true;
		}

		/// <summary>  returns the ASTReference that is the LHS of the set statememt
		/// </summary>

		/// <summary>  returns the RHS Node of the set statement
		/// </summary>
	}
}