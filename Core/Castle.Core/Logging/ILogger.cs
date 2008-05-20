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

	/// <summary>
	/// Supporting Logger levels.
	/// </summary>
	public enum LoggerLevel
	{
		/// <summary>
		/// Logging will be off
		/// </summary>
		Off = 0,
		/// <summary>
		/// Fatal logging level
		/// </summary>
		Fatal = 1,
		/// <summary>
		/// Error logging level
		/// </summary>
		Error = 2,
		/// <summary>
		/// Warn logging level
		/// </summary>
		Warn = 3,
		/// <summary>
		/// Info logging level
		/// </summary>
		Info = 4,
		/// <summary>
		/// Debug logging level
		/// </summary>
		Debug = 5,
	}

	/// <summary>
	/// Manages logging.
	/// </summary>
	/// <remarks>
	/// This is a facade for the different logging subsystems.
	/// It offers a simplified interface that follows IOC patterns
	/// and a simplified priority/level/severity abstraction. 
	/// </remarks>
	public interface ILogger
	{
		#region Debug

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The message to log</param>
		void Debug(String message);

		/// <summary>
		/// Logs a debug message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		void Debug(String message, Exception exception);

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void Debug(String format, params Object[] args);

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void DebugFormat(String format, params Object[] args);

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void DebugFormat(Exception exception, String format, params Object[] args);

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void DebugFormat(IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void DebugFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);


		/// <summary>
		/// Determines if messages of priority "debug" will be logged.
		/// </summary>
		/// <value>True if "debug" messages will be logged.</value> 
		bool IsDebugEnabled { get; }

		#endregion

		#region Info

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="message">The message to log</param>
		void Info(String message);

		/// <summary>
		/// Logs an info message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		void Info(String message, Exception exception);

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void Info(String format, params Object[] args);

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void InfoFormat(String format, params Object[] args);

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void InfoFormat(Exception exception, String format, params Object[] args);

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void InfoFormat(IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void InfoFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);


		/// <summary>
		/// Determines if messages of priority "info" will be logged.
		/// </summary>
		/// <value>True if "info" messages will be logged.</value> 
		bool IsInfoEnabled { get; }

		#endregion

		#region Warn

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="message">The message to log</param>
		void Warn(String message);

		/// <summary>
		/// Logs a warn message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		void Warn(String message, Exception exception);

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void Warn(String format, params Object[] args);

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void WarnFormat(String format, params Object[] args);

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void WarnFormat(Exception exception, String format, params Object[] args);

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void WarnFormat(IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void WarnFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);


		/// <summary>
		/// Determines if messages of priority "warn" will be logged.
		/// </summary>
		/// <value>True if "warn" messages will be logged.</value> 
		bool IsWarnEnabled { get; }

		#endregion

		#region Error

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The message to log</param>
		void Error(String message);

		/// <summary>
		/// Logs an error message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		void Error(String message, Exception exception);

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void Error(String format, params Object[] args);

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void ErrorFormat(String format, params Object[] args);

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void ErrorFormat(Exception exception, String format, params Object[] args);

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void ErrorFormat(IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void ErrorFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);


		/// <summary>
		/// Determines if messages of priority "error" will be logged.
		/// </summary>
		/// <value>True if "error" messages will be logged.</value> 
		bool IsErrorEnabled { get; }

		#endregion

		#region Fatal

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="message">The message to log</param>
		void Fatal(String message);

		/// <summary>
		/// Logs a fatal message. 
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="message">The message to log</param>
		void Fatal(String message, Exception exception);

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void Fatal(String format, params Object[] args);

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void FatalFormat(String format, params Object[] args);

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void FatalFormat(Exception exception, String format, params Object[] args);

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void FatalFormat(IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		/// Logs a fatal message.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		/// <param name="formatProvider">The format provider to use</param>
		/// <param name="format">Format string for the message to log</param>
		/// <param name="args">Format arguments for the message to log</param>
		void FatalFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);


		/// <summary>
		/// Determines if messages of priority "fatal" will be logged.
		/// </summary>
		/// <value>True if "fatal" messages will be logged.</value> 
		bool IsFatalEnabled { get; }

		#endregion

		#region FatalError (obsolete)

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="message">The Message</param>
		[Obsolete("Use Fatal instead")]
		void FatalError(String message);

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		[Obsolete("Use Fatal instead")]
		void FatalError(String message, Exception exception);

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		[Obsolete("Use Fatal or FatalFormat instead")]
		void FatalError(String format, params Object[] args);

		/// <summary>
		/// Determines if messages of priority "fatalError" will be logged.
		/// </summary>
		/// <value>True if "fatalError" messages will be logged.</value>
		[Obsolete("Use IsFatalEnabled instead")]
		bool IsFatalErrorEnabled { get; }

		#endregion

		/// <summary>
		/// Create a new child logger.
		/// The name of the child logger is [current-loggers-name].[passed-in-name]
		/// </summary>
		/// <param name="loggerName">The Subname of this logger.</param>
		/// <returns>The New ILogger instance.</returns> 
		/// <exception cref="System.ArgumentException">If the name has an empty element name.</exception>
		ILogger CreateChildLogger(String loggerName);
	}
}