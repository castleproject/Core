using System;
using NVelocity.Tool;

namespace NVelocity.Http.Tool {

    /// <summary>
    /// ToolInfo implementation for view tools. New instances
    /// are returned for every call to getInstance(obj), and tools
    /// that implement (@link ViewTool} are initialized with the
    /// given object before being returned.
    /// </summary>
    /// <author> <a href="mailto:nathan@esha.com">Nathan Bubna</a></author>
    public class ViewToolInfo : IToolInfo {

	private String key;
	private Type clazz;
	private bool initializable = false;


	/// <summary>
	/// Constructor.  If an instance of the tool cannot be created from
	/// the classname, it will throw an exception.
	/// </summary>
	/// <param name="key">the context key for the tool</param>
	/// <param name="classname">the fully qualified java.lang.Class of the tool</param>
	public ViewToolInfo(String key, String classname) {
	    this.key = key;
	    this.clazz = Type.GetType(classname);

	    //create an instance and see if it is initializable
	    Object tool = Activator.CreateInstance(clazz);
	    if (tool is IViewTool) {
		this.initializable = true;
	    }
	}


	public virtual String Key {
	    get {
		return key;
	    }
	}

	public virtual String Classname {
	    get {
		return clazz.FullName;
	    }
	}


	/// <summary>
	/// Returns a new instance of the tool. If the tool
	/// implements {@link ViewTool}, the new instance
	/// will be initialized using the given data.
	/// </summary>
	public virtual Object getInstance(Object initData) {
	    Object tool = null;
	    try {
		tool = Activator.CreateInstance(clazz);
	    } catch (System.Exception) {
		//we should never get here since we
		//got a new instance just fine when we
		//created this tool info
	    }

	    if (initializable) {
		((IViewTool)tool).Init(initData);
	    }
	    return tool;
	}


    }
}
