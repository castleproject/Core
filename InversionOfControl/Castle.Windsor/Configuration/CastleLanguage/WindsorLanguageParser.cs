// $ANTLR 2.7.5 (20050128): "langparser.g" -> "WindsorLanguageParser.cs"$

    using antlr;
    using System.Text;
    using Castle.Model.Configuration;

namespace Castle.Windsor.Configuration.CastleLanguage
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
	
	public 	class WindsorLanguageParser : antlr.LLkParser
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int IN = 4;
		public const int IMPORT = 5;
		public const int EOS = 6;
		public const int NEWLINE = 7;
		public const int COLON = 8;
		public const int INDENT = 9;
		public const int DEDENT = 10;
		public const int ID = 11;
		public const int DOT = 12;
		
		
    protected StringBuilder sbuilder = new StringBuilder();

	protected LexicalInfo ToLexicalInfo(IToken token)
	{
		int line = token.getLine();
		int startColumn = token.getColumn();
		int endColumn = token.getColumn() + token.getText().Length;
		String filename = token.getFilename();
		return new LexicalInfo(filename, line, startColumn, endColumn);
	}
		
		protected void initialize()
		{
			tokenNames = tokenNames_;
		}
		
		
		protected WindsorLanguageParser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			initialize();
		}
		
		public WindsorLanguageParser(TokenBuffer tokenBuf) : this(tokenBuf,1)
		{
		}
		
		protected WindsorLanguageParser(TokenStream lexer, int k) : base(lexer,k)
		{
			initialize();
		}
		
		public WindsorLanguageParser(TokenStream lexer) : this(lexer,1)
		{
		}
		
		public WindsorLanguageParser(ParserSharedInputState state) : base(state,1)
		{
			initialize();
		}
		
	public ConfigurationDefinition  start() //throws RecognitionException, TokenStreamException
{
		ConfigurationDefinition conf;
		
		
				conf = new ConfigurationDefinition();
			
		
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
					if ((LA(1)==IMPORT))
					{
						import_directive(conf);
					}
					else
					{
						goto _loop5_breakloop;
					}
					
				}
_loop5_breakloop:				;
			}    // ( ... )*
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==ID))
					{
						nodes(conf.Root);
					}
					else
					{
						goto _loop7_breakloop;
					}
					
				}
_loop7_breakloop:				;
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
		return conf;
	}
	
	protected void import_directive(
		ConfigurationDefinition conf
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  i = null;
		
				String ns;
				String assemblyName;
				ImportDirective import = null;
			
		
		try {      // for error handling
			i = LT(1);
			match(IMPORT);
			ns=name();
			if (0==inputState.guessing)
			{
				
						import = new ImportDirective( ToLexicalInfo(i), ns );
						conf.Imports.Add(import);
					
			}
			{
				switch ( LA(1) )
				{
				case IN:
				{
					match(IN);
					assemblyName=name();
					if (0==inputState.guessing)
					{
						
							        import.AssemblyReference = new AssemblyReference( ToLexicalInfo(i), assemblyName);
							
					}
					break;
				}
				case NEWLINE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				match(NEWLINE);
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
	
	public void nodes(
		MutableConfiguration conf
	) //throws RecognitionException, TokenStreamException
{
		
		
				String i = null;
				MutableConfiguration newNode = null;
			
		
		try {      // for error handling
			i=name();
			match(COLON);
			match(NEWLINE);
			if (0==inputState.guessing)
			{
				
						newNode = new MutableConfiguration(i);
						conf.Children.Add(newNode);
					
			}
			match(INDENT);
			{    // ( ... )*
				for (;;)
				{
					bool synPredMatched14 = false;
					if (((LA(1)==ID)))
					{
						int _m14 = mark();
						synPredMatched14 = true;
						inputState.guessing++;
						try {
							{
								name();
								match(COLON);
								value();
							}
						}
						catch (RecognitionException)
						{
							synPredMatched14 = false;
						}
						rewind(_m14);
						inputState.guessing--;
					}
					if ( synPredMatched14 )
					{
						attribute(newNode);
					}
					else if ((LA(1)==ID)) {
						nodes(newNode);
					}
					else
					{
						goto _loop15_breakloop;
					}
					
				}
_loop15_breakloop:				;
			}    // ( ... )*
			match(DEDENT);
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
	
	protected String  name() //throws RecognitionException, TokenStreamException
{
		String value;
		
		IToken  id = null;
		IToken  id2 = null;
		
				value = null; sbuilder.Length = 0;
			
		
		try {      // for error handling
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
									
						sbuilder.Append(id.getText());
						value = sbuilder.ToString();
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==DOT))
					{
						match(DOT);
						id2 = LT(1);
						match(ID);
						if (0==inputState.guessing)
						{
							
										sbuilder.Append('.');
										sbuilder.Append(id2.getText());
									
						}
					}
					else
					{
						goto _loop19_breakloop;
					}
					
				}
_loop19_breakloop:				;
			}    // ( ... )*
			if (0==inputState.guessing)
			{
				
					    value = sbuilder.ToString();
					
			}
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
		return value;
	}
	
	protected String  value() //throws RecognitionException, TokenStreamException
{
		String value;
		
		IToken  id = null;
		IToken  id2 = null;
		IToken  id3 = null;
		
				value = null; sbuilder.Length = 0;
			
		
		try {      // for error handling
			id = LT(1);
			match(ID);
			if (0==inputState.guessing)
			{
									
						sbuilder.Append(id.getText());
						value = sbuilder.ToString();
					
			}
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case DOT:
					{
						{
							match(DOT);
							id2 = LT(1);
							match(ID);
							if (0==inputState.guessing)
							{
								
												sbuilder.Append('.');
												sbuilder.Append(id2.getText());
											
							}
						}
						break;
					}
					case IN:
					{
						{
							match(IN);
							id3 = LT(1);
							match(ID);
							if (0==inputState.guessing)
							{
								
												sbuilder.Append(" in ");
												sbuilder.Append(id3.getText());
											
							}
						}
						break;
					}
					default:
					{
						goto _loop24_breakloop;
					}
					 }
				}
_loop24_breakloop:				;
			}    // ( ... )*
			if (0==inputState.guessing)
			{
				
					    value = sbuilder.ToString();
					
			}
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
		return value;
	}
	
	protected void attribute(
		MutableConfiguration conf
	) //throws RecognitionException, TokenStreamException
{
		
		
				String i = null;
				String v = null;
			
		
		try {      // for error handling
			i=name();
			match(COLON);
			v=value();
			match(NEWLINE);
			if (0==inputState.guessing)
			{
				
						conf.Attributes[i] = v;
					
			}
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
	
	private void initializeFactory()
	{
	}
	
	public static readonly string[] tokenNames_ = new string[] {
		@"""<0>""",
		@"""EOF""",
		@"""<2>""",
		@"""NULL_TREE_LOOKAHEAD""",
		@"""in""",
		@"""import""",
		@"""EOS""",
		@"""NEWLINE""",
		@"""COLON""",
		@"""INDENT""",
		@"""DEDENT""",
		@"""ID""",
		@"""DOT"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { 2L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { 2082L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { 3074L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 400L, 0L};
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
		long[] data = { 3072L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	
}
}
