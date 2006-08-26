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
	using System.Web;

	/// <summary>
	///	The WebLogger sends everything to the HttpContext.Trace
	/// </summary>
	public class WebLogger : ILogger
	{
		/// <summary>
		/// Default logger level
		/// </summary>
		private LoggerLevel _logLevel = LoggerLevel.Debug;

		/// <summary>
		/// Default name
		/// </summary>
		private String _name = String.Empty;

		/// <summary>
		/// Creates a new WebLogger with the priority set to DEBUG.
		/// </summary>
		public WebLogger() : this(LoggerLevel.Debug)
		{
		}

		/// <summary>
		/// Creates a new WebLogger.
		/// </summary>
		/// <param name="logLevel">The Log level typecode.</param>
		public WebLogger(LoggerLevel logLevel)
		{
			this._logLevel = logLevel;
		}

		/// <summary>
		/// Creates a new WebLogger.
		/// </summary>
		/// <param name="name">The Log name.</param>
		public WebLogger(String name)
		{
			this._name = name;
		}

		/// <summary>
		/// Creates a new WebLogger.
		/// </summary>
		/// <param name="name">The Log name.</param>
		/// <param name="logLevel">The Log level typecode.</param>
		public WebLogger(String name, LoggerLevel logLevel) : this(name)
		{
			this._logLevel = logLevel;
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The Message</param>
		public void Debug(String message)
		{
			Debug(message, null as Exception);
		}

		/// <summary>
		/// Logs a debug message. 
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		public void Debug(String message, Exception exception)
		{
			Log(LoggerLevel.Debug, message, exception);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Debug(String format, params Object[] args)
		{
			Debug(String.Format(format, args));
		}

		/// <summary>
		/// Determines if messages of priority "debug" will be logged.
		/// </summary>
		/// <value>True if "debug" messages will be logged.</value> 
		public bool IsDebugEnabled
		{
			get { return (_logLevel <= LoggerLevel.Debug); }
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="message">The Message</param>
		public void Info(String message)
		{
			Info(message, null as Exception);
		}

		/// <summary>
		/// Logs an info message. 
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		public void Info(String message, Exception exception)
		{
			Log(LoggerLevel.Info, message, exception);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Info(String format, params Object[] args)
		{
			Info(String.Format(format, args));
		}

		/// <summary>
		/// Determines if messages of priority "info" will be logged.
		/// </summary>
		/// <value>True if "info" messages will be logged.</value>
		public bool IsInfoEnabled
		{
			get { return (_logLevel <= LoggerLevel.Info); }
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="message">The Message</param>
		public void Warn(String message)
		{
			Warn(message, null as Exception);
		}

		/// <summary>
		/// Logs a warn message. 
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		public void Warn(String message, Exception exception)
		{
			Log(LoggerLevel.Warn, message, exception);
		}

		/// <summary>
		/// Logs an warn message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Warn(String format, params Object[] args)
		{
			Warn(String.Format(format, args));
		}

		/// <summary>
		/// Determines if messages of priority "warn" will be logged.
		/// </summary>
		/// <value>True if "warn" messages will be logged.</value>
		public bool IsWarnEnabled
		{
			get { return (_logLevel <= LoggerLevel.Warn); }
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The Message</param>
		public void Error(String message)
		{
			Error(message, null as Exception);
		}

		/// <summary>
		/// Logs an error message. 
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		public void Error(String message, Exception exception)
		{
			Log(LoggerLevel.Error, message, exception);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void Error(String format, params Object[] args)
		{
			Error(String.Format(format, args));
		}

		/// <summary>
		/// Determines if messages of priority "error" will be logged.
		/// </summary>
		/// <value>True if "error" messages will be logged.</value>
		public bool IsErrorEnabled
		{
			get { return (_logLevel <= LoggerLevel.Error); }
		}

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="message">The Message</param>
		public void FatalError(String message)
		{
			FatalError(message, null as Exception);
		}

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		public void FatalError(String message, Exception exception)
		{
			Log(LoggerLevel.Fatal, message, exception);
		}

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		public void FatalError(String format, params Object[] args)
		{
			FatalError(String.Format(format, args));
		}

		/// <summary>
		/// Determines if messages of priority "fatalError" will be logged.
		/// </summary>
		/// <value>True if "fatalError" messages will be logged.</value>
		public bool IsFatalErrorEnabled
		{
			get { return (_logLevel <= LoggerLevel.Fatal); }
		}

		/// <summary>
		/// A Common method to log.
		/// </summary>
		/// <param name="level">The level of logging</param>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		protected void Log(LoggerLevel level, String message, Exception exception)
		{
			TraceContext ctx = HttpContext.Current.Trace;

			if (_logLevel <= level && ctx.IsEnabled)
			{
				ctx.Write(String.Format("[{0}]",level.ToString()), String.Format("{0} {1}", _name, message));

				if (exception != null)
				{
					ctx.Warn(String.Format("[{0}]",level.ToString()), String.Format("{0} {1}", exception.Message, exception.StackTrace));
				}
			}
		}

		/// <summary>
		///	Just returns this logger (<c>WebLogger</c> is not hierarchical).
		/// </summary>
		/// <param name="name">Ignored</param>
		/// <returns>This ILogger instance.</returns> 
		public ILogger CreateChildLogger(String name)
		{
			return new WebLogger(String.Format("{0}.{1}", this._name, name), _logLevel);
		}
	}
}