using System;
using System.IO;
using System.Web;
using System.Web.SessionState;

using Commons.Collections;

using NVelocity.App;
using NVelocity.Context;
using NVelocity.Http;
using NVelocity.Runtime;
using NVelocity.Http.Context;
using NVelocity.Http.Resource.Loader;
using NVelocity.Http.Tool;

namespace NVelocity.Http {

    /// <summary>
    /// <p>A servlet to process Velocity templates. This is comparable to the
    /// the JspServlet for JSP-based applications.</p>
    /// <p>The servlet provides the following features:</p>
    /// <ul>
    /// <li>renders Velocity templates</li>
    /// <li>provides support for an auto-loaded, configurable toolbox</li>
    /// <li>provides transparent access to the servlet request attributes,
    /// servlet session attributes and servlet context attributes by
    /// auto-searching them</li>
    /// <li>logs to the logging facility of the servlet API</li>
    /// </ul>
    /// <p>VelocityViewServlet supports the following configuration parameters
    /// in webl.xml:</p>
    /// <dl>
    /// <dt>toolbox</dt>
    /// <dd>Path and name of the toolbox configuration file. The path must be
    /// relative to the web application root directory. If this parameter is
    /// not found, no toolbox is instantiated.</dd>
    /// <dt>org.apache.velocity.properties</dt>
    /// <dd>Path and name of the Velocity configuration file. The path must be
    /// relative to the web application root directory. If this parameter
    /// is not present, Velocity is initialized with default settings.</dd>
    /// </dl>
    /// </summary>
    /// <author> <a href="mailto:sidler@teamup.com">Gabe Sidler</a></author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    public class NVelocityViewHandler : NVelocityHandler {

	/// <summary>
	/// <p>Initializes servlet, toolbox and Velocity template engine.</p>
	/// </summary>
	/// <param name="config">servlet configuation
	/// </param>
	public override void Init(HttpContext context, String requestType, String url, String pathTranslated) {
	    base.Init(context, requestType, url, pathTranslated);
	    LoadToolbox();
	}


	/// <summary>
	/// Initializes Velocity.
	/// </summary>
	/// <param name="config">servlet configuration parameters
	/// </param>
	protected override void InitVelocity() {
	    // Try reading Velocity configuration
	    try {
		ExtendedProperties p = base.LoadConfiguration();
		Velocity.ExtendedProperties = ExtendedProperties.ConvertProperties(p);
	    } catch (System.Exception e) {
		Velocity.Error("Unable to read Velocity configuration file: " + e);
		Velocity.Info("Using default Velocity configuration.");
	    }

	    // load resources with webapp resource loader
	    WebappLoaderAppContext wlac = new WebappLoaderAppContext(this.context);
	    Velocity.SetApplicationAttribute("NVelocity.Http.Resource.Loader.WebappLoader", wlac);
	    Velocity.SetProperty("resource.loader", "webapp");
	    Velocity.SetProperty("webapp.resource.loader.class", @"NVelocity.Http.Resource.Loader.WebappLoader\,NVelocity.Http");

	    // now all is ready - init Velocity
	    try {
		Velocity.Init();
	    } catch (System.Exception e) {
		Velocity.Error("VELOCITY PANIC : unable to init() : " + e);
		throw new HttpException("VELOCITY PANIC : unable to init()", e);
	    }
	}


	/// <summary>
	/// <p>Handle the template processing request.</p>
	/// </summary>
	/// <param name="request">client request</param>
	/// <param name="response">client response</param>
	/// <param name="ctx"> VelocityContext to fill</param>
	/// <returns>Velocity Template object or null</returns>
	protected override Template HandleRequest(HttpContext context, IContext ctx) {
	    // invoke handleRequest
	    // request.Path holds the entire path to the file from the root of the web (i.e. /nvelocity/examples/simplehttp/foo/bar/template.vm)
	    // request.ApplicationPath should hold the path to the root of this application (i.e. /nvelocity/examples/simplehttp)
	    // the difference between the (changeing case to lower as they may not be the same) is the path from the root to the file (i.e. /foo/bar/template.vm)
	    String template = context.Request.Path;
	    if (template.ToLower().StartsWith(context.Request.ApplicationPath.ToLower())) {
		template = template.Substring(context.Request.ApplicationPath.Length)
		;
	    }

	    ctx.Put("page", context.Request.Path);
	    ctx.Put("template", template)
	    ;

	    Template t = GetTemplate(template)
	    ;

	    if (t == null) {
		throw new System.Exception ("HandleRequest(HttpContext, IContext) returned null - no template selected!" );
	    }

	    return t;
	}


	/// <summary>
	/// <p>Creates and returns an initialized Velocity context.</p>
	/// A new context of class {@link ChainedContext} is created and
	/// initialized. This method overwrites
	/// {@link org.apache.velocity.servlet.VelocityServlet#createContext(HttpServletRequest request, HttpServletResponse response)}.</p>
	/// </summary>
	/// <param name="request">servlet request from client
	/// </param>
	/// <param name="response">servlet reponse to client
	/// </param>
	protected override IContext CreateContext(HttpContext context) {
	    // create a ChainedContext()
	    ChainedContext ctx = new ChainedContext(null, context);

	    // if we have a toolbox manager, let it make a context for us
	    if (toolboxManager != null) {
		ToolboxContext tc = toolboxManager.getToolboxContext(ctx);
		ctx.Toolbox = tc;
	    }

	    return ctx;
	}

    }
}
