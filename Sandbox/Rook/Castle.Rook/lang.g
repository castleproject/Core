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
	:
	type_fields[classNode]
	
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
type_fields![TypeNode type]
	:
	(access_level static_fields[type] | instance_fields[type])*
	;

protected
static_fields![TypeNode type]
	:
	id:STATIC_IDENTIFIER
	{
		type.StaticFields.Add( new StaticFieldIdentifier(id.getText()) );
	}
	;

protected
instance_fields![TypeNode type]
	:
	id:INSTANCE_IDENTIFIER
	{
		type.InstanceFields.Add( new InstanceFieldIdentifier(id.getText()) );
	}
	;
	
protected
access_level!
	:
	"public" { currentAccessLevel = AccessLevel.Public; }
	|
	"private" { currentAccessLevel = AccessLevel.Private; }
	|
	"protected" { currentAccessLevel = AccessLevel.Protected; }
	| 
	"internal" { currentAccessLevel = AccessLevel.Internal; }
	|
		/* nothing */  { currentAccessLevel = AccessLevel.Public; }
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

/*
literal
	:	
	INTEGER_LITERAL
	;
	
identifier!
	:	
	id:IDENTIFIER 
	;
	
// ***** A.2.1 Basic concepts *****
type_name
	:	
	t:namespace_or_type_name
	;
	
namespace_or_type_name
	:	
	simple_name (DOT! simple_name)*
	;

simple_name
	:	identifier
//		{#simple_name = #([QualIdent], id);}
	;


statement_list 
	:	
	(statement)+
	;

statement 
	:	
	(
		declaration_statement 
		|
		expression_statement 
	)
	(SEMI!)?
	;

declaration_statement 
	:	
	local_variable_declaration
	;
	
local_variable_declaration 
	:	
	(multiple_local_variable_declarators)=>multiple_local_variable_declarators
	{ 
	}	
	|
	local_variable_declarator
	;
	
multiple_local_variable_declarators
	:	
	type:type_name identifier (COMMA! (type2:type_name)? identifier)*
	(
		ASSIGN!
		expression_list
	)?
	;

local_variable_declarator!
	:	
	type:type_name id:identifier (ASSIGN! expression)?
	;

literal_expression
	:
	literal
	;

assign_expression
	:
	id:identifier ASSIGN expression
	;

multiple_assign_expression
	:
	id:identifier (COMMA! identifier)+ ASSIGN expression_list
	;

invocation_expression
	:
	(type_name DOT!)? id:identifier LPAREN! (expression_list)? RPAREN!
	;

expression
	:	
	literal_expression
	|
	(multiple_assign_expression) => multiple_assign_expression
	|
	assign_expression
	|
	invocation_expression
	;
	
expression_list
	:	
	expression (COMMA! expression)*
	;

expression_statement
	:
	(multiple_assign_expression) => multiple_assign_expression
	|
	assign_expression
	|
	invocation_expression
	;
*/

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
	}
	:	
	IDENTIFIER_START_CHARACTER (IDENTIFIER_PART_CHARACTER)*
	;
	
STATIC_IDENTIFIER
options 
	{
	 testLiterals=false; 
	}
	:	
	"@@" IDENTIFIER_START_CHARACTER (IDENTIFIER_PART_CHARACTER)*
	;

INSTANCE_IDENTIFIER
options 
	{
	 testLiterals=false; 
	}
	:	
	"@" IDENTIFIER_START_CHARACTER (IDENTIFIER_PART_CHARACTER)*
	;
	
protected
IDENTIFIER_START_CHARACTER
	:	('a'..'z'|'A'..'Z'|'_'|'$') 
	;
	
protected
IDENTIFIER_PART_CHARACTER
	:	('a'..'z'|'A'..'Z'|'_'|'0'..'9') 
	;

/*
NUMERIC_LITERAL
	:
	// integer
	(DECIMAL_DIGIT)+ 
	{
		$setType(INTEGER_LITERAL);
	}
	;
*/

// nums
protected
DECIMAL_DIGIT
	: 	
	('0'..'9')
	;
