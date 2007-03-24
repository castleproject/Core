namespace antlr.debug
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
	using Stack = System.Collections.Stack;
	using antlr;
	using BitSet = antlr.collections.impl.BitSet;

	/// <summary>
	/// Specifies the behaviour required (i.e. parser modifications) 
	/// specifically to support parse tree debugging and derivation.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Override the standard matching and rule entry/exit routines
	/// to build parse trees.  This class is useful for 2.7.3 where
	/// you can specify a superclass like
	/// </para>
	/// <para>
	/// class TinyCParser extends Parser(ParseTreeDebugParser);
	/// </para>
	/// </remarks>
	public class ParseTreeDebugParser : LLkParser 
	{
		/// <summary>
		/// Each new rule invocation must have it's own subtree. Tokens are
		/// added to the current root so we must have a stack of subtree roots.
		/// </summary>
		protected Stack currentParseTreeRoot = new Stack();

		/// <summary>
		/// Track most recently created parse subtree so that when parsing
		/// is finished, we can get to the root.
		/// </summary>
		protected ParseTreeRule mostRecentParseTreeRoot = null;

		/// <summary>
		/// For every rule replacement with a production, we bump up count.
		/// </summary>
		protected int numberOfDerivationSteps = 1; // n replacements plus step 0

		public ParseTreeDebugParser(int k_) : base(k_)
		{
		}

		public ParseTreeDebugParser(ParserSharedInputState state, int k_) : base(state, k_)
		{
		}

		public ParseTreeDebugParser(TokenBuffer tokenBuf, int k_) : base(tokenBuf, k_)
		{
		}

		public ParseTreeDebugParser(TokenStream lexer, int k_) : base(lexer,k_)
		{
		}

		public ParseTree getParseTree() 
		{
			return mostRecentParseTreeRoot;
		}

		public int getNumberOfDerivationSteps() 
		{
			return numberOfDerivationSteps;
		}

		public override void match(int i) 			// throws MismatchedTokenException, TokenStreamException 
		{
			addCurrentTokenToParseTree();
			base.match(i);
		}

		public override void match(BitSet bitSet) 	// throws MismatchedTokenException, TokenStreamException 
		{
			addCurrentTokenToParseTree();
			base.match(bitSet);
		}

		public override void matchNot(int i) 		// throws MismatchedTokenException, TokenStreamException
		{
			addCurrentTokenToParseTree();
			base.matchNot(i);
		}

		/// <summary>
		/// Adds LT(1) to the current parse subtree.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Note that the match() routines add the node before checking for 
		/// correct match.  This means that, upon mismatched token, there 
		/// will a token node in the tree corresponding to where that token 
		/// was expected.  For no viable alternative errors, no node will 
		/// be in the tree as nothing was matched() (the lookahead failed 
		/// to predict an alternative).
		/// </para>
		/// </remarks>
		protected void addCurrentTokenToParseTree() 		// throws TokenStreamException 
		{
			if (inputState.guessing > 0) 
			{
				return;
			}
			ParseTreeRule root = (ParseTreeRule) currentParseTreeRoot.Peek();
			ParseTreeToken tokenNode = null;
			if ( LA(1) == Token.EOF_TYPE ) 
			{
				tokenNode = new ParseTreeToken(new antlr.CommonToken("EOF"));
			}
			else 
			{
				tokenNode = new ParseTreeToken(LT(1));
			}
			root.addChild(tokenNode);
		}

		/// <summary>
		/// Create a rule node, add to current tree, and make it current root
		/// </summary>
		/// <param name="s"></param>
		public override void traceIn(string s) 				// throws TokenStreamException 
		{
			if (inputState.guessing > 0) 
			{
				return;
			}
			ParseTreeRule subRoot = new ParseTreeRule(s);
			if ( currentParseTreeRoot.Count > 0 ) 
			{
				ParseTreeRule oldRoot = (ParseTreeRule) currentParseTreeRoot.Peek();
				oldRoot.addChild(subRoot);
			}
			currentParseTreeRoot.Push(subRoot);
			numberOfDerivationSteps++;
		}

		/// <summary>
		/// Pop current root; back to adding to old root
		/// </summary>
		/// <param name="s"></param>
		public override void traceOut(string s) 				// throws TokenStreamException
		{
			if (inputState.guessing > 0) 
			{
				return;
			}
			mostRecentParseTreeRoot = (ParseTreeRule) currentParseTreeRoot.Pop();
		}
	}
}
