using System;
using System.Collections;
using System.IO;
using System.Web;

using Commons.Collections;

using NVelocity.App;
using NVelocity.Exception;
using NVelocity.Runtime.Resource;
using NVelocity.Runtime.Resource.Loader;

namespace NVelocity.Http.Resource.Loader {

    /// <summary>
    /// a resource loader for use with web applications, sets up the path list with the physical application path.
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    public class WebappLoader : FileResourceLoader {
	protected internal HttpContext httpContext = null;

	/// <summary>
	/// This is abstract in the base class, so we need it
	/// </summary>
	public override void init(ExtendedProperties configuration) {
	    rsvc.info("WebappLoader : initialization starting.");

	    System.Object o = rsvc.getApplicationAttribute("NVelocity.Http.Resource.Loader.WebappLoader");
	    if (o is IWebappLoaderAppContext) {
		httpContext = ((IWebappLoaderAppContext)o).HttpContext;
	    } else {
		rsvc.error("WebappLoader : unable to retrieve NVelocity.Http.Resource.Loader.WebappLoader from ApplicationAttributes, attempting to use HttpContext.Current");
		httpContext = HttpContext.Current;
	    }

	    paths = new ArrayList();
	    paths.Add(httpContext.Request.PhysicalApplicationPath);
	    rsvc.info("WebappLoader : using path '" + httpContext.Request.PhysicalApplicationPath + "'");

	    rsvc.info("WebappLoader : initialization complete.");
	}

    }
}
