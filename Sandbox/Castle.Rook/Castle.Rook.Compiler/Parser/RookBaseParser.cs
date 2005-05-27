// $ANTLR 2.7.5 (20050128): "lang4.g" -> "RookBaseParser.cs"$

	// using CommonAST					= antlr.CommonAST; 
	using System.Text;
	using System.Collections;
	using Castle.Rook.Compiler.AST;

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
		public const int UNTIL = 29;
		public const int LITERAL_for = 30;
		public const int IDENT = 31;
		public const int COMMA = 32;
		public const int LITERAL_in = 33;
		public const int LITERAL_if = 34;
		public const int LITERAL_then = 35;
		public const int LITERAL_elsif = 36;
		public const int LITERAL_else = 37;
		public const int LITERAL_unless = 38;
		public const int LITERAL_redo = 39;
		public const int LITERAL_break = 40;
		public const int LITERAL_next = 41;
		public const int LITERAL_retry = 42;
		public const int LITERAL_public = 43;
		public const int COLON = 44;
		public const int LITERAL_private = 45;
		public const int LITERAL_protected = 46;
		public const int LITERAL_internal = 47;
		public const int LT = 48;
		public const int SL = 49;
		public const int LPAREN = 50;
		public const int RPAREN = 51;
		public const int ASSIGN = 52;
		public const int STAR = 53;
		public const int BAND = 54;
		public const int PLUS_ASSIGN = 55;
		public const int MINUS_ASSIGN = 56;
		public const int STAR_ASSIGN = 57;
		public const int DIV_ASSIGN = 58;
		public const int MOD_ASSIGN = 59;
		public const int BAND_ASSIGN = 60;
		public const int BOR_ASSIGN = 61;
		public const int BXOR_ASSIGN = 62;
		public const int LITERAL_until = 63;
		public const int DOT = 64;
		public const int SYMBOL = 65;
		public const int LITERAL_or = 66;
		public const int LITERAL_lambda = 67;
		public const int LCURLY = 68;
		public const int RCURLY = 69;
		public const int LITERAL_raise = 70;
		public const int BOR = 71;
		public const int LITERAL_and = 72;
		public const int LITERAL_not = 73;
		public const int GT = 74;
		public const int EQUAL = 75;
		public const int GE = 76;
		public const int LE = 77;
		public const int NOT_EQUAL = 78;
		public const int BXOR = 79;
		public const int PLUS = 80;
		public const int MINUS = 81;
		public const int SLASH = 82;
		public const int PERCENT = 83;
		public const int BNOT = 84;
		public const int LBRACK = 85;
		public const int RBRACK = 86;
		public const int STRING_LITERAL = 87;
		public const int CHAR_LITERAL = 88;
		public const int DOTDOT = 89;
		public const int DOTDOTDOT = 90;
		public const int MAPASSIGN = 91;
		public const int QUESTION = 92;
		public const int LNOT = 93;
		public const int DIV = 94;
		public const int INC = 95;
		public const int DEC = 96;
		public const int MOD = 97;
		public const int SR = 98;
		public const int SR_ASSIGN = 99;
		public const int BSR = 100;
		public const int BSR_ASSIGN = 101;
		public const int SL_ASSIGN = 102;
		public const int LOR = 103;
		public const int LAND = 104;
		public const int NEWLINE = 105;
		public const int SL_COMMENT = 106;
		public const int WS = 107;
		public const int ESC = 108;
		public const int HEX_DIGIT = 109;
		public const int VOCAB = 110;
		public const int NUMBER = 111;
		public const int Int = 112;
		public const int NonZeroDigit = 113;
		public const int FloatTrailer = 114;
		public const int Exponent = 115;
		public const int CONTINUED_LINE = 116;
		
		
	AccessLevel currentAccessLevel = AccessLevel.Public;
	
	public override void reportError(RecognitionException ex)
	{
		throw ex;
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
		suite(comp.Statements);
		nothing();
		match(Token.EOF_TYPE);
		return comp;
	}
	
	public void suite(
		IList stmts
	) //throws RecognitionException, TokenStreamException
{
		
		IStatement stmt = null;
		
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_2_.member(LA(1))))
				{
					stmt=statement();
					if (0==inputState.guessing)
					{
						if (stmt != null) stmts.Add(stmt);
					}
				}
				else
				{
					goto _loop7_breakloop;
				}
				
			}
_loop7_breakloop:			;
		}    // ( ... )*
	}
	
	public IStatement  statement() //throws RecognitionException, TokenStreamException
{
		IStatement stmt;
		
		stmt = null;
		
		{
			switch ( LA(1) )
			{
			case CLASS:
			{
				stmt=type_def_statement();
				break;
			}
			case WHILE:
			{
				stmt=while_statement();
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
			case LITERAL_redo:
			case LITERAL_break:
			case LITERAL_next:
			case LITERAL_retry:
			{
				flow_statements();
				break;
			}
			case LITERAL_if:
			{
				stmt=if_statement();
				break;
			}
			case LITERAL_unless:
			{
				stmt=unless_statement();
				break;
			}
			case DO:
			case DEF:
			case BEGIN:
			case NUM_INT:
			case NUM_FLOAT:
			case NUM_LONG:
			case IDENT:
			case LPAREN:
			case SYMBOL:
			case LITERAL_lambda:
			case LCURLY:
			case LITERAL_raise:
			case LITERAL_not:
			case PLUS:
			case MINUS:
			case BNOT:
			case LBRACK:
			case STRING_LITERAL:
			case CHAR_LITERAL:
			{
				expression_statement();
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		statement_term();
		return stmt;
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
		
		rs = new RepeatStatement(RepeatType.While);
		
		match(WHILE);
		test();
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
		suite(rs.Statements);
		match(END);
		return rs;
	}
	
	public RepeatStatement  until_statement() //throws RecognitionException, TokenStreamException
{
		RepeatStatement rs;
		
		rs = new RepeatStatement(RepeatType.Until);
		
		match(UNTIL);
		test();
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
		statement_term();
		suite(rs.Statements);
		match(END);
		return rs;
	}
	
	public ForStatement  for_statement() //throws RecognitionException, TokenStreamException
{
		ForStatement fors;
		
		fors = new ForStatement();
		
		match(LITERAL_for);
		match(IDENT);
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					match(IDENT);
				}
				else
				{
					goto _loop18_breakloop;
				}
				
			}
_loop18_breakloop:			;
		}    // ( ... )*
		match(LITERAL_in);
		test();
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
	
	public void flow_statements() //throws RecognitionException, TokenStreamException
{
		
		
		switch ( LA(1) )
		{
		case LITERAL_redo:
		{
			match(LITERAL_redo);
			break;
		}
		case LITERAL_break:
		{
			match(LITERAL_break);
			break;
		}
		case LITERAL_next:
		{
			match(LITERAL_next);
			break;
		}
		case LITERAL_retry:
		{
			match(LITERAL_retry);
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
	}
	
	public IfStatement  if_statement() //throws RecognitionException, TokenStreamException
{
		IfStatement ifs;
		
		ifs = new IfStatement(IfType.If);
		
		match(LITERAL_if);
		test();
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
		suite(ifs.TrueStatements);
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==LITERAL_elsif))
				{
					match(LITERAL_elsif);
					test();
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
					suite(ifs.TrueStatements);
				}
				else
				{
					goto _loop24_breakloop;
				}
				
			}
_loop24_breakloop:			;
		}    // ( ... )*
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
	
	public IfStatement  unless_statement() //throws RecognitionException, TokenStreamException
{
		IfStatement ifs;
		
		ifs = new IfStatement(IfType.Unless);
		
		match(LITERAL_unless);
		test();
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
		
		stmt = null; PostfixCondition pfc = null;
		
		switch ( LA(1) )
		{
		case DO:
		case BEGIN:
		case NUM_INT:
		case NUM_FLOAT:
		case NUM_LONG:
		case IDENT:
		case LPAREN:
		case SYMBOL:
		case LITERAL_lambda:
		case LCURLY:
		case LITERAL_raise:
		case LITERAL_not:
		case PLUS:
		case MINUS:
		case BNOT:
		case LBRACK:
		case STRING_LITERAL:
		case CHAR_LITERAL:
		{
			{
				if ((tokenSet_3_.member(LA(1))) && (tokenSet_4_.member(LA(2))))
				{
					test();
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
							augassign();
							test();
							break;
						}
						case ASSIGN:
						{
							{ // ( ... )+
								int _cnt54=0;
								for (;;)
								{
									if ((LA(1)==ASSIGN))
									{
										match(ASSIGN);
										test();
									}
									else
									{
										if (_cnt54 >= 1) { goto _loop54_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
									}
									
									_cnt54++;
								}
_loop54_breakloop:								;
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
					compound();
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
	
	public void test() //throws RecognitionException, TokenStreamException
{
		
		
		switch ( LA(1) )
		{
		case LITERAL_lambda:
		{
			lambda();
			break;
		}
		case LITERAL_raise:
		{
			raise();
			break;
		}
		default:
			if ((tokenSet_5_.member(LA(1))) && (tokenSet_6_.member(LA(2))))
			{
				and_test();
				{    // ( ... )*
					for (;;)
					{
						if ((LA(1)==LITERAL_or))
						{
							match(LITERAL_or);
							and_test();
						}
						else
						{
							goto _loop74_breakloop;
						}
						
					}
_loop74_breakloop:					;
				}    // ( ... )*
			}
			else if ((LA(1)==DO||LA(1)==LCURLY) && (tokenSet_7_.member(LA(2)))) {
				block();
			}
		else
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		break; }
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
					case EOF:
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
					case EOF:
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
					case EOF:
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
					case EOF:
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
			case EOF:
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
	
	public TypeDefinitionStatement  class_def_statement() //throws RecognitionException, TokenStreamException
{
		TypeDefinitionStatement tdstmt;
		
		tdstmt = new TypeDefinitionStatement();
		
		match(CLASS);
		match(IDENT);
		{
			switch ( LA(1) )
			{
			case LT:
			case SL:
			{
				{
					switch ( LA(1) )
					{
					case LT:
					{
						match(LT);
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
							goto _loop41_breakloop;
						}
						
					}
_loop41_breakloop:					;
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
	
	public void qualified_name() //throws RecognitionException, TokenStreamException
{
		
		
		match(IDENT);
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==DOT))
				{
					match(DOT);
					match(IDENT);
				}
				else
				{
					goto _loop65_breakloop;
				}
				
			}
_loop65_breakloop:			;
		}    // ( ... )*
	}
	
	public MethodDefinitionStatement  method_def_statement() //throws RecognitionException, TokenStreamException
{
		MethodDefinitionStatement mdstmt;
		
		mdstmt = new MethodDefinitionStatement();
		
		match(DEF);
		qualified_name();
		match(LPAREN);
		{
			switch ( LA(1) )
			{
			case IDENT:
			case STAR:
			case BAND:
			{
				methodParams();
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
				type();
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
		suite(mdstmt.Statements);
		match(END);
		return mdstmt;
	}
	
	public void methodParams() //throws RecognitionException, TokenStreamException
{
		
		
		methodParam();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					methodParam();
				}
				else
				{
					goto _loop47_breakloop;
				}
				
			}
_loop47_breakloop:			;
		}    // ( ... )*
	}
	
	public void type() //throws RecognitionException, TokenStreamException
{
		
		
		qualified_symbol();
	}
	
	public void methodParam() //throws RecognitionException, TokenStreamException
{
		
		
		switch ( LA(1) )
		{
		case IDENT:
		{
			type_name();
			{
				switch ( LA(1) )
				{
				case ASSIGN:
				{
					match(ASSIGN);
					expression();
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
			match(IDENT);
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
	
	public void type_name() //throws RecognitionException, TokenStreamException
{
		
		
		match(IDENT);
		{
			switch ( LA(1) )
			{
			case SYMBOL:
			{
				type();
				break;
			}
			case COMMA:
			case RPAREN:
			case ASSIGN:
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
	
	public void expression() //throws RecognitionException, TokenStreamException
{
		
		
		xor_expr();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==BXOR))
				{
					match(BXOR);
					xor_expr();
				}
				else
				{
					goto _loop105_breakloop;
				}
				
			}
_loop105_breakloop:			;
		}    // ( ... )*
	}
	
	public void augassign() //throws RecognitionException, TokenStreamException
{
		
		
		switch ( LA(1) )
		{
		case PLUS_ASSIGN:
		{
			match(PLUS_ASSIGN);
			break;
		}
		case MINUS_ASSIGN:
		{
			match(MINUS_ASSIGN);
			break;
		}
		case STAR_ASSIGN:
		{
			match(STAR_ASSIGN);
			break;
		}
		case DIV_ASSIGN:
		{
			match(DIV_ASSIGN);
			break;
		}
		case MOD_ASSIGN:
		{
			match(MOD_ASSIGN);
			break;
		}
		case BAND_ASSIGN:
		{
			match(BAND_ASSIGN);
			break;
		}
		case BOR_ASSIGN:
		{
			match(BOR_ASSIGN);
			break;
		}
		case BXOR_ASSIGN:
		{
			match(BXOR_ASSIGN);
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
	}
	
	public CompoundStatement  compound() //throws RecognitionException, TokenStreamException
{
		CompoundStatement cstmt;
		
		cstmt = new CompoundStatement();
		
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
		suite(cstmt.Statements);
		match(END);
		return cstmt;
	}
	
	public PostfixCondition  postFixCondition() //throws RecognitionException, TokenStreamException
{
		PostfixCondition pfc;
		
		pfc = null;
		
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
			test();
		}
		return pfc;
	}
	
	public void qualified_symbol() //throws RecognitionException, TokenStreamException
{
		
		
		match(SYMBOL);
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==DOT))
				{
					match(DOT);
					match(IDENT);
				}
				else
				{
					goto _loop68_breakloop;
				}
				
			}
_loop68_breakloop:			;
		}    // ( ... )*
	}
	
	public void testlist() //throws RecognitionException, TokenStreamException
{
		
		
		test();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					test();
				}
				else
				{
					goto _loop71_breakloop;
				}
				
			}
_loop71_breakloop:			;
		}    // ( ... )*
	}
	
	public void and_test() //throws RecognitionException, TokenStreamException
{
		
		
		not_test();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==LITERAL_and))
				{
					match(LITERAL_and);
					not_test();
				}
				else
				{
					goto _loop94_breakloop;
				}
				
			}
_loop94_breakloop:			;
		}    // ( ... )*
	}
	
	public LambdaExpression  lambda() //throws RecognitionException, TokenStreamException
{
		LambdaExpression lexp;
		
		BlockExpression bexp=null;
			  lexp = null;
		
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
						if ((LA(1)==STATEMENT_END||LA(1)==SEMI) && (tokenSet_8_.member(LA(2))))
						{
							statement_term();
						}
						else if ((tokenSet_8_.member(LA(1))) && (tokenSet_9_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					{
						switch ( LA(1) )
						{
						case BOR:
						{
							blockargs(bexp);
							break;
						}
						case CLASS:
						case DO:
						case END:
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
						case IDENT:
						case LITERAL_if:
						case LITERAL_unless:
						case LITERAL_redo:
						case LITERAL_break:
						case LITERAL_next:
						case LITERAL_retry:
						case LPAREN:
						case SYMBOL:
						case LITERAL_lambda:
						case LCURLY:
						case LITERAL_raise:
						case LITERAL_not:
						case PLUS:
						case MINUS:
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
					{
						switch ( LA(1) )
						{
						case STATEMENT_END:
						case SEMI:
						{
							statement_term();
							break;
						}
						case CLASS:
						case DO:
						case END:
						case DEF:
						case BEGIN:
						case WHILE:
						case NUM_INT:
						case NUM_FLOAT:
						case NUM_LONG:
						case UNTIL:
						case LITERAL_for:
						case IDENT:
						case LITERAL_if:
						case LITERAL_unless:
						case LITERAL_redo:
						case LITERAL_break:
						case LITERAL_next:
						case LITERAL_retry:
						case LPAREN:
						case SYMBOL:
						case LITERAL_lambda:
						case LCURLY:
						case LITERAL_raise:
						case LITERAL_not:
						case PLUS:
						case MINUS:
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
						if ((LA(1)==STATEMENT_END||LA(1)==SEMI) && (tokenSet_10_.member(LA(2))))
						{
							statement_term();
						}
						else if ((tokenSet_10_.member(LA(1))) && (tokenSet_9_.member(LA(2)))) {
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
					{
						switch ( LA(1) )
						{
						case BOR:
						{
							blockargs(bexp);
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
						case IDENT:
						case LITERAL_if:
						case LITERAL_unless:
						case LITERAL_redo:
						case LITERAL_break:
						case LITERAL_next:
						case LITERAL_retry:
						case LPAREN:
						case SYMBOL:
						case LITERAL_lambda:
						case LCURLY:
						case RCURLY:
						case LITERAL_raise:
						case LITERAL_not:
						case PLUS:
						case MINUS:
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
					{
						switch ( LA(1) )
						{
						case STATEMENT_END:
						case SEMI:
						{
							statement_term();
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
						case UNTIL:
						case LITERAL_for:
						case IDENT:
						case LITERAL_if:
						case LITERAL_unless:
						case LITERAL_redo:
						case LITERAL_break:
						case LITERAL_next:
						case LITERAL_retry:
						case LPAREN:
						case SYMBOL:
						case LITERAL_lambda:
						case LCURLY:
						case RCURLY:
						case LITERAL_raise:
						case LITERAL_not:
						case PLUS:
						case MINUS:
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
	
	public void raise() //throws RecognitionException, TokenStreamException
{
		
		
		match(LITERAL_raise);
		expression();
	}
	
	public void blockargs(
		BlockExpression bexp
	) //throws RecognitionException, TokenStreamException
{
		
		
		match(BOR);
		match(IDENT);
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					match(IDENT);
				}
				else
				{
					goto _loop89_breakloop;
				}
				
			}
_loop89_breakloop:			;
		}    // ( ... )*
		match(BOR);
	}
	
	public void not_test() //throws RecognitionException, TokenStreamException
{
		
		
		switch ( LA(1) )
		{
		case LITERAL_not:
		{
			match(LITERAL_not);
			not_test();
			break;
		}
		case NUM_INT:
		case NUM_FLOAT:
		case NUM_LONG:
		case IDENT:
		case LPAREN:
		case SYMBOL:
		case LCURLY:
		case PLUS:
		case MINUS:
		case BNOT:
		case LBRACK:
		case STRING_LITERAL:
		case CHAR_LITERAL:
		{
			comparison();
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
	}
	
	public void comparison() //throws RecognitionException, TokenStreamException
{
		
		
		expression();
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_11_.member(LA(1))))
				{
					comp_op();
					expression();
				}
				else
				{
					goto _loop98_breakloop;
				}
				
			}
_loop98_breakloop:			;
		}    // ( ... )*
	}
	
	public void comp_op() //throws RecognitionException, TokenStreamException
{
		
		
		switch ( LA(1) )
		{
		case LT:
		{
			match(LT);
			break;
		}
		case GT:
		{
			match(GT);
			break;
		}
		case EQUAL:
		{
			match(EQUAL);
			break;
		}
		case GE:
		{
			match(GE);
			break;
		}
		case LE:
		{
			match(LE);
			break;
		}
		case NOT_EQUAL:
		{
			match(NOT_EQUAL);
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
	}
	
	public void expressionList() //throws RecognitionException, TokenStreamException
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
					goto _loop102_breakloop;
				}
				
			}
_loop102_breakloop:			;
		}    // ( ... )*
	}
	
	public void xor_expr() //throws RecognitionException, TokenStreamException
{
		
		
		and_expr();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==BOR))
				{
					match(BOR);
					and_expr();
				}
				else
				{
					goto _loop108_breakloop;
				}
				
			}
_loop108_breakloop:			;
		}    // ( ... )*
	}
	
	public void and_expr() //throws RecognitionException, TokenStreamException
{
		
		
		arith_expr();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==BAND))
				{
					match(BAND);
					arith_expr();
				}
				else
				{
					goto _loop111_breakloop;
				}
				
			}
_loop111_breakloop:			;
		}    // ( ... )*
	}
	
	public void arith_expr() //throws RecognitionException, TokenStreamException
{
		
		
		term();
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
					term();
				}
				else
				{
					goto _loop115_breakloop;
				}
				
			}
_loop115_breakloop:			;
		}    // ( ... )*
	}
	
	public void term() //throws RecognitionException, TokenStreamException
{
		
		
		factor();
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_12_.member(LA(1))))
				{
					{
						switch ( LA(1) )
						{
						case STAR:
						{
							match(STAR);
							break;
						}
						case SLASH:
						{
							match(SLASH);
							break;
						}
						case PERCENT:
						{
							match(PERCENT);
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					factor();
				}
				else
				{
					goto _loop119_breakloop;
				}
				
			}
_loop119_breakloop:			;
		}    // ( ... )*
	}
	
	public void factor() //throws RecognitionException, TokenStreamException
{
		
		
		switch ( LA(1) )
		{
		case PLUS:
		case MINUS:
		case BNOT:
		{
			{
				switch ( LA(1) )
				{
				case PLUS:
				{
					match(PLUS);
					break;
				}
				case MINUS:
				{
					match(MINUS);
					break;
				}
				case BNOT:
				{
					match(BNOT);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			factor();
			break;
		}
		case NUM_INT:
		case NUM_FLOAT:
		case NUM_LONG:
		case IDENT:
		case LPAREN:
		case SYMBOL:
		case LCURLY:
		case LBRACK:
		case STRING_LITERAL:
		case CHAR_LITERAL:
		{
			primary();
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
	}
	
	public void primary() //throws RecognitionException, TokenStreamException
{
		
		
		atom();
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_13_.member(LA(1))))
				{
					trailer();
				}
				else
				{
					goto _loop124_breakloop;
				}
				
			}
_loop124_breakloop:			;
		}    // ( ... )*
	}
	
	public void atom() //throws RecognitionException, TokenStreamException
{
		
		
		switch ( LA(1) )
		{
		case LBRACK:
		{
			match(LBRACK);
			{
				switch ( LA(1) )
				{
				case DO:
				case NUM_INT:
				case NUM_FLOAT:
				case NUM_LONG:
				case IDENT:
				case LPAREN:
				case SYMBOL:
				case LITERAL_lambda:
				case LCURLY:
				case LITERAL_raise:
				case LITERAL_not:
				case PLUS:
				case MINUS:
				case BNOT:
				case LBRACK:
				case STRING_LITERAL:
				case CHAR_LITERAL:
				{
					listmaker();
					break;
				}
				case RBRACK:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RBRACK);
			break;
		}
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
				case SYMBOL:
				case LCURLY:
				case PLUS:
				case MINUS:
				case BNOT:
				case LBRACK:
				case STRING_LITERAL:
				case CHAR_LITERAL:
				{
					dictmaker();
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
		{
			match(IDENT);
			break;
		}
		case NUM_INT:
		{
			match(NUM_INT);
			break;
		}
		case NUM_LONG:
		{
			match(NUM_LONG);
			break;
		}
		case NUM_FLOAT:
		{
			match(NUM_FLOAT);
			break;
		}
		case SYMBOL:
		{
			match(SYMBOL);
			break;
		}
		case STRING_LITERAL:
		{
			match(STRING_LITERAL);
			break;
		}
		case CHAR_LITERAL:
		{
			match(CHAR_LITERAL);
			break;
		}
		default:
			bool synPredMatched127 = false;
			if (((LA(1)==LPAREN) && (tokenSet_14_.member(LA(2)))))
			{
				int _m127 = mark();
				synPredMatched127 = true;
				inputState.guessing++;
				try {
					{
						interval();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched127 = false;
				}
				rewind(_m127);
				inputState.guessing--;
			}
			if ( synPredMatched127 )
			{
				interval();
			}
			else if ((LA(1)==LPAREN) && (tokenSet_15_.member(LA(2)))) {
				match(LPAREN);
				{
					switch ( LA(1) )
					{
					case DO:
					case NUM_INT:
					case NUM_FLOAT:
					case NUM_LONG:
					case IDENT:
					case LPAREN:
					case SYMBOL:
					case LITERAL_lambda:
					case LCURLY:
					case LITERAL_raise:
					case LITERAL_not:
					case PLUS:
					case MINUS:
					case BNOT:
					case LBRACK:
					case STRING_LITERAL:
					case CHAR_LITERAL:
					{
						testlist();
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
		else
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		break; }
	}
	
	public void trailer() //throws RecognitionException, TokenStreamException
{
		
		
		switch ( LA(1) )
		{
		case LPAREN:
		{
			match(LPAREN);
			{
				switch ( LA(1) )
				{
				case DO:
				case NUM_INT:
				case NUM_FLOAT:
				case NUM_LONG:
				case IDENT:
				case LPAREN:
				case SYMBOL:
				case LITERAL_lambda:
				case LCURLY:
				case LITERAL_raise:
				case LITERAL_not:
				case PLUS:
				case MINUS:
				case BNOT:
				case LBRACK:
				case STRING_LITERAL:
				case CHAR_LITERAL:
				{
					arglist();
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
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
	}
	
	public void interval() //throws RecognitionException, TokenStreamException
{
		
		
		match(LPAREN);
		expression();
		{
			switch ( LA(1) )
			{
			case DOTDOT:
			{
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
		expression();
		match(RPAREN);
	}
	
	public void listmaker() //throws RecognitionException, TokenStreamException
{
		
		
		test();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					test();
				}
				else
				{
					goto _loop145_breakloop;
				}
				
			}
_loop145_breakloop:			;
		}    // ( ... )*
	}
	
	public void dictmaker() //throws RecognitionException, TokenStreamException
{
		
		
		expression();
		match(MAPASSIGN);
		test();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					expression();
					match(MAPASSIGN);
					test();
				}
				else
				{
					goto _loop148_breakloop;
				}
				
			}
_loop148_breakloop:			;
		}    // ( ... )*
	}
	
	public void arglist() //throws RecognitionException, TokenStreamException
{
		
		
		argument();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					argument();
				}
				else
				{
					goto _loop141_breakloop;
				}
				
			}
_loop141_breakloop:			;
		}    // ( ... )*
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
					goto _loop137_breakloop;
				}
				
			}
_loop137_breakloop:			;
		}    // ( ... )*
	}
	
	public void subscript() //throws RecognitionException, TokenStreamException
{
		
		
		expression();
	}
	
	public void argument() //throws RecognitionException, TokenStreamException
{
		
		
		test();
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
		@"""UNTIL""",
		@"""for""",
		@"""IDENT""",
		@"""COMMA""",
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
		@"""LT""",
		@"""SL""",
		@"""LPAREN""",
		@"""RPAREN""",
		@"""ASSIGN""",
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
		@"""DOT""",
		@"""SYMBOL""",
		@"""or""",
		@"""lambda""",
		@"""LCURLY""",
		@"""RCURLY""",
		@"""raise""",
		@"""BOR""",
		@"""and""",
		@"""not""",
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
		@"""LNOT""",
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
		long[] data = { 1134442303194802L, 28508762L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { -835882265344270L, 33554431L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { 1134442168977072L, 28508762L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 1125902163378208L, 28508762L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { -835882265344272L, 33554431L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { 1125902163378176L, 28508690L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { -844091558460384L, 33554431L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { 1134442571630320L, 28508922L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { 1134442571630320L, 28508890L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { -835843610638608L, 33554431L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { 1134442571630256L, 28508922L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { 281474976710656L, 31744L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { 9007199254740992L, 786432L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	private static long[] mk_tokenSet_13_()
	{
		long[] data = { 1125899906842624L, 2097153L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
	private static long[] mk_tokenSet_14_()
	{
		long[] data = { 1125902163378176L, 28508178L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
	private static long[] mk_tokenSet_15_()
	{
		long[] data = { 3377701977063456L, 28508762L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
	
}
}
