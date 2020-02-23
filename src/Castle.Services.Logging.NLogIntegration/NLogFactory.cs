// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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
	using NLog.Config;

	/// <summary>
	///   Implementation of <see cref="ILoggerFactory" /> for NLog.
	/// </summary>
	public class NLogFactory : AbstractLoggerFactory
	{
		internal const string defaultConfigFileName = "nlog.config";

		/// <summary>
		///   Initializes a new instance of the <see cref="NLogFactory" /> class.
		/// </summary>
		public NLogFactory()
			: this(defaultConfigFileName)
		{
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="NLogFactory" /> class.
		/// </summary>
		/// <param name="configuredExternally">If <c>true</c>. Skips the initialization of log4net assuming it will happen externally. Useful if you're using another framework that wants to take over configuration of NLog.</param>
		public NLogFactory(bool configuredExternally)
		{
			if (configuredExternally)
			{
				return;
			}

			var file = GetConfigFile(defaultConfigFileName);
			LogManager.Configuration = new XmlLoggingConfiguration(file.FullName);
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="NLogFactory" /> class.
		/// </summary>
		/// <param name="configFile"> The config file. </param>
		public NLogFactory(string configFile)
		{
			var file = GetConfigFile(configFile);
			LogManager.Configuration = new XmlLoggingConfiguration(file.FullName);
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="NLogFactory" /> class.
		/// </summary>
		/// <param name="loggingConfiguration"> The NLog Configuration </param>
		public NLogFactory(LoggingConfiguration loggingConfiguration)
		{
			LogManager.Configuration = loggingConfiguration;
		}

		/// <summary>
		///   Creates a logger with specified <paramref name="name" />.
		/// </summary>
		/// <param name="name"> The name. </param>
		public override Core.Logging.ILogger Create(String name)
		{
			var log = LogManager.GetLogger(name);
			return new NLogLogger(log, this);
		}

		/// <summary>
		///   Not implemented, NLog logger levels cannot be set at runtime.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="level"> The level. </param>
		/// <exception cref="NotImplementedException" />
		public override Core.Logging.ILogger Create(String name, LoggerLevel level)
		{
			throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
		}
	}
}