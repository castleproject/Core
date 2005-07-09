using System;

using NVelocity.App;
using NVelocity.Runtime;
using NVelocity.Runtime.Log;

namespace NVelocity.Runtime.Log {

    /// <summary>
    /// Simple wrapper for the servlet log
    /// </summary>
    /// <author> <a href="mailto:cort@xmission.com">Cort Schaefer</a></author>
    public class Log4NetExtensionLogSystem : LogSystem {

	private log4net.ILog log;

	/// <summary>
	/// Default constructor, fine when NVelocity will be called from the same app domain.
	/// </summary>
	public Log4NetExtensionLogSystem() {
	    log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}

	/// <summary>
	/// to be used when NVelocity will be called from the same app domain and you want to name the category used
	/// for NVelocity messages.
	/// </summary>
	/// <param name="category">category to use for log messages instead of the name of the actual class</param>
	public Log4NetExtensionLogSystem(String category) {
	    log = log4net.LogManager.GetLogger(category);
	}

	/// <summary>
	/// specify named category and domain that logging will participate with (needed when using NVelocity in environment
	/// where it will be hosted by external process; i.e. IIS or COM+).
	/// </summary>
	/// <param name="assembly"></param>
	/// <param name="category"></param>
	public Log4NetExtensionLogSystem(System.Reflection.Assembly assembly, String category) {
	    log = log4net.LogManager.GetLogger(assembly, category);
	}

	/// <summary>
	/// specify domain that logging will participate with (needed when using NVelocity in environment
	/// where it will be hosted by external process; i.e. IIS or COM+)
	/// </summary>
	/// <param name="assembly"></param>
	public Log4NetExtensionLogSystem(System.Reflection.Assembly assembly) {
	    log = log4net.LogManager.GetLogger(assembly, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}

	/// <summary>
	/// Init
	/// </summary>
	public virtual void Init(RuntimeServices rs) {}

	/// <summary>
	/// Send a log message from NVelocity.
	/// </summary>
	public virtual void LogVelocityMessage(int level, System.String message) {
	    switch (level) {
		case LogSystem_Fields.WARN_ID:
		    log.Warn(message);
		    break;
		case LogSystem_Fields.INFO_ID:
		    log.Info(message);
		    break;
		case LogSystem_Fields.DEBUG_ID:
		    log.Debug(message);
		    break;
		case LogSystem_Fields.ERROR_ID:
		    log.Error(message);
		    break;
		default:
		    log.Info(message);
		    break;
	    }
	}


    }
}
