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

namespace Castle.Services.Logging.NLogIntegration
{
	using System;
	using Castle.Core.Logging;
	using NLog;

	public class NLogLogger : ILogger
	{
		private Logger _log;
		private NLogFactory _factory;
		private String _loggerName;

		public NLogLogger(Logger log, NLogFactory factory, String loggerName)
		{
			_log = log;
			_factory = factory;
			_loggerName = loggerName;
		}

		public ILogger CreateChildLogger(String name)
		{
			return _factory.Create(_loggerName + "." + name);
		}

		public override string ToString()
		{
			return _log.ToString();
		}

		#region Debug

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Debug(string message)
		{
			if (_log.IsDebugEnabled)
				_log.Debug(message);
		}

		/// <summary>
		/// Logs a debug message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Debug(string message, Exception exception)
		{
			if (_log.IsDebugEnabled)
				_log.DebugException(message, exception);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		[Obsolete("Use DebugFormat instead")]
		public void Debug(string format, params object[] args)
		{
			if (_log.IsDebugEnabled)
				_log.Debug(format, args);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void DebugFormat(string format, params object[] args)
		{
			if (_log.IsDebugEnabled)
				_log.Debug(format, args);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void DebugFormat(Exception exception, string format, params object[] args)
		{
			if (_log.IsDebugEnabled)
				_log.DebugException(String.Format(format, args), exception);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (_log.IsDebugEnabled)
				_log.Debug(formatProvider, format, args);
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
			if (_log.IsDebugEnabled)
				_log.DebugException(String.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Info

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Info(string message)
		{
			if (_log.IsInfoEnabled)
				_log.Info(message);
		}

		/// <summary>
		/// Logs an info message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Info(string message, Exception exception)
		{
			if (_log.IsInfoEnabled)
				_log.InfoException(message, exception);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		[Obsolete("Use InfoFormat instead")]
		public void Info(string format, params object[] args)
		{
			if (_log.IsInfoEnabled)
				_log.Info(format, args);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void InfoFormat(string format, params object[] args)
		{
			if (_log.IsInfoEnabled)
				_log.Info(format, args);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void InfoFormat(Exception exception, string format, params object[] args)
		{
			if (_log.IsInfoEnabled)
				_log.InfoException(String.Format(format, args), exception);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (_log.IsInfoEnabled)
				_log.Info(formatProvider, format, args);
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
			if (_log.IsInfoEnabled)
				_log.InfoException(String.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Warn

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Warn(string message)
		{
			if (_log.IsWarnEnabled)
				_log.Warn(message);
		}

		/// <summary>
		/// Logs a warn message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Warn(string message, Exception exception)
		{
			if (_log.IsWarnEnabled)
				_log.WarnException(message, exception);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		[Obsolete("Use WarnFormat instead")]
		public void Warn(string format, params object[] args)
		{
			if (_log.IsWarnEnabled)
				_log.Warn(format, args);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void WarnFormat(string format, params object[] args)
		{
			if (_log.IsWarnEnabled)
				_log.Warn(format, args);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void WarnFormat(Exception exception, string format, params object[] args)
		{
			if (_log.IsWarnEnabled)
				_log.WarnException(String.Format(format, args), exception);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (_log.IsWarnEnabled)
				_log.Warn(formatProvider, format, args);
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
			if (_log.IsWarnEnabled)
				_log.WarnException(String.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Error

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Error(string message)
		{
			if (_log.IsErrorEnabled)
				_log.Error(message);
		}

		/// <summary>
		/// Logs an error message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Error(string message, Exception exception)
		{
			if (_log.IsErrorEnabled)
				_log.ErrorException(message, exception);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		[Obsolete("Use ErrorFormat instead")]
		public void Error(string format, params object[] args)
		{
			if (_log.IsErrorEnabled)
				_log.Error(format, args);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void ErrorFormat(string format, params object[] args)
		{
			if (_log.IsErrorEnabled)
				_log.Error(format, args);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void ErrorFormat(Exception exception, string format, params object[] args)
		{
			if (_log.IsErrorEnabled)
				_log.ErrorException(String.Format(format, args), exception);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (_log.IsErrorEnabled)
				_log.Error(formatProvider, format, args);
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
			if (_log.IsErrorEnabled)
				_log.ErrorException(String.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Fatal

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Fatal(string message)
		{
			if (_log.IsFatalEnabled)
				_log.Fatal(message);
		}

		/// <summary>
		/// Logs a fatal message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Fatal(string message, Exception exception)
		{
			if (_log.IsFatalEnabled)
				_log.FatalException(message, exception);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		[Obsolete("Use FatalFormat instead")]
		public void Fatal(string format, params object[] args)
		{
			if (_log.IsFatalEnabled)
				_log.Fatal(format, args);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void FatalFormat(string format, params object[] args)
		{
			if (_log.IsFatalEnabled)
				_log.Fatal(format, args);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void FatalFormat(Exception exception, string format, params object[] args)
		{
			if (_log.IsFatalEnabled)
				_log.FatalException(String.Format(format, args), exception);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (_log.IsFatalEnabled)
				_log.Fatal(formatProvider, format, args);
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
			if (_log.IsFatalEnabled)
				_log.FatalException(String.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region FatalError (obsolete)

		[Obsolete("Use FatalFormat instead")]
		public void FatalError(String format, params object[] args)
		{
			if (_log.IsFatalEnabled)
				_log.Fatal(format, args);
		}

		[Obsolete("Use Fatal instead")]
		public void FatalError(String message, Exception exception)
		{
			if (_log.IsFatalEnabled)
				_log.FatalException(message, exception);
		}

		[Obsolete("Use Fatal instead")]
		public void FatalError(String message)
		{
			if (_log.IsFatalEnabled)
				_log.Fatal(message);
		}

		#endregion

		#region Is (...) Enabled

		/// <summary>
		/// Determines if messages of priority "debug" will be logged.
		/// </summary>
		/// <value>True if "debug" messages will be logged.</value> 
		public bool IsDebugEnabled
		{
			get { return _log.IsDebugEnabled; }
		}

		/// <summary>
		/// Determines if messages of priority "info" will be logged.
		/// </summary>
		/// <value>True if "info" messages will be logged.</value> 
		public bool IsInfoEnabled
		{
			get { return _log.IsInfoEnabled; }
		}

		/// <summary>
		/// Determines if messages of priority "warn" will be logged.
		/// </summary>
		/// <value>True if "warn" messages will be logged.</value> 
		public bool IsWarnEnabled
		{
			get { return _log.IsWarnEnabled; }
		}

		/// <summary>
		/// Determines if messages of priority "error" will be logged.
		/// </summary>
		/// <value>True if "error" messages will be logged.</value> 
		public bool IsErrorEnabled
		{
			get { return _log.IsErrorEnabled; }
		}

		/// <summary>
		/// Determines if messages of priority "fatal" will be logged.
		/// </summary>
		/// <value>True if "fatal" messages will be logged.</value> 
		public bool IsFatalEnabled
		{
			get { return _log.IsFatalEnabled; }
		}

		[Obsolete("Use IsFatalEnabled instead")]
		public bool IsFatalErrorEnabled
		{
			get { return _log.IsFatalEnabled; }
		}

		#endregion
	}
}