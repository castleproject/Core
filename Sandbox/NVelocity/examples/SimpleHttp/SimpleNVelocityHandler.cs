using System;
using System.IO;
using System.Web;

using Commons.Collections;

using NVelocity;
using NVelocity.Context;
using NVelocity.Http;

namespace SimpleHttp {

    /// <summary>
    /// Simple handler that show how you can load a template from the same path as the ApplicationPath
    /// </summary>
    public class SimpleNVelocityHandler : NVelocityHandler {

	public SimpleNVelocityHandler() {}

	/// <summary>
	/// Override HandleRequest method so that what is put in the context and what template
	/// is loaded can be determined by this application.
	/// </summary>
	/// <param name="request"></param>
	/// <param name="response"></param>
	/// <param name="ctx"></param>
	/// <returns></returns>
	protected override Template HandleRequest(HttpContext context, IContext ctx ) {

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
	/// overridden LoadConfiguration that will create a properties file on the fly
	/// </summary>
	/// <returns></returns>
	protected override ExtendedProperties LoadConfiguration() {
	    ExtendedProperties p = new ExtendedProperties();

	    // override the loader path and log file locations based on where the physical application path is
	    p.SetProperty(NVelocity.Runtime.RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, context.Request.PhysicalApplicationPath);
	    p.SetProperty(NVelocity.Runtime.RuntimeConstants_Fields.RUNTIME_LOG,
			  context.Request.PhysicalApplicationPath + p.GetString(NVelocity.Runtime.RuntimeConstants_Fields.RUNTIME_LOG, "nvelocity.log"));

	    return p;
	}

    }
}
