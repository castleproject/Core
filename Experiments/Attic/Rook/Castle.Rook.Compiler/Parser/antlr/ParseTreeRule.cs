namespace antlr
{

	/* ANTLR Translator Generator
	 * Project led by Terence Parr at http://www.jGuru.com
	 * Software rights: http://www.antlr.org/license.html
	 */

	//
	// ANTLR C# Code Generator by Micheal Jordan
	//                            Kunle Odutola       : kunle UNDERSCORE odutola AT hotmail DOT com
	//                            Anthony Oguntimehin
	//

	using System;
	using StringBuilder 	= System.Text.StringBuilder;
	using AST 				= antlr.collections.AST;

	public class ParseTreeRule : ParseTree 
	{
		public const int INVALID_ALT = -1;

		protected string ruleName;
		protected int altNumber;  // unused until I modify antlr to record this

		public ParseTreeRule(string ruleName) : this(ruleName, INVALID_ALT)
		{
		}

		public ParseTreeRule(string ruleName, int altNumber) 
		{
			this.ruleName  = ruleName;
			this.altNumber = altNumber;
		}

		public string getRuleName() 
		{
			return ruleName;
		}

		/// <summary>
		/// Do a step-first walk, building up a buffer of tokens until
		/// you've reached a particular step and print out any rule subroots
		/// insteads of descending.
		/// </summary>
		/// <param name="buf">derivation buffer</param>
		/// <param name="step">derivation steps</param>
		/// <returns></returns>
		protected internal override int getLeftmostDerivation(StringBuilder buf, int step) 
		{
			int numReplacements = 0;
			if ( step <= 0 ) 
			{
				buf.Append(' ');
				buf.Append(ToString());
				return numReplacements;
			}
			AST child = getFirstChild();
			numReplacements = 1;
			// walk child printing them out, descending into at most one
			while ( child != null ) 
			{
				if ( (numReplacements >= step) || (child is ParseTreeToken) )
				{
					buf.Append(' ');
					buf.Append(child.ToString());
				}
				else 
				{
					// descend for at least one more derivation; update count
					int remainingReplacements = step - numReplacements;
					int n = ((ParseTree) child).getLeftmostDerivation(buf, remainingReplacements);
					numReplacements += n;
				}
				child = child.getNextSibling();
			}
			return numReplacements;
		}

		public override string ToString()
		{
			if ( altNumber == INVALID_ALT ) 
			{
				return '<'+ruleName+'>';
			}
			else 
			{
				return '<'+ruleName+"["+altNumber+"]>";
			}
		}
	}
}
