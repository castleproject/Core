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

	 /// <summary>
	 /// This token tracks it's own index 0..n-1 relative to the beginning 
	 /// of the stream. It is designed to work with 
	 /// <see cref="TokenStreamRewriteEngine"/> in TokenStreamRewriteEngine.cs
	 /// </summary> 
	public class TokenWithIndex : CommonToken 
	{
		/// <summary>
		/// Index into token array indicating position in input stream
		/// </summary>
	    int index;

	    public TokenWithIndex() : base()
	    {
	    }

	    public TokenWithIndex(int i, string t) : base(i, t)
	    {
	    }

		public void setIndex(int i)
		{
			index = i;
		}

		public int getIndex() 
		{
			return index;
		}

		public override string ToString()
		{
			return "["+index+":\"" + getText() + "\",<" + Type + ">,line=" + line + ",col=" + col + "]\n";
		}
	}
}