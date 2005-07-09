using System;

namespace NVelocity.Http.Tool {

    /// <summary>
    /// Generic view tool interface to assist in tool management.
    /// This interface provides the {@link #init(Object initData)} method
    /// as a hook for ToolboxManager implementations to pass data in to
    /// tools to initialize them.  See
    /// {@link org.apache.velocity.tools.view.ViewToolInfo} for more on this.
    /// </summary>
    /// <author> <a href="mailto:nathan@esha.com">Nathan Bubna</a></author>
    public interface IViewTool {

	/// <summary>
	/// Initializes this instance using the given data
	/// </summary>
	/// <param name="initData">the initialization data</param>
	void Init(Object initData);


    }
}
