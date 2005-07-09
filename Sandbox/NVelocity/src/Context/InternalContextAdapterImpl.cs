using System;
using IntrospectionCacheData = NVelocity.Util.Introspection.IntrospectionCacheData;
using Resource = NVelocity.Runtime.Resource.Resource;
using NVelocity.App.Events;

namespace NVelocity.Context {

    /// <summary>  This adapter class is the container for all context types for internal
    /// use.  The AST now uses this class rather than the app-level Context
    /// interface to allow flexibility in the future.
    /// *
    /// Currently, we have two context interfaces which must be supported :
    /// <ul>
    /// <li> Context : used for application/template data access
    /// <li> InternalHousekeepingContext : used for internal housekeeping and caching
    /// <li> InternalWrapperContext : used for getting root cache context and other
    /// such.
    /// <li> InternalEventContext : for event handling.
    /// </ul>
    /// *
    /// This class implements the two interfaces to ensure that all methods are
    /// supported.  When adding to the interfaces, or adding more context
    /// functionality, the interface is the primary definition, so alter that first
    /// and then all classes as necessary.  As of this writing, this would be
    /// the only class affected by changes to InternalContext
    /// *
    /// This class ensures that an InternalContextBase is available for internal
    /// use.  If an application constructs their own Context-implementing
    /// object w/o subclassing AbstractContext, it may be that support for
    /// InternalContext is not available.  Therefore, InternalContextAdapter will
    /// create an InternalContextBase if necessary for this support.  Note that
    /// if this is necessary, internal information such as node-cache data will be
    /// lost from use to use of the context.  This may or may not be important,
    /// depending upon application.
    ///
    /// *
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version> $Id: InternalContextAdapterImpl.cs,v 1.4 2003/10/27 13:54:08 corts Exp $
    ///
    /// </version>


    //TODO: class was sealed
    public class InternalContextAdapterImpl : InternalContextAdapter {
	public virtual System.String CurrentTemplateName {
	    get {
		return icb.CurrentTemplateName;
	    }

	}
	public virtual System.Object[] TemplateNameStack {
	    get {
		return icb.TemplateNameStack;
	    }

	}
	public virtual Resource CurrentResource {
	    get {
		return icb.CurrentResource;
	    }

	    set {
		icb.CurrentResource = value;
	    }

	}
	public virtual System.Object[] Keys {
	    get {
		return context.Keys;
	    }

	}
	public virtual IContext InternalUserContext {
	    get {
		return context;
	    }

	}
	public virtual InternalContextAdapter BaseContext {
	    get {
		return this;
	    }

	}

	public virtual EventCartridge EventCartridge {
	    get {
		if (iec != null) {
		    return iec.EventCartridge;
		}

		return null;
	    }

	}
	///
	/// <summary>  the user data Context that we are wrapping
	/// </summary>
	internal IContext context = null;

	///
	/// <summary>  the ICB we are wrapping.  We may need to make one
	/// if the user data context implementation doesn't
	/// support one.  The default AbstractContext-derived
	/// VelocityContext does, and it's recommended that
	/// people derive new contexts from AbstractContext
	/// rather than piecing things together
	/// </summary>
	internal InternalHousekeepingContext icb = null;

	/// <summary>  The InternalEventContext that we are wrapping.  If
	/// the context passed to us doesn't support it, no
	/// biggie.  We don't make it for them - since its a
	/// user context thing, nothing gained by making one
	/// for them now
	/// </summary>
	internal InternalEventContext iec = null;

	/// <summary>  CTOR takes a Context and wraps it, delegating all 'data' calls
	/// to it.
	///
	/// For support of internal contexts, it will create an InternalContextBase
	/// if need be.
	/// </summary>
	public InternalContextAdapterImpl(IContext c) {
	    context = c;

	    if (!(c is InternalHousekeepingContext)) {
		icb = new InternalContextBase();
	    } else {
		icb = (InternalHousekeepingContext) context;
	    }

	    if (c is InternalEventContext) {
		iec = (InternalEventContext) context;
	    }
	}

	/* --- InternalHousekeepingContext interface methods --- */

	public void PushCurrentTemplateName(System.String s) {
	    icb.PushCurrentTemplateName(s);
	}

	public void PopCurrentTemplateName() {
	    icb.PopCurrentTemplateName();
	}



	public IntrospectionCacheData ICacheGet(System.Object key) {
	    return icb.ICacheGet(key);
	}

	public void ICachePut(System.Object key, IntrospectionCacheData o) {
	    icb.ICachePut(key, o);
	}




	/* ---  Context interface methods --- */

	public System.Object Put(System.String key, System.Object value_Renamed) {
	    return context.Put(key, value_Renamed);
	}

	public System.Object Get(System.String key) {
	    return context.Get(key);
	}

	public bool ContainsKey(System.Object key) {
	    return context.ContainsKey(key);
	}


	public System.Object Remove(System.Object key) {
	    return context.Remove(key);
	}


	/* ---- InternalWrapperContext --- */

	/// <summary>  returns the user data context that
	/// we are wrapping
	/// </summary>

	/// <summary>  Returns the base context that we are
	/// wrapping. Here, its this, but for other thing
	/// like VM related context contortions, it can
	/// be something else
	/// </summary>

	/* -----  InternalEventContext ---- */

	public EventCartridge AttachEventCartridge(EventCartridge ec) {
	    if (iec != null) {
		return iec.AttachEventCartridge(ec);
	    }

	    return null;
	}

    }
}
