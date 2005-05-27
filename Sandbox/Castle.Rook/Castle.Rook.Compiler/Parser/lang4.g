header 
{
	// using CommonAST					= antlr.CommonAST; 
	using System.Text;
	using System.Collections;
	using Castle.Rook.Compiler.AST;
}
options 
{	
	language = "CSharp";
	namespace = "Castle.Rook.Parse";
}

class RookBaseParser extends Parser;
options 
{
	k = 2;                           // two token lookahead
	exportVocab=Rook;                
	codeGenMakeSwitchThreshold = 2;  // Some optimizations
	codeGenBitsetTestThreshold = 3;
	defaultErrorHandler = false;     // Don't generate parser error handlers
	// buildAST = true;
}
tokens 
{
	CLASS = "class";
	DO = "do";
	END = "end";
	DEF = "def";
	OPERATOR = "operator";
	BEGIN = "begin";
	WHILE = "while";	
	TYPE; METHOD_CALL; SUPER_CTOR_CALL; POST_INC; POST_DEC; EXPR; ELIST; INDEX_OP; 
	UNARY_MINUS; UNARY_PLUS; TYPECAST; ARRAY_DECLARATOR; 
	NUM_INT; NUM_DOUBLE; NUM_FLOAT; NUM_LONG;
	STATEMENT_END;
}
{
	AccessLevel currentAccessLevel = AccessLevel.Public;
	
	public override void reportError(RecognitionException ex)
	{
		throw ex;
	}
}

protected 
statement_term!
    :
    STATEMENT_END | SEMI
    ;

protected 
nothing
	:
	(options { generateAmbigWarnings=false; }:STATEMENT_END)?
	;

compilationUnit returns[CompilationUnit comp]
	{ comp = new CompilationUnit(); }
	:
	nothing
	suite[comp.Statements]
	nothing
	EOF!
	;

suite[IList stmts]
	{ IStatement stmt = null; }
	:
	(stmt=statement { if (stmt != null) stmts.Add(stmt); } )*
	;

statement returns[IStatement stmt]
	{ stmt = null; }
	:
	(
		// declaration_statement
		// |
		// operator_def_statement
		// |
		stmt=type_def_statement
		|
		stmt=while_statement
		|
		stmt=until_statement
		|
		stmt=for_statement
		|
		flow_statements
		|
		stmt=if_statement
		|
		(unless_statement) => stmt=unless_statement
		|
		expression_statement
	)
	statement_term
	;

while_statement returns[RepeatStatement rs]
	{ rs = new RepeatStatement(RepeatType.While); }
	:
	WHILE^ test (DO|statement_term) suite[rs.Statements] END!
	;

until_statement returns[RepeatStatement rs]
	{ rs = new RepeatStatement(RepeatType.Until); }
	:
	UNTIL^ test (DO|statement_term) statement_term suite[rs.Statements] END!
	;

for_statement returns[ForStatement fors]
	{ fors = new ForStatement(); }
	:
	"for" IDENT (COMMA! IDENT)* "in" test (DO|statement_term) 
	suite[fors.Statements]
	END!
	;

if_statement returns[IfStatement ifs]
	{ ifs = new IfStatement(IfType.If); }
	: 
	"if" test ("then"|statement_term) suite[ifs.TrueStatements] 
	("elsif" test ("then"|statement_term) suite[ifs.TrueStatements])*  // TODO: Add Else ifs to the AST
	("else" statement_term suite[ifs.FalseStatements])?
	END!
	;

unless_statement returns[IfStatement ifs]
	{ ifs = new IfStatement(IfType.Unless); }
	: 
	"unless" test ("then"|statement_term) suite[ifs.TrueStatements]
	("else" statement_term suite[ifs.FalseStatements])?
	END
	;

flow_statements
	:
	"redo"
	|
	"break"
	|
	"next"
	|
	"retry"
	;

protected
access_level
	:
	(
		"public"^    (COLON)? // { currentAccessLevel = AccessLevel.Public; }
		|
		"private"^   (COLON)? // { currentAccessLevel = AccessLevel.Private; }
		|
		"protected"^ (COLON)? // { currentAccessLevel = AccessLevel.Protected; }
		| 
		"internal"^  (COLON)? // { currentAccessLevel = AccessLevel.Internal; }
		|
			/* nothing - inherits the access level defined previously */  
	)
	;

// declaration_statement
// 	:
// 	type_name (COMMA! type_name)* (ASSIGN testlist)?
// 	{#declaration_statement = #(#[EXPR,"DECLS"],#declaration_statement);}
// 	;

type_def_statement returns[TypeDefinitionStatement tdstmt]
	{ tdstmt = null; }
	:
	tdstmt=class_def_statement
	;

class_def_statement returns[TypeDefinitionStatement tdstmt]
	{ tdstmt = new TypeDefinitionStatement(); } // TODO: Create ClassDefinitionStatement 
												// and support modifiers like visibility and abstract etc
	:
	CLASS IDENT ( (LT|SL) qualified_name (COMMA! qualified_name)* )? statement_term
	suite[tdstmt.Statements]
	END
	;

method_def_statement returns[MethodDefinitionStatement mdstmt]
	{ mdstmt = new MethodDefinitionStatement(); }
	:
	DEF^ qualified_name LPAREN (methodParams)? RPAREN (type)? statement_term
	suite[mdstmt.Statements]
	END!
	;

// operator_def_statement
//	:
//	OPERATOR^ qualified_name LPAREN (methodParams)? RPAREN (type)? statement_term
//	suite
//	END!
//	;

methodParams
	:
	methodParam (COMMA! methodParam)*
	;

methodParam
	:
	type_name (ASSIGN expression)?
	|
	STAR IDENT
	|
	BAND IDENT
	;

expression_statement returns[IStatement stmt]
	{ stmt = null; PostfixCondition pfc = null; }
	:
	(
		test
		(	
			augassign test
			|
			(ASSIGN test)+
		)?
		|
		compound		
	)
	(pfc=postFixCondition)?
	|
	stmt=method_def_statement
	;

augassign
    : PLUS_ASSIGN
	| MINUS_ASSIGN
	| STAR_ASSIGN
	| DIV_ASSIGN
	| MOD_ASSIGN
	| BAND_ASSIGN
	| BOR_ASSIGN
	| BXOR_ASSIGN
//	| LEFTSHIFTEQUAL
//	| RIGHTSHIFTEQUAL
//	| DOUBLESTAREQUAL
//	| DOUBLESLASHEQUAL
	;

postFixCondition returns[PostfixCondition pfc]
	{ pfc = null; }
	:
	(
		("if" { pfc = new PostfixCondition(PostfixConditionType.If); }
		|"unless" { pfc = new PostfixCondition(PostfixConditionType.Unless); }
		|"while" { pfc = new PostfixCondition(PostfixConditionType.While); }
		|"until" { pfc = new PostfixCondition(PostfixConditionType.Until); }
		) 
		test
	)
	;

type_name
	:
	IDENT^ (type)?
	;

type
	:
	qualified_symbol // (LBRACK RBRACK)? // We do not support multi-dimensional arrays yet
	;

qualified_name
	:
	IDENT 
    (options{greedy=true;}:DOT! IDENT)*
    ;

qualified_symbol
	:
	SYMBOL
    (options{greedy=true;}:DOT! IDENT)*
    ;

// Expressions

testlist
    :   
    test (options {greedy=true;}:COMMA! test)*
    // (options {greedy=true;}:COMMA)?
    ;

test
	: 
	and_test ("or" and_test)*
	| 
	lambda
	|
	block
	| 
	raise
	;

lambda returns[LambdaExpression lexp]
	{ BlockExpression bexp=null;
	  lexp = null; }
	:
	"lambda"^ bexp=block
	{ lexp = new LambdaExpression(bexp); }
	;

block returns[BlockExpression bexp]
	{ bexp = new BlockExpression(); }
	:
	(
		(DO^ (statement_term)? (blockargs[bexp])? (statement_term)? suite[bexp.Statements] END!)
		|
		(LCURLY^ (statement_term)? (blockargs[bexp])? (statement_term)? suite[bexp.Statements] RCURLY!)
	)
	;

raise
	:
	"raise" expression
	;

blockargs[BlockExpression bexp]
	:
	BOR^ IDENT (options {greedy=true;}:COMMA! IDENT)* BOR!
	;

compound returns[CompoundStatement cstmt]
	{ cstmt = new CompoundStatement(); }
	:
	(DO^|BEGIN^) statement_term
	suite[cstmt.Statements]
	END! 
	;

and_test
	: not_test ("and" not_test)*
	;

not_test
	: "not" not_test
	| comparison
	;

comparison
	: 
	expression (comp_op expression)*
	;

comp_op: 
	LT	|	GT	|	EQUAL	|	GE	|	LE	|	NOT_EQUAL
//	|ALT_NOTEQUAL
// 	|"in"
//	|"not" "in"
//	|"is"
//	|"is" "not"
	;

expressionList
 	:
 	expression (options {greedy=true;}:COMMA! expression)*
 	;	

expression
	: 
	xor_expr (BXOR xor_expr)*
	// {#expression = #(#[EXPR,"expression"],#expression);}
	;

xor_expr
	: 
	and_expr (BOR and_expr)*
	;

and_expr
	: 
	// shift_expr (BAND shift_expr)*
	arith_expr (BAND arith_expr)*
	;

// shift_expr: arith_expr ((LEFTSHIFT|RIGHTSHIFT) arith_expr)*
//	;

arith_expr
	: 
	term ((PLUS|MINUS) term)*
	;

term: 
	factor ((STAR | SLASH | PERCENT) factor)*
	;

factor
	: 
	(PLUS|MINUS|BNOT) factor
	| 
	primary
	;

primary
	:
	atom (trailer)* // (options {greedy=true;}:DOUBLESTAR factor)?
	;

atom: 
	(interval) => interval
	| LPAREN (testlist)? RPAREN
	| LBRACK (listmaker)? RBRACK
	| LCURLY (dictmaker)? RCURLY
//	| BACKQUOTE testlist BACKQUOTE
	| IDENT
	| NUM_INT
	| NUM_LONG
	| NUM_FLOAT
	| SYMBOL
	| STRING_LITERAL
	| CHAR_LITERAL
//    | LONGINT
//    | FLOAT
//    | COMPLEX
//	| (STRING)+
	;

trailer: 
	LPAREN (arglist)? RPAREN
	| 
	LBRACK subscriptlist RBRACK
	| 
	DOT IDENT
	;

interval
	:
	LPAREN expression (DOTDOT|DOTDOTDOT) expression RPAREN
	;

subscriptlist
    :   
    subscript (options {greedy=true;}:COMMA subscript)*
	;

subscript
	: 
    expression
    ;

arglist: 
	argument (options {greedy=true;}:COMMA! argument)*
//	{#arglist = #[ELIST,"ELIST"];}
	/*
        ( COMMA
          ( STAR test (COMMA DOUBLESTAR test)?
          | DOUBLESTAR test
          )?
        )?
    |   STAR test (COMMA DOUBLESTAR test)?
    |   DOUBLESTAR test
    */
    ;

argument : test //(ASSIGN test)?
         ;

listmaker
	: 
	test (options {greedy=true;}:COMMA! test)* 
	;

dictmaker
    :   
    expression MAPASSIGN test
    (options {greedy=true;}:COMMA! expression MAPASSIGN test)* 
    ;




class RookLexer extends Lexer;

options {
	exportVocab=Rook;      
	testLiterals=false;    // don't automatically test for literals
	k=4;                   // four characters of lookahead
	charVocabulary='\u0003'..'\uFFFF';
	// without inlining some bitset tests, couldn't do unicode;
	// I need to make ANTLR generate smaller bitsets; see
	// bottom of JavaLexer.java
	codeGenBitsetTestThreshold=20;
}
{
    private int lastToken;

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

	protected override IToken makeToken(int type) {
		lastToken = type;
		return base.makeToken(type);
	}
}

// OPERATORS
QUESTION		:	'?'		;
LPAREN			:	'('		;
RPAREN			:	')'		;
LBRACK			:	'['		;
RBRACK			:	']'		;
LCURLY			:	'{'		;
RCURLY			:	'}'		;
COLON			:	':'		;
COMMA			:	','		;
DOT				:	'.'		;
DOTDOT			:	".."	;
DOTDOTDOT		:	"..."	;
ASSIGN			:	'='		;
EQUAL			:	"=="	;
LNOT			:	'!'		;
BNOT			:	'~'		;
NOT_EQUAL		:	"!="	;
DIV				:	'/'		;
DIV_ASSIGN		:	"/="	;
PLUS			:	'+'		;
PLUS_ASSIGN		:	"+="	;
INC				:	"++"	;
MINUS			:	'-'		;
MINUS_ASSIGN	:	"-="	;
DEC				:	"--"	;
STAR			:	'*'		;
STAR_ASSIGN		:	"*="	;
MOD				:	'%'		;
MOD_ASSIGN		:	"%="	;
SR				:	">>"	;
SR_ASSIGN		:	">>="	;
BSR				:	">>>"	;
BSR_ASSIGN		:	">>>="	;
GE				:	">="	;
GT				:	">"		;
SL				:	"<<"	;
SL_ASSIGN		:	"<<="	;
LE				:	"<="	;
LT				:	'<'		;
BXOR			:	'^'		;
BXOR_ASSIGN		:	"^="	;
BOR				:	'|'		;
BOR_ASSIGN		:	"|="	;
LOR				:	"||"	;
BAND			:	'&'		;
BAND_ASSIGN		:	"&="	;
LAND			:	"&&"	;
SEMI			:	';'		;
MAPASSIGN		:	"=>"	;

NEWLINE
	options { paraphrase = "a new line"; }
	: 
	( '\r' '\n' | '\n' | '\r') {
    newline();
    $setType(getProperType());
	};

// Single-line comments
SL_COMMENT
	options { paraphrase = "comments"; }
	:	'#'
		(~('\n'|'\r'))* ('\n'|'\r'('\n')?)
		{$setType(Token.SKIP); newline();}
	;

WS
	:	
	( ' ' | '\t' | '\f' )+
	{ $setType(Token.SKIP); }
	;


// multiple-line comments
// 
// ML_COMMENT
// 	:	"/*"
// 		(	/*	'\r' '\n' can be matched in one alternative or by matching
// 				'\r' in one iteration and '\n' in another.  I am trying to
// 				handle any flavor of newline that comes in, but the language
// 				that allows both "\r\n" and "\r" and "\n" to all be valid
// 				newline is ambiguous.  Consequently, the resulting grammar
// 				must be ambiguous.  I'm shutting this warning off.
// 			 */
// 			options {
// 				generateAmbigWarnings=false;
// 			}
// 		:
// 			{ LA(2)!='/' }? '*'
// 		|	'\r' '\n'		{newline();}
// 		|	'\r'			{newline();}
// 		|	'\n'			{newline();}
// 		|	~('*'|'\n'|'\r')
// 		)*
// 		"*/"
// 		{$setType(Token.SKIP);}
// 	;

// character literals
CHAR_LITERAL
	:	'\'' ( ESC | ~'\'' ) '\''
	;

// string literals
STRING_LITERAL
	:	'"' (ESC|~('"'|'\\'))* '"'
	;

// escape sequence -- note that this is protected; it can only be called
//   from another lexer rule -- it will not ever directly return a token to
//   the parser
// There are various ambiguities hushed in this rule.  The optional
// '0'...'9' digit matches should be matched here rather than letting
// them go back to STRING_LITERAL to be matched.  ANTLR does the
// right thing by matching immediately; hence, it's ok to shut off
// the FOLLOW ambig warnings.
protected
ESC
	:	'\\'
		(	'n'
		|	'r'
		|	't'
		|	'b'
		|	'f'
		|	'"'
		|	'\''
		|	'\\'
		|	('u')+ HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
		|	'0'..'3'
			(
				options {
					warnWhenFollowAmbig = false;
				}
			:	'0'..'7'
				(
					options {
						warnWhenFollowAmbig = false;
					}
				:	'0'..'7'
				)?
			)?
		|	'4'..'7'
			(
				options {
					warnWhenFollowAmbig = false;
				}
			:	'0'..'7'
			)?
		)
	;

// hexadecimal digit (again, note it's protected!)
protected
HEX_DIGIT
	:	('0'..'9'|'A'..'F'|'a'..'f')
	;

// a dummy rule to force vocabulary to be all characters (except special
//   ones that ANTLR uses internally (0 to 2)
protected
VOCAB
	:	'\3'..'\377'
	;

// an identifier.  Note that testLiterals is set to true!  This means
// that after we match the rule, we look in the literals table to see
// if it's a literal or really an identifer
IDENT
	options {testLiterals=true;}
	:	('a'..'z'|'A'..'Z'|'_'|'$') ('a'..'z'|'A'..'Z'|'_'|'0'..'9'|'$')* ('?'|'!')?
	;

SYMBOL
	:
	COLON IDENT 
	;

// a numeric literal
/*
NUM_INT
	{boolean isDecimal=false; Token t=null;}
    :   '.' {_ttype = DOT;}
            (	('0'..'9')+ (EXPONENT)? (f1:FLOAT_SUFFIX {t=f1;})?
                {
				if (t != null && t.getText().toUpperCase().indexOf('F')>=0) {
                	_ttype = NUM_FLOAT;
				}
				else {
                	_ttype = NUM_DOUBLE; // assume double
				}
				}
            )?

	|	(	'0' {isDecimal = true;} // special case for just '0'
			(	('x'|'X')
				(											// hex
					// the 'e'|'E' and float suffix stuff look
					// like hex digits, hence the (...)+ doesn't
					// know when to stop: ambig.  ANTLR resolves
					// it correctly by matching immediately.  It
					// is therefor ok to hush warning.
					options {
						warnWhenFollowAmbig=false;
					}
				:	HEX_DIGIT
				)+
			|	('0'..'7')+									// octal
			)?
		|	('1'..'9') ('0'..'9')*  {isDecimal=true;}		// non-zero decimal
		)
		(	('l'|'L') { _ttype = NUM_LONG; }

		// only check to see if it's a float if looks like decimal so far
		|	{isDecimal}?
            (   '.' ('0'..'9')* (EXPONENT)? (f2:FLOAT_SUFFIX {t=f2;})?
            |   EXPONENT (f3:FLOAT_SUFFIX {t=f3;})?
            |   f4:FLOAT_SUFFIX {t=f4;}
            )
            {
			if (t != null && t.getText().toUpperCase() .indexOf('F') >= 0) {
                _ttype = NUM_FLOAT;
			}
            else {
	           	_ttype = NUM_DOUBLE; // assume double
			}
			}
        )?
	;

// a couple protected methods to assist in matching floating point numbers
protected
EXPONENT
	:	
	('e'|'E') ('+'|'-')? ('0'..'9')+
	;

protected
FLOAT_SUFFIX
	:	
	'f'|'F'|'d'|'D'
	;
*/

NUMBER
	:
	( Int DOTDOT ) => Int {$setType(NUM_INT);}
	|
	( Int DOTDOTDOT ) => Int {$setType(NUM_INT);}
	|
	// Hex
    '0' ('x' | 'X') ( '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' )+ {$setType(NUM_INT);} ('l' | 'L' {$setType(NUM_LONG);})?
//    |   // Octal
//        '0' Int {$setType(NUM_INT);}
//		(   FloatTrailer
//        |   ('l' | 'L')	{$setType(NUM_LONG);}
//        )?
    |  
	Int {$setType(NUM_INT);}
	(   
		FloatTrailer {$setType(NUM_FLOAT);}
    	|   
    	('l' | 'L')	{$setType(NUM_LONG);}
    )?
//    |   // Int or float
//        (	NonZeroDigit (Int)?
//			(	('l' | 'L')	{$setType(NUM_LONG);}
//			|	FloatTrailer {$setType(NUM_FLOAT);}
//			|	{$setType(NUM_INT);}
//			)
//		)
//	|	'.' Int (Exponent)? {$setType(FLOAT);}
//    |   '.' {$setType(DOT);} // DOT (non number; e.g., field access)
	;

protected
Int : ( '0' .. '9' )+ ;

protected
NonZeroDigit : '1' .. '9' ;

protected
FloatTrailer
	:	
//	'.'
//	|	
	'.' Int (Exponent)?
//	|   
//	Exponent
	;

protected
Exponent
	:	('e' | 'E') ( '+' | '-' )? Int
	;

/** Consume a newline and any whitespace at start of next line */
CONTINUED_LINE
	:	
	'\\' ('\r')? '\n' (' '|'\t')* { newline(); $setType(Token.SKIP); }
	;

