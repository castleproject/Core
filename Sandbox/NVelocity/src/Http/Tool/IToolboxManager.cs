using System;
using NVelocity.Http.Context;
using NVelocity.Tool;

namespace NVelocity.Http.Tool {

    /// <summary>
    /// Common interface for toolbox manager implementations.
    /// </summary>
    /// <author> <a href="mailto:nathan@esha.com">Nathan Bubna</a></author>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    /// <author> <a href="mailto:sidler@teamup.com">Gabe Sidler</a></author>
    public interface IToolboxManager {

	/// <summary>
	/// Adds a tool to be managed
	/// </summary>
	void AddTool(IToolInfo info);

	/// <summary>
	/// Creates a {@link ToolboxContext} from the tools and data
	/// in this manager.  Tools that implement the ViewTool
	/// interface should be initialized using the given initData.
	/// </summary>
	/// <param name="initData">data used to initialize ViewTools</param>
	/// <returns>the created ToolboxContext</returns>
	ToolboxContext getToolboxContext(System.Object initData);

    }
}
