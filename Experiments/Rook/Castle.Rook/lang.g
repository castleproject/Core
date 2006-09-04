header 
{
	// using CommonAST					= antlr.CommonAST; 
	using System.Text;
	using System.Collections;
	using Castle.Rook.AST;
}
options 
{	
	language = "CSharp";
	namespace = "Castle.Rook.Parse";
}

class RookLangParser extends Parser;

options 
{
	k = 2;                           	// two token lookahead
	defaultErrorHandler = true;     	// Don't generate parser error handlers
	buildAST = false;   
 	exportVocab=Rook;
}
tokens
{
	CLASS_DEF = "class";
	MIXIN_DEF = "mixin";
	NAMESPACE = "namespace";
	INTERFACE = "interface";
	INIT	  = "initialize";
	INIT2	  = "init";
	END       = "end";
	DEF       = "def";
	ATTR      = "attr";
	GET       = "get";
	SET       = "set";
	INC       = "++";
	DEC       = "--";
	SELF      = "self";
	BASE      = "base";
	COLON     = ":";
	DO        = "do";
}

{
	AccessLevel currentAccessLevel = AccessLevel.Public;
	
	public override void reportError(RecognitionException ex)
	{
		throw ex;
	}
}

compilation_unit returns[CompilationUnitNode unit]
	{
		unit = new CompilationUnitNode();
	}
	:	
	(options { greedy=true;}: EOS!)*
	(declaration[unit])*
	EOF!
	;
	
declaration[AbstractDeclarationContainer container]
	:
	mixin_declaration[container.MixinTypes] 
	| 
	class_declaration[container.ClassesTypes] 
	| 
	namespace_member_declaration[container.Namespaces]
	;

namespace_member_declaration[IList namespaces]
	:	
	namespace_declaration[namespaces]
	;

namespace_declaration![IList namespaces]
	{
		NamespaceNode ns = null;
		Identifier qi = null;
	}
	:	
	NAMESPACE qi=qualified_identifier 
	{ 
		ns = new NamespaceNode( qi );
	}
	namespace_body[ns]
	{
		namespaces.Add(ns);
	}
	;
	
namespace_body[NamespaceNode ns]
	:	
	(declaration[ns])* END!
	;

class_declaration![IList types]
	{
		ClassDefinitionStatement classNode = null;
		Identifier id = null;
	}
	:
	/* TODO:visibility public/private/etc */ 
	CLASS_DEF! id=identifier 
	{
		classNode = new ClassDefinitionStatement(id.Name);
		types.Add(classNode);
	}
	(baseTypes[classNode])?
	class_body[classNode]
	;

mixin_declaration![IList mixins]
	{
		MixinDefinitionStatement mixinNode = null;
		Identifier id = null;
	}
	:
	MIXIN_DEF! id=identifier 
	{
		mixinNode = new MixinDefinitionStatement(id.Name);
		mixins.Add(mixinNode);
	}
	mixin_body[mixinNode]
	;

mixin_body[MixinDefinitionStatement mixinNode]
	:
	END!
	;

class_body[ClassDefinitionStatement classNode]
	{
		// Default access level for the method body
		currentAccessLevel = AccessLevel.Public;
	}
	:
	(access_level class_level_supported_statements[classNode.Statements])* END!
	;

protected
baseTypes![TypeDefinitionStatement type]
	{
		Identifier qi = null;
	}
	:
	LESSTHAN! qi=qualified_identifier 
	{
		type.BaseTypes.Add( qi );
	}
	(COMMA! qi=qualified_identifier
	{
		type.BaseTypes.Add( qi );
	})*
	;

protected
access_level!
	:
	"public"    { currentAccessLevel = AccessLevel.Public; }
	|
	"private"   { currentAccessLevel = AccessLevel.Private; }
	|
	"protected" { currentAccessLevel = AccessLevel.Protected; }
	| 
	"internal"  { currentAccessLevel = AccessLevel.Internal; }
	|
		/* nothing - inherits the access level defined previously */  
	;

protected
identifier! returns [Identifier ident]
	{
		ident = null;
	}
	:	
	id:IDENTIFIER 
	{ ident = new Identifier(id.getText()); }
	;

protected
qualified_identifier returns [Identifier ident]
	{
		ident = null;
		StringBuilder sb = new StringBuilder();
		Identifier id = null;
	}
	:	
	id=identifier
	{
		sb.Append(id.Name);
	}
	( options { greedy=true; } : DOT! id=identifier
	{
		sb.Append('.');
		sb.Append(id.Name);
	}
	)*
	{
		ident = new Identifier( sb.ToString() );
	}
	;

protected
method_name returns [String[] parts]
	{
		parts = new String[2];
		Identifier id = null;
	}
	:	
	(
		id=identifier
		{
			parts[0] = id.Name;
		}
		|
		SELF
		{
			parts[0] = "self";
		}
	)
	(DOT! id=identifier
	{
		parts[1] = id.Name;
	}
	)?
	;

protected
type_name returns [Identifier qi]
	{
		qi = null;
	}
	:	
	(COLON qi=qualified_identifier)?
	;

///
/// Statements
///

block_statement returns [BlockStatement block]
	{
		block = new BlockStatement();
	}
	:
	DO
		statement_list[block.Statements]
	END	(SEMI)?
	;

method_def_stmt returns [MethodDefinitionStatement method]
	{
		String[] nameParts;
		Identifier retType = null;
		method = null;
	}
	:
	DEF! nameParts=method_name 
	{
		method = new MethodDefinitionStatement(currentAccessLevel, nameParts);
	}
	formal_param_list[method]
	retType=type_name
	{
		method.ReturnType = retType;
	}
	method_body[method]
	(SEMI)?
	;
	
protected
formal_param_list[MethodDefinitionStatement method]
	:
	LPAREN! ( method_param[method] (COMMA! method_param[method])* )? RPAREN!
	;
	
protected
method_param[MethodDefinitionStatement method]
	{
		Identifier id = null;
		Identifier qi = null;
	}
	:
	(REF|OUT)? id=identifier qi=type_name
	{
		method.Parameters.Add( new MethodParameterNode(id.Name, qi) );
	}
	;

protected
method_body[MethodDefinitionStatement method]
	:
	statement_list[method.Statements]
	END!
	;

protected 
statement returns [Statement stmt]
	{
		stmt = null;
	}
	:
	stmt=method_def_stmt
	|
	stmt=expression_statement
	|
	stmt=block_statement
	|
	stmt=simple_field_decl_stmt
	;

protected
statement_list[IList statements]
	{
		Statement stmt = null;
	}
	:
	(stmt=statement { statements.Add( stmt ); } )*
	;
	
protected
class_level_supported_statements[IList statements]
	{
		Statement stmt = null;
	}
	:
	(
		stmt=field_decl_stmt
		|
		stmt=method_def_stmt
	)
	{
		statements.Add( stmt );
	}
	;

protected
var_reference returns [IdentifierReferenceExpression exp]
	{
		Identifier qi = null; exp = null;
	}
	:
	id:STATIC_IDENTIFIER { exp = new StaticFieldReferenceExpression(id.getText()); }
	|
	id2:INSTANCE_IDENTIFIER { exp = new InstanceFieldReferenceExpression(id2.getText()); }
	|
	qi=qualified_identifier { exp = new IdentifierReferenceExpression(qi); }
	;	

protected
simple_field_decl_stmt returns [FieldDeclarationStatement stmt]
	{
		stmt = null; IdentifierReferenceExpression var; 
		Expression initializer = null; Identifier tn = null;
	} 
	:
	var=var_reference tn=type_name (ASSIGN initializer=expression)?
	{
		stmt = new FieldDeclarationStatement(currentAccessLevel, var, tn, initializer);
	}
	;

protected
field_decl_stmt returns [FieldDeclarationStatement stmt]
	{
		stmt = null; IdentifierReferenceExpression var; 
		Expression initializer = null; Identifier tn = null;
	} 
	:
	var=var_reference tn=type_name (ASSIGN initializer=expression)?
	{
		stmt = new FieldDeclarationStatement(currentAccessLevel, var, tn, initializer);
	}
	;

/*
protected
assign_stmt returns [AssignmentStatement stmt]
	{
		stmt = null; IdentifierReferenceExpression target; Expression value;
	} 
	:
	target=var_reference ASSIGN value=expression
	{
		stmt = new AssignmentStatement(currentAccessLevel, target, value);
	}
	;
*/

protected
expression_statement returns [ExpressionStatement stmt]
	{
		stmt = null;
		Expression exp = null;
	}
	:
	(
		(assign_exp) => exp=assign_exp
		|
		exp=unary_exp
	)
	{
		stmt = new ExpressionStatement(exp);
	}
	;
	
///	
/// Expressions
/// 

protected 
assign_exp returns [AssignmentExpression ue]
	{
		Expression target, value = null; ue = null;
	}
	:
	target=unary_exp ASSIGN value=expression
	{
		ue = new AssignmentExpression(target, value);
	}
	;

protected
unary_exp returns [Expression exp]
	{ exp = null; }
	:
	exp=primary_exp
	;

protected
expression returns [Expression exp]
	{ exp = null; }
	:
	exp=primary_exp
	;

literal_exp returns [LiteralExpression le]
	{ le = null; }
	:	
	t:INTEGER_LITERAL
	{
		le = new LiteralIntegerExpression( t.getText() );
	}
	;

/*
symbol_exp returns [SymbolExpression se]
	{ se = null; }
	:	
	t:SYMBOLID
	{
		se = new SymbolExpression( t.getText() );
	}
	;
*/
	
protected 
method_invoke_exp[Expression target] returns [MethodInvokeExpression mie]
	{
		mie = new MethodInvokeExpression(target);
		Expression exp;
	}
	:
	LPAREN ( exp=expression { mie.Arguments.Add( exp ); }
	(COMMA! exp=expression { mie.Arguments.Add( exp ); } )* )? RPAREN
	( options {greedy=true;} :  SEMI)?
	;
	
protected
primary_start returns [Expression exp]
	{ exp = null; }
	:
	/* 
	exp=symbol_exp
	|
	*/
	exp=literal_exp
	|
	exp=var_reference
	|
	SELF { exp = SelfReferenceExpression.Instance; }
	|
	BASE { exp = BaseReferenceExpression.Instance; }
	;

protected
primary_exp returns [Expression exp]
	{
		Expression ps = null; exp = null;
	}
	:
	(
		ps=primary_start
		(	options {greedy=true;}:	
			exp=postfix_exp[ps]
			|
			exp=method_invoke_exp[ps]
			|	
			exp=member_access[ps]
		)*
	)
	{
		if (exp == null) exp = ps;
	}
	;	

protected
member_access[Expression target] returns [MemberAccessExpression mae]
	{
		mae = null;
		Identifier id;
	}
	:	
	DOT id=identifier 
	{
		mae = new MemberAccessExpression(target, id);
	}
	;


protected
postfix_exp[Expression target] returns [PostFixExpression pfe]
	{
		pfe = null;
	}
	:
	INC	{ pfe = new PostFixExpression( target, 1 ); }
	| 
	DEC	{ pfe = new PostFixExpression( target, 2 ); }
	;	

/// 
/// Lexer
///	
		
class RookLexer extends Lexer;

options 
{
	k=4;                       // four characters of lookahead
	charVocabulary='\u0003'..'\u7FFF'; 	// to avoid hanging eof on comments (eof = -1)
    importVocab=Rook;
	testLiterals=false;
	filter=true;
}

ASSIGN			:   '='		;
SEMI  			:   ';'		;
COMMA 			:   ','		;
LPAREN			:	'('		;
RPAREN			:	')'		;
LBRACK			:	'['		;
RBRACK			:	']'		;
LCURLY			:	'{'		;
RCURLY			:	'}'		;
COLON			:	':'		;
DOT             :   '.'     ;
LESSTHAN        :   '<'     ;

	
// ***** A.1.1 LINE TERMINATORS *****
protected
NEW_LINE
	:	(	// carriage return character followed by possible line feed character	
			{ LA(2)=='\u000A' }? '\u000D' '\u000A'			
		|	'\u000D'			// line feed character							
		|	'\u000A'			// line feed character							
		|	'\u2028'			// line separator character
		|	'\u2029'			// paragraph separator character
		)
		{newline();}
	;
	
protected
NEW_LINE_CHARACTER
	:	('\u000D' | '\u000A' | '\u2028' | '\u2029')
	;
	
protected
NOT_NEW_LINE
	:	~( '\u000D' | '\u000A' | '\u2028' | '\u2029')
	;
	
// ***** A.1.2 WHITESPACE *****
WHITESPACE
	:	(	' '
		|	'\u0009' // horizontal tab character
		|	'\u000B' // vertical tab character
		|	'\u000C' // form feed character 
		|	NEW_LINE 
		)+
		{ _ttype = Token.SKIP; }
	;	
	
	
// ***** A.1.3 COMMENTS *****
SINGLE_LINE_COMMENT
	:	"#" 
		(NOT_NEW_LINE)* 
		(NEW_LINE)? // may be eof
		{_ttype = Token.SKIP;}
	;

		
// ***** A.1.6 IDENTIFIERS *****

IDENTIFIER
options 
	{
	 testLiterals=true; 
 	 paraphrase = "an identifier";
	}
	:	
	IDENTIFIER_START_CHARACTER (IDENTIFIER_PART_CHARACTER)*
	;
	
	/*
SYMBOLID
options 
	{
	 testLiterals=false; 
 	 paraphrase = "a symbol";
	}
	:	
	COLON IDENTIFIER
	;
	*/
	
protected
IDENTIFIER_START_CHARACTER
	:	('a'..'z'|'A'..'Z'|'_'|'$') 
	;
	
protected
IDENTIFIER_PART_CHARACTER
	:	('a'..'z'|'A'..'Z'|'_'|'0'..'9') 
	;
	
STATIC_IDENTIFIER
options 
	{
	 testLiterals=false; 
	 paraphrase = "an static variable name";
	}
	:	
	"@@" IDENTIFIER_START_CHARACTER (IDENTIFIER_PART_CHARACTER)*
	;

INSTANCE_IDENTIFIER
options 
	{
	 testLiterals=false; 
	 paraphrase = "an instance variable name";
	}
	:	
	"@" IDENTIFIER_START_CHARACTER (IDENTIFIER_PART_CHARACTER)*
	;

NUMERIC_LITERAL
	:
	// integer
	(DECIMAL_DIGIT)+ 
	{
		$setType(INTEGER_LITERAL);
	}
	;

// nums
protected
DECIMAL_DIGIT
	: 	
	('0'..'9')
	;
