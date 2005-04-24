header 
{
	using CommonAST					= antlr.CommonAST; 
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
	CLASS = "class";
}

compilation_unit returns [CompilationUnitNode unit] 
	{
		unit = new CompilationUnitNode();
	}
	:	
	(options { greedy=true;}: EOS!)*			 
	(statement_list)*  
	EOF!
	;

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
	namespace_or_type_name
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
	declaration_statement (SEMI!)?
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

expression
	:	
	literal_expression
	|
	assign_expression
	;
	
expression_list
	:	
	expression (COMMA! expression)*
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

ASSIGN : '=';
SEMI   : ';';
COMMA  : ',';


	
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
	
protected
IDENTIFIER_START_CHARACTER
	:	('@'|'a'..'z'|'A'..'Z'|'_'|'$') 
	;
	
protected
IDENTIFIER_PART_CHARACTER
	:	('a'..'z'|'A'..'Z'|'_'|'0'..'9') 
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
