// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

	/// <summary>
	/// The Level Filtered Logger class.  This is a base class which
	/// provides a LogLevel attribute and reroutes all functions into
	/// one Log method.
	/// </summary>
#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public abstract class LevelFilteredLogger : ILogger
	{
		private LoggerLevel level = LoggerLevel.Off;
		private string name = "unnamed";

		/// <summary>
		///   Creates a new <c>LevelFilteredLogger</c>.
		/// </summary>
		protected LevelFilteredLogger()
		{
		}

		protected LevelFilteredLogger(string name)
		{
			ChangeName(name);
		}

		protected LevelFilteredLogger(LoggerLevel loggerLevel)
		{
			level = loggerLevel;
		}

		protected LevelFilteredLogger(string loggerName, LoggerLevel loggerLevel) : this(loggerLevel)
		{
			ChangeName(loggerName);
		}

		public abstract ILogger CreateChildLogger(string loggerName);

		/// <value>
		///   The <c>LoggerLevel</c> that this logger
		///   will be using. Defaults to <c>LoggerLevel.Off</c>
		/// </value>
		public LoggerLevel Level
		{
			get { return level; }
			set { level = value; }
		}

		/// <value>
		///   The name that this logger will be using. 
		///   Defaults to <c>string.Empty</c>
		/// </value>
		public string Name
		{
			get { return name; }
		}

		#region ILogger implementation

		#region Trace

		/// <summary>
		///   Logs a trace message.
		/// </summary>
		/// <param name = "message">The message to log</param>
		public void Trace(string message)
		{
			if (IsTraceEnabled)
			{
				Log(LoggerLevel.Trace, message, null);
			}
		}

		/// <summary>
		///   Logs a trace message.
		/// </summary>
		/// <param name="messageFactory">A functor to create the message</param>
		public void Trace(Func<string> messageFactory)
		{
			if (IsTraceEnabled)
			{
				Log(LoggerLevel.Trace, messageFactory.Invoke(), null);
			}
		}

		/// <summary>
		///   Logs a trace message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "message">The message to log</param>
		public void Trace(string message, Exception exception)
		{
			if (IsTraceEnabled)
			{
				Log(LoggerLevel.Trace, message, exception);
			}
		}

		/// <summary>
		///   Logs a trace message.
		/// </summary>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void TraceFormat(string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				Log(LoggerLevel.Trace, string.Format(CultureInfo.CurrentCulture, format, args), null);
			}
		}

		/// <summary>
		///   Logs a trace message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void TraceFormat(Exception exception, string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				Log(LoggerLevel.Trace, string.Format(CultureInfo.CurrentCulture, format, args), exception);
			}
		}

		/// <summary>
		///   Logs a trace message.
		/// </summary>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				Log(LoggerLevel.Trace, string.Format(formatProvider, format, args), null);
			}
		}

		/// <summary>
		///   Logs a trace message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void TraceFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				Log(LoggerLevel.Trace, string.Format(formatProvider, format, args), exception);
			}
		}

		#endregion

		#region Debug

		/// <summary>
		///   Logs a debug message.
		/// </summary>
		/// <param name = "message">The message to log</param>
		public void Debug(string message)
		{
			if (!IsDebugEnabled)
			{
				return;
			}

			Log(LoggerLevel.Debug, message, null);
		}

		public void Debug(Func<string> messageFactory)
		{
			if (!IsDebugEnabled)
			{
				return;
			}

			Log(LoggerLevel.Debug, messageFactory.Invoke(), null);
		}

		/// <summary>
		///   Logs a debug message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "message">The message to log</param>
		public void Debug(string message, Exception exception)
		{
			if (!IsDebugEnabled)
			{
				return;
			}

			Log(LoggerLevel.Debug, message, exception);
		}

		/// <summary>
		///   Logs a debug message.
		/// </summary>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void DebugFormat(string format, params object[] args)
		{
			if (!IsDebugEnabled)
			{
				return;
			}

			Log(LoggerLevel.Debug, string.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		/// <summary>
		///   Logs a debug message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void DebugFormat(Exception exception, string format, params object[] args)
		{
			if (!IsDebugEnabled)
			{
				return;
			}

			Log(LoggerLevel.Debug, string.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		/// <summary>
		///   Logs a debug message.
		/// </summary>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsDebugEnabled)
			{
				return;
			}

			Log(LoggerLevel.Debug, string.Format(formatProvider, format, args), null);
		}

		/// <summary>
		///   Logs a debug message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsDebugEnabled)
			{
				return;
			}

			Log(LoggerLevel.Debug, string.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Info

		/// <summary>
		///   Logs an info message.
		/// </summary>
		/// <param name = "message">The message to log</param>
		public void Info(string message)
		{
			if (!IsInfoEnabled)
			{
				return;
			}

			Log(LoggerLevel.Info, message, null);
		}

		public void Info(Func<string> messageFactory)
		{
			if (!IsInfoEnabled)
			{
				return;
			}

			Log(LoggerLevel.Info, messageFactory.Invoke(), null);
		}

		/// <summary>
		///   Logs an info message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "message">The message to log</param>
		public void Info(string message, Exception exception)
		{
			if (!IsInfoEnabled)
			{
				return;
			}

			Log(LoggerLevel.Info, message, exception);
		}

		/// <summary>
		///   Logs an info message.
		/// </summary>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void InfoFormat(string format, params object[] args)
		{
			if (!IsInfoEnabled)
			{
				return;
			}

			Log(LoggerLevel.Info, string.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		/// <summary>
		///   Logs an info message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void InfoFormat(Exception exception, string format, params object[] args)
		{
			if (!IsInfoEnabled)
			{
				return;
			}

			Log(LoggerLevel.Info, string.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		/// <summary>
		///   Logs an info message.
		/// </summary>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsInfoEnabled)
			{
				return;
			}

			Log(LoggerLevel.Info, string.Format(formatProvider, format, args), null);
		}

		/// <summary>
		///   Logs an info message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsInfoEnabled)
			{
				return;
			}

			Log(LoggerLevel.Info, string.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Warn

		/// <summary>
		///   Logs a warn message.
		/// </summary>
		/// <param name = "message">The message to log</param>
		public void Warn(string message)
		{
			if (!IsWarnEnabled)
			{
				return;
			}

			Log(LoggerLevel.Warn, message, null);
		}

		public void Warn(Func<string> messageFactory)
		{
			if (!IsWarnEnabled)
			{
				return;
			}

			Log(LoggerLevel.Warn, messageFactory.Invoke(), null);
		}

		/// <summary>
		///   Logs a warn message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "message">The message to log</param>
		public void Warn(string message, Exception exception)
		{
			if (!IsWarnEnabled)
			{
				return;
			}

			Log(LoggerLevel.Warn, message, exception);
		}

		/// <summary>
		///   Logs a warn message.
		/// </summary>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void WarnFormat(string format, params object[] args)
		{
			if (!IsWarnEnabled)
			{
				return;
			}

			Log(LoggerLevel.Warn, string.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		/// <summary>
		///   Logs a warn message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void WarnFormat(Exception exception, string format, params object[] args)
		{
			if (!IsWarnEnabled)
			{
				return;
			}

			Log(LoggerLevel.Warn, string.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		/// <summary>
		///   Logs a warn message.
		/// </summary>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsWarnEnabled)
			{
				return;
			}

			Log(LoggerLevel.Warn, string.Format(formatProvider, format, args), null);
		}

		/// <summary>
		///   Logs a warn message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsWarnEnabled)
			{
				return;
			}

			Log(LoggerLevel.Warn, string.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Error

		/// <summary>
		///   Logs an error message.
		/// </summary>
		/// <param name = "message">The message to log</param>
		public void Error(string message)
		{
			if (!IsErrorEnabled)
			{
				return;
			}

			Log(LoggerLevel.Error, message, null);
		}

		public void Error(Func<string> messageFactory)
		{
			if (!IsErrorEnabled)
			{
				return;
			}

			Log(LoggerLevel.Error, messageFactory.Invoke(), null);
		}

		/// <summary>
		///   Logs an error message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "message">The message to log</param>
		public void Error(string message, Exception exception)
		{
			if (!IsErrorEnabled)
			{
				return;
			}

			Log(LoggerLevel.Error, message, exception);
		}

		/// <summary>
		///   Logs an error message.
		/// </summary>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void ErrorFormat(string format, params object[] args)
		{
			if (!IsErrorEnabled)
			{
				return;
			}

			Log(LoggerLevel.Error, string.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		/// <summary>
		///   Logs an error message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void ErrorFormat(Exception exception, string format, params object[] args)
		{
			if (!IsErrorEnabled)
			{
				return;
			}

			Log(LoggerLevel.Error, string.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		/// <summary>
		///   Logs an error message.
		/// </summary>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsErrorEnabled)
			{
				return;
			}

			Log(LoggerLevel.Error, string.Format(formatProvider, format, args), null);
		}

		/// <summary>
		///   Logs an error message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsErrorEnabled)
			{
				return;
			}

			Log(LoggerLevel.Error, string.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Fatal

		/// <summary>
		///   Logs a fatal message.
		/// </summary>
		/// <param name = "message">The message to log</param>
		public void Fatal(string message)
		{
			if (!IsFatalEnabled)
			{
				return;
			}

			Log(LoggerLevel.Fatal, message, null);
		}

		public void Fatal(Func<string> messageFactory)
		{
			if (!IsFatalEnabled)
			{
				return;
			}

			Log(LoggerLevel.Fatal, messageFactory.Invoke(), null);
		}

		/// <summary>
		///   Logs a fatal message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "message">The message to log</param>
		public void Fatal(string message, Exception exception)
		{
			if (!IsFatalEnabled)
			{
				return;
			}

			Log(LoggerLevel.Fatal, message, exception);
		}

		/// <summary>
		///   Logs a fatal message.
		/// </summary>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void FatalFormat(string format, params object[] args)
		{
			if (!IsFatalEnabled)
			{
				return;
			}

			Log(LoggerLevel.Fatal, string.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		/// <summary>
		///   Logs a fatal message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void FatalFormat(Exception exception, string format, params object[] args)
		{
			if (!IsFatalEnabled)
			{
				return;
			}

			Log(LoggerLevel.Fatal, string.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		/// <summary>
		///   Logs a fatal message.
		/// </summary>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsFatalEnabled)
			{
				return;
			}

			Log(LoggerLevel.Fatal, string.Format(formatProvider, format, args), null);
		}

		/// <summary>
		///   Logs a fatal message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsFatalEnabled)
			{
				return;
			}

			Log(LoggerLevel.Fatal, string.Format(formatProvider, format, args), exception);
		}

		#endregion

		/// <summary>
		///   Determines if messages of priority "trace" will be logged.
		/// </summary>
		/// <value><c>true</c> if log level flags include the <see cref = "LoggerLevel.Trace" /> bit</value>
		public bool IsTraceEnabled
		{
			get { return (Level >= LoggerLevel.Trace); }
		}

		/// <summary>
		///   Determines if messages of priority "debug" will be logged.
		/// </summary>
		/// <value><c>true</c> if log level flags include the <see cref = "LoggerLevel.Debug" /> bit</value>
		public bool IsDebugEnabled
		{
			get { return (Level >= LoggerLevel.Debug); }
		}

		/// <summary>
		///   Determines if messages of priority "info" will be logged.
		/// </summary>
		/// <value><c>true</c> if log level flags include the <see cref = "LoggerLevel.Info" /> bit</value>
		public bool IsInfoEnabled
		{
			get { return (Level >= LoggerLevel.Info); }
		}

		/// <summary>
		///   Determines if messages of priority "warn" will be logged.
		/// </summary>
		/// <value><c>true</c> if log level flags include the <see cref = "LoggerLevel.Warn" /> bit</value>
		public bool IsWarnEnabled
		{
			get { return (Level >= LoggerLevel.Warn); }
		}

		/// <summary>
		///   Determines if messages of priority "error" will be logged.
		/// </summary>
		/// <value><c>true</c> if log level flags include the <see cref = "LoggerLevel.Error" /> bit</value>
		public bool IsErrorEnabled
		{
			get { return (Level >= LoggerLevel.Error); }
		}

		/// <summary>
		///   Determines if messages of priority "fatal" will be logged.
		/// </summary>
		/// <value><c>true</c> if log level flags include the <see cref = "LoggerLevel.Fatal" /> bit</value>
		public bool IsFatalEnabled
		{
			get { return (Level >= LoggerLevel.Fatal); }
		}

		#endregion

		/// <summary>
		///   Implementors output the log content by implementing this method only.
		///   Note that exception can be null
		/// </summary>
		protected abstract void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception);

		protected void ChangeName(string newName)
		{
			if (newName == null)
			{
				throw new ArgumentNullException(nameof(newName));
			}

			name = newName;
		}

		private void Log(LoggerLevel loggerLevel, string message, Exception exception)
		{
			Log(loggerLevel, Name, message, exception);
		}
	}
}