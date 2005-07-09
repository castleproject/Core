using System;
using IntrospectionCacheData = NVelocity.Util.Introspection.IntrospectionCacheData;
using Resource = NVelocity.Runtime.Resource.Resource;


namespace NVelocity.Context {

    /// <summary>
    /// interface to encapsulate the 'stuff' for internal operation of velocity.
    /// We use the context as a thread-safe storage : we take advantage of the
    /// fact that it's a visitor  of sorts  to all nodes (that matter) of the
    /// AST during init() and render().
    ///
    /// Currently, it carries the template name for namespace
    /// support, as well as node-local context data introspection caching.
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    /// <author> <a href="mailto:Christoph.Reck@dlr.de">Christoph Reck</a></author>
    /// <version> $Id: InternalHousekeepingContext.cs,v 1.4 2003/10/27 13:54:08 corts Exp $</version>
    public interface InternalHousekeepingContext {

	System.String CurrentTemplateName {
	    get
		;
	    }

	    System.Object[] TemplateNameStack {
		get
		    ;
		}

		Resource CurrentResource {
		    get
			;
			set
			    ;
			}

			/// <summary>
			/// set the current template name on top of stack
			/// </summary>
			/// <param name="s">current template name</param>
			void  PushCurrentTemplateName(System.String s);

	/// <summary>
	/// remove the current template name from stack
	/// </summary>
	void  PopCurrentTemplateName();

	/// <summary>  get the current template name
	/// *
	/// </summary>
	/// <returns>String current template name
	///
	/// </returns>
	/// <summary>  Returns the template name stack in form of an array.
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
	IntrospectionCacheData ICacheGet(System.Object key);

	/// <seealso cref=" IntrospectionCacheData)
	/// element in the cache for specified key
	/// *
	/// "/>
	/// <param name="key"> key
	/// </param>
	/// <param name="o"> IntrospectionCacheData object to place in cache
	///
	/// </param>
	void  ICachePut(System.Object key, IntrospectionCacheData o);

	/// <summary>
	/// temporary fix to enable #include() to figure out
	/// current encoding.
	/// </summary>
    }
}
