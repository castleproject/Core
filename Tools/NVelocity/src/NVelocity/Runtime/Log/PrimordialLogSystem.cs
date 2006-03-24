namespace NVelocity.Runtime.Log
{
	using System;
	using System.Collections;
	using System.Diagnostics;

	/// <summary>
	/// Pre-init logger.  I believe that this was suggested by
	/// Carsten Ziegeler <cziegeler@sundn.de> and
	/// Jeroen C. van Gelderen.  If this isn't correct, let me
	/// know as this was a good idea...
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	public class PrimordialLogSystem : ILogSystem
	{
		private ArrayList pendingMessages = new ArrayList();
		private IRuntimeServices rsvc = null;

		private DefaultTraceListener debug = new DefaultTraceListener();

		/// <summary>
		/// default CTOR.
		/// </summary>
		public PrimordialLogSystem()
		{
		}

		public void Init(IRuntimeServices rs)
		{
			this.rsvc = rs;
		}

		/// <summary>
		/// logs messages.  All we do is store them until 'later'.
		/// </summary>
		/// <param name="level">severity level</param>
		/// <param name="message">complete error message</param>
		public void LogVelocityMessage(LogLevel level, String message)
		{
			lock (this)
			{
				Object[] data = { level, message };
				pendingMessages.Add(data);

				// log the the OutputDebugPrint API (see www.sysinternals.com for DebugView to see debug messages)
				debug.WriteLine("PrimordialLogSystem: " + level.ToString() + " - " + message);
			}
		}

		/// <summary>
		/// dumps the log messages this logger is holding into a new logger
		/// </summary>
		public void DumpLogMessages(ILogSystem newLogger)
		{
			lock (this)
			{
				if (!(pendingMessages.Count == 0))
				{
					// iterate and log each individual message...
					foreach (Object[] data in pendingMessages)
					{
						newLogger.LogVelocityMessage(((LogLevel) data[0]), (String) data[1]);
					}
				}

				pendingMessages.Clear();
			}
		}
	}
}
