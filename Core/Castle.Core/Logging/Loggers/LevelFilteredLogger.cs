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
	using System;
	using System.Globalization;
	using System.Security.Permissions;

	/// <summary>
	///	The Level Filtered Logger class.  This is a base clase which
	///	provides a LogLevel attribute and reroutes all functions into
	///	one Log method.
	/// </summary>
#if SILVERLIGHT
	public abstract class LevelFilteredLogger : ILogger
#else  
	[Serializable]
	public abstract class LevelFilteredLogger : MarshalByRefObject, ILogger
#endif
	{
		private LoggerLevel level = LoggerLevel.Off;
		private String name = "unnamed";

		/// <summary>
		/// Creates a new <c>LevelFilteredLogger</c>.
		/// </summary>
		protected LevelFilteredLogger()
		{
		}

		protected LevelFilteredLogger(String name)
		{
			ChangeName(name);
		}

		protected LevelFilteredLogger(LoggerLevel loggerLevel)
		{
			this.level = loggerLevel;
		}

		protected LevelFilteredLogger(String loggerName, LoggerLevel loggerLevel) : this(loggerLevel)
		{
			ChangeName(loggerName);
		}

#if !SILVERLIGHT
        /// <summary>
		/// Keep the instance alive in a remoting scenario
		/// </summary>
		/// <returns></returns>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
		public override object InitializeLifetimeService()
		{
			return null;
		}
#endif

		public abstract ILogger CreateChildLogger(string loggerName);

		/// <value>
		/// The <c>LoggerLevel</c> that this logger
		/// will be using. Defaults to <c>LoggerLevel.Off</c>
		/// </value>
		public LoggerLevel Level
		{
			get { return level; }
			set { level = value; }
		}

		/// <value>
		/// The name that this logger will be using. 
		/// Defaults to <c>String.Empty</c>
		/// </value>
		public String Name
		{
			get { return name; }
		}

		#region ILogger implementation

		#region Debug

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Debug(string message)
		{
			if (!IsDebugEnabled) return;

			Log(LoggerLevel.Debug, message, null);
		}

		/// <summary>
		/// Logs a debug message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Debug(string message, Exception exception)
		{
			if (!IsDebugEnabled) return;

			Log(LoggerLevel.Debug, message, exception);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void DebugFormat(string format, params object[] args)
		{
			if (!IsDebugEnabled) return;

			Log(LoggerLevel.Debug, String.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void DebugFormat(Exception exception, string format, params object[] args)
		{
			if (!IsDebugEnabled) return;

			Log(LoggerLevel.Debug, String.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsDebugEnabled) return;

			Log(LoggerLevel.Debug, String.Format(formatProvider, format, args), null);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsDebugEnabled) return;

			Log(LoggerLevel.Debug, String.Format(formatProvider, format, args), exception);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Debug(string format, params Object[] args)
		{
			if (!IsDebugEnabled) return;

			Log(LoggerLevel.Debug, String.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		#endregion

		#region Info

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Info(string message)
		{
			if (!IsInfoEnabled) return;

			Log(LoggerLevel.Info, message, null);
		}

		/// <summary>
		/// Logs an info message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Info(string message, Exception exception)
		{
			if (!IsInfoEnabled) return;

			Log(LoggerLevel.Info, message, exception);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void InfoFormat(string format, params object[] args)
		{
			if (!IsInfoEnabled) return;

			Log(LoggerLevel.Info, String.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void InfoFormat(Exception exception, string format, params object[] args)
		{
			if (!IsInfoEnabled) return;

			Log(LoggerLevel.Info, String.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsInfoEnabled) return;

			Log(LoggerLevel.Info, String.Format(formatProvider, format, args), null);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsInfoEnabled) return;

			Log(LoggerLevel.Info, String.Format(formatProvider, format, args), exception);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Info(string format, params Object[] args)
		{
			if (!IsInfoEnabled) return;

			Log(LoggerLevel.Info, String.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		#endregion

		#region Warn

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Warn(string message)
		{
			if (!IsWarnEnabled) return;

			Log(LoggerLevel.Warn, message, null);
		}

		/// <summary>
		/// Logs a warn message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Warn(string message, Exception exception)
		{
			if (!IsWarnEnabled) return;

			Log(LoggerLevel.Warn, message, exception);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void WarnFormat(string format, params object[] args)
		{
			if (!IsWarnEnabled) return;

			Log(LoggerLevel.Warn, String.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void WarnFormat(Exception exception, string format, params object[] args)
		{
			if (!IsWarnEnabled) return;

			Log(LoggerLevel.Warn, String.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsWarnEnabled) return;

			Log(LoggerLevel.Warn, String.Format(formatProvider, format, args), null);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsWarnEnabled) return;

			Log(LoggerLevel.Warn, String.Format(formatProvider, format, args), exception);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Warn(string format, params Object[] args)
		{
			if (!IsWarnEnabled) return;

			Log(LoggerLevel.Warn, String.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		#endregion

		#region Error

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Error(string message)
		{
			if (!IsErrorEnabled) return;

			Log(LoggerLevel.Error, message, null);
		}

		/// <summary>
		/// Logs an error message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Error(string message, Exception exception)
		{
			if (!IsErrorEnabled) return;

			Log(LoggerLevel.Error, message, exception);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void ErrorFormat(string format, params object[] args)
		{
			if (!IsErrorEnabled) return;

			Log(LoggerLevel.Error, String.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void ErrorFormat(Exception exception, string format, params object[] args)
		{
			if (!IsErrorEnabled) return;

			Log(LoggerLevel.Error, String.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsErrorEnabled) return;

			Log(LoggerLevel.Error, String.Format(formatProvider, format, args), null);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsErrorEnabled) return;

			Log(LoggerLevel.Error, String.Format(formatProvider, format, args), exception);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Error(string format, params Object[] args)
		{
			if (!IsErrorEnabled) return;

			Log(LoggerLevel.Error, String.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		#endregion

		#region Fatal

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Fatal(string message)
		{
			if (!IsFatalEnabled) return;

			Log(LoggerLevel.Fatal, message, null);
		}

		/// <summary>
		/// Logs a fatal message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Fatal(string message, Exception exception)
		{
			if (!IsFatalEnabled) return;

			Log(LoggerLevel.Fatal, message, exception);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void FatalFormat(string format, params object[] args)
		{
			if (!IsFatalEnabled) return;

			Log(LoggerLevel.Fatal, String.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void FatalFormat(Exception exception, string format, params object[] args)
		{
			if (!IsFatalEnabled) return;

			Log(LoggerLevel.Fatal, String.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsFatalEnabled) return;

			Log(LoggerLevel.Fatal, String.Format(formatProvider, format, args), null);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsFatalEnabled) return;

			Log(LoggerLevel.Fatal, String.Format(formatProvider, format, args), exception);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Fatal(string format, params Object[] args)
		{
			if (!IsFatalEnabled) return;

			Log(LoggerLevel.Fatal, String.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		#endregion

		#region FatalError (obsolete)

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="message">The Message</param>
		[Obsolete("Use Fatal instead")]
		public void FatalError(string message)
		{
			if (!IsFatalEnabled) return;

			Log(LoggerLevel.Fatal, message, null);
		}

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		[Obsolete("Use Fatal instead")]
		public void FatalError(string message, Exception exception)
		{
			if (!IsFatalEnabled) return;

			Log(LoggerLevel.Fatal, message, exception);
		}

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		[Obsolete("Use Fatal or FatalFormat instead")]
		public void FatalError(string format, params Object[] args)
		{
			if (!IsFatalEnabled) return;

			Log(LoggerLevel.Fatal, String.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		#endregion

		/// <summary>
		/// Determines if messages of priority "debug" will be logged.
		/// </summary>
		/// <value><c>true</c> if log level flags include the <see cref="LoggerLevel.Debug"/> bit</value> 
		public bool IsDebugEnabled
		{
			get { return (Level >= LoggerLevel.Debug); }
		}

		/// <summary>
		/// Determines if messages of priority "info" will be logged.
		/// </summary>
		/// <value><c>true</c> if log level flags include the <see cref="LoggerLevel.Info"/> bit</value> 
		public bool IsInfoEnabled
		{
			get { return (Level >= LoggerLevel.Info); }
		}

		/// <summary>
		/// Determines if messages of priority "warn" will be logged.
		/// </summary>
		/// <value><c>true</c> if log level flags include the <see cref="LoggerLevel.Warn"/> bit</value> 
		public bool IsWarnEnabled
		{
			get { return (Level >= LoggerLevel.Warn); }
		}

		/// <summary>
		/// Determines if messages of priority "error" will be logged.
		/// </summary>
		/// <value><c>true</c> if log level flags include the <see cref="LoggerLevel.Error"/> bit</value> 
		public bool IsErrorEnabled
		{
			get { return (Level >= LoggerLevel.Error); }
		}

		/// <summary>
		/// Determines if messages of priority "fatal" will be logged.
		/// </summary>
		/// <value><c>true</c> if log level flags include the <see cref="LoggerLevel.Fatal"/> bit</value> 
		public bool IsFatalEnabled
		{
			get { return (Level >= LoggerLevel.Fatal); }
		}

		/// <summary>
		/// Determines if messages of priority "fatal" will be logged.
		/// </summary>
		/// <value><c>true</c> if log level flags include the <see cref="LoggerLevel.Fatal"/> bit</value> 
		[Obsolete("Use IsFatalEnabled instead")]
		public bool IsFatalErrorEnabled
		{
			get { return (Level >= LoggerLevel.Fatal); }
		}

		#endregion

		/// <summary>
		/// Implementors output the log content by implementing this method only.
		/// Note that exception can be null
		/// </summary>
		/// <param name="loggerLevel"></param>
		/// <param name="loggerName"></param>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		protected abstract void Log(LoggerLevel loggerLevel, String loggerName, String message, Exception exception);

		protected void ChangeName(String newName)
		{
			if (newName == null)
			{
				throw new ArgumentNullException("newName");
			}

			name = newName;
		}

		private void Log(LoggerLevel loggerLevel, String message, Exception exception)
		{
			Log(loggerLevel, Name, message, exception);
		}
	}
}
