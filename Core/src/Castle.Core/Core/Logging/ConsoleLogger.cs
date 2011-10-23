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
	using System.Globalization;

	/// <summary>
	///	The Logger sending everything to the standard output streams.
	/// This is mainly for the cases when you have a utility that
	/// does not have a logger to supply.
	/// </summary>
	[Serializable]
	public class ConsoleLogger : LevelFilteredLogger
	{
		/// <summary>
		///   Creates a new ConsoleLogger with the <c>Level</c>
		///   set to <c>LoggerLevel.Debug</c> and the <c>Name</c>
		///   set to <c>String.Empty</c>.
		/// </summary>
		public ConsoleLogger() : this(String.Empty, LoggerLevel.Debug)
		{
		}

		/// <summary>
		///   Creates a new ConsoleLogger with the <c>Name</c>
		///   set to <c>String.Empty</c>.
		/// </summary>
		/// <param name = "logLevel">The logs Level.</param>
		public ConsoleLogger(LoggerLevel logLevel) : this(String.Empty, logLevel)
		{
		}

		/// <summary>
		///   Creates a new ConsoleLogger with the <c>Level</c>
		///   set to <c>LoggerLevel.Debug</c>.
		/// </summary>
		/// <param name = "name">The logs Name.</param>
		public ConsoleLogger(String name) : this(name, LoggerLevel.Debug)
		{
		}

		/// <summary>
		///   Creates a new ConsoleLogger.
		/// </summary>
		/// <param name = "name">The logs Name.</param>
		/// <param name = "logLevel">The logs Level.</param>
		public ConsoleLogger(String name, LoggerLevel logLevel) : base(name, logLevel)
		{
		}

		/// <summary>
		///   A Common method to log.
		/// </summary>
		/// <param name = "loggerLevel">The level of logging</param>
		/// <param name = "loggerName">The name of the logger</param>
		/// <param name = "message">The Message</param>
		/// <param name = "exception">The Exception</param>
		protected override void Log(LoggerLevel loggerLevel, String loggerName, String message, Exception exception)
		{
			Console.Out.WriteLine("[{0}] '{1}' {2}", loggerLevel, loggerName, message);

			if (exception != null)
			{
				Console.Out.WriteLine("[{0}] '{1}' {2}: {3} {4}", loggerLevel, loggerName, exception.GetType().FullName,
				                      exception.Message, exception.StackTrace);
			}
		}

		///<summary>
		///  Returns a new <c>ConsoleLogger</c> with the name
		///  added after this loggers name, with a dot in between.
		///</summary>
		///<param name = "loggerName">The added hierarchical name.</param>
		///<returns>A new <c>ConsoleLogger</c>.</returns>
		public override ILogger CreateChildLogger(string loggerName)
		{
			if (loggerName == null)
			{
				throw new ArgumentNullException("loggerName", "To create a child logger you must supply a non null name");
			}

			return new ConsoleLogger(String.Format(CultureInfo.CurrentCulture, "{0}.{1}", Name, loggerName), Level);
		}
	}
}