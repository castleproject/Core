using System;
using System.Collections;

using NVelocity;
using NVelocity.Context;

namespace NVelocity.Dvsl {

    /// <summary>  <p>
    /// Context implementation that is the outer context
    /// during the transformation.  Holds the node stack
    /// and also protects the 'special' context elements
    /// like 'node'
    /// </p>
    /// <p>
    /// There are special elements like 'node', which is
    /// readonly and corresponds to the current node, and
    /// 'attrib', which corresponds to a map of attributes
    /// for the current node.
    /// </p>
    /// </summary>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    class DVSLNodeContext : VelocityContext {

	/// <summary>
	/// Magic context entity that corresponds
	/// to the current node
	/// </summary>
	private const String NODE = "node";

	/// <summary>
	/// Magic context entity that corresponds to
	/// a Map of attributes for the current node
	/// </summary>
	private const String ATTRIB = "attrib";

	/// <summary>
	/// Used to hold the nodes as we get invoked from
	/// within the document for applyTemplates() duties
	/// </summary>
	private Stack nodeStack = new Stack();

	protected internal Hashtable ctx = new Hashtable();

	public DVSLNodeContext(IContext context):base(context) {}

	public DVSLNodeContext() {}

	/// <summary>
	/// retrieves value for key from internal storage
	/// </summary>
	/// <param name="key">name of value to get</param>
	/// <returns>value as object</returns>
	public override Object InternalGet(String key) {
	    Object o = null;

	    /*
	    *  special token : NODE
	    *
	    *  returns current node
	    */

	    if (key.Equals(NODE)) {
		return PeekNode();
	    }

	    /*
	    *  ATTRIB - returns attribute map
	    */
	    if (key.Equals(ATTRIB)) {
		DvslNode n = PeekNode();
		return n.AttribMap;
	    }

	    /*
	    *  start with local storage
	    */
	    return ctx[key];
	}

	/// <summary>
	/// stores the value for key to internal
	/// storage
	/// </summary>
	/// <param name="key">name of value to store
	/// </param>
	/// <param name="value">value to store
	/// </param>
	/// <returns>previous value of key as Object
	/// </returns>
	public override Object InternalPut(String key, Object value) {
	    /*
	    *  protect both NODE and ATTRIB for now.  We
	    *  might want to let people set ATTRIB, but
	    *  I suspect not
	    */

	    if (key.Equals(NODE))
		return null;

	    if (key.Equals(ATTRIB))
		return null;

	    return ctx[key] = value;
	}

	/// <summary>
	/// determines if there is a value for the
	/// given key
	/// </summary>
	/// <param name="key">name of value to check
	/// </param>
	/// <returns>true if non-null value in store
	/// </returns>
	public override bool InternalContainsKey(Object key) {
	    return ctx.ContainsKey(key);
	}

	/// <summary>
	/// returns array of keys
	/// $$$ GMJ todo
	/// </summary>
	/// <returns>keys as []
	/// </returns>
	public override Object[] InternalGetKeys() {
	    return null;
	}

	/// <summary>
	/// remove a key/value pair from the internal storage
	/// </summary>
	/// <param name="key">name of value to remove</param>
	/// <returns>value removed</returns>
	public override Object InternalRemove(Object key) {
	    Object o = ctx[key];
	    ctx.Remove(key);
	    return o;
	}


	/* === routines to manage current node stack === */

	internal virtual DvslNode PushNode(DvslNode n) {
	    nodeStack.Push(n);
	    return n;
	}

	internal virtual DvslNode PeekNode() {
	    return (DvslNode) nodeStack.Peek();
	}

	internal virtual DvslNode PopNode() {
	    return (DvslNode) nodeStack.Pop();
	}

	internal virtual void ClearNode() {
	    nodeStack.Clear();
	    return;
	}


    }
}
