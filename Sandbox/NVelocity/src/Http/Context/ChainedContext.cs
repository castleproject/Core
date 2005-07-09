using System;
using System.Web;
using System.Web.SessionState;

using NVelocity;
using NVelocity.Context;

namespace NVelocity.Http.Context {


    /// <summary>
    /// <p>Velocity context implementation specific to the Servlet environment.</p>
    /// <p>It provides the following special features:</p>
    /// <ul>
    /// <li>puts the request, response, session, and servlet context objects
    /// into the Velocity context for direct access, and keeps them
    /// read-only</li>
    /// <li>supports a read-only toolbox of view tools</li>
    /// <li>auto-searches servlet request attributes, session attributes and
    /// servlet context attribues for objects</li>
    /// </ul>
    /// <p>The {@link #internalGet(String key)} method implements the following search order
    /// for objects:</p>
    /// <ol>
    /// <li>servlet request, servlet response, servlet session, servlet context</li>
    /// <li>toolbox</li>
    /// <li>local hashtable of objects (traditional use)</li>
    /// <li>servlet request attribues, servlet session attribute, servlet context
    /// attributes</li>
    /// </ol>
    /// <p>The purpose of this class is to make it easy for web designer to work
    /// with Java servlet based web applications. They do not need to be concerned
    /// with the concepts of request, session or application attributes and the
    /// live time of objects in these scopes.</p>
    /// <p>Note that the put() method always puts objects into the local hashtable.
    /// </p>
    /// </summary>
    /// <author><a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    /// <author><a href="mailto:sidler@teamup.com">Gabe Sidler</a></author>
    public class ChainedContext : VelocityContext, IViewContext {

	/// <summary>
	/// A local reference to the http context.
	/// </summary>
	private HttpContext httpContext;

	/// <summary>
	/// The toolbox.
	/// </summary>
	private ToolboxContext toolboxContext = null;

	/// <summary>
	/// Key to the HTTP request object.
	/// </summary>
	public const System.String REQUEST = "request";

	/// <summary>
	/// Key to the HTTP response object.
	/// </summary>
	public const System.String RESPONSE = "response";

	/// <summary>
	/// Key to the HTTP session object.
	/// </summary>
	public const System.String SESSION = "session";

	/// <summary>
	/// Key to the HttpContext application object.
	/// </summary>
	public const System.String APPLICATION = "application";

	/// <summary>
	/// Key to the HttpContext object.
	/// </summary>
	public const System.String CONTEXT = "context";


	/// <summary>
	/// Default constructor.
	/// </summary>
	public ChainedContext(IContext ctx, HttpContext httpContext):base(null, ctx) {
	    this.httpContext = httpContext;
	}


	/// <summary>
	/// <p>Looks up and returns the object with the specified key.</p>
	/// <p>See the class documentation for more details.</p>
	/// </summary>
	/// <param name="key">the key of the object requested</param>
	/// <returns>the requested object or null if not found</returns>
	public override System.Object InternalGet(System.String key) {
	    // make the four scopes of the Apocalypse Read only
	    // really five in .Net, but the comment was too good to change
	    if (key.Equals(REQUEST)) {
		return httpContext.Request;
	    } else if (key.Equals(RESPONSE)) {
		return httpContext.Response;
	    } else if (key.Equals(SESSION)) {
		return httpContext.Session;
	    } else if (key.Equals(APPLICATION)) {
		return httpContext.Application;
	    } else if (key.Equals(CONTEXT)) {
		return httpContext;
	    }

	    // search the toolbox
	    System.Object o = null;

	    if (toolboxContext != null) {
		o = toolboxContext.Get(key);
		if (o != null) {
		    return o;
		}
	    }

	    // try the local hashtable
	    o = base.InternalGet(key);

	    // if not found, wander down the scopes...
	    if (o == null) {
		o = httpContext.Items[key];
		if (o == null) {
		    o = httpContext.Request[key];
		    if (o == null) {
			if (httpContext.Session != null) {
			    o = httpContext.Session[key];
			}
			if (o == null) {
			    o = httpContext.Application[key];
			}
		    }
		}
	    }

	    return o;
	}

	/// <summary>
	/// <p>Sets the toolbox of view tools.</p>
	/// </summary>
	/// <param name="box">toolbox of view tools</param>
	public virtual ToolboxContext Toolbox {
	    set {
		toolboxContext = value;
	    }
	}


	/// <summary>
	/// <p>Returns the servlet context.</p>
	/// </summary>
	public virtual HttpContext HttpContext {
	    get {
		return httpContext;
	    }
	}


	/// <summary>
	/// <p>Returns a reference to the Velocity context (this object).</p>
	/// </summary>
	public virtual IContext VelocityContext {
	    get {
		return this;
	    }
	}


    }
}
