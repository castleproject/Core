options
{
	language = "CSharp";
	namespace = "Castle.Windsor.Configuration.CastleLanguage";
}
class WindsorConfLanguageLexer extends Lexer;
options 
{
    importVocab=windsorLanguage;
    testLiterals=false;
    k=2;
    filter=true;
}

// Keywords
COLON      : ':'  ;
COMMA      : ','  ;
INHERITS   : '<'  ;
DOT        : '.'  ;

SL_COMMENT : 
	"//" 
	(~'\n')* '\n'
	{ $setType(Token.SKIP); newline(); }
	;
  
/** 
 *  Grab everything before a real symbol.  Then if newline, kill it
 *  as this is a blank line.  If whitespace followed by comment, kill it
 *  as it's a comment on a line by itself.
 *
 *  Ignore leading whitespace when nested in [..], (..), {..}.
 */
LEADING_WS
{
    int spaces = 0;
}
    :   {getColumn()==1}?
        // match spaces or tabs, tracking indentation count
        ( 	' '  { spaces++; }
        |	'\t' { spaces += 8; spaces -= (spaces % 8); }
        )+
        {
                // make a string of n spaces where n is column number - 1
                char[] indentation = new char[spaces];
                for (int i=0; i<spaces; i++) {
                    indentation[i] = ' ';
                }
                String s = new String(indentation);
                $setText(s);
        }

        // kill trailing newline or comment
        (   ('\r')? '\n' {newline();}
            {$setType(Token.SKIP);}

        |   // if comment, then only thing on a line; kill so we
            // ignore totally also wack any following newlines as
            // they cannot be terminating a statement
            "//" (~'\n')* ('\n' {newline();})+ 
            {$setType(Token.SKIP);}
        )?
    ;

ID
    options {testLiterals=true;}
    : ('a'..'z'|'A'..'Z'|'0'..'9'|'_')+
    ;

NEWLINE
{
    int startCol = getColumn();
}
    :   (options{greedy=true;}:('\r')? '\n' {newline();})+
        {if ( startCol==1  )
            $setType(Token.SKIP);
        }
    ;

WS  :   (' '|'\t')+ {$setType(Token.SKIP);}
    ;

