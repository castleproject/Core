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
		public const int INC = 15;
		public const int DEC = 16;
		public const int SELF = 17;
		public const int BASE = 18;
		public const int COLON = 19;
		public const int DO = 20;
		public const int EOS = 21;
		public const int LESSTHAN = 22;
		public const int COMMA = 23;
		public const int LITERAL_public = 24;
		public const int LITERAL_private = 25;
		public const int LITERAL_protected = 26;
		public const int LITERAL_internal = 27;
		public const int IDENTIFIER = 28;
		public const int DOT = 29;
		public const int SEMI = 30;
		public const int LPAREN = 31;
		public const int RPAREN = 32;
		public const int REF = 33;
		public const int OUT = 34;
		public const int STATIC_IDENTIFIER = 35;
		public const int INSTANCE_IDENTIFIER = 36;
		public const int ASSIGN = 37;
		public const int INTEGER_LITERAL = 38;
		
		
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
		
		
				MixinDefinitionStatement mixinNode = null;
				Identifier id = null;
			
		
		try {      // for error handling
			match(MIXIN_DEF);
			id=identifier();
			if (0==inputState.guessing)
			{
				
						mixinNode = new MixinDefinitionStatement(id.Name);
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
		
		
				ClassDefinitionStatement classNode = null;
				Identifier id = null;
			
		
		try {      // for error handling
			match(CLASS_DEF);
			id=identifier();
			if (0==inputState.guessing)
			{
				
						classNode = new ClassDefinitionStatement(id.Name);
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
				Identifier qi = null;
			
		
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
	
	protected Identifier  qualified_identifier() //throws RecognitionException, TokenStreamException
{
		Identifier ident;
		
		
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
					if ((LA(1)==DOT) && (LA(2)==IDENTIFIER))
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
				
						ident = new Identifier( sb.ToString() );
					
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
		TypeDefinitionStatement type
	) //throws RecognitionException, TokenStreamException
{
		
		
				Identifier qi = null;
			
		
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
		ClassDefinitionStatement classNode
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
						class_level_supported_statements(classNode.Statements);
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
		MixinDefinitionStatement mixinNode
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
	
	protected void class_level_supported_statements(
		IList statements
	) //throws RecognitionException, TokenStreamException
{
		
		
				Statement stmt = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case IDENTIFIER:
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
				{
					stmt=field_decl_stmt();
					break;
				}
				case DEF:
				{
					stmt=method_def_stmt();
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				
						statements.Add( stmt );
					
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
	
	protected Identifier  type_name() //throws RecognitionException, TokenStreamException
{
		Identifier qi;
		
		
				qi = null;
			
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case COLON:
				{
					match(COLON);
					qi=qualified_identifier();
					break;
				}
				case END:
				case DEF:
				case SELF:
				case BASE:
				case DO:
				case COMMA:
				case LITERAL_public:
				case LITERAL_private:
				case LITERAL_protected:
				case LITERAL_internal:
				case IDENTIFIER:
				case RPAREN:
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
				case ASSIGN:
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
	
	public BlockStatement  block_statement() //throws RecognitionException, TokenStreamException
{
		BlockStatement block;
		
		
				block = new BlockStatement();
			
		
		try {      // for error handling
			match(DO);
			statement_list(block.Statements);
			match(END);
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
				case DO:
				case IDENTIFIER:
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
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
				recover(ex,tokenSet_9_);
			}
			else
			{
				throw ex;
			}
		}
		return block;
	}
	
	protected void statement_list(
		IList statements
	) //throws RecognitionException, TokenStreamException
{
		
		
				Statement stmt = null;
			
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_10_.member(LA(1))))
					{
						stmt=statement();
						if (0==inputState.guessing)
						{
							statements.Add( stmt );
						}
					}
					else
					{
						goto _loop48_breakloop;
					}
					
				}
_loop48_breakloop:				;
			}    // ( ... )*
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
	
	public MethodDefinitionStatement  method_def_stmt() //throws RecognitionException, TokenStreamException
{
		MethodDefinitionStatement method;
		
		
				String[] nameParts;
				Identifier retType = null;
				method = null;
			
		
		try {      // for error handling
			match(DEF);
			nameParts=method_name();
			if (0==inputState.guessing)
			{
				
						method = new MethodDefinitionStatement(currentAccessLevel, nameParts);
					
			}
			formal_param_list(method);
			retType=type_name();
			if (0==inputState.guessing)
			{
				
						method.ReturnType = retType;
					
			}
			method_body(method);
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
				case DO:
				case LITERAL_public:
				case LITERAL_private:
				case LITERAL_protected:
				case LITERAL_internal:
				case IDENTIFIER:
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
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
				recover(ex,tokenSet_12_);
			}
			else
			{
				throw ex;
			}
		}
		return method;
	}
	
	protected void formal_param_list(
		MethodDefinitionStatement method
	) //throws RecognitionException, TokenStreamException
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
					method_param(method);
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								method_param(method);
							}
							else
							{
								goto _loop39_breakloop;
							}
							
						}
_loop39_breakloop:						;
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
				recover(ex,tokenSet_13_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void method_body(
		MethodDefinitionStatement method
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			statement_list(method.Statements);
			match(END);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_14_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected void method_param(
		MethodDefinitionStatement method
	) //throws RecognitionException, TokenStreamException
{
		
		
				Identifier id = null;
				Identifier qi = null;
			
		
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
			if (0==inputState.guessing)
			{
				
						method.Parameters.Add( new MethodParameterNode(id.Name, qi) );
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	protected Statement  statement() //throws RecognitionException, TokenStreamException
{
		Statement stmt;
		
		
				stmt = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case DEF:
			{
				stmt=method_def_stmt();
				break;
			}
			case DO:
			{
				stmt=block_statement();
				break;
			}
			default:
				if ((tokenSet_16_.member(LA(1))) && (tokenSet_17_.member(LA(2))))
				{
					stmt=expression_statement();
				}
				else {
					bool synPredMatched45 = false;
					if (((LA(1)==IDENTIFIER||LA(1)==STATIC_IDENTIFIER||LA(1)==INSTANCE_IDENTIFIER) && (tokenSet_18_.member(LA(2)))))
					{
						int _m45 = mark();
						synPredMatched45 = true;
						inputState.guessing++;
						try {
							{
								field_decl_stmt();
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
						stmt=field_decl_stmt();
					}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				}break; }
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
			return stmt;
		}
		
	protected ExpressionStatement  expression_statement() //throws RecognitionException, TokenStreamException
{
		ExpressionStatement stmt;
		
		
				stmt = null;
				Expression exp = null;
			
		
		try {      // for error handling
			{
				bool synPredMatched57 = false;
				if (((tokenSet_16_.member(LA(1))) && (tokenSet_19_.member(LA(2)))))
				{
					int _m57 = mark();
					synPredMatched57 = true;
					inputState.guessing++;
					try {
						{
							assign_exp();
						}
					}
					catch (RecognitionException)
					{
						synPredMatched57 = false;
					}
					rewind(_m57);
					inputState.guessing--;
				}
				if ( synPredMatched57 )
				{
					exp=assign_exp();
				}
				else if ((tokenSet_16_.member(LA(1))) && (tokenSet_20_.member(LA(2)))) {
					exp=unary_exp();
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			if (0==inputState.guessing)
			{
				
						stmt = new ExpressionStatement(exp);
					
			}
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
		return stmt;
	}
	
	protected FieldDeclarationStatement  field_decl_stmt() //throws RecognitionException, TokenStreamException
{
		FieldDeclarationStatement stmt;
		
		
				stmt = null; IdentifierReferenceExpression var; 
				Expression initializer = null; Identifier tn = null;
			
		
		try {      // for error handling
			var=var_reference();
			tn=type_name();
			{
				switch ( LA(1) )
				{
				case ASSIGN:
				{
					match(ASSIGN);
					initializer=expression();
					break;
				}
				case END:
				case DEF:
				case SELF:
				case BASE:
				case DO:
				case LITERAL_public:
				case LITERAL_private:
				case LITERAL_protected:
				case LITERAL_internal:
				case IDENTIFIER:
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
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
			if (0==inputState.guessing)
			{
				
						stmt = new FieldDeclarationStatement(currentAccessLevel, var, tn, initializer);
					
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
		return stmt;
	}
	
	protected IdentifierReferenceExpression  var_reference() //throws RecognitionException, TokenStreamException
{
		IdentifierReferenceExpression exp;
		
		IToken  id = null;
		IToken  id2 = null;
		
				Identifier qi = null; exp = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case STATIC_IDENTIFIER:
			{
				id = LT(1);
				match(STATIC_IDENTIFIER);
				if (0==inputState.guessing)
				{
					exp = new StaticFieldReferenceExpression(id.getText());
				}
				break;
			}
			case INSTANCE_IDENTIFIER:
			{
				id2 = LT(1);
				match(INSTANCE_IDENTIFIER);
				if (0==inputState.guessing)
				{
					exp = new InstanceFieldReferenceExpression(id2.getText());
				}
				break;
			}
			case IDENTIFIER:
			{
				qi=qualified_identifier();
				if (0==inputState.guessing)
				{
					exp = new IdentifierReferenceExpression(qi);
				}
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
				recover(ex,tokenSet_21_);
			}
			else
			{
				throw ex;
			}
		}
		return exp;
	}
	
	protected Expression  expression() //throws RecognitionException, TokenStreamException
{
		Expression exp;
		
		exp = null;
		
		try {      // for error handling
			exp=primary_exp();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_22_);
			}
			else
			{
				throw ex;
			}
		}
		return exp;
	}
	
	protected AssignmentExpression  assign_exp() //throws RecognitionException, TokenStreamException
{
		AssignmentExpression ue;
		
		
				Expression target, value = null; ue = null;
			
		
		try {      // for error handling
			target=unary_exp();
			match(ASSIGN);
			value=expression();
			if (0==inputState.guessing)
			{
				
						ue = new AssignmentExpression(target, value);
					
			}
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
		return ue;
	}
	
	protected Expression  unary_exp() //throws RecognitionException, TokenStreamException
{
		Expression exp;
		
		exp = null;
		
		try {      // for error handling
			exp=primary_exp();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_23_);
			}
			else
			{
				throw ex;
			}
		}
		return exp;
	}
	
	protected Expression  primary_exp() //throws RecognitionException, TokenStreamException
{
		Expression exp;
		
		
				Expression ps = null; exp = null;
			
		
		try {      // for error handling
			{
				ps=primary_start();
				{    // ( ... )*
					for (;;)
					{
						switch ( LA(1) )
						{
						case INC:
						case DEC:
						{
							exp=postfix_exp(ps);
							break;
						}
						case LPAREN:
						{
							exp=method_invoke_exp(ps);
							break;
						}
						case DOT:
						{
							exp=member_access(ps);
							break;
						}
						default:
						{
							goto _loop71_breakloop;
						}
						 }
					}
_loop71_breakloop:					;
				}    // ( ... )*
			}
			if (0==inputState.guessing)
			{
				
						if (exp == null) exp = ps;
					
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
		return exp;
	}
	
	public LiteralExpression  literal_exp() //throws RecognitionException, TokenStreamException
{
		LiteralExpression le;
		
		IToken  t = null;
		le = null;
		
		try {      // for error handling
			t = LT(1);
			match(INTEGER_LITERAL);
			if (0==inputState.guessing)
			{
				
						le = new LiteralIntegerExpression( t.getText() );
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_24_);
			}
			else
			{
				throw ex;
			}
		}
		return le;
	}
	
	protected MethodInvokeExpression  method_invoke_exp(
		Expression target
	) //throws RecognitionException, TokenStreamException
{
		MethodInvokeExpression mie;
		
		
				mie = new MethodInvokeExpression(target);
				Expression exp;
			
		
		try {      // for error handling
			match(LPAREN);
			{
				switch ( LA(1) )
				{
				case SELF:
				case BASE:
				case IDENTIFIER:
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
				case INTEGER_LITERAL:
				{
					exp=expression();
					if (0==inputState.guessing)
					{
						mie.Arguments.Add( exp );
					}
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								exp=expression();
								if (0==inputState.guessing)
								{
									mie.Arguments.Add( exp );
								}
							}
							else
							{
								goto _loop65_breakloop;
							}
							
						}
_loop65_breakloop:						;
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
				switch ( LA(1) )
				{
				case SEMI:
				{
					match(SEMI);
					break;
				}
				case END:
				case DEF:
				case INC:
				case DEC:
				case SELF:
				case BASE:
				case DO:
				case COMMA:
				case LITERAL_public:
				case LITERAL_private:
				case LITERAL_protected:
				case LITERAL_internal:
				case IDENTIFIER:
				case DOT:
				case LPAREN:
				case RPAREN:
				case STATIC_IDENTIFIER:
				case INSTANCE_IDENTIFIER:
				case ASSIGN:
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
				recover(ex,tokenSet_24_);
			}
			else
			{
				throw ex;
			}
		}
		return mie;
	}
	
	protected Expression  primary_start() //throws RecognitionException, TokenStreamException
{
		Expression exp;
		
		exp = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case INTEGER_LITERAL:
			{
				exp=literal_exp();
				break;
			}
			case IDENTIFIER:
			case STATIC_IDENTIFIER:
			case INSTANCE_IDENTIFIER:
			{
				exp=var_reference();
				break;
			}
			case SELF:
			{
				match(SELF);
				if (0==inputState.guessing)
				{
					exp = SelfReferenceExpression.Instance;
				}
				break;
			}
			case BASE:
			{
				match(BASE);
				if (0==inputState.guessing)
				{
					exp = BaseReferenceExpression.Instance;
				}
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
				recover(ex,tokenSet_24_);
			}
			else
			{
				throw ex;
			}
		}
		return exp;
	}
	
	protected PostFixExpression  postfix_exp(
		Expression target
	) //throws RecognitionException, TokenStreamException
{
		PostFixExpression pfe;
		
		
				pfe = null;
			
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case INC:
			{
				match(INC);
				if (0==inputState.guessing)
				{
					pfe = new PostFixExpression( target, 1 );
				}
				break;
			}
			case DEC:
			{
				match(DEC);
				if (0==inputState.guessing)
				{
					pfe = new PostFixExpression( target, 2 );
				}
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
				recover(ex,tokenSet_24_);
			}
			else
			{
				throw ex;
			}
		}
		return pfe;
	}
	
	protected MemberAccessExpression  member_access(
		Expression target
	) //throws RecognitionException, TokenStreamException
{
		MemberAccessExpression mae;
		
		
				mae = null;
				Identifier id;
			
		
		try {      // for error handling
			match(DOT);
			id=identifier();
			if (0==inputState.guessing)
			{
				
						mae = new MemberAccessExpression(target, id);
					
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_24_);
			}
			else
			{
				throw ex;
			}
		}
		return mae;
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
		@"""++""",
		@"""--""",
		@"""self""",
		@"""base""",
		@""":""",
		@"""do""",
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
		long[] data = { 522905947248L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 522910141552L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { 103599311872L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { 103599310848L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { 103347652608L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { 2147483648L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { 520220969984L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { 378227002368L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { 378227001344L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { 1024L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { 378478660608L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	private static long[] mk_tokenSet_13_()
	{
		long[] data = { 378227526656L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
	private static long[] mk_tokenSet_14_()
	{
		long[] data = { 379552402432L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
	private static long[] mk_tokenSet_15_()
	{
		long[] data = { 4303355904L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
	private static long[] mk_tokenSet_16_()
	{
		long[] data = { 378225950720L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_16_ = new BitSet(mk_tokenSet_16_());
	private static long[] mk_tokenSet_17_()
	{
		long[] data = { 518350408704L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_17_ = new BitSet(mk_tokenSet_17_());
	private static long[] mk_tokenSet_18_()
	{
		long[] data = { 516203351040L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_18_ = new BitSet(mk_tokenSet_18_());
	private static long[] mk_tokenSet_19_()
	{
		long[] data = { 140123406336L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_19_ = new BitSet(mk_tokenSet_19_());
	private static long[] mk_tokenSet_20_()
	{
		long[] data = { 380911455232L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_20_ = new BitSet(mk_tokenSet_20_());
	private static long[] mk_tokenSet_21_()
	{
		long[] data = { 522905947136L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_21_ = new BitSet(mk_tokenSet_21_());
	private static long[] mk_tokenSet_22_()
	{
		long[] data = { 382782016512L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_22_ = new BitSet(mk_tokenSet_22_());
	private static long[] mk_tokenSet_23_()
	{
		long[] data = { 515665955840L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_23_ = new BitSet(mk_tokenSet_23_());
	private static long[] mk_tokenSet_24_()
	{
		long[] data = { 522905422848L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_24_ = new BitSet(mk_tokenSet_24_());
	
}
}
