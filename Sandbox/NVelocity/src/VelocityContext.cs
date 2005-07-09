using System;
using System.Collections;
using NVelocity.Context;


namespace NVelocity {

    /// <summary>
    /// General purpose implemention of the application Context
    /// interface for general application use.  This class should
    /// be used in place of the original Context class.
    /// </summary>
    /// <seealso cref=" java.util.HashMap )
    /// for data storage.
    ///
    /// This context implementation cannot be shared between threads
    /// without those threads synchronizing access between them, as
    /// the HashMap is not synchronized, nor are some of the fundamentals
    /// of AbstractContext.  If you need to share a Context between
    /// threads with simultaneous access for some reason, please create
    /// your own and extend the interface Context
    ///
    /// "/>
    /// <seealso cref="NVelocity.Context.Context"/>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
    /// <author> <a href="mailto:fedor.karpelevitch@home.com">Fedor Karpelevitch</a></author>
    /// <author> <a href="mailto:dlr@finemaltcoding.com">Daniel Rall</a></author>
    /// <version> $Id: VelocityContext.cs,v 1.4 2003/10/27 13:54:07 corts Exp $</version>
    public class VelocityContext:AbstractContext {

	/// <summary>
	/// Storage for key/value pairs.
	/// </summary>
	private Hashtable context = null;

	/// <summary>
	/// Creates a new instance (with no inner context).
	/// </summary>
	public VelocityContext():this(null, null) {}

	///
	/// <summary>
	/// Creates a new instance with the provided storage (and no inner context).
	/// </summary>
	public VelocityContext(Hashtable context):this(context, null) {}

	/// <summary>
	/// Chaining constructor, used when you want to
	/// wrap a context in another.  The inner context
	/// will be 'read only' - put() calls to the
	/// wrapping context will only effect the outermost
	/// context
	/// </summary>
	/// <param name="innerContext">The <code>Context</code> implementation to wrap.</param>
	public VelocityContext(IContext innerContext):this(null, innerContext) {}

	/// <summary>
	/// Initializes internal storage (never to <code>null</code>), and
	/// inner context.
	/// </summary>
	/// <param name="context">Internal storage, or <code>null</code> to
	/// create default storage.
	/// </param>
	/// <param name="innerContext">Inner context.
	///
	/// </param>
	public VelocityContext(Hashtable context, IContext innerContext):base(innerContext) {
	    this.context = (context == null?new Hashtable():context);
	}

	/// <summary>
	/// retrieves value for key from internal
	/// storage
	/// </summary>
	/// <param name="key">name of value to get</param>
	/// <returns>value as object</returns>
	public override System.Object InternalGet(System.String key) {
	    return context[key];
	}

	/// <summary>
	/// stores the value for key to internal
	/// storage
	/// </summary>
	/// <param name="key">name of value to store</param>
	/// <param name="value">value to store</param>
	/// <returns>previous value of key as Object</returns>
	public override System.Object InternalPut(System.String key, System.Object value_Renamed) {
	    return context[key] = value_Renamed;
	}

	/// <summary>
	/// determines if there is a value for the
	/// given key
	/// </summary>
	/// <param name="key">name of value to check</param>
	/// <returns>true if non-null value in store</returns>
	public override bool InternalContainsKey(System.Object key) {
	    return context.ContainsKey(key);
	}

	/// <summary>
	/// returns array of keys
	/// </summary>
	/// <returns>keys as []</returns>
	public override System.Object[] InternalGetKeys() {
	    throw new NotImplementedException();

	    //TODO
	    //return context.keySet().toArray();
	}

	/// <summary>
	/// remove a key/value pair from the
	/// internal storage
	/// </summary>
	/// <param name="key">name of value to remove</param>
	/// <returns>value removed</returns>
	public override System.Object InternalRemove(System.Object key) {
	    Object o = context[key];
	    context.Remove(key);
	    return o;
	}

	/// <summary>
	/// Clones this context object.
	/// </summary>
	/// <returns>A deep copy of this <code>Context</code>.</returns>
	public System.Object Clone() {
	    VelocityContext clone = null;
	    try {
		clone = (VelocityContext) base.MemberwiseClone();
		clone.context = new Hashtable(context);
	    } catch (System.Exception) {
		// ignore
	    }
	    return clone;
	}
    }
}
