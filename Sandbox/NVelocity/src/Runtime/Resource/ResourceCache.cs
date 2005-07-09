using System;
using System.Collections;

namespace NVelocity.Runtime.Resource {

    /// <summary> Interface that defines the shape of a pluggable resource cache
    /// for the included ResourceManager
    /// *
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version> $Id: ResourceCache.cs,v 1.3 2003/10/27 13:54:11 corts Exp $
    ///
    /// </version>
    public interface ResourceCache {
	/// <summary>  initializes the ResourceCache.  Will be
	/// called before any utilization
	/// *
	/// </summary>
	/// <param name="rs">RuntimeServices to use for logging, etc
	///
	/// </param>
	void  initialize(RuntimeServices rs);
	/// <summary>  retrieves a Resource from the
	/// cache
	/// *
	/// </summary>
	/// <param name="resourceKey">key for Resource to be retrieved
	/// </param>
	/// <returns>Resource specified or null if not found
	///
	/// </returns>
	Resource get
	    (System.Object resourceKey);
	/// <summary>  stores a Resource in the cache
	/// *
	/// </summary>
	/// <param name="resourceKey">key to associate with the Resource
	/// </param>
	/// <param name="resource">Resource to be stored
	/// </param>
	/// <returns>existing Resource stored under this key, or null if none
	///
	/// </returns>
	Resource put(System.Object resourceKey, Resource resource);
	/// <summary>  removes a Resource from the cache
	/// *
	/// </summary>
	/// <param name="resourceKey">resource to be removed
	/// </param>
	/// <param name="Resource">stored under key
	///
	/// </param>
	Resource remove
	    (System.Object resourceKey);
	/// <summary>  returns an Iterator of Keys in the cache
	/// </summary>
	IEnumerator enumerateKeys();
    }
}
