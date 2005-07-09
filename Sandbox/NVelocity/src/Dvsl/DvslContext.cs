using System;
using System.Collections;

using NVelocity.Context;

namespace NVelocity.Dvsl {

    /// <summary>
    /// Context implementation that handles wrapping several
    /// contexts simultaneously.  The style context gets
    /// special treatment, getting checked first.
    /// </summary>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    class DvslContext : VelocityContext {

	protected internal IContext styleContext = null;
	protected internal IList contextList = new ArrayList();

	/// <summary>
	/// Used to hold the nodes as we get invoked from
	/// within the document for applyTemplates() duties
	/// </summary>
	private Stack nodeStack = new Stack();
	protected internal Hashtable ctx = new Hashtable();


	public DvslContext() {}

	public virtual DvslNode PushNode(DvslNode n) {
	    nodeStack.Push(n);
	    return n;
	}

	public virtual DvslNode PeekNode() {
	    return (DvslNode) nodeStack.Peek();
	}

	public virtual DvslNode PopNode() {
	    return (DvslNode) nodeStack.Pop();
	}

	public virtual void  ClearNode() {
	    nodeStack.Clear();
	    return ;
	}

	public virtual void ClearContexts() {
	    styleContext = null;
	    contextList.Clear();
	}

	public virtual void AddContext(IContext c) {
	    if (c != null)
		contextList.Add(c);
	}


	/// <summary>
	/// retrieves value for key from internal storage
	/// </summary>
	/// <param name="key">name of value to get</param>
	/// <returns>value as object</returns>
	public override Object InternalGet(String key) {
	    Object o = null;

	    /*
	    *  special tokens 
	    */
	    if (key.Equals("node")) {
		return PeekNode();
	    }

	    /*
	    *  start with local storage
	    */
	    o = ctx[key];
	    if (o != null)
		return o;

	    /*
	    *  if that doesn't work, try style first
	    *  then others
	    */
	    if (styleContext != null) {
		o = styleContext.Get(key);
		if (o != null)
		    return o;
	    }

	    for (int i = 0; i < contextList.Count; i++) {
		IContext c = (IContext) contextList[i];
		o = c.Get(key);
		if (o != null)
		    return o;
	    }
	    return null;
	}

	/// <summary>
	/// stores the value for key to internal storage
	/// </summary>
	/// <param name="key">name of value to store</param>
	/// <param name="value">value to store</param>
	/// <returns>previous value of key as Object</returns>
	public override Object InternalPut(String key, Object value) {
	    if (key.Equals("node"))
		return null;

	    return ctx[key] = value;
	}

	/// <summary>
	/// determines if there is a value for the given key
	/// </summary>
	/// <param name="key">name of value to check</param>
	/// <returns>true if non-null value in store</returns>
	public override bool InternalContainsKey(Object key) {
	    /*
	    *  start with local storage
	    */
	    if (ctx.ContainsKey(key))
		return true;

	    /*
	    *  if that doesn't work, try style first
	    *  then others
	    */
	    if (styleContext != null && styleContext.ContainsKey(key))
		return true;

	    for (int i = 0; i < contextList.Count; i++) {
		IContext c = (IContext) contextList[i];
		if (c.ContainsKey(key))
		    return true;
	    }

	    return false;
	}

	/// <summary>
	/// returns array of keys
	///
	/// $$$ GMJ todo
	///
	/// </summary>
	/// <returns>keys as []</returns>
	public override Object[] InternalGetKeys() {
	    return null;
	}

	/// <summary>
	/// remove a key/value pair from the
	/// internal storage
	/// </summary>
	/// <param name="key">name of value to remove</param>
	/// <returns>value removed</returns>
	public override Object InternalRemove(Object key) {
	    Object o = ctx[key];
	    ctx.Remove(key);
	    return o;
	}


	public virtual IContext StyleContext {
	    set {
		styleContext = value;
	    }
	}


    }
}
