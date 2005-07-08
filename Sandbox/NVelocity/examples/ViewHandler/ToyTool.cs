using System;

/// <summary>
/// Simple example of a tool
/// </summary>
public class ToyTool {

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public virtual String Message {
	get {
	    log.Debug("returning message from ToyTool");
	    return "Hello from ToyTool!";
	}
    }

}
