// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Services.Logging
{
	using System;
	using System.Diagnostics; 

	/// <summary>
	/// The Logger using standart Diagnostics namespace.
	/// </summary>
	public class DiagnosticsLogger : ILogger
	{
		private EventLog _logger;

		/// <summary>
		/// Creates a logger based on <see cref="System.Diagnostics.EventLog"/>.
		/// </summary>
		/// <param name="logName"><see cref="EventLog.Log"/></param>
		public DiagnosticsLogger(string logName)
		{
			_logger = new EventLog(logName);
		}

		/// <summary>
		/// Creates a logger based on <see cref="System.Diagnostics.EventLog"/>.
		/// </summary>
		/// <param name="logName"><see cref="EventLog.Log"/></param>
		/// <param name="source"><see cref="EventLog.Source"/></param>
		public DiagnosticsLogger(string logName, string source)
		{
			// Create the source, if it does not already exist.
			if(!EventLog.SourceExists(source))
			{
				EventLog.CreateEventSource(source, logName);
			}

			_logger = new EventLog(logName); 
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
			if(!EventLog.SourceExists(source, machineName))
			{
				EventLog.CreateEventSource(source, logName, machineName);
			}
  			
			_logger = new EventLog(logName, machineName, source);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The Message</param>
		public void Debug(string message )
		{
			Debug(message, null as Exception);
		}

		/// <summary>
		/// Logs a debug message. 
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		public void Debug(string message, Exception exception)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("message: {0}", message));
			
			if (exception != null)
			{
				System.Diagnostics.Debug.WriteLine(string.Format("exception: {0}", exception));
			}
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Debug( string format, params Object[] args )
		{
			Debug(String.Format(format, args), null as Exception);
		}

		/// <summary>
		/// Debug level is always enabled.
		/// </summary>
		/// <value>The Value is always True</value> 
		public bool IsDebugEnabled
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="message">The Message</param>
		public void Info(string message )
		{
			Info(message, null as Exception);
		}

		/// <summary>
		/// Logs an info message. 
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		public void Info(string message, Exception exception)
		{
			Log(message, exception, EventLogEntryType.Information);  
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Info( string format, params Object[] args )
		{
			Info(String.Format(format, args));
		}

		/// <summary>
		/// Information level is always enabled.
		/// </summary>
		/// <value>The Value is always True</value> 
		public bool IsInfoEnabled
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="message">The Message</param>
		public void Warn(string message )
		{
			Warn(message, null as Exception);
		}

		/// <summary>
		/// Logs a warn message. 
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		public void Warn(string message, Exception exception)
		{
			Log(message, exception, EventLogEntryType.Warning); 
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Warn( string format, params Object[] args )
		{
			Warn(String.Format(format, args));
		}

		/// <summary>
		/// Warning level is always enabled.
		/// </summary>
		/// <value>The Value is always True</value> 
		public bool IsWarnEnabled
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The Message</param>
		public void Error(string message )
		{
			Error(message, null as Exception);
		}

		/// <summary>
		/// Logs an error message. 
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		public void Error(string message, Exception exception)
		{
			Log(message, exception, EventLogEntryType.Error);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Error( string format, params Object[] args )
		{
			Error(String.Format(format, args));
		}

		/// <summary>
		/// Error level is always enabled.
		/// </summary>
		/// <value>The Value is always True</value> 
		public bool IsErrorEnabled
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="message">The Message</param>
		public void FatalError(string message )
		{
			FatalError(message, null as Exception);
		}

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		public void FatalError(string message, Exception exception)
		{
			Log(message, exception, EventLogEntryType.Error);
		}

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void FatalError( string format, params Object[] args )
		{
			FatalError(String.Format(format, args));
		}

		/// <summary>
		/// FatalError level is always enabled.
		/// </summary>
		/// <value>The Value is always True</value> 
		public bool IsFatalErrorEnabled
		{
			get 
			{
				return true; 
			}
		}

		/// <summary>
		/// Create a new logger with the same Log, MachineName and Source properties.
		/// </summary>
		/// <param name="name">Ignored, cause a source can only be registered to one log at a time.</param>
		/// <returns>The New ILogger instance.</returns> 
		/// <exception cref="System.ArgumentException">If the name has an empty element name.</exception>
		public ILogger CreateChildLogger(string name )
		{
			return new DiagnosticsLogger(_logger.Log, _logger.MachineName, _logger.Source);  
		}

		private void Log(string message, Exception exception, EventLogEntryType type)
		{
			string result;

			if (exception == null)
			{
				result = string.Format("message: {0}", message);
			}
			else
			{
				result = string.Format("message: {0}; exception: {1}", message, exception);
			}
			
			_logger.WriteEntry(result, type);
		}
	}
}