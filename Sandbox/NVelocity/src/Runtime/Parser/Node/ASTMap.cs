using System;
using System.Collections;
using InternalContextAdapter = NVelocity.Context.InternalContextAdapter;
using Parser = NVelocity.Runtime.Parser.Parser;
using MethodInvocationException = NVelocity.Exception.MethodInvocationException;

namespace NVelocity.Runtime.Parser.Node {

    /// <summary>
    /// AST Node for creating a map / dictionary.
    /// This class was originally generated from Parset.jjt.
    /// </summary>
    /// <version>$Id: ASTMap.cs,v 1.2 2004/12/27 05:50:11 corts Exp $</version>
    public class ASTMap:SimpleNode {

	public ASTMap(int id):base(id) {
	}
		
	public ASTMap(Parser p, int id):base(p, id) {
	}
		
	/// <summary>
	/// Accept the visitor. 
	/// </summary>
	public override System.Object jjtAccept(ParserVisitor visitor, System.Object data) {
	    return visitor.visit(this, data);
	}
		
	/// <summary>
	/// Evaluate the node.
	/// </summary>
	public override System.Object value_Renamed(InternalContextAdapter context) {
	    int size = jjtGetNumChildren();
			
	    IDictionary objectMap = new Hashtable();
			
	    for (int i = 0; i < size; i += 2) {
		SimpleNode keyNode = (SimpleNode) jjtGetChild(i);
		SimpleNode valueNode = (SimpleNode) jjtGetChild(i + 1);
				
		System.Object key = (keyNode == null?null:keyNode.value_Renamed(context));
		System.Object value_Renamed = (valueNode == null?null:valueNode.value_Renamed(context));
				
		objectMap.Add(key, value_Renamed);
	    }
			
	    return objectMap;
	}

    }
}