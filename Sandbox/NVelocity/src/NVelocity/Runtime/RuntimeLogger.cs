namespace NVelocity.Runtime
{
	using System;

	/// <summary>
	/// Interface for internal runtime logging services that are needed by the
	/// </summary>
	/// <author><a href="mailto:geirm@apache.org">Geir Magusson Jr.</a></author>
	/// <version>$Id: RuntimeLogger.cs,v 1.1 2004/01/02 00:04:50 corts Exp $</version>
	public interface RuntimeLogger
	{
		/// <summary>
		/// Log a warning message.
		/// </summary>
		/// <param name="Object">message to log</param>
		void warn(Object message);

		/// <summary>
		/// Log an info message.
		/// </summary>
		/// <param name="Object">message to log</param>
		void info(Object message);

		/// <summary>
		/// Log an error message.
		/// </summary>
		/// <param name="Object">message to log</param>
		void error(Object message);

		/// <summary>
		/// Log a debug message.
		/// </summary>
		/// <param name="Object">message to log</param>
		void debug(Object message);
	}
}