namespace NVelocity.Runtime.Log
{
	using System;

	/// <summary>  Logger used in case of failure. Does nothing.
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: NullLogSystem.cs,v 1.4 2003/10/27 13:54:10 corts Exp $
	///
	/// </version>
	public class NullLogSystem : ILogSystem
	{
		public NullLogSystem()
		{
		}

		public void Init(IRuntimeServices rs)
		{
		}

		/// <summary>
		/// logs messages to the great Garbage Collector in the sky
		/// </summary>
		/// <param name="level">severity level</param>
		/// <param name="message">complete error message</param>
		public void LogVelocityMessage(LogLevel level, String message)
		{
		}
	}
}
