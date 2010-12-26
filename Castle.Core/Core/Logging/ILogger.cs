// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
	///   Manages logging.
	/// </summary>
	/// <remarks>
	///   This is a facade for the different logging subsystems.
	///   It offers a simplified interface that follows IOC patterns
	///   and a simplified priority/level/severity abstraction.
	/// </remarks>
	public interface ILogger
	{
		/// <summary>
		///   Determines if messages of priority "debug" will be logged.
		/// </summary>
		/// <value>True if "debug" messages will be logged.</value>
		bool IsDebugEnabled { get; }

		/// <summary>
		///   Determines if messages of priority "error" will be logged.
		/// </summary>
		/// <value>True if "error" messages will be logged.</value>
		bool IsErrorEnabled { get; }

		/// <summary>
		///   Determines if messages of priority "fatal" will be logged.
		/// </summary>
		/// <value>True if "fatal" messages will be logged.</value>
		bool IsFatalEnabled { get; }

		/// <summary>
		///   Determines if messages of priority "info" will be logged.
		/// </summary>
		/// <value>True if "info" messages will be logged.</value>
		bool IsInfoEnabled { get; }

		/// <summary>
		///   Determines if messages of priority "warn" will be logged.
		/// </summary>
		/// <value>True if "warn" messages will be logged.</value>
		bool IsWarnEnabled { get; }

		/// <summary>
		///   Create a new child logger.
		///   The name of the child logger is [current-loggers-name].[passed-in-name]
		/// </summary>
		/// <param name = "loggerName">The Subname of this logger.</param>
		/// <returns>The New ILogger instance.</returns>
		/// <exception cref = "System.ArgumentException">If the name has an empty element name.</exception>
		ILogger CreateChildLogger(String loggerName);

		/// <summary>
		///   Logs a debug message.
		/// </summary>
		/// <param name = "message">The message to log</param>
		void Debug(String message);

		/// <summary>
		///   Logs a debug message with lazily constructed message. The message will be constructed only if the <see cref = "IsDebugEnabled" /> is true.
		/// </summary>
		/// <param name = "messageFactory"></param>
		void Debug(Func<string> messageFactory);

		/// <summary>
		///   Logs a debug message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "message">The message to log</param>
		void Debug(String message, Exception exception);

		/// <summary>
		///   Logs a debug message.
		/// </summary>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void DebugFormat(String format, params Object[] args);

		/// <summary>
		///   Logs a debug message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void DebugFormat(Exception exception, String format, params Object[] args);

		/// <summary>
		///   Logs a debug message.
		/// </summary>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void DebugFormat(IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		///   Logs a debug message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void DebugFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		///   Logs an error message.
		/// </summary>
		/// <param name = "message">The message to log</param>
		void Error(String message);

		/// <summary>
		///   Logs an error message with lazily constructed message. The message will be constructed only if the <see cref = "IsErrorEnabled" /> is true.
		/// </summary>
		/// <param name = "messageFactory"></param>
		void Error(Func<string> messageFactory);

		/// <summary>
		///   Logs an error message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "message">The message to log</param>
		void Error(String message, Exception exception);

		/// <summary>
		///   Logs an error message.
		/// </summary>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void ErrorFormat(String format, params Object[] args);

		/// <summary>
		///   Logs an error message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void ErrorFormat(Exception exception, String format, params Object[] args);

		/// <summary>
		///   Logs an error message.
		/// </summary>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void ErrorFormat(IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		///   Logs an error message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void ErrorFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		///   Logs a fatal message.
		/// </summary>
		/// <param name = "message">The message to log</param>
		void Fatal(String message);

		/// <summary>
		///   Logs a fatal message with lazily constructed message. The message will be constructed only if the <see cref = "IsFatalEnabled" /> is true.
		/// </summary>
		/// <param name = "messageFactory"></param>
		void Fatal(Func<string> messageFactory);

		/// <summary>
		///   Logs a fatal message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "message">The message to log</param>
		void Fatal(String message, Exception exception);

		/// <summary>
		///   Logs a fatal message.
		/// </summary>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void FatalFormat(String format, params Object[] args);

		/// <summary>
		///   Logs a fatal message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void FatalFormat(Exception exception, String format, params Object[] args);

		/// <summary>
		///   Logs a fatal message.
		/// </summary>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void FatalFormat(IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		///   Logs a fatal message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void FatalFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		///   Logs an info message.
		/// </summary>
		/// <param name = "message">The message to log</param>
		void Info(String message);

		/// <summary>
		///   Logs a info message with lazily constructed message. The message will be constructed only if the <see cref = "IsInfoEnabled" /> is true.
		/// </summary>
		/// <param name = "messageFactory"></param>
		void Info(Func<string> messageFactory);

		/// <summary>
		///   Logs an info message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "message">The message to log</param>
		void Info(String message, Exception exception);

		/// <summary>
		///   Logs an info message.
		/// </summary>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void InfoFormat(String format, params Object[] args);

		/// <summary>
		///   Logs an info message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void InfoFormat(Exception exception, String format, params Object[] args);

		/// <summary>
		///   Logs an info message.
		/// </summary>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void InfoFormat(IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		///   Logs an info message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void InfoFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		///   Logs a warn message.
		/// </summary>
		/// <param name = "message">The message to log</param>
		void Warn(String message);

		/// <summary>
		///   Logs a warn message with lazily constructed message. The message will be constructed only if the <see cref = "IsWarnEnabled" /> is true.
		/// </summary>
		/// <param name = "messageFactory"></param>
		void Warn(Func<string> messageFactory);

		/// <summary>
		///   Logs a warn message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "message">The message to log</param>
		void Warn(String message, Exception exception);

		/// <summary>
		///   Logs a warn message.
		/// </summary>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void WarnFormat(String format, params Object[] args);

		/// <summary>
		///   Logs a warn message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void WarnFormat(Exception exception, String format, params Object[] args);

		/// <summary>
		///   Logs a warn message.
		/// </summary>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void WarnFormat(IFormatProvider formatProvider, String format, params Object[] args);

		/// <summary>
		///   Logs a warn message.
		/// </summary>
		/// <param name = "exception">The exception to log</param>
		/// <param name = "formatProvider">The format provider to use</param>
		/// <param name = "format">Format string for the message to log</param>
		/// <param name = "args">Format arguments for the message to log</param>
		void WarnFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);
	}
}