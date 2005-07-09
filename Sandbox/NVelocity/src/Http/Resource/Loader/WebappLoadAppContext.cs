using System;
using System.Web;

namespace NVelocity.Http.Resource.Loader {

    /// <summary>
    /// Wrapper class to safely pass the http context to the web app loader.
    /// </summary>
    public class WebappLoaderAppContext : IWebappLoaderAppContext {

	/// <summary>
	/// A reference to the http context
	/// </summary>
	private HttpContext httpContext = null;

	/// <summary>
	/// Default constructor.
	/// </summary>
	public WebappLoaderAppContext(HttpContext context) {
	    httpContext = context;
	}

	/// <summary>
	/// Returns a reference to the http context.
	/// </summary>
	public virtual HttpContext HttpContext {
	    get {
		return httpContext;
	    }
	}

    }
}
