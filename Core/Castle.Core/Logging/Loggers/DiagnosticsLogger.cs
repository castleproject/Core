// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System;
	using System.Diagnostics;

	/// <summary>
	/// The Logger using standart Diagnostics namespace.
	/// </summary>
	[Serializable]
	public class DiagnosticsLogger : LevelFilteredLogger, IDisposable
	{
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
				EventLog.CreateEventSource(source, logName, machineName);
			}

			eventLog = new EventLog(logName, machineName, source);
		}
		
		~DiagnosticsLogger()
		{
			Close(false);
		}

		#region IDisposable Members

		public void Dispose()
		{
			Close(true);
		}

		#endregion

		public void Close()
		{
			Close(true);
		}

		protected void Close(bool supressFinalize)
		{
			if (supressFinalize)
			{
				GC.SuppressFinalize(this);
			}

			if (eventLog != null)
			{
				eventLog.Close();
				eventLog = null;
			}
		}

		public override ILogger CreateChildLogger(string newName)
		{
			return new DiagnosticsLogger(eventLog.Log, eventLog.MachineName, eventLog.Source);
		}

		protected override void Log(LoggerLevel level, string name, string message, Exception exception)
		{
			if (eventLog == null) return; // just in case it was disposed
			
			EventLogEntryType type = TranslateLevel(level);
			
			String contentToLog;
			
			if (exception == null)
			{
				contentToLog = string.Format("[{0}] '{1}' message: {2}", level.ToString(), name, message);
			}
			else
			{
				contentToLog = string.Format("[{0}] '{1}' message: {2} exception: {3} {4} {5}", 
					level.ToString(), name, message, exception.GetType(), exception.Message, exception.StackTrace);
			}

			eventLog.WriteEntry(contentToLog, type);
		}

		private EventLogEntryType TranslateLevel(LoggerLevel level)
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
}