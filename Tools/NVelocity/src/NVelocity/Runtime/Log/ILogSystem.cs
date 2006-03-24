namespace NVelocity.Runtime.Log
{
	using System;

	public enum LogLevel
	{
		/// <summary>
		/// Prefix for debug messages.
		/// </summary>
		Debug = 0,

		/// <summary>
		/// Prefix for info messages.
		/// </summary>
		Info = 1,

		/// <summary>
		/// Prefix for warning messages.
		/// </summary>
		Warn = 2,

		/// <summary>
		/// Prefix for error messages.
		/// </summary>
		Error = 3,
	}

	/// <summary>
	/// Base interface that Logging systems need to implement.
	/// </summary>
	/// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a></author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	public interface ILogSystem
	{
		/// <summary>
		/// init()
		/// </summary>
		void Init(IRuntimeServices rs);

		/// <summary>
		/// Send a log message from Velocity.
		/// </summary>
		void LogVelocityMessage(LogLevel level, String message);
	}

}