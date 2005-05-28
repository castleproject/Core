header 
{
	// using CommonAST					= antlr.CommonAST; 
	using System.Text;
	using System.Collections;
	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Services;
}
options 
{	
	language = "CSharp";
	namespace = "Castle.Rook.Compiler.Parser";
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
}

protected 
statement_term!
    :
    (options { greedy=true; }:STATEMENT_END | SEMI)
    ;

protected 
nothing
	:
	(options { greedy=true; generateAmbigWarnings=false; }:STATEMENT_END)?
	;

compilationUnit returns[CompilationUnit comp]
	{ comp = new CompilationUnit(); }
	:
	nothing
	(
		("namespace" qualified_name) => namespace_declaration[comp.Namespaces]
		|
		suite[comp.Statements]
	)
	nothing
	EOF
	;

namespace_declaration[IList namespaces]
	options { defaultErrorHandler=true; }
	{ NamespaceDeclaration nsdec = new NamespaceDeclaration(); 
	  namespaces.Add(nsdec); String qn = null; }
	:
	"namespace" qn=qualified_name statement_term
	{ nsdec.Name = qn; }
	suite[nsdec.Statements]
	END
	;

suite[IList stmts]
	{ IStatement stmt = null; }
	:
	(stmt=statement { if (stmt != null) stmts.Add(stmt); } )*
	;

statement returns[IStatement stmt]
	{ stmt = null; }
	:
	access_level
	(
		(declaration_statement) => stmt=declaration_statement
		|
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
		stmt=flow_statements
		|
		stmt=if_statement
		|
		(unless_statement) => stmt=unless_statement
		|
		stmt=expression_statement
	)
	statement_term
	;

while_statement returns[RepeatStatement rs]
	{ rs = null; IExpression testexp; }
	:
	WHILE^ testexp=test (DO|statement_term) 
	{ rs = new RepeatStatement(RepeatType.While, testexp); }
	suite[rs.Statements] END
	;

until_statement returns[RepeatStatement rs]
	{ rs = null; IExpression testexp; }
	:
	UNTIL^ testexp=test (DO|statement_term) 
	{ rs = new RepeatStatement(RepeatType.Until, testexp); }
	suite[rs.Statements] END
	;

for_statement returns[ForStatement fors]
	{ fors = new ForStatement(); VariableReferenceExpression vre = null;
	  IExpression evalexp; }
	:
	"for" vre=varref { fors.AddVarRef(vre); } /* (COMMA vre=varref { fors.AddVarRef(vre); } )*  */
	"in" evalexp=test { fors.EvalExp = evalexp; } (DO|statement_term) 
	suite[fors.Statements]
	END
	;

if_statement returns[IfStatement ifs]
	{ ifs = new IfStatement(IfType.If); IfStatement inner = ifs; 
	  IExpression testexp; }
	: 
	"if" testexp=test ("then"|statement_term) { ifs.Condition = testexp; } 
	suite[ifs.TrueStatements] 
	("elsif" testexp=test ("then"|statement_term) { inner = new IfStatement(IfType.If); inner.Condition = testexp; }
	suite[inner.TrueStatements])*  
	("else" statement_term suite[inner.FalseStatements])?
	END
	;

unless_statement returns[IfStatement ifs]
	{ ifs = new IfStatement(IfType.Unless); IExpression testexp; }
	: 
	"unless" testexp=test ("then"|statement_term) { ifs.Condition = testexp; } 
	suite[ifs.TrueStatements]
	("else" statement_term suite[ifs.FalseStatements])?
	END
	;

flow_statements returns[IStatement stmt]
	{ stmt = null; }
	:
	"redo"	{ stmt = new RedoStatement(); }
	|
	"break"	{ stmt = new BreakStatement(); }
	|
	"next"	{ stmt = new NextStatement(); }
	|
	"retry"	{ stmt = new RetryStatement(); }
	;

protected
access_level
	:
	(
		"public"^    (COLON)? { currentAccessLevel = AccessLevel.Public; }
		|
		"private"^   (COLON)? { currentAccessLevel = AccessLevel.Private; }
		|
		"protected"^ (COLON)? { currentAccessLevel = AccessLevel.Protected; }
		| 
		"internal"^  (COLON)? { currentAccessLevel = AccessLevel.Internal; }
		|
			/* nothing - inherits the access level defined previously */  
	)
	;

declaration_statement returns [VariableDeclarationStatement vdstmt]
	{ vdstmt = new VariableDeclarationStatement(); TypeDeclarationExpression tdstmt = null; }
	:
 	tdstmt=type_name_withtype (COMMA tdstmt=type_name_withtype)* (ASSIGN test (COMMA test)* )?
 	  {  }
 	;

type_def_statement returns [TypeDefinitionStatement tdstmt]
	{ tdstmt = null; }
	:
	tdstmt=class_def_statement
	;

class_def_statement returns [TypeDefinitionStatement tdstmt]
	{ tdstmt = null; }	// TODO: Create ClassDefinitionStatement 
						// and support modifiers like visibility and abstract etc
	:
	CLASS t:IDENT 
	{ tdstmt = new TypeDefinitionStatement( t.getText() ); }
	( (LTHAN|SL) qualified_name (COMMA qualified_name)* )? statement_term
	suite[tdstmt.Statements]
	END
	;

method_def_statement returns [MethodDefinitionStatement mdstmt]
	{ mdstmt = null; String qn = null; TypeReference retType = null; }
	:
	DEF^ qn=qualified_name 
	{ mdstmt = new MethodDefinitionStatement(qn); }
	LPAREN (methodParams[mdstmt])? RPAREN (retType=type)? statement_term
	{ mdstmt.ReturnType = retType; }
	suite[mdstmt.Statements]
	END
	;

// operator_def_statement
//	:
//	OPERATOR^ qualified_name LPAREN (methodParams)? RPAREN (type)? statement_term
//	suite
//	END
//	;

methodParams[MethodDefinitionStatement mdstmt]
	:
	methodParam[mdstmt] (COMMA methodParam[mdstmt])*
	;

methodParam[MethodDefinitionStatement mdstmt]
	{ IExpression exp = null; TypeDeclarationExpression typeName = null; }
	:
	typeName=type_name (ASSIGN exp=expression { typeName.InitExp = exp; } )?
	|
	STAR type_name		// TODO: Infer List or array
	|
	BAND IDENT			// TODO: block/delegate signature
	;

expression_statement returns[IStatement stmt]
	{ stmt = null; PostfixCondition pfc = null; IExpression exp = null; IExpression rhs = null;
	  AugType rel = AugType.Undefined; }
	:
	(
		exp=test
		(	
			rel=augassign rhs=test	{ exp = new AugAssignmentExpression(exp, rhs, rel); }
			|
			(ASSIGN rhs=test		{ exp = new AssignmentExpression(exp, rhs); } )+
		)?
		|
		exp=compound		
	)
	(pfc=postFixCondition { exp.PostFixStatement = pfc; } )?
	{ stmt = new ExpressionStatement(exp); }
	|
	stmt=method_def_statement
	;

augassign returns [AugType rel]
	{ rel = AugType.Undefined; }
    : PLUS_ASSIGN		{ rel = AugType.PlusAssign; }
	| MINUS_ASSIGN		{ rel = AugType.MinusAssign; }
	| STAR_ASSIGN		{ rel = AugType.MultAssign; }
	| DIV_ASSIGN		{ rel = AugType.DivAssign; }
	| MOD_ASSIGN		{ rel = AugType.ModAssign; }
	| BAND_ASSIGN		{ rel = AugType.BitwiseAndAssign; }
	| BOR_ASSIGN		{ rel = AugType.BitwiseOrAssign; }
	| BXOR_ASSIGN		{ rel = AugType.BitwiseXorAssign; }
//	| LEFTSHIFTEQUAL
//	| RIGHTSHIFTEQUAL
//	| DOUBLESTAREQUAL
//	| DOUBLESLASHEQUAL
	;

postFixCondition returns[PostfixCondition pfc]
	{ pfc = null; IExpression exp; }
	:
	(
		("if"			{ pfc = new PostfixCondition(PostfixConditionType.If); }
		|"unless"		{ pfc = new PostfixCondition(PostfixConditionType.Unless); }
		|"while"		{ pfc = new PostfixCondition(PostfixConditionType.While); }
		|"until"		{ pfc = new PostfixCondition(PostfixConditionType.Until); }
		) 
		exp=test		{ pfc.Condition = exp; }
	)
	;

type_name_withtype returns [TypeDeclarationExpression tdexp]
	{ tdexp = null; String n=null; TypeReference tr = null; }
	:
	n=name tr=type
	{ tdexp = new TypeDeclarationExpression(n, tr); }
	;

type_name returns [TypeDeclarationExpression tdexp]
	{ tdexp = null; String n=null; TypeReference tr = null; }
	:
	n=name (tr=type)? 
	{ tdexp = new TypeDeclarationExpression(n, tr); }
	;

type returns [ TypeReference tr ]
	{ tr = null; String n; }
	:
	n=qualified_symbol	{ tr = ObtainTypeReference(n); }
	
	// (LBRACK RBRACK)? // We do not support multi-dimensional arrays yet
	;

name returns[String name]
	{ name = null; }
	:
	t1:IDENT		{ name = t1.getText(); }
	|
	t2:STATICIDENT	{ name = t2.getText(); }
	|
	t3:INSTIDENT	{ name = t3.getText(); }
    ;

qualified_name returns[String name]
	{ name = null; }
	:
	t:IDENT { name = t.getText(); }
    (options{greedy=true;}:DOT t2:IDENT { name += "." + t2.getText(); } )*
    ;

qualified_symbol returns[String name]
	{ name = null; }
	:
	t:SYMBOL { name = t.getText(); }
    (options{greedy=true;}:DOT t2:IDENT { name += "." + t2.getText(); } )*
    ;

// Expressions

lambda returns [LambdaExpression lexp]
	{ BlockExpression bexp=null; lexp = null; }
	:
	"lambda"^ bexp=block
	{ lexp = new LambdaExpression(bexp); }
	;

block returns [BlockExpression bexp]
	{ bexp = new BlockExpression(); }
	:
	(
		(DO^ (statement_term)? (blockargs[bexp])? (statement_term)? suite[bexp.Statements] END)
		|
		(LCURLY^ (statement_term)? (blockargs[bexp])? (statement_term)? suite[bexp.Statements] RCURLY)
	)
	;

raise returns [RaiseExpression rexp]
	{ rexp = null; IExpression exp; }
	:
	"raise" exp=expression	{ rexp = new RaiseExpression(exp); }
	;

yield returns [YieldExpression rexp]
	{ rexp = null; ExpressionCollection expColl; }
	:
	"yield" expColl=expressionList	{ rexp = new YieldExpression(expColl); }
	;

blockargs[BlockExpression bexp]
	{ bexp = new BlockExpression(); TypeDeclarationExpression tdexp = null; }
	:
	BOR tdexp=type_name 
	  { bexp.AddBlockArgument(tdexp); }
	(options {greedy=true;}:COMMA tdexp=type_name { bexp.AddBlockArgument(tdexp); } )* 
	BOR
	;

compound returns[CompoundExpression cexp]
	{ cexp = new CompoundExpression(); }
	:
	(DO^|BEGIN^) statement_term
	suite[cexp.Statements]
	END 
	;

// testlist
//     :
//     test (options {greedy=true;}:COMMA test)*
    // (options {greedy=true;}:COMMA)?
//     ;

test returns [IExpression exp]
	{ exp = null; IExpression rhs = null; }
	: 
	exp=and_test ("or" rhs=and_test { exp = new BinaryExpression(exp, rhs, BinaryOp.Or); })*
	| 
	exp=lambda
	|
	exp=block
	| 
	exp=raise
	| 
	exp=yield
	;

and_test returns [IExpression exp]
	{ exp = null; IExpression rhs = null; }
	: 
	exp=not_test ("and" rhs=not_test { exp = new BinaryExpression(exp, rhs, BinaryOp.And); })*
	;

not_test returns [IExpression exp]
	{ exp = null; IExpression inner = null; }
	: 
	("not"|LNOT) inner=not_test { exp = new UnaryExpression(inner, UnaryOp.Not); }
	| 
	exp=comparison
	;

comparison returns [IExpression exp]
	{ exp = null; IExpression rhs = null; BinaryOp op = BinaryOp.Undefined; }
	: 
	exp=expression (op=comp_op rhs=expression { exp = new BinaryExpression(exp, rhs, op); } )*
	;

comp_op returns [BinaryOp op]
	{ op = BinaryOp.Undefined; }
	: 
	LTHAN		{ op = BinaryOp.LessThan; }
	|
	GT			{ op = BinaryOp.GreaterThan; }
	|
	EQUAL		{ op = BinaryOp.Equal; }
	|
	GE			{ op = BinaryOp.GreaterEqual; }
	|
	LE			{ op = BinaryOp.LessEqual; }
	|
	NOT_EQUAL	{ op = BinaryOp.NotEqual; }
	
//	|ALT_NOTEQUAL
// 	|"in"
//	|"not" "in"
//	|"is"
//	|"is" "not"
	;

expressionList returns [ExpressionCollection expColl]
	{ expColl = new ExpressionCollection(); IExpression exp = null; }
 	:
 	exp=expression { expColl.Add(exp); } 
 	(options {greedy=true;}:COMMA exp=expression { expColl.Add(exp); } )*
 	;	

expression returns [IExpression exp]
	{ exp = null; IExpression rhs = null; }
	: 
	exp=xor_expr (BXOR rhs=xor_expr { exp = new BinaryExpression(exp, rhs, BinaryOp.Xor); })*
	;

xor_expr returns [IExpression exp]
	{ exp = null; IExpression rhs = null; }
	: 
	exp=and_expr (BOR rhs=and_expr { exp = new BinaryExpression(exp, rhs, BinaryOp.Or2); })*
	;

and_expr returns [IExpression exp]
	{ exp = null; IExpression rhs = null; }
	: 
	// shift_expr (BAND shift_expr)*
	exp=arith_expr (BAND rhs=arith_expr { exp = new BinaryExpression(exp, rhs, BinaryOp.And2); })*
	;

// shift_expr: arith_expr ((LEFTSHIFT|RIGHTSHIFT) arith_expr)*
//	;

arith_expr returns [IExpression exp]
	{ exp = null; IExpression rhs = null; }
	: 
	exp=term ((t:PLUS|MINUS) rhs=term { exp = new BinaryExpression(exp, rhs, t != null ? BinaryOp.Plus : BinaryOp.Minus); })*
	;

term returns [IExpression exp]
	{ exp = null; IExpression rhs = null; BinaryOp op = BinaryOp.Undefined; }
	: 
	exp=unary 
	(
		(
			STAR		{ op = BinaryOp.Mult; }
			| SLASH 	{ op = BinaryOp.Div; }
			| PERCENT	{ op = BinaryOp.Mod; }
		) 
		rhs=unary { exp = new BinaryExpression(exp, rhs, op); }
	)*
	;

unary returns [IExpression exp]
	{ exp = null; IExpression inner = null; UnaryOp op = UnaryOp.Plus; }
	: 
	(
	    PLUS	{ op = UnaryOp.Plus; }
	  | MINUS	{ op = UnaryOp.Minus; }
	  | BNOT	{ op = UnaryOp.BitwiseNot; }
	) 
	inner=unary { exp = new UnaryExpression(inner, op); }
	| 
	exp=primary
	;

primary returns [IExpression exp]
	{ exp = null; }
	:
	exp=atom (exp=trailer[exp])* 
	;

atom returns [IExpression exp]
	{ exp = null; }
	: 
	(range) => exp=range
	| LPAREN (exp=test)? RPAREN // LPAREN (testlist)? RPAREN
	| LBRACK (exp=listmaker)? RBRACK
	| LCURLY (exp=dictmaker)? RCURLY
//	| BACKQUOTE testlist BACKQUOTE
	| exp=varref
	| exp=constantref
	;

varref returns [VariableReferenceExpression vre]
	{ vre = null; }
	:
	t1:IDENT 
	  { vre = new VariableReferenceExpression(t1.getText(), VariableReferenceType.LocalOrArgument); }
	|
	t2:STATICIDENT  
	  { vre = new VariableReferenceExpression(t2.getText(), VariableReferenceType.StaticField); }
	| 
	t3:INSTIDENT  
	  { vre = new VariableReferenceExpression(t3.getText(), VariableReferenceType.InstanceField); }
	;

constantref returns [LiteralReferenceExpression lre]
	{ lre = null; }
	:
	| t1:NUM_INT
	  { lre = new LiteralReferenceExpression(t1.getText(), LiteralReferenceType.IntLiteral); }
	| t2:NUM_LONG
	  { lre = new LiteralReferenceExpression(t2.getText(), LiteralReferenceType.LongLiteral); }
	| t3:NUM_FLOAT
	  { lre = new LiteralReferenceExpression(t3.getText(), LiteralReferenceType.FloatLiteral); }
	| t4:SYMBOL
	  { lre = new LiteralReferenceExpression(t4.getText(), LiteralReferenceType.SymbolLiteral); }
	| t5:STRING_LITERAL
	  { lre = new LiteralReferenceExpression(t5.getText(), LiteralReferenceType.StringLiteral); }
	| t6:CHAR_LITERAL
	  { lre = new LiteralReferenceExpression(t6.getText(), LiteralReferenceType.CharLiteral); }
//    | LONGINT
//    | FLOAT
//    | COMPLEX
//	| (STRING)+
	;

trailer[IExpression inner] returns [IExpression exp]
	{ exp = null; ExpressionCollection args = null; }
	: 
	LPAREN (args=arglist)? RPAREN { exp = new MethodInvocationExpression(inner, args); }
	| 
	LBRACK subscriptlist RBRACK // TODO: Array/list/indexer access
	| 
	DOT IDENT { exp = new MemberAccessExpression(inner); }
	;

range returns[IExpression rex]
	{ rex = null; IExpression lhs = null; IExpression rhs = null; }
	:
	LPAREN lhs=expression (t:DOTDOT|DOTDOTDOT) rhs=expression RPAREN
	{ rex = new RangeExpression(lhs, rhs, t != null); }
	;

subscriptlist
    :   
    subscript (options {greedy=true;}:COMMA subscript)*
	;

subscript
	: 
    expression
    ;

arglist returns [ExpressionCollection expcoll]
	{ IExpression exp; expcoll = new ExpressionCollection(); }
	: 
	exp=argument		{ expcoll.Add(exp); }
	(options {greedy=true;}:COMMA exp=argument { expcoll.Add(exp); } )*
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

argument returns [IExpression exp]
	{ exp = null; }
	: 
	exp=test //(ASSIGN test)?
	;

listmaker returns [ListExpression exp]
	{ exp = new ListExpression(); IExpression item; }
	: 
	item=test 
	  { exp.Add(item); }
	(options {greedy=true;}:COMMA item=test { exp.Add(item); } )* 
	;

dictmaker returns [DictExpression exp]
	{ exp = new DictExpression(); IExpression key, value; }
    :   
    key=expression MAPASSIGN value=test
      { exp.Add(key, exp); }
    (options {greedy=true;}:COMMA key=expression MAPASSIGN value=test 
      { exp.Add(key, exp); } )* 
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
LTHAN			:	'<'		;
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
	SL_NEWLINE 
	{
		newline();
		$setType(getProperType());
	}
	(   
		(SL_NEWLINE | WS | SL_COMMENT)+
	)?
	;

/*
	options { paraphrase = "a new line"; }
	: 
	( '\r' '\n' | '\n' | '\r') 
	{
		newline();
		$setType(getProperType());
	}
	(   (WS | SL_COMMENT)+   )?
	;

*/

protected 
SL_NEWLINE
	:
	(   options {generateAmbigWarnings=false;}:
		"\r\n"
		|   
		'\r'
		|   
		'\n'
	)
    ;

SL_COMMENT
	options { paraphrase = "comments"; }
	:	'#'
		( options {  greedy = true;  }: ~('\n'|'\r'|'\uffff') )*
		{$setType(Token.SKIP); }
	;

WS
	:	
	( options { greedy=true; }:' ' | '\t' | '\f' )+
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

IDENT
	options {testLiterals=true;}
	:	('a'..'z'|'A'..'Z'|'_'|'$') ('a'..'z'|'A'..'Z'|'_'|'0'..'9'|'$')* ('?'|'!')?
	;

INSTIDENT
	:	
	'@' IDENT
	;

STATICIDENT
	:	
	"@@" IDENT
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

