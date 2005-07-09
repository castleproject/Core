using System;

namespace NVelocity.Runtime {
	
    /// <summary>
    /// Interface for internal runtime logging services that are needed by the
    /// </summary>
    /// <author><a href="mailto:geirm@apache.org">Geir Magusson Jr.</a></author>
    /// <version>$Id: RuntimeLogger.cs,v 1.1 2004/01/02 00:04:50 corts Exp $</version>
    public interface RuntimeLogger {
	/// <summary>
	/// Log a warning message.
	/// </summary>
	/// <param name="Object">message to log</param>
	void  warn(System.Object message);

	/// <summary>
	/// Log an info message.
	/// </summary>
	/// <param name="Object">message to log</param>
	void  info(System.Object message);

	/// <summary>
	/// Log an error message.
	/// </summary>
	/// <param name="Object">message to log</param>
	void  error(System.Object message);

	/// <summary>
	/// Log a debug message.
	/// </summary>
	/// <param name="Object">message to log</param>
	void  debug(System.Object message);
    }
}