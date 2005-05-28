// $ANTLR 2.7.5 (20050128): "lang4.g" -> "RookBaseParser.cs"$

	// using CommonAST					= antlr.CommonAST; 
	using System.Text;
	using System.Collections;
	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Services;

namespace Castle.Rook.Compiler.Parser
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
	
	public 	class RookBaseParser : antlr.LLkParser
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int CLASS = 4;
		public const int DO = 5;
		public const int END = 6;
		public const int DEF = 7;
		public const int OPERATOR = 8;
		public const int BEGIN = 9;
		public const int WHILE = 10;
		public const int TYPE = 11;
		public const int METHOD_CALL = 12;
		public const int SUPER_CTOR_CALL = 13;
		public const int POST_INC = 14;
		public const int POST_DEC = 15;
		public const int EXPR = 16;
		public const int ELIST = 17;
		public const int INDEX_OP = 18;
		public const int UNARY_MINUS = 19;
		public const int UNARY_PLUS = 20;
		public const int TYPECAST = 21;
		public const int ARRAY_DECLARATOR = 22;
		public const int NUM_INT = 23;
		public const int NUM_DOUBLE = 24;
		public const int NUM_FLOAT = 25;
		public const int NUM_LONG = 26;
		public const int STATEMENT_END = 27;
		public const int SEMI = 28;
		public const int LITERAL_namespace = 29;
		public const int UNTIL = 30;
		public const int LITERAL_for = 31;
		public const int LITERAL_in = 32;
		public const int LITERAL_if = 33;
		public const int LITERAL_then = 34;
		public const int LITERAL_elsif = 35;
		public const int LITERAL_else = 36;
		public const int LITERAL_unless = 37;
		public const int LITERAL_redo = 38;
		public const int LITERAL_break = 39;
		public const int LITERAL_next = 40;
		public const int LITERAL_retry = 41;
		public const int LITERAL_public = 42;
		public const int COLON = 43;
		public const int LITERAL_private = 44;
		public const int LITERAL_protected = 45;
		public const int LITERAL_internal = 46;
		public const int COMMA = 47;
		public const int ASSIGN = 48;
		public const int IDENT = 49;
		public const int LTHAN = 50;
		public const int SL = 51;
		public const int LPAREN = 52;
		public const int RPAREN = 53;
		public const int STAR = 54;
		public const int BAND = 55;
		public const int PLUS_ASSIGN = 56;
		public const int MINUS_ASSIGN = 57;
		public const int STAR_ASSIGN = 58;
		public const int DIV_ASSIGN = 59;
		public const int MOD_ASSIGN = 60;
		public const int BAND_ASSIGN = 61;
		public const int BOR_ASSIGN = 62;
		public const int BXOR_ASSIGN = 63;
		public const int LITERAL_until = 64;
		public const int STATICIDENT = 65;
		public const int INSTIDENT = 66;
		public const int DOT = 67;
		public const int SYMBOL = 68;
		public const int LITERAL_lambda = 69;
		public const int LCURLY = 70;
		public const int RCURLY = 71;
		public const int LITERAL_raise = 72;
		public const int LITERAL_yield = 73;
		public const int BOR = 74;
		public const int LITERAL_or = 75;
		public const int LITERAL_and = 76;
		public const int LITERAL_not = 77;
		public const int LNOT = 78;
		public const int GT = 79;
		public const int EQUAL = 80;
		public const int GE = 81;
		public const int LE = 82;
		public const int NOT_EQUAL = 83;
		public const int BXOR = 84;
		public const int PLUS = 85;
		public const int MINUS = 86;
		public const int SLASH = 87;
		public const int PERCENT = 88;
		public const int BNOT = 89;
		public const int LBRACK = 90;
		public const int RBRACK = 91;
		public const int STRING_LITERAL = 92;
		public const int CHAR_LITERAL = 93;
		public const int DOTDOT = 94;
		public const int DOTDOTDOT = 95;
		public const int MAPASSIGN = 96;
		public const int QUESTION = 97;
		public const int DIV = 98;
		public const int INC = 99;
		public const int DEC = 100;
		public const int MOD = 101;
		public const int SR = 102;
		public const int SR_ASSIGN = 103;
		public const int BSR = 104;
		public const int BSR_ASSIGN = 105;
		public const int SL_ASSIGN = 106;
		public const int LOR = 107;
		public const int LAND = 108;
		public const int NEWLINE = 109;
		public const int SL_NEWLINE = 110;
		public const int SL_COMMENT = 111;
		public const int WS = 112;
		public const int ESC = 113;
		public const int HEX_DIGIT = 114;
		public const int VOCAB = 115;
		public const int NUMBER = 116;
		public const int Int = 117;
		public const int NonZeroDigit = 118;
		public const int FloatTrailer = 119;
		public const int Exponent = 120;
		public const int CONTINUED_LINE = 121;
		
		
	public IErrorReport ErrorReport;

	AccessLevel currentAccessLevel = AccessLevel.Public;
	
	public override void reportError(RecognitionException ex)
	{
		LexicalPosition lpos = new LexicalPosition( ex.getLine(), ex.getColumn() );
		
		ErrorReport.Error( ex.getFilename(), lpos, ex.Message );
	}
	
	private Hashtable typeRefs = new Hashtable();
	
	private TypeReference ObtainTypeReference(String name)
	{
		if (typeRefs.ContainsKey(name))
		{
			return typeRefs[name] as TypeReference;
		}
		else
		{
			TypeReference typeRef = new TypeReference(name);
			typeRefs[name] = typeRef;
			return typeRef;
		}
	}
		
		protected void initialize()
		{
			tokenNames = tokenNames_;
		}
		
		
		protected RookBaseParser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			initialize();
		}
		
		public RookBaseParser(TokenBuffer tokenBuf) : this(tokenBuf,2)
		{
		}
		
		protected RookBaseParser(TokenStream lexer, int k) : base(lexer,k)
		{
			initialize();
		}
		
		public RookBaseParser(TokenStream lexer) : this(lexer,2)
		{
		}
		
		public RookBaseParser(ParserSharedInputState state) : base(state,2)
		{
			initialize();
		}
		
	protected void statement_term() //throws RecognitionException, TokenStreamException
{
		
		
		{
			switch ( LA(1) )
			{
			case STATEMENT_END:
			{
				match(STATEMENT_END);
				break;
			}
			case SEMI:
			{
				match(SEMI);
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
	}
	
	protected void nothing() //throws RecognitionException, TokenStreamException
{
		
		
		{
			if ((LA(1)==STATEMENT_END) && (tokenSet_0_.member(LA(2))))
			{
				match(STATEMENT_END);
			}
			else if ((tokenSet_0_.member(LA(1))) && (tokenSet_1_.member(LA(2)))) {
			}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			
		}
	}
	
	public CompilationUnit  compilationUnit() //throws RecognitionException, TokenStreamException
{
		CompilationUnit comp;
		
		comp = new CompilationUnit();
		
		nothing();
		{
			switch ( LA(1) )
			{
			case LITERAL_namespace:
			{
				namespace_declaration(comp.Namespaces);
				break;
			}
			case EOF:
			case CLASS:
			case DO:
			case DEF:
			case BEGIN:
			case WHILE:
			case NUM_INT:
			case NUM_FLOAT:
			case NUM_LONG:
			case STATEMENT_END:
			case SEMI:
			case UNTIL:
			case LITERAL_for:
			case LITERAL_if:
			case LITERAL_unless:
			case LITERAL_redo:
			case LITERAL_break:
			case LITERAL_next:
			case LITERAL_retry:
			case LITERAL_public:
			case LITERAL_private:
			case LITERAL_protected:
			case LITERAL_internal:
			case ASSIGN:
			case IDENT:
			case LTHAN:
			case LPAREN:
			case STAR:
			case BAND:
			case PLUS_ASSIGN:
			case MINUS_ASSIGN:
			case STAR_ASSIGN:
			case DIV_ASSIGN:
			case MOD_ASSIGN:
			case BAND_ASSIGN:
			case BOR_ASSIGN:
			case BXOR_ASSIGN:
			case LITERAL_until:
			case STATICIDENT:
			case INSTIDENT:
			case DOT:
			case SYMBOL:
			case LITERAL_lambda:
			case LCURLY:
			case LITERAL_raise:
			case LITERAL_yield:
			case BOR:
			case LITERAL_or:
			case LITERAL_and:
			case LITERAL_not:
			case LNOT:
			case GT:
			case EQUAL:
			case GE:
			case LE:
			case NOT_EQUAL:
			case BXOR:
			case PLUS:
			case MINUS:
			case SLASH:
			case PERCENT:
			case BNOT:
			case LBRACK:
			case STRING_LITERAL:
			case CHAR_LITERAL:
			{
				suite(comp.Statements);
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		nothing();
		match(Token.EOF_TYPE);
		return comp;
	}
	
	public String  qualified_name() //throws RecognitionException, TokenStreamException
{
		String name;
		
		IToken  t = null;
		IToken  t2 = null;
		name = null;
		
		t = LT(1);
		match(IDENT);
		if (0==inputState.guessing)
		{
			name = t.getText();
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==DOT))
				{
					match(DOT);
					t2 = LT(1);
					match(IDENT);
					if (0==inputState.guessing)
					{
						name += "." + t2.getText();
					}
				}
				else
				{
					goto _loop78_breakloop;
				}
				
			}
_loop78_breakloop:			;
		}    // ( ... )*
		return name;
	}
	
	public void namespace_declaration(
		IList namespaces
	) //throws RecognitionException, TokenStreamException
{
		
		NamespaceDeclaration nsdec = new NamespaceDeclaration(); 
			  namespaces.Add(nsdec); String qn = null;
		
		try {      // for error handling
			match(LITERAL_namespace);
			qn=qualified_name();
			statement_term();
			if (0==inputState.guessing)
			{
				nsdec.Name = qn;
			}
			suite(nsdec.Statements);
			match(END);
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
	
	public void suite(
		IList stmts
	) //throws RecognitionException, TokenStreamException
{
		
		IStatement stmt = null;
		
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_3_.member(LA(1))) && (tokenSet_4_.member(LA(2))))
				{
					stmt=statement();
					if (0==inputState.guessing)
					{
						if (stmt != null) stmts.Add(stmt);
					}
				}
				else
				{
					goto _loop12_breakloop;
				}
				
			}
_loop12_breakloop:			;
		}    // ( ... )*
	}
	
	public IStatement  statement() //throws RecognitionException, TokenStreamException
{
		IStatement stmt;
		
		stmt = null;
		
		access_level();
		{
			switch ( LA(1) )
			{
			case CLASS:
			{
				stmt=type_def_statement();
				break;
			}
			case UNTIL:
			{
				stmt=until_statement();
				break;
			}
			case LITERAL_for:
			{
				stmt=for_statement();
				break;
			}
			default:
				bool synPredMatched16 = false;
				if (((tokenSet_5_.member(LA(1))) && (LA(2)==SYMBOL)))
				{
					int _m16 = mark();
					synPredMatched16 = true;
					inputState.guessing++;
					try {
						{
							declaration_statement();
						}
					}
					catch (RecognitionException)
					{
						synPredMatched16 = false;
					}
					rewind(_m16);
					inputState.guessing--;
				}
				if ( synPredMatched16 )
				{
					stmt=declaration_statement();
				}
				else if ((LA(1)==WHILE) && (tokenSet_6_.member(LA(2)))) {
					stmt=while_statement();
				}
				else if ((LA(1)==LITERAL_if) && (tokenSet_7_.member(LA(2)))) {
					stmt=if_statement();
				}
				else {
					bool synPredMatched18 = false;
					if (((LA(1)==LITERAL_unless) && (tokenSet_7_.member(LA(2)))))
					{
						int _m18 = mark();
						synPredMatched18 = true;
						inputState.guessing++;
						try {
							{
								unless_statement();
							}
						}
						catch (RecognitionException)
						{
							synPredMatched18 = false;
						}
						rewind(_m18);
						inputState.guessing--;
					}
					if ( synPredMatched18 )
					{
						stmt=unless_statement();
					}
					else if ((tokenSet_8_.member(LA(1))) && (tokenSet_9_.member(LA(2)))) {
						stmt=expression_statement();
					}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				}break; }
			}
			statement_term();
			return stmt;
		}
		
	protected void access_level() //throws RecognitionException, TokenStreamException
{
		
		
		{
			switch ( LA(1) )
			{
			case LITERAL_public:
			{
				match(LITERAL_public);
				{
					switch ( LA(1) )
					{
					case COLON:
					{
						match(COLON);
						break;
					}
					case CLASS:
					case DO:
					case DEF:
					case BEGIN:
					case WHILE:
					case NUM_INT:
					case NUM_FLOAT:
					case NUM_LONG:
					case STATEMENT_END:
					case SEMI:
					case UNTIL:
					case LITERAL_for:
					case LITERAL_if:
					case LITERAL_unless:
					case LITERAL_redo:
					case LITERAL_break:
					case LITERAL_next:
					case LITERAL_retry:
					case ASSIGN:
					case IDENT:
					case LTHAN:
					case LPAREN:
					case STAR:
					case BAND:
					case PLUS_ASSIGN:
					case MINUS_ASSIGN:
					case STAR_ASSIGN:
					case DIV_ASSIGN:
					case MOD_ASSIGN:
					case BAND_ASSIGN:
					case BOR_ASSIGN:
					case BXOR_ASSIGN:
					case LITERAL_until:
					case STATICIDENT:
					case INSTIDENT:
					case DOT:
					case SYMBOL:
					case LITERAL_lambda:
					case LCURLY:
					case LITERAL_raise:
					case LITERAL_yield:
					case BOR:
					case LITERAL_or:
					case LITERAL_and:
					case LITERAL_not:
					case LNOT:
					case GT:
					case EQUAL:
					case GE:
					case LE:
					case NOT_EQUAL:
					case BXOR:
					case PLUS:
					case MINUS:
					case SLASH:
					case PERCENT:
					case BNOT:
					case LBRACK:
					case STRING_LITERAL:
					case CHAR_LITERAL:
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
					currentAccessLevel = AccessLevel.Public;
				}
				break;
			}
			case LITERAL_private:
			{
				match(LITERAL_private);
				{
					switch ( LA(1) )
					{
					case COLON:
					{
						match(COLON);
						break;
					}
					case CLASS:
					case DO:
					case DEF:
					case BEGIN:
					case WHILE:
					case NUM_INT:
					case NUM_FLOAT:
					case NUM_LONG:
					case STATEMENT_END:
					case SEMI:
					case UNTIL:
					case LITERAL_for:
					case LITERAL_if:
					case LITERAL_unless:
					case LITERAL_redo:
					case LITERAL_break:
					case LITERAL_next:
					case LITERAL_retry:
					case ASSIGN:
					case IDENT:
					case LTHAN:
					case LPAREN:
					case STAR:
					case BAND:
					case PLUS_ASSIGN:
					case MINUS_ASSIGN:
					case STAR_ASSIGN:
					case DIV_ASSIGN:
					case MOD_ASSIGN:
					case BAND_ASSIGN:
					case BOR_ASSIGN:
					case BXOR_ASSIGN:
					case LITERAL_until:
					case STATICIDENT:
					case INSTIDENT:
					case DOT:
					case SYMBOL:
					case LITERAL_lambda:
					case LCURLY:
					case LITERAL_raise:
					case LITERAL_yield:
					case BOR:
					case LITERAL_or:
					case LITERAL_and:
					case LITERAL_not:
					case LNOT:
					case GT:
					case EQUAL:
					case GE:
					case LE:
					case NOT_EQUAL:
					case BXOR:
					case PLUS:
					case MINUS:
					case SLASH:
					case PERCENT:
					case BNOT:
					case LBRACK:
					case STRING_LITERAL:
					case CHAR_LITERAL:
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
					currentAccessLevel = AccessLevel.Private;
				}
				break;
			}
			case LITERAL_protected:
			{
				match(LITERAL_protected);
				{
					switch ( LA(1) )
					{
					case COLON:
					{
						match(COLON);
						break;
					}
					case CLASS:
					case DO:
					case DEF:
					case BEGIN:
					case WHILE:
					case NUM_INT:
					case NUM_FLOAT:
					case NUM_LONG:
					case STATEMENT_END:
					case SEMI:
					case UNTIL:
					case LITERAL_for:
					case LITERAL_if:
					case LITERAL_unless:
					case LITERAL_redo:
					case LITERAL_break:
					case LITERAL_next:
					case LITERAL_retry:
					case ASSIGN:
					case IDENT:
					case LTHAN:
					case LPAREN:
					case STAR:
					case BAND:
					case PLUS_ASSIGN:
					case MINUS_ASSIGN:
					case STAR_ASSIGN:
					case DIV_ASSIGN:
					case MOD_ASSIGN:
					case BAND_ASSIGN:
					case BOR_ASSIGN:
					case BXOR_ASSIGN:
					case LITERAL_until:
					case STATICIDENT:
					case INSTIDENT:
					case DOT:
					case SYMBOL:
					case LITERAL_lambda:
					case LCURLY:
					case LITERAL_raise:
					case LITERAL_yield:
					case BOR:
					case LITERAL_or:
					case LITERAL_and:
					case LITERAL_not:
					case LNOT:
					case GT:
					case EQUAL:
					case GE:
					case LE:
					case NOT_EQUAL:
					case BXOR:
					case PLUS:
					case MINUS:
					case SLASH:
					case PERCENT:
					case BNOT:
					case LBRACK:
					case STRING_LITERAL:
					case CHAR_LITERAL:
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
					currentAccessLevel = AccessLevel.Protected;
				}
				break;
			}
			case LITERAL_internal:
			{
				match(LITERAL_internal);
				{
					switch ( LA(1) )
					{
					case COLON:
					{
						match(COLON);
						break;
					}
					case CLASS:
					case DO:
					case DEF:
					case BEGIN:
					case WHILE:
					case NUM_INT:
					case NUM_FLOAT:
					case NUM_LONG:
					case STATEMENT_END:
					case SEMI:
					case UNTIL:
					case LITERAL_for:
					case LITERAL_if:
					case LITERAL_unless:
					case LITERAL_redo:
					case LITERAL_break:
					case LITERAL_next:
					case LITERAL_retry:
					case ASSIGN:
					case IDENT:
					case LTHAN:
					case LPAREN:
					case STAR:
					case BAND:
					case PLUS_ASSIGN:
					case MINUS_ASSIGN:
					case STAR_ASSIGN:
					case DIV_ASSIGN:
					case MOD_ASSIGN:
					case BAND_ASSIGN:
					case BOR_ASSIGN:
					case BXOR_ASSIGN:
					case LITERAL_until:
					case STATICIDENT:
					case INSTIDENT:
					case DOT:
					case SYMBOL:
					case LITERAL_lambda:
					case LCURLY:
					case LITERAL_raise:
					case LITERAL_yield:
					case BOR:
					case LITERAL_or:
					case LITERAL_and:
					case LITERAL_not:
					case LNOT:
					case GT:
					case EQUAL:
					case GE:
					case LE:
					case NOT_EQUAL:
					case BXOR:
					case PLUS:
					case MINUS:
					case SLASH:
					case PERCENT:
					case BNOT:
					case LBRACK:
					case STRING_LITERAL:
					case CHAR_LITERAL:
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
					currentAccessLevel = AccessLevel.Internal;
				}
				break;
			}
			case CLASS:
			case DO:
			case DEF:
			case BEGIN:
			case WHILE:
			case NUM_INT:
			case NUM_FLOAT:
			case NUM_LONG:
			case STATEMENT_END:
			case SEMI:
			case UNTIL:
			case LITERAL_for:
			case LITERAL_if:
			case LITERAL_unless:
			case LITERAL_redo:
			case LITERAL_break:
			case LITERAL_next:
			case LITERAL_retry:
			case ASSIGN:
			case IDENT:
			case LTHAN:
			case LPAREN:
			case STAR:
			case BAND:
			case PLUS_ASSIGN:
			case MINUS_ASSIGN:
			case STAR_ASSIGN:
			case DIV_ASSIGN:
			case MOD_ASSIGN:
			case BAND_ASSIGN:
			case BOR_ASSIGN:
			case BXOR_ASSIGN:
			case LITERAL_until:
			case STATICIDENT:
			case INSTIDENT:
			case DOT:
			case SYMBOL:
			case LITERAL_lambda:
			case LCURLY:
			case LITERAL_raise:
			case LITERAL_yield:
			case BOR:
			case LITERAL_or:
			case LITERAL_and:
			case LITERAL_not:
			case LNOT:
			case GT:
			case EQUAL:
			case GE:
			case LE:
			case NOT_EQUAL:
			case BXOR:
			case PLUS:
			case MINUS:
			case SLASH:
			case PERCENT:
			case BNOT:
			case LBRACK:
			case STRING_LITERAL:
			case CHAR_LITERAL:
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
	
	public VariableDeclarationStatement  declaration_statement() //throws RecognitionException, TokenStreamException
{
		VariableDeclarationStatement vdstmt;
		
		vdstmt = new VariableDeclarationStatement(); TypeDeclarationExpression tdstmt = null;
			  IExpression initExp = null;
		
		tdstmt=type_name_withtype();
		if (0==inputState.guessing)
		{
			vdstmt.Add(tdstmt);
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					tdstmt=type_name_withtype();
					if (0==inputState.guessing)
					{
						vdstmt.Add(tdstmt);
					}
				}
				else
				{
					goto _loop43_breakloop;
				}
				
			}
_loop43_breakloop:			;
		}    // ( ... )*
		{
			switch ( LA(1) )
			{
			case ASSIGN:
			{
				match(ASSIGN);
				initExp=test();
				if (0==inputState.guessing)
				{
					vdstmt.AddInitExp(initExp);
				}
				{    // ( ... )*
					for (;;)
					{
						if ((LA(1)==COMMA))
						{
							match(COMMA);
							initExp=test();
							if (0==inputState.guessing)
							{
								vdstmt.AddInitExp(initExp);
							}
						}
						else
						{
							goto _loop46_breakloop;
						}
						
					}
_loop46_breakloop:					;
				}    // ( ... )*
				break;
			}
			case STATEMENT_END:
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
		return vdstmt;
	}
	
	public TypeDefinitionStatement  type_def_statement() //throws RecognitionException, TokenStreamException
{
		TypeDefinitionStatement tdstmt;
		
		tdstmt = null;
		
		tdstmt=class_def_statement();
		return tdstmt;
	}
	
	public RepeatStatement  while_statement() //throws RecognitionException, TokenStreamException
{
		RepeatStatement rs;
		
		rs = null; IExpression testexp;
		
		match(WHILE);
		testexp=test();
		{
			switch ( LA(1) )
			{
			case DO:
			{
				match(DO);
				break;
			}
			case STATEMENT_END:
			case SEMI:
			{
				statement_term();
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
			rs = new RepeatStatement(RepeatType.While, testexp);
		}
		suite(rs.Statements);
		match(END);
		return rs;
	}
	
	public RepeatStatement  until_statement() //throws RecognitionException, TokenStreamException
{
		RepeatStatement rs;
		
		rs = null; IExpression testexp;
		
		match(UNTIL);
		testexp=test();
		{
			switch ( LA(1) )
			{
			case DO:
			{
				match(DO);
				break;
			}
			case STATEMENT_END:
			case SEMI:
			{
				statement_term();
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
			rs = new RepeatStatement(RepeatType.Until, testexp);
		}
		suite(rs.Statements);
		match(END);
		return rs;
	}
	
	public ForStatement  for_statement() //throws RecognitionException, TokenStreamException
{
		ForStatement fors;
		
		fors = new ForStatement(); VariableReferenceExpression vre = null;
			  IExpression evalexp;
		
		match(LITERAL_for);
		vre=varref();
		if (0==inputState.guessing)
		{
			fors.AddVarRef(vre);
		}
		match(LITERAL_in);
		evalexp=test();
		if (0==inputState.guessing)
		{
			fors.EvalExp = evalexp;
		}
		{
			switch ( LA(1) )
			{
			case DO:
			{
				match(DO);
				break;
			}
			case STATEMENT_END:
			case SEMI:
			{
				statement_term();
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		suite(fors.Statements);
		match(END);
		return fors;
	}
	
	public IfStatement  if_statement() //throws RecognitionException, TokenStreamException
{
		IfStatement ifs;
		
		ifs = new IfStatement(IfType.If); IfStatement inner = ifs; 
			  IExpression testexp;
		
		match(LITERAL_if);
		testexp=test();
		{
			switch ( LA(1) )
			{
			case LITERAL_then:
			{
				match(LITERAL_then);
				break;
			}
			case STATEMENT_END:
			case SEMI:
			{
				statement_term();
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
			ifs.Condition = testexp;
		}
		suite(ifs.TrueStatements);
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==LITERAL_elsif))
				{
					match(LITERAL_elsif);
					testexp=test();
					{
						switch ( LA(1) )
						{
						case LITERAL_then:
						{
							match(LITERAL_then);
							break;
						}
						case STATEMENT_END:
						case SEMI:
						{
							statement_term();
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
						inner = new IfStatement(IfType.If); inner.Condition = testexp;
					}
					suite(inner.TrueStatements);
				}
				else
				{
					goto _loop29_breakloop;
				}
				
			}
_loop29_breakloop:			;
		}    // ( ... )*
		{
			switch ( LA(1) )
			{
			case LITERAL_else:
			{
				match(LITERAL_else);
				statement_term();
				suite(inner.FalseStatements);
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
		return ifs;
	}
	
	public IfStatement  unless_statement() //throws RecognitionException, TokenStreamException
{
		IfStatement ifs;
		
		ifs = new IfStatement(IfType.Unless); IExpression testexp;
		
		match(LITERAL_unless);
		testexp=test();
		{
			switch ( LA(1) )
			{
			case LITERAL_then:
			{
				match(LITERAL_then);
				break;
			}
			case STATEMENT_END:
			case SEMI:
			{
				statement_term();
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
			ifs.Condition = testexp;
		}
		suite(ifs.TrueStatements);
		{
			switch ( LA(1) )
			{
			case LITERAL_else:
			{
				match(LITERAL_else);
				statement_term();
				suite(ifs.FalseStatements);
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
		return ifs;
	}
	
	public IStatement  expression_statement() //throws RecognitionException, TokenStreamException
{
		IStatement stmt;
		
		stmt = null; PostfixCondition pfc = null; IExpression exp = null; IExpression rhs = null;
			  AugType rel = AugType.Undefined;
		
		switch ( LA(1) )
		{
		case DO:
		case BEGIN:
		case WHILE:
		case NUM_INT:
		case NUM_FLOAT:
		case NUM_LONG:
		case STATEMENT_END:
		case SEMI:
		case LITERAL_if:
		case LITERAL_unless:
		case LITERAL_redo:
		case LITERAL_break:
		case LITERAL_next:
		case LITERAL_retry:
		case ASSIGN:
		case IDENT:
		case LTHAN:
		case LPAREN:
		case STAR:
		case BAND:
		case PLUS_ASSIGN:
		case MINUS_ASSIGN:
		case STAR_ASSIGN:
		case DIV_ASSIGN:
		case MOD_ASSIGN:
		case BAND_ASSIGN:
		case BOR_ASSIGN:
		case BXOR_ASSIGN:
		case LITERAL_until:
		case STATICIDENT:
		case INSTIDENT:
		case DOT:
		case SYMBOL:
		case LITERAL_lambda:
		case LCURLY:
		case LITERAL_raise:
		case LITERAL_yield:
		case BOR:
		case LITERAL_or:
		case LITERAL_and:
		case LITERAL_not:
		case LNOT:
		case GT:
		case EQUAL:
		case GE:
		case LE:
		case NOT_EQUAL:
		case BXOR:
		case PLUS:
		case MINUS:
		case SLASH:
		case PERCENT:
		case BNOT:
		case LBRACK:
		case STRING_LITERAL:
		case CHAR_LITERAL:
		{
			{
				if ((tokenSet_10_.member(LA(1))) && (tokenSet_9_.member(LA(2))))
				{
					exp=test();
					{
						switch ( LA(1) )
						{
						case PLUS_ASSIGN:
						case MINUS_ASSIGN:
						case STAR_ASSIGN:
						case DIV_ASSIGN:
						case MOD_ASSIGN:
						case BAND_ASSIGN:
						case BOR_ASSIGN:
						case BXOR_ASSIGN:
						{
							rel=augassign();
							rhs=test();
							if (0==inputState.guessing)
							{
								exp = new AugAssignmentExpression(exp, rhs, rel);
							}
							break;
						}
						case ASSIGN:
						{
							{ // ( ... )+
								int _cnt65=0;
								for (;;)
								{
									if ((LA(1)==ASSIGN))
									{
										match(ASSIGN);
										rhs=test();
										if (0==inputState.guessing)
										{
											exp = new AssignmentExpression(exp, rhs);
										}
									}
									else
									{
										if (_cnt65 >= 1) { goto _loop65_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
									}
									
									_cnt65++;
								}
_loop65_breakloop:								;
							}    // ( ... )+
							break;
						}
						case WHILE:
						case STATEMENT_END:
						case SEMI:
						case LITERAL_if:
						case LITERAL_unless:
						case LITERAL_until:
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
				else if ((LA(1)==DO||LA(1)==BEGIN) && (LA(2)==STATEMENT_END||LA(2)==SEMI)) {
					exp=compound();
				}
				else if (((LA(1) >= LITERAL_redo && LA(1) <= LITERAL_retry))) {
					exp=flow_expressions();
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			{
				switch ( LA(1) )
				{
				case WHILE:
				case LITERAL_if:
				case LITERAL_unless:
				case LITERAL_until:
				{
					pfc=postFixCondition();
					if (0==inputState.guessing)
					{
						exp.PostFixStatement = pfc;
					}
					break;
				}
				case STATEMENT_END:
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
			if (0==inputState.guessing)
			{
				stmt = new ExpressionStatement(exp);
			}
			break;
		}
		case DEF:
		{
			stmt=method_def_statement();
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
		return stmt;
	}
	
	public IExpression  test() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null; IExpression rhs = null;
		
		switch ( LA(1) )
		{
		case LITERAL_lambda:
		{
			exp=lambda();
			break;
		}
		case LITERAL_raise:
		{
			exp=raise();
			break;
		}
		case LITERAL_yield:
		{
			exp=yield();
			break;
		}
		default:
			if ((tokenSet_11_.member(LA(1))) && (tokenSet_12_.member(LA(2))))
			{
				exp=and_test();
				{    // ( ... )*
					for (;;)
					{
						if ((LA(1)==LITERAL_or))
						{
							match(LITERAL_or);
							rhs=and_test();
							if (0==inputState.guessing)
							{
								exp = new BinaryExpression(exp, rhs, BinaryOp.Or);
							}
						}
						else
						{
							goto _loop102_breakloop;
						}
						
					}
_loop102_breakloop:					;
				}    // ( ... )*
			}
			else if ((LA(1)==DO||LA(1)==LCURLY) && (tokenSet_13_.member(LA(2)))) {
				exp=block();
			}
		else
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		break; }
		return exp;
	}
	
	public VariableReferenceExpression  varref() //throws RecognitionException, TokenStreamException
{
		VariableReferenceExpression vre;
		
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		vre = null;
		
		switch ( LA(1) )
		{
		case IDENT:
		{
			t1 = LT(1);
			match(IDENT);
			if (0==inputState.guessing)
			{
				vre = new VariableReferenceExpression(t1.getText(), VariableReferenceType.LocalOrArgument);
			}
			break;
		}
		case STATICIDENT:
		{
			t2 = LT(1);
			match(STATICIDENT);
			if (0==inputState.guessing)
			{
				vre = new VariableReferenceExpression(t2.getText(), VariableReferenceType.StaticField);
			}
			break;
		}
		case INSTIDENT:
		{
			t3 = LT(1);
			match(INSTIDENT);
			if (0==inputState.guessing)
			{
				vre = new VariableReferenceExpression(t3.getText(), VariableReferenceType.InstanceField);
			}
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
		return vre;
	}
	
	public IExpression  flow_expressions() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null;
		
		switch ( LA(1) )
		{
		case LITERAL_redo:
		{
			match(LITERAL_redo);
			if (0==inputState.guessing)
			{
				exp = new RedoExpression();
			}
			break;
		}
		case LITERAL_break:
		{
			match(LITERAL_break);
			if (0==inputState.guessing)
			{
				exp = new BreakExpression();
			}
			break;
		}
		case LITERAL_next:
		{
			match(LITERAL_next);
			if (0==inputState.guessing)
			{
				exp = new NextExpression();
			}
			break;
		}
		case LITERAL_retry:
		{
			match(LITERAL_retry);
			if (0==inputState.guessing)
			{
				exp = new RetryExpression();
			}
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
		return exp;
	}
	
	public TypeDeclarationExpression  type_name_withtype() //throws RecognitionException, TokenStreamException
{
		TypeDeclarationExpression tdexp;
		
		tdexp = null; String n=null; TypeReference tr = null;
		
		n=name();
		tr=type();
		if (0==inputState.guessing)
		{
			tdexp = new TypeDeclarationExpression(n, tr);
		}
		return tdexp;
	}
	
	public TypeDefinitionStatement  class_def_statement() //throws RecognitionException, TokenStreamException
{
		TypeDefinitionStatement tdstmt;
		
		IToken  t = null;
		tdstmt = null;
		
		match(CLASS);
		t = LT(1);
		match(IDENT);
		if (0==inputState.guessing)
		{
			tdstmt = new TypeDefinitionStatement( t.getText() );
		}
		{
			switch ( LA(1) )
			{
			case LTHAN:
			case SL:
			{
				{
					switch ( LA(1) )
					{
					case LTHAN:
					{
						match(LTHAN);
						break;
					}
					case SL:
					{
						match(SL);
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				qualified_name();
				{    // ( ... )*
					for (;;)
					{
						if ((LA(1)==COMMA))
						{
							match(COMMA);
							qualified_name();
						}
						else
						{
							goto _loop52_breakloop;
						}
						
					}
_loop52_breakloop:					;
				}    // ( ... )*
				break;
			}
			case STATEMENT_END:
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
		statement_term();
		suite(tdstmt.Statements);
		match(END);
		return tdstmt;
	}
	
	public MethodDefinitionStatement  method_def_statement() //throws RecognitionException, TokenStreamException
{
		MethodDefinitionStatement mdstmt;
		
		mdstmt = null; String qn = null; TypeReference retType = null;
		
		match(DEF);
		qn=qualified_name();
		if (0==inputState.guessing)
		{
			mdstmt = new MethodDefinitionStatement(qn);
		}
		match(LPAREN);
		{
			switch ( LA(1) )
			{
			case IDENT:
			case STAR:
			case BAND:
			case STATICIDENT:
			case INSTIDENT:
			{
				methodParams(mdstmt);
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
			case SYMBOL:
			{
				retType=type();
				break;
			}
			case STATEMENT_END:
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
		statement_term();
		if (0==inputState.guessing)
		{
			mdstmt.ReturnType = retType;
		}
		suite(mdstmt.Statements);
		match(END);
		return mdstmt;
	}
	
	public void methodParams(
		MethodDefinitionStatement mdstmt
	) //throws RecognitionException, TokenStreamException
{
		
		
		methodParam(mdstmt);
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					methodParam(mdstmt);
				}
				else
				{
					goto _loop58_breakloop;
				}
				
			}
_loop58_breakloop:			;
		}    // ( ... )*
	}
	
	public  TypeReference  type() //throws RecognitionException, TokenStreamException
{
		 TypeReference tr ;
		
		tr = null; String n;
		
		n=qualified_symbol();
		if (0==inputState.guessing)
		{
			tr = ObtainTypeReference(n);
		}
		return tr ;
	}
	
	public void methodParam(
		MethodDefinitionStatement mdstmt
	) //throws RecognitionException, TokenStreamException
{
		
		IExpression exp = null; TypeDeclarationExpression typeName = null;
		
		switch ( LA(1) )
		{
		case IDENT:
		case STATICIDENT:
		case INSTIDENT:
		{
			typeName=type_name();
			{
				switch ( LA(1) )
				{
				case ASSIGN:
				{
					match(ASSIGN);
					exp=expression();
					if (0==inputState.guessing)
					{
						typeName.InitExp = exp;
					}
					break;
				}
				case COMMA:
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
			break;
		}
		case STAR:
		{
			match(STAR);
			type_name();
			break;
		}
		case BAND:
		{
			match(BAND);
			match(IDENT);
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
	}
	
	public TypeDeclarationExpression  type_name() //throws RecognitionException, TokenStreamException
{
		TypeDeclarationExpression tdexp;
		
		tdexp = null; String n=null; TypeReference tr = null;
		
		n=name();
		{
			switch ( LA(1) )
			{
			case SYMBOL:
			{
				tr=type();
				break;
			}
			case COMMA:
			case ASSIGN:
			case RPAREN:
			case BOR:
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
			tdexp = new TypeDeclarationExpression(n, tr);
		}
		return tdexp;
	}
	
	public IExpression  expression() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null; IExpression rhs = null;
		
		exp=xor_expr();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==BXOR))
				{
					match(BXOR);
					rhs=xor_expr();
					if (0==inputState.guessing)
					{
						exp = new BinaryExpression(exp, rhs, BinaryOp.Xor);
					}
				}
				else
				{
					goto _loop117_breakloop;
				}
				
			}
_loop117_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public AugType  augassign() //throws RecognitionException, TokenStreamException
{
		AugType rel;
		
		rel = AugType.Undefined;
		
		switch ( LA(1) )
		{
		case PLUS_ASSIGN:
		{
			match(PLUS_ASSIGN);
			if (0==inputState.guessing)
			{
				rel = AugType.PlusAssign;
			}
			break;
		}
		case MINUS_ASSIGN:
		{
			match(MINUS_ASSIGN);
			if (0==inputState.guessing)
			{
				rel = AugType.MinusAssign;
			}
			break;
		}
		case STAR_ASSIGN:
		{
			match(STAR_ASSIGN);
			if (0==inputState.guessing)
			{
				rel = AugType.MultAssign;
			}
			break;
		}
		case DIV_ASSIGN:
		{
			match(DIV_ASSIGN);
			if (0==inputState.guessing)
			{
				rel = AugType.DivAssign;
			}
			break;
		}
		case MOD_ASSIGN:
		{
			match(MOD_ASSIGN);
			if (0==inputState.guessing)
			{
				rel = AugType.ModAssign;
			}
			break;
		}
		case BAND_ASSIGN:
		{
			match(BAND_ASSIGN);
			if (0==inputState.guessing)
			{
				rel = AugType.BitwiseAndAssign;
			}
			break;
		}
		case BOR_ASSIGN:
		{
			match(BOR_ASSIGN);
			if (0==inputState.guessing)
			{
				rel = AugType.BitwiseOrAssign;
			}
			break;
		}
		case BXOR_ASSIGN:
		{
			match(BXOR_ASSIGN);
			if (0==inputState.guessing)
			{
				rel = AugType.BitwiseXorAssign;
			}
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
		return rel;
	}
	
	public CompoundExpression  compound() //throws RecognitionException, TokenStreamException
{
		CompoundExpression cexp;
		
		cexp = new CompoundExpression();
		
		{
			switch ( LA(1) )
			{
			case DO:
			{
				match(DO);
				break;
			}
			case BEGIN:
			{
				match(BEGIN);
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		statement_term();
		suite(cexp.Statements);
		match(END);
		return cexp;
	}
	
	public PostfixCondition  postFixCondition() //throws RecognitionException, TokenStreamException
{
		PostfixCondition pfc;
		
		pfc = null; IExpression exp;
		
		{
			{
				switch ( LA(1) )
				{
				case LITERAL_if:
				{
					match(LITERAL_if);
					if (0==inputState.guessing)
					{
						pfc = new PostfixCondition(PostfixConditionType.If);
					}
					break;
				}
				case LITERAL_unless:
				{
					match(LITERAL_unless);
					if (0==inputState.guessing)
					{
						pfc = new PostfixCondition(PostfixConditionType.Unless);
					}
					break;
				}
				case WHILE:
				{
					match(WHILE);
					if (0==inputState.guessing)
					{
						pfc = new PostfixCondition(PostfixConditionType.While);
					}
					break;
				}
				case LITERAL_until:
				{
					match(LITERAL_until);
					if (0==inputState.guessing)
					{
						pfc = new PostfixCondition(PostfixConditionType.Until);
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			exp=test();
			if (0==inputState.guessing)
			{
				pfc.Condition = exp;
			}
		}
		return pfc;
	}
	
	public String  name() //throws RecognitionException, TokenStreamException
{
		String name;
		
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		name = null;
		
		switch ( LA(1) )
		{
		case IDENT:
		{
			t1 = LT(1);
			match(IDENT);
			if (0==inputState.guessing)
			{
				name = t1.getText();
			}
			break;
		}
		case STATICIDENT:
		{
			t2 = LT(1);
			match(STATICIDENT);
			if (0==inputState.guessing)
			{
				name = t2.getText();
			}
			break;
		}
		case INSTIDENT:
		{
			t3 = LT(1);
			match(INSTIDENT);
			if (0==inputState.guessing)
			{
				name = t3.getText();
			}
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
		return name;
	}
	
	public String  qualified_symbol() //throws RecognitionException, TokenStreamException
{
		String name;
		
		IToken  t = null;
		IToken  t2 = null;
		name = null;
		
		t = LT(1);
		match(SYMBOL);
		if (0==inputState.guessing)
		{
			name = t.getText();
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==DOT))
				{
					match(DOT);
					t2 = LT(1);
					match(IDENT);
					if (0==inputState.guessing)
					{
						name += "." + t2.getText();
					}
				}
				else
				{
					goto _loop81_breakloop;
				}
				
			}
_loop81_breakloop:			;
		}    // ( ... )*
		return name;
	}
	
	public LambdaExpression  lambda() //throws RecognitionException, TokenStreamException
{
		LambdaExpression lexp;
		
		BlockExpression bexp=null; lexp = null;
		
		match(LITERAL_lambda);
		bexp=block();
		if (0==inputState.guessing)
		{
			lexp = new LambdaExpression(bexp);
		}
		return lexp;
	}
	
	public BlockExpression  block() //throws RecognitionException, TokenStreamException
{
		BlockExpression bexp;
		
		bexp = new BlockExpression();
		
		{
			switch ( LA(1) )
			{
			case DO:
			{
				{
					match(DO);
					{
						if ((LA(1)==STATEMENT_END||LA(1)==SEMI) && (tokenSet_14_.member(LA(2))))
						{
							statement_term();
						}
						else if ((tokenSet_14_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					{
						if ((LA(1)==BOR) && (tokenSet_5_.member(LA(2))))
						{
							blockargs(bexp);
						}
						else if ((tokenSet_14_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					{
						if ((LA(1)==STATEMENT_END||LA(1)==SEMI) && (tokenSet_14_.member(LA(2))))
						{
							statement_term();
						}
						else if ((tokenSet_14_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					suite(bexp.Statements);
					match(END);
				}
				break;
			}
			case LCURLY:
			{
				{
					match(LCURLY);
					{
						if ((LA(1)==STATEMENT_END||LA(1)==SEMI) && (tokenSet_16_.member(LA(2))))
						{
							statement_term();
						}
						else if ((tokenSet_16_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					{
						if ((LA(1)==BOR) && (tokenSet_5_.member(LA(2))))
						{
							blockargs(bexp);
						}
						else if ((tokenSet_16_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					{
						if ((LA(1)==STATEMENT_END||LA(1)==SEMI) && (tokenSet_16_.member(LA(2))))
						{
							statement_term();
						}
						else if ((tokenSet_16_.member(LA(1))) && (tokenSet_15_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					suite(bexp.Statements);
					match(RCURLY);
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		return bexp;
	}
	
	public void blockargs(
		BlockExpression bexp
	) //throws RecognitionException, TokenStreamException
{
		
		bexp = new BlockExpression(); TypeDeclarationExpression tdexp = null;
		
		match(BOR);
		tdexp=type_name();
		if (0==inputState.guessing)
		{
			bexp.AddBlockArgument(tdexp);
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					tdexp=type_name();
					if (0==inputState.guessing)
					{
						bexp.AddBlockArgument(tdexp);
					}
				}
				else
				{
					goto _loop97_breakloop;
				}
				
			}
_loop97_breakloop:			;
		}    // ( ... )*
		match(BOR);
	}
	
	public RaiseExpression  raise() //throws RecognitionException, TokenStreamException
{
		RaiseExpression rexp;
		
		rexp = null; IExpression exp;
		
		match(LITERAL_raise);
		exp=expression();
		if (0==inputState.guessing)
		{
			rexp = new RaiseExpression(exp);
		}
		return rexp;
	}
	
	public YieldExpression  yield() //throws RecognitionException, TokenStreamException
{
		YieldExpression rexp;
		
		rexp = null; ExpressionCollection expColl;
		
		match(LITERAL_yield);
		expColl=expressionList();
		if (0==inputState.guessing)
		{
			rexp = new YieldExpression(expColl);
		}
		return rexp;
	}
	
	public ExpressionCollection  expressionList() //throws RecognitionException, TokenStreamException
{
		ExpressionCollection expColl;
		
		expColl = new ExpressionCollection(); IExpression exp = null;
		
		exp=expression();
		if (0==inputState.guessing)
		{
			expColl.Add(exp);
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA) && (tokenSet_17_.member(LA(2))))
				{
					match(COMMA);
					exp=expression();
					if (0==inputState.guessing)
					{
						expColl.Add(exp);
					}
				}
				else
				{
					goto _loop114_breakloop;
				}
				
			}
_loop114_breakloop:			;
		}    // ( ... )*
		return expColl;
	}
	
	public IExpression  and_test() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null; IExpression rhs = null;
		
		exp=not_test();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==LITERAL_and))
				{
					match(LITERAL_and);
					rhs=not_test();
					if (0==inputState.guessing)
					{
						exp = new BinaryExpression(exp, rhs, BinaryOp.And);
					}
				}
				else
				{
					goto _loop105_breakloop;
				}
				
			}
_loop105_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public IExpression  not_test() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null; IExpression inner = null;
		
		switch ( LA(1) )
		{
		case LITERAL_not:
		case LNOT:
		{
			{
				switch ( LA(1) )
				{
				case LITERAL_not:
				{
					match(LITERAL_not);
					break;
				}
				case LNOT:
				{
					match(LNOT);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			inner=not_test();
			if (0==inputState.guessing)
			{
				exp = new UnaryExpression(inner, UnaryOp.Not);
			}
			break;
		}
		case DO:
		case WHILE:
		case NUM_INT:
		case NUM_FLOAT:
		case NUM_LONG:
		case STATEMENT_END:
		case SEMI:
		case LITERAL_if:
		case LITERAL_then:
		case LITERAL_unless:
		case COMMA:
		case ASSIGN:
		case IDENT:
		case LTHAN:
		case LPAREN:
		case RPAREN:
		case STAR:
		case BAND:
		case PLUS_ASSIGN:
		case MINUS_ASSIGN:
		case STAR_ASSIGN:
		case DIV_ASSIGN:
		case MOD_ASSIGN:
		case BAND_ASSIGN:
		case BOR_ASSIGN:
		case BXOR_ASSIGN:
		case LITERAL_until:
		case STATICIDENT:
		case INSTIDENT:
		case DOT:
		case SYMBOL:
		case LCURLY:
		case RCURLY:
		case BOR:
		case LITERAL_or:
		case LITERAL_and:
		case GT:
		case EQUAL:
		case GE:
		case LE:
		case NOT_EQUAL:
		case BXOR:
		case PLUS:
		case MINUS:
		case SLASH:
		case PERCENT:
		case BNOT:
		case LBRACK:
		case RBRACK:
		case STRING_LITERAL:
		case CHAR_LITERAL:
		{
			exp=comparison();
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
		return exp;
	}
	
	public IExpression  comparison() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null; IExpression rhs = null; BinaryOp op = BinaryOp.Undefined;
		
		exp=expression();
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_18_.member(LA(1))))
				{
					op=comp_op();
					rhs=expression();
					if (0==inputState.guessing)
					{
						exp = new BinaryExpression(exp, rhs, op);
					}
				}
				else
				{
					goto _loop110_breakloop;
				}
				
			}
_loop110_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public BinaryOp  comp_op() //throws RecognitionException, TokenStreamException
{
		BinaryOp op;
		
		op = BinaryOp.Undefined;
		
		switch ( LA(1) )
		{
		case LTHAN:
		{
			match(LTHAN);
			if (0==inputState.guessing)
			{
				op = BinaryOp.LessThan;
			}
			break;
		}
		case GT:
		{
			match(GT);
			if (0==inputState.guessing)
			{
				op = BinaryOp.GreaterThan;
			}
			break;
		}
		case EQUAL:
		{
			match(EQUAL);
			if (0==inputState.guessing)
			{
				op = BinaryOp.Equal;
			}
			break;
		}
		case GE:
		{
			match(GE);
			if (0==inputState.guessing)
			{
				op = BinaryOp.GreaterEqual;
			}
			break;
		}
		case LE:
		{
			match(LE);
			if (0==inputState.guessing)
			{
				op = BinaryOp.LessEqual;
			}
			break;
		}
		case NOT_EQUAL:
		{
			match(NOT_EQUAL);
			if (0==inputState.guessing)
			{
				op = BinaryOp.NotEqual;
			}
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
		return op;
	}
	
	public IExpression  xor_expr() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null; IExpression rhs = null;
		
		exp=and_expr();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==BOR))
				{
					match(BOR);
					rhs=and_expr();
					if (0==inputState.guessing)
					{
						exp = new BinaryExpression(exp, rhs, BinaryOp.Or2);
					}
				}
				else
				{
					goto _loop120_breakloop;
				}
				
			}
_loop120_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public IExpression  and_expr() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null; IExpression rhs = null;
		
		exp=arith_expr();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==BAND))
				{
					match(BAND);
					rhs=arith_expr();
					if (0==inputState.guessing)
					{
						exp = new BinaryExpression(exp, rhs, BinaryOp.And2);
					}
				}
				else
				{
					goto _loop123_breakloop;
				}
				
			}
_loop123_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public IExpression  arith_expr() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		IToken  t = null;
		exp = null; IExpression rhs = null;
		
		exp=term();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==PLUS||LA(1)==MINUS))
				{
					{
						switch ( LA(1) )
						{
						case PLUS:
						{
							t = LT(1);
							match(PLUS);
							break;
						}
						case MINUS:
						{
							match(MINUS);
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					rhs=term();
					if (0==inputState.guessing)
					{
						exp = new BinaryExpression(exp, rhs, t != null ? BinaryOp.Plus : BinaryOp.Minus);
					}
				}
				else
				{
					goto _loop127_breakloop;
				}
				
			}
_loop127_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public IExpression  term() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null; IExpression rhs = null; BinaryOp op = BinaryOp.Undefined;
		
		exp=unary();
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_19_.member(LA(1))))
				{
					{
						switch ( LA(1) )
						{
						case STAR:
						{
							match(STAR);
							if (0==inputState.guessing)
							{
								op = BinaryOp.Mult;
							}
							break;
						}
						case SLASH:
						{
							match(SLASH);
							if (0==inputState.guessing)
							{
								op = BinaryOp.Div;
							}
							break;
						}
						case PERCENT:
						{
							match(PERCENT);
							if (0==inputState.guessing)
							{
								op = BinaryOp.Mod;
							}
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					rhs=unary();
					if (0==inputState.guessing)
					{
						exp = new BinaryExpression(exp, rhs, op);
					}
				}
				else
				{
					goto _loop131_breakloop;
				}
				
			}
_loop131_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public IExpression  unary() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null; IExpression inner = null; UnaryOp op = UnaryOp.Plus;
		
		if ((tokenSet_20_.member(LA(1))) && (tokenSet_21_.member(LA(2))))
		{
			{
				switch ( LA(1) )
				{
				case PLUS:
				{
					match(PLUS);
					if (0==inputState.guessing)
					{
						op = UnaryOp.Plus;
					}
					break;
				}
				case MINUS:
				{
					match(MINUS);
					if (0==inputState.guessing)
					{
						op = UnaryOp.Minus;
					}
					break;
				}
				case BNOT:
				{
					match(BNOT);
					if (0==inputState.guessing)
					{
						op = UnaryOp.BitwiseNot;
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			inner=unary();
			if (0==inputState.guessing)
			{
				exp = new UnaryExpression(inner, op);
			}
		}
		else if ((tokenSet_22_.member(LA(1))) && (tokenSet_12_.member(LA(2)))) {
			exp=primary();
		}
		else
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		
		return exp;
	}
	
	public IExpression  primary() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null;
		
		exp=atom();
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_23_.member(LA(1))))
				{
					exp=trailer(exp);
				}
				else
				{
					goto _loop136_breakloop;
				}
				
			}
_loop136_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public IExpression  atom() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null;
		
		switch ( LA(1) )
		{
		case LCURLY:
		{
			match(LCURLY);
			{
				switch ( LA(1) )
				{
				case NUM_INT:
				case NUM_FLOAT:
				case NUM_LONG:
				case IDENT:
				case LPAREN:
				case STAR:
				case BAND:
				case STATICIDENT:
				case INSTIDENT:
				case DOT:
				case SYMBOL:
				case LCURLY:
				case BOR:
				case BXOR:
				case PLUS:
				case MINUS:
				case SLASH:
				case PERCENT:
				case BNOT:
				case LBRACK:
				case STRING_LITERAL:
				case CHAR_LITERAL:
				case MAPASSIGN:
				{
					exp=dictmaker();
					break;
				}
				case RCURLY:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RCURLY);
			break;
		}
		case IDENT:
		case STATICIDENT:
		case INSTIDENT:
		{
			exp=varref();
			break;
		}
		default:
			bool synPredMatched139 = false;
			if (((LA(1)==LPAREN) && (tokenSet_24_.member(LA(2)))))
			{
				int _m139 = mark();
				synPredMatched139 = true;
				inputState.guessing++;
				try {
					{
						range();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched139 = false;
				}
				rewind(_m139);
				inputState.guessing--;
			}
			if ( synPredMatched139 )
			{
				exp=range();
			}
			else if ((LA(1)==LPAREN) && (tokenSet_25_.member(LA(2)))) {
				match(LPAREN);
				{
					if ((tokenSet_25_.member(LA(1))) && (tokenSet_26_.member(LA(2))))
					{
						exp=test();
					}
					else if ((LA(1)==RPAREN) && (tokenSet_27_.member(LA(2)))) {
					}
					else
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					
				}
				match(RPAREN);
			}
			else if ((LA(1)==LBRACK) && (tokenSet_28_.member(LA(2)))) {
				match(LBRACK);
				{
					if ((tokenSet_28_.member(LA(1))) && (tokenSet_26_.member(LA(2))))
					{
						exp=listmaker();
					}
					else if ((LA(1)==RBRACK) && (tokenSet_27_.member(LA(2)))) {
					}
					else
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					
				}
				match(RBRACK);
			}
			else if ((tokenSet_29_.member(LA(1))) && (tokenSet_12_.member(LA(2)))) {
				exp=constantref();
			}
		else
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		break; }
		return exp;
	}
	
	public IExpression  trailer(
		IExpression inner
	) //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null; ExpressionCollection args = null;
		
		switch ( LA(1) )
		{
		case LPAREN:
		{
			match(LPAREN);
			{
				if ((tokenSet_30_.member(LA(1))) && (tokenSet_26_.member(LA(2))))
				{
					args=arglist();
				}
				else if ((LA(1)==RPAREN) && (tokenSet_27_.member(LA(2)))) {
				}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				
			}
			match(RPAREN);
			if (0==inputState.guessing)
			{
				exp = new MethodInvocationExpression(inner, args);
			}
			break;
		}
		case LBRACK:
		{
			match(LBRACK);
			subscriptlist();
			match(RBRACK);
			break;
		}
		case DOT:
		{
			match(DOT);
			match(IDENT);
			if (0==inputState.guessing)
			{
				exp = new MemberAccessExpression(inner);
			}
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
		return exp;
	}
	
	public IExpression  range() //throws RecognitionException, TokenStreamException
{
		IExpression rex;
		
		IToken  t = null;
		rex = null; IExpression lhs = null; IExpression rhs = null;
		
		match(LPAREN);
		lhs=expression();
		{
			switch ( LA(1) )
			{
			case DOTDOT:
			{
				t = LT(1);
				match(DOTDOT);
				break;
			}
			case DOTDOTDOT:
			{
				match(DOTDOTDOT);
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		rhs=expression();
		match(RPAREN);
		if (0==inputState.guessing)
		{
			rex = new RangeExpression(lhs, rhs, t != null);
		}
		return rex;
	}
	
	public ListExpression  listmaker() //throws RecognitionException, TokenStreamException
{
		ListExpression exp;
		
		exp = new ListExpression(); IExpression item;
		
		item=test();
		if (0==inputState.guessing)
		{
			exp.Add(item);
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					item=test();
					if (0==inputState.guessing)
					{
						exp.Add(item);
					}
				}
				else
				{
					goto _loop159_breakloop;
				}
				
			}
_loop159_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public DictExpression  dictmaker() //throws RecognitionException, TokenStreamException
{
		DictExpression exp;
		
		exp = new DictExpression(); IExpression key, value;
		
		key=expression();
		match(MAPASSIGN);
		value=test();
		if (0==inputState.guessing)
		{
			exp.Add(key, exp);
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					key=expression();
					match(MAPASSIGN);
					value=test();
					if (0==inputState.guessing)
					{
						exp.Add(key, exp);
					}
				}
				else
				{
					goto _loop162_breakloop;
				}
				
			}
_loop162_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public LiteralReferenceExpression  constantref() //throws RecognitionException, TokenStreamException
{
		LiteralReferenceExpression lre;
		
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		IToken  t4 = null;
		IToken  t5 = null;
		IToken  t6 = null;
		lre = null;
		
		switch ( LA(1) )
		{
		case DO:
		case WHILE:
		case STATEMENT_END:
		case SEMI:
		case LITERAL_if:
		case LITERAL_then:
		case LITERAL_unless:
		case COMMA:
		case ASSIGN:
		case LTHAN:
		case LPAREN:
		case RPAREN:
		case STAR:
		case BAND:
		case PLUS_ASSIGN:
		case MINUS_ASSIGN:
		case STAR_ASSIGN:
		case DIV_ASSIGN:
		case MOD_ASSIGN:
		case BAND_ASSIGN:
		case BOR_ASSIGN:
		case BXOR_ASSIGN:
		case LITERAL_until:
		case DOT:
		case RCURLY:
		case BOR:
		case LITERAL_or:
		case LITERAL_and:
		case GT:
		case EQUAL:
		case GE:
		case LE:
		case NOT_EQUAL:
		case BXOR:
		case PLUS:
		case MINUS:
		case SLASH:
		case PERCENT:
		case LBRACK:
		case RBRACK:
		case DOTDOT:
		case DOTDOTDOT:
		case MAPASSIGN:
		{
			break;
		}
		case NUM_INT:
		{
			t1 = LT(1);
			match(NUM_INT);
			if (0==inputState.guessing)
			{
				lre = new LiteralReferenceExpression(t1.getText(), LiteralReferenceType.IntLiteral);
			}
			break;
		}
		case NUM_LONG:
		{
			t2 = LT(1);
			match(NUM_LONG);
			if (0==inputState.guessing)
			{
				lre = new LiteralReferenceExpression(t2.getText(), LiteralReferenceType.LongLiteral);
			}
			break;
		}
		case NUM_FLOAT:
		{
			t3 = LT(1);
			match(NUM_FLOAT);
			if (0==inputState.guessing)
			{
				lre = new LiteralReferenceExpression(t3.getText(), LiteralReferenceType.FloatLiteral);
			}
			break;
		}
		case SYMBOL:
		{
			t4 = LT(1);
			match(SYMBOL);
			if (0==inputState.guessing)
			{
				lre = new LiteralReferenceExpression(t4.getText(), LiteralReferenceType.SymbolLiteral);
			}
			break;
		}
		case STRING_LITERAL:
		{
			t5 = LT(1);
			match(STRING_LITERAL);
			if (0==inputState.guessing)
			{
				lre = new LiteralReferenceExpression(t5.getText(), LiteralReferenceType.StringLiteral);
			}
			break;
		}
		case CHAR_LITERAL:
		{
			t6 = LT(1);
			match(CHAR_LITERAL);
			if (0==inputState.guessing)
			{
				lre = new LiteralReferenceExpression(t6.getText(), LiteralReferenceType.CharLiteral);
			}
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
		return lre;
	}
	
	public ExpressionCollection  arglist() //throws RecognitionException, TokenStreamException
{
		ExpressionCollection expcoll;
		
		IExpression exp; expcoll = new ExpressionCollection();
		
		exp=argument();
		if (0==inputState.guessing)
		{
			expcoll.Add(exp);
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					exp=argument();
					if (0==inputState.guessing)
					{
						expcoll.Add(exp);
					}
				}
				else
				{
					goto _loop155_breakloop;
				}
				
			}
_loop155_breakloop:			;
		}    // ( ... )*
		return expcoll;
	}
	
	public void subscriptlist() //throws RecognitionException, TokenStreamException
{
		
		
		subscript();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					subscript();
				}
				else
				{
					goto _loop151_breakloop;
				}
				
			}
_loop151_breakloop:			;
		}    // ( ... )*
	}
	
	public void subscript() //throws RecognitionException, TokenStreamException
{
		
		
		expression();
	}
	
	public IExpression  argument() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null;
		
		exp=test();
		return exp;
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
		@"""do""",
		@"""end""",
		@"""def""",
		@"""operator""",
		@"""begin""",
		@"""while""",
		@"""TYPE""",
		@"""METHOD_CALL""",
		@"""SUPER_CTOR_CALL""",
		@"""POST_INC""",
		@"""POST_DEC""",
		@"""EXPR""",
		@"""ELIST""",
		@"""INDEX_OP""",
		@"""UNARY_MINUS""",
		@"""UNARY_PLUS""",
		@"""TYPECAST""",
		@"""ARRAY_DECLARATOR""",
		@"""NUM_INT""",
		@"""NUM_DOUBLE""",
		@"""NUM_FLOAT""",
		@"""NUM_LONG""",
		@"""STATEMENT_END""",
		@"""SEMI""",
		@"""namespace""",
		@"""UNTIL""",
		@"""for""",
		@"""in""",
		@"""if""",
		@"""then""",
		@"""elsif""",
		@"""else""",
		@"""unless""",
		@"""redo""",
		@"""break""",
		@"""next""",
		@"""retry""",
		@"""public""",
		@"""COLON""",
		@"""private""",
		@"""protected""",
		@"""internal""",
		@"""COMMA""",
		@"""ASSIGN""",
		@"""IDENT""",
		@"""LTHAN""",
		@"""SL""",
		@"""LPAREN""",
		@"""RPAREN""",
		@"""STAR""",
		@"""BAND""",
		@"""PLUS_ASSIGN""",
		@"""MINUS_ASSIGN""",
		@"""STAR_ASSIGN""",
		@"""DIV_ASSIGN""",
		@"""MOD_ASSIGN""",
		@"""BAND_ASSIGN""",
		@"""BOR_ASSIGN""",
		@"""BXOR_ASSIGN""",
		@"""until""",
		@"""STATICIDENT""",
		@"""INSTIDENT""",
		@"""DOT""",
		@"""SYMBOL""",
		@"""lambda""",
		@"""LCURLY""",
		@"""RCURLY""",
		@"""raise""",
		@"""yield""",
		@"""BOR""",
		@"""or""",
		@"""and""",
		@"""not""",
		@"""LNOT""",
		@"""GT""",
		@"""EQUAL""",
		@"""GE""",
		@"""LE""",
		@"""NOT_EQUAL""",
		@"""BXOR""",
		@"""PLUS""",
		@"""MINUS""",
		@"""SLASH""",
		@"""PERCENT""",
		@"""BNOT""",
		@"""LBRACK""",
		@"""RBRACK""",
		@"""STRING_LITERAL""",
		@"""CHAR_LITERAL""",
		@"""DOTDOT""",
		@"""DOTDOTDOT""",
		@"""MAPASSIGN""",
		@"""QUESTION""",
		@"""DIV""",
		@"""INC""",
		@"""DEC""",
		@"""MOD""",
		@"""SR""",
		@"""SR_ASSIGN""",
		@"""BSR""",
		@"""BSR_ASSIGN""",
		@"""SL_ASSIGN""",
		@"""LOR""",
		@"""LAND""",
		@"""a new line""",
		@"""SL_NEWLINE""",
		@"""comments""",
		@"""WS""",
		@"""ESC""",
		@"""HEX_DIGIT""",
		@"""VOCAB""",
		@"""NUMBER""",
		@"""Int""",
		@"""NonZeroDigit""",
		@"""FloatTrailer""",
		@"""Exponent""",
		@"""CONTINUED_LINE"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { -11408657229019470L, 939523967L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { -2251907749902606L, 8589934591L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { 134217730L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { -11408657765890384L, 939523967L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { -2251804670687502L, 8589934591L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { 562949953421312L, 6L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { 60235645527785504L, 939523966L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { 60235662707654688L, 939523966L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { -11536204335937888L, 939523967L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { -2260617943578894L, 8589934591L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { -11540327504542688L, 939523967L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { -2392373581577184L, 1073741023L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { -2260600763709710L, 8589934591L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	private static long[] mk_tokenSet_13_()
	{
		long[] data = { -11408657765890320L, 939524095L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
	private static long[] mk_tokenSet_14_()
	{
		long[] data = { -11408657765890320L, 939523967L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
	private static long[] mk_tokenSet_15_()
	{
		long[] data = { -2251907749902608L, 8589934591L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
	private static long[] mk_tokenSet_16_()
	{
		long[] data = { -11408657765890384L, 939524095L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_16_ = new BitSet(mk_tokenSet_16_());
	private static long[] mk_tokenSet_17_()
	{
		long[] data = { -3518273488419808L, 1072694495L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_17_ = new BitSet(mk_tokenSet_17_());
	private static long[] mk_tokenSet_18_()
	{
		long[] data = { 1125899906842624L, 1015808L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_18_ = new BitSet(mk_tokenSet_18_());
	private static long[] mk_tokenSet_19_()
	{
		long[] data = { 18014398509481984L, 25165824L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_19_ = new BitSet(mk_tokenSet_19_());
	private static long[] mk_tokenSet_20_()
	{
		long[] data = { 0L, 39845888L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_20_ = new BitSet(mk_tokenSet_20_());
	private static long[] mk_tokenSet_21_()
	{
		long[] data = { -2392373581577184L, 8589909215L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_21_ = new BitSet(mk_tokenSet_21_());
	private static long[] mk_tokenSet_22_()
	{
		long[] data = { -2392373581577184L, 8556354783L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_22_ = new BitSet(mk_tokenSet_22_());
	private static long[] mk_tokenSet_23_()
	{
		long[] data = { 4503599627370496L, 67108872L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_23_ = new BitSet(mk_tokenSet_23_());
	private static long[] mk_tokenSet_24_()
	{
		long[] data = { 59109745218289664L, 4159702110L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_24_ = new BitSet(mk_tokenSet_24_());
	private static long[] mk_tokenSet_25_()
	{
		long[] data = { 69242844379873312L, 939523966L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_25_ = new BitSet(mk_tokenSet_25_());
	private static long[] mk_tokenSet_26_()
	{
		long[] data = { -2260703842924816L, 8589934591L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_26_ = new BitSet(mk_tokenSet_26_());
	private static long[] mk_tokenSet_27_()
	{
		long[] data = { -2955323644050400L, 7751048329L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_27_ = new BitSet(mk_tokenSet_27_());
	private static long[] mk_tokenSet_28_()
	{
		long[] data = { 60376382613487648L, 1073741694L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_28_ = new BitSet(mk_tokenSet_28_());
	private static long[] mk_tokenSet_29_()
	{
		long[] data = { -2955323534998496L, 8556354713L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_29_ = new BitSet(mk_tokenSet_29_());
	private static long[] mk_tokenSet_30_()
	{
		long[] data = { 69383581868228640L, 939523966L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_30_ = new BitSet(mk_tokenSet_30_());
	
}
}
