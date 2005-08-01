header 
{
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
	analyzerDebug = false;
	codeGenDebug = false;
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
	
	private bool withinStatic, withinVirtual, withinAbstract, withinNew, withinOverride;
	
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
}

qualified_name returns [String ident]
	{ String name = String.Empty; ident = null; }
	:
	t:IDENT { name = t.getText(); }
	( options{greedy=true;}:
		(
			DOT			{ name += "."; }
			| 
			COLONCOLON	{ name += "::"; }
		)
		t2:IDENT { name += t2.getText(); } 
	)*
    { ident = name; }
    ;

identifier returns [Identifier ident]
	{ ident = null; TypeReference tr = null; }
	:
	ident=name (tr=type { ident.TypeReference = tr; })? 
	;

identifier_withtype returns [Identifier ident]
	{ ident = null; TypeReference tr = null; }
	:
	ident=name tr=type	{ ident.TypeReference = tr; }
	;

type returns [ TypeReference tr ]
	{ tr = null; String n; }
	:
	"as" n=qualified_name	
	{ 
	  tr = new TypeReference(n); 
	}
	// (LBRACK RBRACK)? // We do not support multi-dimensional arrays yet
	;

name returns [Identifier ident]
	{ ident = null; }
	:
	t1:IDENT		{ ident = new OpaqueIdentifier(t1.getText()); }
	|
	t2:STATICIDENT	{ ident = new StaticVarIdentifier(t2.getText()); }
	|
	t3:INSTIDENT	{ ident = new InstanceVarIdentifier(t3.getText()); }
    ;

// qualified_symbol returns[String name]
// 	{ name = null; }
// 	:
// 	t:SYMBOL { name = t.getText().Substring(1); }
//     (options{greedy=true;}:DOT t2:IDENT { name += "." + t2.getText(); } )*
//     ;

protected 
statement_term!
    :
    (options { greedy=true; }:(STATEMENT_END | SEMI | EOF))
    ;

protected 
nothing
	:
	(options { greedy=true; generateAmbigWarnings=false; }:(STATEMENT_END|EOF) )?
	;

sourceUnit[CompilationUnit cunit] returns[SourceUnit unit]
	{ 
	  topLevelScope = cunit.DefiningSymbolTable;
	  unit = new SourceUnit(cunit, getFilename()); 
	  PushScope(unit, ScopeType.SourceUnit); 
	}
	:
	nothing
	(
		("namespace" qualified_name) => namespace_declaration[unit.Namespaces]
		|
		suite[unit.Statements]
	)
	nothing
	EOF
	{ 
	  cunit.SourceUnits.Add(unit); 
	  PopScope(); 
	  if (!ErrorReport.HasErrors && scopes.Count != 0) 
	    ErrorReport.Error("Invalid scope count. " + 
			"Something seems to be very wrong. Contact Castle's team and report the " + 
			"code that caused this error.");  
	}
	;

namespace_declaration[IList namespaces]
	options { defaultErrorHandler=true; }
	{ 
	  NamespaceDescriptor nsdec = new NamespaceDescriptor(); 
	  PushScope(nsdec, ScopeType.Namespace);
	  namespaces.Add(nsdec); String qn = null;
	  TypeDefinitionStatement typeDef = null;
	}
	:
	t:"namespace" qn=qualified_name statement_term
	{ nsdec.Name = qn; }
	(typeDef=type_def_statement { nsdec.TypeDefinitions.Add(typeDef); } )*
	END	{ PopScope(); }
	;

suite[IList stmts]
	{ IStatement stmt = null; }
	:
	(stmt=statement { if (stmt != null) stmts.Add(stmt); } )*
	;

type_suite[IList stmts]
	{ IStatement stmt = null; }
	:
	(access_level 
		(
			stmt=statement { if (stmt != null) stmts.Add(stmt); } 
			|
			method_scope[stmts]
		)
	)*
	;

// class << self
//   def method <- will be static
// end
//
// class << override
//   def method <- will be override
// end
//
// class << new
//   def method <- will be new
// end
//
// class << virtual
//   def method <- will be virtual
// end
protected 
method_scope[IList stmts]
	{ IStatement stmt = null; int index=0; }
	:
	CLASS SL 
	
	(
	  "self"		{ withinStatic = true; }
	  |
	  "override"	{ index = 1; withinOverride = true; }
	  |
  	  "abstract"	{ index = 2; withinAbstract = true; }
	  |
  	  "new"			{ index = 3; withinNew = true; }
	  |
  	  "virtual"		{ index = 4; withinVirtual = true; }
	)

	statement_term

    (access_level stmt=statement { if (stmt != null) stmts.Add(stmt); } )*
	
	END
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
		withinVirtual = false;
	}
	statement_term
	;

protected
access_level
	:
	(
		"public"^    { currentAccessLevel = AccessLevel.Public; }
		|
		"private"^   { currentAccessLevel = AccessLevel.Private; }
		|
		"protected"^ { currentAccessLevel = AccessLevel.Protected; }
		| 
		"internal"^  { currentAccessLevel = AccessLevel.Internal; }
		|
			/* nothing - inherits the access level defined previously */  
	)
	;

statement returns[IStatement stmt]
	{ stmt = null; }
	:
	(
		(declaration_statement) => stmt=declaration_statement
		// |
		// property_def_statement
		// |
		// operator_def_statement
		|
		stmt=type_def_statement
		// |
		// stmt=while_statement
		// |
		// stmt=until_statement
		// |
		// stmt=for_statement
		// |
		// stmt=if_statement
		// |
		// (unless_statement) => stmt=unless_statement
		|
		stmt=expression_statement
		// |
		// stmt=return_statement
		|
		stmt=require_statement
	)
	statement_term
	;

declaration_statement returns [MultipleVariableDeclarationStatement stmt]
	{ 
	  stmt = new MultipleVariableDeclarationStatement(currentAccessLevel); 
	  Identifier ident = null;
	  IExpression initExp = null; 
	}
	:
 	ident=identifier_withtype			{ stmt.AddIdentifier(ident); }
 	(
 		COMMA ident=identifier_withtype	{ stmt.AddIdentifier(ident); }
 	)* 
 	(
 		ASSIGN initExp=test { stmt.AddInitExp(initExp); } 
 		(COMMA initExp=test { stmt.AddInitExp(initExp); } )* 
 	)?
 	;

method_def_statement returns [MethodDefinitionStatement mdstmt]
	{ 
	  mdstmt = new MethodDefinitionStatement( currentAccessLevel ); 
	  String qn = null; TypeReference retType = null; 
	}
	:
	DEF^ modifier[mdstmt] qn=qualified_name 
	{ 
		mdstmt.Name = qn;
		PushScope(mdstmt, ScopeType.Method); 
	}
	(
		LPAREN (methodParams[mdstmt])? RPAREN (retType=type)? 
	)?
	statement_term
	{ mdstmt.ReturnType = retType; }
	suite[mdstmt.Statements]
	END { PopScope(); }
	;

protected
modifier[MethodDefinitionStatement mdstmt]
	:
	"override"			{ mdstmt.IsOverride = true; }
	|
	{ withinOverride }? { mdstmt.IsOverride = true; }
	|
	"new"				{ mdstmt.IsNewSlot  = true; }
	|
	{ withinNew }?		{ mdstmt.IsNewSlot  = true; }
	|
	"abstract"			{ mdstmt.IsAbstract = true; }
	|
	{ withinAbstract }?	{ mdstmt.IsAbstract = true; }
	|
	"virtual"			{ mdstmt.IsVirtual  = true; }
	|
	{ withinVirtual }?	{ mdstmt.IsVirtual  = true; }
	|
	{ withinStatic }?	{ mdstmt.IsStatic   = true; }

	|
	// nothing
	;

require_statement returns[RequireDirectiveStatement rd]
	{ rd = null; String ident = null; } 
	:
	"require" ident=qualified_name { rd = new RequireDirectiveStatement(ident); }
	;

type_def_statement returns [TypeDefinitionStatement tdstmt]
	{ tdstmt = null; currentAccessLevel = AccessLevel.Public; }
	:
	tdstmt=class_def_statement
	;

class_def_statement returns [TypeDefinitionStatement tdstmt]
	{ tdstmt = null; String qn = null; }
						// TODO: Create ClassDefinitionStatement 
						// and support modifiers like visibility and abstract etc
	:
	CLASS t:IDENT 
	{ tdstmt = new TypeDefinitionStatement( currentAccessLevel, t.getText() );
	  PushScope(tdstmt, ScopeType.Type); 
	}
	( (LTHAN|SL) qn=qualified_name { tdstmt.BaseTypes.Add( new TypeReference(qn) ); }
	  (COMMA qn=qualified_name  { tdstmt.BaseTypes.Add( new TypeReference(qn) ); } )* 
	)? 
	statement_term
	type_suite[tdstmt.Statements]
	END { PopScope(); }
	;

methodParams[MethodDefinitionStatement mdstmt]
	{ ParameterVarIdentifier param = null; } 
	:
	param=methodParam 			{ mdstmt.AddFormalParameter( param ); } 
	  (COMMA param=methodParam 	{ mdstmt.AddFormalParameter( param ); } )*
	;

methodParam returns [ParameterVarIdentifier param]
	{ IExpression exp = null; Identifier ident = null; param = null; }
	:
	(
	ident=identifier				{ param = ParameterVarIdentifier.FromIdentifier(ParameterType.Ordinary, ident); } 
	|
	(STAR identifier_withtype) => STAR ident=identifier_withtype	
									{ param = ParameterVarIdentifier.FromIdentifier(ParameterType.Params, ident); } 
	|
	STAR ident=identifier			{ param = ParameterVarIdentifier.FromIdentifier(ParameterType.List, ident); } 
	|
	BAND ident=identifier			{ param = ParameterVarIdentifier.FromIdentifier(ParameterType.Block, ident); } 
	)
	(ASSIGN exp=expression	{ param.InitExpression = exp; } )?
	;

expression_statement returns[IStatement stmt]
	{ stmt = null; PostfixCondition pfc = null; IExpression exp = null; IExpression rhs = null;
	  AugType rel = AugType.Undefined; }
	:
	(
		(compound) => exp=compound
		|
		exp=test
		(	
			rel=augassign rhs=test	{ exp = new AugAssignmentExpression(exp, rhs, rel); }
			|
			(ASSIGN rhs=test		{ exp = new AssignmentExpression(exp, rhs); } )+
		)?
		|
		exp=flow_expressions
	)
	(
	  pfc=postFixCondition 
	  { exp.PostfixCondition = pfc; } 
	)?
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

compound returns[CompoundExpression cexp]
	{ cexp = new CompoundExpression(); }
	:
	(DO^|BEGIN^) { PushScope(cexp, ScopeType.Compound); } 
	statement_term
	suite[cexp.Statements]
	END  { PopScope(); }
	;

flow_expressions returns[IExpression exp]
	{ exp = null; }
	:
	"redo"	{ exp = new RedoExpression(); }
	|
	"break"	{ exp = new BreakExpression(); }
	|
	"next"	{ exp = new NextExpression(); }
	|
	"retry"	{ exp = new RetryExpression(); }
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
	{ PushScope(bexp, ScopeType.Block); }
	(
		
		(DO nothing (blockargs[bexp])? (statement_term)? suite[bexp.Statements] END)
		|
		(LCURLY nothing (blockargs[bexp])? (statement_term)? suite[bexp.Statements] RCURLY)
	)
	{ PopScope(); }
	;

raise returns [RaiseExpression rexp]
	{ rexp = null; IExpression exp; }
	:
	"raise" exp=expression	{ rexp = new RaiseExpression(exp); }
	;

yield returns [YieldExpression rexp]
	{ rexp = new YieldExpression(); }
	:
	"yield" expressionList[rexp.ExpColl]
	;

blockargs[BlockExpression bexp]
	{ ParameterVarIdentifier ident = null; }
	:
	BOR ident=methodParam	
	{ bexp.AddBlockFormalParameter(ident); }
	(options {greedy=true;}:COMMA ident=methodParam { bexp.AddBlockFormalParameter(ident); } )* 
	BOR
	;


test returns [IExpression exp]
	{ exp = null; IExpression rhs = null; }
	: 
	// (block) => exp=block
	// | 
	exp=and_test ("or" rhs=and_test { exp = new BinaryExpression(exp, rhs, BinaryOp.Or); })*
	| 
	exp=lambda
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
	|
	"is"		{ op = BinaryOp.IsA; }
	
//	|ALT_NOTEQUAL
// 	|"in"
//	|"not" "in"
//	|"is" "not"
	;

expressionList[IList expColl]
	{ IExpression exp = null; }
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
	exp=and_expr (options {greedy=true;}:BOR rhs=and_expr { exp = new BinaryExpression(exp, rhs, BinaryOp.Or2); })*
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
	| exp=varref
	| exp=constantref
	| "self" { exp = SelfReferenceExpression.Instance; }
	| "base" { exp = BaseReferenceExpression.Instance; }
	;

varref returns [VariableReferenceExpression vre]
	{ Identifier ident = null; vre = null; }
	:
	ident=name
	{ vre = new VariableReferenceExpression(ident); }
	;

constantref returns [ConstExpression lre]
	{ lre = null; }
	:
	  t1:NUM_INT
	  { lre = new ConstExpression(t1.getText(), ConstExpressionType.IntLiteral); }
	| t2:NUM_LONG
	  { lre = new ConstExpression(t2.getText(), ConstExpressionType.LongLiteral); }
	| t3:NUM_FLOAT
	  { lre = new ConstExpression(t3.getText(), ConstExpressionType.FloatLiteral); }
	| t4:SYMBOL
	  { lre = new ConstExpression(t4.getText(), ConstExpressionType.SymbolLiteral); }
	| t5:STRING_LITERAL
	  { lre = new ConstExpression(t5.getText(), ConstExpressionType.StringLiteral); }
	| t6:CHAR_LITERAL
	  { lre = new ConstExpression(t6.getText(), ConstExpressionType.CharLiteral); }
	;

trailer[IExpression inner] returns [IExpression exp]
	{ exp = null; BlockExpression temp = null; String qp = String.Empty; }
	: 
	LPAREN 
	{ exp = new MethodInvocationExpression(inner); } 
	  (arglist[(exp as MethodInvocationExpression).Arguments])? 
	RPAREN 
	|
	temp=block
	{
	  exp = inner;
	  exp.Block = temp;
	}
	| 
//	LBRACK subscriptlist RBRACK // TODO: Array/indexer access
//	| 
//	(DOT|COLONCOLON) => qp=qualified_postfix	{ exp = new MemberAccessExpression(inner, qp); }
	COLONCOLON t1:IDENT	{ exp = new MemberAccessExpression(inner, t1.getText()); }
	|
	DOT "nil?" { exp = new NullCheckExpression(inner); }
	|
	DOT t:IDENT { exp = new MemberAccessExpression(inner, t.getText()); }
	;

range returns[IExpression rex]
	{ rex = null; IExpression lhs = null; IExpression rhs = null; }
	:
	LPAREN lhs=expression (t:DOTDOT|DOTDOTDOT) rhs=expression RPAREN
	{ rex = new RangeExpression(lhs, rhs, t != null); }
	;

// subscriptlist
//     :   
//     subscript (options {greedy=true;}:COMMA subscript)*
// 	;

// subscript
// 	: 
//     expression
//     ;

arglist[IList expcoll]
	{ IExpression exp; }
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
      { exp.Add(key, value); }
    (options {greedy=true;}:COMMA key=expression MAPASSIGN value=test 
      { exp.Add(key, value); } )* 
    ;




class RookLexer extends Lexer;

options 
{
	exportVocab=Rook;      
	testLiterals=false;    // don't automatically test for literals
	k=4;                   // four characters of lookahead
	charVocabulary='\u0003'..'\uFFFF';
	// without inlining some bitset tests, couldn't do unicode;
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
COLON			:	':'		;
COLONCOLON		:	"::"	;

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

