using System;

namespace NVelocity.Runtime.Log {

    public struct LogSystem_Fields {
	public const bool DEBUG_ON = true;

	/// <summary>
	/// Prefix for debug messages.
	/// </summary>
	public const int DEBUG_ID = 0;

	/// <summary>
	/// Prefix for info messages.
	/// </summary>
	public const int INFO_ID = 1;

	/// <summary>
	/// Prefix for warning messages.
	/// </summary>
	public const int WARN_ID = 2;

	/// <summary>
	/// Prefix for error messages.
	/// </summary>
	public const int ERROR_ID = 3;
    }

    /// <summary>
    /// Base interface that Logging systems need to implement.
    /// </summary>
    /// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a></author>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    public interface LogSystem {

	/// <summary>
	/// init()
	/// </summary>
	void Init(RuntimeServices rs);

	/// <summary>
	/// Send a log message from Velocity.
	/// </summary>
	void LogVelocityMessage(int level, System.String message);
    }

}
