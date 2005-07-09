using System;
using System.Collections;
using NVelocity;

namespace NVelocity.Http.Context {


    /// <summary>
    /// <p>Read-only context used to carry a set of view tools.</p>
    /// <p>Writes get dropped.</p>
    /// </summary>
    /// <author> <a href="mailto:sidler@apache.org">Gabriel Sidler</a></author>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    public class ToolboxContext : VelocityContext {
	/// <summary>
	/// The collection of view tools in this toolbox.
	/// </summary>
	private Hashtable toolbox;


	/// <summary>
	/// Default constructor.
	/// </summary>
	public ToolboxContext(Hashtable tb) {
	    toolbox = tb;
	}


	/// <summary>
	/// Get value for key.
	/// </summary>
	public override System.Object InternalGet(System.String key) {
	    return toolbox[key];
	}


	/// <summary>
	/// Does nothing. Returns <code>null</code> always.
	/// </summary>
	public override System.Object InternalPut(System.String key, System.Object value_Renamed) {
	    return null;
	}
    }
}
