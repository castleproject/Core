using System;
using System.Collections;
using IntrospectionCacheData = NVelocity.Util.Introspection.IntrospectionCacheData;
using Resource = NVelocity.Runtime.Resource.Resource;
using NVelocity.App.Events;

namespace NVelocity.Context {

    /// <summary>  class to encapsulate the 'stuff' for internal operation of velocity.
    /// We use the context as a thread-safe storage : we take advantage of the
    /// fact that it's a visitor  of sorts  to all nodes (that matter) of the
    /// AST during init() and render().
    /// Currently, it carries the template name for namespace
    /// support, as well as node-local context data introspection caching.
    /// *
    /// Note that this is not a public class.  It is for package access only to
    /// keep application code from accessing the internals, as AbstractContext
    /// is derived from this.
    /// *
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version> $Id: InternalContextBase.cs,v 1.4 2003/10/27 13:54:08 corts Exp $
    ///
    /// </version>
    [Serializable]

    public class InternalContextBase : InternalHousekeepingContext, InternalEventContext { //, System.Runtime.Serialization.ISerializable {
	public InternalContextBase() {
	    InitBlock();
	}
	private void  InitBlock() {
	    introspectionCache = new Hashtable(33);
	    templateNameStack = new System.Collections.Stack();
	}
	public virtual System.String CurrentTemplateName {
	    get {
		if ((templateNameStack.Count == 0))
		    return "<undef>";
		else
		    return (System.String) templateNameStack.Peek();
	    }

	}
	public virtual System.Object[] TemplateNameStack {
	    get {
		return templateNameStack.ToArray();
	    }

	}
	public virtual Resource CurrentResource {
	    get {
		return currentResource;
	    }

	    set {
		currentResource = value;
	    }

	}

	public virtual EventCartridge EventCartridge {
	    get {
		return eventCartridge;
	    }

	}
	/// <summary>  cache for node/context specific introspection information
	/// </summary>
	//UPGRADE_NOTE: The initialization of  'introspectionCache' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
	private Hashtable introspectionCache;

	/// <summary>  Template name stack. The stack top contains the current template name.
	/// </summary>
	//UPGRADE_NOTE: The initialization of  'templateNameStack' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
	private System.Collections.Stack templateNameStack;

	/// <summary>  EventCartridge we are to carry.  Set by application
	/// </summary>
	private EventCartridge eventCartridge = null;

	/// <summary>  Current resource - used for carrying encoding and other
	/// information down into the rendering process
	/// </summary>
	private Resource currentResource = null;

	/// <summary>  set the current template name on top of stack
	/// *
	/// </summary>
	/// <param name="s">current template name
	///
	/// </param>
	public virtual void  PushCurrentTemplateName(System.String s) {
	    System.Object temp_object;
	    temp_object = s;
	    System.Object generatedAux = temp_object;
	    templateNameStack.Push(temp_object);
	    return ;
	}

	/// <summary>  remove the current template name from stack
	/// </summary>
	public virtual void  PopCurrentTemplateName() {
	    templateNameStack.Pop();
	    return ;
	}

	/// <summary>  get the current template name
	/// *
	/// </summary>
	/// <returns>String current template name
	///
	/// </returns>

	/// <summary>  get the current template name stack
	/// *
	/// </summary>
	/// <returns>Object[] with the template name stack contents.
	///
	/// </returns>

	/// <seealso cref=" IntrospectionCacheData)
	/// object if exists for the key
	/// *
	/// "/>
	/// <param name="key"> key to find in cache
	/// </param>
	/// <returns>cache object
	///
	/// </returns>
	public virtual IntrospectionCacheData ICacheGet(System.Object key) {
	    return (IntrospectionCacheData) introspectionCache[key];
	}

	/// <seealso cref=" IntrospectionCacheData)
	/// element in the cache for specified key
	/// *
	/// "/>
	/// <param name="key"> key
	/// </param>
	/// <param name="o"> IntrospectionCacheData object to place in cache
	///
	/// </param>
	public virtual void  ICachePut(System.Object key, IntrospectionCacheData o) {
	    introspectionCache[key] = o;
	}


	public virtual EventCartridge AttachEventCartridge(EventCartridge ec) {
	    EventCartridge temp = eventCartridge;

	    eventCartridge = ec;

	    return temp;
	}

    }
}
