header
{
    using antlr;
    using System.Text;
    using Castle.Model.Configuration;
}
options 
{
	language = "CSharp";
	namespace = "Castle.Windsor.Configuration.CastleLanguage";
}
class WindsorLanguageParser extends Parser;
options 
{
    buildAST = false;
    exportVocab=windsorLanguage;
    defaultErrorHandler = true;
}
tokens 
{
	IN="in";
	IMPORT="import"; 
}
{
    protected StringBuilder sbuilder = new StringBuilder();

	protected LexicalInfo ToLexicalInfo(IToken token)
	{
		int line = token.getLine();
		int startColumn = token.getColumn();
		int endColumn = token.getColumn() + token.getText().Length;
		String filename = token.getFilename();
		return new LexicalInfo(filename, line, startColumn, endColumn);
	}
}

start returns [ConfigurationDefinition conf]
	{
		conf = new ConfigurationDefinition();
	}:
	(options { greedy=true;}: EOS!)*			 
	(import_directive[conf])*
	(nodes[conf.Root])*
	EOF!
	;

protected
import_directive[ConfigurationDefinition conf]
	{
		String ns;
		String assemblyName;
		ImportDirective import = null;
	}: 
	i:IMPORT! ns=identifier!
	{
		import = new ImportDirective( ToLexicalInfo(i), ns );
		conf.Imports.Add(import);
	}
	(
	    IN! assemblyName=identifier!
	    {
	        import.AssemblyReference = new AssemblyReference( ToLexicalInfo(i), assemblyName);
	    }
	)?
	;
	    
nodes[MutableConfiguration conf]
	{
		String i = null;
		MutableConfiguration newNode = null;
	}:
	i=identifier COLON NEWLINE
	{
		newNode = new MutableConfiguration(i);
		conf.Children.Add(newNode);
	}	
	INDENT 
	
	 ( (identifier COLON identifier)=> attribute[newNode] | nodes[newNode])*

	DEDENT
	;
	
protected
attribute [MutableConfiguration conf] 
	{
		String i = null;
		String v = null;
	}:
	i=identifier COLON v=identifier NEWLINE
	{
		conf.Attributes[i] = v;
	}
	;

protected
identifier returns [String value]
	{
		value = null; sbuilder.Length = 0;
	}:			
	id:ID			
	{					
		sbuilder.Append(id.getText());
		value = sbuilder.ToString();
	}
	( options { greedy = true; }:
	    DOT id2:ID
	    {
	        sbuilder.Append('.');
	        sbuilder.Append(id2.getText());
	    }
	)*
	{
	    value = sbuilder.ToString();
	}
	;

