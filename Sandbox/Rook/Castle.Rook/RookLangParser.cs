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
		public const int INC = 16;
		public const int DEC = 17;
		public const int SELF = 18;
		public const int BASE = 19;
		public const int EOS = 20;
		public const int LESSTHAN = 21;
		public const int COMMA = 22;
		public const int LITERAL_public = 23;
		public const int LITERAL_private = 24;
		public const int LITERAL_protected = 25;
		public const int LITERAL_internal = 26;
		public const int IDENTIFIER = 27;
		public const int DOT = 28;
		public const int SEMI = 29;
		public const int LPAREN = 30;
		public const int RPAREN = 31;
		public const int REF = 32;
		public const int OUT = 33;
		public const int STATIC_IDENTIFIER = 34;
		public const int INSTANCE_IDENTIFIER = 35;
		public const int ASSIGN = 36;
		public const int INTEGER_LITERAL = 37;
		
		
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
	
	public void mixin_declaration(
		IList mixins
	) //throws RecognitionException, TokenStreamException
{
		
		
				MixinNode mixinNode = null;
				Identifier id = null;
			
		
		try {      // for error handling
			match(MIXIN_DEF);
			id=identifier();
			if (0==inputState.guessing)
			{
				
						mixinNode = new MixinNode(id.Name);
						mixins.Add(mixinNode);
					
			}
			mixin_body(mixinNode);
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
	
	public void class_declaration(
		IList types
	) //throws RecognitionException, TokenStreamException
{
		
		
				ClassNode classNode = null;
				Identifier id = null;
			
		
		try {      // for error handling
			match(CLASS_DEF);
			id=identifier();
			if (0==inputState.guessing)
			{
				
						classNode = new ClassNode(id.Name);
						types.Add(classNode);
					
			}
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
				case SELF:
				case BASE:
				case LITERAL_public:
				case LITERAL_private:
				case LITERAL_protected:
				case LITERAL_internal:
				case IDENTIFIER:
				case INTEGER_LITERAL:
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
	
	public void namespace_member_declaration(
		IList namespaces
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			namespace_declaration(namespaces);
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
	
	public void namespace_declaration(
		IList namespaces
	) //throws RecognitionException, TokenStreamException
{
		
		
				NamespaceNode ns = null;
				QualifiedIdentifier qi = null;
			
		
		try {      // for error handling
			match(NAMESPACE);
			qi=qualified_identifier();
			if (0==inputState.guessing)
			{
				
						ns = new NamespaceNode( qi );
					
			}
			namespace_body(ns);
			if (0==inputState.guessing)
			{
				
						namespaces.Add(ns);
					
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
	
	protected QualifiedIdentifier  qualified_identifier() //throws RecognitionException, TokenStreamException
{
		QualifiedIdentifier ident;
		
		
				ident = null;
				StringBuilder sb = new StringBuilder();
				Identifier id = null;
			
		
		try {      // for error handling
			id=identifier();
			if (0==inputState.guessing)
			{
				
						sb.Append(id.Name);
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==DOT))
					{
						match(DOT);
						id=identifier();
						if (0==inputState.guessing)
						{
							
									sb.Append('.');
									sb.Append(id.Name);
								
						}
					}
					else
					{
						goto _loop26_breakloop;
					}
					
				}
_loop26_breakloop:				;
			}    // ( ... )*
			if (0==inputState.guessing)
			{
				
						ident = new QualifiedIdentifier( sb.ToString() );
					
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
	
	protected Identifier  identifier() //throws RecognitionException, TokenStreamException
{
		Identifier ident;
		
		IToken  id = null;
		
				ident = null;
			
		
		try {      // for error handling
			id = LT(1);
			match(IDENTIFIER);
			if (0==inputState.guessing)
			{
				ident = new Identifier(id.getText());
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
			if (0==inputState.guessing)
			{
				
						type.BaseTypes.Add( qi );
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						qi=qualified_identifier();
						if (0==inputState.guessing)
						{
							
									type.BaseTypes.Add( qi );
								
						}
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
	
	public void mixin_body(
		MixinNode mixinNode
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(END);
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
	
	protected void access_level() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case LITERAL_public:
			{
				match(LITERAL_public);
				if (0==inputState.guessing)
				{
					currentAccessLevel = AccessLevel.Public;
				}
				break;
			}
			case LITERAL_private:
			{
				match(LITERAL_private);
				if (0==inputState.guessing)
				{
					currentAccessLevel = AccessLevel.Private;
				}
				break;
			}
			case LITERAL_protected:
			{
				match(LITERAL_protected);
				if (0==inputState.guessing)
				{
					currentAccessLevel = AccessLevel.Protected;
				}
				break;
			}
			case LITERAL_internal:
			{
				match(LITERAL_internal);
				if (0==inputState.guessing)
				{
					currentAccessLevel = AccessLevel.Internal;
				}
				break;
			}
			case DEF:
			case SELF:
			case BASE:
			case IDENTIFIER:
			case INTEGER_LITERAL:
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
	
	protected void class_level_supported_statements() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case SELF:
			case BASE:
			case IDENTIFIER:
			case INTEGER_LITERAL:
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
	
	protected String[]  method_name() //throws RecognitionException, TokenStreamException
{
		String[] parts;
		
		
				parts = new String[2];
				Identifier id = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case IDENTIFIER:
				{
					id=identifier();
					if (0==inputState.guessing)
					{
						
									parts[0] = id.Name;
								
					}
					break;
				}
				case SELF:
				{
					match(SELF);
					if (0==inputState.guessing)
					{
						
									parts[0] = "self";
								
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				switch ( LA(1) )
				{
				case DOT:
				{
					match(DOT);
					id=identifier();
					if (0==inputState.guessing)
					{
						
								parts[1] = id.Name;
							
					}
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
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_7_);
			}
			else
			{
				throw ex;
			}
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
					if (0==inputState.guessing)
					{
						
								qi = new QualifiedIdentifier( id.Name );
							
					}
					break;
				}
				case END:
				case DEF:
				case SELF:
				case BASE:
				case COMMA:
				case IDENTIFIER:
				case RPAREN:
				case INTEGER_LITERAL:
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
				recover(ex,tokenSet_8_);
			}
			else
			{
				throw ex;
			}
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
			if (0==inputState.guessing)
			{
				
						MethodNodeBuilder.Build(nameParts);
					
			}
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
				case SELF:
				case BASE:
				case LITERAL_public:
				case LITERAL_private:
				case LITERAL_protected:
				case LITERAL_internal:
				case IDENTIFIER:
				case INTEGER_LITERAL:
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
				recover(ex,tokenSet_4_);
			}
			else
			{
				throw ex;
			}
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
								goto _loop37_breakloop;
							}
							
						}
_loop37_breakloop:						;
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
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_9_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void method_body() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_6_.member(LA(1))))
					{
						statement_list();
					}
					else
					{
						goto _loop42_breakloop;
					}
					
				}
_loop42_breakloop:				;
			}    // ( ... )*
			match(END);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_10_);
			}
			else
			{
				throw ex;
			}
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
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_11_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void statement_list() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			statement();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_12_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void statement() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			bool synPredMatched45 = false;
			if (((tokenSet_13_.member(LA(1))) && (tokenSet_14_.member(LA(2)))))
			{
				int _m45 = mark();
				synPredMatched45 = true;
				inputState.guessing++;
				try {
					{
						assign_stmt();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched45 = false;
				}
				rewind(_m45);
				inputState.guessing--;
			}
			if ( synPredMatched45 )
			{
				assign_stmt();
			}
			else if ((LA(1)==DEF)) {
				method_def_stmt();
			}
			else if ((tokenSet_13_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
				expression_statement();
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
				recover(ex,tokenSet_12_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void assign_stmt() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			unary_exp();
			match(ASSIGN);
			expression();
			if (0==inputState.guessing)
			{
				
					
			}
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
				case SELF:
				case BASE:
				case LITERAL_public:
				case LITERAL_private:
				case LITERAL_protected:
				case LITERAL_internal:
				case IDENTIFIER:
				case INTEGER_LITERAL:
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
				recover(ex,tokenSet_4_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void expression_statement() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			unary_exp();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_12_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void var_reference() //throws RecognitionException, TokenStreamException
{
		
		IToken  id = null;
		IToken  id2 = null;
		
				QualifiedIdentifier qi = null;
			
		
		try {      // for error handling
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
	}
	
	protected void unary_exp() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			primary_exp();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_16_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void expression() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			primary_exp();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_17_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void primary_exp() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			primary_start();
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case INC:
					case DEC:
					{
						postfix_exp();
						break;
					}
					case LPAREN:
					{
						method_invoke_exp();
						break;
					}
					case DOT:
					{
						member_access();
						break;
					}
					default:
					{
						goto _loop63_breakloop;
					}
					 }
				}
_loop63_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_18_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void literal_exp() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(INTEGER_LITERAL);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_19_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void method_invoke_exp() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(LPAREN);
			{
				switch ( LA(1) )
				{
				case SELF:
				case BASE:
				case IDENTIFIER:
				case INTEGER_LITERAL:
				{
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
								goto _loop58_breakloop;
							}
							
						}
_loop58_breakloop:						;
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
			{
				if ((LA(1)==SEMI) && (tokenSet_19_.member(LA(2))))
				{
					match(SEMI);
				}
				else if ((tokenSet_19_.member(LA(1))) && (tokenSet_20_.member(LA(2)))) {
				}
				else
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
				recover(ex,tokenSet_19_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void primary_start() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case INTEGER_LITERAL:
			{
				literal_exp();
				break;
			}
			case IDENTIFIER:
			{
				identifier();
				break;
			}
			case SELF:
			{
				match(SELF);
				break;
			}
			case BASE:
			{
				match(BASE);
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
				recover(ex,tokenSet_19_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void postfix_exp() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case INC:
			{
				match(INC);
				break;
			}
			case DEC:
			{
				match(DEC);
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
				recover(ex,tokenSet_19_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void member_access() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(DOT);
			identifier();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_19_);
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
		@"""++""",
		@"""--""",
		@"""self""",
		@"""base""",
		@"""EOS""",
		@"""LESSTHAN""",
		@"""COMMA""",
		@"""public""",
		@"""private""",
		@"""protected""",
		@"""internal""",
		@"""an identifier""",
		@"""DOT""",
		@"""SEMI""",
		@"""LPAREN""",
		@"""RPAREN""",
		@"""REF""",
		@"""OUT""",
		@"""an static variable name""",
		@"""an instance variable name""",
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
		long[] data = { 137703984242L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 210452319346L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { 137699789824L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { 137699788800L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { 137573959680L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { 1073741824L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { 139725638656L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { 137573993472L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { 138236660736L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { 2151677952L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { 137573960704L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	private static long[] mk_tokenSet_13_()
	{
		long[] data = { 137573957632L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
	private static long[] mk_tokenSet_14_()
	{
		long[] data = { 70061850624L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
	private static long[] mk_tokenSet_15_()
	{
		long[] data = { 138916334592L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
	private static long[] mk_tokenSet_16_()
	{
		long[] data = { 206293437440L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_16_ = new BitSet(mk_tokenSet_16_());
	private static long[] mk_tokenSet_17_()
	{
		long[] data = { 140388338688L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_17_ = new BitSet(mk_tokenSet_17_());
	private static long[] mk_tokenSet_18_()
	{
		long[] data = { 209107815424L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_18_ = new BitSet(mk_tokenSet_18_());
	private static long[] mk_tokenSet_19_()
	{
		long[] data = { 210450189312L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_19_ = new BitSet(mk_tokenSet_19_());
	private static long[] mk_tokenSet_20_()
	{
		long[] data = { 210450189426L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_20_ = new BitSet(mk_tokenSet_20_());
	
}
}
