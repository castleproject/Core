// $ANTLR 2.7.5 (20050128): "lang4.g" -> "RookBaseParser.cs"$

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
		public const int IDENT = 28;
		public const int DOT = 29;
		public const int COLONCOLON = 30;
		public const int COLON = 31;
		public const int STATICIDENT = 32;
		public const int INSTIDENT = 33;
		public const int SEMI = 34;
		public const int LITERAL_namespace = 35;
		public const int SL = 36;
		public const int LITERAL_self = 37;
		public const int LITERAL_override = 38;
		public const int LITERAL_abstract = 39;
		public const int LITERAL_new = 40;
		public const int LITERAL_final = 41;
		public const int LITERAL_public = 42;
		public const int LITERAL_private = 43;
		public const int LITERAL_protected = 44;
		public const int LITERAL_internal = 45;
		public const int COMMA = 46;
		public const int ASSIGN = 47;
		public const int LITERAL_initialize = 48;
		public const int LPAREN = 49;
		public const int RPAREN = 50;
		public const int LITERAL_require = 51;
		public const int LTHAN = 52;
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
		public const int LITERAL_if = 63;
		public const int LITERAL_unless = 64;
		public const int LITERAL_until = 65;
		public const int LITERAL_redo = 66;
		public const int LITERAL_break = 67;
		public const int LITERAL_next = 68;
		public const int LITERAL_retry = 69;
		public const int LITERAL_lambda = 70;
		public const int LCURLY = 71;
		public const int RCURLY = 72;
		public const int LITERAL_raise = 73;
		public const int LITERAL_yield = 74;
		public const int BOR = 75;
		public const int LITERAL_or = 76;
		public const int LITERAL_and = 77;
		public const int LITERAL_not = 78;
		public const int LNOT = 79;
		public const int GT = 80;
		public const int EQUAL = 81;
		public const int GE = 82;
		public const int LE = 83;
		public const int NOT_EQUAL = 84;
		public const int LITERAL_is = 85;
		public const int BXOR = 86;
		public const int PLUS = 87;
		public const int MINUS = 88;
		public const int SLASH = 89;
		public const int PERCENT = 90;
		public const int BNOT = 91;
		public const int LBRACK = 92;
		public const int RBRACK = 93;
		public const int LITERAL_base = 94;
		public const int STRING_LITERAL = 95;
		public const int CHAR_LITERAL = 96;
		// "nil?" = 97
		public const int DOTDOT = 98;
		public const int DOTDOTDOT = 99;
		public const int MAPASSIGN = 100;
		public const int QUESTION = 101;
		public const int DIV = 102;
		public const int INC = 103;
		public const int DEC = 104;
		public const int MOD = 105;
		public const int SR = 106;
		public const int SR_ASSIGN = 107;
		public const int BSR = 108;
		public const int BSR_ASSIGN = 109;
		public const int SL_ASSIGN = 110;
		public const int LOR = 111;
		public const int LAND = 112;
		public const int NEWLINE = 113;
		public const int SL_NEWLINE = 114;
		public const int SL_COMMENT = 115;
		public const int WS = 116;
		public const int ESC = 117;
		public const int HEX_DIGIT = 118;
		public const int VOCAB = 119;
		public const int NUMBER = 120;
		public const int Int = 121;
		public const int NonZeroDigit = 122;
		public const int FloatTrailer = 123;
		public const int Exponent = 124;
		public const int CONTINUED_LINE = 125;
		
		
	public IErrorReport ErrorReport;
	
	private bool withinStatic, withinFinal, withinAbstract, withinNew, withinOverride;
	
	private ISymbolTable topLevelScope;

	AccessLevel currentAccessLevel = AccessLevel.Public;
	
	public override void reportError(RecognitionException ex)
	{
		LexicalPosition lpos = new LexicalPosition( ex.getLine(), ex.getColumn() );
		
		ErrorReport.Error( ex.getFilename(), lpos, ex.Message );
	}

	// TODO: Research for a better way to set lexical information
	// on our AST	
	private void SetLexical(IASTNode node, IToken t)
	{
		node.Position.Line = t.getLine();
		node.Position.Column = t.getColumn();
	}

	private Stack scopes = new Stack();
	
	private void PushScope(IASTNode node, ScopeType scopeType)
	{
		if (node.DefiningSymbolTable != null)
		{
			throw new ArgumentException("We can't override a scope");
		}
		
		node.DefiningSymbolTable = new SymbolTable( GetCurrentScope(), scopeType );
		
		ISymbolTable scope = node.DefiningSymbolTable;
		
		if (scope == null) throw new ArgumentNullException("null scope?");
		
		scopes.Push(scope);
	}

	private void PopScope()
	{
		scopes.Pop();
	}
	
	private ISymbolTable GetCurrentScope()
	{
		if (scopes.Count == 0) return topLevelScope;
			
		return scopes.Peek() as ISymbolTable;
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
		
	public String  qualified_name() //throws RecognitionException, TokenStreamException
{
		String ident;
		
		IToken  t = null;
		IToken  t2 = null;
		String name = String.Empty; ident = null;
		
		t = LT(1);
		match(IDENT);
		if (0==inputState.guessing)
		{
			name = t.getText();
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==DOT||LA(1)==COLONCOLON))
				{
					{
						switch ( LA(1) )
						{
						case DOT:
						{
							match(DOT);
							if (0==inputState.guessing)
							{
								name += ".";
							}
							break;
						}
						case COLONCOLON:
						{
							match(COLONCOLON);
							if (0==inputState.guessing)
							{
								name += "::";
							}
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					t2 = LT(1);
					match(IDENT);
					if (0==inputState.guessing)
					{
						name += t2.getText();
					}
				}
				else
				{
					goto _loop4_breakloop;
				}
				
			}
_loop4_breakloop:			;
		}    // ( ... )*
		if (0==inputState.guessing)
		{
			ident = name;
		}
		return ident;
	}
	
	public Identifier  identifier() //throws RecognitionException, TokenStreamException
{
		Identifier ident;
		
		ident = null; TypeReference tr = null;
		
		ident=name();
		{
			switch ( LA(1) )
			{
			case COLON:
			{
				tr=type();
				if (0==inputState.guessing)
				{
					ident.TypeReference = tr;
				}
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
		return ident;
	}
	
	public Identifier  name() //throws RecognitionException, TokenStreamException
{
		Identifier ident;
		
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		ident = null;
		
		switch ( LA(1) )
		{
		case IDENT:
		{
			t1 = LT(1);
			match(IDENT);
			if (0==inputState.guessing)
			{
				ident = new OpaqueIdentifier(t1.getText());
			}
			break;
		}
		case STATICIDENT:
		{
			t2 = LT(1);
			match(STATICIDENT);
			if (0==inputState.guessing)
			{
				ident = new StaticVarIdentifier(t2.getText());
			}
			break;
		}
		case INSTIDENT:
		{
			t3 = LT(1);
			match(INSTIDENT);
			if (0==inputState.guessing)
			{
				ident = new InstanceVarIdentifier(t3.getText());
			}
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
		return ident;
	}
	
	public  TypeReference  type() //throws RecognitionException, TokenStreamException
{
		 TypeReference tr ;
		
		tr = null; String n;
		
		match(COLON);
		n=qualified_name();
		if (0==inputState.guessing)
		{
			
				  tr = new TypeReference(n); 
				
		}
		return tr ;
	}
	
	public Identifier  identifier_withtype() //throws RecognitionException, TokenStreamException
{
		Identifier ident;
		
		ident = null; TypeReference tr = null;
		
		ident=name();
		tr=type();
		if (0==inputState.guessing)
		{
			ident.TypeReference = tr;
		}
		return ident;
	}
	
	protected void statement_term() //throws RecognitionException, TokenStreamException
{
		
		
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
				case EOF:
				{
					match(Token.EOF_TYPE);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
	}
	
	protected void nothing() //throws RecognitionException, TokenStreamException
{
		
		
		{
			if ((LA(1)==EOF||LA(1)==STATEMENT_END) && (tokenSet_0_.member(LA(2))))
			{
				{
					switch ( LA(1) )
					{
					case STATEMENT_END:
					{
						match(STATEMENT_END);
						break;
					}
					case EOF:
					{
						match(Token.EOF_TYPE);
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
			}
			else if ((tokenSet_0_.member(LA(1))) && (tokenSet_1_.member(LA(2)))) {
			}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			
		}
	}
	
	public SourceUnit  sourceUnit(
		CompilationUnit cunit
	) //throws RecognitionException, TokenStreamException
{
		SourceUnit unit;
		
		
			  topLevelScope = cunit.DefiningSymbolTable;
			  unit = new SourceUnit(cunit, getFilename()); 
			  PushScope(unit, ScopeType.SourceUnit); 
			
		
		nothing();
		{
			switch ( LA(1) )
			{
			case LITERAL_namespace:
			{
				namespace_declaration(unit.Namespaces);
				break;
			}
			case EOF:
			case CLASS:
			case DO:
			case DEF:
			case BEGIN:
			case NUM_INT:
			case NUM_FLOAT:
			case NUM_LONG:
			case STATEMENT_END:
			case IDENT:
			case STATICIDENT:
			case INSTIDENT:
			case LITERAL_self:
			case LPAREN:
			case LITERAL_require:
			case LITERAL_redo:
			case LITERAL_break:
			case LITERAL_next:
			case LITERAL_retry:
			case LITERAL_lambda:
			case LCURLY:
			case LITERAL_raise:
			case LITERAL_yield:
			case LITERAL_not:
			case LNOT:
			case PLUS:
			case MINUS:
			case BNOT:
			case LBRACK:
			case LITERAL_base:
			case STRING_LITERAL:
			case CHAR_LITERAL:
			{
				suite(unit.Statements);
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
		if (0==inputState.guessing)
		{
			
				  cunit.SourceUnits.Add(unit); 
				  PopScope(); 
				  if (!ErrorReport.HasErrors && scopes.Count != 0) 
				    ErrorReport.Error("Invalid scope count. " + 
						"Something seems to be very wrong. Contact Castle's team and report the " + 
						"code that caused this error.");  
				
		}
		return unit;
	}
	
	public void namespace_declaration(
		IList namespaces
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  t = null;
		
			  NamespaceDescriptor nsdec = new NamespaceDescriptor(); 
			  PushScope(nsdec, ScopeType.Namespace);
			  namespaces.Add(nsdec); String qn = null;
			  TypeDefinitionStatement typeDef = null;
			
		
		try {      // for error handling
			t = LT(1);
			match(LITERAL_namespace);
			qn=qualified_name();
			statement_term();
			if (0==inputState.guessing)
			{
				nsdec.Name = qn;
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==CLASS))
					{
						typeDef=type_def_statement();
						if (0==inputState.guessing)
						{
							nsdec.TypeDefinitions.Add(typeDef);
						}
					}
					else
					{
						goto _loop22_breakloop;
					}
					
				}
_loop22_breakloop:				;
			}    // ( ... )*
			match(END);
			if (0==inputState.guessing)
			{
				PopScope();
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
	
	public void suite(
		IList stmts
	) //throws RecognitionException, TokenStreamException
{
		
		IStatement stmt = null;
		
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_3_.member(LA(1))))
				{
					stmt=statement();
					if (0==inputState.guessing)
					{
						if (stmt != null) stmts.Add(stmt);
					}
				}
				else
				{
					goto _loop25_breakloop;
				}
				
			}
_loop25_breakloop:			;
		}    // ( ... )*
	}
	
	public TypeDefinitionStatement  type_def_statement() //throws RecognitionException, TokenStreamException
{
		TypeDefinitionStatement tdstmt;
		
		tdstmt = null; currentAccessLevel = AccessLevel.Public;
		
		tdstmt=class_def_statement();
		return tdstmt;
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
			case LITERAL_require:
			{
				stmt=require_statement();
				break;
			}
			default:
				bool synPredMatched39 = false;
				if (((tokenSet_4_.member(LA(1))) && (LA(2)==COLON)))
				{
					int _m39 = mark();
					synPredMatched39 = true;
					inputState.guessing++;
					try {
						{
							declaration_statement();
						}
					}
					catch (RecognitionException)
					{
						synPredMatched39 = false;
					}
					rewind(_m39);
					inputState.guessing--;
				}
				if ( synPredMatched39 )
				{
					stmt=declaration_statement();
				}
				else if ((tokenSet_5_.member(LA(1))) && (tokenSet_6_.member(LA(2)))) {
					stmt=expression_statement();
				}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			break; }
		}
		statement_term();
		return stmt;
	}
	
	public void type_suite(
		IList stmts
	) //throws RecognitionException, TokenStreamException
{
		
		IStatement stmt = null;
		
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_7_.member(LA(1))))
				{
					access_level();
					{
						if ((tokenSet_3_.member(LA(1))) && (tokenSet_8_.member(LA(2))))
						{
							stmt=statement();
							if (0==inputState.guessing)
							{
								if (stmt != null) stmts.Add(stmt);
							}
						}
						else if ((LA(1)==CLASS) && (LA(2)==SL)) {
							method_scope(stmts);
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
				}
				else
				{
					goto _loop29_breakloop;
				}
				
			}
_loop29_breakloop:			;
		}    // ( ... )*
	}
	
	protected void access_level() //throws RecognitionException, TokenStreamException
{
		
		
		{
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
			case CLASS:
			case DO:
			case DEF:
			case BEGIN:
			case NUM_INT:
			case NUM_FLOAT:
			case NUM_LONG:
			case IDENT:
			case STATICIDENT:
			case INSTIDENT:
			case LITERAL_self:
			case LPAREN:
			case LITERAL_require:
			case LITERAL_redo:
			case LITERAL_break:
			case LITERAL_next:
			case LITERAL_retry:
			case LITERAL_lambda:
			case LCURLY:
			case LITERAL_raise:
			case LITERAL_yield:
			case LITERAL_not:
			case LNOT:
			case PLUS:
			case MINUS:
			case BNOT:
			case LBRACK:
			case LITERAL_base:
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
	
	protected void method_scope(
		IList stmts
	) //throws RecognitionException, TokenStreamException
{
		
		IStatement stmt = null; int index=0;
		
		match(CLASS);
		match(SL);
		{
			switch ( LA(1) )
			{
			case LITERAL_self:
			{
				match(LITERAL_self);
				if (0==inputState.guessing)
				{
					withinStatic = true;
				}
				break;
			}
			case LITERAL_override:
			{
				match(LITERAL_override);
				if (0==inputState.guessing)
				{
					index = 1; withinOverride = true;
				}
				break;
			}
			case LITERAL_abstract:
			{
				match(LITERAL_abstract);
				if (0==inputState.guessing)
				{
					index = 2; withinAbstract = true;
				}
				break;
			}
			case LITERAL_new:
			{
				match(LITERAL_new);
				if (0==inputState.guessing)
				{
					index = 3; withinNew = true;
				}
				break;
			}
			case LITERAL_final:
			{
				match(LITERAL_final);
				if (0==inputState.guessing)
				{
					index = 4; withinFinal = true;
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		statement_term();
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_7_.member(LA(1))))
				{
					access_level();
					stmt=statement();
					if (0==inputState.guessing)
					{
						if (stmt != null) stmts.Add(stmt);
					}
				}
				else
				{
					goto _loop33_breakloop;
				}
				
			}
_loop33_breakloop:			;
		}    // ( ... )*
		match(END);
		statement_term();
		if (0==inputState.guessing)
		{
			
				  if (index == 0)
					withinStatic = false;
				  else if (index == 1)
					withinOverride = false;
				  else if (index == 2)
					withinAbstract = false;
				  else if (index == 3)
					withinNew = false;
				  else if (index == 4)
					withinFinal = false;
				
		}
	}
	
	public MultipleVariableDeclarationStatement  declaration_statement() //throws RecognitionException, TokenStreamException
{
		MultipleVariableDeclarationStatement stmt;
		
		
			  stmt = new MultipleVariableDeclarationStatement(currentAccessLevel); 
			  Identifier ident = null;
			  IExpression initExp = null; 
			
		
		ident=identifier_withtype();
		if (0==inputState.guessing)
		{
			stmt.AddIdentifier(ident);
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					ident=identifier_withtype();
					if (0==inputState.guessing)
					{
						stmt.AddIdentifier(ident);
					}
				}
				else
				{
					goto _loop42_breakloop;
				}
				
			}
_loop42_breakloop:			;
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
					stmt.AddInitExp(initExp);
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
								stmt.AddInitExp(initExp);
							}
						}
						else
						{
							goto _loop45_breakloop;
						}
						
					}
_loop45_breakloop:					;
				}    // ( ... )*
				break;
			}
			case EOF:
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
		return stmt;
	}
	
	public IStatement  expression_statement() //throws RecognitionException, TokenStreamException
{
		IStatement stmt;
		
		stmt = null; PostfixCondition pfc = null; IExpression exp = null; IExpression rhs = null;
			  AugType rel = AugType.Undefined;
		
		if ((tokenSet_9_.member(LA(1))))
		{
			{
				switch ( LA(1) )
				{
				case DO:
				case BEGIN:
				{
					exp=compound();
					break;
				}
				case NUM_INT:
				case NUM_FLOAT:
				case NUM_LONG:
				case IDENT:
				case STATICIDENT:
				case INSTIDENT:
				case LITERAL_self:
				case LPAREN:
				case LITERAL_lambda:
				case LCURLY:
				case LITERAL_raise:
				case LITERAL_yield:
				case LITERAL_not:
				case LNOT:
				case PLUS:
				case MINUS:
				case BNOT:
				case LBRACK:
				case LITERAL_base:
				case STRING_LITERAL:
				case CHAR_LITERAL:
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
								int _cnt75=0;
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
										if (_cnt75 >= 1) { goto _loop75_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
									}
									
									_cnt75++;
								}
_loop75_breakloop:								;
							}    // ( ... )+
							break;
						}
						case EOF:
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
					break;
				}
				case LITERAL_redo:
				case LITERAL_break:
				case LITERAL_next:
				case LITERAL_retry:
				{
					exp=flow_expressions();
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
				case WHILE:
				case LITERAL_if:
				case LITERAL_unless:
				case LITERAL_until:
				{
					pfc=postFixCondition();
					if (0==inputState.guessing)
					{
						exp.PostfixCondition = pfc;
					}
					break;
				}
				case EOF:
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
		}
		else if ((LA(1)==DEF) && (tokenSet_10_.member(LA(2)))) {
			stmt=method_def_statement();
		}
		else if ((LA(1)==DEF) && (LA(2)==LITERAL_initialize)) {
			stmt=constructor_def_statement();
		}
		else
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		
		return stmt;
	}
	
	public RequireDirectiveStatement  require_statement() //throws RecognitionException, TokenStreamException
{
		RequireDirectiveStatement rd;
		
		rd = null; String ident = null;
		
		match(LITERAL_require);
		ident=qualified_name();
		if (0==inputState.guessing)
		{
			rd = new RequireDirectiveStatement(ident);
		}
		return rd;
	}
	
	public IExpression  test() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null; IExpression rhs = null;
		
		switch ( LA(1) )
		{
		case NUM_INT:
		case NUM_FLOAT:
		case NUM_LONG:
		case IDENT:
		case STATICIDENT:
		case INSTIDENT:
		case LITERAL_self:
		case LPAREN:
		case LCURLY:
		case LITERAL_not:
		case LNOT:
		case PLUS:
		case MINUS:
		case BNOT:
		case LBRACK:
		case LITERAL_base:
		case STRING_LITERAL:
		case CHAR_LITERAL:
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
						goto _loop100_breakloop;
					}
					
				}
_loop100_breakloop:				;
			}    // ( ... )*
			break;
		}
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
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
		return exp;
	}
	
	public ConstructorDefinitionStatement  constructor_def_statement() //throws RecognitionException, TokenStreamException
{
		ConstructorDefinitionStatement mdstmt;
		
		
			  mdstmt = new ConstructorDefinitionStatement( currentAccessLevel ); 
			
		
		match(DEF);
		match(LITERAL_initialize);
		{
			switch ( LA(1) )
			{
			case LPAREN:
			{
				match(LPAREN);
				{
					switch ( LA(1) )
					{
					case IDENT:
					case STATICIDENT:
					case INSTIDENT:
					case STAR:
					case BAND:
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
				break;
			}
			case EOF:
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
	
	public void methodParams(
		MethodDefinitionStatement mdstmt
	) //throws RecognitionException, TokenStreamException
{
		
		ParameterVarIdentifier param = null;
		
		param=methodParam();
		if (0==inputState.guessing)
		{
			mdstmt.AddFormalParameter( param );
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					param=methodParam();
					if (0==inputState.guessing)
					{
						mdstmt.AddFormalParameter( param );
					}
				}
				else
				{
					goto _loop63_breakloop;
				}
				
			}
_loop63_breakloop:			;
		}    // ( ... )*
	}
	
	public MethodDefinitionStatement  method_def_statement() //throws RecognitionException, TokenStreamException
{
		MethodDefinitionStatement mdstmt;
		
		
			  mdstmt = new MethodDefinitionStatement( currentAccessLevel ); 
			  String qn = null; TypeReference retType = null; 
			
		
		match(DEF);
		modifier(mdstmt);
		qn=qualified_name();
		if (0==inputState.guessing)
		{
			
				    if (qn.StartsWith("self."))
				    {
				      mdstmt.IsStatic = true;
				      mdstmt.Name = qn.Substring( "self.".Length );
				    }
				    else
				    {
					  mdstmt.Name = qn;
				    }
					PushScope(mdstmt, ScopeType.Method); 
				
		}
		{
			switch ( LA(1) )
			{
			case LPAREN:
			{
				match(LPAREN);
				{
					switch ( LA(1) )
					{
					case IDENT:
					case STATICIDENT:
					case INSTIDENT:
					case STAR:
					case BAND:
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
					case COLON:
					{
						retType=type();
						break;
					}
					case EOF:
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
			case EOF:
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
		if (0==inputState.guessing)
		{
			PopScope();
		}
		return mdstmt;
	}
	
	protected void modifier(
		MethodDefinitionStatement mdstmt
	) //throws RecognitionException, TokenStreamException
{
		
		
		switch ( LA(1) )
		{
		case LITERAL_override:
		{
			match(LITERAL_override);
			if (0==inputState.guessing)
			{
				mdstmt.IsOverride = true;
			}
			break;
		}
		case LITERAL_new:
		{
			match(LITERAL_new);
			if (0==inputState.guessing)
			{
				mdstmt.IsNewSlot  = true;
			}
			break;
		}
		case LITERAL_abstract:
		{
			match(LITERAL_abstract);
			if (0==inputState.guessing)
			{
				mdstmt.IsAbstract = true;
			}
			break;
		}
		case LITERAL_final:
		{
			match(LITERAL_final);
			if (0==inputState.guessing)
			{
				mdstmt.IsFinal    = true;
			}
			break;
		}
		default:
			if (((LA(1)==IDENT) && (tokenSet_11_.member(LA(2))))&&( withinOverride ))
			{
				if (0==inputState.guessing)
				{
					mdstmt.IsOverride = true;
				}
			}
			else if (((LA(1)==IDENT) && (tokenSet_11_.member(LA(2))))&&( withinNew )) {
				if (0==inputState.guessing)
				{
					mdstmt.IsNewSlot  = true;
				}
			}
			else if (((LA(1)==IDENT) && (tokenSet_11_.member(LA(2))))&&( withinAbstract )) {
				if (0==inputState.guessing)
				{
					mdstmt.IsAbstract = true;
				}
			}
			else if (((LA(1)==IDENT) && (tokenSet_11_.member(LA(2))))&&( withinFinal )) {
				if (0==inputState.guessing)
				{
					mdstmt.IsFinal    = true;
				}
			}
			else if (((LA(1)==IDENT) && (tokenSet_11_.member(LA(2))))&&( withinStatic )) {
				if (0==inputState.guessing)
				{
					mdstmt.IsStatic   = true;
				}
			}
			else if ((LA(1)==IDENT) && (tokenSet_11_.member(LA(2)))) {
			}
		else
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		break; }
	}
	
	public TypeDefinitionStatement  class_def_statement() //throws RecognitionException, TokenStreamException
{
		TypeDefinitionStatement tdstmt;
		
		IToken  t = null;
		tdstmt = null; String qn = null;
		
		match(CLASS);
		t = LT(1);
		match(IDENT);
		if (0==inputState.guessing)
		{
			tdstmt = new TypeDefinitionStatement( currentAccessLevel, t.getText() );
				  PushScope(tdstmt, ScopeType.Type); 
				
		}
		{
			switch ( LA(1) )
			{
			case SL:
			case LTHAN:
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
				qn=qualified_name();
				if (0==inputState.guessing)
				{
					tdstmt.BaseTypes.Add( new TypeReference(qn) );
				}
				{    // ( ... )*
					for (;;)
					{
						if ((LA(1)==COMMA))
						{
							match(COMMA);
							qn=qualified_name();
							if (0==inputState.guessing)
							{
								tdstmt.BaseTypes.Add( new TypeReference(qn) );
							}
						}
						else
						{
							goto _loop60_breakloop;
						}
						
					}
_loop60_breakloop:					;
				}    // ( ... )*
				break;
			}
			case EOF:
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
		type_suite(tdstmt.Statements);
		match(END);
		if (0==inputState.guessing)
		{
			PopScope();
		}
		return tdstmt;
	}
	
	public ParameterVarIdentifier  methodParam() //throws RecognitionException, TokenStreamException
{
		ParameterVarIdentifier param;
		
		IExpression exp = null; Identifier ident = null; param = null;
		
		{
			switch ( LA(1) )
			{
			case IDENT:
			case STATICIDENT:
			case INSTIDENT:
			{
				ident=identifier();
				if (0==inputState.guessing)
				{
					param = ParameterVarIdentifier.FromIdentifier(ParameterType.Ordinary, ident);
				}
				break;
			}
			case BAND:
			{
				match(BAND);
				ident=identifier();
				if (0==inputState.guessing)
				{
					param = ParameterVarIdentifier.FromIdentifier(ParameterType.Block, ident);
				}
				break;
			}
			default:
				bool synPredMatched67 = false;
				if (((LA(1)==STAR) && (tokenSet_4_.member(LA(2)))))
				{
					int _m67 = mark();
					synPredMatched67 = true;
					inputState.guessing++;
					try {
						{
							match(STAR);
							identifier_withtype();
						}
					}
					catch (RecognitionException)
					{
						synPredMatched67 = false;
					}
					rewind(_m67);
					inputState.guessing--;
				}
				if ( synPredMatched67 )
				{
					match(STAR);
					ident=identifier_withtype();
					if (0==inputState.guessing)
					{
						param = ParameterVarIdentifier.FromIdentifier(ParameterType.Params, ident);
					}
				}
				else if ((LA(1)==STAR) && (tokenSet_4_.member(LA(2)))) {
					match(STAR);
					ident=identifier();
					if (0==inputState.guessing)
					{
						param = ParameterVarIdentifier.FromIdentifier(ParameterType.List, ident);
					}
				}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			break; }
		}
		{
			switch ( LA(1) )
			{
			case ASSIGN:
			{
				match(ASSIGN);
				exp=expression();
				if (0==inputState.guessing)
				{
					param.InitExpression = exp;
				}
				break;
			}
			case COMMA:
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
		return param;
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
					goto _loop115_breakloop;
				}
				
			}
_loop115_breakloop:			;
		}    // ( ... )*
		return exp;
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
		if (0==inputState.guessing)
		{
			PushScope(cexp, ScopeType.Compound);
		}
		statement_term();
		suite(cexp.Statements);
		match(END);
		if (0==inputState.guessing)
		{
			PopScope();
		}
		return cexp;
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
		
		if (0==inputState.guessing)
		{
			PushScope(bexp, ScopeType.Block);
		}
		{
			switch ( LA(1) )
			{
			case DO:
			{
				{
					match(DO);
					nothing();
					{
						switch ( LA(1) )
						{
						case BOR:
						{
							blockargs(bexp);
							break;
						}
						case EOF:
						case CLASS:
						case DO:
						case END:
						case DEF:
						case BEGIN:
						case NUM_INT:
						case NUM_FLOAT:
						case NUM_LONG:
						case STATEMENT_END:
						case IDENT:
						case STATICIDENT:
						case INSTIDENT:
						case SEMI:
						case LITERAL_self:
						case LPAREN:
						case LITERAL_require:
						case LITERAL_redo:
						case LITERAL_break:
						case LITERAL_next:
						case LITERAL_retry:
						case LITERAL_lambda:
						case LCURLY:
						case LITERAL_raise:
						case LITERAL_yield:
						case LITERAL_not:
						case LNOT:
						case PLUS:
						case MINUS:
						case BNOT:
						case LBRACK:
						case LITERAL_base:
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
						case EOF:
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
						case NUM_INT:
						case NUM_FLOAT:
						case NUM_LONG:
						case IDENT:
						case STATICIDENT:
						case INSTIDENT:
						case LITERAL_self:
						case LPAREN:
						case LITERAL_require:
						case LITERAL_redo:
						case LITERAL_break:
						case LITERAL_next:
						case LITERAL_retry:
						case LITERAL_lambda:
						case LCURLY:
						case LITERAL_raise:
						case LITERAL_yield:
						case LITERAL_not:
						case LNOT:
						case PLUS:
						case MINUS:
						case BNOT:
						case LBRACK:
						case LITERAL_base:
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
					nothing();
					{
						switch ( LA(1) )
						{
						case BOR:
						{
							blockargs(bexp);
							break;
						}
						case EOF:
						case CLASS:
						case DO:
						case DEF:
						case BEGIN:
						case NUM_INT:
						case NUM_FLOAT:
						case NUM_LONG:
						case STATEMENT_END:
						case IDENT:
						case STATICIDENT:
						case INSTIDENT:
						case SEMI:
						case LITERAL_self:
						case LPAREN:
						case LITERAL_require:
						case LITERAL_redo:
						case LITERAL_break:
						case LITERAL_next:
						case LITERAL_retry:
						case LITERAL_lambda:
						case LCURLY:
						case RCURLY:
						case LITERAL_raise:
						case LITERAL_yield:
						case LITERAL_not:
						case LNOT:
						case PLUS:
						case MINUS:
						case BNOT:
						case LBRACK:
						case LITERAL_base:
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
						case EOF:
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
						case NUM_INT:
						case NUM_FLOAT:
						case NUM_LONG:
						case IDENT:
						case STATICIDENT:
						case INSTIDENT:
						case LITERAL_self:
						case LPAREN:
						case LITERAL_require:
						case LITERAL_redo:
						case LITERAL_break:
						case LITERAL_next:
						case LITERAL_retry:
						case LITERAL_lambda:
						case LCURLY:
						case RCURLY:
						case LITERAL_raise:
						case LITERAL_yield:
						case LITERAL_not:
						case LNOT:
						case PLUS:
						case MINUS:
						case BNOT:
						case LBRACK:
						case LITERAL_base:
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
		if (0==inputState.guessing)
		{
			PopScope();
		}
		return bexp;
	}
	
	public void blockargs(
		BlockExpression bexp
	) //throws RecognitionException, TokenStreamException
{
		
		ParameterVarIdentifier ident = null;
		
		match(BOR);
		ident=methodParam();
		if (0==inputState.guessing)
		{
			bexp.AddBlockFormalParameter(ident);
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					ident=methodParam();
					if (0==inputState.guessing)
					{
						bexp.AddBlockFormalParameter(ident);
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
		
		rexp = new YieldExpression();
		
		match(LITERAL_yield);
		expressionList(rexp.ExpColl);
		return rexp;
	}
	
	public void expressionList(
		IList expColl
	) //throws RecognitionException, TokenStreamException
{
		
		IExpression exp = null;
		
		exp=expression();
		if (0==inputState.guessing)
		{
			expColl.Add(exp);
		}
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA) && (tokenSet_12_.member(LA(2))))
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
					goto _loop112_breakloop;
				}
				
			}
_loop112_breakloop:			;
		}    // ( ... )*
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
					goto _loop103_breakloop;
				}
				
			}
_loop103_breakloop:			;
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
		case NUM_INT:
		case NUM_FLOAT:
		case NUM_LONG:
		case IDENT:
		case STATICIDENT:
		case INSTIDENT:
		case LITERAL_self:
		case LPAREN:
		case LCURLY:
		case PLUS:
		case MINUS:
		case BNOT:
		case LBRACK:
		case LITERAL_base:
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
				if ((tokenSet_13_.member(LA(1))))
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
					goto _loop108_breakloop;
				}
				
			}
_loop108_breakloop:			;
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
		case LITERAL_is:
		{
			match(LITERAL_is);
			if (0==inputState.guessing)
			{
				op = BinaryOp.IsA;
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
				if ((LA(1)==BOR) && (tokenSet_12_.member(LA(2))))
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
					goto _loop118_breakloop;
				}
				
			}
_loop118_breakloop:			;
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
					goto _loop121_breakloop;
				}
				
			}
_loop121_breakloop:			;
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
					goto _loop125_breakloop;
				}
				
			}
_loop125_breakloop:			;
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
				if ((tokenSet_14_.member(LA(1))))
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
					goto _loop129_breakloop;
				}
				
			}
_loop129_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public IExpression  unary() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null; IExpression inner = null; UnaryOp op = UnaryOp.Plus;
		
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
			break;
		}
		case NUM_INT:
		case NUM_FLOAT:
		case NUM_LONG:
		case IDENT:
		case STATICIDENT:
		case INSTIDENT:
		case LITERAL_self:
		case LPAREN:
		case LCURLY:
		case LBRACK:
		case LITERAL_base:
		case STRING_LITERAL:
		case CHAR_LITERAL:
		{
			exp=primary();
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
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
				if ((tokenSet_15_.member(LA(1))))
				{
					exp=trailer(exp);
				}
				else
				{
					goto _loop134_breakloop;
				}
				
			}
_loop134_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public IExpression  atom() //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		exp = null;
		
		switch ( LA(1) )
		{
		case LBRACK:
		{
			match(LBRACK);
			{
				switch ( LA(1) )
				{
				case NUM_INT:
				case NUM_FLOAT:
				case NUM_LONG:
				case IDENT:
				case STATICIDENT:
				case INSTIDENT:
				case LITERAL_self:
				case LPAREN:
				case LITERAL_lambda:
				case LCURLY:
				case LITERAL_raise:
				case LITERAL_yield:
				case LITERAL_not:
				case LNOT:
				case PLUS:
				case MINUS:
				case BNOT:
				case LBRACK:
				case LITERAL_base:
				case STRING_LITERAL:
				case CHAR_LITERAL:
				{
					exp=listmaker();
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
				case STATICIDENT:
				case INSTIDENT:
				case LITERAL_self:
				case LPAREN:
				case LCURLY:
				case PLUS:
				case MINUS:
				case BNOT:
				case LBRACK:
				case LITERAL_base:
				case STRING_LITERAL:
				case CHAR_LITERAL:
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
		case NUM_INT:
		case NUM_FLOAT:
		case NUM_LONG:
		case STRING_LITERAL:
		case CHAR_LITERAL:
		{
			exp=constantref();
			break;
		}
		case LITERAL_self:
		{
			match(LITERAL_self);
			if (0==inputState.guessing)
			{
				exp = SelfReferenceExpression.Instance;
			}
			break;
		}
		case LITERAL_base:
		{
			match(LITERAL_base);
			if (0==inputState.guessing)
			{
				exp = BaseReferenceExpression.Instance;
			}
			break;
		}
		default:
			bool synPredMatched137 = false;
			if (((LA(1)==LPAREN) && (tokenSet_12_.member(LA(2)))))
			{
				int _m137 = mark();
				synPredMatched137 = true;
				inputState.guessing++;
				try {
					{
						range();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched137 = false;
				}
				rewind(_m137);
				inputState.guessing--;
			}
			if ( synPredMatched137 )
			{
				exp=range();
			}
			else if ((LA(1)==LPAREN) && (tokenSet_16_.member(LA(2)))) {
				match(LPAREN);
				{
					switch ( LA(1) )
					{
					case NUM_INT:
					case NUM_FLOAT:
					case NUM_LONG:
					case IDENT:
					case STATICIDENT:
					case INSTIDENT:
					case LITERAL_self:
					case LPAREN:
					case LITERAL_lambda:
					case LCURLY:
					case LITERAL_raise:
					case LITERAL_yield:
					case LITERAL_not:
					case LNOT:
					case PLUS:
					case MINUS:
					case BNOT:
					case LBRACK:
					case LITERAL_base:
					case STRING_LITERAL:
					case CHAR_LITERAL:
					{
						exp=test();
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
		return exp;
	}
	
	public IExpression  trailer(
		IExpression inner
	) //throws RecognitionException, TokenStreamException
{
		IExpression exp;
		
		IToken  t1 = null;
		IToken  t = null;
		exp = null; BlockExpression temp = null; String qp = String.Empty;
		
		switch ( LA(1) )
		{
		case LPAREN:
		{
			match(LPAREN);
			if (0==inputState.guessing)
			{
				exp = new MethodInvocationExpression(inner);
			}
			{
				switch ( LA(1) )
				{
				case NUM_INT:
				case NUM_FLOAT:
				case NUM_LONG:
				case IDENT:
				case STATICIDENT:
				case INSTIDENT:
				case LITERAL_self:
				case LPAREN:
				case LITERAL_lambda:
				case LCURLY:
				case LITERAL_raise:
				case LITERAL_yield:
				case LITERAL_not:
				case LNOT:
				case PLUS:
				case MINUS:
				case BNOT:
				case LBRACK:
				case LITERAL_base:
				case STRING_LITERAL:
				case CHAR_LITERAL:
				{
					arglist((exp as MethodInvocationExpression).Arguments);
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
		case DO:
		case LCURLY:
		{
			temp=block();
			if (0==inputState.guessing)
			{
				
					  exp = inner;
					  exp.Block = temp;
					
			}
			break;
		}
		case COLONCOLON:
		{
			match(COLONCOLON);
			t1 = LT(1);
			match(IDENT);
			if (0==inputState.guessing)
			{
				exp = new MemberAccessExpression(inner, t1.getText());
			}
			break;
		}
		default:
			if ((LA(1)==DOT) && (LA(2)==97))
			{
				match(DOT);
				match(97);
				if (0==inputState.guessing)
				{
					exp = new NullCheckExpression(inner);
				}
			}
			else if ((LA(1)==DOT) && (LA(2)==IDENT)) {
				match(DOT);
				t = LT(1);
				match(IDENT);
				if (0==inputState.guessing)
				{
					exp = new MemberAccessExpression(inner, t.getText());
				}
			}
		else
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		break; }
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
					goto _loop153_breakloop;
				}
				
			}
_loop153_breakloop:			;
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
			exp.Add(key, value);
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
						exp.Add(key, value);
					}
				}
				else
				{
					goto _loop156_breakloop;
				}
				
			}
_loop156_breakloop:			;
		}    // ( ... )*
		return exp;
	}
	
	public VariableReferenceExpression  varref() //throws RecognitionException, TokenStreamException
{
		VariableReferenceExpression vre;
		
		Identifier ident = null; vre = null;
		
		ident=name();
		if (0==inputState.guessing)
		{
			vre = new VariableReferenceExpression(ident);
		}
		return vre;
	}
	
	public ConstExpression  constantref() //throws RecognitionException, TokenStreamException
{
		ConstExpression lre;
		
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		IToken  t5 = null;
		IToken  t6 = null;
		lre = null;
		
		switch ( LA(1) )
		{
		case NUM_INT:
		{
			t1 = LT(1);
			match(NUM_INT);
			if (0==inputState.guessing)
			{
				lre = new ConstExpression(t1.getText(), ConstExpressionType.IntLiteral);
			}
			break;
		}
		case NUM_LONG:
		{
			t2 = LT(1);
			match(NUM_LONG);
			if (0==inputState.guessing)
			{
				lre = new ConstExpression(t2.getText(), ConstExpressionType.LongLiteral);
			}
			break;
		}
		case NUM_FLOAT:
		{
			t3 = LT(1);
			match(NUM_FLOAT);
			if (0==inputState.guessing)
			{
				lre = new ConstExpression(t3.getText(), ConstExpressionType.FloatLiteral);
			}
			break;
		}
		case STRING_LITERAL:
		{
			t5 = LT(1);
			match(STRING_LITERAL);
			if (0==inputState.guessing)
			{
				lre = new ConstExpression(t5.getText(), ConstExpressionType.StringLiteral);
			}
			break;
		}
		case CHAR_LITERAL:
		{
			t6 = LT(1);
			match(CHAR_LITERAL);
			if (0==inputState.guessing)
			{
				lre = new ConstExpression(t6.getText(), ConstExpressionType.CharLiteral);
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
	
	public void arglist(
		IList expcoll
	) //throws RecognitionException, TokenStreamException
{
		
		IExpression exp;
		
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
					goto _loop149_breakloop;
				}
				
			}
_loop149_breakloop:			;
		}    // ( ... )*
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
		@"""IDENT""",
		@"""DOT""",
		@"""COLONCOLON""",
		@"""COLON""",
		@"""STATICIDENT""",
		@"""INSTIDENT""",
		@"""SEMI""",
		@"""namespace""",
		@"""SL""",
		@"""self""",
		@"""override""",
		@"""abstract""",
		@"""new""",
		@"""final""",
		@"""public""",
		@"""private""",
		@"""protected""",
		@"""internal""",
		@"""COMMA""",
		@"""ASSIGN""",
		@"""initialize""",
		@"""LPAREN""",
		@"""RPAREN""",
		@"""require""",
		@"""LTHAN""",
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
		@"""if""",
		@"""unless""",
		@"""until""",
		@"""redo""",
		@"""break""",
		@"""next""",
		@"""retry""",
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
		@"""is""",
		@"""BXOR""",
		@"""PLUS""",
		@"""MINUS""",
		@"""SLASH""",
		@"""PERCENT""",
		@"""BNOT""",
		@"""LBRACK""",
		@"""RBRACK""",
		@"""base""",
		@"""STRING_LITERAL""",
		@"""CHAR_LITERAL""",
		@"""nil?""",
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
		long[] data = { 2814952142275314L, 7944065020L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { -66073802045710L, 128849018879L, 0L, 0L};
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
		long[] data = { 2814900468449968L, 7944062716L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { 13153337344L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { 563100654764704L, 7944062716L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { -2388244507392990L, 8589934531L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { 2880871166116528L, 7944062716L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { -2388242359909342L, 8589934531L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { 563100654764576L, 7944062716L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { 4123437039616L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { 562968878120962L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { 563100654764032L, 7944011904L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	private static long[] mk_tokenSet_13_()
	{
		long[] data = { 4503599627370496L, 4128768L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
	private static long[] mk_tokenSet_14_()
	{
		long[] data = { 9007199254740992L, 100663296L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
	private static long[] mk_tokenSet_15_()
	{
		long[] data = { 562951564034080L, 128L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
	private static long[] mk_tokenSet_16_()
	{
		long[] data = { 1689000561606656L, 7944062656L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_16_ = new BitSet(mk_tokenSet_16_());
	
}
}
