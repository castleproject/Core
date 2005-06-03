// $ANTLR 2.7.4: "langparser.g" -> "AspectLanguageParser.cs"$

    using antlr;
    using System.Text;
    using AspectSharp.Lang.AST;

	// Generate the header common to all output files.
	using System;
	
	using TokenBuffer              = antlr.TokenBuffer;
	using TokenStreamException     = antlr.TokenStreamException;
	using TokenStreamIOException   = antlr.TokenStreamIOException;
	using ANTLRException           = antlr.ANTLRException;
	using LLkParser = antlr.LLkParser;
	using Token                    = antlr.Token;
	using TokenStream              = antlr.TokenStream;
	using RecognitionException     = antlr.RecognitionException;
	using NoViableAltException     = antlr.NoViableAltException;
	using MismatchedTokenException = antlr.MismatchedTokenException;
	using SemanticException        = antlr.SemanticException;
	using ParserSharedInputState   = antlr.ParserSharedInputState;
	using BitSet                   = antlr.collections.impl.BitSet;
	
	public 	class AspectLanguageParser : antlr.LLkParser
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int ASPECT = 4;
		public const int FOR = 5;
		public const int IN = 6;
		public const int END = 7;
		public const int IMPORT = 8;
		public const int MIXINS = 9;
		public const int INCLUDE = 10;
		public const int INTERCEPTORS = 11;
		public const int ADVICEINTERCEPTOR = 12;
		public const int POINTCUT = 13;
		public const int METHOD = 14;
		public const int PROPERTY = 15;
		public const int PROPERTY_READ = 16;
		public const int PROPERTY_WRITE = 17;
		public const int ASSIGNFROM = 18;
		public const int CUSTOMMATCHER = 19;
		public const int EXCLUDES = 20;
		public const int INCLUDES = 21;
		public const int EOS = 22;
		public const int LBRACK = 23;
		public const int SEMI = 24;
		public const int RBRACK = 25;
		public const int STRING_LITERAL = 26;
		public const int COLON = 27;
		public const int ID = 28;
		public const int LCURLY = 29;
		public const int RCURLY = 30;
		public const int OR = 31;
		public const int ALL = 32;
		public const int COMMA = 33;
		public const int DOT = 34;
		public const int WS = 35;
		
		
    protected StringBuilder sbuilder = new StringBuilder();

	protected LexicalInfo ToLexicalInfo(antlr.Token token)
	{
		int line = token.getLine();
		int startColumn = token.getColumn();
		int endColumn = token.getColumn() + token.getText().Length;
		String filename = token.getFilename();
		return new LexicalInfo(filename, line, startColumn, endColumn);
	}
		
    protected String methodAll(String s)
   {
        if (s == "*")
            return ".*";
        return s;   
    }
		
		protected void initialize()
		{
			tokenNames = tokenNames_;
		}
		
		
		protected AspectLanguageParser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			initialize();
		}
		
		public AspectLanguageParser(TokenBuffer tokenBuf) : this(tokenBuf,1)
		{
		}
		
		protected AspectLanguageParser(TokenStream lexer, int k) : base(lexer,k)
		{
			initialize();
		}
		
		public AspectLanguageParser(TokenStream lexer) : this(lexer,1)
		{
		}
		
		public AspectLanguageParser(ParserSharedInputState state) : base(state,1)
		{
			initialize();
		}
		
	public EngineConfiguration  start() //throws RecognitionException, TokenStreamException
{
		EngineConfiguration conf;
		
		
				conf = new EngineConfiguration();		
			
		
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
					if ((LA(1)==IMPORT))
					{
						import_directive(conf);
					}
					else
					{
						goto _loop5_breakloop;
					}
					
				}
_loop5_breakloop:				;
			}    // ( ... )*
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==INTERCEPTORS))
					{
						interceptors_global(conf);
					}
					else
					{
						goto _loop7_breakloop;
					}
					
				}
_loop7_breakloop:				;
			}    // ( ... )*
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==MIXINS))
					{
						mixins_global(conf);
					}
					else
					{
						goto _loop9_breakloop;
					}
					
				}
_loop9_breakloop:				;
			}    // ( ... )*
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==ASPECT))
					{
						aspects(conf);
					}
					else
					{
						goto _loop11_breakloop;
					}
					
				}
_loop11_breakloop:				;
			}    // ( ... )*
			match(Token.EOF_TYPE);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_0_);
		}
		return conf;
	}
	
	protected void import_directive(
		EngineConfiguration conf
	) //throws RecognitionException, TokenStreamException
{
		
		Token  i = null;
		
				String ns;
				String assemblyName;
				ImportDirective import = null;
			
		
		try {      // for error handling
			i = LT(1);
			match(IMPORT);
			ns=identifier();
			
					import = new ImportDirective( ToLexicalInfo(i), ns );
					conf.Imports.Add(import);
				
			{
				switch ( LA(1) )
				{
				case IN:
				{
					match(IN);
					assemblyName=identifier();
					
						        import.AssemblyReference = new AssemblyReference( ToLexicalInfo(i), assemblyName);
						
					break;
				}
				case EOF:
				case ASPECT:
				case IMPORT:
				case MIXINS:
				case INTERCEPTORS:
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
			consume();
			consumeUntil(tokenSet_1_);
		}
	}
	
	protected void interceptors_global(
		EngineConfiguration conf
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(INTERCEPTORS);
			keytypepair(conf.Interceptors);
			
			
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_2_);
		}
	}
	
	protected void mixins_global(
		EngineConfiguration conf
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(MIXINS);
			keytypepair(conf.Mixins);
			
			
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_3_);
		}
	}
	
	protected void aspects(
		EngineConfiguration conf
	) //throws RecognitionException, TokenStreamException
{
		
		Token  a = null;
		Token  aspectId = null;
		
		AspectDefinition aspect = null;
		TargetTypeDefinition target = null;
		TypeReference tr = null;
		
		
		try {      // for error handling
			a = LT(1);
			match(ASPECT);
			aspectId = LT(1);
			match(ID);
			match(FOR);
			
			aspect = new AspectDefinition( ToLexicalInfo(a), aspectId.getText() );
			conf.Aspects.Add(aspect);
			
			{
				switch ( LA(1) )
				{
				case ID:
				{
					tr=type_name_def();
					
					target = new TargetTypeDefinition( tr );
					target.TargetStrategy = TargetStrategyEnum.SingleType;
					aspect.TargetType = target;
					
					break;
				}
				case LBRACK:
				{
					match(LBRACK);
					
					target = new TargetTypeDefinition( );
					aspect.TargetType = target;
					String namespaceRegEx = null;
					
					{
						switch ( LA(1) )
						{
						case ASSIGNFROM:
						{
							match(ASSIGNFROM);
							match(LCURLY);
							tr=type_name_def();
							match(RCURLY);
							
							target.TargetStrategy = TargetStrategyEnum.Assignable;
							target.AssignType = tr;
							
							break;
						}
						case CUSTOMMATCHER:
						{
							match(CUSTOMMATCHER);
							match(LCURLY);
							tr=type_name_def();
							match(RCURLY);
							
							target.TargetStrategy = TargetStrategyEnum.Custom;
							target.CustomMatcherType = tr;
							
							break;
						}
						case ID:
						{
							{
								namespaceRegEx=identifier();
								
								target.TargetStrategy = TargetStrategyEnum.Namespace;
								target.NamespaceRoot = namespaceRegEx;
								
								{
									switch ( LA(1) )
									{
									case EXCLUDES:
									{
										match(EXCLUDES);
										match(LCURLY);
										type_list(target.Excludes);
										match(RCURLY);
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
							}
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
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				{    // ( ... )*
					for (;;)
					{
						if ((LA(1)==INCLUDE))
						{
							include(aspect);
						}
						else
						{
							goto _loop33_breakloop;
						}
						
					}
_loop33_breakloop:					;
				}    // ( ... )*
				{    // ( ... )*
					for (;;)
					{
						if ((LA(1)==POINTCUT))
						{
							pointcut(aspect);
						}
						else
						{
							goto _loop35_breakloop;
						}
						
					}
_loop35_breakloop:					;
				}    // ( ... )*
			}
			match(END);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_4_);
		}
	}
	
	protected String  identifier() //throws RecognitionException, TokenStreamException
{
		String value;
		
		Token  id = null;
		Token  id2 = null;
		
				value = null; sbuilder.Length = 0;
			
		
		try {      // for error handling
			id = LT(1);
			match(ID);
								
					sbuilder.Append(id.getText());
					value = sbuilder.ToString();
				
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==DOT))
					{
						match(DOT);
						id2 = LT(1);
						match(ID);
						
							        sbuilder.Append('.');
							        sbuilder.Append(id2.getText());
							
					}
					else
					{
						goto _loop65_breakloop;
					}
					
				}
_loop65_breakloop:				;
			}    // ( ... )*
			
				    value = sbuilder.ToString();
				
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_5_);
		}
		return value;
	}
	
	protected void keytypepair(
		IDeclarationCollection collection
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(LBRACK);
			{
				{
					pairvalue(collection);
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==SEMI))
							{
								match(SEMI);
								pairvalue(collection);
							}
							else
							{
								goto _loop20_breakloop;
							}
							
						}
_loop20_breakloop:						;
					}    // ( ... )*
				}
			}
			match(RBRACK);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_2_);
		}
	}
	
	protected void pairvalue(
		IDeclarationCollection collection
	) //throws RecognitionException, TokenStreamException
{
		
		Token  keyToken = null;
		
		DefinitionBase definition = null;
		
		
		try {      // for error handling
			keyToken = LT(1);
			match(STRING_LITERAL);
			match(COLON);
			
			String key = keyToken.getText();
			definition = collection.Add( key, ToLexicalInfo(keyToken) );
			
			type_name(definition);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_6_);
		}
	}
	
	protected void type_name(
		DefinitionBase definition
	) //throws RecognitionException, TokenStreamException
{
		
		
		TypeReference tr = null;
		
		
		try {      // for error handling
			tr=type_name_def();
			
			definition.TypeReference = tr;
			
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_6_);
		}
	}
	
	protected TypeReference  type_name_or_ref() //throws RecognitionException, TokenStreamException
{
		TypeReference type;
		
		Token  refTypeToken = null;
		
		type = null;
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case STRING_LITERAL:
			{
				refTypeToken = LT(1);
				match(STRING_LITERAL);
				
				type = new TypeReference();
				type.LinkRef = refTypeToken.getText();
				
				break;
			}
			case ID:
			{
				type=type_name_def();
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
			consume();
			consumeUntil(tokenSet_7_);
		}
		return type;
	}
	
	protected TypeReference  type_name_def() //throws RecognitionException, TokenStreamException
{
		TypeReference type;
		
		Token  i = null;
		
		type = new TypeReference();
		String typeToken = null;
		String assemblyToken = null;
		
		
		try {      // for error handling
			typeToken=identifier();
			
			type.TypeName = typeToken;
			
			{
				switch ( LA(1) )
				{
				case IN:
				{
					i = LT(1);
					match(IN);
					assemblyToken=identifier();
					
					type.AssemblyReference = new AssemblyReference( ToLexicalInfo(i), assemblyToken );
					
					break;
				}
				case END:
				case INCLUDE:
				case POINTCUT:
				case SEMI:
				case RBRACK:
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
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_8_);
		}
		return type;
	}
	
	protected void type_list(
		TypeReferenceCollection types
	) //throws RecognitionException, TokenStreamException
{
		
		
		TypeReference tr = null;
		
		
		try {      // for error handling
			tr=type_name_def();
			
			types.Add(tr);
			
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==SEMI))
					{
						match(SEMI);
						tr=type_name_def();
						
						types.Add(tr);
						
					}
					else
					{
						goto _loop38_breakloop;
					}
					
				}
_loop38_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_9_);
		}
	}
	
	protected void include(
		AspectDefinition aspect
	) //throws RecognitionException, TokenStreamException
{
		
		Token  i = null;
		
		TypeReference tr = null;
		MixinDefinition md;
		
		
		try {      // for error handling
			i = LT(1);
			match(INCLUDE);
			
			md = new MixinDefinition( ToLexicalInfo(i) );
			
			tr=type_name_or_ref();
			
			md.TypeReference = tr;
			aspect.Mixins.Add( md );
			
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_10_);
		}
	}
	
	protected void pointcut(
		AspectDefinition aspect
	) //throws RecognitionException, TokenStreamException
{
		
		Token  p = null;
		
		PointCutDefinition pointcut = null;
		PointCutFlags flags = PointCutFlags.Unspecified;
		
		
		try {      // for error handling
			p = LT(1);
			match(POINTCUT);
			flags=pointcutflags();
			
			pointcut = new PointCutDefinition( ToLexicalInfo(p), flags );
			aspect.PointCuts.Add( pointcut );
			
			pointcuttarget(pointcut);
			advices(pointcut);
			match(END);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_11_);
		}
	}
	
	protected PointCutFlags  pointcutflags() //throws RecognitionException, TokenStreamException
{
		PointCutFlags flags;
		
		
		flags = PointCutFlags.Unspecified;
		
		
		try {      // for error handling
			flags=pointcutflag(flags);
			{
				switch ( LA(1) )
				{
				case OR:
				{
					match(OR);
					flags=pointcutflag(flags);
					break;
				}
				case LCURLY:
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
			consume();
			consumeUntil(tokenSet_12_);
		}
		return flags;
	}
	
	protected void pointcuttarget(
		PointCutDefinition pointcut
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(LCURLY);
			pointcutsignature(pointcut);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_13_);
		}
	}
	
	protected void advices(
		PointCutDefinition pointcut
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==ADVICEINTERCEPTOR))
					{
						advice(pointcut);
					}
					else
					{
						goto _loop43_breakloop;
					}
					
				}
_loop43_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_14_);
		}
	}
	
	protected void advice(
		PointCutDefinition pointcut
	) //throws RecognitionException, TokenStreamException
{
		
		Token  i = null;
		
		TypeReference tr = null;
		InterceptorDefinition interDef = null;
		
		
		try {      // for error handling
			i = LT(1);
			match(ADVICEINTERCEPTOR);
			
			interDef = new InterceptorDefinition( ToLexicalInfo(i) );
			
			match(LCURLY);
			tr=type_name_or_ref();
			
			interDef.TypeReference = tr;
			pointcut.Advices.Add( interDef );
			
			match(RCURLY);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_13_);
		}
	}
	
	protected PointCutFlags  pointcutflag(
		PointCutFlags flags
	) //throws RecognitionException, TokenStreamException
{
		PointCutFlags retValue;
		
		
		retValue = flags;
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case METHOD:
			{
				match(METHOD);
				retValue |= PointCutFlags.Method;
				break;
			}
			case PROPERTY:
			{
				match(PROPERTY);
				retValue |= PointCutFlags.Property;
				break;
			}
			case PROPERTY_READ:
			{
				match(PROPERTY_READ);
				retValue |= PointCutFlags.PropertyRead;
				break;
			}
			case PROPERTY_WRITE:
			{
				match(PROPERTY_WRITE);
				retValue |= PointCutFlags.PropertyWrite;
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
			consume();
			consumeUntil(tokenSet_15_);
		}
		return retValue;
	}
	
	protected void pointcutsignature(
		PointCutDefinition pointcut
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			pointcutsig1(pointcut);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_13_);
		}
	}

	protected void pointcutsig1(
		PointCutDefinition pointcut
	) //throws RecognitionException, TokenStreamException
{
		
		
		String part1;
		MethodSignature ms = null; 
				
				
		try {      // for error handling
				{
					switch ( LA(1) )
					{
				case ALL:
					{
							match(ALL);
						
					part1 = "*"; 
					ms = AllMethodSignature.Instance; 

								break;
							}
				case ID:
							{
					part1=reg_ex(true);
					ms = new MethodSignature(part1, methodAll("*"));
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
			pointcut.Method = pointcutsig2(part1, ms);
					}
		catch (RecognitionException ex)
					{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_13_);
				}
			}
	
	protected String  reg_ex(
		Boolean allowALL
	) //throws RecognitionException, TokenStreamException
{
		String value;
		
		Token  id = null;
		
				value = null; sbuilder.Length = 0;
			
		
		try {      // for error handling
				{
				id = LT(1);
				match(ID);
					
						    sbuilder.Append(id.getText());
						    value = sbuilder.ToString();
					
					{
						switch ( LA(1) )
						{
					case WS:
						{
						match(WS);
						break;
							}
					case DOT:
						{
						match(DOT);
							
							            sbuilder.Append('.');
							
						match(ALL);
						
							            sbuilder.Append('*');
							
							break;
						}
					default:
						if ((tokenSet_16_.member(LA(1))))
						{
							if (LA(1) == ALL) {
								       
								            if (allowALL) 
							break;
								            throw new NoViableAltException(LT(1), getFilename()); 
						}
								
						}
						else if ((tokenSet_16_.member(LA(1)))) {
						}
					else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
					break; }
						 }
					}
			
				    value = sbuilder.ToString();
				
				}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_16_);
		}
		return value;
	}
	
	protected MethodSignature  pointcutsig2(
		String part1, MethodSignature ms
	) //throws RecognitionException, TokenStreamException
{
		MethodSignature ret;
		
		
		String part2; 
		ret = null;
		
		
		try {      // for error handling
				{
					switch ( LA(1) )
					{
				case ALL:
				{
					match(ALL);
					part2 = "*";
					ret=pointcutsig3(part1, part2, ms);
					break;
				}
				case LCURLY:
					{
						match(LCURLY);
					pointcutarguments(ms);
					match(RCURLY);
						match(RCURLY);
					ret = ms;
						break;
					}
				case RCURLY:
					{
					match(RCURLY);
					ret = ms;
						break;
					}
				case ID:
					{
					part2=reg_ex(true);
					ret=pointcutsig3(part1, part2, ms);
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
			consume();
			consumeUntil(tokenSet_0_);
		}
		return ret;
	}
	
	protected MethodSignature  pointcutsig3(
		String part1, String part2, MethodSignature ms
	) //throws RecognitionException, TokenStreamException
{
		MethodSignature ret;
		
		
		String part3; 
		ret = null;
			
		
		try {      // for error handling
			{
					switch ( LA(1) )
					{
				case ALL:
					{
					match(ALL);
					part3 = "*";
					ms = new MethodSignature(part1, part2, methodAll(part3));
					pointcutsig4(ms);
						break;
					}
				case LCURLY:
					{
					match(LCURLY);
					ms = new MethodSignature(part1, methodAll(part2));
					pointcutarguments(ms);
					match(RCURLY);
					match(RCURLY);
						break;
					}
					case RCURLY:
					{
					match(RCURLY);
					ms = new MethodSignature(part1, methodAll(part2));
						break;
					}
				case ID:
					{
					part3=reg_ex(false);
					ms = new MethodSignature(part1, part2, methodAll(part3));
					pointcutsig4(ms);
							break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
			ret = ms;
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_0_);
		}
		return ret;
	}
	
	protected void pointcutarguments(
		MethodSignature ms
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case ID:
				case ALL:
				{
					pointcutargument(ms);
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								pointcutargument(ms);
							}
							else
							{
								goto _loop61_breakloop;
							}
							
						}
_loop61_breakloop:						;
					}    // ( ... )*
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
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			consume();
			consumeUntil(tokenSet_9_);
		}
	}
	
	protected void pointcutsig4(
		MethodSignature ms
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case LCURLY:
				{
					match(LCURLY);
					pointcutarguments(ms);
					match(RCURLY);
					match(RCURLY);
					break;
				}
				case RCURLY:
				{
					match(RCURLY);
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
			consume();
			consumeUntil(tokenSet_0_);
		}
	}
	
	protected void pointcutargument(
		MethodSignature ms
	) //throws RecognitionException, TokenStreamException
{
		
		
		String argType = String.Empty;
		
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case ALL:
			{
				match(ALL);
				
				ms.AddArgumentType( "*" );
				
				break;
			}
			case ID:
			{
				argType=reg_ex(false);
				
				ms.AddArgumentType( argType );
				
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
			consume();
			consumeUntil(tokenSet_17_);
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
		@"""aspect""",
		@"""for""",
		@"""in""",
		@"""end""",
		@"""import""",
		@"""mixins""",
		@"""include""",
		@"""interceptors""",
		@"""advice""",
		@"""pointcut""",
		@"""method""",
		@"""property""",
		@"""propertyread""",
		@"""propertywrite""",
		@"""assignableFrom""",
		@"""customMatcher""",
		@"""excludes""",
		@"""includes""",
		@"""EOS""",
		@"""LBRACK""",
		@"""SEMI""",
		@"""RBRACK""",
		@"""STRING_LITERAL""",
		@"""COLON""",
		@"""ID""",
		@"""LCURLY""",
		@"""RCURLY""",
		@"""OR""",
		@"""ALL""",
		@"""COMMA""",
		@"""DOT""",
		@"""WS"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { 2L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { 2834L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { 2578L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 530L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { 18L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { 1125134290L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { 50331648L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { 1073751168L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { 1124082816L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { 1073741824L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { 9344L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { 8320L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { 536870912L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	private static long[] mk_tokenSet_13_()
	{
		long[] data = { 4224L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
	private static long[] mk_tokenSet_14_()
	{
		long[] data = { 128L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
	private static long[] mk_tokenSet_15_()
	{
		long[] data = { 2684354560L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
	private static long[] mk_tokenSet_16_()
	{
		long[] data = { 14763954304L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_16_ = new BitSet(mk_tokenSet_16_());
	private static long[] mk_tokenSet_17_()
	{
		long[] data = { 9663676416L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_17_ = new BitSet(mk_tokenSet_17_());
	
}
