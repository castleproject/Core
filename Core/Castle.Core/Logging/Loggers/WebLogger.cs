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

namespace Castle.Core.Logging
{
	#if !SILVERLIGHT

	using System;
	using System.Globalization;
	using System.Web;

	/// <summary>
	///	The WebLogger sends everything to the HttpContext.Trace 
	/// </summary>
	/// <remarks>
	/// Trace must be enabled on the Asp.Net configuration file (web.config or machine.config)
	/// </remarks>
	public class WebLogger : LevelFilteredLogger
	{
		private static readonly LoggerLevel DefaultLogLevel = LoggerLevel.Debug;

		/// <summary>
		/// Creates a new WebLogger with the priority set to DEBUG.
		/// </summary>
		public WebLogger() : base(DefaultLogLevel)
		{
		}

		/// <summary>
		/// Creates a new WebLogger.
		/// </summary>
		/// <param name="logLevel">The Log level typecode.</param>
		public WebLogger(LoggerLevel logLevel) : base(logLevel)
		{
		}

		/// <summary>
		/// Creates a new WebLogger.
		/// </summary>
		/// <param name="name">The Log name.</param>
		public WebLogger(String name) : base(name, DefaultLogLevel)
		{
		}

		/// <summary>
		/// Creates a new WebLogger.
		/// </summary>
		/// <param name="name">The Log name.</param>
		/// <param name="loggerLevel">The Log level typecode.</param>
		public WebLogger(String name, LoggerLevel loggerLevel) : base(name, loggerLevel)
		{
		}

		/// <summary>
		/// A Common method to log.
		/// </summary>
		/// <param name="loggerLevel">The level of logging</param>
		/// <param name="loggerName">The Log name.</param>
		/// <param name="message">The Message</param>
		/// <param name="exception">The Exception</param>
		protected override void Log(LoggerLevel loggerLevel, String loggerName, String message, Exception exception)
		{
			TraceContext ctx = TryToGetTraceContext();
			if (ctx == null)
				return;

			if (ctx.IsEnabled)
			{
				String category = String.Format(CultureInfo.CurrentCulture, "[{0}]", loggerLevel.ToString());
				String formattedMessage = String.Format(CultureInfo.CurrentCulture, "{0} {1}", loggerName, message);

				ctx.Write(category, formattedMessage);

				if (exception != null)
				{
					formattedMessage = String.Format(CultureInfo.CurrentCulture, "{0}: {1} {2}Stack Trace: {3}",
					                                 exception.GetType(), exception.Message, Environment.NewLine, exception.StackTrace);

					ctx.Warn(category, formattedMessage);
				}
			}
		}

		/// <summary>
		///	Just returns this logger (<c>WebLogger</c> is not hierarchical).
		/// </summary>
		/// <param name="loggerName">Ignored</param>
		/// <returns>This ILogger instance.</returns> 
		public override ILogger CreateChildLogger(String loggerName)
		{
			if (loggerName == null)
			{
				throw new ArgumentNullException("loggerName", "To create a child logger you must supply a non null name");
			}

			return new WebLogger(String.Format(CultureInfo.CurrentCulture, "{0}.{1}", Name, loggerName), Level);
		}

		/// <summary>
		/// Tries to get the current http context's trace context.
		/// </summary>
		/// <returns>The current http context's trace context or null if none is 
		/// available</returns>
		protected virtual TraceContext TryToGetTraceContext()
		{
			if (null == HttpContext.Current)
				return null;

			return HttpContext.Current.Trace;
		}
	}

	#endif
}