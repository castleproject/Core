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

	public abstract class ParseTree : BaseAST
	{
		/// <summary>
		/// Walk parse tree and return requested number of derivation steps.
		/// If steps less-than 0, return node text.  If steps equals 1, return derivation
		/// string at step.
		/// </summary>
		/// <param name="step">derivation steps</param>
		/// <returns></returns>
		public string getLeftmostDerivationStep(int step) 
		{
			if ( step <= 0 ) 
			{
				return ToString();
			}
			StringBuilder buf = new StringBuilder (2000);
			getLeftmostDerivation(buf, step);
			return buf.ToString();
		}

		public string getLeftmostDerivation(int maxSteps)
		{
			StringBuilder buf = new StringBuilder(2000);
			buf.Append("    " + this.ToString());
			buf.Append("\n");
			for (int d=1; d < maxSteps; d++) 
			{
				buf.Append(" =>");
				buf.Append(getLeftmostDerivationStep(d));
				buf.Append("\n");
			}
			return buf.ToString();
		}

		/// <summary>
		/// Get derivation and return how many you did (less than requested for
		/// subtree roots.
		/// </summary>
		/// <param name="buf">string buffer</param>
		/// <param name="step">derivation steps</param>
		/// <returns></returns>
		protected internal abstract int getLeftmostDerivation(StringBuilder buf, int step);

		// just satisfy BaseAST interface; unused as we manually create nodes

		public override void initialize(int i, string s) 
		{
		}
		
		public override void initialize(AST ast) 
		{
		}
		
		public override void initialize(IToken token) 
		{
		}
	}
}
