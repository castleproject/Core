// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Core.Logging
{
	#if !SILVERLIGHT

	using System;
	using System.Diagnostics;
	using System.Globalization;

	/// <summary>
	/// The Logger using standart Diagnostics namespace.
	/// </summary>
	[Serializable]
	public class DiagnosticsLogger : LevelFilteredLogger, IDisposable
	{
		[NonSerialized]
		private EventLog eventLog;

		/// <summary>
		/// Creates a logger based on <see cref="System.Diagnostics.EventLog"/>.
		/// </summary>
		/// <param name="logName"><see cref="EventLog.Log"/></param>
		public DiagnosticsLogger(string logName) : this(logName, "default")
		{
		}

		/// <summary>
		/// Creates a logger based on <see cref="System.Diagnostics.EventLog"/>.
		/// </summary>
		/// <param name="logName"><see cref="EventLog.Log"/></param>
		/// <param name="source"><see cref="EventLog.Source"/></param>
		public DiagnosticsLogger(string logName, string source) : base(LoggerLevel.Debug)
		{
			// Create the source, if it does not already exist.
			if (!EventLog.SourceExists(source))
			{
				EventLog.CreateEventSource(source, logName);
			}

			eventLog = new EventLog(logName);
			eventLog.Source = source;
		}

		/// <summary>
		/// Creates a logger based on <see cref="System.Diagnostics.EventLog"/>.
		/// </summary>
		/// <param name="logName"><see cref="EventLog.Log"/></param>
		/// <param name="machineName"><see cref="EventLog.MachineName"/></param>
		/// <param name="source"><see cref="EventLog.Source"/></param>
		public DiagnosticsLogger(string logName, string machineName, string source)
		{
			// Create the source, if it does not already exist.
			if (!EventLog.SourceExists(source, machineName))
			{
				EventSourceCreationData eventSourceCreationData = new EventSourceCreationData(source, logName);
				eventSourceCreationData.MachineName = machineName;
				EventLog.CreateEventSource(eventSourceCreationData);
			}

			eventLog = new EventLog(logName, machineName, source);
		}

		~DiagnosticsLogger()
		{
			Dispose(false);
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if( disposing )
			{
				if (eventLog != null)
				{
					eventLog.Close();
					eventLog = null;
				}
			}
		}

		public override ILogger CreateChildLogger(string loggerName)
		{
			return new DiagnosticsLogger(eventLog.Log, eventLog.MachineName, eventLog.Source);
		}

		protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
		{
			if (eventLog == null) return; // just in case it was disposed

			EventLogEntryType type = TranslateLevel(loggerLevel);

			String contentToLog;

			if (exception == null)
			{
				contentToLog = string.Format(CultureInfo.CurrentCulture, "[{0}] '{1}' message: {2}", loggerLevel.ToString(), loggerName, message);
			}
			else
			{
				contentToLog = string.Format(CultureInfo.CurrentCulture, "[{0}] '{1}' message: {2} exception: {3} {4} {5}",
				                             loggerLevel.ToString(), loggerName, message, exception.GetType(), exception.Message,
				                             exception.StackTrace);
			}

			eventLog.WriteEntry(contentToLog, type);
		}

		private static EventLogEntryType TranslateLevel(LoggerLevel level)
		{
			switch(level)
			{
				case LoggerLevel.Error:
				case LoggerLevel.Fatal:
					return EventLogEntryType.Error;
				case LoggerLevel.Warn:
					return EventLogEntryType.Warning;
				default:
					return EventLogEntryType.Information;
			}
		}
	}
	#endif
}
