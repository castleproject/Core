using System;
using System.Collections;
using NVelocity.Runtime.Parser.Node;

namespace NVelocity.Runtime.Visitor {

    /// <summary>
    /// This class is a visitor used by the VM proxy to change the
    /// literal representation of a reference in a VM.  The reason is
    /// to preserve the 'render literal if null' behavior w/o making
    /// the VMProxy stuff more complicated than it is already.
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    /// <version> $Id: VMReferenceMungeVisitor.cs,v 1.3 2003/10/27 13:54:11 corts Exp $</version>
    public class VMReferenceMungeVisitor : BaseVisitor {

	/// <summary>
	/// Map containing VM arg to instance-use reference
	/// Passed in with CTOR
	/// </summary>
	private Hashtable argmap = null;

	/// <summary>
	/// CTOR - takes a map of args to reference
	/// </summary>
	public VMReferenceMungeVisitor(Hashtable map) {
	    argmap = map;
	}

	/// <summary>
	/// Visitor method - if the literal is right, will
	/// set the literal in the ASTReference node
	/// </summary>
	/// <param name="node">ASTReference to work on</param>
	/// <param name="data">Object to pass down from caller</param>
	public override System.Object visit(ASTReference node, System.Object data) {
	    /*
	    *  see if there is an override value for this
	    *  reference
	    */
	    System.String override_Renamed = (System.String) argmap[node.literal().Substring(1)];

	    /*
	    *  if so, set in the node
	    */
	    if (override_Renamed != null) {
		node.Literal = override_Renamed;
	    }

	    /*
	    *  feed the children...
	    */
	    data = node.childrenAccept(this, data);

	    return data;
	}
    }
}
