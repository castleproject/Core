// $ANTLR 2.7.5 (20050128): "langlexer.g" -> "WindsorConfLanguageLexer.cs"$

namespace Castle.Windsor.Configuration.Interpreters.CastleLanguage.Internal
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
	
	public 	class WindsorConfLanguageLexer : antlr.CharScanner	, TokenStream
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
		public const int STRING_LITERAL = 13;
		public const int DATA = 14;
		public const int COMMA = 15;
		public const int LLITERAL = 16;
		public const int RLITERAL = 17;
		public const int SL_COMMENT = 18;
		public const int LEADING_WS = 19;
		public const int WS = 20;
		
		
	int implicitLineJoiningLevel = 0;
		public WindsorConfLanguageLexer(Stream ins) : this(new ByteBuffer(ins))
		{
		}
		
		public WindsorConfLanguageLexer(TextReader r) : this(new CharBuffer(r))
		{
		}
		
		public WindsorConfLanguageLexer(InputBuffer ib)		 : this(new LexerSharedInputState(ib))
		{
		}
		
		public WindsorConfLanguageLexer(LexerSharedInputState state) : base(state)
		{
			initialize();
		}
		private void initialize()
		{
			caseSensitiveLiterals = true;
			setCaseSensitive(true);
			literals = new Hashtable(100, (float) 0.4, null, Comparer.Default);
			literals.Add("import", 5);
			literals.Add("in", 4);
		}
		
		override public IToken nextToken()			//throws TokenStreamException
		{
			IToken theRetToken = null;
tryAgain:
			for (;;)
			{
				IToken _token = null;
				int _ttype = Token.INVALID_TYPE;
				setCommitToPath(false);
				resetText();
				try     // for char stream error handling
				{
					try     // for lexical error handling
					{
						switch ( cached_LA1 )
						{
						case ':':
						{
							mCOLON(true);
							theRetToken = returnToken_;
							break;
						}
						case ',':
						{
							mCOMMA(true);
							theRetToken = returnToken_;
							break;
						}
						case '.':
						{
							mDOT(true);
							theRetToken = returnToken_;
							break;
						}
						case '>':
						{
							mRLITERAL(true);
							theRetToken = returnToken_;
							break;
						}
						case '/':
						{
							mSL_COMMENT(true);
							theRetToken = returnToken_;
							break;
						}
						case '"':
						{
							mSTRING_LITERAL(true);
							theRetToken = returnToken_;
							break;
						}
						case '0':  case '1':  case '2':  case '3':
						case '4':  case '5':  case '6':  case '7':
						case '8':  case '9':  case 'A':  case 'B':
						case 'C':  case 'D':  case 'E':  case 'F':
						case 'G':  case 'H':  case 'I':  case 'J':
						case 'K':  case 'L':  case 'M':  case 'N':
						case 'O':  case 'P':  case 'Q':  case 'R':
						case 'S':  case 'T':  case 'U':  case 'V':
						case 'W':  case 'X':  case 'Y':  case 'Z':
						case '_':  case 'a':  case 'b':  case 'c':
						case 'd':  case 'e':  case 'f':  case 'g':
						case 'h':  case 'i':  case 'j':  case 'k':
						case 'l':  case 'm':  case 'n':  case 'o':
						case 'p':  case 'q':  case 'r':  case 's':
						case 't':  case 'u':  case 'v':  case 'w':
						case 'x':  case 'y':  case 'z':
						{
							mID(true);
							theRetToken = returnToken_;
							break;
						}
						case '\n':  case '\r':
						{
							mNEWLINE(true);
							theRetToken = returnToken_;
							break;
						}
						default:
							if ((cached_LA1=='<') && (cached_LA2=='<'))
							{
								mDATA(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='<') && (true)) {
								mLLITERAL(true);
								theRetToken = returnToken_;
							}
							else if (((cached_LA1=='\t'||cached_LA1==' ') && (true))&&(getColumn()==1)) {
								mLEADING_WS(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='\t'||cached_LA1==' ') && (true)) {
								mWS(true);
								theRetToken = returnToken_;
							}
						else
						{
							if (cached_LA1==EOF_CHAR) { uponEOF(); returnToken_ = makeToken(Token.EOF_TYPE); }
								else				{					consume();					goto tryAgain;				}
						}
						break; }
						if ( null==returnToken_ ) goto tryAgain; // found SKIP token
						_ttype = returnToken_.Type;
						returnToken_.Type = _ttype;
						return returnToken_;
					}
					catch (RecognitionException e) {
						if (!getCommitToPath())
						{
							consume();
							goto tryAgain;
						}
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
	
	public void mLLITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LLITERAL;
		
		match('<');
		implicitLineJoiningLevel++;
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mRLITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RLITERAL;
		
		match('>');
		implicitLineJoiningLevel--;
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
		
		match("//");
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_0_.member(cached_LA1)))
				{
					matchNot('\n');
				}
				else
				{
					goto _loop8_breakloop;
				}
				
			}
_loop8_breakloop:			;
		}    // ( ... )*
		match('\n');
		_ttype = Token.SKIP; newline();
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
		
		int _saveIndex = 0;
		_saveIndex = text.Length;
		match('"');
		text.Length = _saveIndex;
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_1_.member(cached_LA1)))
				{
					{
						match(tokenSet_1_);
					}
				}
				else
				{
					goto _loop12_breakloop;
				}
				
			}
_loop12_breakloop:			;
		}    // ( ... )*
		_saveIndex = text.Length;
		match('"');
		text.Length = _saveIndex;
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mDATA(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DATA;
		
		int _saveIndex = 0;
		_saveIndex = text.Length;
		match("<<");
		text.Length = _saveIndex;
		
			implicitLineJoiningLevel++;
			
		{ // ( ... )+
			int _cnt16=0;
			for (;;)
			{
				if ((cached_LA1=='\r') && (cached_LA2=='\n'))
				{
					match('\r');
					match('\n');
					newline();
				}
				else if ((cached_LA1=='\r') && ((cached_LA2 >= '\u0000' && cached_LA2 <= '\u007f'))) {
					match('\r');
					newline();
				}
				else if ((cached_LA1=='\n') && ((cached_LA2 >= '\u0000' && cached_LA2 <= '\u007f'))) {
					match('\n');
					newline();
				}
				else if ((tokenSet_2_.member(cached_LA1)) && ((cached_LA2 >= '\u0000' && cached_LA2 <= '\u007f'))) {
					{
						match(tokenSet_2_);
					}
				}
				else
				{
					if (_cnt16 >= 1) { goto _loop16_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				
				_cnt16++;
			}
_loop16_breakloop:			;
		}    // ( ... )+
		_saveIndex = text.Length;
		match(">>");
		text.Length = _saveIndex;
		
			implicitLineJoiningLevel--;
			
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
/** 
 *  Grab everything before a real symbol.  Then if newline, kill it
 *  as this is a blank line.  If whitespace followed by comment, kill it
 *  as it's a comment on a line by itself.
 *
 *  Ignore leading whitespace when nested in [..], (..), {..}.
 */
	public void mLEADING_WS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LEADING_WS;
		
		int spaces = 0;
		
		
		if (!(getColumn()==1))
		  throw new SemanticException("getColumn()==1");
		{ // ( ... )+
			int _cnt19=0;
			for (;;)
			{
				switch ( cached_LA1 )
				{
				case ' ':
				{
					match(' ');
					spaces++;
					break;
				}
				case '\t':
				{
					match('\t');
					spaces += 8; spaces -= (spaces % 8);
					break;
				}
				default:
				{
					if (_cnt19 >= 1) { goto _loop19_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				break; }
				_cnt19++;
			}
_loop19_breakloop:			;
		}    // ( ... )+
		
		if ( implicitLineJoiningLevel>0 ) 
		{
		// ignore ws if nested
		_ttype = Token.SKIP;
		}
		else
					{
						// make a string of n spaces where n is column number - 1
						char[] indentation = new char[spaces];
						for (int i=0; i<spaces; i++) {
							indentation[i] = ' ';
						}
						String s = new String(indentation);
						text.Length = _begin; text.Append(s);
					}
				
		{
			if (((cached_LA1=='\n'||cached_LA1=='\r'))&&(implicitLineJoiningLevel==0))
			{
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
				newline();
				_ttype = Token.SKIP;
			}
			else if ((cached_LA1=='/')) {
				match("//");
				{    // ( ... )*
					for (;;)
					{
						if ((tokenSet_0_.member(cached_LA1)))
						{
							matchNot('\n');
						}
						else
						{
							goto _loop23_breakloop;
						}
						
					}
_loop23_breakloop:					;
				}    // ( ... )*
				{ // ( ... )+
					int _cnt25=0;
					for (;;)
					{
						if ((cached_LA1=='\n'))
						{
							match('\n');
							newline();
						}
						else
						{
							if (_cnt25 >= 1) { goto _loop25_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
						}
						
						_cnt25++;
					}
_loop25_breakloop:					;
				}    // ( ... )+
				_ttype = Token.SKIP;
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
	
	public void mID(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ID;
		
		{ // ( ... )+
			int _cnt28=0;
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
				case '0':  case '1':  case '2':  case '3':
				case '4':  case '5':  case '6':  case '7':
				case '8':  case '9':
				{
					matchRange('0','9');
					break;
				}
				case '_':
				{
					match('_');
					break;
				}
				default:
				{
					if (_cnt28 >= 1) { goto _loop28_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				break; }
				_cnt28++;
			}
_loop28_breakloop:			;
		}    // ( ... )+
		_ttype = testLiteralsTable(_ttype);
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
		
		int startCol = getColumn();
		
		
		{ // ( ... )+
			int _cnt32=0;
			for (;;)
			{
				if ((cached_LA1=='\n'||cached_LA1=='\r'))
				{
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
					newline();
				}
				else
				{
					if (_cnt32 >= 1) { goto _loop32_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				
				_cnt32++;
			}
_loop32_breakloop:			;
		}    // ( ... )+
		
		if ( startCol==1 || implicitLineJoiningLevel>0 )
		_ttype = Token.SKIP;
		
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
			int _cnt35=0;
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
					if (_cnt35 >= 1) { goto _loop35_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				break; }
				_cnt35++;
			}
_loop35_breakloop:			;
		}    // ( ... )+
		_ttype = Token.SKIP;
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { -1025L, -1L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { -17179870209L, -1L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { -4611686018427387905L, -1L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	
}
}
