using System;
using System.Collections;
using SimpleNode = NVelocity.Runtime.Parser.Node.SimpleNode;
using NodeUtils = NVelocity.Runtime.Parser.Node.NodeUtils;
using Token = NVelocity.Runtime.Parser.Token;
using InternalContextAdapter = NVelocity.Context.InternalContextAdapter;
using Node = NVelocity.Runtime.Parser.Node.INode;

namespace NVelocity.Runtime.Directive {

    /// <summary>
    /// Macro.java
    ///
    /// Macro implements the macro definition directive of VTL.
    ///
    /// example :
    ///
    /// #macro( isnull $i )
    /// #if( $i )
    /// $i
    /// #end
    /// #end
    ///
    /// This object is used at parse time to mainly process and register the
    /// macro.  It is used inline in the parser when processing a directive.
    ///
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    /// <version> $Id: Macro.cs,v 1.3 2003/10/27 13:54:10 corts Exp $</version>
    public class Macro:Directive {

	public override System.String Name {
	    get {
		return "macro";
	    }
	    set {
		throw new NotSupportedException();
	    }
	}

	public override int Type {
	    get {
		return NVelocity.Runtime.Directive.DirectiveConstants_Fields.BLOCK;
	    }
	}

	private static bool debugMode = false;

	/// <summary> Return name of this directive.
	/// </summary>

	/// <summary> Return type of this directive.
	/// </summary>

	/// <summary>   render() doesn't do anything in the final output rendering.
	/// There is no output from a #macro() directive.
	/// </summary>
	public override bool render(InternalContextAdapter context, System.IO.TextWriter writer, Node node) {
	    /*
	    *  do nothing : We never render.  The VelocimacroProxy object does that
	    */
	    return true;
	}

	public override void  init(RuntimeServices rs, InternalContextAdapter context, Node node) {
	    base.init(rs, context, node);

	    /*
	    * again, don't do squat.  We want the AST of the macro 
	    * block to hang off of this but we don't want to 
	    * init it... it's useless...
	    */
	    return ;
	}

	/// <summary>
	/// Used by Parser.java to process VMs withing the parsing process
	///
	/// processAndRegister() doesn't actually render the macro to the output
	/// Processes the macro body into the internal representation used by the
	/// VelocimacroProxy objects, and if not currently used, adds it
	/// to the macro Factory
	/// </summary>
	public static void  processAndRegister(RuntimeServices rs, Node node, System.String sourceTemplate) {
	    /*
	    *  There must be at least one arg to  #macro,
	    *  the name of the VM.  Note that 0 following 
	    *  args is ok for naming blocks of HTML
	    */
	    int numArgs = node.jjtGetNumChildren();

	    /*
	    *  this number is the # of args + 1.  The + 1
	    *  is for the block tree
	    */
	    if (numArgs < 2) {

		/*
		*  error - they didn't name the macro or
		*  define a block
		*/
		rs.error("#macro error : Velocimacro must have name as 1st " + "argument to #macro()");

		return ;
	    }

	    /*
	    *  get the arguments to the use of the VM
	    */
	    System.String[] argArray = getArgArray(node);

	    /*
	    *   now, try and eat the code block. Pass the root.
	    */
	    IList macroArray = getASTAsStringArray(node.jjtGetChild(numArgs - 1));

	    /*
	    *  make a big string out of our macro
	    */
	    System.Text.StringBuilder temp = new System.Text.StringBuilder();

	    for (int i = 0; i < macroArray.Count; i++)
		temp.Append(macroArray[i]);

	    System.String macroBody = temp.ToString();

	    /*
	    * now, try to add it.  The Factory controls permissions, 
	    * so just give it a whack...
	    */
	    bool bRet = rs.addVelocimacro(argArray[0], macroBody, argArray, sourceTemplate);

	    return ;
	}


	/// <summary>  creates an array containing the literal
	/// strings in the macro arguement
	/// </summary>
	private static System.String[] getArgArray(Node node) {
	    /*
	    *  remember : this includes the block tree
	    */

	    int numArgs = node.jjtGetNumChildren();

	    numArgs--; // avoid the block tree...

	    System.String[] argArray = new System.String[numArgs];

	    int i = 0;

	    /*
	    *  eat the args
	    */

	    while (i < numArgs) {
		argArray[i] = node.jjtGetChild(i).FirstToken.image;

		/*
		*  trim off the leading $ for the args after the macro name.
		*  saves everyone else from having to do it
		*/

		if (i > 0) {
		    if (argArray[i].StartsWith("$"))
			argArray[i] = argArray[i].Substring(1, (argArray[i].Length) - (1));
		}

		i++;
	    }

	    if (debugMode) {
		System.Console.Out.WriteLine("Macro.getArgArray() : #args = " + numArgs);
		System.Console.Out.Write(argArray[0] + "(");

		for (i = 1; i < numArgs; i++)
		    System.Console.Out.Write(" " + argArray[i]);

		System.Console.Out.WriteLine(" )");
	    }

	    return argArray;
	}

	/// <summary>  Returns an array of the literal rep of the AST
	/// </summary>
	private static IList getASTAsStringArray(Node rootNode) {
	    /*
	    *  this assumes that we are passed in the root 
	    *  node of the code block
	    */

	    Token t = rootNode.FirstToken;
	    Token tLast = rootNode.LastToken;

	    /*
	    *  now, run down the part of the tree bounded by
	    *  our first and last tokens
	    */

	    ArrayList list = new ArrayList();

	    t = rootNode.FirstToken;

	    while (t != tLast) {
		list.Add(NodeUtils.tokenLiteral(t));
		t = t.next;
	    }

	    /*
	    *  make sure we get the last one...
	    */

	    list.Add(NodeUtils.tokenLiteral(t));

	    return list;
	}


    }
}
