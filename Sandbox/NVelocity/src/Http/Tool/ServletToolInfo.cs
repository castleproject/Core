using System;

namespace NVelocity.Http.Tool {

    /// <summary>
    /// ToolInfo implementation that holds scope information for tools
    /// used in a servlet environment.
    /// </summary>
    /// <author> <a href="mailto:nathan@esha.com">Nathan Bubna</a></author>
    public class ServletToolInfo : ViewToolInfo {

	public const String REQUEST_SCOPE = "request";
	public const String SESSION_SCOPE = "session";
	public const String APPLICATION_SCOPE = "application";

	private String scope;


	/// <summary>
	/// Creates a new tool of the specified class with the given key and scope.
	/// </summary>
	public ServletToolInfo(String key, String classname, String scope):base(key, classname) {
	    this.scope = scope;
	}


	/// <returns>
	/// the scope of the tool
	/// </returns>
	public virtual String Scope {
	    get {
		return scope;
	    }
	}


    }
}
