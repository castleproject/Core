using System;
using System.Collections;

using System.Diagnostics;

namespace NVelocity.Runtime.Log {

    /// <summary>
    /// Pre-init logger.  I believe that this was suggested by
    /// Carsten Ziegeler <cziegeler@sundn.de> and
    /// Jeroen C. van Gelderen.  If this isn't correct, let me
    /// know as this was a good idea...
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    public class PrimordialLogSystem : LogSystem {

	private ArrayList pendingMessages = new ArrayList();
	private RuntimeServices rsvc = null;

	private DefaultTraceListener debug = new DefaultTraceListener();

	/// <summary>
	/// default CTOR.
	/// </summary>
	public PrimordialLogSystem() {}

	public virtual void Init(RuntimeServices rs) {
	    rsvc = rs;
	}

	/// <summary>
	/// logs messages.  All we do is store them until 'later'.
	/// </summary>
	/// <param name="level">severity level</param>
	/// <param name="message">complete error message</param>
	public virtual void LogVelocityMessage(int level, System.String message) {
	    lock(this) {
		System.Object[] data = new System.Object[2];
		data[0] = level;
		data[1] = message;
		pendingMessages.Add(data);

		// log the the OutputDebugPrint API (see www.sysinternals.com for DebugView to see debug messages)
		debug.WriteLine("PrimordialLogSystem: " + level.ToString() + " - " + message);
	    }
	}

	/// <summary>
	/// dumps the log messages this logger is holding into a new logger
	/// </summary>
	public virtual void DumpLogMessages(LogSystem newLogger) {
	    lock(this) {
		if (!(pendingMessages.Count == 0)) {
		    // iterate and log each individual message...
		    foreach(Object[] data in pendingMessages) {
			newLogger.LogVelocityMessage(((Int32)data[0]), (String)data[1]);
		    }
		}
	    }
	}


    }
}
