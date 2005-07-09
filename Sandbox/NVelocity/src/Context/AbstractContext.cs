using System;


namespace NVelocity.Context {

    /// <summary>  This class is the abstract base class for all conventional
    /// Velocity Context  implementations.  Simply extend this class
    /// and implement the abstract routines that access your preferred
    /// storage method.
    /// *
    /// Takes care of context chaining.
    /// *
    /// Also handles / enforces policy on null keys and values :
    /// *
    /// <ul>
    /// <li> Null keys and values are accepted and basically dropped.
    /// <li> If you place an object into the context with a null key, it
    /// will be ignored and logged.
    /// <li> If you try to place a null into the context with any key, it
    /// will be dropped and logged.
    /// </ul>
    /// *
    /// The default implementation of this for application use is
    /// org.apache.velocity.VelocityContext.
    /// *
    /// All thanks to Fedor for the chaining idea.
    /// *
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author> <a href="mailto:fedor.karpelevitch@home.com">Fedor Karpelevitch</a>
    /// </author>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version> $Id: AbstractContext.cs,v 1.4 2003/10/27 13:54:08 corts Exp $
    ///
    /// </version>

    [Serializable]
    public abstract class AbstractContext:InternalContextBase, IContext { //, System.Runtime.Serialization.ISerializable

	public virtual System.Object[] Keys {
	    get {
		return InternalGetKeys();
	    }

	}

	public virtual IContext ChainedContext {
	    get {
		return innerContext;
	    }

	}
	/// <summary>  the chained Context if any
	/// </summary>
	private IContext innerContext = null;

	///
	/// <summary>  Implement to return a value from the context storage.
	/// <br><br>
	/// The implementation of this method is required for proper
	/// operation of a Context implementation in general
	/// Velocity use.
	///
	/// </summary>
	/// <param name="key">key whose associated value is to be returned
	/// </param>
	/// <returns>object stored in the context
	///
	/// </returns>
	public abstract System.Object InternalGet(System.String key);

	///
	/// <summary>  Implement to put a value into the context storage.
	/// <br><br>
	/// The implementation of this method is required for
	/// proper operation of a Context implementation in
	/// general Velocity use.
	/// *
	/// </summary>
	/// <param name="key">key with which to associate the value
	/// </param>
	/// <param name="value">value to be associated with the key
	/// </param>
	/// <returns>previously stored value if exists, or null
	///
	/// </returns>
	public abstract System.Object InternalPut(System.String key, System.Object value_Renamed);

	///
	/// <summary>  Implement to determine if a key is in the storage.
	/// <br><br>
	/// Currently, this method is not used internally by
	/// the Velocity core.
	/// *
	/// </summary>
	/// <param name="key">key to test for existance
	/// </param>
	/// <returns>true if found, false if not
	///
	/// </returns>
	public abstract bool InternalContainsKey(System.Object key);

	///
	/// <summary>  Implement to return an object array of key
	/// strings from your storage.
	/// <br><br>
	/// Currently, this method is not used internally by
	/// the Velocity core.
	/// *
	/// </summary>
	/// <returns>array of keys
	///
	/// </returns>
	public abstract System.Object[] InternalGetKeys();

	///
	/// <summary>  I mplement to remove an item from your storage.
	/// <br><br>
	/// Currently, this method is not used internally by
	/// the Velocity core.
	/// *
	/// </summary>
	/// <param name="key">key to remove
	/// </param>
	/// <returns>object removed if exists, else null
	///
	/// </returns>
	public abstract System.Object InternalRemove(System.Object key);

	/// <summary>  default CTOR
	/// </summary>
	public AbstractContext() {}

	/// <summary>  Chaining constructor accepts a Context argument.
	/// It will relay get() operations into this Context
	/// in the even the 'local' get() returns null.
	///
	/// </summary>
	/// <param name="inner">context to be chained
	///
	/// </param>
	public AbstractContext(IContext inner) {
	    innerContext = inner;

	    /*
	    *  now, do a 'forward pull' of event cartridge so
	    *  it's accessable, bringing to the top level.
	    */

	    if (innerContext is InternalEventContext) {
		AttachEventCartridge(((InternalEventContext) innerContext).EventCartridge);
	    }
	}

	/// <summary> Adds a name/value pair to the context.
	///
	/// </summary>
	/// <param name="key">  The name to key the provided value with.
	/// </param>
	/// <param name="value">The corresponding value.
	/// </param>
	/// <returns>Object that was replaced in the the Context if
	/// applicable or null if not.
	///
	/// </returns>
	public virtual System.Object Put(System.String key, System.Object value_Renamed) {
	    /*
	    * don't even continue if key or value is null
	    */

	    if (key == null) {
		return null;
	    } else if (value_Renamed == null) {
		return null;
	    }

	    return InternalPut(key, value_Renamed);
	}

	/// <summary>  Gets the value corresponding to the provided key from the context.
	/// *
	/// Supports the chaining context mechanism.  If the 'local' context
	/// doesn't have the value, we try to get it from the chained context.
	/// *
	/// </summary>
	/// <param name="key">The name of the desired value.
	/// </param>
	/// <returns>   The value corresponding to the provided key or null if
	/// the key param is null.
	///
	/// </returns>
	public virtual System.Object Get(System.String key) {
	    /*
	    *  punt if key is null
	    */

	    if (key == null) {
		return null;
	    }

	    /*
	    *  get the object for this key.  If null, and we are chaining another Context
	    *  call the get() on it.
	    */

	    System.Object o = InternalGet(key);

	    if (o == null && innerContext != null) {
		o = innerContext.Get(key);
	    }

	    return o;
	}

	/// <summary>  Indicates whether the specified key is in the context.  Provided for
	/// debugging purposes.
	/// *
	/// </summary>
	/// <param name="key">The key to look for.
	/// </param>
	/// <returns>true if the key is in the context, false if not.
	///
	/// </returns>
	public virtual bool ContainsKey(System.Object key) {
	    if (key == null) {
		return false;
	    }

	    return InternalContainsKey(key);
	}

	/// <summary>  Get all the keys for the values in the context
	/// </summary>
	/// <returns>Object[] of keys in the Context. Does not return
	/// keys in chained context.
	///
	/// </returns>

	/// <summary> Removes the value associated with the specified key from the context.
	/// *
	/// </summary>
	/// <param name="key">The name of the value to remove.
	/// </param>
	/// <returns>   The value that the key was mapped to, or <code>null</code>
	/// if unmapped.
	///
	/// </returns>
	public virtual System.Object Remove(System.Object key) {
	    if (key == null) {
		return null;
	    }

	    return InternalRemove(key);
	}

	/// <summary>  returns innerContext if one is chained
	/// *
	/// </summary>
	/// <returns>Context if chained, <code>null</code> if not
	///
	/// </returns>
    }
}
