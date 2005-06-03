header
{
    using antlr;
    using System.Text;
    using AspectSharp.Lang.AST;
}
options 
{
	language = "CSharp";
}
class AspectLanguageParser extends Parser;
options 
{
    buildAST = false;
    exportVocab=aspectLanguage;
    defaultErrorHandler = true;
}
tokens 
{
	ASPECT="aspect"; 
	FOR="for";
	IN="in";
	END="end";
	IMPORT="import"; 
	MIXINS="mixins"; 
	INCLUDE="include"; 
	INTERCEPTORS="interceptors"; 
	ADVICEINTERCEPTOR="advice";
	POINTCUT="pointcut";
	METHOD="method";
	PROPERTY="property";
	PROPERTY_READ="propertyread";
	PROPERTY_WRITE="propertywrite";
	ASSIGNFROM="assignableFrom";
	CUSTOMMATCHER="customMatcher";
	EXCLUDES="excludes";
	INCLUDES="includes";
}
{
    protected StringBuilder sbuilder = new StringBuilder();

	protected LexicalInfo ToLexicalInfo(antlr.Token token)
	{
		int line = token.getLine();
		int startColumn = token.getColumn();
		int endColumn = token.getColumn() + token.getText().Length;
		String filename = token.getFilename();
		return new LexicalInfo(filename, line, startColumn, endColumn);
	}

    protected String methodAll(String s)
   {
        if (s == "*")
            return ".*";
        return s;   
    }
}


start returns [EngineConfiguration conf]
	{
		conf = new EngineConfiguration();		
	}:
	(options { greedy=true;}: EOS!)*			 
	(import_directive[conf])*
	(interceptors_global[conf])*
	(mixins_global[conf])*
	(aspects[conf])*
	EOF!
	;

protected
import_directive[EngineConfiguration conf]
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
	
protected
interceptors_global[EngineConfiguration conf] :
    INTERCEPTORS! keytypepair[conf.Interceptors]
    {
    }
    ;

protected
mixins_global[EngineConfiguration conf] :
    MIXINS! keytypepair[conf.Mixins]
    {
    }
    ;
    
protected
keytypepair [IDeclarationCollection collection] :
    LBRACK!
    (
        (pairvalue[collection] (SEMI pairvalue[collection])*)
    )
    RBRACK!
    ;

protected 
pairvalue [IDeclarationCollection collection] 
    {
        DefinitionBase definition = null;
    } :
    keyToken:STRING_LITERAL! COLON 
    {
        String key = keyToken.getText();
        definition = collection.Add( key, ToLexicalInfo(keyToken) );
    }
    type_name[definition]
    ;

protected 
type_name_or_ref returns [TypeReference type] 
    {
        type = null;
    } :
    refTypeToken:STRING_LITERAL!
    {
        type = new TypeReference();
        type.LinkRef = refTypeToken.getText();
    }
    |
    type=type_name_def!
    ;

protected
type_name[DefinitionBase definition]
    {
        TypeReference tr = null;
    } :
    tr=type_name_def!
    {
        definition.TypeReference = tr;
    }
    ;

protected
type_name_def returns [TypeReference type] 
    {
        type = new TypeReference();
        String typeToken = null;
        String assemblyToken = null;
    } :
    typeToken=identifier!
    {
        type.TypeName = typeToken;
    }
    (
        i:IN! assemblyToken=identifier!
        {
            type.AssemblyReference = new AssemblyReference( ToLexicalInfo(i), assemblyToken );
        }
    )?
    ;
    
protected
aspects [EngineConfiguration conf]
    {
        AspectDefinition aspect = null;
        TargetTypeDefinition target = null;
        TypeReference tr = null;
    } :
    a:ASPECT! aspectId:ID FOR! 
    {
        aspect = new AspectDefinition( ToLexicalInfo(a), aspectId.getText() );
        conf.Aspects.Add(aspect);
    }
    (
        tr=type_name_def!
        {
            target = new TargetTypeDefinition( tr );
            target.TargetStrategy = TargetStrategyEnum.SingleType;
            aspect.TargetType = target;
        }
        |
        LBRACK! 
        {
            target = new TargetTypeDefinition( );
            aspect.TargetType = target;
            String namespaceRegEx = null;
        }
        (
            ASSIGNFROM LCURLY! tr=type_name_def! RCURLY!
            {
                target.TargetStrategy = TargetStrategyEnum.Assignable;
                target.AssignType = tr;
            }
            |
            CUSTOMMATCHER LCURLY! tr=type_name_def! RCURLY!
            {
                target.TargetStrategy = TargetStrategyEnum.Custom;
                target.CustomMatcherType = tr;
            }
            |
            (
                namespaceRegEx=identifier 
                {
                    target.TargetStrategy = TargetStrategyEnum.Namespace;
                    target.NamespaceRoot = namespaceRegEx;
                }
                (
                    EXCLUDES LCURLY type_list[target.Excludes] RCURLY
                )?
            )
        )
        RBRACK!
    )
    (
        (include[aspect])*
        (pointcut[aspect])*
    )
    END!
    ;

protected 
type_list [TypeReferenceCollection types] 
    {
        TypeReference tr = null;
    }:
    tr=type_name_def!
    {
        types.Add(tr);
    }
    (
        SEMI! tr=type_name_def!
        {
            types.Add(tr);
        }
    )*
    ;

protected
include [AspectDefinition aspect]
    {
        TypeReference tr = null;
        MixinDefinition md;
    }:
    i:INCLUDE!
    {
        md = new MixinDefinition( ToLexicalInfo(i) );
    }
    tr=type_name_or_ref!
    {
        md.TypeReference = tr;
        aspect.Mixins.Add( md );
    }
    ;

protected 
pointcut [AspectDefinition aspect]
    {
        PointCutDefinition pointcut = null;
        PointCutFlags flags = PointCutFlags.Unspecified;
    } :
    p:POINTCUT! flags=pointcutflags 
    {
        pointcut = new PointCutDefinition( ToLexicalInfo(p), flags );
        aspect.PointCuts.Add( pointcut );
    }
    pointcuttarget[pointcut]
    advices[pointcut]
    END!
    ;

protected
advices [PointCutDefinition pointcut] :
    (advice[pointcut])*
    ;

protected
advice [PointCutDefinition pointcut] 
    {
        TypeReference tr = null;
        InterceptorDefinition interDef = null;
    } :
    i:ADVICEINTERCEPTOR 
    {
        interDef = new InterceptorDefinition( ToLexicalInfo(i) );
    }
    LCURLY tr=type_name_or_ref!
    {
        interDef.TypeReference = tr;
        pointcut.Advices.Add( interDef );
    }
    RCURLY
    ;
    
protected
pointcutflags returns [PointCutFlags flags]
    {
        flags = PointCutFlags.Unspecified;
    } : 
    flags=pointcutflag[flags] (OR flags=pointcutflag[flags])?
    ;

protected
pointcutflag[PointCutFlags flags] returns [PointCutFlags retValue]
    {
        retValue = flags;
    } :
    METHOD
    { retValue |= PointCutFlags.Method; }
    |
    PROPERTY
    { retValue |= PointCutFlags.Property; }
    |
    PROPERTY_READ 
    { retValue |= PointCutFlags.PropertyRead; }
    |
    PROPERTY_WRITE
    { retValue |= PointCutFlags.PropertyWrite; }
    ;

protected
pointcuttarget [PointCutDefinition pointcut] :
    LCURLY!
    pointcutsignature[pointcut]
//    RCURLY!
    ;

 
protected
pointcutsig1 [PointCutDefinition pointcut]
{ 
    String part1;
        MethodSignature ms = null;
} :
( 
    ALL
    {
        part1 = "*"; 
        ms = AllMethodSignature.Instance;
    }
    |
    part1 = reg_ex[true]
    { ms = new MethodSignature(part1, methodAll("*")); }  
)
{ pointcut.Method = pointcutsig2(part1, ms); }
; 

protected
pointcutsig2 [String part1, MethodSignature ms] returns [MethodSignature ret]
{ 
    String part2; 
    ret = null;
} :
(
    ALL  { part2 = "*"; }
    ret = pointcutsig3[part1, part2, ms] 
   |
    LCURLY  
            pointcutarguments[ms]
    RCURLY
    RCURLY 
    { ret = ms; }
    | 
    RCURLY
    { ret = ms; } 
   |
    part2 = reg_ex[true] 
    ret = pointcutsig3[part1, part2, ms] 
) 
;

protected
pointcutsig3 [String part1, String part2, MethodSignature ms] returns [MethodSignature ret]
{ 
    String part3; 
    ret = null;
} :
(
    ALL { part3 = "*"; }
    { ms = new MethodSignature(part1, part2, methodAll(part3)); }
    pointcutsig4[ms]
   |
    LCURLY
    { ms = new MethodSignature(part1, methodAll(part2)); }
        pointcutarguments[ms]
    RCURLY
    RCURLY 
   |
    RCURLY
    { ms = new MethodSignature(part1, methodAll(part2)); }
   |        
    part3 = reg_ex[false]
    { ms = new MethodSignature(part1, part2, methodAll(part3)); }  
    pointcutsig4[ms] 
) { ret = ms; }
; 

protected
pointcutsig4 [MethodSignature ms]
:
(
    LCURLY
    pointcutarguments[ms]
    RCURLY
    RCURLY 
    |  
    RCURLY
)
; 

protected
pointcutsignature [PointCutDefinition pointcut]
:
    pointcutsig1[pointcut]
; 


protected 
pointcutarguments [MethodSignature ms] :
    (pointcutargument[ms] (COMMA pointcutargument[ms])*)?
    ;
    
protected 
pointcutargument [MethodSignature ms] 
    {
        String argType = String.Empty;
    }:
    ALL
    {
        ms.AddArgumentType( "*" );
    }
    |
    argType=reg_ex[false]!
    {
        ms.AddArgumentType( argType );
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
	    DOT!
	    id2:ID
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
reg_ex [Boolean allowALL] returns [String value]
	{
		value = null; sbuilder.Length = 0;
	}:
	( 
	    id:ID! 
	    {					
		    sbuilder.Append(id.getText());
		    value = sbuilder.ToString();
	    }
	    ( options { greedy = true; }:
	        WS
	        |
	        { if (LA(1) == ALL) {
	       
	            if (allowALL) 
	                break;
	            throw new NoViableAltException(LT(1), getFilename()); 
	           } 
	        }
	        | 
	        DOT!
	        {
	            sbuilder.Append('.');
	        }
	        ALL!
	        {
	            sbuilder.Append('*');
	        }
	    )?
	)
	{
	    value = sbuilder.ToString();
	}
	;
