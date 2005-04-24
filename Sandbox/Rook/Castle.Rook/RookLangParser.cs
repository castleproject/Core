// $ANTLR 2.7.5 (20050128): "lang.g" -> "RookLangParser.cs"$

	using CommonAST					= antlr.CommonAST; 
	using System.Collections;
	using Castle.Rook.AST;

namespace Castle.Rook.Parse
{
	// Generate the header common to all output files.
	using System;
	
	using TokenBuffer              = antlr.TokenBuffer;
	using TokenStreamException     = antlr.TokenStreamException;
	using TokenStreamIOException   = antlr.TokenStreamIOException;
	using ANTLRException           = antlr.ANTLRException;
	using LLkParser = antlr.LLkParser;
	using Token                    = antlr.Token;
	using IToken                   = antlr.IToken;
	using TokenStream              = antlr.TokenStream;
	using RecognitionException     = antlr.RecognitionException;
	using NoViableAltException     = antlr.NoViableAltException;
	using MismatchedTokenException = antlr.MismatchedTokenException;
	using SemanticException        = antlr.SemanticException;
	using ParserSharedInputState   = antlr.ParserSharedInputState;
	using BitSet                   = antlr.collections.impl.BitSet;
	
	public 	class RookLangParser : antlr.LLkParser
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int CLASS = 4;
		public const int EOS = 5;
		public const int INTEGER_LITERAL = 6;
		public const int IDENTIFIER = 7;
		public const int DOT = 8;
		public const int SEMI = 9;
		public const int COMMA = 10;
		public const int ASSIGN = 11;
		
		
		protected void initialize()
		{
			tokenNames = tokenNames_;
		}
		
		
		protected RookLangParser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			initialize();
		}
		
		public RookLangParser(TokenBuffer tokenBuf) : this(tokenBuf,2)
		{
		}
		
		protected RookLangParser(TokenStream lexer, int k) : base(lexer,k)
		{
			initialize();
		}
		
		public RookLangParser(TokenStream lexer) : this(lexer,2)
		{
		}
		
		public RookLangParser(ParserSharedInputState state) : base(state,2)
		{
			initialize();
		}
		
	public CompilationUnitNode  compilation_unit() //throws RecognitionException, TokenStreamException
{
		CompilationUnitNode unit;
		
		
				unit = new CompilationUnitNode();
			
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==EOS))
					{
						match(EOS);
					}
					else
					{
						goto _loop3_breakloop;
					}
					
				}
_loop3_breakloop:				;
			}    // ( ... )*
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==IDENTIFIER))
					{
						statement_list();
					}
					else
					{
						goto _loop5_breakloop;
					}
					
				}
_loop5_breakloop:				;
			}    // ( ... )*
			match(Token.EOF_TYPE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_0_);
			}
			else
			{
				throw ex;
			}
		}
		return unit;
	}
	
	public void statement_list() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{ // ( ... )+
				int _cnt15=0;
				for (;;)
				{
					if ((LA(1)==IDENTIFIER) && (LA(2)==IDENTIFIER||LA(2)==DOT))
					{
						statement();
					}
					else
					{
						if (_cnt15 >= 1) { goto _loop15_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt15++;
				}
_loop15_breakloop:				;
			}    // ( ... )+
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_1_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void literal() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(INTEGER_LITERAL);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_2_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void identifier() //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
		try {      // for error handling
			id = LT(1);
			match(IDENTIFIER);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_3_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void type_name() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			namespace_or_type_name();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_4_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void namespace_or_type_name() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			simple_name();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==DOT))
					{
						match(DOT);
						simple_name();
					}
					else
					{
						goto _loop11_breakloop;
					}
					
				}
_loop11_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_4_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void simple_name() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			identifier();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_5_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void statement() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			declaration_statement();
			{
				switch ( LA(1) )
				{
				case SEMI:
				{
					match(SEMI);
					break;
				}
				case EOF:
				case IDENTIFIER:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_1_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void declaration_statement() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			local_variable_declaration();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_6_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void local_variable_declaration() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			bool synPredMatched21 = false;
			if (((LA(1)==IDENTIFIER) && (LA(2)==IDENTIFIER||LA(2)==DOT)))
			{
				int _m21 = mark();
				synPredMatched21 = true;
				inputState.guessing++;
				try {
					{
						multiple_local_variable_declarators();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched21 = false;
				}
				rewind(_m21);
				inputState.guessing--;
			}
			if ( synPredMatched21 )
			{
				multiple_local_variable_declarators();
				if (0==inputState.guessing)
				{
					
						
				}
			}
			else if ((LA(1)==IDENTIFIER) && (LA(2)==IDENTIFIER||LA(2)==DOT)) {
				local_variable_declarator();
			}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_6_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void multiple_local_variable_declarators() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			type_name();
			identifier();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						{
							if ((LA(1)==IDENTIFIER) && (LA(2)==IDENTIFIER||LA(2)==DOT))
							{
								type_name();
							}
							else if ((LA(1)==IDENTIFIER) && (tokenSet_7_.member(LA(2)))) {
							}
							else
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							
						}
						identifier();
					}
					else
					{
						goto _loop25_breakloop;
					}
					
				}
_loop25_breakloop:				;
			}    // ( ... )*
			{
				switch ( LA(1) )
				{
				case ASSIGN:
				{
					match(ASSIGN);
					expression_list();
					break;
				}
				case EOF:
				case IDENTIFIER:
				case SEMI:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_6_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void local_variable_declarator() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			type_name();
			identifier();
			{
				switch ( LA(1) )
				{
				case ASSIGN:
				{
					match(ASSIGN);
					expression();
					break;
				}
				case EOF:
				case IDENTIFIER:
				case SEMI:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_6_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void expression_list() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			expression();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						expression();
					}
					else
					{
						goto _loop34_breakloop;
					}
					
				}
_loop34_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_6_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void expression() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case INTEGER_LITERAL:
			{
				literal_expression();
				break;
			}
			case IDENTIFIER:
			{
				assign_expression();
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_2_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void literal_expression() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			literal();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_2_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void assign_expression() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			identifier();
			match(ASSIGN);
			expression();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_2_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	private void initializeFactory()
	{
	}
	
	public static readonly string[] tokenNames_ = new string[] {
		@"""<0>""",
		@"""EOF""",
		@"""<2>""",
		@"""NULL_TREE_LOOKAHEAD""",
		@"""class""",
		@"""EOS""",
		@"""INTEGER_LITERAL""",
		@"""IDENTIFIER""",
		@"""DOT""",
		@"""SEMI""",
		@"""COMMA""",
		@"""ASSIGN"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { 2L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { 130L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { 1666L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 3970L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { 128L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { 384L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { 642L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { 3714L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	
}
}
