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
		public const int EOS = 11;
		public const int LESSTHAN = 12;
		public const int COMMA = 13;
		public const int STATIC_IDENTIFIER = 14;
		public const int INSTANCE_IDENTIFIER = 15;
		public const int LITERAL_public = 16;
		public const int LITERAL_private = 17;
		public const int LITERAL_protected = 18;
		public const int LITERAL_internal = 19;
		public const int IDENTIFIER = 20;
		public const int DOT = 21;
		
		
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
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
				case LITERAL_public:
				case LITERAL_private:
				case LITERAL_protected:
				case LITERAL_internal:
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
						goto _loop29_breakloop;
					}
					
				}
_loop29_breakloop:				;
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
						goto _loop19_breakloop;
					}
					
				}
_loop19_breakloop:				;
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
		
		
		try {      // for error handling
			type_fields(classNode);
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
	
	protected void type_fields(
		TypeNode type
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case STATIC_IDENTIFIER:
					case LITERAL_public:
					case LITERAL_private:
					case LITERAL_protected:
					case LITERAL_internal:
					{
						access_level();
						static_fields(type);
						break;
					}
					case INSTANCE_IDENTIFIER:
					{
						instance_fields(type);
						break;
					}
					default:
					{
						goto _loop22_breakloop;
					}
					 }
				}
_loop22_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_5_);
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
			case STATIC_IDENTIFIER:
			{
				/* currentAccessLevel = AccessLevel.Public; */
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
	
	protected void static_fields(
		TypeNode type
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
		try {      // for error handling
			id = LT(1);
			match(STATIC_IDENTIFIER);
			
					type.StaticFields.Add( new StaticFieldIdentifier(id.getText(), currentAccessLevel) );
				
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_4_);
		}
	}
	
	protected void instance_fields(
		TypeNode type
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		
		try {      // for error handling
			id = LT(1);
			match(INSTANCE_IDENTIFIER);
			
					type.InstanceFields.Add( new InstanceFieldIdentifier(id.getText(), currentAccessLevel) );
				
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_4_);
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
		@"""EOS""",
		@"""LESSTHAN""",
		@"""COMMA""",
		@"""STATIC_IDENTIFIER""",
		@"""INSTANCE_IDENTIFIER""",
		@"""public""",
		@"""private""",
		@"""protected""",
		@"""internal""",
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
		long[] data = { 1138L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { 1041520L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 3142768L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { 1033216L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { 1024L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { 16384L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	
}
}
