// $ANTLR 2.7.5 (20050128): "lang.g" -> "RookLangParser.cs"$

	// using CommonAST					= antlr.CommonAST; 
	using System.Text;
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
		public const int CLASS_DEF = 4;
		public const int MIXIN_DEF = 5;
		public const int NAMESPACE = 6;
		public const int END = 7;
		public const int EOS = 8;
		public const int IDENTIFIER = 9;
		public const int DOT = 10;
		
		
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
					if ((LA(1)==NAMESPACE))
					{
						namespace_member_declaration(unit.Namespaces);
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
			reportError(ex);
			recover(ex,tokenSet_0_);
		}
		return unit;
	}
	
	public void namespace_member_declaration(
		IList namespaces
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			namespace_declaration(namespaces);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void namespace_declaration(
		IList namespaces
	) //throws RecognitionException, TokenStreamException
{
		
		
				NamespaceNode ns = null;
				QualifiedIdentifier qi = null;
			
		
		try {      // for error handling
			match(NAMESPACE);
			qi=qualified_identifier();
			
					ns = new NamespaceNode( qi );
				
			namespace_body(ns);
			
					namespaces.Add(ns);
				
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	protected QualifiedIdentifier  qualified_identifier() //throws RecognitionException, TokenStreamException
{
		QualifiedIdentifier ident;
		
		
				ident = null;
				StringBuilder sb = new StringBuilder();
				Identifier id = null;
			
		
		try {      // for error handling
			id=identifier();
			
					sb.Append(id.Name);
				
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==DOT))
					{
						match(DOT);
						id=identifier();
						
								sb.Append('.');
								sb.Append(id.Name);
							
					}
					else
					{
						goto _loop11_breakloop;
					}
					
				}
_loop11_breakloop:				;
			}    // ( ... )*
			
					ident = new QualifiedIdentifier( sb.ToString() );
				
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_2_);
		}
		return ident;
	}
	
	public void namespace_body(
		NamespaceNode ns
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==NAMESPACE))
					{
						namespace_member_declaration(ns.Namespaces);
					}
					else
					{
						goto _loop14_breakloop;
					}
					
				}
_loop14_breakloop:				;
			}    // ( ... )*
			match(END);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	protected Identifier  identifier() //throws RecognitionException, TokenStreamException
{
		Identifier ident;
		
		IToken  id = null;
		
				ident = null;
			
		
		try {      // for error handling
			id = LT(1);
			match(IDENTIFIER);
			ident = new Identifier(id.getText());
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_3_);
		}
		return ident;
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
		@"""mixin""",
		@"""namespace""",
		@"""end""",
		@"""EOS""",
		@"""IDENTIFIER""",
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
		long[] data = { 194L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { 192L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 1216L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	
}
}
