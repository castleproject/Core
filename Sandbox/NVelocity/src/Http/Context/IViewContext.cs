using System;
using System.Web;
using NVelocity.Context;


namespace NVelocity.Http.Context {

    /// <summary>
    /// <p>Objects implementing this interface are passed to view tools
    /// upon initialization by the
    /// {@link org.apache.velocity.tools.view.servlet.ServletToolboxManager}.</p>
    /// <p>The interface provides view tools in a servlet environment
    /// access to relevant context information, like servlet request, servlet
    /// context and the velocity context.</p>
    /// </summary>
    /// <author> <a href="mailto:sidler@teamup.com">Gabe Sidler</a></author>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    public interface IViewContext {

	/// <summary>
	/// Returns the instance of {@link HttpContext} for this request.
	/// </summary>
	HttpContext HttpContext {
	    get
		;
	    }

	    /// <summary>
	    /// Returns a reference to the current Velocity context.
	    /// </summary>
	    IContext VelocityContext {
		get
		    ;
		}


	    }
}
