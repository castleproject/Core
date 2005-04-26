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
		public const int INTERFACE = 7;
		public const int INIT = 8;
		public const int INIT2 = 9;
		public const int END = 10;
		public const int DEF = 11;
		public const int ATTR = 12;
		public const int GET = 13;
		public const int SET = 14;
		public const int AS = 15;
		public const int EOS = 16;
		public const int LESSTHAN = 17;
		public const int COMMA = 18;
		public const int LITERAL_public = 19;
		public const int LITERAL_private = 20;
		public const int LITERAL_protected = 21;
		public const int LITERAL_internal = 22;
		public const int IDENTIFIER = 23;
		public const int DOT = 24;
		public const int SEMI = 25;
		public const int LPAREN = 26;
		public const int RPAREN = 27;
		public const int REF = 28;
		public const int OUT = 29;
		public const int STATIC_IDENTIFIER = 30;
		public const int INSTANCE_IDENTIFIER = 31;
		public const int ASSIGN = 32;
		public const int INTEGER_LITERAL = 33;
		
		
	AccessLevel currentAccessLevel = AccessLevel.Public;
	
	public override void reportError(RecognitionException ex)
	{
		throw ex;
	}
		
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
					if (((LA(1) >= CLASS_DEF && LA(1) <= NAMESPACE)))
					{
						declaration(unit);
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
	
	public void declaration(
		AbstractDeclarationContainer container
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case MIXIN_DEF:
			{
				mixin_declaration(container.MixinTypes);
				break;
			}
			case CLASS_DEF:
			{
				class_declaration(container.ClassesTypes);
				break;
			}
			case NAMESPACE:
			{
				namespace_member_declaration(container.Namespaces);
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
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void mixin_declaration(
		IList mixins
	) //throws RecognitionException, TokenStreamException
{
		
		
				MixinNode mixinNode = null;
				Identifier id = null;
			
		
		try {      // for error handling
			match(MIXIN_DEF);
			id=identifier();
			
					mixinNode = new MixinNode(id.Name);
					mixins.Add(mixinNode);
				
			mixin_body(mixinNode);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void class_declaration(
		IList types
	) //throws RecognitionException, TokenStreamException
{
		
		
				ClassNode classNode = null;
				Identifier id = null;
			
		
		try {      // for error handling
			match(CLASS_DEF);
			id=identifier();
			
					classNode = new ClassNode(id.Name);
					types.Add(classNode);
				
			{
				switch ( LA(1) )
				{
				case LESSTHAN:
				{
					baseTypes(classNode);
					break;
				}
				case END:
				case DEF:
				case LITERAL_public:
				case LITERAL_private:
				case LITERAL_protected:
				case LITERAL_internal:
				case IDENTIFIER:
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			class_body(classNode);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
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
						goto _loop26_breakloop;
					}
					
				}
_loop26_breakloop:				;
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
					if (((LA(1) >= CLASS_DEF && LA(1) <= NAMESPACE)))
					{
						declaration(ns);
					}
					else
					{
						goto _loop11_breakloop;
					}
					
				}
_loop11_breakloop:				;
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
	
	protected void baseTypes(
		TypeNode type
	) //throws RecognitionException, TokenStreamException
{
		
		
				QualifiedIdentifier qi = null;
			
		
		try {      // for error handling
			match(LESSTHAN);
			qi=qualified_identifier();
			
					type.BaseTypes.Add( qi );
				
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						qi=qualified_identifier();
						
								type.BaseTypes.Add( qi );
							
					}
					else
					{
						goto _loop21_breakloop;
					}
					
				}
_loop21_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_4_);
		}
	}
	
	public void class_body(
		ClassNode classNode
	) //throws RecognitionException, TokenStreamException
{
		
		
				// Default access level for the method body
				currentAccessLevel = AccessLevel.Public;
			
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_5_.member(LA(1))))
					{
						access_level();
						class_level_supported_statements();
					}
					else
					{
						goto _loop18_breakloop;
					}
					
				}
_loop18_breakloop:				;
			}    // ( ... )*
			match(END);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void mixin_body(
		MixinNode mixinNode
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(END);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	protected void access_level() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case LITERAL_public:
			{
				match(LITERAL_public);
				currentAccessLevel = AccessLevel.Public;
				break;
			}
			case LITERAL_private:
			{
				match(LITERAL_private);
				currentAccessLevel = AccessLevel.Private;
				break;
			}
			case LITERAL_protected:
			{
				match(LITERAL_protected);
				currentAccessLevel = AccessLevel.Protected;
				break;
			}
			case LITERAL_internal:
			{
				match(LITERAL_internal);
				currentAccessLevel = AccessLevel.Internal;
				break;
			}
			case DEF:
			case IDENTIFIER:
			case STATIC_IDENTIFIER:
			case INSTANCE_IDENTIFIER:
			{
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
			reportError(ex);
			recover(ex,tokenSet_6_);
		}
	}
	
	protected void class_level_supported_statements() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case IDENTIFIER:
			case STATIC_IDENTIFIER:
			case INSTANCE_IDENTIFIER:
			{
				assign_stmt();
				break;
			}
			case DEF:
			{
				method_def_stmt();
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
			reportError(ex);
			recover(ex,tokenSet_4_);
		}
	}
	
	protected String[]  method_name() //throws RecognitionException, TokenStreamException
{
		String[] parts;
		
		
				parts = new String[2];
				Identifier id = null;
			
		
		try {      // for error handling
			id=identifier();
			
					parts[0] = id.Name;
				
			{
				switch ( LA(1) )
				{
				case DOT:
				{
					match(DOT);
					id=identifier();
					
							parts[1] = id.Name;
						
					break;
				}
				case LPAREN:
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
			reportError(ex);
			recover(ex,tokenSet_7_);
		}
		return parts;
	}
	
	protected QualifiedIdentifier  type_name() //throws RecognitionException, TokenStreamException
{
		QualifiedIdentifier qi;
		
		
				qi = null;
				Identifier id = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case AS:
				{
					match(AS);
					id=identifier();
					
							qi = new QualifiedIdentifier( id.Name );
						
					break;
				}
				case END:
				case DEF:
				case COMMA:
				case IDENTIFIER:
				case RPAREN:
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
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
			reportError(ex);
			recover(ex,tokenSet_8_);
		}
		return qi;
	}
	
	public void method_def_stmt() //throws RecognitionException, TokenStreamException
{
		
		
				String[] nameParts;
				QualifiedIdentifier retType = null;
			
		
		try {      // for error handling
			match(DEF);
			nameParts=method_name();
			
					MethodNodeBuilder.Build(nameParts);
				
			formal_param_list();
			retType=type_name();
			method_body();
			{
				switch ( LA(1) )
				{
				case SEMI:
				{
					match(SEMI);
					break;
				}
				case END:
				case DEF:
				case LITERAL_public:
				case LITERAL_private:
				case LITERAL_protected:
				case LITERAL_internal:
				case IDENTIFIER:
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
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
			reportError(ex);
			recover(ex,tokenSet_4_);
		}
	}
	
	protected void formal_param_list() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(LPAREN);
			{
				switch ( LA(1) )
				{
				case IDENTIFIER:
				case REF:
				case OUT:
				{
					method_param();
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								method_param();
							}
							else
							{
								goto _loop36_breakloop;
							}
							
						}
_loop36_breakloop:						;
					}    // ( ... )*
					break;
				}
				case RPAREN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_9_);
		}
	}
	
	protected void method_body() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case DEF:
				case IDENTIFIER:
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
				{
					statement_list();
					break;
				}
				case END:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(END);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_10_);
		}
	}
	
	protected void method_param() //throws RecognitionException, TokenStreamException
{
		
		
				Identifier id = null;
				QualifiedIdentifier qi = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case REF:
				{
					match(REF);
					break;
				}
				case OUT:
				{
					match(OUT);
					break;
				}
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
			id=identifier();
			qi=type_name();
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_11_);
		}
	}
	
	protected void statement_list() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			statement();
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_12_);
		}
	}
	
	protected void statement() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case IDENTIFIER:
			case STATIC_IDENTIFIER:
			case INSTANCE_IDENTIFIER:
			{
				assign_stmt();
				break;
			}
			case DEF:
			{
				method_def_stmt();
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
			reportError(ex);
			recover(ex,tokenSet_12_);
		}
	}
	
	protected void assign_stmt() //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		IToken  id2 = null;
		
				QualifiedIdentifier qi = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case STATIC_IDENTIFIER:
				{
					id = LT(1);
					match(STATIC_IDENTIFIER);
					break;
				}
				case INSTANCE_IDENTIFIER:
				{
					id2 = LT(1);
					match(INSTANCE_IDENTIFIER);
					break;
				}
				case IDENTIFIER:
				{
					qi=qualified_identifier();
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(ASSIGN);
			expression();
			
					
				
			{
				switch ( LA(1) )
				{
				case SEMI:
				{
					match(SEMI);
					break;
				}
				case END:
				case DEF:
				case LITERAL_public:
				case LITERAL_private:
				case LITERAL_protected:
				case LITERAL_internal:
				case IDENTIFIER:
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
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
			reportError(ex);
			recover(ex,tokenSet_4_);
		}
	}
	
	protected void expression() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			literal_exp();
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_10_);
		}
	}
	
	public void literal_exp() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(INTEGER_LITERAL);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_10_);
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
		@"""mixin""",
		@"""namespace""",
		@"""interface""",
		@"""initialize""",
		@"""init""",
		@"""end""",
		@"""def""",
		@"""attr""",
		@"""get""",
		@"""set""",
		@"""as""",
		@"""EOS""",
		@"""LESSTHAN""",
		@"""COMMA""",
		@"""public""",
		@"""private""",
		@"""protected""",
		@"""internal""",
		@"""IDENTIFIER""",
		@"""DOT""",
		@"""SEMI""",
		@"""LPAREN""",
		@"""RPAREN""",
		@"""REF""",
		@"""OUT""",
		@"""STATIC_IDENTIFIER""",
		@"""INSTANCE_IDENTIFIER""",
		@"""ASSIGN""",
		@"""INTEGER_LITERAL"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { 2L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { 1138L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { 7532711024L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 7750978672L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { 3237481472L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { 3237480448L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { 3229616128L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { 67108864L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { 3364097024L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { 3229649920L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { 3271035904L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { 134479872L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { 1024L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	
}
}
