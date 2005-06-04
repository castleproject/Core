// $ANTLR 2.7.5 (20050128): "lang4.g" -> "RookLexer.cs"$

	// using CommonAST					= antlr.CommonAST; 
	using System.Text;
	using System.Collections;
	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Services;

namespace Castle.Rook.Compiler.Parser
{
	// Generate header specific to lexer CSharp file
	using System;
	using Stream                          = System.IO.Stream;
	using TextReader                      = System.IO.TextReader;
	using Hashtable                       = System.Collections.Hashtable;
	using Comparer                        = System.Collections.Comparer;
	
	using TokenStreamException            = antlr.TokenStreamException;
	using TokenStreamIOException          = antlr.TokenStreamIOException;
	using TokenStreamRecognitionException = antlr.TokenStreamRecognitionException;
	using CharStreamException             = antlr.CharStreamException;
	using CharStreamIOException           = antlr.CharStreamIOException;
	using ANTLRException                  = antlr.ANTLRException;
	using CharScanner                     = antlr.CharScanner;
	using InputBuffer                     = antlr.InputBuffer;
	using ByteBuffer                      = antlr.ByteBuffer;
	using CharBuffer                      = antlr.CharBuffer;
	using Token                           = antlr.Token;
	using IToken                          = antlr.IToken;
	using CommonToken                     = antlr.CommonToken;
	using SemanticException               = antlr.SemanticException;
	using RecognitionException            = antlr.RecognitionException;
	using NoViableAltForCharException     = antlr.NoViableAltForCharException;
	using MismatchedCharException         = antlr.MismatchedCharException;
	using TokenStream                     = antlr.TokenStream;
	using LexerSharedInputState           = antlr.LexerSharedInputState;
	using BitSet                          = antlr.collections.impl.BitSet;
	
	public 	class RookLexer : antlr.CharScanner	, TokenStream
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
		public const int LITERAL_return = 42;
		public const int LITERAL_require = 43;
		public const int LITERAL_public = 44;
		public const int LITERAL_private = 45;
		public const int LITERAL_protected = 46;
		public const int LITERAL_internal = 47;
		public const int COMMA = 48;
		public const int ASSIGN = 49;
		public const int IDENT = 50;
		public const int LTHAN = 51;
		public const int SL = 52;
		public const int LPAREN = 53;
		public const int RPAREN = 54;
		public const int STAR = 55;
		public const int BAND = 56;
		public const int PLUS_ASSIGN = 57;
		public const int MINUS_ASSIGN = 58;
		public const int STAR_ASSIGN = 59;
		public const int DIV_ASSIGN = 60;
		public const int MOD_ASSIGN = 61;
		public const int BAND_ASSIGN = 62;
		public const int BOR_ASSIGN = 63;
		public const int BXOR_ASSIGN = 64;
		public const int LITERAL_until = 65;
		public const int STATICIDENT = 66;
		public const int INSTIDENT = 67;
		public const int DOT = 68;
		public const int COLONCOLON = 69;
		public const int SYMBOL = 70;
		public const int LITERAL_lambda = 71;
		public const int LCURLY = 72;
		public const int RCURLY = 73;
		public const int LITERAL_raise = 74;
		public const int LITERAL_yield = 75;
		public const int BOR = 76;
		public const int LITERAL_or = 77;
		public const int LITERAL_and = 78;
		public const int LITERAL_not = 79;
		public const int LNOT = 80;
		public const int GT = 81;
		public const int EQUAL = 82;
		public const int GE = 83;
		public const int LE = 84;
		public const int NOT_EQUAL = 85;
		public const int BXOR = 86;
		public const int PLUS = 87;
		public const int MINUS = 88;
		public const int SLASH = 89;
		public const int PERCENT = 90;
		public const int BNOT = 91;
		public const int LBRACK = 92;
		public const int RBRACK = 93;
		public const int STRING_LITERAL = 94;
		public const int CHAR_LITERAL = 95;
		public const int DOTDOT = 96;
		public const int DOTDOTDOT = 97;
		public const int MAPASSIGN = 98;
		public const int QUESTION = 99;
		public const int DIV = 100;
		public const int INC = 101;
		public const int DEC = 102;
		public const int MOD = 103;
		public const int SR = 104;
		public const int SR_ASSIGN = 105;
		public const int BSR = 106;
		public const int BSR_ASSIGN = 107;
		public const int SL_ASSIGN = 108;
		public const int LOR = 109;
		public const int LAND = 110;
		public const int COLON = 111;
		public const int NEWLINE = 112;
		public const int SL_NEWLINE = 113;
		public const int SL_COMMENT = 114;
		public const int WS = 115;
		public const int ESC = 116;
		public const int HEX_DIGIT = 117;
		public const int VOCAB = 118;
		public const int NUMBER = 119;
		public const int Int = 120;
		public const int NonZeroDigit = 121;
		public const int FloatTrailer = 122;
		public const int Exponent = 123;
		public const int CONTINUED_LINE = 124;
		
		
    private int lastToken = 0;

    private int getProperType() 
    {
        int result;
        if (lastToken == STATEMENT_END || lastToken == SEMI ) 
        {
            result = Token.SKIP;
        } 
        else 
        {
            result = STATEMENT_END;
        }
        return result;
    }

	protected internal override IToken makeToken(int type) {
		lastToken = type;
		return base.makeToken(type);
	}
		public RookLexer(Stream ins) : this(new ByteBuffer(ins))
		{
		}
		
		public RookLexer(TextReader r) : this(new CharBuffer(r))
		{
		}
		
		public RookLexer(InputBuffer ib)		 : this(new LexerSharedInputState(ib))
		{
		}
		
		public RookLexer(LexerSharedInputState state) : base(state)
		{
			initialize();
		}
		private void initialize()
		{
			caseSensitiveLiterals = true;
			setCaseSensitive(true);
			literals = new Hashtable(100, (float) 0.4, null, Comparer.Default);
			literals.Add("raise", 74);
			literals.Add("for", 31);
			literals.Add("class", 4);
			literals.Add("redo", 38);
			literals.Add("internal", 47);
			literals.Add("require", 43);
			literals.Add("and", 78);
			literals.Add("def", 7);
			literals.Add("elsif", 35);
			literals.Add("retry", 41);
			literals.Add("private", 45);
			literals.Add("end", 6);
			literals.Add("next", 40);
			literals.Add("until", 65);
			literals.Add("begin", 9);
			literals.Add("do", 5);
			literals.Add("in", 32);
			literals.Add("namespace", 29);
			literals.Add("operator", 8);
			literals.Add("lambda", 71);
			literals.Add("while", 10);
			literals.Add("or", 77);
			literals.Add("protected", 46);
			literals.Add("break", 39);
			literals.Add("return", 42);
			literals.Add("if", 33);
			literals.Add("yield", 75);
			literals.Add("public", 44);
			literals.Add("else", 36);
			literals.Add("unless", 37);
			literals.Add("not", 79);
			literals.Add("then", 34);
		}
		
		override public IToken nextToken()			//throws TokenStreamException
		{
			IToken theRetToken = null;
tryAgain:
			for (;;)
			{
				IToken _token = null;
				int _ttype = Token.INVALID_TYPE;
				resetText();
				try     // for char stream error handling
				{
					try     // for lexical error handling
					{
						switch ( cached_LA1 )
						{
						case '?':
						{
							mQUESTION(true);
							theRetToken = returnToken_;
							break;
						}
						case '(':
						{
							mLPAREN(true);
							theRetToken = returnToken_;
							break;
						}
						case ')':
						{
							mRPAREN(true);
							theRetToken = returnToken_;
							break;
						}
						case '[':
						{
							mLBRACK(true);
							theRetToken = returnToken_;
							break;
						}
						case ']':
						{
							mRBRACK(true);
							theRetToken = returnToken_;
							break;
						}
						case '{':
						{
							mLCURLY(true);
							theRetToken = returnToken_;
							break;
						}
						case '}':
						{
							mRCURLY(true);
							theRetToken = returnToken_;
							break;
						}
						case ',':
						{
							mCOMMA(true);
							theRetToken = returnToken_;
							break;
						}
						case '~':
						{
							mBNOT(true);
							theRetToken = returnToken_;
							break;
						}
						case ';':
						{
							mSEMI(true);
							theRetToken = returnToken_;
							break;
						}
						case '\n':  case '\r':
						{
							mNEWLINE(true);
							theRetToken = returnToken_;
							break;
						}
						case '\t':  case '\u000c':  case ' ':
						{
							mWS(true);
							theRetToken = returnToken_;
							break;
						}
						case '#':
						{
							mSL_COMMENT(true);
							theRetToken = returnToken_;
							break;
						}
						case '\'':
						{
							mCHAR_LITERAL(true);
							theRetToken = returnToken_;
							break;
						}
						case '"':
						{
							mSTRING_LITERAL(true);
							theRetToken = returnToken_;
							break;
						}
						case '$':  case 'A':  case 'B':  case 'C':
						case 'D':  case 'E':  case 'F':  case 'G':
						case 'H':  case 'I':  case 'J':  case 'K':
						case 'L':  case 'M':  case 'N':  case 'O':
						case 'P':  case 'Q':  case 'R':  case 'S':
						case 'T':  case 'U':  case 'V':  case 'W':
						case 'X':  case 'Y':  case 'Z':  case '_':
						case 'a':  case 'b':  case 'c':  case 'd':
						case 'e':  case 'f':  case 'g':  case 'h':
						case 'i':  case 'j':  case 'k':  case 'l':
						case 'm':  case 'n':  case 'o':  case 'p':
						case 'q':  case 'r':  case 's':  case 't':
						case 'u':  case 'v':  case 'w':  case 'x':
						case 'y':  case 'z':
						{
							mIDENT(true);
							theRetToken = returnToken_;
							break;
						}
						case '0':  case '1':  case '2':  case '3':
						case '4':  case '5':  case '6':  case '7':
						case '8':  case '9':
						{
							mNUMBER(true);
							theRetToken = returnToken_;
							break;
						}
						case '\\':
						{
							mCONTINUED_LINE(true);
							theRetToken = returnToken_;
							break;
						}
						default:
							if ((cached_LA1=='>') && (cached_LA2=='>') && (LA(3)=='>') && (LA(4)=='='))
							{
								mBSR_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='.') && (cached_LA2=='.') && (LA(3)=='.')) {
								mDOTDOTDOT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='>') && (cached_LA2=='>') && (LA(3)=='=')) {
								mSR_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='>') && (cached_LA2=='>') && (LA(3)=='>') && (true)) {
								mBSR(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='<') && (cached_LA2=='<') && (LA(3)=='=')) {
								mSL_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='.') && (cached_LA2=='.') && (true)) {
								mDOTDOT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='=') && (cached_LA2=='=')) {
								mEQUAL(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='!') && (cached_LA2=='=')) {
								mNOT_EQUAL(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='/') && (cached_LA2=='=')) {
								mDIV_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='+') && (cached_LA2=='=')) {
								mPLUS_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='+') && (cached_LA2=='+')) {
								mINC(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='-') && (cached_LA2=='=')) {
								mMINUS_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='-') && (cached_LA2=='-')) {
								mDEC(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='*') && (cached_LA2=='=')) {
								mSTAR_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='%') && (cached_LA2=='=')) {
								mMOD_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='>') && (cached_LA2=='>') && (true)) {
								mSR(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='>') && (cached_LA2=='=')) {
								mGE(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='<') && (cached_LA2=='<') && (true)) {
								mSL(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='<') && (cached_LA2=='=')) {
								mLE(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='^') && (cached_LA2=='=')) {
								mBXOR_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='|') && (cached_LA2=='=')) {
								mBOR_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='|') && (cached_LA2=='|')) {
								mLOR(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='&') && (cached_LA2=='=')) {
								mBAND_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='&') && (cached_LA2=='&')) {
								mLAND(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='=') && (cached_LA2=='>')) {
								mMAPASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1==':') && (cached_LA2==':')) {
								mCOLONCOLON(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='@') && (tokenSet_0_.member(cached_LA2))) {
								mINSTIDENT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='@') && (cached_LA2=='@')) {
								mSTATICIDENT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1==':') && (tokenSet_0_.member(cached_LA2))) {
								mSYMBOL(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='.') && (true)) {
								mDOT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='=') && (true)) {
								mASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='!') && (true)) {
								mLNOT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='/') && (true)) {
								mDIV(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='+') && (true)) {
								mPLUS(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='-') && (true)) {
								mMINUS(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='*') && (true)) {
								mSTAR(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='%') && (true)) {
								mMOD(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='>') && (true)) {
								mGT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='<') && (true)) {
								mLTHAN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='^') && (true)) {
								mBXOR(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='|') && (true)) {
								mBOR(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='&') && (true)) {
								mBAND(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1==':') && (true)) {
								mCOLON(true);
								theRetToken = returnToken_;
							}
						else
						{
							if (cached_LA1==EOF_CHAR) { uponEOF(); returnToken_ = makeToken(Token.EOF_TYPE); }
				else {throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());}
						}
						break; }
						if ( null==returnToken_ ) goto tryAgain; // found SKIP token
						_ttype = returnToken_.Type;
						returnToken_.Type = _ttype;
						return returnToken_;
					}
					catch (RecognitionException e) {
							throw new TokenStreamRecognitionException(e);
					}
				}
				catch (CharStreamException cse) {
					if ( cse is CharStreamIOException ) {
						throw new TokenStreamIOException(((CharStreamIOException)cse).io);
					}
					else {
						throw new TokenStreamException(cse.Message);
					}
				}
			}
		}
		
	public void mQUESTION(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = QUESTION;
		
		match('?');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLPAREN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LPAREN;
		
		match('(');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mRPAREN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RPAREN;
		
		match(')');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLBRACK(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LBRACK;
		
		match('[');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mRBRACK(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RBRACK;
		
		match(']');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLCURLY(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LCURLY;
		
		match('{');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mRCURLY(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RCURLY;
		
		match('}');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCOMMA(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = COMMA;
		
		match(',');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mDOT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DOT;
		
		match('.');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mDOTDOT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DOTDOT;
		
		match("..");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mDOTDOTDOT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DOTDOTDOT;
		
		match("...");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ASSIGN;
		
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mEQUAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = EQUAL;
		
		match("==");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLNOT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LNOT;
		
		match('!');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mBNOT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = BNOT;
		
		match('~');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mNOT_EQUAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NOT_EQUAL;
		
		match("!=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mDIV(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DIV;
		
		match('/');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mDIV_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DIV_ASSIGN;
		
		match("/=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mPLUS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = PLUS;
		
		match('+');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mPLUS_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = PLUS_ASSIGN;
		
		match("+=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mINC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = INC;
		
		match("++");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mMINUS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = MINUS;
		
		match('-');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mMINUS_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = MINUS_ASSIGN;
		
		match("-=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mDEC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DEC;
		
		match("--");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSTAR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = STAR;
		
		match('*');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSTAR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = STAR_ASSIGN;
		
		match("*=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mMOD(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = MOD;
		
		match('%');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mMOD_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = MOD_ASSIGN;
		
		match("%=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SR;
		
		match(">>");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SR_ASSIGN;
		
		match(">>=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mBSR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = BSR;
		
		match(">>>");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mBSR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = BSR_ASSIGN;
		
		match(">>>=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mGE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = GE;
		
		match(">=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mGT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = GT;
		
		match(">");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SL;
		
		match("<<");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSL_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SL_ASSIGN;
		
		match("<<=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LE;
		
		match("<=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLTHAN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LTHAN;
		
		match('<');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mBXOR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = BXOR;
		
		match('^');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mBXOR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = BXOR_ASSIGN;
		
		match("^=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mBOR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = BOR;
		
		match('|');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mBOR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = BOR_ASSIGN;
		
		match("|=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLOR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LOR;
		
		match("||");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mBAND(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = BAND;
		
		match('&');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mBAND_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = BAND_ASSIGN;
		
		match("&=");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLAND(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LAND;
		
		match("&&");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSEMI(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SEMI;
		
		match(';');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mMAPASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = MAPASSIGN;
		
		match("=>");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCOLON(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = COLON;
		
		match(':');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCOLONCOLON(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = COLONCOLON;
		
		match("::");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mNEWLINE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NEWLINE;
		
		mSL_NEWLINE(false);
		if (0==inputState.guessing)
		{
			
					newline();
					_ttype = getProperType();
				
		}
		{
			if ((cached_LA1=='\t'||cached_LA1=='\n'||cached_LA1=='\u000c'||cached_LA1=='\r'||cached_LA1==' '||cached_LA1=='#'))
			{
				{ // ( ... )+
					int _cnt224=0;
					for (;;)
					{
						switch ( cached_LA1 )
						{
						case '\n':  case '\r':
						{
							mSL_NEWLINE(false);
							break;
						}
						case '\t':  case '\u000c':  case ' ':
						{
							mWS(false);
							break;
						}
						case '#':
						{
							mSL_COMMENT(false);
							break;
						}
						default:
						{
							if (_cnt224 >= 1) { goto _loop224_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
						}
						break; }
						_cnt224++;
					}
_loop224_breakloop:					;
				}    // ( ... )+
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mSL_NEWLINE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SL_NEWLINE;
		
		{
			if ((cached_LA1=='\r') && (cached_LA2=='\n') && (true) && (true))
			{
				match("\r\n");
			}
			else if ((cached_LA1=='\r') && (true) && (true) && (true)) {
				match('\r');
			}
			else if ((cached_LA1=='\n')) {
				match('\n');
			}
			else
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mWS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = WS;
		
		{ // ( ... )+
			int _cnt233=0;
			for (;;)
			{
				if ((cached_LA1==' ') && (true) && (true) && (true))
				{
					match(' ');
				}
				else if ((cached_LA1=='\t') && (true) && (true) && (true)) {
					match('\t');
				}
				else if ((cached_LA1=='\u000c') && (true) && (true) && (true)) {
					match('\f');
				}
				else
				{
					if (_cnt233 >= 1) { goto _loop233_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				
				_cnt233++;
			}
_loop233_breakloop:			;
		}    // ( ... )+
		if (0==inputState.guessing)
		{
			_ttype = Token.SKIP;
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSL_COMMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SL_COMMENT;
		
		match('#');
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_1_.member(cached_LA1)) && (true) && (true) && (true))
				{
					{
						match(tokenSet_1_);
					}
				}
				else
				{
					goto _loop230_breakloop;
				}
				
			}
_loop230_breakloop:			;
		}    // ( ... )*
		if (0==inputState.guessing)
		{
			_ttype = Token.SKIP;
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCHAR_LITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = CHAR_LITERAL;
		
		match('\'');
		{
			if ((cached_LA1=='\\'))
			{
				mESC(false);
			}
			else if ((tokenSet_2_.member(cached_LA1))) {
				matchNot('\'');
			}
			else
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			
		}
		match('\'');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mESC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ESC;
		
		match('\\');
		{
			switch ( cached_LA1 )
			{
			case 'n':
			{
				match('n');
				break;
			}
			case 'r':
			{
				match('r');
				break;
			}
			case 't':
			{
				match('t');
				break;
			}
			case 'b':
			{
				match('b');
				break;
			}
			case 'f':
			{
				match('f');
				break;
			}
			case '"':
			{
				match('"');
				break;
			}
			case '\'':
			{
				match('\'');
				break;
			}
			case '\\':
			{
				match('\\');
				break;
			}
			case 'u':
			{
				{ // ( ... )+
					int _cnt243=0;
					for (;;)
					{
						if ((cached_LA1=='u'))
						{
							match('u');
						}
						else
						{
							if (_cnt243 >= 1) { goto _loop243_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
						}
						
						_cnt243++;
					}
_loop243_breakloop:					;
				}    // ( ... )+
				mHEX_DIGIT(false);
				mHEX_DIGIT(false);
				mHEX_DIGIT(false);
				mHEX_DIGIT(false);
				break;
			}
			case '0':  case '1':  case '2':  case '3':
			{
				matchRange('0','3');
				{
					if (((cached_LA1 >= '0' && cached_LA1 <= '7')) && ((cached_LA2 >= '\u0003' && cached_LA2 <= '\uffff')) && (true) && (true))
					{
						matchRange('0','7');
						{
							if (((cached_LA1 >= '0' && cached_LA1 <= '7')) && ((cached_LA2 >= '\u0003' && cached_LA2 <= '\uffff')) && (true) && (true))
							{
								matchRange('0','7');
							}
							else if (((cached_LA1 >= '\u0003' && cached_LA1 <= '\uffff')) && (true) && (true) && (true)) {
							}
							else
							{
								throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
							}
							
						}
					}
					else if (((cached_LA1 >= '\u0003' && cached_LA1 <= '\uffff')) && (true) && (true) && (true)) {
					}
					else
					{
						throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
					}
					
				}
				break;
			}
			case '4':  case '5':  case '6':  case '7':
			{
				matchRange('4','7');
				{
					if (((cached_LA1 >= '0' && cached_LA1 <= '7')) && ((cached_LA2 >= '\u0003' && cached_LA2 <= '\uffff')) && (true) && (true))
					{
						matchRange('0','7');
					}
					else if (((cached_LA1 >= '\u0003' && cached_LA1 <= '\uffff')) && (true) && (true) && (true)) {
					}
					else
					{
						throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
					}
					
				}
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSTRING_LITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = STRING_LITERAL;
		
		match('"');
		{    // ( ... )*
			for (;;)
			{
				if ((cached_LA1=='\\'))
				{
					mESC(false);
				}
				else if ((tokenSet_3_.member(cached_LA1))) {
					{
						match(tokenSet_3_);
					}
				}
				else
				{
					goto _loop239_breakloop;
				}
				
			}
_loop239_breakloop:			;
		}    // ( ... )*
		match('"');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mHEX_DIGIT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = HEX_DIGIT;
		
		{
			switch ( cached_LA1 )
			{
			case '0':  case '1':  case '2':  case '3':
			case '4':  case '5':  case '6':  case '7':
			case '8':  case '9':
			{
				matchRange('0','9');
				break;
			}
			case 'A':  case 'B':  case 'C':  case 'D':
			case 'E':  case 'F':
			{
				matchRange('A','F');
				break;
			}
			case 'a':  case 'b':  case 'c':  case 'd':
			case 'e':  case 'f':
			{
				matchRange('a','f');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mVOCAB(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = VOCAB;
		
		matchRange('\x3','\xff');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mIDENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = IDENT;
		
		{
			switch ( cached_LA1 )
			{
			case 'a':  case 'b':  case 'c':  case 'd':
			case 'e':  case 'f':  case 'g':  case 'h':
			case 'i':  case 'j':  case 'k':  case 'l':
			case 'm':  case 'n':  case 'o':  case 'p':
			case 'q':  case 'r':  case 's':  case 't':
			case 'u':  case 'v':  case 'w':  case 'x':
			case 'y':  case 'z':
			{
				matchRange('a','z');
				break;
			}
			case 'A':  case 'B':  case 'C':  case 'D':
			case 'E':  case 'F':  case 'G':  case 'H':
			case 'I':  case 'J':  case 'K':  case 'L':
			case 'M':  case 'N':  case 'O':  case 'P':
			case 'Q':  case 'R':  case 'S':  case 'T':
			case 'U':  case 'V':  case 'W':  case 'X':
			case 'Y':  case 'Z':
			{
				matchRange('A','Z');
				break;
			}
			case '_':
			{
				match('_');
				break;
			}
			case '$':
			{
				match('$');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		{    // ( ... )*
			for (;;)
			{
				switch ( cached_LA1 )
				{
				case 'a':  case 'b':  case 'c':  case 'd':
				case 'e':  case 'f':  case 'g':  case 'h':
				case 'i':  case 'j':  case 'k':  case 'l':
				case 'm':  case 'n':  case 'o':  case 'p':
				case 'q':  case 'r':  case 's':  case 't':
				case 'u':  case 'v':  case 'w':  case 'x':
				case 'y':  case 'z':
				{
					matchRange('a','z');
					break;
				}
				case 'A':  case 'B':  case 'C':  case 'D':
				case 'E':  case 'F':  case 'G':  case 'H':
				case 'I':  case 'J':  case 'K':  case 'L':
				case 'M':  case 'N':  case 'O':  case 'P':
				case 'Q':  case 'R':  case 'S':  case 'T':
				case 'U':  case 'V':  case 'W':  case 'X':
				case 'Y':  case 'Z':
				{
					matchRange('A','Z');
					break;
				}
				case '_':
				{
					match('_');
					break;
				}
				case '0':  case '1':  case '2':  case '3':
				case '4':  case '5':  case '6':  case '7':
				case '8':  case '9':
				{
					matchRange('0','9');
					break;
				}
				case '$':
				{
					match('$');
					break;
				}
				default:
				{
					goto _loop253_breakloop;
				}
				 }
			}
_loop253_breakloop:			;
		}    // ( ... )*
		{
			switch ( cached_LA1 )
			{
			case '?':
			{
				match('?');
				break;
			}
			case '!':
			{
				match('!');
				break;
			}
			default:
				{
				}
			break; }
		}
		_ttype = testLiteralsTable(_ttype);
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mINSTIDENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = INSTIDENT;
		
		match('@');
		mIDENT(false);
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSTATICIDENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = STATICIDENT;
		
		match("@@");
		mIDENT(false);
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSYMBOL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SYMBOL;
		
		mCOLON(false);
		mIDENT(false);
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mNUMBER(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NUMBER;
		
		if ((cached_LA1=='0') && (cached_LA2=='X'||cached_LA2=='x'))
		{
			match('0');
			{
				switch ( cached_LA1 )
				{
				case 'x':
				{
					match('x');
					break;
				}
				case 'X':
				{
					match('X');
					break;
				}
				default:
				{
					throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
				}
				 }
			}
			{ // ( ... )+
				int _cnt265=0;
				for (;;)
				{
					switch ( cached_LA1 )
					{
					case '0':  case '1':  case '2':  case '3':
					case '4':  case '5':  case '6':  case '7':
					case '8':  case '9':
					{
						matchRange('0','9');
						break;
					}
					case 'a':  case 'b':  case 'c':  case 'd':
					case 'e':  case 'f':
					{
						matchRange('a','f');
						break;
					}
					case 'A':  case 'B':  case 'C':  case 'D':
					case 'E':  case 'F':
					{
						matchRange('A','F');
						break;
					}
					default:
					{
						if (_cnt265 >= 1) { goto _loop265_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
					}
					break; }
					_cnt265++;
				}
_loop265_breakloop:				;
			}    // ( ... )+
			if (0==inputState.guessing)
			{
				_ttype = NUM_INT;
			}
			{
				switch ( cached_LA1 )
				{
				case 'l':
				{
					match('l');
					break;
				}
				case 'L':
				{
					match('L');
					if (0==inputState.guessing)
					{
						_ttype = NUM_LONG;
					}
					break;
				}
				default:
					{
					}
				break; }
			}
		}
		else {
			bool synPredMatched260 = false;
			if ((((cached_LA1 >= '0' && cached_LA1 <= '9')) && (true) && (true) && (true)))
			{
				int _m260 = mark();
				synPredMatched260 = true;
				inputState.guessing++;
				try {
					{
						mInt(false);
						mDOTDOT(false);
					}
				}
				catch (RecognitionException)
				{
					synPredMatched260 = false;
				}
				rewind(_m260);
				inputState.guessing--;
			}
			if ( synPredMatched260 )
			{
				mInt(false);
				if (0==inputState.guessing)
				{
					_ttype = NUM_INT;
				}
			}
			else {
				bool synPredMatched262 = false;
				if ((((cached_LA1 >= '0' && cached_LA1 <= '9')) && (true) && (true) && (true)))
				{
					int _m262 = mark();
					synPredMatched262 = true;
					inputState.guessing++;
					try {
						{
							mInt(false);
							mDOTDOTDOT(false);
						}
					}
					catch (RecognitionException)
					{
						synPredMatched262 = false;
					}
					rewind(_m262);
					inputState.guessing--;
				}
				if ( synPredMatched262 )
				{
					mInt(false);
					if (0==inputState.guessing)
					{
						_ttype = NUM_INT;
					}
				}
				else if (((cached_LA1 >= '0' && cached_LA1 <= '9')) && (true) && (true) && (true)) {
					mInt(false);
					if (0==inputState.guessing)
					{
						_ttype = NUM_INT;
					}
					{
						switch ( cached_LA1 )
						{
						case '.':
						{
							mFloatTrailer(false);
							if (0==inputState.guessing)
							{
								_ttype = NUM_FLOAT;
							}
							break;
						}
						case 'L':  case 'l':
						{
							{
								switch ( cached_LA1 )
								{
								case 'l':
								{
									match('l');
									break;
								}
								case 'L':
								{
									match('L');
									break;
								}
								default:
								{
									throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
								}
								 }
							}
							if (0==inputState.guessing)
							{
								_ttype = NUM_LONG;
							}
							break;
						}
						default:
							{
							}
						break; }
					}
				}
				else
				{
					throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
				}
				}}
				if (_createToken && (null == _token) && (_ttype != Token.SKIP))
				{
					_token = makeToken(_ttype);
					_token.setText(text.ToString(_begin, text.Length-_begin));
				}
				returnToken_ = _token;
			}
			
	protected void mInt(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = Int;
		
		{ // ( ... )+
			int _cnt271=0;
			for (;;)
			{
				if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
				{
					matchRange('0','9');
				}
				else
				{
					if (_cnt271 >= 1) { goto _loop271_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				
				_cnt271++;
			}
_loop271_breakloop:			;
		}    // ( ... )+
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mFloatTrailer(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = FloatTrailer;
		
		match('.');
		mInt(false);
		{
			if ((cached_LA1=='E'||cached_LA1=='e'))
			{
				mExponent(false);
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mNonZeroDigit(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NonZeroDigit;
		
		matchRange('1','9');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mExponent(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = Exponent;
		
		{
			switch ( cached_LA1 )
			{
			case 'e':
			{
				match('e');
				break;
			}
			case 'E':
			{
				match('E');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		{
			switch ( cached_LA1 )
			{
			case '+':
			{
				match('+');
				break;
			}
			case '-':
			{
				match('-');
				break;
			}
			case '0':  case '1':  case '2':  case '3':
			case '4':  case '5':  case '6':  case '7':
			case '8':  case '9':
			{
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		mInt(false);
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
/** Consume a newline and any whitespace at start of next line */
	public void mCONTINUED_LINE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = CONTINUED_LINE;
		
		match('\\');
		{
			switch ( cached_LA1 )
			{
			case '\r':
			{
				match('\r');
				break;
			}
			case '\n':
			{
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		match('\n');
		{    // ( ... )*
			for (;;)
			{
				switch ( cached_LA1 )
				{
				case ' ':
				{
					match(' ');
					break;
				}
				case '\t':
				{
					match('\t');
					break;
				}
				default:
				{
					goto _loop281_breakloop;
				}
				 }
			}
_loop281_breakloop:			;
		}    // ( ... )*
		if (0==inputState.guessing)
		{
			newline(); _ttype = Token.SKIP;
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = new long[1025];
		data[0]=68719476736L;
		data[1]=576460745995190270L;
		for (int i = 2; i<=1024; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = new long[2048];
		data[0]=-9224L;
		for (int i = 1; i<=1022; i++) { data[i]=-1L; }
		data[1023]=9223372036854775807L;
		for (int i = 1024; i<=2047; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = new long[2048];
		data[0]=-549755813896L;
		data[1]=-268435457L;
		for (int i = 2; i<=1023; i++) { data[i]=-1L; }
		for (int i = 1024; i<=2047; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = new long[2048];
		data[0]=-17179869192L;
		data[1]=-268435457L;
		for (int i = 2; i<=1023; i++) { data[i]=-1L; }
		for (int i = 1024; i<=2047; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	
}
}
