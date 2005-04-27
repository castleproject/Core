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
	AS        = "as";
	INC       = "++";
	DEC       = "--";
	SELF      = "self";
	BASE      = "base";
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
		QualifiedIdentifier qi = null;
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
		ClassNode classNode = null;
		Identifier id = null;
	}
	:
	/* TODO:visibility public/private/etc */ 
	CLASS_DEF! id=identifier 
	{
		classNode = new ClassNode(id.Name);
		types.Add(classNode);
	}
	(baseTypes[classNode])?
	class_body[classNode]
	;

mixin_declaration![IList mixins]
	{
		MixinNode mixinNode = null;
		Identifier id = null;
	}
	:
	MIXIN_DEF! id=identifier 
	{
		mixinNode = new MixinNode(id.Name);
		mixins.Add(mixinNode);
	}
	mixin_body[mixinNode]
	;

mixin_body[MixinNode mixinNode]
	:
	END!
	;

class_body[ClassNode classNode]
	{
		// Default access level for the method body
		currentAccessLevel = AccessLevel.Public;
	}
	:
	(access_level class_level_supported_statements)*
	
	END!
	;

protected
baseTypes![TypeNode type]
	{
		QualifiedIdentifier qi = null;
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
qualified_identifier returns [QualifiedIdentifier ident]
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
	(DOT! id=identifier
	{
		sb.Append('.');
		sb.Append(id.Name);
	}
	)*
	{
		ident = new QualifiedIdentifier( sb.ToString() );
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
type_name returns [QualifiedIdentifier qi]
	{
		qi = null;
		Identifier id = null;
	}
	:	
	(AS! id=identifier
	{
		qi = new QualifiedIdentifier( id.Name );
	}
	)?
	;

///
/// Statements
///

method_def_stmt
	{
		String[] nameParts;
		QualifiedIdentifier retType = null;
	}
	:
	DEF! nameParts=method_name 
	{
		MethodNodeBuilder.Build(nameParts);
	}
	formal_param_list 
	retType=type_name
	method_body
	(SEMI)?
	;
	
protected
formal_param_list
	:
	LPAREN! ( method_param (COMMA! method_param)* )? RPAREN!
	;
	
protected
method_param
	{
		Identifier id = null;
		QualifiedIdentifier qi = null;
	}
	:
	(REF|OUT)? id=identifier qi=type_name
	;

protected
method_body
	:
	(statement_list)*
	END!
	;

protected 
statement
	:
	(assign_stmt)=> assign_stmt
	| 
	method_def_stmt
	|
	expression_statement
	;

protected
statement_list
	:
	statement
	;
	
protected
class_level_supported_statements
	:
	assign_stmt
	| 
	method_def_stmt
	;

protected
var_reference
	{
		QualifiedIdentifier qi = null;
	}
	:
	id:STATIC_IDENTIFIER|id2:INSTANCE_IDENTIFIER|qi=qualified_identifier
	;	
	
protected 
assign_stmt
	:
	unary_exp ASSIGN expression
	{
	}
	(SEMI)?
	;

protected
expression_statement
	:
	unary_exp
	;
	
///	
/// Expressions
/// 

protected
unary_exp
	:
	primary_exp
	;

protected
expression
	:
	primary_exp
	;

literal_exp
	:	
	INTEGER_LITERAL
	;
	
protected 
method_invoke_exp
	:
	LPAREN ( expression (COMMA! expression)* )? RPAREN
	( options {greedy=true;} :  SEMI)?
	;
	
protected
primary_start
	:	
	literal_exp
	|	
	identifier	
	|	
	SELF
	|	
	BASE
	;

protected
primary_exp
	:
	primary_start
	(	options {greedy=true;}:	
		postfix_exp
		|
		method_invoke_exp
		|	
		member_access
	)*
	;	

protected
member_access
	:	
	DOT id:identifier 
	;


protected
postfix_exp
	:
	INC
	|
	DEC
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
