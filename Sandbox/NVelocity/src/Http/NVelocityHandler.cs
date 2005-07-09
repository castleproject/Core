using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;

using Commons.Collections;

using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.IO;
using NVelocity.Runtime;
using NVelocity.Util;
using NVelocity.Http.Resource.Loader;

namespace NVelocity.Http {

    public abstract class NVelocityHandler : HandlerSkeleton, IHttpHandler, IRequiresSessionState {

	// The HTTP request object context key.
	public static readonly String CONTEXT = "context";

	// The HTTP request object context key.
	public static readonly String REQUEST = "request";

	// The HTTP response object context key.
	public static readonly String RESPONSE = "response";

	// The HTTP content type context key.
	public static readonly String CONTENT_TYPE = "default.contentType";

	// The default content type for the response
	public static readonly String DEFAULT_CONTENT_TYPE = "text/html";


	// Encoding for the output stream
	public static readonly String DEFAULT_OUTPUT_ENCODING = "ISO-8859-1";

	// The encoding to use when generating outputing.
	private static String encoding = null;

	// The default content type.
	private static String defaultContentType;

	// parameters available to IHttpHandlerFactor - passed through
	protected HttpContext context;
	protected String requestType;
	protected String url;
	protected String pathTranslated;

	/// <summary>
	/// Implementation of IHttpHandler method, called by container upon
	/// loading (is Isreusable returns true, instances may be pooled)
	/// </summary>
	/// <param name="context"></param>
	/// <param name="requestType"></param>
	/// <param name="url"></param>
	/// <param name="pathTranslated"></param>
	public virtual void Init(HttpContext context, String requestType, String url, String pathTranslated) {
	    this.context = context;
	    this.requestType = requestType;
	    this.url = url;
	    this.pathTranslated = pathTranslated;

	    lock(this) {
		if (!initialized) {
		    // do whatever we have to do to init Velocity
		    InitVelocity();

		    // we can get these now that velocity is initialized
		    defaultContentType = RuntimeSingleton.getString(CONTENT_TYPE, DEFAULT_CONTENT_TYPE);
		    encoding = RuntimeSingleton.getString(RuntimeConstants_Fields.OUTPUT_ENCODING, DEFAULT_OUTPUT_ENCODING);

		    initialized = true;
		}
	    }

	    return;
	}


	/// <summary>
	/// Handles all requests
	/// </summary>
	/// <param name="context">HttpContext for current request</param>
	public virtual void ProcessRequest(HttpContext context) {
	    try {
		// first, get a context
		IContext ctx = CreateContext(context);

		// set the content type
		SetContentType(context);

		// let someone handle the request
		Template template = HandleRequest(context, ctx)
		;

		// bail if we can't find the template
		if (template == null) {
		    return;
		}

		// now merge it
		MergeTemplate(template, ctx, context.Response.Output)
		;

		// call cleanup routine to let a derived class do some cleanup
		RequestCleanup(context, ctx);
	    } catch (System.Exception e) {
		// call the error handler to let the derived class
		// do something useful with this failure.
		Error(context, e);
	    }
	}


	/// <summary>
	/// cleanup routine called at the end of the request processing sequence
	/// allows a derived class to do resource cleanup or other end of
	/// process cycle tasks
	/// </summary>
	/// <param name="context">HttpContext passed from client</param>
	/// <param name="ctx">velocity context create by the CreateContext() method</param>
	protected virtual void RequestCleanup(HttpContext context, IContext ctx ) {
	    return;
	}


	/// <summary>
	/// merges the template with the context.  Only override this if you really, really
	/// really need to. (And don't call us with questions if it breaks :)
	/// </summary>
	/// <param name="template">template object returned by the handleRequest() method</param>
	/// <param name="ctx">context created by the CreateContext() method</param>
	/// <param name="response">TextWriter to write to (i.e. Response.Output)</param>
	protected virtual void MergeTemplate(Template template, IContext ctx, TextWriter response) {
	    template.Merge(ctx, response);
	}


	/// <summary>
	/// Sets the content type of the response.  This is available to be overriden
	/// by a derived class.
	///
	/// The default implementation is :
	///
	///	response.setContentType( defaultContentType );
	///
	/// where defaultContentType is set to the value of the default.contentType
	/// property, or "text/html" if that is not set.
	/// </summary>
	/// <param name="context">HttpContext for current request</param>
	protected virtual void SetContentType(HttpContext context) {
	    context.Response.ContentType = defaultContentType;
	}


	/// <summary>
	/// Returns a context suitable to pass to the handleRequest() method
	/// <br><br>
	/// Default implementation will create a VelocityContext object,
	/// put the HttpServletRequest and HttpServletResponse
	/// into the context accessable via the keys VelocityServlet.REQUEST and
	/// VelocityServlet.RESPONSE, respectively.
	/// </summary>
	/// <param name="context">HttpContext for the current request</param>
	/// <returns>a Velocity context</returns>
	protected virtual IContext CreateContext(HttpContext context) {
	    // create a new context
	    VelocityContext ctx = new VelocityContext();

	    // put the HttpContext/Request/Response objects into the context
	    ctx.Put(CONTEXT, context);
	    ctx.Put(REQUEST,  context.Request);
	    ctx.Put(RESPONSE, context.Response);

	    return ctx;
	}


	/// <summary>
	/// Retrieves the requested template.
	/// </summary>
	/// <param name="name">The file name of the template to retrieve relative to the template root.</param>
	/// <returns>teh requested template</returns>
	public virtual Template GetTemplate(String name) {
	    return RuntimeSingleton.getTemplate(name);
	}


	/// <summary>
	/// Retrieves the requested template with the specified
	/// character encoding.
	/// </summary>
	/// <param name="name">The file name of the template to retrieve relative to the template root.</param>
	/// <param name="encoding">encoding the character encoding of the template</param>
	/// <returns>The requested template.</returns>
	public virtual Template GetTemplate(String name, String encoding) {
	    return RuntimeSingleton.getTemplate(name, encoding);
	}


	/// <summary>
	/// Implement this method to add your application data to the context,
	/// calling the <code>getTemplate()</code> method to produce your return
	/// value.
	/// <br><br>
	/// In the event of a problem, you may handle the request directly
	/// and return <code>null</code> or throw a more meaningful exception
	/// for the error handler to catch.
	/// </summary>
	/// <param name="context">HttpContext for current request</param>
	/// <param name="ctx">velocity context</param>
	/// <returns>The template to merge with your context or null, indicating that you handled the processing.</returns>
	protected virtual Template HandleRequest(HttpContext context, IContext ctx) {
	    throw new System.Exception ("You must override NVelocityHandler.HandleRequest(HttpContext, IContext)" );
	}


	/// <summary>
	/// Invoked when there is an error thrown in any part of doRequest() processing.
	/// <br><br>
	/// Default will send a simple HTML response indicating there was a problem.
	/// </summary>
	/// <param name="context">original HttpContext from container.</param>
	/// <param name="cause">Exception that was thrown by some other part of process.</param>
	protected virtual void Error(HttpContext context, System.Exception cause ) {
	    StringBuilder html = new StringBuilder();
	    html.Append("<html>");
	    html.Append("<body bgcolor=\"#ffffff\">");
	    html.Append("<h2>VelocityServlet : Error processing the template</h2>");
	    html.Append("<pre>");
	    html.Append( cause );
	    html.Append("<br>");

	    html.Append(cause.StackTrace);

	    html.Append("</pre>");
	    html.Append("</body>");
	    html.Append("</html>");
	    context.Response.Write(html.ToString());
	}


	/// <summary>
	/// Implement the IHttpHandler interface method, return true because the class is poolable
	/// </summary>
	public virtual bool IsReusable {
	    get {
		return true;
	    }
	}


    }
}
