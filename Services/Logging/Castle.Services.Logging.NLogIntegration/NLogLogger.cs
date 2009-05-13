// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
		private Logger logger;
		private NLogFactory factory;

		internal NLogLogger()
		{
		}

		public NLogLogger(Logger logger, NLogFactory factory)
		{
			Logger = logger;
			Factory = factory;
		}

		public virtual ILogger CreateChildLogger(String name)
		{
			return Factory.Create(Logger.Name + "." + name);
		}

		protected internal Logger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		protected internal NLogFactory Factory
		{
			get { return factory; }
			set { factory = value; }
		}

		public override string ToString()
		{
			return Logger.ToString();
		}

		#region Debug

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Debug(string message)
		{
			if (Logger.IsDebugEnabled)
				Logger.Debug(message);
		}

		/// <summary>
		/// Logs a debug message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Debug(string message, Exception exception)
		{
			if (Logger.IsDebugEnabled)
				Logger.DebugException(message, exception);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		[Obsolete("Use DebugFormat instead")]
		public void Debug(string format, params object[] args)
		{
			if (Logger.IsDebugEnabled)
				Logger.Debug(format, args);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void DebugFormat(string format, params object[] args)
		{
			if (Logger.IsDebugEnabled)
				Logger.Debug(format, args);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void DebugFormat(Exception exception, string format, params object[] args)
		{
			if (Logger.IsDebugEnabled)
				Logger.DebugException(String.Format(format, args), exception);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (Logger.IsDebugEnabled)
				Logger.Debug(formatProvider, format, args);
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
			if (Logger.IsDebugEnabled)
				Logger.DebugException(String.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Info

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Info(string message)
		{
			if (Logger.IsInfoEnabled)
				Logger.Info(message);
		}

		/// <summary>
		/// Logs an info message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Info(string message, Exception exception)
		{
			if (Logger.IsInfoEnabled)
				Logger.InfoException(message, exception);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		[Obsolete("Use InfoFormat instead")]
		public void Info(string format, params object[] args)
		{
			if (Logger.IsInfoEnabled)
				Logger.Info(format, args);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void InfoFormat(string format, params object[] args)
		{
			if (Logger.IsInfoEnabled)
				Logger.Info(format, args);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void InfoFormat(Exception exception, string format, params object[] args)
		{
			if (Logger.IsInfoEnabled)
				Logger.InfoException(String.Format(format, args), exception);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (Logger.IsInfoEnabled)
				Logger.Info(formatProvider, format, args);
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
			if (Logger.IsInfoEnabled)
				Logger.InfoException(String.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Warn

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Warn(string message)
		{
			if (Logger.IsWarnEnabled)
				Logger.Warn(message);
		}

		/// <summary>
		/// Logs a warn message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Warn(string message, Exception exception)
		{
			if (Logger.IsWarnEnabled)
				Logger.WarnException(message, exception);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		[Obsolete("Use WarnFormat instead")]
		public void Warn(string format, params object[] args)
		{
			if (Logger.IsWarnEnabled)
				Logger.Warn(format, args);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void WarnFormat(string format, params object[] args)
		{
			if (Logger.IsWarnEnabled)
				Logger.Warn(format, args);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void WarnFormat(Exception exception, string format, params object[] args)
		{
			if (Logger.IsWarnEnabled)
				Logger.WarnException(String.Format(format, args), exception);
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (Logger.IsWarnEnabled)
				Logger.Warn(formatProvider, format, args);
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
			if (Logger.IsWarnEnabled)
				Logger.WarnException(String.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Error

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Error(string message)
		{
			if (Logger.IsErrorEnabled)
				Logger.Error(message);
		}

		/// <summary>
		/// Logs an error message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Error(string message, Exception exception)
		{
			if (Logger.IsErrorEnabled)
				Logger.ErrorException(message, exception);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		[Obsolete("Use ErrorFormat instead")]
		public void Error(string format, params object[] args)
		{
			if (Logger.IsErrorEnabled)
				Logger.Error(format, args);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void ErrorFormat(string format, params object[] args)
		{
			if (Logger.IsErrorEnabled)
				Logger.Error(format, args);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void ErrorFormat(Exception exception, string format, params object[] args)
		{
			if (Logger.IsErrorEnabled)
				Logger.ErrorException(String.Format(format, args), exception);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (Logger.IsErrorEnabled)
				Logger.Error(formatProvider, format, args);
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
			if (Logger.IsErrorEnabled)
				Logger.ErrorException(String.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Fatal

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Fatal(string message)
		{
			if (Logger.IsFatalEnabled)
				Logger.Fatal(message);
		}

		/// <summary>
		/// Logs a fatal message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		public void Fatal(string message, Exception exception)
		{
			if (Logger.IsFatalEnabled)
				Logger.FatalException(message, exception);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		[Obsolete("Use FatalFormat instead")]
		public void Fatal(string format, params object[] args)
		{
			if (Logger.IsFatalEnabled)
				Logger.Fatal(format, args);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void FatalFormat(string format, params object[] args)
		{
			if (Logger.IsFatalEnabled)
				Logger.Fatal(format, args);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void FatalFormat(Exception exception, string format, params object[] args)
		{
			if (Logger.IsFatalEnabled)
				Logger.FatalException(String.Format(format, args), exception);
		}

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (Logger.IsFatalEnabled)
				Logger.Fatal(formatProvider, format, args);
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
			if (Logger.IsFatalEnabled)
				Logger.FatalException(String.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region FatalError (obsolete)

		[Obsolete("Use FatalFormat instead")]
		public void FatalError(String format, params object[] args)
		{
			if (Logger.IsFatalEnabled)
				Logger.Fatal(format, args);
		}

		[Obsolete("Use Fatal instead")]
		public void FatalError(String message, Exception exception)
		{
			if (Logger.IsFatalEnabled)
				Logger.FatalException(message, exception);
		}

		[Obsolete("Use Fatal instead")]
		public void FatalError(String message)
		{
			if (Logger.IsFatalEnabled)
				Logger.Fatal(message);
		}

		#endregion

		#region Is (...) Enabled

		/// <summary>
		/// Determines if messages of priority "debug" will be logged.
		/// </summary>
		/// <value>True if "debug" messages will be logged.</value> 
		public bool IsDebugEnabled
		{
			get { return Logger.IsDebugEnabled; }
		}

		/// <summary>
		/// Determines if messages of priority "info" will be logged.
		/// </summary>
		/// <value>True if "info" messages will be logged.</value> 
		public bool IsInfoEnabled
		{
			get { return Logger.IsInfoEnabled; }
		}

		/// <summary>
		/// Determines if messages of priority "warn" will be logged.
		/// </summary>
		/// <value>True if "warn" messages will be logged.</value> 
		public bool IsWarnEnabled
		{
			get { return Logger.IsWarnEnabled; }
		}

		/// <summary>
		/// Determines if messages of priority "error" will be logged.
		/// </summary>
		/// <value>True if "error" messages will be logged.</value> 
		public bool IsErrorEnabled
		{
			get { return Logger.IsErrorEnabled; }
		}

		/// <summary>
		/// Determines if messages of priority "fatal" will be logged.
		/// </summary>
		/// <value>True if "fatal" messages will be logged.</value> 
		public bool IsFatalEnabled
		{
			get { return Logger.IsFatalEnabled; }
		}

		[Obsolete("Use IsFatalEnabled instead")]
		public bool IsFatalErrorEnabled
		{
			get { return Logger.IsFatalEnabled; }
		}

		#endregion
	}
}
