using System;
using System.Web;

namespace NVelocity.Http {

    /// <summary>
    /// Factory class that creates a handler object based on a request
    /// for either abc.aspx or xyz.aspx as specified in the Web.config file.
    /// </summary>
    public class NVelocityHandlerFactory : IHttpHandlerFactory {

	/// <summary>
	/// IHttpHandlerFactory method to
	/// </summary>
	/// <param name="context"></param>
	/// <param name="requestType"></param>
	/// <param name="url"></param>
	/// <param name="pathTranslated"></param>
	/// <returns></returns>
	public virtual IHttpHandler GetHandler(HttpContext context, String requestType, String url, String pathTranslated) {
	    String className = System.Configuration.ConfigurationSettings.AppSettings["nvelocity.httphandler.class"];

	    Object o = null;
	    NVelocityHandler h = null;

	    // Try to create the handler object.
	    try {
		o = Activator.CreateInstance(Type.GetType(className));
		if (!(o is NVelocityHandler) || !(o is IHttpHandler)) {
		    throw new HttpException("Factory couldn't create instance of type NVelocityHandler");
		}
		h = (NVelocityHandler)o;
		h.Init(context, requestType, url, pathTranslated);
	    } catch(System.Exception e) {
		throw new HttpException("Factory couldn't create instance of type " + className, e);
	    }
	    return (IHttpHandler)h;
	}

	/// <summary>
	/// IHttpHandlerFactory must override method
	/// </summary>
	/// <param name="handler"></param>
	public virtual void ReleaseHandler(IHttpHandler handler) {}}
}
