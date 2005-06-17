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

	/// <summary>
	/// Supporting Logger levels.
	/// </summary>
	public enum LoggerLevel
	{
		/// <summary>
		/// Debug logging level
		/// </summary>
		Debug,
		/// <summary>
		/// Info logging level
		/// </summary>
		Info,
		/// <summary>
		/// Warn logging level
		/// </summary>
		Warn,
		/// <summary>
		/// Error logging level
		/// </summary>
		Error,
		/// <summary>
		/// Fatal logging level
		/// </summary>
		Fatal,
		/// <summary>
		/// Logging will be off
		/// </summary>
		Off
	}

	/// <summary>
	/// This is a facade for the different logging subsystems.
	/// It offers a simplified interface that follows IOC patterns
	/// and a simplified priority/level/severity abstraction. 
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The Message</param>
		void Debug( string message );

		/// <summary>
		/// Logs a debug message. 
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		void Debug( string message, Exception exception);

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		void Debug( string format, params Object[] args );

		/// <summary>
		/// Determines if messages of priority "debug" will be logged.
		/// </summary>
		/// <value>True if "debug" messages will be logged.</value> 
		bool IsDebugEnabled 
		{
			get;
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="message">The Message</param>
		void Info( string message );

		/// <summary>
		/// Logs an info message. 
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		void Info( string message, Exception exception);

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		void Info( string format, params Object[] args );

		/// <summary>
		/// Determines if messages of priority "info" will be logged.
		/// </summary>
		/// <value>True if "info" messages will be logged.</value>
		bool IsInfoEnabled
		{
			get;
		}

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="message">The Message</param>
		void Warn( string message );

		/// <summary>
		/// Logs a warn message. 
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		void Warn( string message, Exception exception);

		/// <summary>
		/// Logs a warn message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		void Warn( string format, params Object[] args );

		/// <summary>
		/// Determines if messages of priority "warn" will be logged.
		/// </summary>
		/// <value>True if "warn" messages will be logged.</value>
		bool IsWarnEnabled
		{
			get;
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The Message</param>
		void Error( string message );

		/// <summary>
		/// Logs an error message. 
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		void Error( string message, Exception exception);

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		void Error( string format, params Object[] args );

		/// <summary>
		/// Determines if messages of priority "error" will be logged.
		/// </summary>
		/// <value>True if "error" messages will be logged.</value>
		bool IsErrorEnabled
		{
			get;
		}

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="message">The Message</param>
		void FatalError( string message );

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		void FatalError( string message, Exception exception);

		/// <summary>
		/// Logs a fatal error message.
		/// </summary>
		/// <param name="format">Message format</param>
		/// <param name="args">Array of objects to write using format</param>
		void FatalError( string format, params Object[] args );

		/// <summary>
		/// Determines if messages of priority "fatalError" will be logged.
		/// </summary>
		/// <value>True if "fatalError" messages will be logged.</value>
		bool IsFatalErrorEnabled
		{
			get;
		}
		
		/// <summary>
		/// Create a new child logger.
		/// The name of the child logger is [current-loggers-name].[passed-in-name]
		/// </summary>
		/// <param name="name">The Subname of this logger.</param>
		/// <returns>The New ILogger instance.</returns> 
		/// <exception cref="System.ArgumentException">If the name has an empty element name.</exception>
		ILogger CreateChildLogger( string name );
	}		
}
