header
{
    using antlr;
    using System.Text;
    using Castle.Model.Configuration;
}
options 
{
	language = "CSharp";
	namespace = "Castle.Windsor.Configuration.Interpreters.CastleLanguage.Internal";
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
	i:IMPORT ns=name
	{
		import = new ImportDirective( ToLexicalInfo(i), ns );
		conf.Imports.Add(import);
	}
	(
	    IN assemblyName=name
	    {
	        import.AssemblyReference = new AssemblyReference( ToLexicalInfo(i), assemblyName);
	    }
	)?
	(
		NEWLINE
	)
	;
	    
nodes[MutableConfiguration conf]
	{
		String i = null;
		MutableConfiguration newNode = null;
	}:
	i=name COLON NEWLINE
	{
		newNode = new MutableConfiguration(i);
		conf.Children.Add(newNode);
	}	
	INDENT 
	
	 ( (name COLON value)=> attribute[newNode] | nodes[newNode])*

	DEDENT
	;
	
protected
attribute [MutableConfiguration conf] 
	{
		String i = null;
		String v = null;
	}:
	i=name COLON v=value NEWLINE
	{
		conf.Attributes[i] = v;
	}
	;

protected
name returns [String value]
	{
		value = null; sbuilder.Length = 0;
	}:			
	id:ID			
	{					
		sbuilder.Append(id.getText());
		value = sbuilder.ToString();
	}
	( 
	    options 
	    { greedy = true; }:
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

protected
value returns [String value]
	{
		value = null; sbuilder.Length = 0;
	}:	
	(		
		valueToken:STRING_LITERAL
		{
			String val = valueToken.getText();
			sbuilder.Append(val);
		} |
		id:ID			
		{					
			sbuilder.Append(id.getText());
			value = sbuilder.ToString();
		}
		( 
			options 
			{ greedy = true; }:
			(
				DOT id2:ID 
				{
					sbuilder.Append('.');
					sbuilder.Append(id2.getText());
				}
			)
			|
			(
				IN id3:ID
				{
					sbuilder.Append(" in ");
					sbuilder.Append(id3.getText());
				}
			)
		)*
	)
	{
	    value = sbuilder.ToString();
	}
	;
	
