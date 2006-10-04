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

		private ASTReference LeftHandSide
		{
			get { return (ASTReference) GetChild(0).GetChild(0).GetChild(0); }
		}

		private INode RightHandSide
		{
			get { return GetChild(0).GetChild(0).GetChild(1).GetChild(0); }
		}

		/// <summary>Accept the visitor. *
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		/// <summary>  simple init.  We can get the RHS and LHS as the the tree structure is static
		/// </summary>
		public override Object Init(IInternalContextAdapter context, Object data)
		{
			/*
	    *  init the tree correctly
	    */

			base.Init(context, data);

			right = RightHandSide;
			left = LeftHandSide;

			blather = rsvc.GetBoolean(RuntimeConstants.RUNTIME_LOG_REFERENCE_LOG_INVALID, true);

			/*
	    *  grab this now.  No need to redo each time
	    */
			leftReference = left.FirstToken.Image.Substring(1);

			return data;
		}

		/// <summary>   puts the value of the RHS into the context under the key of the LHS
		/// </summary>
		public override bool Render(IInternalContextAdapter context, TextWriter writer)
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
						doit = ec.ShouldLogOnNullSet(left.Literal, right.Literal);
					}

					if (doit)
					{
						rsvc.Error("RHS of #set statement is null. Context will not be modified. " + context.CurrentTemplateName + " [line " + Line + ", column " + Column + "]");
					}
				}

				return false;
			}

			/*
	    *  if the LHS is simple, just punch the value into the context
	    *  otherwise, use the setValue() method do to it.
	    *  Maybe we should always use setValue()
	    */

			if (left.ChildrenCount == 0)
			{
				context.Put(leftReference, value_);
			}
			else
			{
				left.SetValue(context, value_);
			}

			return true;
		}
	}
}
