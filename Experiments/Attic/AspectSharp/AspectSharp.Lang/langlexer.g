options
{
	language = "CSharp";
}
class AspectLanguageLexer extends Lexer;
options 
{
    importVocab=aspectLanguage;
    testLiterals=false;
	k=2;
	filter=true;
}

// Keywords
ALL        : '*'  ;
COLON      : ':'  ;
SEMI       : ';'  ;
COMMA      : ','  ;
OR         : '|'  ;
INHERITS   : '<'  ;

LBRACK     : '['  ;
RBRACK     : ']'  ;
LCURLY     : '('  ;
RCURLY     : ')'  ;

DOT        : '.'  ;

COMMENT    : "//" (~('\n'|'\r'))* { $setType(Token.SKIP);};   
  
// Literals
protected 
DIGIT : 
    '0'..'9'
    ;

INTLIT : 
    (DIGIT)+
    ;

CHARLIT : 
    '\''! . '\''!  ;

STRING_LITERAL : 
    '"'!
    ( '"' '"'!
    | ~('"'|'\n'|'\r')
    )*
    ( '"'!
    | // nothing -- write error message
    )  ;   
  
WS : 
    ( ' ' | '\t' | '\f'
    | (   "\r\n"  // Evil DOS
    | '\r'    // Macintosh
    | '\n'    // Unix (the right way)
    )
    { newline(); }
    )
    { $setType(Token.SKIP); }
    ;

ID
    options {testLiterals=true;}
    : ('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'0'..'9'|'_')*
    ;

